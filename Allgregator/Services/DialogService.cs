using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace Allgregator.Services {
    public class DialogService {
        public void Show(string message, Action<object> callback, object parameter = null, double fontSize = 16, bool isStrikethrough = false) {
            var panel = new Grid() {
                Background = Brushes.Red,
                Width = 398d,
                Height = 246d
            };

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

            panel.Children.Add(textBlock);

            var popup = new Popup() {
                AllowsTransparency = true,
                UseLayoutRounding = true,
                StaysOpen = false,
                Placement = PlacementMode.Mouse,
                Child = panel,
                IsOpen = true
            };

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

                button.Click += (s, e) => { popup.IsOpen = false; callback(parameter); };
                panel.Children.Add(button);
            }
        }
    }
}
