
using Microsoft.Web.WebView2.Core;
using MobileView.Core;
using MobileView.Core.Enums;
using System.ComponentModel;

namespace MobileView.Winforms.Utilities
{
    public class Client
    {
        public TitleBar titleBar { get; set; }
        public FormManager formManager { get; set; }
        public WV2Service WebService { get; set; }
        public List<string> _extensionsPaths { get; set; }
        public bool _incognito { get; set; }
        public bool _newWindow { get; set; }
        public string _url { get; set; }
        public Form MainForm { get; set; }
        private UIFramework UIFramework => UIFramework.Winforms;
        public SettingsManager Settings { get; set; }
        public object WebViewControl { get; set; }

        public Client(Form mainForm, object webViewControl)
        {
            MainForm = mainForm;
            WebViewControl = webViewControl;
            formManager = new FormManager(mainForm);
            Settings = new SettingsManager();

            WebService = new WV2Service(UIFramework, WebViewControl)
            {
                UserAgent = Settings.UserAgent
            };
            WebService.Extensions.EnsureDirectory();
            _extensionsPaths = WebService.Extensions.GetExtensionsPath();
        }
        public void InitializeBrowser()
        {
            WebService = new WV2Service(UIFramework, WebViewControl)
            {
                ProfileName = Settings.ProfileName,
                UserAgent = Settings.UserAgent
            };
            WebService.Extensions.ExtensionsPath = _extensionsPaths;
            WebService.PropertyChanged += WebView_PropertyChanged;
            WebService.NewWindowRequested += OnNewWindowRequested;
            WebService.InitializeBrowser();
        }
        public void InitializeNewWindow(string profileFolder)
        {
            if (profileFolder == null) return;

            _newWindow = true;
            WebService = new WV2Service(UIFramework, WebViewControl);
            WebService.UserAgent = Settings.UserAgent;
            WebService.PropertyChanged += WebView_PropertyChanged;
            WebService.NewWindowRequested += OnNewWindowRequested;
            WebService.InitializeNewTab(profileFolder);
        }
        public void InitializeIncognito()
        {
            List<string> ublock = _extensionsPaths.Where(dir => dir.Contains("uBlock")).ToList();
            WebService = new WV2Service(UIFramework, WebViewControl)
            {
                ProfileName = Settings.ProfileName,
                UserAgent = Settings.UserAgent
            };
            WebService.Extensions.ExtensionsPath = ublock;
            WebService.PropertyChanged += WebView_PropertyChanged;
            WebService.Incognito_InitializeWebView();
        }
        public void OpenNewWindow(string link)
        {
            using (Form newWindow = new Form_Main(currentForm: MainForm, url: link, profileFolder: WebService.ProfileFolder))
            {
                newWindow.Show();
            }
        }
        public async Task ViewExtensions()
        {
            List<string> extensions = await WebService.Extensions.GetExtensionsList();
            string extensionstring = string.Join(",\n", extensions);
            MessageBox.Show(extensionstring);
        }
        public async Task<List<ToolStripMenuItem>> GetFavorites()
        {
            List<ToolStripMenuItem> favorites = new List<ToolStripMenuItem>();
            Dictionary<string, string> fv = await WebService.History.GetFavoritesDict();
            
            if (fv == null)  return null; 

            foreach (var kvp in fv)
            {
                ToolStripMenuItem favoritesMenuItem = new ToolStripMenuItem(kvp.Key);
                favoritesMenuItem.Click += (sender, e) => WebService.Navigation.GoTo(kvp.Value);
                favorites.Add(favoritesMenuItem);
            }
            return favorites;
        }
        public async Task OnFormLoad()
        {
            if (_incognito) { await Task.Delay(1000); }
            if (!string.IsNullOrWhiteSpace(_url)) { WebService.Navigation.NewTabGoTo(_url); return; }
            WebService.Navigation.GoTo("www.google.com");
        }
        public async Task<bool> OnFormClose(Form form)
        {
            bool isClosing = false;
            if (_incognito)
                await WebService.Navigation.Incognito_DisposeSession();

            if (_incognito || _newWindow)
            {
                form.Hide();
                form.Dispose();
                return false;
            }
            return true;
        }

        public void Reload() => WebService.Navigation.Reload();
        public void Back() => WebService.Navigation.GoBack();
        public void GoTo(string URL) => WebService.Navigation.GoTo(URL);
        public void ClearAllBrowserData() => WebService.History.ClearAllBrowserData();
        public void ClearAllBrowsingData() => WebService.History.ClearAllBrowsingData();

        // Form Events / Controls
        private void OnNewWindowRequested(object? sender, CoreWebView2NewWindowRequestedEventArgs e) // custom event when a link new tab/window is requested
        {
            e.Handled = true;
            string url = e.Uri.ToString();
            OpenNewWindow(e.Uri);
            return;
        }
        private void WebView_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            //if (e.PropertyName == nameof(WebService.URL)) { URLTextBox.Text = WebService.URL; } //add a oneway binding to URL
        }
    }
}
