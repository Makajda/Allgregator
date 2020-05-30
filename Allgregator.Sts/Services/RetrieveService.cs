using Allgregator.Aux.Models;
using Allgregator.Aux.Services;
using Allgregator.Sts.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Allgregator.Sts.Services {
    internal class RetrieveService : RetrieveServiceBase<string, Area> {
        private readonly WebService webService;

        public RetrieveService(
            WebService webService
            ) {
            this.webService = webService;
        }

        public override async Task ProductionAsync(string address) {
            try {
                //var html = await webService.GetHtml(address);
                var html = await File.ReadAllTextAsync(@"c:\files\unicode charts.txt");

                var sgs = Regex.Split(html, "<p class=\"sg\">", RegexOptions.Singleline);

                foreach (var sg in sgs) {
                    var title = Regex.Match(sg,
                        "(?: title=\")([A-F0-9]*?)-([A-F0-9]*?)(?:\">)",
                        RegexOptions.Singleline);

                    var area = new Area();

                    while (title.Success) {
                        var begin = title.Groups[1].Value;
                        var end = title.Groups[2].Value;

                        var valueBegin = Convert.ToInt32(begin, 16);
                        var valueEnd = Convert.ToInt32(end, 16);

                        area.Ranges.Add((valueBegin, valueEnd));

                        title = title.NextMatch();
                    }

                    if (area.Ranges.Count > 0) {
                        area.Name = sg.Substring(0, sg.IndexOf("</p>"))
                            .Replace("amp;", null)
                            .Replace("<a>", null)
                            .Replace("</a>", null)
                            .Trim();

                        var removes = new List<(int, int)>();
                        foreach (var range1 in area.Ranges) {
                            foreach (var range2 in area.Ranges) {
                                if (range1 != range2) {
                                    if (range2.Begin >= range1.Begin && range2.End <= range1.End) {
                                        removes.Add(range2);
                                    }
                                }
                            }
                        }

                        foreach (var r in removes) {
                            area.Ranges.Remove(r);
                        }

                        Items.Add(area);
                    }
                }
            }
            catch (OperationCanceledException) { }
            catch (Exception exception) {
                Errors.Add(new Error() { Source = address, Message = exception.Message });
            }
        }
    }
}


//var addresses = new[] { "https://unicode.org/charts/charindex.html" };
//public override async Task ProductionAsync(string address) {
//    try {
//        var html = await webService.GetHtml(address);

//        var regex = new Regex("(?<=<tr><td>).*?(?=</a></td></tr>)", RegexOptions.Singleline);
//        var matches = regex.Matches(html);
//        foreach (Match match in matches) {
//            var value = match.Value;
//            try {
//                TakeSymbol(value);
//            }
//            catch (Exception exception) {
//                Errors.Add(new Error() { Source = value, Message = exception.Message });
//            }
//        }
//    }
//    catch (OperationCanceledException) { }
//    catch (Exception exception) {
//        Errors.Add(new Error() { Source = address, Message = exception.Message });
//    }
//}

//private void TakeSymbol(string str) {
//    if (!str.Contains("BOOP") && !str.Contains("DOOD")) {
//        var indexCode = str.LastIndexOf(">") + 1;
//        var codeChar = str.Substring(indexCode);

//        var valueChar = (char)Convert.ToInt32(codeChar, 16);
//        if (Items.All(n => n.Char != valueChar)) {
//            var indexName = str.IndexOf("</td>");
//            var name = str.Substring(0, indexName).ToLower();

//            var symbol = new Symbol() { Char = valueChar, Name = name };
//            Items.Add(symbol);
//        }
//    }
//}
