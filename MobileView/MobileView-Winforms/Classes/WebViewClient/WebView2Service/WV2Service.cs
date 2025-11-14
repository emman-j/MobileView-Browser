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

        public WebView2 WebViewControl
        {
            get { return _WebViewModel.WebviewControl; }
            set
            {
                if (_WebViewModel.WebviewControl != value)
                {
                    _WebViewModel.WebviewControl = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public CoreWebView2Environment environment
        {
            get { return _WebViewModel.Environment; }
            set
            {
                if (_WebViewModel.Environment != value)
                {
                    _WebViewModel.Environment = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public CoreWebView2Profile Profile
        {
            get { return _WebViewModel.Profile; }
            set
            {
                if (_WebViewModel.Profile != value)
                {
                    _WebViewModel.Profile = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public List<string> ExtensionsPath
        {
            get => _WebViewModel.ExtensionsPath;
            set
            {
                if (_WebViewModel.ExtensionsPath != value)
                {
                    _WebViewModel.ExtensionsPath = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public string SiteTitle
        {
            get => _WebViewModel.SiteTitle;
            set
            {
                if (_WebViewModel.SiteTitle != value)
                {
                    _WebViewModel.SiteTitle = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public string ProfileName
        {
            get => _WebViewModel.ProfileName;
            set
            {
                if (_WebViewModel.ProfileName != value)
                {
                    _WebViewModel.ProfileName = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public string URL
        {
            get => _WebViewModel.URL;
            set
            {
                if (_WebViewModel.URL != value)
                {
                    _WebViewModel.URL = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public string ProfileFolder
        {
            get { return _WebViewModel.ProfileFolder; }
            set
            {
                if (_WebViewModel.ProfileFolder != value)
                {
                    _WebViewModel.ProfileFolder = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public string UserAgent
        {
            get { return _WebViewModel.UserAgent; }
            set
            {
                if (_WebViewModel.UserAgent != value)
                {
                    _WebViewModel.UserAgent = value;
                    NotifyPropertyChanged();
                }
            }
        }
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
