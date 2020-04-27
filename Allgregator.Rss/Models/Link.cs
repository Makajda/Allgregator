using Prism.Mvvm;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Allgregator.Rss.Models {
    public class Link : BindableBase, IEquatable<Link> {
        private string name;
        public string Name {
            get => name;
            set => SetProperty(ref name, value);
        }

        private string htmlUrl;
        public string HtmlUrl {
            get => htmlUrl;
            set => SetProperty(ref htmlUrl, value);
        }

        private string xmlUrl;
        public string XmlUrl {
            get => xmlUrl;
            set => SetProperty(ref xmlUrl, value);
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
