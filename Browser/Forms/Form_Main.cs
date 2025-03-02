using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;
using System.ComponentModel;
using System.Xml.Serialization;
using WV2Service;
using MobileView.Classes;

namespace MobileView
{
    public partial class Form_Main : Form
    {
        private readonly TitleBar titleBar;
        private readonly FormManager formManager;
        private WebViewService Browser;
        private List<string> _extensionsPaths;
        private bool _incognito;
        private bool _newWindow;
        private string _url;

        public Form_Main(bool incognito = false, Form? currentForm = null, string? url = null, string? profileFolder = null)
        {
            InitializeComponent();

            _incognito = incognito;
            _url = url;
            Browser = new WebViewService();
            formManager = new FormManager(this);
            titleBar = new TitleBar
            (
                parentForm: this,
                panel: TitleBarPanel,
                formLabel: FormTextLabel,
                closeButton: CloseButton,
                minimizeButton: MinimizeButton
            );

            formManager.PreserveCurrentFormLocationAndSize(currentForm);
            EnableBorderlessWindows();
            Browser.EnsureExtensionsDirectory();
            _extensionsPaths = Browser.GetExtensionsPath();

            if (incognito)
            {
                InitializeIncognito();
            }
            else if (!string.IsNullOrWhiteSpace(_url))
            {
                InitializeNewWindow(profileFolder);
            }
            else
            {
                InitializeBrowser();
            }
        }

        // Methods
        private void InitializeBrowser()
        {
            Browser = new WebViewService
            {
                ProfileName = "User1",
                WebViewControl = WebView21,
                ExtensionsPath = _extensionsPaths
            };
            Browser.PropertyChanged += WebView_PropertyChanged;
            Browser.NewWindowRequested += OnNewWindowRequested;
            Browser.InitializeWebView();
            FormTextLabel.DataBindings.Add("Text", Browser, nameof(Browser.SiteTitle));
        }
        private void InitializeNewWindow(string profileFolder)
        {
            if (profileFolder == null) { return; }
            _newWindow = true;
            MenuButton.Visible = false;
            URLTextBox.Size = new Size(252, 23);
            Browser = new WebViewService();
            Browser.WebViewControl = WebView21;
            Browser.PropertyChanged += WebView_PropertyChanged;
            Browser.NewWindowRequested += OnNewWindowRequested;
            Browser.InitializeWebViewNewTab(profileFolder);
            FormTextLabel.DataBindings.Add("Text", Browser, nameof(Browser.SiteTitle));
        }
        private void InitializeIncognito()
        {
            FormTextLabel.Text = "Private";
            List<string> ublock = _extensionsPaths.Where(dir => dir.Contains("uBlock0")).ToList();

            Browser = new WebViewService
            {
                ProfileName = "User1",
                WebViewControl = WebView21,
                ExtensionsPath = ublock
            };
            Browser.PropertyChanged += WebView_PropertyChanged;
            Browser.Incognito_InitializeWebView();
        }
        private void EnableBorderlessWindows()
        {
            this.MaximizedBounds = Screen.FromHandle(this.Handle).WorkingArea;
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            this.DoubleBuffered = true;
            this.ControlBox = false;
        }
        private void OpenNewWindow(string link)
        {
            Form newWindow = new Form_Main(currentForm: this, url: link, profileFolder: Browser.ProfileFolder);
            newWindow.Show();
        }
        private async void ViewExtensions()
        {
            List<string> extensions = await Browser.GetExtensionsList();
            string extensionstring = string.Join(",\n", extensions);
            MessageBox.Show(extensionstring);
        }
        private async void GetFavorites()
        {
            Dictionary<string, string> favorites = await Browser.GetFavoritesDict();
            if (favorites == null) { return; }
            foreach (var kvp in favorites)
            {
                ToolStripMenuItem favoritesMenuItem = new ToolStripMenuItem(kvp.Key);
                favoritesMenuItem.Click += (sender, e) => Browser.Navigation.GoTo(kvp.Value);
                favoritesToolStripMenuItem.DropDownItems.Add(favoritesMenuItem);
            }
        }
        private async void OnFormLoad()
        {
            if (_incognito) { await Task.Delay(1000); }
            if (!string.IsNullOrWhiteSpace(_url)) { Browser.Navigation.NewTabGoTo(_url); return; }
            GetFavorites();
            Browser.Navigation.GoTo("www.google.com");
        }

        // This method overrides WndProc to pass specific window messages (e.g., WM_NCHITTEST)
        // to the FormManager for handling.
        protected override void WndProc(ref Message m)
        {
            const int WM_NCHITTEST = 0x84;

            base.WndProc(ref m);

            if (m.Msg == WM_NCHITTEST)
            {
                formManager.HandleWndProc(ref m);
            }
        }

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
            if (e.PropertyName == nameof(Browser.URL)) { URLTextBox.Text = Browser.URL; } //add a oneway binding to URL
        }
        private void Form_Main_Load(object sender, EventArgs e)
        {
            OnFormLoad();
        }
        private void Form_Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_incognito)
            {
                Browser.Navigation.Incognito_DisposeSession();
            }
            if (_incognito || _newWindow)
            {
                this.Hide();
                this.Dispose();
                return;
            }
            Application.Exit();
        }
        private void MenuButton_Click(object sender, EventArgs e)
        {
            MenuPanel.Visible = !MenuPanel.Visible;
        }
        private void ReloadButton_Click(object sender, EventArgs e)
        {
            Browser.Navigation.Reload();
        }
        private void BackButton_Click(object sender, EventArgs e)
        {
            Browser.Navigation.GoBack();
        }
        private void URLTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                WebView21.Focus();
                Browser.Navigation.GoTo(URLTextBox.Text);
            }
        }
        private void URLTextBox_DoubleClick(object sender, EventArgs e)
        {
            TextBox textBox = sender as TextBox;
            textBox.Focus();
            textBox.SelectAll();
        }

        // Menu Strip
        private void ClearAllBrowserDataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Browser.Clear.AllBrowserData();
        }
        private void ClearAllBrowsingDataToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Browser.Clear.AllBrowsingData();
        }
        private void IncognitoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form incognito = new Form_Main(incognito: true, currentForm: this);
            this.Hide();
            incognito.ShowDialog();
            formManager.PreserveCurrentFormLocationAndSize(incognito);
            this.Show();
        }
        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void ViewExtensionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ViewExtensions();
        }
        private void RemoveExtensionToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
        private void AddExtensionToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
        private void ViewHistoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (Form_HistoryManager historyManager = new Form_HistoryManager(Browser, this))
            {
                this.Hide();
                historyManager.ShowDialog();
                formManager.PreserveCurrentFormLocationAndSize(historyManager);
            }
            this.Show();
            MenuButton.PerformClick();
            Browser.WebViewControl.Focus();
        }
    }
}
