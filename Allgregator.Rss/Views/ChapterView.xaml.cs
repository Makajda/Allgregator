using Allgregator.Aux.Common;
using Allgregator.Aux.Models;
using Allgregator.Rss.ViewModels;
using System.Windows.Controls;

namespace Allgregator.Rss.Views {
    /// <summary>
    /// Interaction logic for ChapterView.xaml
    /// </summary>
    public partial class ChapterView : UserControl, IChapterView {
        public ChapterView() {
            InitializeComponent();
        }

        public void SetChapter(Chapter chapter) {
            if (DataContext is ChapterViewModel viewModel) {
                viewModel.SetIdAndName(chapter.Id, chapter.Name);
            }
        }
    }
}
