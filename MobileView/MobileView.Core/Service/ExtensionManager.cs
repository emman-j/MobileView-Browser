using Microsoft.Web.WebView2.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MobileView.Core.Service
{
    public class ExtensionManager
    {
        private WV2Service _WV2Service;
        private string _addExtensionsDirectory => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Extensions_Local");
        private string localExtensionsPath => (!string.IsNullOrWhiteSpace(_WV2Service._TempFolder)) ?
            Path.Combine(_WV2Service._TempFolder, "EBWebView", "Default", "Extensions_Local") :
            Path.Combine(_WV2Service.ProfileFolder, "EBWebView", "Default", "Extensions_Local");

        public ExtensionManager(WV2Service wv2Service)
        {
            _WV2Service = wv2Service;
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
        public void EnsureExtensionsDirectory() // added to allow easy installation of extensions for now
        {
            if (!Directory.Exists(_addExtensionsDirectory))
            {
                Directory.CreateDirectory(_addExtensionsDirectory);
            }
        }
        private async void InitializeExtensions()
        {
            List<string> extensionsPath = _WV2Service.ExtensionsPath;
            if (extensionsPath == null || !extensionsPath.Any()) { return; }

            int count = 0;
            foreach (string originalExtensionPath in extensionsPath)
            {
                if (!Directory.Exists(originalExtensionPath))
                { count++; continue; }
            }
            if (count == extensionsPath.Count()) { return; }


            await _WV2Service.WebControl.EnsureCoreWebView2Async(_WV2Service.Environment);
            CoreWebView2BrowserExtension extension = await AddExtensionsAsync();
            await extension.EnableAsync(true);
        }
        private async Task<CoreWebView2BrowserExtension> AddExtensionsAsync()
        {
            try
            {
                CoreWebView2BrowserExtension extension = null;

                if (!Directory.Exists(localExtensionsPath))
                {
                    Directory.CreateDirectory(localExtensionsPath);
                }

                foreach (string originalExtensionPath in _WV2Service.ExtensionsPath)
                {

                    string extensionName = Path.GetFileName(originalExtensionPath);
                    string localExtensionPath = Path.Combine(localExtensionsPath, extensionName);

                    if (!Directory.Exists(localExtensionPath))
                    {
                        CopyDirectory(originalExtensionPath, localExtensionPath);
                    }

                    CoreWebView2Profile profile = await _WV2Service.GetProfile();
                    extension = await profile.AddBrowserExtensionAsync(localExtensionPath);
                }
                return extension;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to load extensions:\n{ex.Message}");
            }
        }
        private async Task<CoreWebView2BrowserExtension> AddExtensionsAsync(string extensionPath)
        {
            try
            {
                CoreWebView2BrowserExtension extension = null;

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

                CoreWebView2Profile profile = await _WV2Service.GetProfile();
                extension = await profile.AddBrowserExtensionAsync(localExtensionPath);

                return extension;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to load extensions:\n{ex.Message}");
            }
        }
        private async Task<IReadOnlyList<CoreWebView2BrowserExtension>> GetBrowserExtensionsListAsync()
        {
            try
            {
                CoreWebView2Profile profile = await _WV2Service.GetProfile();
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
        public async Task<List<string>> GetExtensionsList()
        {
            List<string> extensions = new List<string>();

            IReadOnlyList<CoreWebView2BrowserExtension> extensionsList = await GetBrowserExtensionsListAsync();

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
