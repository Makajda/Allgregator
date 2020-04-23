using Allgregator.Aux.Common;
using Allgregator.Rss.Models;
using Prism.Commands;
using Prism.Mvvm;

namespace Allgregator.Rss.ViewModels {
    public class RecosViewModel : BindableBase {
        public RecosViewModel(
            Chapter chapter
            ) {
            Chapter = chapter;

            OpenCommand = new DelegateCommand<Reco>((reco) => WindowUtilities.Run(reco.Uri.ToString()));
            MoveCommand = new DelegateCommand<Reco>(Move);
        }

        public DelegateCommand<Reco> OpenCommand { get; private set; }
        public DelegateCommand<Reco> MoveCommand { get; private set; }
        public Chapter Chapter { get; private set; }

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

