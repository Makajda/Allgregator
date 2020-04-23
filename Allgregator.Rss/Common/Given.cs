using Allgregator.Rss.Models;
using Prism.Events;

namespace Allgregator.Rss.Common {
    public class CurrentChapterChangedEvent : PubSubEvent<Chapter> { }
    public class LinkMovedEvent : PubSubEvent<(int Id, Link Link)> { }
    public class ChapterDeletedEvent : PubSubEvent<int> { }
    public class ChapterAddedEvent : PubSubEvent<Chapter[]> { }

    public enum ChapterViews {
        NewsView,
        OldsView,
        LinksView
    }

    public enum LinksStates {
        Normal,
        Detection,
        Selection,
        Chapter
    }
}
