using Allgregator.Aux.Services;
using Prism.Mvvm;
using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
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

        internal async Task Retrieve() {
            IsRetrieving = true;


            ProgressMaximum = 3;//todo посчитать количество недостающих дней
            ProgressValue = 1;
            var lastRetrieve = DateTimeOffset.Now;//todo

            using (cancellationTokenSource = new CancellationTokenSource()) {
                IsRetrieving = true;
                var cancellationToken = cancellationTokenSource.Token;

                try {
                    await Task.WhenAll(
                        Task.Factory.StartNew(async () => await RetrieveCbr(), cancellationToken, TaskCreationOptions.LongRunning, TaskScheduler.Default)
                        );
                }
                catch (OperationCanceledException) {
                }
            }

            if (IsRetrieving) {
                IsRetrieving = false;
            }
        }

        private async Task RetrieveCbr() {
            var name = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "cbr.txt");
            //try {
            //    const string address = "https://www.cbr.ru/currency_base/daily/?UniDbQuery.Posted=True&UniDbQuery.To=27.04.2020";
            //    var html = await webService.GetHtml(address);
            //    File.WriteAllText(name, html);
            //}
            //catch (Exception e) {
            //    //todo return e
            //}

            var html = File.ReadAllText(name);
            var regexTr = new Regex("(?<=<tr>).*?(?=</tr>)", RegexOptions.Singleline);
            var matchesTr = regexTr.Matches(html);
            var usd = matchesTr.FirstOrDefault(n => n.Value.Contains("USD"));
            var regexTd = new Regex("(?<=<td>).*?(?=</td>)", RegexOptions.Singleline);
            var matchesTd = regexTd.Matches(usd.Value);
            var s = matchesTd.Last();
            var b = decimal.TryParse(s.Value, out decimal res);
            progressIndicator.Report(1);
        }
    }
}