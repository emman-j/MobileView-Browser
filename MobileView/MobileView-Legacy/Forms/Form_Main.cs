using Microsoft.Web.WebView2.Core;
using MobileView.Classes;
using MobileView.Forms;
using MobileView.WV2Service;
using MobileView.WV2Service.Data;
using System.ComponentModel;
namespace MobileView;

public partial class Form_Main : Form
{
    private static string _userAgent = Properties.Settings.Default.UserAgent;
    private static List<string> _validUrlPSuffxes = Properties.Settings.Default.ValidUrlSuffix.Cast<string>().ToList();
    private readonly TitleBar titleBar;
    private readonly FormManager formManager;
    //private WebViewService Browser;
    private List<string> _extensionsPaths;
    private bool _incognito;
    private bool _newWindow;
    private string _url;
    private List<Form> _windows = new List<Form>();

    private Client wv2Client;

    public Form_Main(bool incognito = false, Form? currentForm = null, string? url = null, string? profileFolder = null)
    {
        InitializeComponent();

        _incognito = incognito;
        _url = url;
        wv2Client = new Client();
        formManager = new FormManager(this);
        titleBar = new TitleBar
        (
            parentForm: this,
            panel: TitleBarPanel,
            formLabel: FormTextLabel,
            closeButton: CloseButton,
            minimizeButton: MinimizeButton
        );
        wv2Client.BrowserService.UserAgent = _userAgent;
        formManager.PreserveCurrentFormLocationAndSize(currentForm);
        EnableBorderlessWindows();
        wv2Client.BrowserService.EnsureExtensionsDirectory();
        _extensionsPaths = wv2Client.BrowserService.GetExtensionsPath();

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
        wv2Client = new Client
        (
            webViewControl: WebView21,
            profileName: "User1",
            extensionsPath: _extensionsPaths,
            userAgent: _userAgent,
            validSuffixes: _validUrlPSuffxes
        );
        Client.EnsureLocalHistoryDB(wv2Client.BrowserService.ProfileName);
        wv2Client.BrowserService.PropertyChanged += WebView_PropertyChanged;
        wv2Client.Navigation.NewWindowRequested += OnNewWindowRequested;
        wv2Client.InitializeWebView();
        FormTextLabel.DataBindings.Add("Text", wv2Client.BrowserService, nameof(wv2Client.BrowserService.SiteTitle));
    }
    private void InitializeNewWindow(string profileFolder)
    {
        if (profileFolder == null) { return; }
        _newWindow = true;
        MenuButton.Visible = false;
        SearchComboBox.Size = new Size(252, 23);
        wv2Client = new Client();
        wv2Client.BrowserService.UserAgent = _userAgent;
        wv2Client.BrowserService.Browser = WebView21;
        wv2Client.Navigation.validUrlSuffixes = _validUrlPSuffxes;
        wv2Client.BrowserService.PropertyChanged += WebView_PropertyChanged;
        wv2Client.Navigation.NewWindowRequested += OnNewWindowRequested;
        wv2Client.InitializeWebViewNewTab(profileFolder);
        FormTextLabel.DataBindings.Add("Text", wv2Client.BrowserService, nameof(wv2Client.BrowserService.SiteTitle));
    }
    private void InitializeIncognito()
    {
        FormTextLabel.Text = "Private";
        List<string> ublock = _extensionsPaths.Where(dir => dir.Contains("uBlock0")).ToList();

        wv2Client = new Client
        (
            webViewControl: WebView21,
            profileName: "User1",
            extensionsPath: _extensionsPaths,
            userAgent: _userAgent,
            validSuffixes: _validUrlPSuffxes
        );
        wv2Client.BrowserService.PropertyChanged += WebView_PropertyChanged;
        wv2Client.InitializeWebView();
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
        Form newWindow = new Form_Main(currentForm: this, url: link, profileFolder: wv2Client.BrowserService.ProfileFolder);
        newWindow.FormClosed += NewWindow_FormClosed;
        _windows.Add(newWindow);
        newWindow.Show();
    }

    private void NewWindow_FormClosed(object? sender, FormClosedEventArgs e)
    {
        if (sender is Form_Main main)
        {
            if (_windows.Contains(main))
                _windows.Remove(main);
        }
    }

