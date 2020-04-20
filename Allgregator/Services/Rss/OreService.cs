using Allgregator.Models.Rss;
using Prism.Mvvm;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Allgregator.Services.Rss {
    public class OreService : BindableBase {
        private CancellationTokenSource cancellationTokenSource;
        private IProgress<int> progressIndicator;
        private int progressValue;
        private int progressMaximum;

        public OreService() {
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

        public async Task Retrieve(Chapter chapter, DateTimeOffset cutoffTime) {
            if (chapter?.Linked?.Links == null || chapter.Mined == null) {
                return;
            }

            ProgressMaximum = chapter.Linked.Links.Count + 1;
            ProgressValue = 1;


            using var retrieveService = new RetrieveService();
            using (cancellationTokenSource = new CancellationTokenSource()) {
                IsRetrieving = true;
                var cancellationToken = cancellationTokenSource.Token;
                var lastRetrieve = DateTimeOffset.Now;

                try {
                    await Task.WhenAll(
                        Partitioner.Create(chapter.Linked.Links).GetPartitions(9).Select(partition => {
                            var task = new Task(() => {
                                using (partition) {
                                    while (partition.MoveNext()) {
                                        retrieveService.Production(partition.Current, cutoffTime);
                                        progressIndicator.Report(1);
                                    }
                                };
                            }, cancellationTokenSource.Token, TaskCreationOptions.LongRunning);
                            task.Start();
                            return task;
                        }));
                }
                catch (OperationCanceledException) {
                }

                if (IsRetrieving) {
                    var mined = chapter.Mined;
                    var newRecos = new List<Reco>();
                    var oldRecos = new List<Reco>();
                    var outdated = mined.OldRecos?.Where(n => n.PublishDate >= chapter.Mined.AcceptTime);

                    foreach (var reco in retrieveService.Recos) {
                        if (reco.PublishDate > mined.AcceptTime) {
                            if (outdated?.FirstOrDefault(n => n.Equals(reco)) == null) {
                                newRecos.Add(reco);
                                continue;
                            }
                        }

                        oldRecos.Add(reco);
                    }

                    mined.NewRecos = new ObservableCollection<Reco>(newRecos.OrderByDescending(n => n.PublishDate));
                    mined.OldRecos = new ObservableCollection<Reco>(oldRecos.OrderByDescending(n => n.PublishDate));
                    mined.Errors = retrieveService.Errors.Count == 0 ? null : retrieveService.Errors.ToList();//cached;
                    mined.LastRetrieve = lastRetrieve;
                    mined.IsNeedToSave = true;
                    IsRetrieving = false;
                }
            }
        }
    }
}
