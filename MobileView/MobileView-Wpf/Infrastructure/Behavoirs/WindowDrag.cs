using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;

namespace MobileView_Wpf.Infrastructure.Behaviors
{
    public static class WindowDrag
    {
        [DllImport("user32.dll")]
        private static extern bool ReleaseCapture();

        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, int wParam, int lParam);

        private const int WM_NCLBUTTONDOWN = 0xA1;
        private const int HTCAPTION = 0x2;

        public static readonly DependencyProperty IsDragEnabledProperty =
            DependencyProperty.RegisterAttached(
                "IsDragEnabled",
                typeof(bool),
                typeof(WindowDrag),
                new UIPropertyMetadata(false, OnIsDragEnabledChanged));

        public static void SetIsDragEnabled(UIElement element, bool value)
        {
            element.SetValue(IsDragEnabledProperty, value);
        }

        public static bool GetIsDragEnabled(UIElement element)
        {
            return (bool)element.GetValue(IsDragEnabledProperty);
        }

        //private static void OnIsDragEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        //{
        //    if (d is UIElement element)
        //    {
        //        if ((bool)e.NewValue)
        //        {
        //            element.MouseLeftButtonDown += Element_MouseLeftButtonDown;
        //            element.MouseLeftButtonUp += Element_MouseLeftButtonUp;
        //            element.MouseLeftButtonDown += Element_MouseDoubleClick;
        //        }
        //        else
        //        {
        //            element.MouseLeftButtonDown -= Element_MouseLeftButtonDown;
        //            element.MouseLeftButtonUp -= Element_MouseLeftButtonUp;
        //            element.MouseLeftButtonDown -= Element_MouseDoubleClick;
        //        }
        //    }
        //}
        private static void OnIsDragEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is UIElement element)
            {
                if ((bool)e.NewValue)
                {
                    element.PreviewMouseLeftButtonDown += Element_PreviewMouseLeftButtonDown;
                }
                else
                {
                    element.PreviewMouseLeftButtonDown -= Element_PreviewMouseLeftButtonDown;
                }
            }
        }
        private static void Element_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Ignore clicks on child controls that handle input (e.g., Buttons)
            if (e.OriginalSource is DependencyObject source &&
                (source is Button || source is TextBox || source is PasswordBox))
            {
                return;
            }

            if (sender is DependencyObject depObj)
            {
                Window window = Window.GetWindow(depObj);
                if (window == null) return;

                if (e.ClickCount == 2)
                {
                    // Double click to toggle maximize/restore
                    if (window.ResizeMode != ResizeMode.NoResize)
                    {
                        window.WindowState = window.WindowState == WindowState.Normal
                            ? WindowState.Maximized
                            : WindowState.Normal;
                    }
                }
                else
                {
                    // Single click to drag
                    ReleaseCapture();
                    SendMessage(new WindowInteropHelper(window).Handle, WM_NCLBUTTONDOWN, HTCAPTION, 0);
                }
            }
        }
        private static void Element_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is DependencyObject depObj)
            {
                var window = Window.GetWindow(depObj);
                if (window != null)
                {
                    ReleaseCapture();
                    SendMessage(new WindowInteropHelper(window).Handle, WM_NCLBUTTONDOWN, HTCAPTION, 0);
                }
            }
        }
        private static void Element_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            // Optional: add logic for releasing mouse after drag
        }
        private static void Element_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && sender is DependencyObject depObj)
            {
                var window = Window.GetWindow(depObj);
                if (window != null && window.ResizeMode != ResizeMode.NoResize)
                {
                    if (window.WindowState == WindowState.Normal)
                        window.WindowState = WindowState.Maximized;
                    else if (window.WindowState == WindowState.Maximized)
                        window.WindowState = WindowState.Normal;
                }
            }
        }
    }
}
