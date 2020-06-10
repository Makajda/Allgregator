using Allgregator.Aux.Models;
using System;
using System.IO;
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
                //var html = await webService.GetHtml(address);
                var html = await File.ReadAllTextAsync(@"c:\files\brushes.html");

                Process(html);
            }
            catch (OperationCanceledException) { }
            catch (Exception exception) {
                Errors.Add(new Error() { Source = address, Message = exception.Message });
            }
        }
    }
}
