using Allgregator.Aux.Models;
using Allgregator.Fin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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

        public IEnumerable<Term> Terms {
            get { return (IEnumerable<Term>)GetValue(TermsProperty); }
            set { SetValue(TermsProperty, value); }
        }

        public static readonly DependencyProperty TermsProperty = DependencyProperty.Register("Terms",
            typeof(IEnumerable<Term>), typeof(CurrencyChart), new PropertyMetadata(null, OnTermsChanged));


        public Settings Settings {
            get { return (Settings)GetValue(SettingsProperty); }
            set { SetValue(SettingsProperty, value); }
        }

        public static readonly DependencyProperty SettingsProperty = DependencyProperty.Register("Settings",
            typeof(Settings), typeof(CurrencyChart), new PropertyMetadata(null, OnSettingsChanged));


        private static void OnSettingsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            if (d is CurrencyChart view) {
                view.SettingsChanged();
            }
        }

        private static void OnTermsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            if (d is CurrencyChart view) {
                view.Draw();
            }
        }


        private void SettingsChanged() {
            var settings = (Settings)GetValue(SettingsProperty);
            var currencies = settings?.FinCurrencies;
            var offs = settings?.FinOffs;

            if (currencies == null) return;

            canvas.Children.Clear();
            currenciesControl.Items.Clear();
            foreach (var key in currencies) {
                var toggleButton = new ToggleButton() {
                    Content = key,
                    Margin = new Thickness(2),
                    Padding = new Thickness(20),
                    IsChecked = offs == null || !offs.Contains(key)
                };
                currenciesControl.Items.Add(toggleButton);
                toggleButton.Checked += (s, e) => ChangeOffs(key, true);
                toggleButton.Unchecked += (s, e) => ChangeOffs(key, false);
            }

            Draw();
        }

        private void ChangeOffs(string key, bool isOn) {
            var settings = (Settings)GetValue(SettingsProperty);
            if (settings != null) { 
                if (isOn) {
                    if (settings.FinOffs != null) {
                        settings.FinOffs = settings.FinOffs.Where(n => n != key);
                    }
                }
                else {
                    if (settings.FinOffs != null) {
                        settings.FinOffs = settings.FinOffs.Union(new[] { key });
                    }
                    else {
                        settings.FinOffs = new[] { key };
                    }
                }
            }

            Draw();
        }

        private void Draw() {
            const double dayWidth = 30d;
            const double marginHost = 50d;
            const int lineIndent = 11;
            const double ellipseSize = 18;

            var heightHost = canvas.ActualHeight - marginHost * 2d;
            if (heightHost <= 0) return;

            var terms = (IEnumerable<Term>)GetValue(TermsProperty);
            var settings = (Settings)GetValue(SettingsProperty);
            var currencies = settings?.FinCurrencies;
            var offs = settings?.FinOffs;

            if (terms == null || currencies == null || terms.Any(n => n.Values == null)) return;

            canvas.Children.Clear();

            var max = new Dictionary<string, decimal>();
            var min = new Dictionary<string, decimal>();

            foreach (var term in terms) {
                foreach (var (key, value) in term.Values) {
                    if (offs == null || !offs.Contains(key)) {
                        if (!max.ContainsKey(key)) max.Add(key, decimal.MinValue);
                        if (value > max[key]) max[key] = value;
                        if (!min.ContainsKey(key)) min.Add(key, decimal.MaxValue);
                        if (value < min[key]) min[key] = value;
                    }
                }
            }

            var prevs = new Dictionary<string, Point>(currencies.Select(n => new KeyValuePair<string, Point>(n, new Point())));
            var day = 0;
            foreach (var term in terms) {
                foreach (var (key, value) in term.Values) {
                    if (offs == null || !offs.Contains(key)) {
                        var brush = curBrushes?.Value[key] ?? Brushes.Black;
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
            var settings = (Settings)GetValue(SettingsProperty);
            var currencies = settings?.FinCurrencies;
            if (currencies != null) {
                var result = new Dictionary<string, Brush>();
                foreach (var key in currencies) {
                    var brush = (Brush)TryFindResource($"Fin.{key}");
                    result.Add(key, brush ?? Brushes.Black);
                }

                return result;
            }

            return null;
        }

        private Brush ForegroundFactory() {
            return (Brush)TryFindResource("Fin.Foreground") ?? Brushes.Black;
        }

        private void Scroll_SizeChanged(object sender, SizeChangedEventArgs e) {
            Draw();
        }
    }
}
