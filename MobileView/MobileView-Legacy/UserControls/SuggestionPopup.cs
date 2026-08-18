namespace MobileView.UserControls;
internal class SuggestionPopup : Form
{
    public ListBox ListBox { get; } = new ListBox();

    public SuggestionPopup()
    {
        FormBorderStyle = FormBorderStyle.None;
        ShowInTaskbar = false;
        StartPosition = FormStartPosition.Manual;
        TopMost = true;

        ListBox.Dock = DockStyle.Fill;
        ListBox.BackColor = Color.FromArgb(39, 39, 39);
        ListBox.ForeColor = Color.White;
        ListBox.ItemHeight = 15;
        Controls.Add(ListBox);
    }

    protected override bool ShowWithoutActivation => true;

    private const int WS_EX_NOACTIVATE = 0x08000000;
    protected override CreateParams CreateParams
    {
        get { var cp = base.CreateParams; cp.ExStyle |= WS_EX_NOACTIVATE; return cp; }
    }
}