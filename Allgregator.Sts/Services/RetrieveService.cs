using Allgregator.Aux.Models;
using Allgregator.Aux.Services;
using System;
using System.Threading.Tasks;

namespace Allgregator.Sts.Services {
    internal class RetrieveService : RetrieveServiceBase<DateTimeOffset, object> {//todo
        private readonly WebService webService;

        public RetrieveService(
            WebService webService
            ) {
            this.webService = webService;
        }

        public override async Task ProductionAsync(DateTimeOffset date) {
            var address = $"";

            try {
                var html = await webService.GetHtml(address);

                lock (syncItems) {
                    Items.Add(new object());
                }
            }
            catch (OperationCanceledException) { }
            catch (Exception exception) {
                lock (syncErrors) {
                    Errors.Add(new Error() { Source = null, Message = exception.Message });
                }
            }
        }
    }
}
