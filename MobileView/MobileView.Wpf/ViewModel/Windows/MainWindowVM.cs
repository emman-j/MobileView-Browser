using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace MobileView_Wpf.ViewModel.Windows
{
    public class MainWindowVM : ViewModelBase
    {
        private string _sharedText;
        public string SharedText
        {
            get => _sharedText;
            set
            {
                _sharedText = value;
                NotifyPropertyChanged();
            }
        }

        public ICommand TitleBarCloseCommand { get; }


        public MainWindowVM()
        {
            TitleBarCloseCommand = new RelayCommand(_ => Application.Current.MainWindow?.Close());
        }
    }
}
