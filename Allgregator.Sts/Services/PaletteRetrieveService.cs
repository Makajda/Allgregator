using Allgregator.Aux.Services;
using Allgregator.Sts.Models;
using System;
using System.Text.RegularExpressions;

namespace Allgregator.Sts.Services {
    internal class PaletteRetrieveService : SiteRetrieveServiceBase<PaletteColor> {
        public PaletteRetrieveService(WebService webService) : base(webService) { }

        protected override void Process(string html) {
            var trs = Regex.Matches(html, "<tr>.*?</tr>", RegexOptions.Singleline);

            foreach (Match tr in trs) {
                var nameMatch = Regex.Match(tr.Value,
                    "(?:<a.*?>)(.*?)(?:</a>)",
                    RegexOptions.Singleline);

                if (nameMatch.Success && nameMatch.Groups.Count > 0) {
                    var valueMatch = Regex.Match(tr.Value,
                        "(?:<code.*?>)(.*?)(?:</code>)",
                        RegexOptions.Singleline);

                    if (valueMatch.Success && valueMatch.Groups.Count > 0) {
                        var name = nameMatch.Groups[1].Value;
                        var value = valueMatch.Groups[1].Value;

                        if (value.Length == 9) {
                            var r = Convert.ToByte(value.Substring(3, 2), 16);
                            var g = Convert.ToByte(value.Substring(5, 2), 16);
                            var b = Convert.ToByte(value.Substring(7, 2), 16);

                            var color = new PaletteColor() { Name = name, R = r, G = g, B = b };

                            Items.Add(color);
                        }
                    }
                }
            }
        }
    }
}
