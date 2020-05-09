using Allgregator.Rss.Models;
using Prism.Events;

namespace Allgregator.Rss.Common {
    public static class Givenloc {
        internal const string Module = "Rss";
        public const string SubmainRegion = "SubmainRegion";
        internal const string ChapterIdParameter = "ChapterId";
        internal const string ChapterNameParameter = "ChapterName";

        internal static int TryDataId = 10010;
        internal static string GetChapterId(int id) => $"{Givenloc.Module}{id}";
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
