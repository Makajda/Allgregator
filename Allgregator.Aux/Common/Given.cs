using Prism.Events;

namespace Allgregator.Aux.Common {
    public static class Given {
        public const string PathData = "Data";

        public const string ButtonShapeTemplateKey = "ButtonShapeTemplate";

        public const string DataParameter = "Data";

        public const string MenuRegion = "MenuRegion";
        public const string MainRegion = "MainRegion";
    }

    public class CurrentChapterChangedEvent : PubSubEvent<string> { }

    public class Pair<T, U> {
        public T First { get; set; }
        public U Second { get; set; }
    };
}
