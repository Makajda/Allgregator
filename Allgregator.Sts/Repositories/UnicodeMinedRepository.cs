using Allgregator.Aux.Models;
using Allgregator.Aux.Repositories;
using Allgregator.Sts.Model;

namespace Allgregator.Sts.Repositories {
    public class UnicodeMinedRepository : ZipRepositoryBase<MinedBase<UnicodeArea>> {
        public UnicodeMinedRepository() : base(Module.Name, "MinedUnicode") { }
    }
}
