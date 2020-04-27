using Allgregator.Rss.Models;
using Prism.Events;

namespace Allgregator.Rss.Common {
    internal class LinkMovedEvent : PubSubEvent<(int Id, Link Link)> { }
    internal class ChapterDeletedEvent : PubSubEvent<int> { }
    internal class ChapterAddedEvent : PubSubEvent<Chapter[]> { }

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
