using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;
using System.Diagnostics;

namespace WV2Service
{
    public partial class WebViewService
    {
        private async void InitializeExtensions(WebView2 webView, CoreWebView2Environment environment, CoreWebView2Profile profile)
        {
            if (ExtensionsPath == null || !ExtensionsPath.Any()) { return; }

            int count = 0;
            foreach (string originalExtensionPath in ExtensionsPath)
            {
                if (!Directory.Exists(originalExtensionPath))
                { count++; continue; }
            }
            if (count == ExtensionsPath.Count()) { return; }

            await webView.EnsureCoreWebView2Async(environment);
            CoreWebView2BrowserExtension extension = await AddExtensionsAsync(webView, environment, profile);
            await extension.EnableAsync(true);
        }
        private async Task<CoreWebView2BrowserExtension> AddExtensionsAsync(WebView2 webView, CoreWebView2Environment environment, CoreWebView2Profile profile)
        {
            try
            {
                CoreWebView2BrowserExtension extension = null;

                string localExtensionsPath = (!string.IsNullOrWhiteSpace(_TempFolder)) ?
                    Path.Combine(_TempFolder, "EBWebView", "Default", "Extensions_Local") :
                    Path.Combine(ProfileFolder, "EBWebView", "Default", "Extensions_Local");

                if (!Directory.Exists(localExtensionsPath))
                {
                    Directory.CreateDirectory(localExtensionsPath);
                }

                foreach (string originalExtensionPath in ExtensionsPath)
                {

                    string extensionName = Path.GetFileName(originalExtensionPath);

                    string localExtensionPath = Path.Combine(localExtensionsPath, extensionName);

                    if (!Directory.Exists(localExtensionPath))
                    {
                        CopyDirectory(originalExtensionPath, localExtensionPath);
                    }

                    profile = profile ?? await GetProfile(webView, environment);
                    extension = await profile.AddBrowserExtensionAsync(localExtensionPath);
                    //extension.RemoveAsync();
                }
                return extension;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to load extensions:\n{ex.Message}");
            }
        }
        private async Task<CoreWebView2BrowserExtension> AddExtensionsAsync(WebView2 webView, CoreWebView2Environment environment, CoreWebView2Profile profile, string extensionPath)
        {
            try
            {
                CoreWebView2BrowserExtension extension = null;

                string localExtensionsPath = (!string.IsNullOrWhiteSpace(_TempFolder)) ?
                    Path.Combine(_TempFolder, "EBWebView", "Default", "Extensions_Local") :
                    Path.Combine(ProfileFolder, "EBWebView", "Default", "Extensions_Local");

                if (!Directory.Exists(localExtensionsPath))
                {
                    Directory.CreateDirectory(localExtensionsPath);
                }

                string extensionName = Path.GetFileName(extensionPath);

                string localExtensionPath = Path.Combine(localExtensionsPath, extensionName);

                if (!Directory.Exists(localExtensionPath))
                {
                    CopyDirectory(extensionPath, localExtensionPath);
                }

                profile = profile ?? await GetProfile(webView, environment);
                extension = await profile.AddBrowserExtensionAsync(localExtensionPath);

                return extension;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to load extensions:\n{ex.Message}");
            }
        }
        private async Task<IReadOnlyList<CoreWebView2BrowserExtension>> GetBrowserExtensionsListAsync(WebView2 webView, CoreWebView2Environment environment, CoreWebView2Profile profile)
        {
            try
            {
                profile = profile != null ? profile : await GetProfile(webView, environment);
                IReadOnlyList<CoreWebView2BrowserExtension> extensions = await profile.GetBrowserExtensionsAsync();

                Debug.WriteLine("Installed Extensions:");
                foreach (var extension in extensions)
                {
                    Debug.WriteLine($"- {extension.Name}, ID: {extension.Id}");
                }
                return extensions;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving extensions:\n{ex.Message}");
            }
        }
        private void CopyDirectory(string sourceDir, string destinationDir)
        {
            Directory.CreateDirectory(destinationDir);

            foreach (var file in Directory.GetFiles(sourceDir))
            {
                string destFile = Path.Combine(destinationDir, Path.GetFileName(file));
                File.Copy(file, destFile, overwrite: true);
            }

            foreach (var directory in Directory.GetDirectories(sourceDir))
            {
                string destDir = Path.Combine(destinationDir, Path.GetFileName(directory));
                CopyDirectory(directory, destDir);
            }
        }
    }
}
