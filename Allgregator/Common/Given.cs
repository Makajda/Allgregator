using Allgregator.Models.Rss;
using Prism.Events;

namespace Allgregator.Common {
    public static class Given {
        public const string PathData = "Data";

        public const string MenuRegion = "MenuRegion";
        public const string MainRegion = "MainRegion";
    }

    public class CurrentChapterChangedEvent : PubSubEvent<Chapter> { }
    public class LinkMovedEvent : PubSubEvent<(int Id, Link Link)> { }

    public enum RssChapterViews {
        NewsView,
        OldsView,
        LinksView
    }

    public enum RssLinksState {
        Normal,
        Detection,
        Selection
    }
}
