using Allgregator.Aux.Repositories;
using Allgregator.Sts.Model;

namespace Allgregator.Sts.Repositories {
    public class AreasRepository : RepositoryBase<Areas> {
        protected override string ModuleName => Module.Name;
        protected override Areas CreateDefault(int id = 0) {
            return new Areas() {
                Items = new[] {
                    new Area() { From="", To="", Name="" }
                }
            };
        }
    }
}
