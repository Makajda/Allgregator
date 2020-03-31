using Prism.Mvvm;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Allgregator.Models.Rss {
    public class Link : BindableBase, IEquatable<Link> {
        private string xmlUrl;
        private string name;
        private string htmlUrl;

        public string XmlUrl {
            get => xmlUrl;
            set => SetProperty(ref xmlUrl, value);
        }

        public string Name {
            get => name;
            set => SetProperty(ref name, value);
        }

        public string HtmlUrl {
            get => htmlUrl;
            set => SetProperty(ref htmlUrl, value);
        }

        public Link Clone() {
            var link = new Link() {
                Name = this.Name,
                HtmlUrl = this.HtmlUrl,
                XmlUrl = this.XmlUrl
            };

            return link;
        }

        public bool Equals([AllowNull] Link other) =>
            Name == other.Name &&
            HtmlUrl == other.HtmlUrl &&
            XmlUrl == other.XmlUrl;
    }
}
