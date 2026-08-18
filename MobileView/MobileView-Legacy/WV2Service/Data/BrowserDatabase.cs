using Microsoft.Data.Sqlite;
namespace MobileView.WV2Service.Data;
internal static class BrowserDatabase
{
    public static string DbPath { get; set; } = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "MobileView", "browser.db");

    public static string ConnectionString => $"Data Source={DbPath}";

    public static void EnsureCreated()
    {
        Directory.CreateDirectory(Path.GetDirectoryName(DbPath)!);

        using var connection = new SqliteConnection(ConnectionString);
        connection.Open();

        using var cmd = connection.CreateCommand();
        cmd.CommandText = @"
            PRAGMA foreign_keys = ON;

            CREATE TABLE IF NOT EXISTS urls (
                id              INTEGER PRIMARY KEY,
                url             TEXT NOT NULL UNIQUE,
                title           TEXT,
                visit_count     INTEGER NOT NULL DEFAULT 0,
                typed_count     INTEGER NOT NULL DEFAULT 0,
                last_visit_utc  INTEGER,
                hidden          INTEGER NOT NULL DEFAULT 0
            );
            CREATE INDEX IF NOT EXISTS idx_urls_visit_count ON urls(visit_count DESC);
            CREATE INDEX IF NOT EXISTS idx_urls_last_visit  ON urls(last_visit_utc DESC);

            CREATE TABLE IF NOT EXISTS visits (
                id                INTEGER PRIMARY KEY,
                url_id            INTEGER NOT NULL REFERENCES urls(id) ON DELETE CASCADE,
                visit_utc         INTEGER NOT NULL,
                visit_duration_ms INTEGER NOT NULL DEFAULT 0,
                transition        TEXT
            );
            CREATE INDEX IF NOT EXISTS idx_visits_url  ON visits(url_id);
            CREATE INDEX IF NOT EXISTS idx_visits_time ON visits(visit_utc DESC);

            CREATE TABLE IF NOT EXISTS search_terms (
                id               INTEGER PRIMARY KEY,
                url_id           INTEGER REFERENCES urls(id) ON DELETE SET NULL,
                term             TEXT NOT NULL,
                normalized_term  TEXT NOT NULL,
                search_utc       INTEGER,
                use_count        INTEGER NOT NULL DEFAULT 1
            );
            CREATE UNIQUE INDEX IF NOT EXISTS idx_search_unique_term ON search_terms(normalized_term);

            CREATE TABLE IF NOT EXISTS bookmark_folders (
                id          INTEGER PRIMARY KEY,
                name        TEXT NOT NULL,
                parent_id   INTEGER REFERENCES bookmark_folders(id) ON DELETE CASCADE,
                sort_order  INTEGER NOT NULL DEFAULT 0
            );
            CREATE INDEX IF NOT EXISTS idx_folders_parent ON bookmark_folders(parent_id);

            CREATE TABLE IF NOT EXISTS bookmarks (
                id             INTEGER PRIMARY KEY,
                url_id         INTEGER NOT NULL REFERENCES urls(id) ON DELETE CASCADE,
                folder_id      INTEGER REFERENCES bookmark_folders(id) ON DELETE SET NULL,
                title          TEXT,
                date_added_utc INTEGER,
                sort_order     INTEGER NOT NULL DEFAULT 0
            );
            CREATE INDEX IF NOT EXISTS idx_bookmarks_folder ON bookmarks(folder_id);
            CREATE INDEX IF NOT EXISTS idx_bookmarks_url    ON bookmarks(url_id);

            CREATE VIEW IF NOT EXISTS most_visited AS
            SELECT id, url, title, visit_count, typed_count, last_visit_utc
            FROM urls WHERE hidden = 0
            ORDER BY visit_count DESC;
        ";
        cmd.ExecuteNonQuery();

        using var seed = connection.CreateCommand();
        seed.CommandText = @"
            INSERT OR IGNORE INTO bookmark_folders (id, name, parent_id, sort_order) VALUES (1, 'Bookmarks Bar', NULL, 0);
            INSERT OR IGNORE INTO bookmark_folders (id, name, parent_id, sort_order) VALUES (2, 'Other Bookmarks', NULL, 1);
        ";
        seed.ExecuteNonQuery();
    }
}