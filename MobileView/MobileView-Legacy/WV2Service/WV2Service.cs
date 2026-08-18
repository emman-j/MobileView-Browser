using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;
using System.ComponentModel;
using System.Runtime.CompilerServices;
namespace MobileView.WV2Service;
public class WV2Service : IWV2Service, INotifyPropertyChanged
{
    private string _SiteTitle = "";
    private string _ProfileName = "";
    private string _URL = "";
    private string _ProfileFolder = "";
    private string _TempFolder = "";
    private string _ExtensionsFolder = "";
    private string _UserAgent = "";

    public string SiteTitle 
    { 
        get => _SiteTitle;
        set
        {
            if(_SiteTitle.Equals(value)) return;
            _SiteTitle = value;
            NotifyPropertyChanged();
        }
    }
    public string ProfileName 
    { 
        get => _ProfileName;
        set
        {
            if(_ProfileName.Equals(value)) return;
            _ProfileName = value;
            NotifyPropertyChanged();
        }
    }
    public string URL 
    { 
        get => _URL;
        set
        {
            if(_URL.Equals(value)) return;
            _URL = value;
            NotifyPropertyChanged();
        }
    }
    public string ProfileFolder 
    { 
        get => _ProfileFolder;
        set
        {
            if(_ProfileFolder.Equals(value)) return;
            _ProfileFolder = value;
            NotifyPropertyChanged();
        }
    }
    public string TempFolder 
    { 
        get => _TempFolder;
        set
        {
            if(_TempFolder.Equals(value)) return;
            _TempFolder = value;
            NotifyPropertyChanged();
        }
    }
    public string ExtensionsFolder 
    { 
        get => _ExtensionsFolder;
        set
        {
            if(_ExtensionsFolder.Equals(value)) return;
            _ExtensionsFolder = value;
            NotifyPropertyChanged();
        }
    }
    public string UserAgent 
    { 
        get => _UserAgent;
        set
        {
            if (_UserAgent.Equals(value)) return;
            _UserAgent = value;
            NotifyPropertyChanged();
        }
    }
    public List<string> ExtensionsPath { get; set; }
    public CoreWebView2Environment Environment { get; set; }
    public CoreWebView2Profile Profile { get; set; }
    public WebView2 Browser { get; set; }

    public event PropertyChangedEventHandler? PropertyChanged;

    public void NotifyPropertyChanged([CallerMemberName] string propertyname = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyname));
    }
}