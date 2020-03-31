﻿using Allgregator.Common;
using Newtonsoft.Json;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Allgregator.Models.Rss {
    public class Reco : IEquatable<Reco> {
        private string summaryText;

        public Uri Uri { get; set; }
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
    }
}
