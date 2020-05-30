using Allgregator.Aux.Services;
using Allgregator.Sts.Model;
using System.Linq;
using System.Threading.Tasks;

namespace Allgregator.Sts.Services {
    internal class OreService : OreServiceBase {
        private readonly RetrieveService retrieveService;
        public OreService(
            RetrieveService retrieveService
            ) {
            this.retrieveService = retrieveService;
        }

        internal async Task Retrieve(Mined mined) {
            if (mined == null) {
                return;
            }

            var addresses = new[] { "https://unicode.org/charts/charindex.html" };

            using (retrieveService) {
                var lastRetrieve = await Retrieve(addresses, retrieveService.ProductionAsync);

                if (IsRetrieving) {
                    mined.Areas = retrieveService.Items.OrderBy(n => n.Name).ToList();
                    mined.Errors = retrieveService.Errors.Count == 0 ? null : retrieveService.Errors.ToList();//cached;
                    mined.LastRetrieve = lastRetrieve;
                    mined.IsNeedToSave = true;
                    IsRetrieving = false;
                }
            }
        }
    }
}