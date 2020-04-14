using Allgregator.Models.Rss;
using Allgregator.ViewModels.Rss;
using System.Windows.Controls;

namespace Allgregator.Views.Rss {
    /// <summary>
    /// Interaction logic for LinksView.xaml
    /// </summary>
    public partial class LinksView : UserControl {
        public LinksView() {
            InitializeComponent();
        }

        Link originalLink;

        private void dg_BeginningEdit(object sender, DataGridBeginningEditEventArgs e) {
            originalLink = (e.Row.Item as Link)?.Clone();
        }

        private void dg_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e) {
            var link = e.Row.Item as Link;
            if (originalLink != null && link != null) {
                if (DataContext is LinksViewModel viewModel && viewModel.Chapter != null && !viewModel.Chapter.IsNeedToSaveLinks) {
                    viewModel.Chapter.IsNeedToSaveLinks = !originalLink.Equals(link);
                }
            }
        }
    }
}