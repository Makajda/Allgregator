using Allgregator.Fin.Common;
using Allgregator.Fin.Models;
using Allgregator.Fin.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Allgregator.Fin.Views {
    /// <summary>
    /// Interaction logic for CurrencyChart.xaml
    /// </summary>
    public partial class CurrencyChart : UserControl {
        private readonly Lazy<Dictionary<string, Brush>> curBrushes;
        private readonly Lazy<Brush> foreground;

        public CurrencyChart() {
            InitializeComponent();

            curBrushes = new Lazy<Dictionary<string, Brush>>(BrushesFactory);
            foreground = new Lazy<Brush>(ForegroundFactory);
        }

        public IEnumerable<Term> ItemsSource {
            get { return (IEnumerable<Term>)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(IEnumerable<Term>), typeof(CurrencyChart), new PropertyMetadata(null, OnItemsSourceChanged));

        private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            if (d is CurrencyChart view) {
                view.Draw();
            }
        }

        private void Draw() {
            const double dayWidth = 30d;
            const double marginHost = 50d;
            const int lineIndent = 11;
            const double ellipseSize = 18;

            var heightHost = canvas.ActualHeight - marginHost * 2d;
            if (heightHost <= 0) return;

            canvas.Children.Clear();

            var terms = (IEnumerable<Term>)GetValue(ItemsSourceProperty);
            var currencies = (IEnumerable<Currency>)currenciesControl.ItemsSource;
            if (terms == null || currencies == null) return;
            if (terms.Any(n => n.Values == null)) return;

            var max = new Dictionary<string, decimal>();
            var min = new Dictionary<string, decimal>();

            foreach (var term in terms) {
                foreach (var (key, value) in term.Values) {
                    if (!max.ContainsKey(key)) max.Add(key, decimal.MinValue);
                    if (value > max[key]) max[key] = value;
                    if (!min.ContainsKey(key)) min.Add(key, decimal.MaxValue);
                    if (value < min[key]) min[key] = value;
                }
            }

            var prevs = new Dictionary<string, Point>();
            foreach (var name in Given.CurrencyNames) prevs.Add(name, new Point());
            var day = 0;
            foreach (var term in terms) {
                foreach (var (key, value) in term.Values) {
                    if (currencies.FirstOrDefault(n => n.Key == key && n.IsOn) != null) {
                        var brush = curBrushes.Value[key];
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
                                StrokeThickness = 3
                            };
                            canvas.Children.Add(line);
                        }

                        var ellipse = new Ellipse() {
                            Width = ellipseSize,
                            Height = ellipseSize,
                            Fill = Brushes.Transparent,
                            Stroke = brush,
                            ToolTip = $"{term.Date.Day} - {value}"
                        };
                        Canvas.SetLeft(ellipse, nextPoint.X - ellipseSize / 2d);
                        Canvas.SetTop(ellipse, nextPoint.Y - ellipseSize / 2d);
                        canvas.Children.Add(ellipse);
                        if (nextPoint.X > canvas.ActualWidth) canvas.Width = nextPoint.X + marginHost;

                        prevs[key] = nextPoint;
                    }
                }

                var textBlock = new TextBlock() {
                    Text = $"{term.Date.Day:D2}",
                    TextAlignment = TextAlignment.Left,
                    Foreground = foreground.Value,
                    Width = dayWidth
                };
                Canvas.SetLeft(textBlock, day * dayWidth + marginHost - ellipseSize / 2d);
                canvas.Children.Add(textBlock);

                day++;
            }

            scroll.ScrollToHorizontalOffset(double.MaxValue);
        }

        private Dictionary<string, Brush> BrushesFactory() {
            var result = new Dictionary<string, Brush>();
            foreach (var name in Given.CurrencyNames) {
                var brush = (Brush)TryFindResource($"Fin.{name}");
                result.Add(name, brush ?? Brushes.Black);
            }

            return result;
        }

        private Brush ForegroundFactory() {
            return (Brush)TryFindResource("Fin.Foreground") ?? Brushes.Black;
        }

        private void Scroll_SizeChanged(object sender, SizeChangedEventArgs e) {
            Draw();
        }

        private void ToggleButton_Checked(object sender, RoutedEventArgs e) {
            Draw();
        }
    }
}
