using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;
using System.ComponentModel;

namespace MobileView.WV2Service;

public interface IWV2Service
{
    string SiteTitle { get; set; }
    string ProfileName { get; set; }
    string URL { get; set; }
    string ProfileFolder { get; set; }
    string TempFolder { get; set; }
    string ExtensionsFolder { get; set; }
    string UserAgent { get; set; }
    List<string> ExtensionsPath { get; set; }
    CoreWebView2Environment Environment { get; set; }
    CoreWebView2Profile Profile { get; set; }
    WebView2 Browser { get; set; }
}
