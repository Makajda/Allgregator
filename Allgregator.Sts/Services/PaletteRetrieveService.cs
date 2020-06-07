using Allgregator.Aux.Services;
using Allgregator.Sts.Model;

namespace Allgregator.Sts.Services {
    internal class PaletteRetrieveService : SiteRetrieveServiceBase<PaletteColor> {
        public PaletteRetrieveService(WebService webService) : base(webService) { }

        protected override void Process(string html) {
        }
    }
}