    private async void ViewExtensions()
    {
        List<string> extensions = await wv2Client.BrowserService.GetExtensionsList();
        string extensionstring = string.Join(",\n", extensions);
        MessageBox.Show(extensionstring);
    }
    private async void GetFavorites()
    {
        List<HistoryEntry> favorites = await wv2Client.DataManager.GetFavorites();
        if (favorites == null) { return; }
        foreach (HistoryEntry emtry in favorites)
        {
            ToolStripMenuItem favoritesMenuItem = new ToolStripMenuItem(emtry.Title);
            favoritesMenuItem.Click += (sender, e) => wv2Client.Navigation.GoTo(emtry.Url);
            favoritesToolStripMenuItem.DropDownItems.Add(favoritesMenuItem);
        }
    }
    private async void GetBookmarks()
    {
        bookmarksToolStripMenuItem.DropDownItems.Clear();

        ToolStripMenuItem addBookmarkItem = new ToolStripMenuItem("&Add Bookmark");
        addBookmarkItem.Click += addNewBookmarkToolStripMenuItem_Click;
        bookmarksToolStripMenuItem.DropDownItems.Add(addBookmarkItem);

        List<BookmarkEntry> bookmarks = await wv2Client.DataManager.GetBookmarks();
        if (bookmarks == null) { return; }

        // path -> submenu, so nested folders reuse the same menu instead of duplicating
        var folderMenus = new Dictionary<string, ToolStripMenuItem>();

        ToolStripMenuItem GetOrCreateFolderMenu(string folderPath)
        {
            if (folderMenus.TryGetValue(folderPath, out ToolStripMenuItem existing))
                return existing;

            string[] parts = folderPath.Split(" / ", StringSplitOptions.RemoveEmptyEntries);
            ToolStripItemCollection currentLevel = bookmarksToolStripMenuItem.DropDownItems;
            string accumulatedPath = "";

            foreach (string part in parts)
            {
                accumulatedPath = accumulatedPath.Length == 0 ? part : accumulatedPath + " / " + part;

                if (!folderMenus.TryGetValue(accumulatedPath, out ToolStripMenuItem folderMenu))
                {
                    folderMenu = new ToolStripMenuItem(part);
                    currentLevel.Add(folderMenu);
                    folderMenus[accumulatedPath] = folderMenu;
                }
                currentLevel = folderMenu.DropDownItems;
            }

            return folderMenus[folderPath];
        }

        foreach (BookmarkEntry entry in bookmarks)
        {
            ToolStripMenuItem bookmarkItem = new ToolStripMenuItem(entry.Title);
            bookmarkItem.Click += (sender, e) => wv2Client.Navigation.GoTo(entry.Url);

            ToolStripMenuItem removeItem = new ToolStripMenuItem("Remove");
            removeItem.Click += async (sender, e) =>
            {
                await wv2Client.DataManager.RemoveBookmark(entry.Id);
                GetBookmarks(); // rebuild the menu so it disappears immediately
            };
            bookmarkItem.DropDownItems.Add(removeItem);

            if (string.IsNullOrWhiteSpace(entry.FolderPath))
            {
                bookmarksToolStripMenuItem.DropDownItems.Add(bookmarkItem);
            }
            else
            {
                ToolStripMenuItem parentFolder = GetOrCreateFolderMenu(entry.FolderPath);
                parentFolder.DropDownItems.Add(bookmarkItem);
            }
        }
    }


    private void addNewBookmarkToolStripMenuItem_Click(object sender, EventArgs e)
    {
        using var dlg = new Bookmark_Form(wv2Client, wv2Client.BrowserService.URL, wv2Client.BrowserService.SiteTitle, this);
        if (dlg.ShowDialog() == DialogResult.OK)
        {
            GetBookmarks(); // refresh the bookmarks menu
        }
    }

