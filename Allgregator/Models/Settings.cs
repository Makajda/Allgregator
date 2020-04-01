using Prism.Mvvm;
using System;
using System.Windows;

namespace Allgregator.Models {
    public class Settings : BindableBase {
        public Rect MainWindowBounds { get; set; }
        public WindowState MainWindowState { get; set; }
        public int RssCollectionId { get; set; } = 1;

        private DateTimeOffset rssCutoffTime = DateTimeOffset.Now.AddMonths(-7);
        public DateTimeOffset RssCutoffTime {
            get => rssCutoffTime;
            set => SetProperty(ref rssCutoffTime, value);
        }
    }
}
