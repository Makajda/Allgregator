using System.Windows;

namespace Allgregator.Models {
    public class Settings {
        public Rect MainWindowBounds { get; set; }
        public WindowState MainWindowState { get; set; }
        public int RssChapterId { get; set; }
        public int RssMaxOpenTabs { get; set; } = 12;
    }
}
