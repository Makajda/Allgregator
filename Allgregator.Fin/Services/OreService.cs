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

        internal async Task Retrieve(Mined mined, DateTimeOffset startDate) {
            //недостающие даты на основании имеющихся mined.Currensies
            var date = startDate.Date;
            var toDate = DateTimeOffset.Now.Date;
            var dates = new List<DateTimeOffset>();

            if (mined.Currencies != null) {
                foreach (var currency in mined.Currencies) {
                    while (date <= toDate && date < currency.Date) {
                        dates.Add(date);
                        date = date.AddDays(1);
                    }

                    date = date.AddDays(1);
                }
            }

            //todo поменять на просто дату
            while (date <= toDate) {
                dates.Add(date);
                date = date.AddDays(1);
            }

            if (dates.Count == 0) {
                dates.Add(toDate);
            }

            IsRetrieving = true;

            ProgressMaximum = dates.Count;
            ProgressValue = 1;
            var lastRetrieve = DateTimeOffset.Now;

            using var retrieveService = new RetrieveService();
            using (cancellationTokenSource = new CancellationTokenSource()) {
                IsRetrieving = true;
                var cancellationToken = cancellationTokenSource.Token;

                try {
                    await Task.WhenAll(
                        Partitioner.Create(dates).GetPartitions(9).Select(partition =>
                            Task.Run(async () => {
                                using (partition) {
                                    while (partition.MoveNext()) {
                                        await retrieveService.Production(partition.Current, webService);
                                        progressIndicator.Report(1);
                                    }
                                };
                            }, cancellationToken)
                        ));
                }
                catch (OperationCanceledException) {
                }
            }

            if (IsRetrieving) {
                if(mined.Currencies == null) {
                    mined.Currencies = retrieveService.Items.OrderBy(n => n.Date).ToList();
                }
                else {
                    mined.Currencies = mined.Currencies.Union(retrieveService.Items).OrderBy(n => n.Date).ToList();
                }
                mined.Errors = retrieveService.Errors.Count == 0 ? null : retrieveService.Errors.ToList();//cached;
                mined.LastRetrieve = lastRetrieve;
                mined.IsNeedToSave = true;
                IsRetrieving = false;
            }
        }
    }
}