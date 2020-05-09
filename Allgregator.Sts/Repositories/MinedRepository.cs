using Allgregator.Aux.Repositories;
using Allgregator.Sts.Model;

namespace Allgregator.Sts.Repositories {
    public class MinedRepository : MinedRepositoryBase<Mined> {
        public MinedRepository() {
            name = "sts";
        }
    }
}
