using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace Allgregator.Aux.Services {
    public class DialogService {
        public void Show(string message, Action callback = null, double fontSize = 16, bool isStrikethrough = false) {
            var (popup, host) = Create();

            var panel = new DockPanel() {
                Background = Brushes.Red,
            };

            host.Content = panel;

            if (callback != null) {
                var button = new Button() {
                    Content = '\u2714',
                    Background = Brushes.Yellow,
                    Foreground = Brushes.Green,
                    FontStyle = FontStyles.Italic,
                    FontSize = 28,
                    Padding = new Thickness(20),
                    Margin = new Thickness(20),
                    VerticalAlignment = VerticalAlignment.Center
                };

                button.SetValue(DockPanel.DockProperty, Dock.Right);
                button.Click += (s, e) => { popup.IsOpen = false; callback(); };
                panel.Children.Add(button);
            }

            var textBlock = new TextBlock() {
                Text = message,
                Foreground = Brushes.White,
                FontSize = fontSize,
                Margin = new Thickness(10),
                TextWrapping = TextWrapping.Wrap,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };

            if (isStrikethrough) {
                textBlock.TextDecorations = TextDecorations.Strikethrough;
            }

            panel.Children.Add(textBlock);
        }

        public void Show(IEnumerable<object> messages, Action<object> callback = null, double fontSize = 16) {
            var (popup, host) = Create();

            var itemsControl = new ItemsControl() {
                Foreground = Brushes.White,
                FontSize = fontSize
            };

            host.Content = itemsControl;

            foreach (var message in messages) {
                var button = new Button { BorderThickness = new Thickness(0), Margin = new Thickness(0, 0, 0, 1) };
                if (message is Color color) {
                    button.Background = new SolidColorBrush { Color = color };
                    button.Height = 40d;
                }
                else {
                    button.Content = message;
                    button.Padding = new Thickness(20);
                }

                button.Click += (s, e) => {
                    popup.IsOpen = false; callback?.Invoke(message);
                };

                itemsControl.Items.Add(button);
            }
        }

        private static (Popup, ContentControl) Create() {
            var popup = new Popup() {
                AllowsTransparency = true,
                Opacity = 0.8d,
                Width = 400d,
                MaxHeight = 647d,
                UseLayoutRounding = true,
                StaysOpen = false,
                Placement = PlacementMode.Mouse,
                IsOpen = true
            };

            var scrollViewer = new ScrollViewer() {
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                HorizontalScrollBarVisibility = ScrollBarVisibility.Disabled
            };

            popup.Child = scrollViewer;

            return (popup, scrollViewer);
        }
    }
}