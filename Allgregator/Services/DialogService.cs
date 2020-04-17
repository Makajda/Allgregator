using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace Allgregator.Services {
    public class DialogService {
        public void Show(string message, Action callback = null, double fontSize = 16, bool isStrikethrough = false) {
            var (popup, grid) = Create();

            var textBlock = new TextBlock() {
                Text = message,
                Foreground = Brushes.White,
                FontSize = fontSize,
                TextWrapping = TextWrapping.Wrap,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };

            if (isStrikethrough) {
                textBlock.TextDecorations = TextDecorations.Strikethrough;
            }

            grid.Children.Add(textBlock);

            if (callback != null) {
                var button = new Button() {
                    Content = '\u2714',
                    Background = Brushes.Yellow,
                    FontStyle = FontStyles.Italic,
                    FontSize = 28,
                    Padding = new Thickness(20),
                    Margin = new Thickness(20),
                    HorizontalAlignment = HorizontalAlignment.Right,
                    VerticalAlignment = VerticalAlignment.Bottom
                };

                button.Click += (s, e) => { popup.IsOpen = false; callback(); };
                grid.Children.Add(button);
            }
        }

        public void Show(IEnumerable<string> messages, Action<string> callback = null, double fontSize = 16) {
            var (popup, grid) = Create();

            var scrollViewer = new ScrollViewer() {
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                HorizontalScrollBarVisibility = ScrollBarVisibility.Auto
            };

            var itemsControl = new ItemsControl() {
                Foreground = Brushes.White,
                FontSize = fontSize
            };

            scrollViewer.Content = itemsControl;
            grid.Children.Add(scrollViewer);

            foreach (var message in messages) {
                var button = new Button() {
                    Content = message,
                    Padding = new Thickness(20)
                };

                button.Click += (s, e) => {
                    popup.IsOpen = false; callback?.Invoke(message);
                };

                itemsControl.Items.Add(button);
            }
        }

        private (Popup, Grid) Create() {
            var panel = new Grid() {
                Background = Brushes.Red,
                Width = 398d,
                Height = 246d
            };

            var popup = new Popup() {
                AllowsTransparency = true,
                UseLayoutRounding = true,
                StaysOpen = false,
                Placement = PlacementMode.Mouse,
                Child = panel,
                IsOpen = true
            };

            return (popup, panel);
        }
    }
}