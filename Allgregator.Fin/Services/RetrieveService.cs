using Allgregator.Aux.Models;
using Allgregator.Aux.Services;
using Allgregator.Fin.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Allgregator.Fin.Services {
    internal class RetrieveService : IDisposable {
        private readonly object syncItems = new object();
        private readonly object syncErrors = new object();

        internal List<Currency> Items { get; } = new List<Currency>();
        internal List<Error> Errors { get; } = new List<Error>();

        public void Dispose() {
            Items.Clear();
            Errors.Clear();
        }

        internal async Task Production(DateTimeOffset date, WebService webService) {
            const string country = "USD";
            var stringDate = $"{date.Day:D2}.{date.Month:D2}.{date.Year:D4}";
            var address = $"https://www.cbr.ru/currency_base/daily/?UniDbQuery.Posted=True&UniDbQuery.To={stringDate}";

            try {
                var html = await webService.GetHtml(address);

                var regexTr = new Regex("(?<=<tr>).*?(?=</tr>)", RegexOptions.Singleline);
                var matchesTr = regexTr.Matches(html);
                var usd = matchesTr.FirstOrDefault(n => n.Value.Contains(country));
                var regexTd = new Regex("(?<=<td>).*?(?=</td>)", RegexOptions.Singleline);
                var matchesTd = regexTd.Matches(usd.Value);
                var s = matchesTd.Last();
                var res = decimal.Parse(s.Value);
                var currency = new Currency() {
                    Date = date,
                    Country = country,
                    Val = res
                };

                lock (syncItems) {
                    Items.Add(currency);
                }
            }
            catch (OperationCanceledException) { }
            catch (Exception exception) {
                lock (syncErrors) {
                    Errors.Add(new Error() { Source = stringDate, Message = exception.Message });
                }
            }
        }
    }
}
