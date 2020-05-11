using Allgregator.Rss.Common;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Allgregator.Rss.Models {
    public class Reco : IEquatable<Reco> {
        private string summaryText;

        public string Uri { get; set; }
        public Uri ImageUri { get; set; }
        public string FeedTitle { get; set; }
        public string ItemTitle { get; set; }
        public DateTimeOffset PublishDate { get; set; }

        [JsonIgnore]
        public string SummaryText {
            get {
                if (summaryText == null) {
                    summaryText = RegexUtilities.GetText(SummaryHtml);
                }

                return summaryText;
            }
        }

        public string SummaryHtml { get; set; }

        public bool Equals([AllowNull] Reco other) =>
            Uri == other.Uri &&
            ImageUri == other.ImageUri &&
            FeedTitle == other.FeedTitle &&
            ItemTitle == other.ItemTitle &&
            PublishDate == other.PublishDate &&
            SummaryText == other.SummaryText &&
            SummaryHtml == other.SummaryHtml;

        public override bool Equals(object obj) =>
            Equals((Reco)obj);

        public override int GetHashCode() =>
            $"{Uri}{ImageUri}{FeedTitle}{ItemTitle}{PublishDate}{SummaryText}{SummaryHtml}".GetHashCode();

        public static bool operator ==(Reco reco1, Reco reco2) {
            if (((object)reco1) == null || ((object)reco2) == null)
                return Object.Equals(reco1, reco2);

            return reco1.Equals(reco2);
        }

        public static bool operator !=(Reco reco1, Reco reco2) {
            if (((object)reco1) == null || ((object)reco2) == null)
                return !Object.Equals(reco1, reco2);

            return !(reco1.Equals(reco2));
        }
    }
}
