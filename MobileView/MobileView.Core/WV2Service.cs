using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;
using MobileView.Core.Enums;
using MobileView.Core.Service;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MobileView.Core
{
    public class WV2Service : IWV2
    {
        public string SiteTitle { get; set; }
        public string ProfileName { get; set; }
        public string URL { get; set; }
        public string ProfileFolder { get; set; }
        public string UserAgent { get; set; }
        public string _TempFolder { get; set; }
        public CoreWebView2Environment Environment { get; set; }
        public CoreWebView2Profile Profile { get; set; }
        public WebViewWrapper WebControl { get; set; }
        public NavigationManager Navigation { get; set; }
        public ExtensionManager Extensions { get; set; }

        public event PropertyChangedEventHandler? PropertyChanged;
        public event EventHandler<CoreWebView2NewWindowRequestedEventArgs> NewWindowRequested;
        public event EventHandler<string> NavigationChanged;

        public delegate void LogErrorHandler(Exception ex, [CallerMemberName] string sender = "");
        public LogErrorHandler LogError;

        public WV2Service(UIFramework ui, object webcontrol)
        {
            Navigation = new NavigationManager(this);
            Extensions = new ExtensionManager(this);
            WebControl = new WebViewWrapper(ui, webcontrol);
            LogError = LogE;
        }

        void LogE(Exception ex, [CallerMemberName] string sender = "")
        {
            Debug.WriteLine($"Error in {sender}: {ex.Message}");
        }
        public async void InitializeBrowser()
        {
            Environment = await InitializeWebEnviromentAsync(ProfileName);
            InitializeWebViewSession();
        }
        public async void InitializeNewTab(string ProfileFolder)
        {
            Environment = await InitializeSharedWebEnviromentAsync(ProfileFolder);
            InitializeWebViewSession(false);
        }
        public async void Incognito_InitializeWebView()
        {
            Environment = await Incognito_InitializeWebEnviromentAsync("Incognito");
            InitializeWebViewSession();
        }
        private async void InitializeWebViewSession(bool initializeExtensions = true)
        {
            InitializeProfile();
            await EnableMobileViewAsync();
            string ver = GetBrowserVersionString();
            if (initializeExtensions)
                await Extensions.InitializeExtensionsAsync();
            await Navigation.EnableNewWindowRequest();
            await Navigation.EnableNavigationMonitoring();
        }
        public async void InitializeProfile() => Profile = await GetProfileAsync();
        public async Task<CoreWebView2Profile> GetProfileAsync()
        {
            try
            {
                await WebControl.EnsureCoreWebView2Async(Environment);
                return WebControl.CoreWebView2.Profile;
            }
            catch (Exception ex)
            {
                LogError(ex);
            }
            return null;
        }
        private async Task<CoreWebView2Environment> InitializeWebEnviromentAsync(string profileName)
        {
            try
            {
                // Path if the desired directory is in the locap appdata dir. I prefer it be in the base dir of the app itself
                //string userDataFolder = Path.Combine( Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "MyWebView2AppData", profileName);
                string appBaseDirectory = AppDomain.CurrentDomain.BaseDirectory;
                ProfileFolder = Path.Combine(appBaseDirectory, "WebControl", "profiles", profileName);

                CoreWebView2EnvironmentOptions environmentOptions = new CoreWebView2EnvironmentOptions { AreBrowserExtensionsEnabled = true };
                CoreWebView2Environment environment = await CoreWebView2Environment.CreateAsync(null, ProfileFolder, environmentOptions);
                await WebControl.EnsureCoreWebView2Async(environment);
                return environment;
            }
            catch (Exception ex)
            {
                LogError(ex);
            }
            return null;
        }
        private async Task<CoreWebView2Environment> Incognito_InitializeWebEnviromentAsync(string profileName)
        {
            try
            {
                _TempFolder = Path.Combine(Path.GetTempPath(), "EBWebView", "Incognito_" + Guid.NewGuid().ToString());
                Directory.CreateDirectory(_TempFolder);

                CoreWebView2EnvironmentOptions environmentOptions = new CoreWebView2EnvironmentOptions { AreBrowserExtensionsEnabled = true };

                CoreWebView2Environment environment = await CoreWebView2Environment.CreateAsync(null, _TempFolder, environmentOptions);

                CoreWebView2ControllerOptions options = environment.CreateCoreWebView2ControllerOptions();
                options.IsInPrivateModeEnabled = true;
                options.ProfileName = profileName;

                await WebControl.EnsureCoreWebView2Async(environment, options);
                return environment;
            }
            catch (Exception ex)
            {
                LogError(ex);
            }
            return null;
        }
        private async Task<CoreWebView2Environment> InitializeSharedWebEnviromentAsync(string FolderPath)
        {
            try
            {
                CoreWebView2EnvironmentOptions environmentOptions = new CoreWebView2EnvironmentOptions { AreBrowserExtensionsEnabled = true };
                CoreWebView2Environment environment = await CoreWebView2Environment.CreateAsync(null, FolderPath, environmentOptions);
                await WebControl.EnsureCoreWebView2Async(environment);
                return environment;
            }
            catch (Exception ex)
            {
                LogError(ex);
            }
            return null;
        }

        //private async void EnableMobileView()
        //{
        //    await WebControl.EnsureCoreWebView2Async(Environment);
        //    //webView.CoreWebView2.Settings.UserAgent = @"Mozilla/5.0 (Linux; Android 10; Mobile) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/108.0.0.0 Mobile Safari/537.36";
        //    WebControl.CoreWebView2.Settings.UserAgent = UserAgent;

        //    // Set viewport dimensions to match Samsung Galaxy Note 20 Ultra
        //    await WebControl.CoreWebView2.ExecuteScriptAsync(@"
        //        Object.defineProperty(window, 'innerWidth', { get: () => 412 });
        //        Object.defineProperty(window, 'innerHeight', { get: () => 915 });
        //        Object.defineProperty(window.screen, 'width', { get: () => 412 });
        //        Object.defineProperty(window.screen, 'height', { get: () => 915 });
        //        Object.defineProperty(window.screen, 'devicePixelRatio', { get: () => 3.5 });
        //    ");

        //    // Inject viewport meta tag for responsive design
        //    await WebControl.CoreWebView2.ExecuteScriptAsync(@"
        //        const meta = document.createElement('meta');
        //        meta.name = 'viewport';
        //        meta.content = 'width=device-width, initial-scale=1.0';
        //        document.head.appendChild(meta);
        //    ");
        //}
        public string GetBrowserVersionString()
        {
            try
            {
                return CoreWebView2Environment.GetAvailableBrowserVersionString();
            }
            catch (Exception ex)
            {
                LogError(ex);
            }
            return string.Empty;
        }
        private async Task EnableMobileViewAsync(int width = 412, int height = 915, double devicePixelRatio = 3.5)
        {
            try
            {
                await WebControl.EnsureCoreWebView2Async(Environment);

                // Set custom User-Agent if provided
                if (!string.IsNullOrEmpty(UserAgent))
                    WebControl.CoreWebView2.Settings.UserAgent = UserAgent;

                // Apply viewport dimensions dynamically
                string script = $@"
                    Object.defineProperty(window, 'innerWidth', {{ get: () => {width} }});
                    Object.defineProperty(window, 'innerHeight', {{ get: () => {height} }});
                    Object.defineProperty(window.screen, 'width', {{ get: () => {width} }});
                    Object.defineProperty(window.screen, 'height', {{ get: () => {height} }});
                    Object.defineProperty(window.screen, 'devicePixelRatio', {{ get: () => {devicePixelRatio} }});
                ";
                await WebControl.CoreWebView2.ExecuteScriptAsync(script);

                // Inject viewport meta tag for responsive layout
                string metaScript = @"
                    const existingMeta = document.querySelector('meta[name=viewport]');
                    if (!existingMeta) {
                        const meta = document.createElement('meta');
                        meta.name = 'viewport';
                        meta.content = 'width=device-width, initial-scale=1.0';
                        document.head.appendChild(meta);
                    } else {
                        existingMeta.content = 'width=device-width, initial-scale=1.0';
                    }
                ";
                await WebControl.CoreWebView2.ExecuteScriptAsync(metaScript);
            }
            catch (Exception ex)
            {
                LogError(ex);
            }
        }
        public void RaiseNavigationChanged(object? sender, string message)
        {
            NavigationChanged?.Invoke(sender, message);
        }
        public void RaiseNewWindowRequested(object? sender, CoreWebView2NewWindowRequestedEventArgs e)
        {
            NewWindowRequested?.Invoke(sender, e);
        }
    }
}
