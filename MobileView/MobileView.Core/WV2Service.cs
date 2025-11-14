using Microsoft.Web.WebView2.Core;
using MobileView.Core.Enums;
using MobileView.Core.Service;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;

namespace MobileView.Core
{
    public class WV2Service : IWV2, INotifyPropertyChanged
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
        public HistoryManager History { get; set; }

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
            History = new HistoryManager(this);
            LogError = LogE;
        }

        // For debug only
        void LogE(Exception ex, [CallerMemberName] string sender = "")
        {
            Debug.WriteLine($"Error in {sender}: {ex.Message}");
        }
        protected void SetValue<T>(ref T field, T value, [CallerMemberName] string propertyName = "")
        {
            if (!Equals(field, value))
            {
                field = value;
                NotifyPropertyChanged(propertyName);
            }
        }
        public void NotifyPropertyChanged([CallerMemberName] string propertyname = "") 
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyname));

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
                await EnsureCoreWebView2Async(environment);
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

                await EnsureCoreWebView2Async(environment, options);
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
                await EnsureCoreWebView2Async(environment);
                return environment;
            }
            catch (Exception ex)
            {
                LogError(ex);
            }
            return null;
        }
        private async Task EnableMobileViewAsync(int width = 412, int height = 915, double devicePixelRatio = 3.5)
        {
            try
            {
                await EnsureCoreWebView2Async(Environment);

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
        public async void InitializeProfile() => Profile = await GetProfileAsync();
        public async Task<CoreWebView2Profile> GetProfileAsync()
        {
            try
            {
                await EnsureCoreWebView2Async(Environment);
                return WebControl.CoreWebView2.Profile;
            }
            catch (Exception ex)
            {
                LogError(ex);
            }
            return null;
        }
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
        public async Task EnsureCoreWebView2Async() => await EnsureCoreWebView2Async(Environment);
        public async Task EnsureCoreWebView2Async(CoreWebView2Environment environment) => await WebControl.EnsureCoreWebView2Async(environment);
        public Task EnsureCoreWebView2Async(CoreWebView2Environment environment, CoreWebView2ControllerOptions controllerOptions)
            => WebControl.EnsureCoreWebView2Async(environment, controllerOptions);
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
