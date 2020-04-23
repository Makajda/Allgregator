using Allgregator.Rss.Models;
using System.Windows.Controls;
using System.Windows.Data;

namespace Allgregator.Rss.Views {
    /// <summary>
    /// Interaction logic for RecosView
    /// </summary>
    public partial class RecosView : UserControl {
        public RecosView(bool isNew) {
            IsNew = isNew;

            InitializeComponent();

            var path = string.Format("{0}.{1}.{2}", nameof(Chapter), nameof(Mined), IsNew ? nameof(Mined.NewRecos) : nameof(Mined.OldRecos));
            list.SetBinding(ItemsControl.ItemsSourceProperty, new Binding(path));
        }

        public bool IsNew { get; private set; }
    }
}
