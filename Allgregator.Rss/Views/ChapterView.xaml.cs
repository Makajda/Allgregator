using Allgregator.Rss.ViewModels;
using System.Windows.Controls;

namespace Allgregator.Rss.Views {
    /// <summary>
    /// Interaction logic for ChapterView.xaml
    /// </summary>
    public partial class ChapterView : UserControl {
        public ChapterView() {
            InitializeComponent();
        }

        public void SetIdAndName(int id, string name) {
            if (DataContext is ChapterViewModel viewModel) {
                viewModel.Data.Id = id;
                viewModel.Data.Name = name;
            }
        }
    }
}
