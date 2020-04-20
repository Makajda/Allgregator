using Allgregator.Models.Rss;
using Prism.Events;

namespace Allgregator.Common {
    public static class Given {
        public const string PathData = "Data";
        public const string ButtonShapeTemplateKey = "ButtonShapeTemplate";

        public const string MenuRegion = "MenuRegion";
        public const string MainRegion = "MainRegion";

        public const string SettingsView = "SettingsView";
    }

    public class CurrentChapterChangedEvent : PubSubEvent<Chapter> { }
    public class LinkMovedEvent : PubSubEvent<(int Id, Link Link)> { }
    public class ChapterDeletedEvent : PubSubEvent<int> { }
    public class ChapterAddedEvent : PubSubEvent<Chapter[]> { }

    public enum RssChapterViews {
        NewsView,
        OldsView,
        LinksView
    }

    public enum RssLinksStates {
        Normal,
        Detection,
        Selection,
        Chapter
    }
}
