using MobileView.Classes;
namespace MobileView.Forms;
public partial class Form_Settings : Form
{
    private FormManager formManager;
    public Form_Settings(Form? currentform = null)
    {
        InitializeComponent();
        formManager = new FormManager(this);
        formManager.PreserveCurrentFormLocationAndSize(currentform);
    }

    private void Form_Settings_Load(object sender, EventArgs e)
    {
        string path = System.Configuration.ConfigurationManager
            .OpenExeConfiguration(System.Configuration.ConfigurationUserLevel.PerUserRoamingAndLocal)
            .FilePath;

        textBox4.Text = path; 

        textBox1.Text = Properties.Settings.Default.UserAgent;

        listBox1.Items.Clear();
        foreach (string url in Properties.Settings.Default.ValidUrlSuffix)
        {
            listBox1.Items.Add(url);
        }
    }

    private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (listBox1.SelectedItem != null)
            textBox3.Text = listBox1.SelectedItem.ToString();
    }
    private void button4_Click(object sender, EventArgs e)
    {
        int idx = listBox1.SelectedIndex;
        if (idx >= 0 && !string.IsNullOrWhiteSpace(textBox2.Text))
            listBox1.Items[idx] = textBox3.Text.Trim();
    }
    private void button2_Click(object sender, EventArgs e)
    {
        if (!string.IsNullOrWhiteSpace(textBox2.Text))
        {
            listBox1.Items.Add(textBox2.Text.Trim());
            textBox2.Clear();
        }
    }
    private void button3_Click(object sender, EventArgs e)
    {
        if (listBox1.SelectedItem != null)
            listBox1.Items.Remove(listBox1.SelectedItem);
    }

    private void button1_Click(object sender, EventArgs e)
    {
        Properties.Settings.Default.UserAgent = textBox1.Text.Trim();

        Properties.Settings.Default.ValidUrlSuffix.Clear();
        Properties.Settings.Default.ValidUrlSuffix.AddRange(
            listBox1.Items.Cast<string>().ToArray());

        Properties.Settings.Default.Save();

        DialogResult result = MessageBox.Show(
            "Settings have been saved. Restart the application now for changes to take effect?",
            "Restart Required",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question);

        if (result == DialogResult.Yes)
        {
            Application.Restart();
            Environment.Exit(0); // ensures the current process fully exits
        }
        else
        {
            this.Close();
        }
    }

    private void BackButton_Click(object sender, EventArgs e)
    {
        this.Close();
    }
}
