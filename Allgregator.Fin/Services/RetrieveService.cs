using Allgregator.Aux.Models;
using Allgregator.Aux.Services;
using Allgregator.Fin.Common;
using Allgregator.Fin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Allgregator.Fin.Services {
    internal class RetrieveService : RetrieveServiceBase<DateTimeOffset, Term> {
        private readonly WebService webService;

        public RetrieveService(
            WebService webService
            ) {
            this.webService = webService;
        }

        public override async Task ProductionAsync(DateTimeOffset date) {
            var stringDate = $"{date.Day:D2}.{date.Month:D2}.{date.Year:D4}";
            var address = $"https://www.cbr.ru/currency_base/daily/?UniDbQuery.Posted=True&UniDbQuery.To={stringDate}";

            try {
                var html = await webService.GetHtml(address);

                var regex = new Regex("(?<=<tr>).*?(?=</tr>)", RegexOptions.Singleline);
                var matches = regex.Matches(html);
                var term = new Term() { Date = date };
                term.Values = new Dictionary<string, decimal>();
                foreach (var key in Given.CurrencyNames) {
                    var value = GetValue(matches, key);
                    term.Values.Add(key, value);
                }

                lock (syncItems) {
                    Items.Add(term);
                }
            }
            catch (OperationCanceledException) { }
            catch (Exception exception) {
                lock (syncErrors) {
                    Errors.Add(new Error() { Source = stringDate, Message = exception.Message });
                }
            }
        }

        private decimal GetValue(MatchCollection matches, string country) {
            var tr = matches.FirstOrDefault(n => n.Value.Contains(country));
            var regexTd = new Regex("(?<=<td>).*?(?=</td>)", RegexOptions.Singleline);
            var matchesTd = regexTd.Matches(tr.Value);
            var s = matchesTd.Last();
            var res = decimal.Parse(s.Value);
            return res;//todo
        }
    }
}
