using Microsoft.Web.WebView2.Core;
using MobileView.Core;
using MobileView.Core.Enums;
using MobileView.Winforms.Utilities;
using System.ComponentModel;

namespace MobileView.Winforms
{
    public partial class Form_Main : Form
    {
        private Client _client;

        public Form_Main(bool incognito = false, Form? currentForm = null, string? url = null, string? profileFolder = null)
        {
            InitializeComponent();
            _client = new Client(this, WebView21);
            _client.titleBar = new TitleBar
            (
                parentForm: this,
                panel: TitleBarPanel,
                formLabel: FormTextLabel,
                closeButton: CloseButton,
                minimizeButton: MinimizeButton
            );
            _client._incognito = incognito;
            _client._url = url;

            _client.formManager.PreserveCurrentFormLocationAndSize(currentForm);
            EnableBorderlessWindows();

            if (incognito)
            {
                FormTextLabel.Text = "Private";
                _client.InitializeIncognito();
                _client.WebService.PropertyChanged += WebView_PropertyChanged;
            }

            else if (!string.IsNullOrWhiteSpace(_client._url))
            {
                MenuButton.Visible = false;
                URLTextBox.Size = new Size(252, 23);
                _client.InitializeNewWindow(profileFolder);
                _client.WebService.PropertyChanged += WebView_PropertyChanged;
                FormTextLabel.DataBindings.Add("Text", _client.WebService, nameof(_client.WebService.SiteTitle));
            }
            else
            {
                _client.InitializeBrowser();
                _client.WebService.PropertyChanged += WebView_PropertyChanged;
                FormTextLabel.DataBindings.Add("Text", _client.WebService, nameof(_client.WebService.SiteTitle));
            }
        }

        private void EnableBorderlessWindows()
        {
            this.MaximizedBounds = Screen.FromHandle(this.Handle).WorkingArea;
            this.SetStyle(ControlStyles.ResizeRedraw, true);
            this.DoubleBuffered = true;
            this.ControlBox = false;
        }

        // This method overrides WndProc to pass specific window messages (e.g., WM_NCHITTEST)
        // to the FormManager for handling.
        protected override void WndProc(ref Message m)
        {
            const int WM_NCHITTEST = 0x84;

            base.WndProc(ref m);

            if (m.Msg == WM_NCHITTEST)
                _client.formManager.HandleWndProc(ref m);
        }

        private void WebView_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(_client.WebService.URL)) { URLTextBox.Text = _client.WebService.URL; } //add a oneway binding to URL
        }
        private async void Form_Main_Load(object sender, EventArgs e)
        {
            if (!_client._incognito) 
            foreach (ToolStripMenuItem item in await _client.GetFavorites())
                favoritesToolStripMenuItem.DropDownItems.Add(item);

            await _client.OnFormLoad();
        }
        private async void Form_Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            bool isClosing = await _client.OnFormClose(this);

            if (isClosing) 
                Application.Exit();
        }
        private void MenuButton_Click(object sender, EventArgs e)
        {
            MenuPanel.Visible = !MenuPanel.Visible;
        }
        private void ReloadButton_Click(object sender, EventArgs e) => _client.Reload();
        private void BackButton_Click(object sender, EventArgs e) => _client.Back();
        private void URLTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                WebView21.Focus();
                _client.GoTo(URLTextBox.Text);
            }
        }
        private void URLTextBox_DoubleClick(object sender, EventArgs e)
        {
            TextBox textBox = sender as TextBox;
            textBox.Focus();
            textBox.SelectAll();
        }

        // Menu Strip
        private void ClearAllBrowserDataToolStripMenuItem_Click(object sender, EventArgs e) => _client.ClearAllBrowserData();
        private void ClearAllBrowsingDataToolStripMenuItem1_Click(object sender, EventArgs e) => _client.ClearAllBrowsingData();
        private void IncognitoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (Form incognito = new Form_Main(incognito: true, currentForm: this))
            {
                this.Hide();
                incognito.ShowDialog();
                _client.formManager.PreserveCurrentFormLocationAndSize(incognito);
            }
            this.Show();
        }
        private void ExitToolStripMenuItem_Click(object sender, EventArgs e) => Close();
        private async void ViewExtensionsToolStripMenuItem_Click(object sender, EventArgs e) => await _client.ViewExtensions();
        private void RemoveExtensionToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
        private void AddExtensionToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
        private void ViewHistoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (Form_HistoryManager historyManager = new Form_HistoryManager(_client.WebService, this))
            {
                this.Hide();
                historyManager.ShowDialog();
                _client.formManager.PreserveCurrentFormLocationAndSize(historyManager);
            }
            this.Show();
            MenuButton.PerformClick();
            _client.WebService.WebControl.Focus();
        }
    }
}
