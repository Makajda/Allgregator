using Allgregator.ViewModels.Rss;
using System.Windows.Controls;

namespace Allgregator.Views.Rss {
    /// <summary>
    /// Interaction logic for ChaptersView.xaml
    /// </summary>
    public partial class ChaptersView : UserControl {
        public ChaptersView() {
            InitializeComponent();
            Loaded += ChaptersView_Loaded;
        }

        private async void ChaptersView_Loaded(object sender, System.Windows.RoutedEventArgs e) {
            if(DataContext is ChaptersViewModel viewModel) {
                await viewModel?.Load();
            }
        }
    }
}
