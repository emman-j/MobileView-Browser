using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;
using System.Diagnostics;

namespace MobileView.WV2Service;
public class PageVisitedEventArgs : EventArgs
{
    public string Url { get; }
    public string Title { get; }
    public string Transition { get; }
    public PageVisitedEventArgs(string url, string title, string transition)
    {
        Url = url; Title = title; Transition = transition;
    }
}

public class NavigationManager(IWV2Service webviewService)
{
    private bool _pendingTyped;

    private WebView2 _browser => webviewService.Browser;
    private CoreWebView2Environment _environment => webviewService.Environment;
    private string _url { get => webviewService.URL; set => webviewService.URL = value; }
    private string _tempFolder { get => webviewService.TempFolder; set => webviewService.TempFolder = value; }
    public List<string> ValidUrlSuffixes { get; set; } = new List<string>() { ".com", ".org", ".net", ".edu", ".gov", ".io", ".co", ".us", ".uk", ".ph", ".html" };
    public event EventHandler<PageVisitedEventArgs> PageVisited;
    public event EventHandler<CoreWebView2NewWindowRequestedEventArgs> NewWindowRequested;
    public event EventHandler<string> NavigationChanged;

    public Task GoTo(string address) => NavigateTo(address);
    public Task NewTabGoTo(string address) => NavigateToNewTab(address);
    public void Reload() => _browser.Reload();
    public void GoBack()
    {
        if (_browser.CanGoBack) _browser.GoBack(); 
    }
    public void GoForward()
    {
        if (_browser.CanGoForward) _browser.GoForward();
    }
    public async Task Incognito_DisposeSession()
    {
        string tempfolderpath = _tempFolder;
        int maxRetries = 5;
        int delayMilliseconds = 2000;
        for (int attempt = 1; attempt <= maxRetries; attempt++)
        {
            try
            {
                //ensure all processes are closed first
                await Task.Run(() =>
                {
                    if (!Directory.Exists(tempfolderpath)) { return; }
                    Directory.Delete(tempfolderpath, true);
                });
                _tempFolder = string.Empty;
            }
            catch
            {
                await Task.Delay(delayMilliseconds * attempt);
            }
        }
    }
    public async Task EnableNewWindowRequest()
    {
        await _browser.EnsureCoreWebView2Async(_environment);
        _browser.CoreWebView2.NewWindowRequested += CoreWebView2_NewWindowRequested;
    }
    public async Task EnableNavigationMonitoring()
    {
        await _browser.EnsureCoreWebView2Async(_environment);
        _browser.NavigationStarting += OnNavigationStarting;
        _browser.NavigationCompleted += OnNavigationCompleted;
    }

    private string EnsureHttpsPrefix(string url)
    {
        if (!url.StartsWith("http://", StringComparison.OrdinalIgnoreCase) &&
            !url.StartsWith("https://", StringComparison.OrdinalIgnoreCase) &&
            !url.StartsWith("edge://", StringComparison.OrdinalIgnoreCase) &&
            !url.StartsWith("chrome-extension://", StringComparison.OrdinalIgnoreCase))
        {
            url = "https://" + url;
        }
        return url;
    }
    private bool IsURLSuffixValid(string url)
    {
        if (url.StartsWith("edge://", StringComparison.OrdinalIgnoreCase)) { return true; }
        foreach (string tld in ValidUrlSuffixes)
        {
            if (url.Contains(tld, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }
        return false;
    }

    private async Task NavigateTo(string address)
    {
        _pendingTyped = true;
        if (IsURLSuffixValid(address))
        {
            _url = EnsureHttpsPrefix(address);
            await _browser.EnsureCoreWebView2Async(_environment);
            _browser.CoreWebView2.Navigate(_url);
            return;
        }
        string searchQuery = Uri.EscapeDataString(address);
        string searchUrl = "https://www.google.com/search?q=" + searchQuery;
        _url = (new Uri(searchUrl)).ToString();
        PageVisited?.Invoke(this, new PageVisitedEventArgs(searchUrl, address, "typed")); // fallthrough, recorded below too
        await _browser.EnsureCoreWebView2Async(_environment);
        _browser.CoreWebView2.Navigate(_url);
    }
    private async Task NavigateToNewTab(string address)
    {
        if (IsURLSuffixValid(address))
        {
            _url = EnsureHttpsPrefix(address);
            await _browser.EnsureCoreWebView2Async();
            _browser.Source = new Uri(_url);
            return;
        }

        string searchQuery = Uri.EscapeDataString(address);
        string searchUrl = "https://www.google.com/search?q=" + searchQuery;
        _url = (new Uri(searchUrl)).ToString();

        await _browser.EnsureCoreWebView2Async();
        _browser.Source = new Uri(searchUrl);
    }
    private void CoreWebView2_NewWindowRequested(object? sender, CoreWebView2NewWindowRequestedEventArgs e)
    {
        NewWindowRequested?.Invoke(sender, e);

        if (NewWindowRequested == null)
        {
            DialogResult result = MessageBox.Show($"A website wants to open a new window:\n{e.Uri}\n\nAllow this popup?", "Confirm Popup", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                e.Handled = false;
                //// Optional: Open in the current WebView
                //NavigateTo(webviewControl, environment, e.Uri);
                return;
            }
            e.Handled = true;
            Debug.WriteLine($"Popup blocked: {e.Uri}");
        }
    }
    private async void OnNavigationCompleted(object? sender, CoreWebView2NavigationCompletedEventArgs e)
    {
        if (e.IsSuccess)
        {
            string siteTitle = await _browser.CoreWebView2.ExecuteScriptAsync("document.title");
            webviewService.SiteTitle = siteTitle.Trim('"');
            string currentUrl = _browser.Source.ToString();
            NavigationChanged?.Invoke(this, currentUrl);

            string transition = _pendingTyped ? "typed" : "link";
            _pendingTyped = false;
            PageVisited?.Invoke(this, new PageVisitedEventArgs(currentUrl, webviewService.SiteTitle, transition));
        }
        else
        {
            _pendingTyped = false;
            NavigationChanged?.Invoke(this, "Navigation failed.");
        }
    }
    private void OnNavigationStarting(object? sender, CoreWebView2NavigationStartingEventArgs e)
    {
        _url = e.Uri;
        NavigationChanged?.Invoke(this, $"Navigating to: {e.Uri}");
    }
}
