using Allgregator.Aux.Models;
using System.Linq;
using System.Threading.Tasks;

namespace Allgregator.Aux.Services {
    public class SiteOreServiceBase<TItem> : OreServiceBase {
        private string address;
        private RetrieveServiceBase<string, TItem> retrieveService;

        public void Initialize(string address, SiteRetrieveServiceBase<TItem> retrieveService) {
            this.address = address;
            this.retrieveService = retrieveService;
        }

        public async Task Retrieve(MinedBase<TItem> mined) {
            if (mined == null) {
                return;
            }

            var addresses = new[] { address };

            using (retrieveService) {
                var lastRetrieve = await Retrieve(addresses, retrieveService.ProductionAsync);

                if (IsRetrieving) {
                    if (typeof(IOrdered).IsAssignableFrom(typeof(TItem)))
                        mined.Items = retrieveService.Items.OrderBy(n => ((IOrdered)n).Name).ToList();
                    else
                        mined.Items = retrieveService.Items.ToList();

                    mined.Errors = retrieveService.Errors.Count == 0 ? null : retrieveService.Errors.ToList();//cached;
                    mined.LastRetrieve = lastRetrieve;
                    mined.IsNeedToSave = true;
                    IsRetrieving = false;
                }
            }
        }
    }
}
