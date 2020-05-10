using Allgregator.Aux.Repositories;
using Allgregator.Sts.Model;

namespace Allgregator.Sts.Repositories {
    public class MinedRepository : ZipRepositoryBase<Mined> {
        protected override string ModuleName => Module.Name;
    }
}
