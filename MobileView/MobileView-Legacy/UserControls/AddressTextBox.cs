using MobileView.WV2Service.Data;
namespace MobileView.UserControls;

public partial class AddressTextBox : UserControl
{
    private CancellationTokenSource? _suggestCts;
    private bool _suppressTextChanged;
    private readonly SuggestionPopup _popup = new();

    public Func<string, Task<List<AddressSuggestion>>>? SuggestionProvider { get; set; }
    public event EventHandler<string>? Navigate;

    public string Text
    {
        get => textBox1.Text;
        set
        {
            _suppressTextChanged = true;
            textBox1.Text = value;
            textBox1.SelectionStart = value?.Length ?? 0;
            _suppressTextChanged = false;
        }
    }

    public AddressTextBox()
    {
        InitializeComponent();

        textBox1.TextChanged += TextBox1_TextChanged;
        textBox1.KeyDown += TextBox1_KeyDown;
        textBox1.DoubleClick += TextBox1_DoubleClick;
        textBox1.LostFocus += TextBox1_LostFocus;

        _popup.ListBox.Click += ListBox1_Click;
        _popup.ListBox.KeyDown += ListBox1_KeyDown;

        HandleDestroyed += (s, e) => _popup.Close(); // don't leak the popup when the form closes
    }

    private async void TextBox1_TextChanged(object? sender, EventArgs e)
    {
        if (_suppressTextChanged) return;

        string prefix = textBox1.Text;

        _suggestCts?.Cancel();
        _suggestCts = new CancellationTokenSource();
        CancellationToken token = _suggestCts.Token;

        if (string.IsNullOrWhiteSpace(prefix) || SuggestionProvider == null)
        {
            HideSuggestions();
            return;
        }

        try { await Task.Delay(200, token); }
        catch (TaskCanceledException) { return; }

        List<AddressSuggestion> suggestions;
        try { suggestions = await SuggestionProvider(prefix); }
        catch { HideSuggestions(); return; }

        if (token.IsCancellationRequested) return;

        ShowSuggestions(suggestions);
    }

    private void ShowSuggestions(List<AddressSuggestion> suggestions)
    {
        if (suggestions.Count == 0) { HideSuggestions(); return; }

        _popup.ListBox.DisplayMember = "Title";
        _popup.ListBox.DataSource = null;
        _popup.ListBox.DataSource = suggestions;

        int height = Math.Min(suggestions.Count, 6) * _popup.ListBox.ItemHeight + 6;

        // Position in screen coordinates, just under the textbox — works regardless of WebView2 z-order
        Point screenLoc = PointToScreen(new Point(0, Height));
        _popup.Bounds = new Rectangle(screenLoc.X, screenLoc.Y, Width, height);

        if (!_popup.Visible) _popup.Show(FindForm()!); // owned by the main form, but a real top-level window
        _popup.BringToFront();
    }

    private void HideSuggestions()
    {
        if (_popup.Visible) _popup.Hide();
        _popup.ListBox.DataSource = null;
    }

    private void TextBox1_KeyDown(object? sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Down && _popup.Visible && _popup.ListBox.Items.Count > 0)
        {
            e.Handled = true;
            e.SuppressKeyPress = true;
            _popup.ListBox.SelectedIndex = 0;
            _popup.ListBox.Focus(); // safe: WS_EX_NOACTIVATE keeps the OWNING window's activation state sane
            return;
        }

        if (e.KeyCode == Keys.Enter)
        {
            e.Handled = true;
            e.SuppressKeyPress = true;
            HideSuggestions();
            Navigate?.Invoke(this, textBox1.Text);
            return;
        }

        if (e.KeyCode == Keys.Escape)
        {
            HideSuggestions();
        }
    }

    private void TextBox1_DoubleClick(object? sender, EventArgs e)
    {
        textBox1.Focus();
        textBox1.SelectAll();
    }

    private void TextBox1_LostFocus(object? sender, EventArgs e)
    {
        var t = new System.Windows.Forms.Timer { Interval = 150 };
        t.Tick += (s, e2) =>
        {
            t.Stop();
            t.Dispose();
            if (!_popup.ListBox.Focused) HideSuggestions();
        };
        t.Start();
    }

    private void ListBox1_KeyDown(object? sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Up && _popup.ListBox.SelectedIndex == 0)
        {
            e.Handled = true;
            e.SuppressKeyPress = true;
            _popup.ListBox.SelectedIndex = -1;
            textBox1.Focus();
            textBox1.SelectionStart = textBox1.Text.Length;
            return;
        }

        if (e.KeyCode == Keys.Enter)
        {
            e.Handled = true;
            e.SuppressKeyPress = true;
            if (_popup.ListBox.SelectedItem is AddressSuggestion chosen) SelectSuggestion(chosen);
            return;
        }

        if (e.KeyCode == Keys.Escape)
        {
            HideSuggestions();
            textBox1.Focus();
        }
    }

    private void ListBox1_Click(object? sender, EventArgs e)
    {
        if (_popup.ListBox.SelectedItem is AddressSuggestion chosen) SelectSuggestion(chosen);
    }

    private void SelectSuggestion(AddressSuggestion chosen)
    {
        Text = chosen.Url;
        HideSuggestions();
        Navigate?.Invoke(this, chosen.Url);
    }
}