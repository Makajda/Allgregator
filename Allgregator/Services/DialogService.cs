using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace Allgregator.Services {
    public class DialogService {
        private Popup popup;

        public void Show(string message, Action callback) {
            var panel = new Grid() {
                Background = Brushes.Red,
                Width = 398d,
                Height = 246d
            };

            var textBlock = new TextBlock() {
                Text = message,
                Foreground = Brushes.White,
                FontSize = 72d,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };

            panel.Children.Add(textBlock);

            if (callback != null) {
                var button = new Button() {
                    Content = "Ok",
                    Background = Brushes.Yellow,
                    FontStyle = FontStyles.Italic,
                    FontSize = 16,
                    Padding = new Thickness(30),
                    Margin = new Thickness(20),
                    HorizontalAlignment = HorizontalAlignment.Right,
                    VerticalAlignment = VerticalAlignment.Bottom
                };

                button.Click += (s, e) => callback();
                panel.Children.Add(button);
            }

            popup = new Popup() {
                AllowsTransparency = true,
                UseLayoutRounding = true,
                StaysOpen = false,
                Placement = PlacementMode.Mouse,
                Child = panel,
                IsOpen = true
            };
        }
    }
}
