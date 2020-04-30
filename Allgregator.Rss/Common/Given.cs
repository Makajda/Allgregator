using Allgregator.Rss.Models;
using Prism.Events;

namespace Allgregator.Rss.Common {
    internal class LinkMovedEvent : PubSubEvent<(int Id, Link Link)> { }

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
