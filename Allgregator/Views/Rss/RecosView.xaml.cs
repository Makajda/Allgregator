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
                view.Background = isNew ? Brushes.LightGreen : Brushes.SandyBrown;
            }
        }
    }
}
