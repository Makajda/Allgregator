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
            other != null &&
            Name == other.Name &&
            HtmlUrl == other.HtmlUrl &&
            XmlUrl == other.XmlUrl;

        public override bool Equals(object obj) =>
            Equals(obj as Link);

        public override int GetHashCode() =>
            $"{Name}{HtmlUrl}{XmlUrl}".GetHashCode();

        public static bool operator ==(Link link1, Link link2) {
            if (((object)link1) == null || ((object)link2) == null)
                return Object.Equals(link1, link2);

            return link1.Equals(link2);
        }

        public static bool operator !=(Link link1, Link link2) {
            if (((object)link1) == null || ((object)link2) == null)
                return !Object.Equals(link1, link2);

            return !(link1.Equals(link2));
        }
    }
}
