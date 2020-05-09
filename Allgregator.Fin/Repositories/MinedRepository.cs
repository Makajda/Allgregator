using Allgregator.Aux.Repositories;
using Allgregator.Fin.Models;

namespace Allgregator.Fin.Repositories {
    public class MinedRepository : MinedRepositoryBase<Mined> {
        public MinedRepository() {
            name = "fin";
        }
    }
}
