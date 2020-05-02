using Allgregator.Rss.Models;
using Allgregator.Rss.ViewModels;
using System.Windows.Controls;

namespace Allgregator.Rss.Views {
    /// <summary>
    /// Interaction logic for LinksView.xaml
    /// </summary>
    public partial class LinksView : UserControl {
        public LinksView() {
            InitializeComponent();
        }

        Link originalLink;

        private void Dg_BeginningEdit(object sender, DataGridBeginningEditEventArgs e) {
            originalLink = (e.Row.Item as Link)?.Clone();
        }

        private void Dg_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e) {//todo mvvm
            if (originalLink != null && e.Row.Item is Link link) {
                if (DataContext is LinksViewModel viewModel && viewModel.Data != null && !viewModel.Data.Linked.IsNeedToSave) {
                    viewModel.Data.Linked.IsNeedToSave = !originalLink.Equals(link);
                }
            }
        }
    }
}