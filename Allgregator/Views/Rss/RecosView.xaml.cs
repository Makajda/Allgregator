using Allgregator.Models.Rss;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

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
            var color1 = (Color)ColorConverter.ConvertFromString("#91C9FF");
            Color color2;
            string part;
            if (IsNew) {
                color2 = Colors.LightGreen;
                part = "New";
            }
            else {
                color2 = Colors.SaddleBrown;
                part = "Old";
            }

            Background = new LinearGradientBrush(color1, color2, 90);
            var path = string.Format("{0}.{1}.{2}{3}s", nameof(Chapter), nameof(Mined), part, nameof(Reco));
            list.SetBinding(ItemsControl.ItemsSourceProperty, new Binding(path));
        }
    }
}
