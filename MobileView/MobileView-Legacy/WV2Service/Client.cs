using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;
using MobileView.WV2Service.Data;
using System.ComponentModel;
namespace MobileView.WV2Service;

public class Client
{
    public WV2Service BrowserService { get; set; }
    public NavigationManager Navigation { get; }
    public BrowsingDataManager DataManager { get; }

    public event PropertyChangedEventHandler? PropertyChanged;

    public Client()
    {
        BrowserService = new WV2Service();
        DataManager = new BrowsingDataManager(BrowserService);
        Navigation = new NavigationManager(BrowserService);
    }
    public Client(WebView2 webViewControl, string profileName, List<string> extensionsPath)
    {
        BrowserService = new WV2Service()
        { 
            Browser = webViewControl,
            ProfileName = profileName,
            ExtensionsPath = extensionsPath,
        };
        DataManager = new BrowsingDataManager(BrowserService);
        Navigation = new NavigationManager(BrowserService);
    }
    public Client(WebView2 webViewControl, string profileName, List<string> extensionsPath, string userAgent, List<string> validSuffixes)
    {
        BrowserService = new WV2Service()
        {
            Browser = webViewControl,
            ProfileName = profileName,
            ExtensionsPath = extensionsPath,
            UserAgent = userAgent,
        };
        DataManager = new BrowsingDataManager(BrowserService);
        Navigation = new NavigationManager(BrowserService);
        Navigation.validUrlSuffixes = validSuffixes;
    }
    public Client(CoreWebView2Profile profile, string profileFolderPath) //SharedProfile e.g. 2 webcontrol 1 user profile
    {
        BrowserService = new WV2Service()
        {
            Profile = profile,
            ProfileFolder = profileFolderPath,
        };
        DataManager = new BrowsingDataManager(BrowserService);
        Navigation = new NavigationManager(BrowserService);
    }
    public Client(CoreWebView2Profile profile, CoreWebView2Environment environment) //SharedProfile e.g. 2 webcontrol 1 user profile
    {
        BrowserService = new WV2Service()
        {
            Profile = profile,
            Environment = environment,
        };
        DataManager = new BrowsingDataManager(BrowserService);
        Navigation = new NavigationManager(BrowserService);
    }
    private void InitializeBrowser()
    {
        InitializeProfile();
        EnableMobileView();
        InitializeExtensions();
        EnableNewWindowRequest();
        EnableNavigationMonitoring();
    }
    public static void EnsureLocalHistoryDB(string profileName)
    {
        BrowserDatabase.DbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "WebControl", "profiles",
            profileName, "LocalHistory", "browser.db");
        BrowserDatabase.EnsureCreated();
    }

    public void InitializeWebView()
    {
        InitializeEnviroment();
        InitializeBrowser();
    }
    public void InitializeWebViewNewTab(string ProfileFolder)
    {
        InitializeSharedEnviroment(ProfileFolder);
        InitializeProfile();
        EnableMobileView();
        EnableNewWindowRequest();
        EnableNavigationMonitoring();
    }
    public void Incognito_InitializeWebView()
    {
        Incognito_InitializeEnviroment();
        InitializeBrowser();
    }
    public async void InitializeEnviroment()
    {
        BrowserService.Environment = await BrowserService.InitializeWebEnviromentAsync(BrowserService.ProfileName);
    }
    public async void InitializeSharedEnviroment(string profileFolder)
    {
        BrowserService.ProfileFolder = profileFolder;
        BrowserService.Environment = await BrowserService.InitializeSharedWebEnviromentAsync(profileFolder);
    }
    public async void InitializeProfile()
    {
        BrowserService.Profile = await BrowserService.GetProfile();
    }
    public async void Incognito_InitializeEnviroment()
    {
        BrowserService.Environment = await BrowserService.Incognito_InitializeWebEnviromentAsync("Incognito");
    }
    public void EnableMobileView()
    {
        BrowserService.EnableMobileView();
    }
    public void InitializeExtensions()
    {
        BrowserService.InitializeExtensions();
    }
    public async void EnableNewWindowRequest()
    {
        await Navigation.EnableNewWindowRequest();
    }
    public async void EnableNavigationMonitoring()
    {
        await Navigation.EnableNavigationMonitoring();
        Navigation.PageVisited += async (s, e) => await DataManager.RecordVisit(e.Url, e.Title, e.Transition);
    }


}
