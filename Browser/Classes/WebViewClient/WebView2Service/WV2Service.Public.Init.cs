using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;

namespace WV2Service
{
    public partial class WebViewService
    {
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
            environment = await InitializeWebEnviromentAsync(WebViewControl, ProfileName);
        }
        public async void InitializeSharedEnviroment(string profileFolder)
        {
            ProfileFolder = profileFolder;
            environment = await InitializeSharedWebEnviromentAsync(WebViewControl, profileFolder);
        }
        public async void InitializeProfile()
        {
            Profile = await GetProfile(WebViewControl, environment);
        }
        public async void Incognito_InitializeEnviroment()
        {
            environment = await Incognito_InitializeWebEnviromentAsync(WebViewControl, "Incognito");
        }
        public void EnableMobileView()
        {
            EnableMobileView(WebViewControl, environment);
        }
        public void InitializeExtensions()
        {
            InitializeExtensions(WebViewControl, environment, Profile);
        }
        public void EnableNewWindowRequest()
        {
            EnableNewWindowRequest(WebViewControl, environment);
        }
        public void EnableNavigationMonitoring()
        {
            EnableNavigationMonitoring(WebViewControl, environment);
        }
    }
}
