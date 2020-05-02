using Prism.Mvvm;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Allgregator.Aux.Services {
    public class OreServiceBase : BindableBase {
        private CancellationTokenSource cancellationTokenSource;
        private readonly IProgress<int> progressIndicator;
        private int progressValue;
        private int progressMaximum;

        public OreServiceBase() {
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

        protected async Task<DateTimeOffset> Retrieve<Tin>(IList<Tin> args, Func<Tin, Task> production) =>
            await RetrieveReal(args, production);

        protected async Task<DateTimeOffset> Retrieve<Tin>(IList<Tin> args, Action<Tin> production) =>
            await RetrieveReal(args, null, production);

        private async Task<DateTimeOffset> RetrieveReal<Tin>(IList<Tin> args, Func<Tin, Task> productionAsync, Action<Tin> production = null) {
            IsRetrieving = true;

            ProgressMaximum = args.Count;
            ProgressValue = 1;
            var lastRetrieve = DateTimeOffset.Now;

            using (cancellationTokenSource = new CancellationTokenSource()) {
                IsRetrieving = true;
                var cancellationToken = cancellationTokenSource.Token;

                try {
                    await Task.WhenAll(
                        production == null ?

                        Partitioner.Create(args).GetPartitions(9).Select(partition =>
                            Task.Run(async () => {
                                using (partition) {
                                    while (partition.MoveNext()) {
                                        await productionAsync(partition.Current);
                                        progressIndicator.Report(1);
                                    }
                                };
                            }, cancellationToken)
                        ) :

                        Partitioner.Create(args).GetPartitions(9).Select(partition =>
                            Task.Factory.StartNew(() => {
                                using (partition) {
                                    while (partition.MoveNext()) {
                                        production(partition.Current);
                                        progressIndicator.Report(1);
                                    }
                                };
                            }, cancellationToken, TaskCreationOptions.LongRunning, TaskScheduler.Default)
                        ));
                }
                catch (OperationCanceledException) {
                }
            }

            return lastRetrieve;
        }
    }
}