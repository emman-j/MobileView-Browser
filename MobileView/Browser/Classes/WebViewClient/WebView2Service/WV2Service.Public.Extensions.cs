using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;
using System.Diagnostics;

namespace WV2Service
{
    public partial class WebViewService
    {
        public void EnsureExtensionsDirectory() // added to allow easy installation of extensions for now
        {
            string appBaseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            _addExtensionsDirectory = Path.Combine(appBaseDirectory, "Extensions_Local");

            if (!Directory.Exists(_addExtensionsDirectory))
            {
                Directory.CreateDirectory(_addExtensionsDirectory);
            }
        }
        public async Task<List<string>> GetExtensionsList()
        {
            List<string> extensions = new List<string>();

            IReadOnlyList<CoreWebView2BrowserExtension> extensionsList = await GetBrowserExtensionsListAsync(WebViewControl, environment, Profile);

            foreach (var extension in extensionsList)
            {
                extensions.Add(extension.Name);
                Console.WriteLine($"- {extension.Name}, ID: {extension.Id}");
            }

            return extensions;
        }
        public List<string> GetExtensionsPath()
        {
            string[] subDirectories = Directory.GetDirectories(_addExtensionsDirectory);

            List<string> pathList = new List<string>();

            foreach (string subDir in subDirectories)
            {
                pathList.Add(subDir);
            }

            return pathList;
        }
    }
}
