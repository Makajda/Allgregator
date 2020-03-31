using Allgregator.Models.Rss;
using Allgregator.Repositories.Rss;
using Prism.Mvvm;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Allgregator.Services.Rss {
    public class OreService : BindableBase {
        private SettingsService settingsService;
        private LinkRepository linkRepository;

        private CancellationTokenSource cancellationTokenSource;
        private bool isRetrieveCanceled = true;

        private IProgress<int> progressIndicator;
        private int progressValue;
        private int progressMaximum;

        public OreService(
            SettingsService settingsService,
            LinkRepository linkRepository) {
            this.settingsService = settingsService;
            this.linkRepository = linkRepository;

            progressIndicator = new Progress<int>((one) => ProgressValue++);
        }

        public int ProgressValue {
            get => progressValue;
            private set => SetProperty(ref progressValue, value);
        }

        public int ProgressMaximum {
            get => progressMaximum;
            private set => SetProperty(ref progressMaximum, value);
        }

        public void CancelRetrieve() {
            try {
                if (cancellationTokenSource != null && cancellationTokenSource.Token.CanBeCanceled) {
                    isRetrieveCanceled = true;
                    cancellationTokenSource.Cancel();
                }
            }
            catch (ObjectDisposedException) { }
        }

        public async Task Retrieve(Collection collection) {
            if (collection == null || collection.Links == null || collection.Mined == null) {
                return;
            }

            isRetrieveCanceled = false;
            using (var retrieveService = new RetrieveService()) {
                using (cancellationTokenSource = new CancellationTokenSource()) {
                    ProgressMaximum = collection.Links.Count;
                    ProgressValue = 0;
                    var lastRetrieve = DateTimeOffset.Now;
                    var outdated = collection.Mined.OldRecos?.Where(n => n.PublishDate >= collection.Mined.AcceptTime);
                    await Task.WhenAll(collection.Links.Select(link => Task.Run(() => {
                        retrieveService.Production(link, collection.Mined.AcceptTime, settingsService.CutoffTime, outdated);
                        progressIndicator.Report(1);
                    }, cancellationTokenSource.Token)));

                    if (!isRetrieveCanceled) {
                        collection.Mined.NewRecos = new ObservableCollection<Reco>(retrieveService.NewRecos.OrderBy(n => n.PublishDate));
                        collection.Mined.OldRecos = new ObservableCollection<Reco>(retrieveService.OldRecos.OrderBy(n => n.PublishDate));
                        collection.Mined.Errors = retrieveService.Errors.ToList();
                        collection.Mined.LastRetrieve = lastRetrieve;
                    }
                }
            }
        }
    }
}
