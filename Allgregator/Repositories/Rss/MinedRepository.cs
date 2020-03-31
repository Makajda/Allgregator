using Allgregator.Models.Rss;
using System;
using System.Collections.Generic;
using System.Text;

namespace Allgregator.Repositories.Rss {
    public class MinedRepository {
        public Mined GetMined(int collectionId) {
            var mined = new Mined();
            mined.AcceptTime = DateTimeOffset.Now.AddDays(-3);
            mined.LastRetrieve = DateTimeOffset.Now.AddDays(-1);
            return mined;
        }
    }
}
