using Microsoft.Web.WebView2.WinForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using WV2Service.Tab;
using static WV2Service.WebViewService;

namespace WV2Service.Client
{
    public class WV2Client
    {
        internal ClearManager Clear;
        internal NavigationManager Navigation;

        private WebViewService WV2Service;
        private WV2TabCollection Tabs;
        private WV2Tab Tab;
        private Form ParentForm;
        private List<string> _extensionsPaths;
        private string _profileFolder;
        private bool _incognito;

        public WV2Client(Form form, WebView2 webcontrol, string profileName)
        {
            ParentForm = form;
            //WV2Service = new WebViewService
            //{
            //    ProfileName = profileName,
            //    WebViewControl = webcontrol,
            //};
            //WV2Service.EnsureExtensionsDirectory();
            //WV2Service.ExtensionsPath = _extensionsPaths = WV2Service.GetExtensionsPath();
            //Browser.PropertyChanged += WebView_PropertyChanged;
            //WV2Service.NewWindowRequested += OnNewWindowRequested;
            Tab = new WV2Tab(webcontrol, profileName);
            Clear = Tab.Clear;
            Navigation = Tab.Navigation;
            WV2Service = Tab.WV2Service;
            ParentForm.Load += ParentForm_Load;
        }
        private async void ParentForm_Load(object? sender, EventArgs e)
        {
            if (_incognito) { await Task.Delay(1000); }
            if (!string.IsNullOrWhiteSpace(URL)) { WV2Service.Navigation.NewTabGoTo(URL); return; }
            GetFavorites();
            webViewService.Navigation.GoTo("www.google.com");
        }

        public async void Form_Load(object sender, EventArgs e)
        {

        }
    }
}
