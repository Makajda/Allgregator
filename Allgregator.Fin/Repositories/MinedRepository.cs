using Allgregator.Aux.Repositories;
using Allgregator.Fin.Models;

namespace Allgregator.Fin.Repositories {
    public class MinedRepository : ZipRepositoryBase<Mined> {
        public MinedRepository() : base(Module.Name) { }
    }
}
