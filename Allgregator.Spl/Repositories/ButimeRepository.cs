using Allgregator.Aux.Common;
using Allgregator.Aux.Repositories;
using Allgregator.Spl.Models;

namespace Allgregator.Spl.Repositories {
    public class ButimeRepository : ZipRepositoryBase<Bumined> {
        protected override Bumined CreateDefault(int id = 0) {
            var result = new Bumined {
                Butasks = new Obsefy<Butask> {
                    new Butask { Name = "Task1" },
                    new Butask { Name = "Task2" },
                    new Butask { Name = "Task3" },
                }
            };
            return result;
        }
    }
}
