using Microsoft.Data.Sqlite;
namespace MobileView.WV2Service.Data;
public class ChromeImportResult
{
    public int UrlsImported { get; set; }
    public int VisitsImported { get; set; }
    public int SearchTermsImported { get; set; }
    public bool Success { get; set; }
    public string? Error { get; set; }
}

internal static class ChromeHistoryImporter
{
    // Chrome/Edge timestamps are microseconds since 1601-01-01 (Windows FILETIME epoch)
    private const long ChromeEpochOffsetUs = 11644473600000000L;

    private static long? ChromeToUnixMs(long? chromeUs)
    {
        if (chromeUs is null or 0) return null;
        return (chromeUs.Value - ChromeEpochOffsetUs) / 1000;
    }

    private static readonly Dictionary<int, string> TransitionNames = new()
    {
        { 0, "link" }, { 1, "typed" }, { 2, "auto_bookmark" }, { 3, "auto_subframe" },
        { 4, "manual_subframe" }, { 5, "generated" }, { 6, "start_page" },
        { 7, "form_submit" }, { 8, "reload" }, { 9, "keyword" }, { 10, "keyword_generated" }
    };

    /// <summary>
    /// Imports urls, visits, and search terms from a Chrome/Edge "History" sqlite file
    /// into our normalized browser.db. Safe to call multiple times — merges by URL.
    /// </summary>
    public static async Task<ChromeImportResult> ImportAsync(string chromeHistoryPath)
    {
        var result = new ChromeImportResult();

        if (!File.Exists(chromeHistoryPath))
        {
            result.Error = "Chrome History file not found.";
            return result;
        }

        // Chrome locks the live file while running — copy it first so we can read it regardless.
        string tempCopyPath = Path.Combine(Path.GetTempPath(), $"chrome_history_import_{Guid.NewGuid():N}.db");

        try
        {
            File.Copy(chromeHistoryPath, tempCopyPath, overwrite: true);

            using var source = new SqliteConnection($"Data Source={tempCopyPath};Mode=ReadOnly");
            await source.OpenAsync();

            using var dest = new SqliteConnection(BrowserDatabase.ConnectionString);
            await dest.OpenAsync();
            using var tx = dest.BeginTransaction();

            // Maps Chrome's internal url id -> our new url id, so visits/search_terms link correctly
            var urlIdMap = new Dictionary<long, long>();

            result.UrlsImported = await ImportUrlsAsync(source, dest, tx, urlIdMap);
            result.VisitsImported = await ImportVisitsAsync(source, dest, tx, urlIdMap);
            result.SearchTermsImported = await ImportSearchTermsAsync(source, dest, tx, urlIdMap);

            tx.Commit();
            result.Success = true;
        }
        catch (Exception ex)
        {
            result.Error = ex.Message;
        }
        finally
        {
            try { if (File.Exists(tempCopyPath)) File.Delete(tempCopyPath); } catch { /* best effort */ }
        }

        return result;
    }

    private static async Task<int> ImportUrlsAsync(
        SqliteConnection source, SqliteConnection dest, SqliteTransaction tx,
        Dictionary<long, long> urlIdMap)
    {
        int count = 0;

        using var selectCmd = source.CreateCommand();
        selectCmd.CommandText = "SELECT id, url, title, visit_count, typed_count, last_visit_time, hidden FROM urls;";
        using var reader = await selectCmd.ExecuteReaderAsync();

        var rows = new List<(long chromeId, string url, string title, int visitCount, int typedCount, long? lastVisit, bool hidden)>();
        while (await reader.ReadAsync())
        {
            rows.Add((
                reader.GetInt64(0),
                reader.GetString(1),
                reader.IsDBNull(2) ? "" : reader.GetString(2),
                reader.GetInt32(3),
                reader.GetInt32(4),
                reader.IsDBNull(5) ? null : reader.GetInt64(5),
                reader.GetInt32(6) != 0
            ));
        }

        foreach (var row in rows)
        {
            long? lastVisitMs = ChromeToUnixMs(row.lastVisit);

            using var upsert = dest.CreateCommand();
            upsert.Transaction = tx;
            upsert.CommandText = @"
                INSERT INTO urls (url, title, visit_count, typed_count, last_visit_utc, hidden)
                VALUES (@url, @title, @visitCount, @typedCount, @lastVisit, @hidden)
                ON CONFLICT(url) DO UPDATE SET
                    visit_count    = visit_count + excluded.visit_count,
                    typed_count    = typed_count + excluded.typed_count,
                    last_visit_utc = MAX(COALESCE(last_visit_utc, 0), COALESCE(excluded.last_visit_utc, 0)),
                    title          = CASE WHEN excluded.title != '' THEN excluded.title ELSE urls.title END;
                SELECT id FROM urls WHERE url = @url;
            ";
            upsert.Parameters.AddWithValue("@url", row.url);
            upsert.Parameters.AddWithValue("@title", row.title);
            upsert.Parameters.AddWithValue("@visitCount", row.visitCount);
            upsert.Parameters.AddWithValue("@typedCount", row.typedCount);
            upsert.Parameters.AddWithValue("@lastVisit", (object?)lastVisitMs ?? DBNull.Value);
            upsert.Parameters.AddWithValue("@hidden", row.hidden ? 1 : 0);

            var newId = await upsert.ExecuteScalarAsync();
            urlIdMap[row.chromeId] = Convert.ToInt64(newId);
            count++;
        }

        return count;
    }

