using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MobileView_Wpf.Infrastructure.Behaviors
{
    public static class AspectRatio
    {
        public static readonly DependencyProperty RatioProperty =
            DependencyProperty.RegisterAttached(
                "Ratio",
                typeof(double),
                typeof(AspectRatio),
                new PropertyMetadata(0.0, OnRatioChanged));

        public static void SetRatio(DependencyObject element, double value)
        {
            element.SetValue(RatioProperty, value);
        }

        public static double GetRatio(DependencyObject element)
        {
            return (double)element.GetValue(RatioProperty);
        }

        private static void OnRatioChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Window window)
            {
                window.Loaded += (s, ev) =>
                {
                    double aspectRatio = GetRatio(window);
                    EnforceAspectRatio(window, aspectRatio);
                };

                window.SizeChanged += (s, ev) =>
                {
                    double aspectRatio = GetRatio(window);

                    if (ev.WidthChanged && !ev.HeightChanged)
                    {
                        window.Height = window.Width / aspectRatio;
                    }
                    else if (ev.HeightChanged && !ev.WidthChanged)
                    {
                        window.Width = window.Height * aspectRatio;
                    }
                };
            }
        }

        private static void EnforceAspectRatio(Window window, double aspectRatio)
        {
            if (aspectRatio <= 0) return;

            // Initial enforce when loaded
            window.Height = window.Width / aspectRatio;
        }
    }

}
