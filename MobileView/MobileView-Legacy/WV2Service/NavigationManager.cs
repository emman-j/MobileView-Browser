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
    public List<string> validUrlSuffixes { get; set; } = new List<string>() { ".com", ".org", ".net", ".edu", ".gov", ".io", ".co", ".us", ".uk", ".ph", ".html" };
    public event EventHandler<PageVisitedEventArgs> PageVisited;
    private bool _pendingTyped;

    private readonly WebView2 browser = webviewService.Browser;
    private readonly CoreWebView2Environment Environment = webviewService.Environment;
    private string URL { get => webviewService.URL; set => webviewService.URL = value; }
    private string TempFolder { get => webviewService.TempFolder; set => webviewService.TempFolder = value; }

    public event EventHandler<CoreWebView2NewWindowRequestedEventArgs> NewWindowRequested;
    public event EventHandler<string> NavigationChanged;

    public Task GoTo(string address) => NavigateTo(address);
    public Task NewTabGoTo(string address) => NavigateToNewTab(address);
    public void Reload() => browser.Reload();
    public void GoBack()
    {
        if (browser.CanGoBack) browser.GoBack(); 
    }
    public void GoForward()
    {
        if (browser.CanGoForward) browser.GoForward();
    }
    public async Task Incognito_DisposeSession()
    {
        string tempfolderpath = TempFolder;
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
                TempFolder = string.Empty;
            }
            catch
            {
                await Task.Delay(delayMilliseconds * attempt);
            }
        }
    }
    public async Task EnableNewWindowRequest()
    {
        await browser.EnsureCoreWebView2Async(Environment);
        browser.CoreWebView2.NewWindowRequested += CoreWebView2_NewWindowRequested;
    }
    public async Task EnableNavigationMonitoring()
    {
        await browser.EnsureCoreWebView2Async(Environment);
        browser.NavigationStarting += OnNavigationStarting;
        browser.NavigationCompleted += OnNavigationCompleted;
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
        foreach (string tld in validUrlSuffixes)
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
            URL = EnsureHttpsPrefix(address);
            await browser.EnsureCoreWebView2Async(Environment);
            browser.CoreWebView2.Navigate(URL);
            return;
        }
        string searchQuery = Uri.EscapeDataString(address);
        string searchUrl = "https://www.google.com/search?q=" + searchQuery;
        URL = (new Uri(searchUrl)).ToString();
        PageVisited?.Invoke(this, new PageVisitedEventArgs(searchUrl, address, "typed")); // fallthrough, recorded below too
        await browser.EnsureCoreWebView2Async(Environment);
        browser.CoreWebView2.Navigate(URL);
    }
    private async Task NavigateToNewTab(string address)
    {
        if (IsURLSuffixValid(address))
        {
            URL = EnsureHttpsPrefix(address);
            await browser.EnsureCoreWebView2Async();
            browser.Source = new Uri(URL);
            return;
        }

        string searchQuery = Uri.EscapeDataString(address);
        string searchUrl = "https://www.google.com/search?q=" + searchQuery;
        URL = (new Uri(searchUrl)).ToString();

        await browser.EnsureCoreWebView2Async();
        browser.Source = new Uri(searchUrl);
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
            string siteTitle = await browser.CoreWebView2.ExecuteScriptAsync("document.title");
            webviewService.SiteTitle = siteTitle.Trim('"');
            string currentUrl = browser.Source.ToString();
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
        URL = e.Uri;
        NavigationChanged?.Invoke(this, $"Navigating to: {e.Uri}");
    }
}
