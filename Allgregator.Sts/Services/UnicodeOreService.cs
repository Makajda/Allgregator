using Allgregator.Aux.Services;
using Allgregator.Sts.Model;

namespace Allgregator.Sts.Services {
    internal class UnicodeOreService : SiteOreServiceBase<UnicodeArea> {
        public UnicodeOreService(UnicodeRetrieveService retrieveService) : base(retrieveService) { }
        //public UnicodeOreService(UnicodeRetrieveService retrieveService) : base("https://unicode.org/charts/", retrieveService) { }
    }
}
