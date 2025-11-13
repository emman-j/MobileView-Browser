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

        private string EnsureHttpsPrefix(string url)
        {
            try
            {
                if (!url.StartsWith("http://", StringComparison.OrdinalIgnoreCase) &&
                    !url.StartsWith("https://", StringComparison.OrdinalIgnoreCase) &&
                    !url.StartsWith("edge://", StringComparison.OrdinalIgnoreCase) &&
                    !url.StartsWith("chrome-extension://", StringComparison.OrdinalIgnoreCase))
                {
                    url = "https://" + url;
                }
            }
            catch (Exception ex)
            {
                _WV2Service.LogError?.Invoke(ex);
            }
            return url;
        }
        private bool IsURLSuffixValid(string url)
        {
            try
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
            }
            catch (Exception ex)
            {
                _WV2Service.LogError?.Invoke(ex);
            }
            return false;
        }
        private async Task NavigateTo(string address)
        {
            try
            {
                if (IsURLSuffixValid(address))
                {
                    _WV2Service.URL = EnsureHttpsPrefix(address);
                    await _WV2Service.EnsureCoreWebView2Async();
                    WebControl.Navigate(_WV2Service.URL);
                    return;
                }
                string searchQuery = Uri.EscapeDataString(address);
                string searchUrl = "https://www.google.com/search?q=" + searchQuery;
                _WV2Service.URL = (new Uri(searchUrl)).ToString();

                await _WV2Service.EnsureCoreWebView2Async();
                WebControl.Navigate(_WV2Service.URL);
            }
            catch (Exception ex)
            {
                _WV2Service.LogError?.Invoke(ex);
            }
        }
        private async Task NavigateToNewTab(string address)
        {
            try
            {
                if (IsURLSuffixValid(address))
                {
                    _WV2Service.URL = EnsureHttpsPrefix(address);
                    await _WV2Service.EnsureCoreWebView2Async();
                    WebControl.Source = new Uri(_WV2Service.URL);
                    return;
                }

                string searchQuery = Uri.EscapeDataString(address);
                string searchUrl = "https://www.google.com/search?q=" + searchQuery;
                _WV2Service.URL = (new Uri(searchUrl)).ToString();

                await _WV2Service.EnsureCoreWebView2Async();
                WebControl.Source = new Uri(searchUrl);
            }
            catch (Exception ex)
            {
                _WV2Service.LogError?.Invoke(ex);
            }
        }
        private void CoreWebView2_NewWindowRequested(object? sender, CoreWebView2NewWindowRequestedEventArgs e)
        {
            try
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
            catch (Exception ex)
            {
                _WV2Service.LogError?.Invoke(ex);
            }
        }
        private async void OnNavigationCompleted(object? sender, CoreWebView2NavigationCompletedEventArgs e)
        {
            try
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
            catch (Exception ex)
            {
                _WV2Service.LogError?.Invoke(ex);
            }
        }
        private void OnNavigationStarting(object? sender, CoreWebView2NavigationStartingEventArgs e)
        {
            try
            {
                _WV2Service.URL = e.Uri;
                _WV2Service.RaiseNavigationChanged(sender, $"Navigating to: {e.Uri}");
            }
            catch (Exception ex)
            {
                _WV2Service.LogError?.Invoke(ex);
            }
        }

        public async void GoTo(string address) => await NavigateTo(address);
        public async void NewTabGoTo(string address) => await NavigateToNewTab(address);
        public void Reload() => WebControl.Reload();
        public void GoBack() { if (WebControl.CanGoBack) WebControl.GoBack(); }
        public void GoForward() { if (WebControl.CanGoForward) WebControl.GoForward(); }
        public async Task Incognito_DisposeSession()
        {
            try
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
            catch (Exception ex)
            {
                _WV2Service.LogError?.Invoke(ex);
            }
        }
        public async Task EnableNewWindowRequest()
        {
            WebControl.CoreWebView2.NewWindowRequested += CoreWebView2_NewWindowRequested;
        }
        public async Task EnableNavigationMonitoring()
        {
            WebControl.NavigationStarting += OnNavigationStarting;
            WebControl.NavigationCompleted += OnNavigationCompleted;
        }
    }
}
