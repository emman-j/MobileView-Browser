using Microsoft.Web.WebView2.Core;
using MobileView.WV2Service.Data;
using MobileView.WV2Service.Enums;
namespace MobileView.WV2Service;

public class BrowsingDataManager(IWV2Service webviewService)
{
    private readonly LocalHistoryStore _localStore = new();
    public Task<ChromeImportResult> ImportChromeHistory(string chromeHistoryPath)
    => ChromeHistoryImporter.ImportAsync(chromeHistoryPath);

    public Task AllBrowsingData() => webviewService.ClearAllBrowsingData();
    public Task BrowsingDataBetweenDates(DateTime startDate, DateTime endDate) => webviewService.ClearBrowsingDataBetweenDateRange(startDate, endDate);
    public Task BrowserData(BrowsingDataKinds dataKind)
    {
        CoreWebView2BrowsingDataKinds _dataKind = EnumMapper.BrowsingDataKindMap[dataKind];
        return webviewService.ClearBrowserData(_dataKind);
    }
    public Task AllBrowserData() => webviewService.ClearAllBrowsingData();

    public Task RecordVisit(string url, string title, string transition = "link") => _localStore.RecordVisitAsync(url, title, transition);
    public Task RecordSearchTerm(string term) => _localStore.RecordSearchTermAsync(term);
    public Task<List<HistoryEntry>> GetHistory(int limit = 200) => _localStore.GetHistoryAsync(limit);
    public Task<List<HistoryEntry>> GetFavorites(int limit = 20) => _localStore.GetMostVisitedAsync(limit);
    public Task<List<string>> GetSearchSuggestions(string prefix, int limit = 10) => _localStore.GetSearchSuggestionsAsync(prefix, limit);
    public Task<List<AddressSuggestion>> GetAddressSuggestions(string prefix, int limit = 10) => _localStore.GetAddressSuggestionsAsync(prefix, limit);
    public Task<int> AddBookmark(string url, string title, int? folderId = null) => _localStore.AddBookmarkAsync(url, title, folderId);
    public Task RemoveBookmark(int bookmarkId) => _localStore.RemoveBookmarkAsync(bookmarkId);
    public Task<List<BookmarkEntry>> GetBookmarks() => _localStore.GetBookmarksAsync();
    public Task<List<BookmarkFolder>> GetBookmarkFolders() => _localStore.GetBookmarkFoldersAsync();
    public Task<int> AddBookmarkFolder(string name, int? parentId = null) => _localStore.AddBookmarkFolderAsync(name, parentId);
    public Task UpdateBookmark(int bookmarkId, string title, int? folderId = null) => _localStore.UpdateBookmarkAsync(bookmarkId, title, folderId);
    public Task ClearLocalHistory() => _localStore.ClearHistoryAsync();
}
