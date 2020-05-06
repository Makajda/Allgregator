using Prism.Events;

namespace Allgregator.Aux.Common {
    public static class Given {
        public const string PathData = "Data";
        public const string ButtonShapeTemplateKey = "ButtonShapeTemplate";

        public const string MenuRegion = "MenuRegion";
        public const string MainRegion = "MainRegion";

        public const int TryChapter = 10010;
        public const int FinChapter = 10020;
        public const int StsChapter = 10030;
    }

    public class CurrentChapterChangedEvent : PubSubEvent<int> { }
}
