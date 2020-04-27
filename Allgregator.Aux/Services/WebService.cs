using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Allgregator.Aux.Services {
    public class WebService : IDisposable {
        private HttpClient httpClient;

        public void Dispose() {
            httpClient.Dispose();
        }

        public async Task<string> TryGetHtml(string address) {
            if (httpClient == null) {
                httpClient = new HttpClient();
            }

            string html;
            try {
                html = await httpClient.GetStringAsync(address);
            }
            catch (Exception e) {
                Serilog.Log.Error(e, System.Reflection.MethodBase.GetCurrentMethod().Name);
                html = null;
            }

            return html;
        }

        public async Task<string> GetHtml(string address) {
            if (httpClient == null) {
                httpClient = new HttpClient();
            }

            var html = await httpClient.GetStringAsync(address);
            return html;
        }
    }
}
