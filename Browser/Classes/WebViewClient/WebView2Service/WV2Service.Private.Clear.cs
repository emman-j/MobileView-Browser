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
        private async void ClearAllBrowsingData(WebView2 webView, CoreWebView2Environment environment, CoreWebView2Profile profile)
        {
            try
            {
                profile = profile != null ? profile : await GetProfile(webView, environment);
                await profile.ClearBrowsingDataAsync(
                    CoreWebView2BrowsingDataKinds.Cookies |
                    CoreWebView2BrowsingDataKinds.BrowsingHistory |
                    CoreWebView2BrowsingDataKinds.GeneralAutofill |
                    CoreWebView2BrowsingDataKinds.PasswordAutosave |
                    CoreWebView2BrowsingDataKinds.ServiceWorkers |
                    CoreWebView2BrowsingDataKinds.CacheStorage |
                    CoreWebView2BrowsingDataKinds.DownloadHistory |
                    CoreWebView2BrowsingDataKinds.DiskCache);
                webView.Reload();
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to clear data:\n{ex.Message}");
            }
        }
        private async void ClearBrowsingDataBetweenDateRange(WebView2 webView, CoreWebView2Environment environment, CoreWebView2Profile profile, DateTime startDate, DateTime endDate)
        {
            try
            {
                profile = profile != null ? profile : await GetProfile(webView, environment);
                await profile.ClearBrowsingDataAsync(
                    CoreWebView2BrowsingDataKinds.Cookies |
                    CoreWebView2BrowsingDataKinds.BrowsingHistory |
                    CoreWebView2BrowsingDataKinds.GeneralAutofill |
                    CoreWebView2BrowsingDataKinds.PasswordAutosave |
                    CoreWebView2BrowsingDataKinds.ServiceWorkers |
                    CoreWebView2BrowsingDataKinds.CacheStorage |
                    CoreWebView2BrowsingDataKinds.DownloadHistory |
                    CoreWebView2BrowsingDataKinds.DiskCache, startDate, endDate);
                webView.Reload();
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to clear data:\n{ex.Message}");
            }
        }
        private async void ClearBrowserData(WebView2 webView, CoreWebView2Environment environment, CoreWebView2Profile profile, CoreWebView2BrowsingDataKinds dataKinds)
        {
            try
            {
                profile = profile != null ? profile : await GetProfile(webView, environment);
                await profile.ClearBrowsingDataAsync(dataKinds);
                webView.Reload();
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to clear data:\n{ex.Message}");
            }
        }
        private async void ClearAllBrowserData(WebView2 webView, CoreWebView2Environment environment, CoreWebView2Profile profile)
        {
            profile = profile != null ? profile : await GetProfile(webView, environment);
            await webView.EnsureCoreWebView2Async(environment);
            await profile.ClearBrowsingDataAsync();
            webView.Reload();
        }
    }
}
