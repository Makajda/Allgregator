using System;

namespace Allgregator.Models.Rss {
    public class Error {
        public Error(Link link, Exception exception) {
            Link = link;
            Exception = exception;
        }

        public Link Link { get; private set; }
        public Exception Exception { get; private set; }
    }
}
