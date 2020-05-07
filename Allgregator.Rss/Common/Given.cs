using Allgregator.Rss.Models;
using Prism.Events;

namespace Allgregator.Rss.Common {
    public static class Given {
        public const string RegionSubmain = "SubmainRegion";
    }

    internal class LinkMovedEvent : PubSubEvent<(int Id, Link Link)> { }

    public enum ChapterViews {
        NewsView,
        OldsView,
        LinksView,
        SettingsView
    }

    public enum LinksStates {
        Normal,
        Detection,
        Selection
    }
}
