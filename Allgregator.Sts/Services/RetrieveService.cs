using Allgregator.Aux.Models;
using Allgregator.Aux.Services;
using Allgregator.Sts.Model;
using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Allgregator.Sts.Services {
    internal class RetrieveService : RetrieveServiceBase<string, Symbol> {
        private readonly WebService webService;

        public RetrieveService(
            WebService webService
            ) {
            this.webService = webService;
        }

        public override async Task ProductionAsync(string address) {
            try {
                var html = await webService.GetHtml(address);

                var regex = new Regex("(?<=<tr><td>).*?(?=</a></td></tr>)", RegexOptions.Singleline);
                var matches = regex.Matches(html);
                foreach (Match match in matches) {
                    var value = match.Value;
                    try {
                        var symbol = GetSymbol(value);
                        Items.Add(symbol);
                    }
                    catch (Exception exception) {
                        if (!value.Contains("BOOP") && !value.Contains("DOOD")) {
                            Errors.Add(new Error() { Source = value, Message = exception.Message });
                        }
                    }
                }
            }
            catch (OperationCanceledException) { }
            catch (Exception exception) {
                Errors.Add(new Error() { Source = address, Message = exception.Message });
            }
        }

        private Symbol GetSymbol(string str) {
            var indexName = str.IndexOf("</td>");
            var name = str.Substring(0, indexName);

            var indexCode = str.LastIndexOf(">") + 1;
            var codeChar = str.Substring(indexCode);

            var valueChar = (char)Convert.ToInt32(codeChar, 16);
            var symbol = new Symbol() { Char = valueChar, Name = name };
            return symbol;
        }
    }
}
