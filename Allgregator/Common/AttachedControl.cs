using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Allgregator.Common {
    public class AttachedControl {
        public static Geometry GetPath(DependencyObject obj) {
            return (Geometry)obj.GetValue(PathProperty);
        }

        public static void SetPath(DependencyObject obj, Geometry value) {
            obj.SetValue(PathProperty, value);
        }

        public static readonly DependencyProperty PathProperty =
            DependencyProperty.RegisterAttached("Path", typeof(Geometry), typeof(AttachedControl), new PropertyMetadata(null, OnPathChanged));


        private static void OnPathChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            if (d is FrameworkElement frameworkElement) {
                frameworkElement.Loaded += FrameworkElement_Loaded;
            }
        }

        private static void FrameworkElement_Loaded(object sender, RoutedEventArgs e) {
            if (sender is Control control) {
                var template = control.Template;
                var border = template?.FindName("border", control);
                if (border is Path path) {
                    path.Data = (Geometry)control.GetValue(AttachedControl.PathProperty);
                }
            }
        }
    }
}
