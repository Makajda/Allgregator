using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Allgregator.Aux.Common {
    public enum Peaks {
        None,
        TopLeft,
        TopRight,
        BottomRight,
        BottomLeft
    }

    public class Shaatt {
        public static bool GetRect(DependencyObject obj) => (bool)obj.GetValue(RectProperty);
        public static void SetRect(DependencyObject obj, bool value) => obj.SetValue(RectProperty, value);
        public static readonly DependencyProperty RectProperty = DependencyProperty.RegisterAttached(
            "Rect", typeof(bool), typeof(Shaatt), new PropertyMetadata(false, OnRectChanged));

        public static bool GetCircle(DependencyObject obj) => (bool)obj.GetValue(CircleProperty);
        public static void SetCircle(DependencyObject obj, bool value) => obj.SetValue(CircleProperty, value);
        public static readonly DependencyProperty CircleProperty = DependencyProperty.RegisterAttached(
            "Circle", typeof(bool), typeof(Shaatt), new PropertyMetadata(false, OnCircleChanged));

        public static Peaks GetQrPeak(DependencyObject obj) => (Peaks)obj.GetValue(QrPeakProperty);
        public static void SetQrPeak(DependencyObject obj, Peaks value) => obj.SetValue(QrPeakProperty, value);
        public static readonly DependencyProperty QrPeakProperty = DependencyProperty.RegisterAttached(
            "QrPeak", typeof(Peaks), typeof(Shaatt), new PropertyMetadata(Peaks.None, OnQrChanged));

        /// <summary>
        /// Width Percent Size
        /// </summary>
        public static double GetQrThick1(DependencyObject obj) => (double)obj.GetValue(QrThick1Property);
        /// <summary>
        /// Width Percent Size
        /// </summary>
        public static void SetQrThick1(DependencyObject obj, double value) => obj.SetValue(QrThick1Property, value);
        /// <summary>
        /// Width Percent Size
        /// </summary>
        public static readonly DependencyProperty QrThick1Property = DependencyProperty.RegisterAttached(
            "QrThick1", typeof(double), typeof(Shaatt), new PropertyMetadata(0d, OnQrChanged));

        public static double GetQrThick2(DependencyObject obj) => (double)obj.GetValue(QrThick2Property);
        public static void SetQrThick2(DependencyObject obj, double value) => obj.SetValue(QrThick2Property, value);
        public static readonly DependencyProperty QrThick2Property = DependencyProperty.RegisterAttached(
            "QrThick2", typeof(double), typeof(Shaatt), new PropertyMetadata(0d, OnQrChanged));

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

        private static void OnQrChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            const double size = 100d;
            var peak = (Peaks)d.GetValue(QrPeakProperty);
            if (peak == Peaks.None) {
                return;
            }

            var thick1 = (double)d.GetValue(QrThick1Property);
            var thick2 = (double)d.GetValue(QrThick2Property);
            var segments = new PathSegment[4];
            Point start;
            switch (peak) {
                case Peaks.TopLeft:
                    start = new Point();
                    segments[0] = new LineSegment(new Point(size, 0), true);
                    segments[1] = new LineSegment(new Point(size, thick1), true);
                    segments[2] = new ArcSegment(new Point(thick2, size), new Size(size, size), 0, false, SweepDirection.Counterclockwise, true);
                    segments[3] = new LineSegment(new Point(0, size), true);
                    break;
                case Peaks.TopRight:
                    start = new Point();
                    segments[0] = new LineSegment(new Point(size, 0), true);
                    segments[1] = new LineSegment(new Point(size, size), true);
                    segments[2] = new LineSegment(new Point(size - thick2, size), true);
                    segments[3] = new ArcSegment(new Point(0, thick1), new Size(size, size), 0, false, SweepDirection.Counterclockwise, true);
                    break;
                case Peaks.BottomRight:
                    start = new Point(size - thick2, 0);
                    segments[0] = new ArcSegment(new Point(0, size - thick1), new Size(size, size), 0, false, SweepDirection.Clockwise, true);
                    segments[1] = new LineSegment(new Point(0, size), true);
                    segments[2] = new LineSegment(new Point(size, size), true);
                    segments[3] = new LineSegment(new Point(size, 0), true);
                    break;
                case Peaks.BottomLeft:
                default:
                    start = new Point();
                    segments[0] = new LineSegment(new Point(thick2, 0), true);
                    segments[1] = new ArcSegment(new Point(size, size - thick1), new Size(size, size), 0, false, SweepDirection.Counterclockwise, true);
                    segments[2] = new LineSegment(new Point(size, size), true);
                    segments[3] = new LineSegment(new Point(0, size), true);
                    break;
            }

            var figures = new List<PathFigure> {
                new PathFigure(start, segments, true)
            };
            SetPath(d, new PathGeometry(figures));
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
                control.SetValue(Shaatt.CircleProperty, false);
                control.SetValue(Shaatt.RectProperty, false);
                control.SetValue(Shaatt.QrPeakProperty, Peaks.None);
            }
        }
    }
}
