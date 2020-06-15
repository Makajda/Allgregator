using Allgregator.Rss.Models;
using Prism.Events;

namespace Allgregator.Rss.Common {
    public static class Givenloc {
        public const string SubmainRegion = "SubmainRegion";

        internal const string ChapterIdParameter = "ChapterId";
        internal const string ChapterNameParameter = "ChapterName";

        internal static int TryNewsId = 10010;
        internal static int TrySportId = 10011;
        internal static int TryGameId = 10012;
        internal static int TryProgId = 10013;
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
