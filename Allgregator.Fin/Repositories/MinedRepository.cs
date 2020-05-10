using Allgregator.Aux.Repositories;
using Allgregator.Fin.Models;

namespace Allgregator.Fin.Repositories {
    public class MinedRepository : ZipRepositoryBase<Mined> {
        protected override string ModuleName => Module.Name;
    }
}
