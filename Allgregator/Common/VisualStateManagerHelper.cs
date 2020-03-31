using System.Windows;
using System.Windows.Controls;

namespace Allgregator.Common {
    public class VisualStateManagerHelper {
        public static readonly DependencyProperty StateProperty =
            DependencyProperty.RegisterAttached(
                "State",
                typeof(string),
                typeof(VisualStateManagerHelper),
                new PropertyMetadata(null, OnStateChanged));

        public static string GetState(DependencyObject obj) {
            return (string)obj.GetValue(StateProperty);
        }

        public static void SetState(DependencyObject obj, string value) {
            obj.SetValue(StateProperty, value);
        }

        private static void OnStateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            if (e.NewValue != null) {
                VisualStateManager.GoToElementState((Control)d, (string)e.NewValue, true);
            }
        }
    }
}
