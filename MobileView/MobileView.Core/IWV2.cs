using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobileView.Core
{
    public interface IWV2
    {
        string SiteTitle { get; set; }
        string ProfileName { get; set; }
        string URL { get; set; }
        string ProfileFolder { get; set; }
        string UserAgent { get; set; }
        List<string> ExtensionsPath { get; set; }
        CoreWebView2Environment Environment { get; set; }
        CoreWebView2Profile Profile { get; set; }
        WebViewWrapper WebControl { get; set; }
    }
}
