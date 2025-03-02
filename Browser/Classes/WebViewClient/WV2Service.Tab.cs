using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;
using MobileView;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using static WV2Service.WebViewService;

namespace WV2Service.Tab
{
    public class WV2Tab : INotifyPropertyChanged
    {
        internal WebViewService WV2Service;
        internal ClearManager Clear { get; }
        internal NavigationManager Navigation { get; }
        public WebView2 WV2Control
        { 
           get { return WV2Service.WebViewControl; }
           set
            {
                if (WV2Service.WebViewControl != value)
                {
                    WV2Service.WebViewControl = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public CoreWebView2Environment environment
        {
            get { return WV2Service.environment; }
            set
            {
                if (WV2Service.environment != value)
                {
                    WV2Service.environment = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public CoreWebView2Profile Profile
        {
            get { return WV2Service.Profile; }
            set
            {
                if (WV2Service.Profile != value)
                {
                    WV2Service.Profile = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public List<string> ExtensionsPath 
        {
            get { return WV2Service.ExtensionsPath; }
            set
            {
                if (WV2Service.ExtensionsPath != value)
                {
                    WV2Service.ExtensionsPath = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public string SiteTitle
        {
            get { return WV2Service.SiteTitle; }
            set
            {
                if (WV2Service.SiteTitle != value)
                {
                    WV2Service.SiteTitle = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public string ProfileName
        {
            get { return WV2Service.ProfileName; }
            set
            {
                if (WV2Service.ProfileName != value)
                {
                    WV2Service.ProfileName = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public string URL
        {
            get { return WV2Service.URL; }
            set
            {
                if (WV2Service.URL != value)
                {
                    WV2Service.URL = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public string ProfileFolder
        {
            get { return WV2Service.ProfileFolder; }
            set
            {
                if (WV2Service.ProfileFolder != value)
                {
                    WV2Service.ProfileFolder = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public WV2Tab(WebView2 WebViewControl, string profileName)
        {
            WV2Service = new WebViewService();
            Clear = WV2Service.Clear;
            Navigation = WV2Service.Navigation;
            WV2Service.EnsureExtensionsDirectory();
            ExtensionsPath = WV2Service.GetExtensionsPath();
            WV2Service.ProfileName = profileName;
            WV2Service.WebViewControl = WebViewControl;
            WV2Service.ExtensionsPath = ExtensionsPath;
            //Browser.PropertyChanged += WebView_PropertyChanged;
            WV2Service.NewWindowRequested += OnNewWindowRequested;
            
            WV2Service.InitializeWebView();
        }


        public WV2Tab(Form form, WebView2 WebViewControl, string profileFolder, string url)
        {
            WV2Service = new WebViewService();
            WV2Service.EnsureExtensionsDirectory();
            WV2Service.WebViewControl = WebViewControl;
            URL = url;
            //Browser.PropertyChanged += WebView_PropertyChanged;
            WV2Service.NewWindowRequested += OnNewWindowRequested;
            WV2Service.InitializeWebViewNewTab(profileFolder);
            WV2Service.Navigation.NewTabGoTo(url);
        }
        private void OnNewWindowRequested(object? sender, CoreWebView2NewWindowRequestedEventArgs e)
        {
            e.Handled = true;
            string url = e.Uri.ToString();
            OpenNewWindow(e.Uri);
            return;
        }

        private void OpenNewWindow(string link)
        {
            //Form newWindow = new Form_Main(currentForm: this, url: link, profileFolder: Browser.ProfileFolder);
            //newWindow.Show();
        }
        public void NotifyPropertyChanged([CallerMemberName] string propertyname = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyname));
        }
        public override string ToString()
        {
            return SiteTitle;
        }
    }
}
