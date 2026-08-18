using Microsoft.Web.WebView2.Core;
using MobileView.Classes;
using MobileView.Forms;
using MobileView.WV2Service;
using MobileView.WV2Service.Data;
using System.ComponentModel;

namespace MobileView;

public partial class Form_Main : Form
{
    #region Fields
    private static readonly string _userAgent = Properties.Settings.Default.UserAgent;
    private static readonly List<string> _validUrlSuffixes = Properties.Settings.Default.ValidUrlSuffix.Cast<string>().ToList();

    private readonly TitleBar titleBar;
    private readonly FormManager formManager;
    private readonly List<Form> _windows = new();

    private List<string> _extensionsPaths;
    private bool _incognito;
    private bool _newWindow;
    private readonly string? _url;

    private Client wv2Client;

    #endregion

    #region Constructor
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
            InitializeIncognito();
        else if (!string.IsNullOrWhiteSpace(_url))
            InitializeNewWindow(profileFolder);
        else
            InitializeBrowser();
        InitializeAddressBar();
    }
    #endregion

    #region Browser Initialization
    private void InitializeAddressBar()
    {
        addressTextBox1.SuggestionProvider = prefix => wv2Client.DataManager.GetAddressSuggestions(prefix);
        addressTextBox1.Navigate += (s, address) =>
        {
            WebView21.Focus();
            wv2Client.Navigation.GoTo(address);
        };
    }
    private void InitializeBrowser()
    {
        wv2Client = new Client
        (
            webViewControl: WebView21,
            profileName: "User1",
            extensionsPath: _extensionsPaths,
            userAgent: _userAgent,
            validSuffixes: _validUrlSuffixes
        );
        Client.EnsureLocalHistoryDB(wv2Client.BrowserService.ProfileName);
        wv2Client.BrowserService.PropertyChanged += WebView_PropertyChanged;
        wv2Client.Navigation.NewWindowRequested += OnNewWindowRequested;
        wv2Client.InitializeWebView();
        FormTextLabel.DataBindings.Add("Text", wv2Client.BrowserService, nameof(wv2Client.BrowserService.SiteTitle));
    }

    private void InitializeNewWindow(string? profileFolder)
    {
        if (profileFolder == null) return;

        _newWindow = true;
        MenuButton.Visible = false;
        addressTextBox1.Size = new Size(252, 23);

        wv2Client = new Client();
        wv2Client.BrowserService.UserAgent = _userAgent;
        wv2Client.BrowserService.Browser = WebView21;
        wv2Client.Navigation.validUrlSuffixes = _validUrlSuffixes;
        wv2Client.BrowserService.PropertyChanged += WebView_PropertyChanged;
        wv2Client.Navigation.NewWindowRequested += OnNewWindowRequested;
        wv2Client.InitializeWebViewNewTab(profileFolder);
        FormTextLabel.DataBindings.Add("Text", wv2Client.BrowserService, nameof(wv2Client.BrowserService.SiteTitle));
    }

    private void InitializeIncognito()
    {
        FormTextLabel.Text = "Private";

        wv2Client = new Client
        (
            webViewControl: WebView21,
            profileName: "User1",
            extensionsPath: _extensionsPaths,
            userAgent: _userAgent,
            validSuffixes: _validUrlSuffixes
        );
        wv2Client.BrowserService.PropertyChanged += WebView_PropertyChanged;
        wv2Client.InitializeWebView();
    }

    private void EnableBorderlessWindows()
    {
        MaximizedBounds = Screen.FromHandle(Handle).WorkingArea;
        SetStyle(ControlStyles.ResizeRedraw, true);
        DoubleBuffered = true;
        ControlBox = false;
    }

    #endregion

    #region Window Management
    private void OpenNewWindow(string link)
    {
        Form newWindow = new Form_Main(currentForm: this, url: link, profileFolder: wv2Client.BrowserService.ProfileFolder);
        newWindow.FormClosed += NewWindow_FormClosed;
        _windows.Add(newWindow);
        newWindow.Show();
    }

    private void NewWindow_FormClosed(object? sender, FormClosedEventArgs e)
    {
        if (sender is Form_Main main && _windows.Contains(main))
            _windows.Remove(main);
    }

    #endregion

    #region Extensions
    private async void ViewExtensions()
    {
        List<string> extensions = await wv2Client.BrowserService.GetExtensionsList();
        MessageBox.Show(string.Join(",\n", extensions));
    }

    #endregion

    #region Favorites & Bookmarks
    private async void GetFavorites()
    {
        favoritesToolStripMenuItem.DropDownItems.Clear();

        List<HistoryEntry> favorites = await wv2Client.DataManager.GetFavorites();
        if (favorites == null) return;

        foreach (HistoryEntry entry in favorites)
        {
            ToolStripMenuItem item = new(entry.Title);
            item.Click += (s, e) => wv2Client.Navigation.GoTo(entry.Url);
            favoritesToolStripMenuItem.DropDownItems.Add(item);
        }
    }

    private async void GetBookmarks()
    {
        bookmarksToolStripMenuItem.DropDownItems.Clear();

        ToolStripMenuItem addBookmarkItem = new("&Add Bookmark");
        addBookmarkItem.Click += AddBookmark_Click;
        bookmarksToolStripMenuItem.DropDownItems.Add(addBookmarkItem);

        List<BookmarkEntry> bookmarks = await wv2Client.DataManager.GetBookmarks();
        if (bookmarks == null) return;

        // folder path -> submenu, so nested folders reuse the same menu instead of duplicating
        var folderMenus = new Dictionary<string, ToolStripMenuItem>();

        ToolStripMenuItem GetOrCreateFolderMenu(string folderPath)
        {
            if (folderMenus.TryGetValue(folderPath, out ToolStripMenuItem? existing))
                return existing;

            string[] parts = folderPath.Split(" / ", StringSplitOptions.RemoveEmptyEntries);
            ToolStripItemCollection currentLevel = bookmarksToolStripMenuItem.DropDownItems;
            string accumulatedPath = "";

            foreach (string part in parts)
            {
                accumulatedPath = accumulatedPath.Length == 0 ? part : accumulatedPath + " / " + part;

                if (!folderMenus.TryGetValue(accumulatedPath, out ToolStripMenuItem? folderMenu))
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
            ToolStripMenuItem bookmarkItem = new(entry.Title);
            bookmarkItem.Click += (s, e) => 
            { 
                wv2Client.Navigation.GoTo(entry.Url);
                wv2Client.BrowserService.Browser.Focus();
                BrowserToolStripMenuItem.HideDropDown();
            };

            ToolStripMenuItem removeItem = new("Remove");
            removeItem.Click += async (s, e) =>
            {
                await wv2Client.DataManager.RemoveBookmark(entry.Id);
                GetBookmarks(); // rebuild so it disappears immediately
            };
            bookmarkItem.DropDownItems.Add(removeItem);

            if (string.IsNullOrWhiteSpace(entry.FolderPath) || entry.FolderPath.Equals("bookmarks bar",StringComparison.OrdinalIgnoreCase))
                bookmarksToolStripMenuItem.DropDownItems.Add(bookmarkItem);
            else
                GetOrCreateFolderMenu(entry.FolderPath).DropDownItems.Add(bookmarkItem);
        }
    }

    private void AddBookmark_Click(object? sender, EventArgs e)
    {
        using var dlg = new Bookmark_Form(wv2Client, wv2Client.BrowserService.URL, wv2Client.BrowserService.SiteTitle, this);
        if (dlg.ShowDialog() == DialogResult.OK)
            GetBookmarks();
    }
    private void bookmarkToolStripMenuItem_Click(object sender, EventArgs e) => AddBookmark_Click(sender, e);
    #endregion

    #region Form Lifecycle
    private async void OnFormLoad()
    {
        if (_incognito) await Task.Delay(1000);

        if (!string.IsNullOrWhiteSpace(_url))
        {
            await wv2Client.Navigation.NewTabGoTo(_url);
            return;
        }

        GetFavorites();
        GetBookmarks();
        await wv2Client.Navigation.GoTo("www.google.com");
    }

    protected override void WndProc(ref Message m)
    {
        const int WM_NCHITTEST = 0x84;
        base.WndProc(ref m);
        if (m.Msg == WM_NCHITTEST)
            formManager.HandleWndProc(ref m);
    }

    private void Form_Main_Load(object sender, EventArgs e) => OnFormLoad();

    private void Form_Main_FormClosing(object sender, FormClosingEventArgs e)
    {
        if (_incognito)
            wv2Client.Navigation.Incognito_DisposeSession();

        if (_incognito || _newWindow)
        {
            Hide();
            Dispose();
            return;
        }

        foreach (Form window in _windows)
        {
            window.FormClosed -= NewWindow_FormClosed;
            window.Close();
        }

        Application.Exit();
    }

    #endregion

    #region Navigation Events
    private async void OnNewWindowRequested(object? sender, CoreWebView2NewWindowRequestedEventArgs e)
    {
        e.Handled = true;
        DialogResult res = MessageBox.Show(
            "Would you like to proceed to a new window?",
            "New Window Requested", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Information);

        string url = e.Uri.ToString();
        if (res == DialogResult.Yes)
        {
            OpenNewWindow(url);
        }
        else if (res == DialogResult.No && !string.IsNullOrWhiteSpace(url))
        {
            await wv2Client.Navigation.NewTabGoTo(url);
        }
    }

    private void WebView_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(wv2Client.BrowserService.URL))
            addressTextBox1.Text = wv2Client.BrowserService.URL;
    }

    #endregion

    #region Toolbar
    private void MenuButton_Click(object sender, EventArgs e) => MenuPanel.Visible = !MenuPanel.Visible;
    private void ReloadButton_Click(object sender, EventArgs e) => wv2Client.Navigation.Reload();
    private void BackButton_Click(object sender, EventArgs e) => wv2Client.Navigation.GoBack();

    #endregion

    #region Menu Strip
    private void ClearAllBrowserDataToolStripMenuItem_Click(object sender, EventArgs e) => wv2Client.DataManager.AllBrowserData();
    private void ClearAllBrowsingDataToolStripMenuItem1_Click(object sender, EventArgs e) => wv2Client.DataManager.AllBrowsingData();

    private void IncognitoToolStripMenuItem_Click(object sender, EventArgs e)
    {
        Form incognito = new Form_Main(incognito: true, currentForm: this);
        Hide();
        incognito.ShowDialog();
        formManager.PreserveCurrentFormLocationAndSize(incognito);
        Show();
    }

    private void ExitToolStripMenuItem_Click(object sender, EventArgs e) => Close();
    private void ViewExtensionsToolStripMenuItem_Click(object sender, EventArgs e) => ViewExtensions();
    private void RemoveExtensionToolStripMenuItem_Click(object sender, EventArgs e) { }
    private void AddExtensionToolStripMenuItem_Click(object sender, EventArgs e) { }

    private void ViewHistoryToolStripMenuItem_Click(object sender, EventArgs e)
    {
        using Form_HistoryManager historyManager = new(wv2Client, this);
        historyManager.ShowDialog();
    }

    private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
    {
        using (Form_Settings settingsManager = new(this))
        {
            Hide();
            settingsManager.ShowDialog();
            formManager.PreserveCurrentFormLocationAndSize(settingsManager);
        }
        Show();
        MenuButton.PerformClick();
        wv2Client.BrowserService.Browser.Focus();
    }

    private void newWindowToolStripMenuItem_Click(object sender, EventArgs e) => OpenNewWindow("www.google.com");

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

    #endregion

}