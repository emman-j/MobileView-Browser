using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.WinForms;
using System.ComponentModel;

namespace WV2Service
{
    public partial class WebViewService
    {
        private interface IWV2ServiceModel
        {
            string SiteTitle { get; set; }
            string ProfileName { get; set; }
            string URL { get; set; }
            string ProfileFolder { get; set; }
            List<string> ExtensionsPath { get; set; }
            CoreWebView2Environment Environment { get; set; }
            CoreWebView2Profile Profile { get; set; }
            WebView2 WebviewControl { get; set; }
        }
        private class WV2ServiceModel : IWV2ServiceModel
        {
            public string SiteTitle { get; set; }
            public string ProfileName { get; set; }
            public string URL { get; set; }
            public string ProfileFolder { get; set; }
            public List<string> ExtensionsPath { get; set; }
            public CoreWebView2Environment Environment { get; set; }
            public CoreWebView2Profile Profile { get; set; }
            public WebView2 WebviewControl { get; set; }
        }
    }
}
