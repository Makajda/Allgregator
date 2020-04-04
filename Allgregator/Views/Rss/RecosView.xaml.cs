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
        }

        public RecosView(bool? isNew)
            : this() {
            IsNew = isNew;
        }

        public bool? IsNew {
            get => (bool?)GetValue(IsNewProperty);
            set => SetValue(IsNewProperty, value);
        }

        public static readonly DependencyProperty IsNewProperty =
            DependencyProperty.Register("IsNew", typeof(bool?), typeof(RecosView), new PropertyMetadata(null, OnIsNewChanged));

        private static void OnIsNewChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            if (e.NewValue != null) {
                var view = (RecosView)d;
                var isNew = ((bool?)e.NewValue).Value;

                var path = string.Format("{0}.{1}.{2}{3}s", nameof(Chapter), nameof(Mined), isNew ? "New" : "Old", nameof(Reco));
                view.list.SetBinding(ItemsControl.ItemsSourceProperty, new Binding(path));
                view.Background = isNew ? Brushes.LightBlue : Brushes.SandyBrown;
            }
        }

        private void Grid_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e) {
            ((Border)sender).Background = Brushes.Azure;
        }

        private void Grid_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e) {
            ((Border)sender).Background = Brushes.White;
        }

        private void Path_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e) {
            ((Path)sender).Fill= Brushes.SandyBrown;
        }

        private void Path_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e) {
            ((Path)sender).Fill = Brushes.Transparent;
        }
    }
}
