using Allgregator.Aux.Common;
using Allgregator.Aux.ViewModels;
using Allgregator.Rss.Models;
using Prism.Commands;

namespace Allgregator.Rss.ViewModels {
    public class OldsViewModel : DataViewModelBase<Data> {
        public OldsViewModel() {
            OpenCommand = new DelegateCommand<Reco>(Open);
        }

        public DelegateCommand<Reco> OpenCommand { get; private set; }

        private void Open(Reco reco) {
            WindowUtilities.Run(reco.Uri);
        }
    }
}

