using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace MobileView_Wpf.ViewModel.UserControl
{
    public class TitleBarVM
    {
        public ICommand CloseCommand { get; }
        public ICommand MinimizeCommand { get; }
        public ICommand MaximizeCommand { get; }

        public TitleBarVM()
        {
            CloseCommand = new RelayCommand(_ => Application.Current.MainWindow?.Close());
            MinimizeCommand = new RelayCommand(_ => Application.Current.MainWindow.WindowState = WindowState.Minimized);
            MaximizeCommand = new RelayCommand(_ =>
            {
                var win = Application.Current.MainWindow;
                win.WindowState = win.WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
            });
        }
    }
}
