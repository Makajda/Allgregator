using Allgregator.Models.Rss;
using System.Windows.Controls;
using System.Windows.Data;

namespace Allgregator.Views.Rss {
    /// <summary>
    /// Interaction logic for RecosView
    /// </summary>
    public partial class RecosView : UserControl {
        public RecosView(bool isNew) {
            IsNew = isNew;

            InitializeComponent();

            var path = string.Format("{0}.{1}.{2}{3}s", nameof(Chapter), nameof(Mined), IsNew ? "New" : "Old", nameof(Reco));
            list.SetBinding(ItemsControl.ItemsSourceProperty, new Binding(path));
        }

        public bool IsNew { get; private set; }
    }
}
