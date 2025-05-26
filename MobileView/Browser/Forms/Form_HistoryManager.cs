using MobileView.Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WV2Service;

namespace MobileView
{
    public partial class Form_HistoryManager : Form
    {
        private readonly WebViewService Browser;
        private readonly FormManager formManager;
        private DataTable HistoryTable;
        public Form_HistoryManager(WebViewService _webViewService, Form? currentform = null)
        {
            InitializeComponent();
            formManager = new FormManager(this);
            Browser = _webViewService;
            formManager.PreserveCurrentFormLocationAndSize(currentform);
        }
        private async void BindDataGridView(DataGridView datagridview)
        {
            datagridview.DataSource = null;
            datagridview.Rows.Clear();
            datagridview.Columns.Clear();
            HistoryTable = await Browser.GetHistory();
            datagridview.DataSource = HistoryTable;
            datagridview.Columns["Visit Count"].Visible = false;
            datagridview.Columns["URL"].Visible = false;
            datagridview.Columns["ID"].Visible = false;
        }
        private void Form_HistoryManager_Shown(object sender, EventArgs e)
        {
            BindDataGridView(dataGridView1);
        }
        private void ReloadButton_Click(object sender, EventArgs e)
        {
            BindDataGridView(dataGridView1);
        }
        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            DataGridView datagridview = (DataGridView)sender;
            DataGridViewRow row = datagridview.Rows[e.RowIndex];

            string? URL = row.Cells["URL"].Value.ToString();
            if (string.IsNullOrWhiteSpace(URL)) { return; }
            Browser.Navigation.GoTo(URL);
            this.Close();
        }
        private void MenuButton_Click(object sender, EventArgs e)
        {
            MenuPanel.Visible = !MenuPanel.Visible;
        }
        private void clearAllBrowsingHistoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Browser.Clear.AllBrowsingData();
        }
        private void BackButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
