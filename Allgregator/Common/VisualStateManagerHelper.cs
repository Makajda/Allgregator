using System.Windows;
using System.Windows.Controls;

namespace Allgregator.Common {
    public class VisualStateManagerHelper {
        /*

    <VisualStateManager.VisualStateGroups>
        <VisualStateGroup x:Name="Common">
            <VisualState x:Name="newsState">
                <Storyboard>
                    <DoubleAnimation Storyboard.TargetName="news"  Storyboard.TargetProperty="Opacity" To="1" Duration="0:0:0.5" />
                    <DoubleAnimation Storyboard.TargetName="olds"  Storyboard.TargetProperty="Opacity" To="0" Duration="0:0:0.5" />
                    <DoubleAnimation Storyboard.TargetName="links" Storyboard.TargetProperty="Opacity" To="0" Duration="0:0:0.5" />
                </Storyboard>
            </VisualState>
        </VisualStateGroup>
    </VisualStateManager.VisualStateGroups>
         */

        //TODO remove
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
