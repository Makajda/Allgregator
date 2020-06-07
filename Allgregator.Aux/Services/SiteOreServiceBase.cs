using Allgregator.Aux.Models;
using System.Linq;
using System.Threading.Tasks;

namespace Allgregator.Aux.Services {
    public class SiteOreServiceBase<TItem> : OreServiceBase where TItem : IName {
        private readonly string address;
        private readonly RetrieveServiceBase<string, TItem> retrieveService;

        public SiteOreServiceBase(
            //string address,
            RetrieveServiceBase<string, TItem> retrieveService
            ) {
            //this.address = address;
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
                    mined.Items = retrieveService.Items.OrderBy(n => n.Name).ToList();
                    mined.Errors = retrieveService.Errors.Count == 0 ? null : retrieveService.Errors.ToList();//cached;
                    mined.LastRetrieve = lastRetrieve;
                    mined.IsNeedToSave = true;
                    IsRetrieving = false;
                }
            }
        }
    }
}
