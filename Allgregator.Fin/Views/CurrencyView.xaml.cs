using Allgregator.Fin.Common;
using Allgregator.Fin.Models;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Allgregator.Fin.Views {
    /// <summary>
    /// Interaction logic for CurrencyView
    /// </summary>
    public partial class CurrencyView : UserControl {
        public CurrencyView() {
            InitializeComponent();

            var path = $"{nameof(Mined)}.{nameof(Mined.Currencies)}";
            SetBinding(ItemsSourceProperty, new Binding(path));
        }

        public IEnumerable<Currency> ItemsSource {
            get { return (IEnumerable<Currency>)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(IEnumerable<Currency>), typeof(CurrencyView), new PropertyMetadata(null, OnItemsSourceChanged));

        private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            if (d is CurrencyView view) {
                if (e.NewValue is IEnumerable<Currency> currencies) {
                    view.UpdateLayout();
                    view.Draw(currencies);
                }
            }
        }

        private void Draw(IEnumerable<Currency> currencies) {
            if (currencies == null) return;
            if (currencies.Any(n => n.Values == null)) return;

            var max = new Dictionary<string, decimal>();
            var min = new Dictionary<string, decimal>();

            foreach (var currency in currencies) {
                foreach (var (key, value) in currency.Values) {
                    if (!max.ContainsKey(key)) max.Add(key, decimal.MinValue);
                    if (value > max[key]) max[key] = value;
                    if (!min.ContainsKey(key)) min.Add(key, decimal.MaxValue);
                    if (value < min[key]) min[key] = value;
                }
            }

            const double dayWidth = 30d;
            var marginHost = 50d;
            var heightHost = canvas.ActualHeight - marginHost * 2;
            if (heightHost <= 0) return;

            var prevs = new Dictionary<string, Point>();
            foreach (var data in Given.CurrencyData) prevs.Add(data.Key, new Point());
            var day = 0;
            foreach (var currency in currencies) {
                foreach (var (key, value) in currency.Values) {
                    var delta = max[key] - min[key];
                    var y = marginHost + (delta == 0m ?
                        heightHost / 2d :
                        (double)(value - min[key]) * heightHost / (double)delta);
                    var nextPoint = new Point(day * dayWidth + marginHost, y);
                    if (day > 0) {
                        var line = new Line() { X1 = prevs[key].X, Y1 = prevs[key].Y, X2 = nextPoint.X, Y2 = nextPoint.Y };
                        line.Stroke = Given.CurrencyData[key];
                        line.StrokeThickness = key == Given.CurrencyData.First().Key ? 3 : 1;
                        canvas.Children.Add(line);
                    }

                    const double ellipseSize = 18;
                    var ellipse = new Ellipse() { Width = ellipseSize, Height = ellipseSize };
                    Canvas.SetLeft(ellipse, nextPoint.X - ellipseSize / 2d);
                    Canvas.SetTop(ellipse, nextPoint.Y - ellipseSize / 2d);
                    ellipse.Fill = Brushes.Transparent;
                    ellipse.Stroke = Given.CurrencyData[key];
                    ellipse.ToolTip = value;
                    canvas.Children.Add(ellipse);
                    if (nextPoint.X > canvas.ActualWidth) canvas.Width = nextPoint.X + marginHost;

                    prevs[key] = nextPoint;
                }

                day++;
            }

            scroll.ScrollToHorizontalOffset(double.MaxValue);
        }
    }
}
