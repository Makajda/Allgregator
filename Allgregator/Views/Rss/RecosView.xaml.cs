using Allgregator.Models.Rss;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Allgregator.Views.Rss {
    /// <summary>
    /// Interaction logic for RecosView
    /// </summary>
    public partial class RecosView : UserControl {
        public RecosView() {
            InitializeComponent();

            Loaded += RecosView_Loaded;
        }

        public RecosView(bool isNew)
            : this() {
            IsNew = isNew;
        }

        public bool IsNew { get; private set; }

        private void RecosView_Loaded(object sender, RoutedEventArgs e) {
            var color = (Color)ColorConverter.ConvertFromString("#91C9FF");
            string part;
            if (IsNew) {
                Background = new SolidColorBrush(color);
                part = "New";
            }
            else {
                Background = new LinearGradientBrush(color, Colors.SandyBrown, 90);
                part = "Old";
            }

            var path = string.Format("{0}.{1}.{2}{3}s", nameof(Chapter), nameof(Mined), part, nameof(Reco));
            list.SetBinding(ItemsControl.ItemsSourceProperty, new Binding(path));
        }

        private void Grid_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e) {
            ((Border)sender).Background = Brushes.Azure;
        }

        private void Grid_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e) {
            ((Border)sender).Background = Brushes.White;
        }

        private void Path_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e) {
            ((Path)sender).Fill = Brushes.SandyBrown;
        }

        private void Path_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e) {
            ((Path)sender).Fill = Brushes.Transparent;
        }
    }
}
