using Prism.Mvvm;
using System;

namespace Allgregator.Models {
    public class Settings : BindableBase {
        public int RssCollectionId { get; set; }

        private DateTimeOffset rssCutoffTime = DateTimeOffset.Now.AddMonths(-7);
        public DateTimeOffset RssCutoffTime {
            get => rssCutoffTime;
            set => SetProperty(ref rssCutoffTime, value);
        }
    }
}
