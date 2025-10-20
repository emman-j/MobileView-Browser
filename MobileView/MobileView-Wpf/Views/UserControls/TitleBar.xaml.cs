using MobileView_Wpf.ViewModel;
using MobileView_Wpf.ViewModel.UserControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MobileView_Wpf.Views.UserControls
{
    /// <summary>
    /// Interaction logic for TitleBar.xaml
    /// </summary>
    public partial class TitleBar : UserControl
    {
        public static readonly DependencyProperty TextValueProperty =
            DependencyProperty.Register(nameof(TextValue), typeof(string), typeof(TitleBar), new PropertyMetadata(string.Empty));

        public string TextValue
        {
            get => (string)GetValue(TextValueProperty);
            set => SetValue(TextValueProperty, value);
        }

        public TitleBar()
        {
            InitializeComponent();
        }

        private void StackPanel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var window = Window.GetWindow(this);

            if (e.ClickCount == 2)
            {
                // Double-click toggles maximize
                if (window.WindowState == WindowState.Maximized)
                    window.WindowState = WindowState.Normal;
                else
                    window.WindowState = WindowState.Maximized;
            }
            else if (e.ButtonState == MouseButtonState.Pressed)
            {
                window?.DragMove();
            }
        }
    }
}
