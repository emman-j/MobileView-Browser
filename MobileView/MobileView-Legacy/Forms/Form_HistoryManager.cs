using MobileView.Classes;
using MobileView.WV2Service;
using MobileView.WV2Service.Data;
namespace MobileView;
public partial class Form_HistoryManager : Form
{
    private readonly Client wv2Client;
    private readonly FormManager formManager;
    private bool _showingFavorites;

    public Form_HistoryManager(Client client, Form? currentform = null)
    {
        InitializeComponent();
        formManager = new FormManager(this);
        wv2Client = client;
        formManager.PreserveCurrentFormLocationAndSize(currentform);
    }

    private async void BindDataGridView(DataGridView dgv)
    {
        dgv.DataSource = null;
        List<HistoryEntry> entries = _showingFavorites
            ? await wv2Client.DataManager.GetFavorites()
            : await wv2Client.DataManager.GetHistory();
        dgv.DataSource = entries;
        if (dgv.Columns["Id"] != null) dgv.Columns["Id"].Visible = false;
        if (dgv.Columns["Url"] != null) dgv.Columns["Url"].Visible = false;
    }

    private void Form_HistoryManager_Shown(object sender, EventArgs e) => BindDataGridView(dataGridView1);
    private void ReloadButton_Click(object sender, EventArgs e) => BindDataGridView(dataGridView1);

    private void FavoritesToggle_CheckedChanged(object sender, EventArgs e)
    {
        _showingFavorites = ((CheckBox)sender).Checked;
        BindDataGridView(dataGridView1);
    }

    private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex < 0) return;
        if (dataGridView1.Rows[e.RowIndex].DataBoundItem is HistoryEntry entry)
        {
            wv2Client.Navigation.GoTo(entry.Url);
            this.Close();
        }
    }

    private void MenuButton_Click(object sender, EventArgs e) => MenuPanel.Visible = !MenuPanel.Visible;

    private async void clearAllBrowsingHistoryToolStripMenuItem_Click(object sender, EventArgs e)
    {
        await wv2Client.DataManager.ClearLocalHistory();  // our db
        await wv2Client.DataManager.AllBrowsingData();    // native WebView2 data too
        BindDataGridView(dataGridView1);
    }

    private void BackButton_Click(object sender, EventArgs e) => this.Close();
}
