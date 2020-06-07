using Allgregator.Aux.Repositories;
using Allgregator.Sts.Model;

namespace Allgregator.Sts.Repositories {
    public class MinedUniRepository : ZipRepositoryBase<MinedUni> {
        protected override string ModuleName => Module.NameUni;
    }
}
