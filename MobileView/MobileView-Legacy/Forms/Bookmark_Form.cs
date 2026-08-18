using MobileView.Classes;
using MobileView.WV2Service;
using MobileView.WV2Service.Data;
namespace MobileView.Forms;

public partial class Bookmark_Form : Form
{
    private readonly FormManager formManager;
    private readonly Client _client;
    private readonly int? _bookmarkId; // null = Add mode, set = Modify mode
    private readonly Dictionary<string, int> _folderPathToId = new();

    // --- Add mode: bookmark a page that isn't bookmarked yet ---
    public Bookmark_Form(Client client, string url, string pageTitle, Form? currentform = null)
    {
        InitializeComponent();
        _client = client;
        _bookmarkId = null;

        UrlTextBox.Text = url;
        TitleTextBox.Text = pageTitle;
        AddModifyButton.Text = "Add";

        BackButton.Click += (s, e) => Close();
        AddModifyButton.Click += AddModifyButton_Click;
        Load += async (s, e) => await LoadFoldersAsync();

        formManager = new FormManager(this);
        formManager.PreserveCurrentFormLocationAndSize(currentform);
    }

    // --- Modify mode: edit an existing bookmark ---
    public Bookmark_Form(Client client, BookmarkEntry existing, Form? currentform = null)
    {
        InitializeComponent();
        _client = client;
        _bookmarkId = existing.Id;

        UrlTextBox.Text = existing.Url;
        TitleTextBox.Text = existing.Title;
        AddModifyButton.Text = "Save";

        BackButton.Click += (s, e) => Close();
        AddModifyButton.Click += AddModifyButton_Click;
        Load += async (s, e) =>
        {
            await LoadFoldersAsync();
            if (!string.IsNullOrWhiteSpace(existing.FolderPath))
                comboBox1.Text = existing.FolderPath;
        };

        formManager = new FormManager(this);
        formManager.PreserveCurrentFormLocationAndSize(currentform);
    }

    private async Task LoadFoldersAsync()
    {
        List<BookmarkFolder> folders = await _client.DataManager.GetBookmarkFolders();
        var byId = folders.ToDictionary(f => f.Id);

        string BuildPath(BookmarkFolder f) =>
            f.ParentId is null || !byId.ContainsKey(f.ParentId.Value)
                ? f.Name
                : BuildPath(byId[f.ParentId.Value]) + " / " + f.Name;

        _folderPathToId.Clear();
        comboBox1.Items.Clear();
        foreach (var folder in folders)
        {
            string path = BuildPath(folder);
            _folderPathToId[path] = folder.Id;
            comboBox1.Items.Add(path);
        }
    }

    private async Task<int?> ResolveFolderIdAsync()
    {
        string folderPath = comboBox1.Text.Trim();
        if (string.IsNullOrWhiteSpace(folderPath)) return null;

        if (_folderPathToId.TryGetValue(folderPath, out int existingId))
            return existingId;

        // typed a name that doesn't exist yet — create it as a new root-level folder
        return await _client.DataManager.AddBookmarkFolder(folderPath);
    }

    private async void AddModifyButton_Click(object sender, EventArgs e)
    {
        string title = TitleTextBox.Text.Trim();
        string url = UrlTextBox.Text.Trim();
        if (string.IsNullOrWhiteSpace(title))
        {
            MessageBox.Show("Please enter a title.", "Bookmark", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        int? folderId = await ResolveFolderIdAsync();

        if (_bookmarkId is null)
            await _client.DataManager.AddBookmark(url, title, folderId);
        else
            await _client.DataManager.UpdateBookmark(_bookmarkId.Value, title, folderId);

        DialogResult = DialogResult.OK;
        Close();
    }
}
