using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Web.WebView2.Core;

namespace WV2Service
{
    public static class EnumMapper
    {
        public static Dictionary<BrowsingDataKinds, CoreWebView2BrowsingDataKinds> BrowsingDataKindMap = new Dictionary<BrowsingDataKinds, CoreWebView2BrowsingDataKinds>
        {
            { BrowsingDataKinds.FileSystems, CoreWebView2BrowsingDataKinds.FileSystems },
            { BrowsingDataKinds.IndexedDb, CoreWebView2BrowsingDataKinds.IndexedDb },
            { BrowsingDataKinds.LocalStorage, CoreWebView2BrowsingDataKinds.LocalStorage },
            { BrowsingDataKinds.WebSql, CoreWebView2BrowsingDataKinds.WebSql },
            { BrowsingDataKinds.CacheStorage, CoreWebView2BrowsingDataKinds.CacheStorage },
            { BrowsingDataKinds.AllDomStorage, CoreWebView2BrowsingDataKinds.AllDomStorage },
            { BrowsingDataKinds.Cookies, CoreWebView2BrowsingDataKinds.Cookies },
            { BrowsingDataKinds.AllSite, CoreWebView2BrowsingDataKinds.AllSite },
            { BrowsingDataKinds.DiskCache, CoreWebView2BrowsingDataKinds.DiskCache },
            { BrowsingDataKinds.DownloadHistory, CoreWebView2BrowsingDataKinds.DownloadHistory },
            { BrowsingDataKinds.GeneralAutofill, CoreWebView2BrowsingDataKinds.GeneralAutofill },
            { BrowsingDataKinds.PasswordAutosave, CoreWebView2BrowsingDataKinds.PasswordAutosave },
            { BrowsingDataKinds.BrowsingHistory, CoreWebView2BrowsingDataKinds.BrowsingHistory },
            { BrowsingDataKinds.Settings, CoreWebView2BrowsingDataKinds.Settings },
            { BrowsingDataKinds.AllProfile, CoreWebView2BrowsingDataKinds.AllProfile },
            { BrowsingDataKinds.ServiceWorkers, CoreWebView2BrowsingDataKinds.ServiceWorkers }
        };
        public static Dictionary<ColorScheme, CoreWebView2PreferredColorScheme> PreferredColorMap = new Dictionary<ColorScheme, CoreWebView2PreferredColorScheme>
        {
            {ColorScheme.Auto, CoreWebView2PreferredColorScheme.Auto },
            {ColorScheme.Light, CoreWebView2PreferredColorScheme.Light },
            {ColorScheme.Dark, CoreWebView2PreferredColorScheme.Dark },
        };
    }
}
