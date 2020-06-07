using Allgregator.Aux.Services;
using Allgregator.Sts.Model;

namespace Allgregator.Sts.Services {
    internal class PaletteOreService : SiteOreServiceBase<PaletteColor> {
        public PaletteOreService(PaletteRetrieveService retrieveService) : base(retrieveService) { }
        //public PaletteOreService(PaletteRetrieveService retrieveService) : base("//todo", retrieveService) { }
    }
}
