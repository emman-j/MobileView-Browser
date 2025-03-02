using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace WV2Service
{
    public partial class WebViewService
    {
        //private WV2ServiceModel _WebViewModel;
        private string _TempFolder { get; set; }
        private string _addExtensionsDirectory { get; set; }
        internal ClearManager Clear { get; }
        internal NavigationManager Navigation { get; }
        public event EventHandler<CoreWebView2NewWindowRequestedEventArgs> NewWindowRequested;
        public event EventHandler<string> NavigationChanged;

        public WebView2 WebViewControl { get; set; }
        public CoreWebView2Environment environment { get; set; }
        public CoreWebView2Profile Profile { get; set; }
        public List<string> ExtensionsPath { get; set; }
        public string SiteTitle { get; set; }
        public string ProfileName { get; set; }
        public string URL { get; set; }
        public string ProfileFolder { get; set; }
        public WebViewService() 
        {
            Clear = new ClearManager(this);
            Navigation = new NavigationManager(this);
        }
        public WebViewService(WebView2 _WebViewControl, string _ProfileName, List<string> _ExtensionsPath)
        {
            Clear = new ClearManager(this);
            Navigation = new NavigationManager(this);
            WebViewControl = _WebViewControl;
            ProfileName = _ProfileName;
            ExtensionsPath = _ExtensionsPath;
        }
        public WebViewService(CoreWebView2Profile _profile, string ProfileFolderPath) //SharedProfile e.g. 2 webcontrol 1 user profile
        {
            Clear = new ClearManager(this);
            Navigation = new NavigationManager(this);
            Profile = _profile;
            ProfileFolder = ProfileFolderPath;
        }
        public WebViewService(CoreWebView2Profile _profile, CoreWebView2Environment _environment) //SharedProfile e.g. 2 webcontrol 1 user profile
        {
            Clear = new ClearManager(this);
            Navigation = new NavigationManager(this);
            Profile = _profile;
            environment = _environment;
        }

    }
   
}
