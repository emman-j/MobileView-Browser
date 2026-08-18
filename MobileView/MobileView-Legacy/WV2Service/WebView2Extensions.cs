using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;
using System.Diagnostics;
namespace MobileView.WV2Service;
public static class WebView2Extensions
{
    public static async Task<CoreWebView2Profile> GetProfile(this IWV2Service service)
    {
        try
        {
            WebView2 webView = service.Browser;
            await webView.EnsureCoreWebView2Async(service.Environment);
            return webView.CoreWebView2.Profile;
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to get profile:\n{ex.Message}");
        }
    }
    public static async Task<CoreWebView2Environment> InitializeWebEnviromentAsync(this IWV2Service service, string profileName)
    {
        try
        {
            WebView2 webView = service.Browser;

            // Path if the desired directory is in the locap appdata dir. I prefer it be in the base dir of the app itself
            //string userDataFolder = Path.Combine( Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "MyWebView2AppData", profileName);
            string appBaseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            service.ProfileFolder = Path.Combine(appBaseDirectory, "WebControl", "profiles", profileName);

            CoreWebView2EnvironmentOptions environmentOptions = new CoreWebView2EnvironmentOptions { AreBrowserExtensionsEnabled = true };
            CoreWebView2Environment environment = await CoreWebView2Environment.CreateAsync(null, service.ProfileFolder, environmentOptions);
            await webView.EnsureCoreWebView2Async(environment);
            return environment;
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to initialize environment:\n{ex.Message}");
        }
    }
    public static async Task<CoreWebView2Environment> Incognito_InitializeWebEnviromentAsync(this IWV2Service service, string profileName)
    {
        try
        {
            WebView2 webView = service.Browser;
            service.TempFolder = Path.Combine(Path.GetTempPath(), "EBWebView", "Incognito_" + Guid.NewGuid().ToString());
            Directory.CreateDirectory(service.TempFolder);

            CoreWebView2EnvironmentOptions environmentOptions = new CoreWebView2EnvironmentOptions { AreBrowserExtensionsEnabled = true };

            CoreWebView2Environment environment = await CoreWebView2Environment.CreateAsync(null, service.TempFolder, environmentOptions);

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
    public static async Task<CoreWebView2Environment> InitializeSharedWebEnviromentAsync(this IWV2Service service, string FolderPath)
    {
        try
        {
            WebView2 webView = service.Browser;
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
    public static async void EnableMobileView(this IWV2Service service)
    {
        WebView2 webView = service.Browser;
        await webView.EnsureCoreWebView2Async(service.Environment);
        //webView.CoreWebView2.Settings.UserAgent = @"Mozilla/5.0 (Linux; Android 10; Mobile) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/108.0.0.0 Mobile Safari/537.36";
        webView.CoreWebView2.Settings.UserAgent = service.UserAgent;

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


    public static async void InitializeExtensions(this IWV2Service service)
    {
        WebView2 webView = service.Browser;
        if (service.ExtensionsPath == null || !service.ExtensionsPath.Any()) { return; }

        int count = 0;
        foreach (string originalExtensionPath in service.ExtensionsPath)
        {
            if (!Directory.Exists(originalExtensionPath))
            { count++; continue; }
        }
        if (count == service.ExtensionsPath.Count()) { return; }

        await webView.EnsureCoreWebView2Async(service.Environment);
        CoreWebView2BrowserExtension extension = await AddExtensionsAsync(service);
        await extension.EnableAsync(true);
    }
    public static async Task<CoreWebView2BrowserExtension> AddExtensionsAsync(this IWV2Service service)
    {
        try
        {
            WebView2 webView = service.Browser;
            CoreWebView2BrowserExtension extension = null;

            string localExtensionsPath = (!string.IsNullOrWhiteSpace(service.TempFolder)) ?
                Path.Combine(service.TempFolder, "EBWebView", "Default", "Extensions_Local") :
                Path.Combine(service.ProfileFolder, "EBWebView", "Default", "Extensions_Local");

            if (!Directory.Exists(localExtensionsPath))
            {
                Directory.CreateDirectory(localExtensionsPath);
            }

            foreach (string originalExtensionPath in service.ExtensionsPath)
            {

                string extensionName = Path.GetFileName(originalExtensionPath);

                string localExtensionPath = Path.Combine(localExtensionsPath, extensionName);

                if (!Directory.Exists(localExtensionPath))
                {
                    CopyDirectory(originalExtensionPath, localExtensionPath);
                }

                CoreWebView2Profile profile = await GetProfile(service);
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
    public static async Task<CoreWebView2BrowserExtension> AddExtensionsAsync(this IWV2Service service, string extensionPath)
    {
        try
        {
            WebView2 webView = service.Browser;
            CoreWebView2BrowserExtension extension = null;

            string localExtensionsPath = (!string.IsNullOrWhiteSpace(service.TempFolder)) ?
                Path.Combine(service.TempFolder, "EBWebView", "Default", "Extensions_Local") :
                Path.Combine(service.ProfileFolder, "EBWebView", "Default", "Extensions_Local");

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

            CoreWebView2Profile profile = await GetProfile(service);
            extension = await profile.AddBrowserExtensionAsync(localExtensionPath);

            return extension;
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to load extensions:\n{ex.Message}");
        }
    }
    public static async Task<IReadOnlyList<CoreWebView2BrowserExtension>> GetBrowserExtensionsListAsync(this IWV2Service service)
    {
        try
        {
            WebView2 webView = service.Browser;
            CoreWebView2Profile profile = await GetProfile(service);
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
    private static void CopyDirectory(string sourceDir, string destinationDir)
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
    public static void EnsureExtensionsDirectory(this IWV2Service service) // added to allow easy installation of extensions for now
    {
        string appBaseDirectory = AppDomain.CurrentDomain.BaseDirectory;
        service.ExtensionsFolder = Path.Combine(appBaseDirectory, "Extensions_Local");

        if (!Directory.Exists(service.ExtensionsFolder))
            Directory.CreateDirectory(service.ExtensionsFolder);
    }
    public static async Task<List<string>> GetExtensionsList(this IWV2Service service)
    {
        List<string> extensions = new List<string>();

        IReadOnlyList<CoreWebView2BrowserExtension> extensionsList = await service.GetBrowserExtensionsListAsync();

        foreach (var extension in extensionsList)
        {
            extensions.Add(extension.Name);
            Console.WriteLine($"- {extension.Name}, ID: {extension.Id}");
        }

        return extensions;
    }
    public static List<string> GetExtensionsPath(this IWV2Service service)
    {
        string[] subDirectories = Directory.GetDirectories(service.ExtensionsFolder);

        List<string> pathList = new List<string>();

        foreach (string subDir in subDirectories)
        {
            pathList.Add(subDir);
        }

        return pathList;
    }


    public static async Task ClearAllBrowsingData(this IWV2Service service)
    {
        try
        {
            WebView2 webView = service.Browser;
            CoreWebView2Profile profile = await GetProfile(service);
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
    public static async Task ClearBrowsingDataBetweenDateRange(this IWV2Service service, DateTime startDate, DateTime endDate)
    {
        try
        {
            WebView2 webView = service.Browser;
            CoreWebView2Profile profile = await GetProfile(service);
            await service.Profile.ClearBrowsingDataAsync(
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
    public static async Task ClearBrowserData(this IWV2Service service, CoreWebView2BrowsingDataKinds dataKinds)
    {
        try
        {
            WebView2 webView = service.Browser;
            CoreWebView2Profile profile = await GetProfile(service);
            await profile.ClearBrowsingDataAsync(dataKinds);
            webView.Reload();
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to clear data:\n{ex.Message}");
        }
    }
    public static async Task ClearAllBrowserData(IWV2Service service)
    {
        WebView2 webView = service.Browser;
        CoreWebView2Profile profile = await GetProfile(service);
        await webView.EnsureCoreWebView2Async(service.Environment);
        await profile.ClearBrowsingDataAsync();
        webView.Reload();
    }
}
