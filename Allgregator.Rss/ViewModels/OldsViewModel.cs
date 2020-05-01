using Allgregator.Aux.Common;
using Allgregator.Rss.Models;
using Prism.Commands;
using Prism.Mvvm;

namespace Allgregator.Rss.ViewModels {
    public class OldsViewModel : BindableBase {
        public OldsViewModel(
            Data data
            ) {
            Data = data;

            OpenCommand = new DelegateCommand<Reco>(Open);
        }

        public DelegateCommand<Reco> OpenCommand { get; private set; }
        public Data Data { get; private set; }

        private void Open(Reco reco) {
            WindowUtilities.Run(reco.Uri);
        }
    }
}

