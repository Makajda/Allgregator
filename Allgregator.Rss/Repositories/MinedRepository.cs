﻿using Allgregator.Aux.Repositories;
using Allgregator.Rss.Models;

namespace Allgregator.Rss.Repositories {
    public class MinedRepository : MinedRepositoryBase<Mined> {
        public MinedRepository() {
            name = "rss";
        }
    }
}
