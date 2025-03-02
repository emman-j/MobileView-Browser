using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;
using Microsoft.Data.Sqlite;
using System.Reflection;
using System.Data;

namespace WV2Service
{
    public partial class WebViewService
    {

        // Normalize the root URLs (e.g., https://www.google.com -> https://google.com)
        private string GetRootUrl(string url)
        {
            try
            {
                var uri = new Uri(url);
                string rootUrl = uri.Scheme + "://" + uri.Host.ToLower(); // Normalize by making the URL lowercase

                // Remove "www" if present
                if (rootUrl.StartsWith("https://www."))
                {
                    rootUrl = "https://" + rootUrl.Substring(12); // Remove "www."
                }
                else if (rootUrl.StartsWith("http://www."))
                {
                    rootUrl = "http://" + rootUrl.Substring(11); // Remove "www."
                }

                return rootUrl;
            }
            catch
            {
                return url; // Return the original URL if parsing fails
            }
        }

        // Data readers
        private DataTable GetDataTable(SqliteCommand command)
        {
            try
            {
                DataTable dataTable = new DataTable();
                using (SqliteCommand cmd = command)
                {
                    using (SqliteDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            dataTable.Load(reader);
                        }
                    }
                }
                command.Dispose();
                return dataTable;
            }
            catch (Exception ex)
            {

            }
            command?.Dispose();
            return null;
        }
        private async Task<DataTable> GetHistoryDataTableAsync(SqliteCommand command)
        {
            try
            {
                DataTable dataTable = new DataTable();
                dataTable.Columns.Add("ID", typeof(int));
                dataTable.Columns.Add("URL", typeof(string));
                dataTable.Columns.Add("Title", typeof(string));
                dataTable.Columns.Add("Visit Count", typeof(int));
                dataTable.Columns.Add("Last Visited", typeof(DateTime));

                using (SqliteDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        int id = reader.GetInt32(0);
                        string url = reader.GetString(1);
                        string title = reader.IsDBNull(2) ? string.Empty : reader.GetString(2);
                        int visitCount = reader.GetInt32(3);
                        long lastVisitTime = reader.GetInt64(4);
                        DateTime lastVisitDate = DateTimeOffset.FromUnixTimeMilliseconds(lastVisitTime / 1000).DateTime; // Convert Chromium timestamp to DateTime
                        DateTime lastVisitLocal = lastVisitDate.ToLocalTime();                                           // Convert to local time (based on system's time zone)

                        dataTable.Rows.Add(id, url, title, visitCount, lastVisitLocal);
                    }
                }
                command.Dispose();
                return dataTable;
            }
            catch (SqliteException ex)
            {
                throw new SqliteException(ex.Message, ex.SqliteErrorCode);
            }
        }
        private async Task<DataTable> GetFavoritesDataTableAsync(SqliteCommand command)
        {
            try
            {
                DataTable dataTable = new DataTable();
                dataTable.Columns.Add("ID", typeof(int));
                dataTable.Columns.Add("URL", typeof(string));
                dataTable.Columns.Add("Title", typeof(string));
                dataTable.Columns.Add("Visit Count", typeof(int));
                dataTable.Columns.Add("Last Visited", typeof(DateTime));

                using (SqliteDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        int id = reader.GetInt32(0);
                        string url = reader.GetString(1);
                        string title = reader.IsDBNull(2) ? string.Empty : reader.GetString(2);
                        int visitCount = reader.GetInt32(3);
                        long lastVisitTime = reader.GetInt64(4);
                        DateTime lastVisitDate = DateTimeOffset.FromUnixTimeMilliseconds(lastVisitTime / 1000).DateTime;
                        DateTime lastVisitLocal = lastVisitDate.ToLocalTime();

                        dataTable.Rows.Add(id, url, title, visitCount, lastVisitLocal);
                    }
                }
                command.Dispose();

                var uniqueRoots = dataTable.AsEnumerable()
                    .Select(row => new
                    {
                        RootUrl = GetRootUrl(row[1].ToString()), // Extract and normalize root URL
                        Row = row
                    })
                    .GroupBy(x => x.RootUrl) // Group by root URL
                    .Select(g => g.First())  // Take the first occurrence for each unique root URL
                    .ToList();

                // Clone the original DataTable (schema only)
                DataTable updatedDataTable = dataTable.Clone();

                foreach (var item in uniqueRoots)
                {
                    DataRow newRow = updatedDataTable.NewRow();
                    newRow.ItemArray = item.Row.ItemArray; // Copy values from the original row
                    updatedDataTable.Rows.Add(newRow);
                }

                return updatedDataTable;
            }
            catch (SqliteException ex)
            {
                throw new SqliteException(ex.Message, ex.SqliteErrorCode);
            }
        }

        // SQL command builders
        private SqliteCommand SqliteCmd_GetHistory(SqliteConnection conn)
        {
            try
            {
                string query = @"
                    SELECT id, url, title, visit_count, last_visit_time 
                    FROM urls 
                    ORDER BY last_visit_time DESC";

                SqliteCommand command = new(query, conn);
                return command;
            }
            catch (SqliteException ex)
            {
                throw new SqliteException(ex.Message, ex.SqliteErrorCode);
            }
        }
        private SqliteCommand SqliteCmd_GetFavorites(SqliteConnection conn)
        {
            try
            {
                string query = @"
                    SELECT id, url, title, visit_count, last_visit_time 
                    FROM urls 
                    WHERE visit_count >= @VisitCount
                    ORDER BY last_visit_time DESC";

                SqliteCommand command = new(query, conn);
                command.Parameters.AddWithValue("@VisitCount", 5); // Parameterized to allow customizability
                return command;
            }
            catch (SqliteException ex)
            {
                throw new SqliteException(ex.Message, ex.SqliteErrorCode);
            }
        }
        private void EnsureTempDirectory()
        {
            string tempPath = Path.Combine(Path.GetTempPath(), "EBWebView");
            if (!Directory.Exists(tempPath))
            {
                Directory.CreateDirectory(tempPath);            }
        }
        private async Task<DataTable> GetHistory(string profileDirectory)
        {
            //string historyFilePath = Path.Combine(profileDirectory, "EBWebView", "Default", "History");

            if (!File.Exists(profileDirectory))
            {
                Console.WriteLine("History file does not exist.");
                return null;
            }
            EnsureTempDirectory();   
            string tempFilePath = Path.Combine(Path.GetTempPath(), "EBWebView", "database_temp.db");   // dir to move the locked database to, as a temporary file
            try
            {
                File.Copy(profileDirectory, tempFilePath, overwrite: true); // this will be readonly. No modifications will be done directly on the db file

                SqliteConnection conn;
                DataTable dataTable;
                using (conn = new SqliteConnection($"Data Source={tempFilePath};"))
                {
                    conn.Open();
                    SqliteCommand command = SqliteCmd_GetHistory(conn);
                    dataTable = await GetHistoryDataTableAsync(command);
                }
                return dataTable;
            }
            catch (SqliteException ex)
            {
                throw new SqliteException(ex.Message, ex.SqliteErrorCode);
            }
        }
        private async Task<DataTable> GetFavorites(string profileDirectory)
        {
            if (!File.Exists(profileDirectory))
            {
                Console.WriteLine("History file does not exist.");
                return null;
            }
            EnsureTempDirectory();
            string tempFilePath = Path.Combine(Path.GetTempPath(), "EBWebView", "database_temp.db");
            try
            {
                File.Copy(profileDirectory, tempFilePath, overwrite: true);

                SqliteConnection conn;
                DataTable dataTable;
                using (conn = new SqliteConnection($"Data Source={tempFilePath};"))
                {
                    conn.Open();
                    SqliteCommand command = SqliteCmd_GetFavorites(conn);
                    dataTable = await GetFavoritesDataTableAsync(command);
                }
                return dataTable;
            }
            catch (SqliteException ex)
            {
                if (ex.Message.Contains("no such table")) // to ignore initial start-up error when url table has not been created yet
                {
                    Console.WriteLine($"Warning: {ex.Message}");
                    return null; // Return an empty DataTable
                }

                throw new SqliteException(ex.Message, ex.SqliteErrorCode);
            }
        }
        public async Task<DataTable> GetHistory()
        {
            string historyFilePath = (!string.IsNullOrWhiteSpace(_TempFolder)) ?
                Path.Combine(  _TempFolder, "EBWebView", "Default", "History") :
                Path.Combine(ProfileFolder, "EBWebView", "Default", "History");
            
            return await GetHistory(historyFilePath);
        }
        public async Task<DataTable> GetFavorites()
        {
            string favoritesFilePath = (!string.IsNullOrWhiteSpace(_TempFolder)) ?
                Path.Combine(_TempFolder, "EBWebView", "Default", "History") :
                Path.Combine(ProfileFolder, "EBWebView", "Default", "History");

            return await GetFavorites(favoritesFilePath);
        }
        public async Task<Dictionary<string, string>> GetFavoritesDict()
        {
            string favoritesFilePath = (!string.IsNullOrWhiteSpace(_TempFolder)) ?
                Path.Combine(_TempFolder, "EBWebView", "Default", "History") :
                Path.Combine(ProfileFolder, "EBWebView", "Default", "History");

            DataTable data = await GetFavorites(favoritesFilePath);

            if (data == null ) { return null; }

            Dictionary<string, string> favoritesDict = new Dictionary<string, string>();
            foreach (DataRow row in data.Rows)
            {
                string title = row.Field<string>("Title");
                string url = row.Field<string>("URL");

                if (!favoritesDict.ContainsKey(title))
                {
                    favoritesDict.Add(title, url);
                }
            }
            return favoritesDict;
        }
    }
}
