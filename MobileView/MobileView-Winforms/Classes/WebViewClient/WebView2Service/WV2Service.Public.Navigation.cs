using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WV2Service
{
    public partial class WebViewService
    {
        public class NavigationManager(WebViewService webviewService)
        {
            private readonly WebViewService wv2service = webviewService;
            public void GoTo(string address)
            {
                wv2service.NavigateTo(wv2service.WebViewControl, wv2service.environment, address);
            }
            public void NewTabGoTo(string address)
            {
                wv2service.NavigateToNewTab(wv2service.WebViewControl, wv2service.environment, address);
            }
            public void Reload()
            {
                wv2service.WebViewControl.Reload();
            }
            public void GoBack()
            {
                if (wv2service.WebViewControl.CanGoBack) wv2service.WebViewControl.GoBack();
            }
            public void GoForward()
            {
                if (wv2service.WebViewControl.CanGoForward) wv2service.WebViewControl.GoForward();
            }
            public async void Incognito_DisposeSession()
            {
                string tempfolderpath = wv2service._TempFolder;
                int maxRetries = 5;
                int delayMilliseconds = 2000;
                for (int attempt = 1; attempt <= maxRetries; attempt++)
                {
                    try
                    {
                        //ensure all processes are closed first
                        await Task.Run(() =>
                        {
                            if (!Directory.Exists(tempfolderpath)) { return; }
                            Directory.Delete(tempfolderpath, true);
                        });
                        wv2service._TempFolder = string.Empty;
                    }
                    catch
                    {
                        await Task.Delay(delayMilliseconds * attempt);
                    }
                }
            }
        }
    }
}
