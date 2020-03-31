using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Allgregator.Common {
    public class RegexUtilities {
        private const int SummaryMax = 399;

        public static Uri GetImage(string text) {
            if (!string.IsNullOrWhiteSpace(text)) {
                var regex = new Regex("(http)?s?:?(\\/\\/[^\"']*\\.(?:png|jpg|jpeg|gif|png|svg))", RegexOptions.IgnoreCase);
                var m = regex.Match(text);
                if (m.Success) {
                    try {
                        var uAddress = new UriBuilder(m.Value);
                        return uAddress.Uri;
                    }
                    catch (Exception) { }
                }
            }

            return null;
        }

        public static string GetText(string text) {
            if (string.IsNullOrWhiteSpace(text)) {
                return null;
            }

            var regex = new Regex("\\<[^\\>]*\\>");
            var sResult = regex.Replace(text, " ");
            sResult = sResult.Trim();
            sResult = System.Net.WebUtility.HtmlDecode(sResult);
            sResult = sResult.Substring(0, Math.Min(SummaryMax, sResult.Length));
            sResult = sResult.Replace('\n', ' ');
            sResult = sResult.Replace('\r', ' ');
            return sResult;
        }

        public static IEnumerable<string> GetHrefs(string text) {
            const int skipA = 9;

            if (string.IsNullOrWhiteSpace(text)) {
                yield break;
            }

            var regex = new Regex("<a\\s+(?:[^>]*?\\s+)?href=([\"'])(.*?)\\1", RegexOptions.IgnoreCase);
            var matches = regex.Matches(text);
            foreach (Match m in matches) {
                var s = m.Value;
                var l = s.Length;
                if (l > skipA) {
                    var r = s.Substring(skipA).TrimEnd('\"');
                    yield return r.ToLower();
                }
            }
        }
    }
}
