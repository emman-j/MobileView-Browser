using Microsoft.Data.Sqlite;

namespace MobileView.WV2Service.Data;

internal class LocalHistoryStore
{
    private static long NowMs => DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

    private SqliteConnection Open()
    {
        var conn = new SqliteConnection(BrowserDatabase.ConnectionString);
        conn.Open();
        return conn;
    }

    public async Task RecordVisitAsync(string url, string title, string transition = "link")
    {
        if (string.IsNullOrWhiteSpace(url)) return;

        using var conn = Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = @"
            INSERT INTO urls (url, title, visit_count, typed_count, last_visit_utc)
            VALUES (@url, @title, 1, CASE WHEN @transition = 'typed' THEN 1 ELSE 0 END, @now)
            ON CONFLICT(url) DO UPDATE SET
                visit_count    = visit_count + 1,
                typed_count    = typed_count + CASE WHEN @transition = 'typed' THEN 1 ELSE 0 END,
                last_visit_utc = @now,
                title          = CASE WHEN @title IS NOT NULL AND @title != '' THEN @title ELSE urls.title END;

            INSERT INTO visits (url_id, visit_utc, transition)
            VALUES ((SELECT id FROM urls WHERE url = @url), @now, @transition);
        ";
        cmd.Parameters.AddWithValue("@url", url);
        cmd.Parameters.AddWithValue("@title", (object?)title ?? "");
        cmd.Parameters.AddWithValue("@transition", transition);
        cmd.Parameters.AddWithValue("@now", NowMs);
        await cmd.ExecuteNonQueryAsync();
    }
    public async Task RecordSearchTermAsync(string term)
    {
        if (string.IsNullOrWhiteSpace(term)) return;
        string normalized = term.Trim().ToLowerInvariant();

        using var conn = Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = @"
            INSERT INTO search_terms (term, normalized_term, search_utc, use_count)
            VALUES (@term, @norm, @now, 1)
            ON CONFLICT(normalized_term) DO UPDATE SET
                use_count  = use_count + 1,
                search_utc = @now;
        ";
        cmd.Parameters.AddWithValue("@term", term.Trim());
        cmd.Parameters.AddWithValue("@norm", normalized);
        cmd.Parameters.AddWithValue("@now", NowMs);
        await cmd.ExecuteNonQueryAsync();
    }
    public async Task<List<HistoryEntry>> GetHistoryAsync(int limit = 200)
    {
        using var conn = Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = @"
            SELECT id, url, title, visit_count, last_visit_utc
            FROM urls
            WHERE hidden = 0
            ORDER BY last_visit_utc DESC
            LIMIT @limit;
        ";
        cmd.Parameters.AddWithValue("@limit", limit);
        return await ReadHistoryEntriesAsync(cmd);
    }
    public async Task<List<HistoryEntry>> GetMostVisitedAsync(int limit = 20)
    {
        using var conn = Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT id, url, title, visit_count, last_visit_utc FROM most_visited LIMIT @limit;";
        cmd.Parameters.AddWithValue("@limit", limit);
        return await ReadHistoryEntriesAsync(cmd);
    }
    private static async Task<List<HistoryEntry>> ReadHistoryEntriesAsync(SqliteCommand cmd)
    {
        var results = new List<HistoryEntry>();
        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            long? lastVisitMs = reader.IsDBNull(4) ? null : reader.GetInt64(4);
            results.Add(new HistoryEntry
            {
                Id = reader.GetInt32(0),
                Url = reader.GetString(1),
                Title = reader.IsDBNull(2) ? "" : reader.GetString(2),
                VisitCount = reader.GetInt32(3),
                LastVisited = lastVisitMs is null ? null : DateTimeOffset.FromUnixTimeMilliseconds(lastVisitMs.Value).LocalDateTime
            });
        }
        return results;
    }
    public async Task<List<string>> GetSearchSuggestionsAsync(string prefix, int limit = 10)
    {
        var results = new List<string>();
        if (string.IsNullOrWhiteSpace(prefix)) return results;

        using var conn = Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = @"
            SELECT term FROM search_terms
            WHERE normalized_term LIKE @prefix || '%'
            ORDER BY use_count DESC, search_utc DESC
            LIMIT @limit;
        ";
        cmd.Parameters.AddWithValue("@prefix", prefix.Trim().ToLowerInvariant());
        cmd.Parameters.AddWithValue("@limit", limit);
        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync()) results.Add(reader.GetString(0));
        return results;
    }
    public async Task<List<AddressSuggestion>> GetAddressSuggestionsAsync(string prefix, int limit = 10)
    {
        var results = new List<AddressSuggestion>();
        if (string.IsNullOrWhiteSpace(prefix)) return results;

        using var conn = Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = @"
        SELECT url, title FROM urls
        WHERE hidden = 0
          AND (
                url   LIKE '%' || @prefix || '%'
             OR title LIKE '%' || @prefix || '%'
          )
        ORDER BY visit_count DESC, last_visit_utc DESC
        LIMIT @limit;
    ";
        cmd.Parameters.AddWithValue("@prefix", prefix.Trim());
        cmd.Parameters.AddWithValue("@limit", limit);

        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            results.Add(new AddressSuggestion
            {
                Url = reader.GetString(0),
                Title = reader.IsDBNull(1) ? "" : reader.GetString(1)
            });
        }
        return results;
    }

    // --- Bookmarks ---
    public async Task<int> AddBookmarkAsync(string url, string title, int? folderId = null)
    {
        using var conn = Open();

        using (var upsertUrl = conn.CreateCommand())
        {
            upsertUrl.CommandText = @"
                INSERT INTO urls (url, title, visit_count, last_visit_utc)
                VALUES (@url, @title, 0, NULL)
                ON CONFLICT(url) DO NOTHING;
            ";
            upsertUrl.Parameters.AddWithValue("@url", url);
            upsertUrl.Parameters.AddWithValue("@title", (object?)title ?? "");
            await upsertUrl.ExecuteNonQueryAsync();
        }

        using var insertBookmark = conn.CreateCommand();
        insertBookmark.CommandText = @"
            INSERT INTO bookmarks (url_id, folder_id, title, date_added_utc)
            VALUES ((SELECT id FROM urls WHERE url = @url), @folderId, @title, @now);
            SELECT last_insert_rowid();
        ";
        insertBookmark.Parameters.AddWithValue("@url", url);
        insertBookmark.Parameters.AddWithValue("@folderId", (object?)folderId ?? 2); // default: "Other Bookmarks"
        insertBookmark.Parameters.AddWithValue("@title", (object?)title ?? "");
        insertBookmark.Parameters.AddWithValue("@now", NowMs);
        var id = await insertBookmark.ExecuteScalarAsync();
        return Convert.ToInt32(id);
    }
    public async Task RemoveBookmarkAsync(int bookmarkId)
    {
        using var conn = Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "DELETE FROM bookmarks WHERE id = @id;";
        cmd.Parameters.AddWithValue("@id", bookmarkId);
        await cmd.ExecuteNonQueryAsync();
    }
    public async Task<List<BookmarkFolder>> GetBookmarkFoldersAsync()
    {
        var results = new List<BookmarkFolder>();
        using var conn = Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT id, name, parent_id FROM bookmark_folders ORDER BY sort_order;";
        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            results.Add(new BookmarkFolder
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1),
                ParentId = reader.IsDBNull(2) ? null : reader.GetInt32(2)
            });
        }
        return results;
    }
    public async Task<int> AddBookmarkFolderAsync(string name, int? parentId = null)
    {
        using var conn = Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = @"
            INSERT INTO bookmark_folders (name, parent_id) VALUES (@name, @parentId);
            SELECT last_insert_rowid();
        ";
        cmd.Parameters.AddWithValue("@name", name);
        cmd.Parameters.AddWithValue("@parentId", (object?)parentId ?? DBNull.Value);
        var id = await cmd.ExecuteScalarAsync();
        return Convert.ToInt32(id);
    }
    public async Task<List<BookmarkEntry>> GetBookmarksAsync()
    {
        var results = new List<BookmarkEntry>();
        using var conn = Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = @"
            WITH RECURSIVE folder_path(id, path) AS (
                SELECT id, name FROM bookmark_folders WHERE parent_id IS NULL
                UNION ALL
                SELECT f.id, fp.path || ' / ' || f.name
                FROM bookmark_folders f JOIN folder_path fp ON f.parent_id = fp.id
            )
            SELECT b.id, u.url, b.title, b.folder_id, COALESCE(fp.path, '')
            FROM bookmarks b
            JOIN urls u ON u.id = b.url_id
            LEFT JOIN folder_path fp ON fp.id = b.folder_id
            ORDER BY fp.path, b.sort_order;
        ";
        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            results.Add(new BookmarkEntry
            {
                Id = reader.GetInt32(0),
                Url = reader.GetString(1),
                Title = reader.IsDBNull(2) ? "" : reader.GetString(2),
                FolderId = reader.IsDBNull(3) ? null : reader.GetInt32(3),
                FolderPath = reader.GetString(4)
            });
        }
        return results;
    }
    public async Task UpdateBookmarkAsync(int bookmarkId, string title, int? folderId)
    {
        using var conn = Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = @"
        UPDATE bookmarks
        SET title = @title, folder_id = @folderId
        WHERE id = @id;
    ";
        cmd.Parameters.AddWithValue("@title", title);
        cmd.Parameters.AddWithValue("@folderId", (object?)folderId ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@id", bookmarkId);
        await cmd.ExecuteNonQueryAsync();
    }
    public async Task ClearHistoryAsync()
    {
        using var conn = Open();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = @"
            DELETE FROM visits;
            UPDATE urls SET visit_count = 0, typed_count = 0, last_visit_utc = NULL;
        ";
        await cmd.ExecuteNonQueryAsync(); // bookmarked urls survive since only stats reset, FK intact
    }
}
