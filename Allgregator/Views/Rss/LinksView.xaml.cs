using Allgregator.Models.Rss;
using Allgregator.Services;
using Allgregator.ViewModels.Rss;
using System.Windows.Controls;

namespace Allgregator.Views.Rss {
    /// <summary>
    /// Interaction logic for LinksView.xaml
    /// </summary>
    public partial class LinksView : UserControl {
        DialogService dialogService;
        public LinksView(DialogService dialogService) {
            InitializeComponent();
            this.dialogService = dialogService;
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

        private void ButtonDelete_Click(object sender, System.Windows.RoutedEventArgs e) {
            if (dg.SelectedCells.Count > 0) {
                if (dg.SelectedCells[0].Item is Link link && link != null) {
                    dialogService.Show($"{link.Name}?", DeleteReal, 20, true);
                }
            }
        }
 
        private void DeleteReal() {
            if (dg.SelectedCells.Count > 0) {
                if (dg.SelectedCells[0].Item is Link link && link != null) {
                    if (DataContext is LinksViewModel viewModel && viewModel.Chapter != null) {
                        viewModel.Chapter.Links.Remove(link);
                        viewModel.Chapter.IsNeedToSaveLinks = true;
                    }
                }
            }
        }
    }
}