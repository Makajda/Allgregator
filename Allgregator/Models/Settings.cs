using Prism.Mvvm;
using System;
using System.Windows;

namespace Allgregator.Models {
    public class Settings : BindableBase {
        public Rect MainWindowBounds { get; set; }
        public WindowState MainWindowState { get; set; }
        public int RssChapterId { get; set; }

        private int rssMaxOpenTabs = 12;
        public int RssMaxOpenTabs {
            get { return rssMaxOpenTabs; }
            set { SetProperty(ref rssMaxOpenTabs, value); }
        }

        private DateTimeOffset cutoffTime;
        public DateTimeOffset CutoffTime {
            get => cutoffTime;
            set => SetProperty(ref cutoffTime, value);
        }
    }
}
