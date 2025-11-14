using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobileView.Core.Service
{
    public class DataManager
    {
        IWV2 WebView { get; set; }

        public DataManager(IWV2 webView)
        {
            WebView = webView;
        }
    }
}
