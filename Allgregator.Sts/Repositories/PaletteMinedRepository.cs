using Allgregator.Aux.Repositories;
using Allgregator.Sts.Model;

namespace Allgregator.Sts.Repositories {
    public class PaletteMinedRepository : ZipRepositoryBase<MinedPalette> {
        protected override string ModuleName => Module.Name;
    }
}
