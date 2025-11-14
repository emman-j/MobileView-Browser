using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;
using System.Diagnostics;

namespace WV2Service
{
    public partial class WebViewService
    {

        private void InitializeBrowser()
        {
            InitializeProfile();
            EnableMobileView();
            InitializeExtensions();
            EnableNewWindowRequest();
            EnableNavigationMonitoring();
        }
        private async Task<CoreWebView2Profile> GetProfile(WebView2 webView, CoreWebView2Environment environment)
        {
            try
            {
                await webView.EnsureCoreWebView2Async(environment);
                return webView.CoreWebView2.Profile;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to get profile:\n{ex.Message}");
            }
        }
        private async Task<CoreWebView2Environment> InitializeWebEnviromentAsync(WebView2 webView, string profileName)
        {
            try
            {
                // Path if the desired directory is in the locap appdata dir. I prefer it be in the base dir of the app itself
                //string userDataFolder = Path.Combine( Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "MyWebView2AppData", profileName);
                string appBaseDirectory = AppDomain.CurrentDomain.BaseDirectory;
                ProfileFolder = Path.Combine(appBaseDirectory, "WebControl", "profiles", profileName);

                CoreWebView2EnvironmentOptions environmentOptions = new CoreWebView2EnvironmentOptions { AreBrowserExtensionsEnabled = true };
                CoreWebView2Environment environment = await CoreWebView2Environment.CreateAsync(null, ProfileFolder, environmentOptions);
                await webView.EnsureCoreWebView2Async(environment);
                return environment;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to initialize environment:\n{ex.Message}");
            }
        }
        private async Task<CoreWebView2Environment> Incognito_InitializeWebEnviromentAsync(WebView2 webView, string profileName)
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

                await webView.EnsureCoreWebView2Async(environment, options);
                //CoreWebView2Controller controller = await environment.CreateCoreWebView2ControllerAsync(webView.Handle, options);

                return environment;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to initialize environment:\n{ex.Message}");
            }
        }
        private async Task<CoreWebView2Environment> InitializeSharedWebEnviromentAsync(WebView2 webView, string FolderPath)
        {
            try
            {
                CoreWebView2EnvironmentOptions environmentOptions = new CoreWebView2EnvironmentOptions { AreBrowserExtensionsEnabled = true };
                CoreWebView2Environment environment = await CoreWebView2Environment.CreateAsync(null, FolderPath, environmentOptions);
                await webView.EnsureCoreWebView2Async(environment);
                return environment;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to initialize environment:\n{ex.Message}");
            }
        }
        private async void EnableMobileView(WebView2 webView, CoreWebView2Environment environment)
        {
            await webView.EnsureCoreWebView2Async(environment);
            //webView.CoreWebView2.Settings.UserAgent = @"Mozilla/5.0 (Linux; Android 10; Mobile) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/108.0.0.0 Mobile Safari/537.36";
            webView.CoreWebView2.Settings.UserAgent = UserAgent;

            // Set viewport dimensions to match Samsung Galaxy Note 20 Ultra
            await webView.CoreWebView2.ExecuteScriptAsync(@"
                Object.defineProperty(window, 'innerWidth', { get: () => 412 });
                Object.defineProperty(window, 'innerHeight', { get: () => 915 });
                Object.defineProperty(window.screen, 'width', { get: () => 412 });
                Object.defineProperty(window.screen, 'height', { get: () => 915 });
                Object.defineProperty(window.screen, 'devicePixelRatio', { get: () => 3.5 });
            ");

            // Inject viewport meta tag for responsive design
            await webView.CoreWebView2.ExecuteScriptAsync(@"
                const meta = document.createElement('meta');
                meta.name = 'viewport';
                meta.content = 'width=device-width, initial-scale=1.0';
                document.head.appendChild(meta);
            ");

        }
    }
}
