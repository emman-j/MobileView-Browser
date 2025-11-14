using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobileView_Wpf.Models.WebView
{
    public interface IWV2ServiceModel
    {
        string SiteTitle { get; set; }
        string ProfileName { get; set; }
        string URL { get; set; }
        string ProfileFolder { get; set; }
        string UserAgent { get; set; }
        List<string> ExtensionsPath { get; set; }
        CoreWebView2Environment Environment { get; set; }
        CoreWebView2Profile Profile { get; set; }
        WebView2 WebViewControl { get; set; }
    }

    public class WV2ServiceModel : IWV2ServiceModel
    {
        public string SiteTitle { get; set; }
        public string ProfileName { get; set; }
        public string URL { get; set; }
        public string ProfileFolder { get; set; }
        public string UserAgent { get; set; }
        public List<string> ExtensionsPath { get; set; }
        public CoreWebView2Environment Environment { get; set; }
        public CoreWebView2Profile Profile { get; set; }
        public WebView2 WebViewControl { get; set; }
    }
}
