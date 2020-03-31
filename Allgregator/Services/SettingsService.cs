using Prism.Mvvm;
using System;

namespace Allgregator.Services {
    public class SettingsService : BindableBase {
        private DateTimeOffset cutoffTime = DateTimeOffset.Now.AddMonths(-7);

        public int CollectionId { get; set; }

        public DateTimeOffset CutoffTime {
            get => cutoffTime;
            set => SetProperty(ref cutoffTime, value);
        }

        public void Load() {
            //TODO
        }

        public void Save() {
            //TODO
        }
    }
}
