using Microsoft.Web.WebView2.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace MobileView.Core.Service
{
    public class NavigationManager
    {
        private WV2Service _WV2Service;

        private WebViewWrapper WebControl => _WV2Service.WebControl;
        public NavigationManager(WV2Service wv2Service)
        {
            _WV2Service = wv2Service;
        }

        public void GoTo(string address) => NavigateTo(address);
        public void NewTabGoTo(string address) => NavigateToNewTab(address);
        public void Reload() => WebControl.Reload();
        public void GoBack()
        {
            if (WebControl.CanGoBack) WebControl.GoBack();
        }
        public void GoForward()
        {
            if (WebControl.CanGoForward) WebControl.GoForward();
        }
        public async void Incognito_DisposeSession()
        {
            string tempfolderpath = _WV2Service._TempFolder;
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
                    _WV2Service._TempFolder = string.Empty;
                }
                catch
                {
                    await Task.Delay(delayMilliseconds * attempt);
                }
            }
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
            string[] validTLDs = { ".com", ".org", ".net", ".edu", ".gov", ".io", ".co", ".us", ".uk", ".ph", ".html" };
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
        private async void NavigateTo(string address)
        {
            if (IsURLSuffixValid(address))
            {
                _WV2Service.URL = EnsureHttpsPrefix(address);
                await WebControl.EnsureCoreWebView2Async(_WV2Service.Environment);
                WebControl.CoreWebView2.Navigate(_WV2Service.URL);
                return;
            }
            string searchQuery = Uri.EscapeDataString(address);
            string searchUrl = "https://www.google.com/search?q=" + searchQuery;
            _WV2Service.URL = (new Uri(searchUrl)).ToString();

            await WebControl.EnsureCoreWebView2Async(_WV2Service.Environment);
            WebControl.CoreWebView2.Navigate(_WV2Service.URL);
        }
        private async void NavigateToNewTab(string address)
        {
            if (IsURLSuffixValid(address))
            {
                _WV2Service.URL = EnsureHttpsPrefix(address);
                await WebControl.EnsureCoreWebView2Async();
                WebControl.Source = new Uri(_WV2Service.URL);
                return;
            }

            string searchQuery = Uri.EscapeDataString(address);
            string searchUrl = "https://www.google.com/search?q=" + searchQuery;
            _WV2Service.URL = (new Uri(searchUrl)).ToString();

            await WebControl.EnsureCoreWebView2Async();
            WebControl.Source = new Uri(searchUrl);
        }
        private async void EnableNewWindowRequest()
        {
            await WebControl.EnsureCoreWebView2Async();
            WebControl.CoreWebView2.NewWindowRequested += CoreWebView2_NewWindowRequested;
        }
        private async void EnableNavigationMonitoring()
        {
            await WebControl.EnsureCoreWebView2Async();
            WebControl.NavigationStarting += OnNavigationStarting;
            WebControl.NavigationCompleted += OnNavigationCompleted;
        }
        private void CoreWebView2_NewWindowRequested(object? sender, CoreWebView2NewWindowRequestedEventArgs e)
        {
            _WV2Service.RaiseNewWindowRequested(sender, e);

            //if (_WV2Service.NewWindowRequested == null)
            //{
            //    DialogResult result = MessageBox.Show($"A website wants to open a new window:\n{e.Uri}\n\nAllow this popup?", "Confirm Popup", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            //    if (result == DialogResult.Yes)
            //    {
            //        e.Handled = false;
            //        //// Optional: Open in the current WebView
            //        //NavigateTo(webviewControl, environment, e.Uri);
            //        return;
            //    }
            //    e.Handled = true;
            //    Debug.WriteLine($"Popup blocked: {e.Uri}");
            //}
        }
        private async void OnNavigationCompleted(object? sender, CoreWebView2NavigationCompletedEventArgs e)
        {
            if (e.IsSuccess)
            {
                string siteTitle = await WebControl.CoreWebView2.ExecuteScriptAsync("document.title");
                _WV2Service.SiteTitle = siteTitle.Trim('"');
                _WV2Service.RaiseNavigationChanged(sender, WebControl.Source.ToString());
            }
            else
            {
                _WV2Service.RaiseNavigationChanged(sender, "Navigation failed.");
            }
        }
        private void OnNavigationStarting(object? sender, CoreWebView2NavigationStartingEventArgs e)
        {
            _WV2Service.URL = e.Uri;
            _WV2Service.RaiseNavigationChanged(sender, $"Navigating to: {e.Uri}");
        }
    }
}
