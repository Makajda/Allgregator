using Allgregator.Aux.Repositories;
using Allgregator.Rss.Models;

namespace Allgregator.Rss.Repositories {
    public class MinedRepository : ZipRepositoryBase<Mined> {
        protected override string ModuleName => Module.Name;
    }
}
