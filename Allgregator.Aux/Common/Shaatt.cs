using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Allgregator.Aux.Common {
    public enum Shapes {
        Path,
        Rect,
        Circle,
        TopLeft,
        TopRight,
        BottomRight,
        BottomLeft,
        CapTop,
        CapLeft,
        TriTop,
        TriBottom
    }

    public class ButtonShaatt : Button {
        public ButtonShaatt(Shapes shapes) => Shaatt.SetShape(this, shapes);
    }

    public class Shaatt {
        public static Shapes GetShape(DependencyObject obj) => (Shapes)obj.GetValue(ShapeProperty);
        public static void SetShape(DependencyObject obj, Shapes value) => obj.SetValue(ShapeProperty, value);
        public static readonly DependencyProperty ShapeProperty = DependencyProperty.RegisterAttached(
            "Shape", typeof(Shapes), typeof(Shaatt), new PropertyMetadata(Shapes.Path, OnShapeChanged));

        public static Size GetThick(DependencyObject obj) => (Size)obj.GetValue(ThickProperty);
        public static void SetThick(DependencyObject obj, Size value) => obj.SetValue(ThickProperty, value);
        public static readonly DependencyProperty ThickProperty = DependencyProperty.RegisterAttached(
            "Thick", typeof(Size), typeof(Shaatt), new PropertyMetadata(default(Size), OnShapeChanged));

        public static Geometry GetPath(DependencyObject obj) => (Geometry)obj.GetValue(PathProperty);
        public static void SetPath(DependencyObject obj, Geometry value) => obj.SetValue(PathProperty, value);
        public static readonly DependencyProperty PathProperty = DependencyProperty.RegisterAttached(
            "Path", typeof(Geometry), typeof(Shaatt), new PropertyMetadata(null, OnPathChanged));

        private static void OnShapeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            const double size = 100d;
            var shape = (Shapes)d.GetValue(ShapeProperty);
            if (shape == Shapes.Path) {
                OnPathChanged(d, e);
                return;
            }

            var thick1 = ((Size)d.GetValue(ThickProperty)).Width;
            var thick2 = ((Size)d.GetValue(ThickProperty)).Height;
            Geometry geometry = shape switch {
                Shapes.Rect => new RectangleGeometry() { Rect = new Rect(0, 0, 1, 1) },//M0,0 V1 H1 V0 Z
                Shapes.Circle => new EllipseGeometry() { RadiusX = 1d, RadiusY = 1d },//M0,1 A1,1 0 1 1 2,1 A1,1 0 1 1 0,1
                Shapes.TopLeft => new PathGeometry(new[] { new PathFigure(new Point(), new PathSegment[] {
                        new LineSegment(new Point(size, 0), true),
                        new LineSegment(new Point(size, thick1), true),
                        new ArcSegment(new Point(thick2, size), new Size(size, size), 0, false, SweepDirection.Counterclockwise, true),
                        new LineSegment(new Point(0, size), true)
                    }, true) }),
                Shapes.TopRight => new PathGeometry(new[] { new PathFigure(new Point(), new PathSegment[] {
                        new LineSegment(new Point(size, 0), true),
                        new LineSegment(new Point(size, size), true),
                        new LineSegment(new Point(size-thick2, size), true),
                        new ArcSegment(new Point(0,thick1), new Size(size, size), 0, false, SweepDirection.Counterclockwise, true)
                    }, true) }),
                Shapes.BottomRight => new PathGeometry(new[] { new PathFigure(new Point(size - thick2, 0), new PathSegment[] {
                        new ArcSegment(new Point(0, size - thick1), new Size(size, size), 0, false, SweepDirection.Clockwise, true),
                        new LineSegment(new Point(0, size), true),
                        new LineSegment(new Point(size, size), true),
                        new LineSegment(new Point(size, 0), true)
                    }, true) }),
                Shapes.BottomLeft => new PathGeometry(new[] { new PathFigure(new Point(), new PathSegment[] {
                        new LineSegment(new Point(thick2, 0), true),
                        new ArcSegment(new Point(size, size - thick1), new Size(size, size), 0, false, SweepDirection.Counterclockwise, true),
                        new LineSegment(new Point(size, size), true),
                        new LineSegment(new Point(0, size), true)
                    }, true) }),
                Shapes.CapTop => new PathGeometry(new[] { new PathFigure(new Point(), new PathSegment[] {
                        new ArcSegment(new Point(size, 0), new Size(size, size), 0, false, SweepDirection.Clockwise, true),
                        new LineSegment(new Point(size, size), true),
                        new LineSegment(new Point(0, size), true)
                    }, true) }),
                Shapes.CapLeft => new PathGeometry(new[] { new PathFigure(new Point(), new PathSegment[] {
                        new ArcSegment(new Point(0, size), new Size(size, size), 0, false, SweepDirection.Counterclockwise, true),
                        new LineSegment(new Point(size, size), true),
                        new LineSegment(new Point(size, 0), true)
                    }, true) }),
                Shapes.TriTop => new PathGeometry(new[] { new PathFigure(new Point(size / 2d, 0), new PathSegment[] {
                        new LineSegment(new Point(size, size), true),
                        new LineSegment(new Point(0, size), true)
                    }, true) }),
                Shapes.TriBottom => new PathGeometry(new[] { new PathFigure(new Point(), new PathSegment[] {
                        new LineSegment(new Point(size, 0), true),
                        new LineSegment(new Point(size / 2d, size), true)
                    }, true) }),
                _ => new RectangleGeometry() { Rect = new Rect(0, 0, 1, 1) }
            };

            SetPath(d, geometry);
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
