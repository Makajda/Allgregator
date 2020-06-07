using Allgregator.Aux.Models;
using Allgregator.Aux.Repositories;
using Allgregator.Sts.Model;

namespace Allgregator.Sts.Repositories {
    public class PaletteMinedRepository : ZipRepositoryBase<MinedBase<PaletteColor>> {
        public PaletteMinedRepository() : base(Module.Name, "MinedPalette") { }
    }
}
