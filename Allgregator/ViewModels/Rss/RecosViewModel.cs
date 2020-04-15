using Allgregator.Common;
using Allgregator.Models.Rss;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;

namespace Allgregator.ViewModels.Rss {
    public class RecosViewModel : BindableBase {
        public RecosViewModel(
            IEventAggregator eventAggregator
            ) {
            OpenCommand = new DelegateCommand<Reco>((reco) => WindowUtilities.Run(reco.Uri.ToString()));
            MoveCommand = new DelegateCommand<Reco>(Move);

            eventAggregator.GetEvent<CurrentChapterChangedEvent>().Subscribe(chapter => Chapter = chapter);
        }

        public DelegateCommand<Reco> OpenCommand { get; private set; }
        public DelegateCommand<Reco> MoveCommand { get; private set; }

        private Chapter chapter;
        public Chapter Chapter {
            get => chapter;
            private set => SetProperty(ref chapter, value);
        }

        private void Move(Reco reco) {
            var mined = Chapter.Mined;
            mined.NewRecos.Remove(reco);
            mined.OldRecos.Insert(0, reco);
            if (mined.NewRecos.Count == 0) {
                mined.AcceptTime = mined.LastRetrieve;
            }

            Chapter.Mined.IsNeedToSave = true;
        }
    }
}

