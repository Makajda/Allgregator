using Allgregator.Models;
using Allgregator.Models.Rss;
using Prism.Mvvm;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Allgregator.Services.Rss {
    public class OreService : BindableBase {
        private readonly Settings settings;
        private CancellationTokenSource cancellationTokenSource;
        private IProgress<int> progressIndicator;
        private int progressValue;
        private int progressMaximum;

        public OreService(
            Settings settings
            ) {
            this.settings = settings;

            progressIndicator = new Progress<int>((one) => ProgressValue++);
        }

        private bool isRetrieving;
        public bool IsRetrieving {
            get { return isRetrieving; }
            set { SetProperty(ref isRetrieving, value); }
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
                    IsRetrieving = false;
                    cancellationTokenSource.Cancel();
                }
            }
            catch (ObjectDisposedException) { }
        }

        public async Task Retrieve(Chapter chapter) {
            if (chapter == null || chapter.Links == null || chapter.Mined == null) {
                return;
            }

            IsRetrieving = true;
            using (var retrieveService = new RetrieveService()) {
                using (cancellationTokenSource = new CancellationTokenSource()) {
                    ProgressMaximum = chapter.Links.Count + 1;
                    ProgressValue = 1;
                    var lastRetrieve = DateTimeOffset.Now;
                    var outdated = chapter.Mined.OldRecos?.Where(n => n.PublishDate >= chapter.Mined.AcceptTime);
                    await Task.WhenAll(chapter.Links.Select(link => Task.Run(() => {
                        retrieveService.Production(link, chapter.Mined.AcceptTime, settings.RssCutoffTime, outdated);
                        progressIndicator.Report(1);
                    }, cancellationTokenSource.Token)));

                    if (IsRetrieving) {
                        chapter.Mined.NewRecos = new ObservableCollection<Reco>(retrieveService.NewRecos.OrderBy(n => n.PublishDate));
                        chapter.Mined.OldRecos = new ObservableCollection<Reco>(retrieveService.OldRecos.OrderBy(n => n.PublishDate));
                        chapter.Mined.Errors = retrieveService.Errors.Count == 0 ? null : retrieveService.Errors.ToList();
                        chapter.Mined.LastRetrieve = lastRetrieve;
                        IsRetrieving = false;
                    }
                }
            }
        }
    }
}
