using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;

namespace WV2Service
{
    public partial class WebViewService
    {
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
            string[] validTLDs = { ".com", ".org", ".net", ".edu", ".gov", ".io", ".co", ".us", ".uk", ".ph", ".html"};
            if (url.StartsWith("edge://", StringComparison.OrdinalIgnoreCase)) { return true; }
            foreach (string tld in validTLDs)
            {
                if (url.Contains(tld, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }
        private async void NavigateTo(WebView2 webView, CoreWebView2Environment environment, string address)
        {
            if (IsURLSuffixValid(address))
            {
                URL = EnsureHttpsPrefix(address);
                await webView.EnsureCoreWebView2Async(environment);
                webView.CoreWebView2.Navigate(URL);
                return;
            }
            string searchQuery = Uri.EscapeDataString(address);
            string searchUrl = "https://www.google.com/search?q=" + searchQuery;
            URL = (new Uri(searchUrl)).ToString();

            await webView.EnsureCoreWebView2Async(environment);
            webView.CoreWebView2.Navigate(URL);
        }
        private async void NavigateToNewTab(WebView2 webView, CoreWebView2Environment environment, string address)
        {
            if (IsURLSuffixValid(address))
            {
                URL = EnsureHttpsPrefix(address);
                await webView.EnsureCoreWebView2Async();
                webView.Source = new Uri(URL);
                return;
            }

            string searchQuery = Uri.EscapeDataString(address);
            string searchUrl = "https://www.google.com/search?q=" + searchQuery;
            URL = (new Uri(searchUrl)).ToString();

            await webView.EnsureCoreWebView2Async();
            webView.Source = new Uri(searchUrl);
        }
        private async void EnableNewWindowRequest(WebView2 webView, CoreWebView2Environment environment)
        {
            await webView.EnsureCoreWebView2Async(environment);
            webView.CoreWebView2.NewWindowRequested += CoreWebView2_NewWindowRequested;
        }
        private async void EnableNavigationMonitoring(WebView2 webView, CoreWebView2Environment environment)
        {
            await webView.EnsureCoreWebView2Async(environment);
            webView.NavigationStarting += OnNavigationStarting;
            webView.NavigationCompleted += OnNavigationCompleted;
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
                string siteTitle = await WebViewControl.CoreWebView2.ExecuteScriptAsync("document.title");
                SiteTitle = siteTitle.Trim('"');
                NavigationChanged?.Invoke(this, WebViewControl.Source.ToString());
            }
            else
            {
                NavigationChanged?.Invoke(this, "Navigation failed.");
            }
        }
        private void OnNavigationStarting(object? sender, CoreWebView2NavigationStartingEventArgs e)
        {
            URL = e.Uri;
            NavigationChanged?.Invoke(this, $"Navigating to: {e.Uri}");
        }
    }
}
