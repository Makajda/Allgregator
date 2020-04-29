using Allgregator.Fin.Common;
using Allgregator.Fin.Models;
using System;
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
            const double marginHost = 50d;
            const int lineIndent = 11;
            const double ellipseSize = 18;

            var heightHost = canvas.ActualHeight - marginHost * 2;
            if (heightHost <= 0) return;

            var prevs = new Dictionary<string, Point>();
            foreach (var name in Given.CurrencyNames) prevs.Add(name, new Point());
            var day = 0;
            foreach (var currency in currencies) {
                foreach (var (key, value) in currency.Values) {
                    if (Given.CurrencyNames.Contains(key)) {
                        if (Given.CurrencyBrushes.TryGetValue(key, out Brush brush)) {
                            var delta = max[key] - min[key];
                            var y = delta == 0m ?
                                heightHost / 2d :
                                (double)(value - min[key]) * heightHost / (double)delta;
                            var nextPoint = new Point(day * dayWidth + marginHost, heightHost - y + marginHost);

                            if (day > 0) {
                                var x1 = prevs[key].X;
                                var y1 = prevs[key].Y;
                                var x2 = nextPoint.X;
                                var y2 = nextPoint.Y;
                                var diagonal = Math.Sqrt(Math.Pow(x1 - x2, 2) + Math.Pow(y1 - y2, 2));
                                var sinx = (x2 - x1) / diagonal;
                                var siny = (y2 - y1) / diagonal;
                                var dxIndend = lineIndent * sinx;
                                var dyIndent = lineIndent * siny;
                                var line = new Line() {
                                    X1 = x1 + dxIndend,
                                    Y1 = y1 + dyIndent,
                                    X2 = x2 - dxIndend,
                                    Y2 = y2 - dyIndent,
                                    Stroke = brush,
                                    StrokeThickness = key == Given.CurrencyNames.FirstOrDefault() ? 3 : 1
                                };
                                canvas.Children.Add(line);
                            }

                            var ellipse = new Ellipse() {
                                Width = ellipseSize,
                                Height = ellipseSize,
                                Fill = Brushes.Transparent,
                                Stroke = brush,
                                ToolTip = $"{currency.Date.Day} - {value}"
                            };
                            Canvas.SetLeft(ellipse, nextPoint.X - ellipseSize / 2d);
                            Canvas.SetTop(ellipse, nextPoint.Y - ellipseSize / 2d);
                            canvas.Children.Add(ellipse);
                            if (nextPoint.X > canvas.ActualWidth) canvas.Width = nextPoint.X + marginHost;

                            prevs[key] = nextPoint;
                        }
                    }
                }

                var textBlock = new TextBlock() {
                    Text = $"{currency.Date.Day:D2}",
                    TextAlignment = TextAlignment.Left,
                    Width = dayWidth
                };
                Canvas.SetLeft(textBlock, day * dayWidth + marginHost - ellipseSize / 2d);
                canvas.Children.Add(textBlock);

                day++;
            }

            scroll.ScrollToHorizontalOffset(double.MaxValue);
        }
    }
}