    private void bookmarkToolStripMenuItem_Click(object sender, EventArgs e)
    {
        using var dlg = new Bookmark_Form(wv2Client, wv2Client.BrowserService.URL, wv2Client.BrowserService.SiteTitle, this);
        if (dlg.ShowDialog() == DialogResult.OK)
        {
            GetBookmarks(); // refresh the bookmarks menu
        }
    }
    private async void OnFormLoad()
    {
        if (_incognito) { await Task.Delay(1000); }
        if (!string.IsNullOrWhiteSpace(_url)) { await wv2Client.Navigation.NewTabGoTo(_url); return; }
        GetFavorites();
        GetBookmarks();
        await wv2Client.Navigation.GoTo("www.google.com");
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
    private async void OnNewWindowRequested(object? sender, CoreWebView2NewWindowRequestedEventArgs e) // custom event when a link new tab/window is requested
    {
        e.Handled = true;
        DialogResult res = MessageBox.Show("Would you like to proceed to a new window?",
        "New Window Requested", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Information);
        if (res == DialogResult.Yes)
        {
            string url = e.Uri.ToString();
            OpenNewWindow(url);
        }
        else if (res == DialogResult.No)
        {
            string url = e.Uri.ToString();
            if (!string.IsNullOrWhiteSpace(url))
                await wv2Client.Navigation.NewTabGoTo(url);
        }
        return;
    }
    private void WebView_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(wv2Client.BrowserService.URL))
        {
            //add a oneway binding to URL
            SearchComboBox.TextChanged -= SearchComboBox_TextChanged;
            SearchComboBox.Text = wv2Client.BrowserService.URL;
            SearchComboBox.TextChanged += SearchComboBox_TextChanged;
        }
    }
    private void Form_Main_Load(object sender, EventArgs e)
    {
        OnFormLoad();
    }
    private void Form_Main_FormClosing(object sender, FormClosingEventArgs e)
    {
        if (_incognito)
        {
            wv2Client.Navigation.Incognito_DisposeSession();
        }
        if (_incognito || _newWindow)
        {
            this.Hide();
            this.Dispose();
            return;
        }

        if (_windows.Count > 0)
            foreach (Form window in _windows)
            {
                window.FormClosed -= NewWindow_FormClosed;
                window.Close();
            }

        Application.Exit();
    }
    private void MenuButton_Click(object sender, EventArgs e)
    {
        MenuPanel.Visible = !MenuPanel.Visible;
    }
    private void ReloadButton_Click(object sender, EventArgs e)
    {
        wv2Client.Navigation.Reload();
    }
    private void BackButton_Click(object sender, EventArgs e)
    {
        wv2Client.Navigation.GoBack();
    }

    // Menu Strip
    private void ClearAllBrowserDataToolStripMenuItem_Click(object sender, EventArgs e)
    {
        wv2Client.DataManager.AllBrowserData();
    }
    private void ClearAllBrowsingDataToolStripMenuItem1_Click(object sender, EventArgs e)
    {
        wv2Client.DataManager.AllBrowsingData();
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
        using Form_HistoryManager historyManager = new Form_HistoryManager(wv2Client, this);
        historyManager.ShowDialog();
    }

    private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
    {
        using (Form_Settings settingsManager = new Form_Settings(this))
        {
            this.Hide();
            settingsManager.ShowDialog();
            formManager.PreserveCurrentFormLocationAndSize(settingsManager);
        }
        this.Show();
        MenuButton.PerformClick();
        wv2Client.BrowserService.Browser.Focus();
    }
    private void newWindowToolStripMenuItem_Click(object sender, EventArgs e)
    {
        string url = "www.google.com";
        if (!string.IsNullOrWhiteSpace(url))
            OpenNewWindow(url);
    }

    private async void importChromeHistoryToolStripMenuItem_Click(object sender, EventArgs e)
    {
        using var dialog = new OpenFileDialog
        {
            Title = "Select Chrome/Edge History file",
            Filter = "Chrome History (History)|History|All files (*.*)|*.*",
            InitialDirectory = AppDomain.CurrentDomain.BaseDirectory
        };

        if (dialog.ShowDialog() != DialogResult.OK) return;

        var result = await wv2Client.DataManager.ImportChromeHistory(dialog.FileName);

        if (result.Success)
        {
            MessageBox.Show(
                $"Imported {result.UrlsImported} URLs, {result.VisitsImported} visits, {result.SearchTermsImported} search terms.",
                "Import Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        else
        {
            MessageBox.Show($"Import failed:\n{result.Error}", "Import Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
    private CancellationTokenSource? _suggestCts;
    private bool _suppressTextChanged;

    private void SearchComboBox_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Enter)
        {
            e.SuppressKeyPress = true;
            WebView21.Focus();
            wv2Client.Navigation.GoTo(SearchComboBox.Text);
        }
    }
    private void SearchComboBox_DoubleClick(object sender, EventArgs e)
    {
        TextBox textBox = sender as TextBox;
        textBox.Focus();
        textBox.SelectAll();
    }
    private async void SearchComboBox_TextChanged(object sender, EventArgs e)
    {
        if (_suppressTextChanged) return;

        string prefix = SearchComboBox.Text;

        _suggestCts?.Cancel();
        _suggestCts = new CancellationTokenSource();
        CancellationToken token = _suggestCts.Token;

        if (string.IsNullOrWhiteSpace(prefix))
        {
            SearchComboBox.DataSource = null;
            SearchComboBox.Items.Clear();
            return;
        }

        try { await Task.Delay(200, token); }
        catch (TaskCanceledException) { return; }

        List<AddressSuggestion> suggestions = await wv2Client.DataManager.GetAddressSuggestions(prefix);
        if (token.IsCancellationRequested) return;

        string currentText = SearchComboBox.Text;
        int caretPos = SearchComboBox.SelectionStart;

        _suppressTextChanged = true;

        SearchComboBox.DataSource = null;
        SearchComboBox.Items.Clear();
        SearchComboBox.DataSource = suggestions;

        SearchComboBox.SelectedIndex = -1;
        SearchComboBox.Text = currentText;
        SearchComboBox.SelectionStart = caretPos;
        SearchComboBox.SelectionLength = 0;

        _suppressTextChanged = false;

        if (suggestions.Count > 0 && SearchComboBox.Focused)
            SearchComboBox.DroppedDown = true;
    }

    private void SearchComboBox_SelectionChangeCommitted(object sender, EventArgs e)
    {
        if (SearchComboBox.SelectedItem is AddressSuggestion chosen)
        {
            wv2Client.Navigation.GoTo(chosen.Url);
        }
    }
}
