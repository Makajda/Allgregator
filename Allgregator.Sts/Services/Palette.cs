using Allgregator.Aux.Services;
using Allgregator.Sts.Model;

namespace Allgregator.Sts.Services {
    internal class PaletteOreService : SiteOreServiceBase<ColorPalette> {
        public PaletteOreService(PaletteRetrieveService retrieveService) : base(retrieveService) { }
    }

    internal class PaletteRetrieveService : SiteRetrieveServiceBase<ColorPalette> {
        public PaletteRetrieveService(
            WebService webService
            ) : base(webService) { }

        protected override void Process(string html) {
        }
    }
}
