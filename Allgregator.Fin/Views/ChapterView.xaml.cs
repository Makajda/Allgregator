using Allgregator.Aux.Common;
using Allgregator.Aux.Models;
using System.Windows.Controls;

namespace Allgregator.Fin.Views {
    /// <summary>
    /// Interaction logic for ChaptersView
    /// </summary>
    public partial class ChapterView : UserControl, IChapterView {
        public ChapterView() {
            InitializeComponent();
        }

        public void SetChapter(ChapterBase chapter) { }
    }
}
