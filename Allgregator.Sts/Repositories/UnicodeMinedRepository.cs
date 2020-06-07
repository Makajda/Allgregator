using Allgregator.Aux.Repositories;
using Allgregator.Sts.Model;

namespace Allgregator.Sts.Repositories {
    public class UnicodeMinedRepository : ZipRepositoryBase<MinedUnicode> {
        protected override string ModuleName => Module.Name;
    }
}
