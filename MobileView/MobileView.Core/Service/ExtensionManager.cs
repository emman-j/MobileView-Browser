using Microsoft.Web.WebView2.Core;
using System.IO;

namespace MobileView.Core.Service
{
    public class ExtensionManager
    {
        private WV2Service _WV2Service;
        private string _addExtensionsDirectory => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Extensions_Local"); //Path to add extensions from
        private string _localExtensionsPath => (!string.IsNullOrWhiteSpace(_WV2Service._TempFolder)) ? // Extensions installation path
            Path.Combine(_WV2Service._TempFolder, "EBWebView", "Default", "Extensions_Local") :
            Path.Combine(_WV2Service.ProfileFolder, "EBWebView", "Default", "Extensions_Local");
        public List<string> ExtensionsPath { get; set; }

        public ExtensionManager(WV2Service wv2Service)
        {
            _WV2Service = wv2Service;
        }

        private async Task CopyDirectoryAsync(string sourceDir, string destinationDir) => await Task.Run(() => CopyDirectory(sourceDir, destinationDir));
        private void CopyDirectory(string sourceDir, string destinationDir)
        {
            try
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
            catch (Exception ex)
            {
                _WV2Service.LogError?.Invoke(ex);
            }
        }
        private async Task AddExtensionsAsync()
        {
            try
            {
                if(ExtensionsPath == null || ExtensionsPath.Count == 0)
                    throw new ArgumentNullException(nameof(ExtensionsPath), "No extensions to load.");

                foreach (string originalExtensionPath in ExtensionsPath)
                {
                    CoreWebView2BrowserExtension extension = await AddExtensionAsync(originalExtensionPath);
                    if (extension != null)
                        await extension.EnableAsync(true);
                }
            }
            catch (Exception ex)
            {
                _WV2Service.LogError?.Invoke(ex);
            }
        }
        private async Task<CoreWebView2BrowserExtension> AddExtensionAsync(string extensionPath)
        {
            try
            {
                CoreWebView2BrowserExtension extension = null;

                EnsureDirectory(_localExtensionsPath);

                string extensionName = Path.GetFileName(extensionPath);
                string newExtensionPath = Path.Combine(_localExtensionsPath, extensionName);

                if (!Directory.Exists(newExtensionPath))
                    CopyDirectory(extensionPath, newExtensionPath);

                await _WV2Service.EnsureCoreWebView2Async();
                CoreWebView2Profile profile = await _WV2Service.GetProfileAsync();
                extension = await profile.AddBrowserExtensionAsync(newExtensionPath);

                return extension;
            }
            catch (Exception ex)
            {
                _WV2Service.LogError?.Invoke(ex);
            }
            return null;
        }
        public async Task InitializeExtensionsAsync()
        {
            try
            {
                if (ExtensionsPath == null || ExtensionsPath.Count == 0) return;

                int count = 0;
                foreach (string originalExtensionPath in ExtensionsPath)
                {
                    if (Directory.Exists(originalExtensionPath))
                        count++;
                }
                EnsureDirectory(_localExtensionsPath);

                int toInstallCount = GetExtensionsPath().Count;
                int installedCount = (await GetExtensionsList()) .Count(x => !x.Contains("Microsoft Clipboard Extension") && !x.Contains("Microsoft Edge PDF Viewer"));
                if (toInstallCount == installedCount) return;

                await AddExtensionsAsync();
            }
            catch (Exception ex)
            {
                _WV2Service.LogError?.Invoke(ex);
            }
        }
        // added to allow easy installation of extensions for now
        public void EnsureDirectory() => EnsureDirectory(_addExtensionsDirectory);
        public void EnsureDirectory(string dir)
        {
            try
            {
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
            }
            catch (Exception ex)
            {
                _WV2Service.LogError?.Invoke(ex);
            }
        }
        public async Task<List<string>> GetExtensionsList()
        {
            List<string> extensions = new List<string>();

            try
            {
                CoreWebView2Profile profile = await _WV2Service.GetProfileAsync();
                IReadOnlyList<CoreWebView2BrowserExtension> extensionsList = await profile.GetBrowserExtensionsAsync();

                foreach (var extension in extensionsList)
                {
                    extensions.Add(extension.Name);
                    Console.WriteLine($"- {extension.Name}, ID: {extension.Id}");
                }
            }
            catch (Exception ex)
            {
                _WV2Service.LogError?.Invoke(ex);
            }

            return extensions;
        }
        public List<string> GetExtensionsPath() => GetExtensionsPath(_addExtensionsDirectory);
        public List<string> GetExtensionsPath(string path) => Directory.GetDirectories(path).ToList();
    }
}
