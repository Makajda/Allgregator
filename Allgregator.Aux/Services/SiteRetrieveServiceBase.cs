using Allgregator.Aux.Models;
using System;
using System.Threading.Tasks;

namespace Allgregator.Aux.Services {
    public abstract class SiteRetrieveServiceBase<TItem> : RetrieveServiceBase<string, TItem> {
        protected readonly WebService webService;

        public SiteRetrieveServiceBase(
            WebService webService
            ) {
            this.webService = webService;
        }

        protected abstract void Process(string html);

        public override async Task ProductionAsync(string address) {
            try {
                var html = await webService.GetHtml(address);

                Process(html);
            }
            catch (OperationCanceledException) { }
            catch (Exception exception) {
                Errors.Add(new Error() { Source = address, Message = exception.Message });
            }
        }
    }
}
