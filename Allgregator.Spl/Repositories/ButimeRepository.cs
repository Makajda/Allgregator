using Allgregator.Aux.Common;
using Allgregator.Aux.Repositories;
using Allgregator.Spl.Models;

namespace Allgregator.Spl.Repositories {
    public class ButimeRepository : ZipRepositoryBase<Mined> {
        protected override Mined CreateDefault(int id = 0) {
            var result = new Mined {
                Butasks = new Obsefy<Butask> {
                    new Butask { Name = "One" },
                    new Butask { Name = "Two" },
                    new Butask { Name = "Three" },
                }
            };
            return result;
        }
    }
}
