using Microsoft.Web.WebView2.Core;
using MobileView.Core.Enums;

namespace MobileView.Core
{
    public class WebViewWrapper
    {
        private Microsoft.Web.WebView2.WinForms.WebView2 _webViewWinforms;
        private Microsoft.Web.WebView2.Wpf.WebView2 _webViewWPF;
        private UIFramework _uiFramework;

        public event EventHandler<CoreWebView2NavigationStartingEventArgs> NavigationStarting;
        public event EventHandler<CoreWebView2NavigationCompletedEventArgs> NavigationCompleted;
        public Uri Source 
        {  
            get => GetBaseWebView().Source; 
            set => GetBaseWebView().Source = value; 
        }
        public CoreWebView2 CoreWebView2 { get => GetBaseWebView().CoreWebView2; }
        public bool CanGoBack { get => GetBaseWebView().CanGoBack; }
        public bool CanGoForward { get => GetBaseWebView().CanGoForward; }

        public WebViewWrapper(UIFramework ui, object webView) 
        {
            if(webView == null)
                throw new ArgumentNullException(nameof(webView), "WebView instance cannot be null.");

            switch (ui)
            { 
                case UIFramework.Winforms:
                    _webViewWinforms = webView as Microsoft.Web.WebView2.WinForms.WebView2;
                    break;
                case UIFramework.WPF:
                    _webViewWPF = webView as Microsoft.Web.WebView2.Wpf.WebView2;
                    break;
            }
            _uiFramework = ui;
            SubscribeEvents();
        }

        private dynamic GetBaseWebView()
        {
            switch (_uiFramework)
            {
                case UIFramework.Winforms:
                    return _webViewWinforms;
                case UIFramework.WPF:
                    return _webViewWPF;
                default:
                    throw new NotSupportedException("The specified UI framework is not supported.");
            }
        }
        private void SubscribeEvents()
        {
            switch (_uiFramework)
            {
                case UIFramework.Winforms:
                    _webViewWinforms.NavigationStarting += (sender, args) => NavigationStarting?.Invoke(this, args); // NavigationStarting
                    _webViewWinforms.NavigationCompleted += (sender, args) => NavigationCompleted?.Invoke(this, args);// NavigationCompleted
                    break;
                case UIFramework.WPF:
                    _webViewWPF.NavigationStarting += (sender, args) => NavigationStarting?.Invoke(this, args);
                    _webViewWPF.NavigationCompleted += (sender, args) => NavigationCompleted?.Invoke(this, args);
                    break;
            }
        }

        public Task EnsureCoreWebView2Async(CoreWebView2Environment environment) => GetBaseWebView().EnsureCoreWebView2Async(environment);
        public Task EnsureCoreWebView2Async(CoreWebView2Environment environment, CoreWebView2ControllerOptions controllerOptions) 
            => GetBaseWebView().EnsureCoreWebView2Async(environment, controllerOptions);
        public void GoBack() => GetBaseWebView().GoBack();
        public void GoForward() => GetBaseWebView().GoForward();
        public void Navigate(string uri) => CoreWebView2.Navigate(uri);
        public void Reload() => GetBaseWebView().Reload();
        public void Focus() => GetBaseWebView().Focus();
    }
}
