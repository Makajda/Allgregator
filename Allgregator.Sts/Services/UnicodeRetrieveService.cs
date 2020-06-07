using Allgregator.Aux.Common;
using Allgregator.Aux.Services;
using Allgregator.Sts.Model;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Allgregator.Sts.Services {
    internal class UnicodeRetrieveService : SiteRetrieveServiceBase<UnicodeArea> {
        public UnicodeRetrieveService(WebService webService) : base(webService) { }

        protected override void Process(string html) {
            var sgs = Regex.Split(html, "<p class=\"sg\">", RegexOptions.Singleline);

            foreach (var sg in sgs) {
                var title = Regex.Match(sg,
                    "(?: title=\")([A-F0-9]*?)-([A-F0-9]*?)(?:\">)",
                    RegexOptions.Singleline);

                var ranges = new List<Pair<int, int>>();

                while (title.Success) {
                    var begin = title.Groups[1].Value;
                    var end = title.Groups[2].Value;

                    var valueBegin = Convert.ToInt32(begin, 16);
                    var valueEnd = Convert.ToInt32(end, 16);

                    ranges.Add(new Pair<int, int>() { First = valueBegin, Second = valueEnd });

                    title = title.NextMatch();
                }

                if (ranges.Count > 0) {
                    var removes = new List<Pair<int, int>>();
                    foreach (var range1 in ranges) {
                        foreach (var range2 in ranges) {
                            if (range1 != range2) {
                                if (range2.First >= range1.First && range2.Second <= range1.Second) {
                                    removes.Add(range2);
                                }
                            }
                        }
                    }

                    foreach (var r in removes) {
                        ranges.Remove(r);
                    }

                    var area = new UnicodeArea() {
                        Ranges = ranges,
                        Name = sg.Substring(0, sg.IndexOf("</p>"))
                            .Replace("amp;", null)
                            .Replace("<a>", null)
                            .Replace("</a>", null)
                            .Trim()
                    };

                    Items.Add(area);
                }
            }
        }
    }
}
