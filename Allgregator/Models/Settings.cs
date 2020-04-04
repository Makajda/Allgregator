using Prism.Mvvm;
using System.Windows;

namespace Allgregator.Models {
    public class Settings : BindableBase {
        public Rect MainWindowBounds { get; set; }
        public WindowState MainWindowState { get; set; }
        public int RssChapterId { get; set; } = 1;
    }
}
