using Allgregator.Aux.Common;
using Allgregator.Aux.ViewModels;
using Allgregator.Rss.Models;
using Prism.Commands;

namespace Allgregator.Rss.ViewModels {
    public class NewsViewModel : DataViewModelBase<Data> {
        public NewsViewModel() {
            OpenCommand = new DelegateCommand<Reco>(Open);
            MoveCommand = new DelegateCommand<Reco>(Move);
        }

        public DelegateCommand<Reco> OpenCommand { get; private set; }
        public DelegateCommand<Reco> MoveCommand { get; private set; }

        private void Open(Reco reco) {
            WindowUtilities.Run(reco.Uri);
            Move(reco);
        }

        private void Move(Reco reco) {
            if (Data.Mined != null) {
                Data.Mined.NewRecos.Remove(reco);
                Data.Mined.OldRecos.Insert(0, reco);
                if (Data.Mined.NewRecos.Count == 0) {
                    Data.Mined.AcceptTime = Data.Mined.LastRetrieve;
                }

                Data.Mined.IsNeedToSave = true;
            }
        }
    }
}

