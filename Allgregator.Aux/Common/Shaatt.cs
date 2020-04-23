using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Allgregator.Aux.Common {
    public class Shaatt {
        public static bool GetRect(DependencyObject obj) => (bool)obj.GetValue(RectProperty);
        public static void SetRect(DependencyObject obj, bool value) => obj.SetValue(RectProperty, value);
        public static readonly DependencyProperty RectProperty = DependencyProperty.RegisterAttached(
            "Rect", typeof(bool), typeof(Shaatt), new PropertyMetadata(false, OnRectChanged));

        public static bool GetCircle(DependencyObject obj) => (bool)obj.GetValue(CircleProperty);
        public static void SetCircle(DependencyObject obj, bool value) => obj.SetValue(CircleProperty, value);
        public static readonly DependencyProperty CircleProperty = DependencyProperty.RegisterAttached(
            "Circle", typeof(bool), typeof(Shaatt), new PropertyMetadata(false, OnCircleChanged));

        public static Geometry GetPath(DependencyObject obj) => (Geometry)obj.GetValue(PathProperty);
        public static void SetPath(DependencyObject obj, Geometry value) => obj.SetValue(PathProperty, value);
        public static readonly DependencyProperty PathProperty = DependencyProperty.RegisterAttached(
            "Path", typeof(Geometry), typeof(Shaatt), new PropertyMetadata(null, OnPathChanged));


        private static void OnRectChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            if (e.NewValue is bool value && value == true) {
                //M0,0 V1 H1 V0 Z
                var geometry = new RectangleGeometry() { Rect = new Rect(0, 0, 1, 1) };
                SetPath(d, geometry);
            }
        }

        private static void OnCircleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            if (e.NewValue is bool value && value == true) {
                //M0,1 A1,1 0 1 1 2,1 A1,1 0 1 1 0,1
                var geometry = new EllipseGeometry() { RadiusX = 1d, RadiusY = 1d };
                SetPath(d, geometry);
            }
        }

        private static void OnPathChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            if (d is Control control) {
                if (control.Template == null) {
                    var templateObj = control.TryFindResource(Given.ButtonShapeTemplateKey);
                    if (templateObj is ControlTemplate template) {
                        control.Template = template;
                        control.ApplyTemplate();
                    }
                }

                SetGeometry(control);
            }
        }

        private static void SetGeometry(Control control) {
            var border = control?.Template?.FindName("border", control);
            if (border is Path path && path != null) {
                path.Data = (Geometry)control.GetValue(Shaatt.PathProperty);
            }
        }
    }
}