    private static async Task<int> ImportVisitsAsync(
        SqliteConnection source, SqliteConnection dest, SqliteTransaction tx,
        Dictionary<long, long> urlIdMap)
    {
        int count = 0;

        using var selectCmd = source.CreateCommand();
        selectCmd.CommandText = "SELECT url, visit_time, visit_duration, transition FROM visits;";
        using var reader = await selectCmd.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            long chromeUrlId = reader.GetInt64(0);
            if (!urlIdMap.TryGetValue(chromeUrlId, out long ourUrlId)) continue; // orphaned visit, skip

            long? visitTime = reader.IsDBNull(1) ? null : reader.GetInt64(1);
            long durationRaw = reader.IsDBNull(2) ? 0 : reader.GetInt64(2);
            int transitionRaw = reader.IsDBNull(3) ? 0 : reader.GetInt32(3);

            long? visitMs = ChromeToUnixMs(visitTime);
            if (visitMs is null) continue;

            int core = transitionRaw & 0xFF;
            string transitionName = TransitionNames.TryGetValue(core, out var name) ? name : $"other_{core}";

            using var insert = dest.CreateCommand();
            insert.Transaction = tx;
            insert.CommandText = @"
                INSERT INTO visits (url_id, visit_utc, visit_duration_ms, transition)
                VALUES (@urlId, @visitUtc, @durationMs, @transition);
            ";
            insert.Parameters.AddWithValue("@urlId", ourUrlId);
            insert.Parameters.AddWithValue("@visitUtc", visitMs.Value);
            insert.Parameters.AddWithValue("@durationMs", durationRaw / 1000);
            insert.Parameters.AddWithValue("@transition", transitionName);
            await insert.ExecuteNonQueryAsync();
            count++;
        }

        return count;
    }

    private static async Task<int> ImportSearchTermsAsync(
        SqliteConnection source, SqliteConnection dest, SqliteTransaction tx,
        Dictionary<long, long> urlIdMap)
    {
        int count = 0;

        // keyword_search_terms doesn't exist on every profile — treat that as "nothing to import"
        using (var checkCmd = source.CreateCommand())
        {
            checkCmd.CommandText = "SELECT name FROM sqlite_master WHERE type='table' AND name='keyword_search_terms';";
            var exists = await checkCmd.ExecuteScalarAsync();
            if (exists is null) return 0;
        }

        using var selectCmd = source.CreateCommand();
        selectCmd.CommandText = "SELECT url_id, term, normalized_term FROM keyword_search_terms;";
        using var reader = await selectCmd.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            long chromeUrlId = reader.GetInt64(0);
            string term = reader.GetString(1);
            string normalized = reader.GetString(2);
            urlIdMap.TryGetValue(chromeUrlId, out long ourUrlId); // 0 if not found — that's fine, url_id is nullable

            using var upsert = dest.CreateCommand();
            upsert.Transaction = tx;
            upsert.CommandText = @"
                INSERT INTO search_terms (url_id, term, normalized_term, search_utc, use_count)
                VALUES (@urlId, @term, @norm, @now, 1)
                ON CONFLICT(normalized_term) DO UPDATE SET
                    use_count = use_count + 1;
            ";
            upsert.Parameters.AddWithValue("@urlId", ourUrlId == 0 ? DBNull.Value : ourUrlId);
            upsert.Parameters.AddWithValue("@term", term);
            upsert.Parameters.AddWithValue("@norm", normalized);
            upsert.Parameters.AddWithValue("@now", DateTimeOffset.UtcNow.ToUnixTimeMilliseconds());
            await upsert.ExecuteNonQueryAsync();
            count++;
        }

        return count;
    }
}
