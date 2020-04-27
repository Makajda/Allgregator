using Allgregator.Aux.Services;
using Allgregator.Fin.Models;
using Prism.Mvvm;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Allgregator.Fin.Services {
    internal class OreService : BindableBase {
        private readonly WebService webService;
        private CancellationTokenSource cancellationTokenSource;
        private readonly IProgress<int> progressIndicator;
        private int progressValue;
        private int progressMaximum;

        public OreService(
            WebService webService
            ) {
            this.webService = webService;
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

        internal void CancelRetrieve() {
            try {
                if (cancellationTokenSource != null && cancellationTokenSource.Token.CanBeCanceled) {
                    IsRetrieving = false;
                    cancellationTokenSource.Cancel();
                }
            }
            catch (ObjectDisposedException) { }
        }

        internal async Task Retrieve(Mined mined) {
            //todo строить недостающие даты на основании mined.Currensies
            var startDate = new DateTimeOffset(2020, 4, 10, 0, 0, 0, TimeSpan.Zero);
            var endDate = new DateTimeOffset(2020, 4, 27, 0, 0, 0, TimeSpan.Zero);

            var date = startDate.Date;
            var toDate = endDate.Date;
            var dates = new List<DateTimeOffset>();

            while (date <= toDate) {
                dates.Add(new DateTimeOffset(date));//todo filter for there are
                date = date.AddDays(1);
            }

            IsRetrieving = true;

            ProgressMaximum = dates.Count;
            ProgressValue = 1;
            var lastRetrieve = DateTimeOffset.Now;//todo

            using var retrieveService = new RetrieveService();
            using (cancellationTokenSource = new CancellationTokenSource()) {
                IsRetrieving = true;
                var cancellationToken = cancellationTokenSource.Token;

                try {
                    await Task.WhenAll(
                        Partitioner.Create(dates).GetPartitions(9).Select(partition =>
                            Task.Factory.StartNew(async () => {
                                using (partition) {
                                    while (partition.MoveNext()) {
                                        await retrieveService.Production(partition.Current, webService);
                                        progressIndicator.Report(1);
                                    }
                                };
                            }, cancellationToken, TaskCreationOptions.LongRunning, TaskScheduler.Default)
                        ));
                }
                catch (OperationCanceledException) {
                }
            }

            if (IsRetrieving) {
                //var name = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "cbr.txt");
                //File.WriteAllText(name, html);
                //var html = File.ReadAllText(name);
                mined.Currencies = retrieveService.Items.OrderByDescending(n => n.Date);
                mined.Errors = retrieveService.Errors.Count == 0 ? null : retrieveService.Errors.ToList();//cached;
                mined.IsNeedToSave = true;
                IsRetrieving = false;
            }
        }
    }
}