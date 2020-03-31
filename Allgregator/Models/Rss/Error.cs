using System;

namespace Allgregator.Models.Rss {
    public class Error {
        public readonly Link Link;
        public readonly Exception Exception;

        public Error(Link link, Exception exception) {
            Link = link;
            Exception = exception;
        }
    }
}
