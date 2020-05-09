using Allgregator.Aux.Common;
using System;
using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;

namespace Allgregator.Aux.Repositories {
    public class RepositoryBase<TModel> where TModel : new() {
        private readonly Lazy<string> startOfName = new Lazy<string>(typeof(TModel).Name);
        public async Task<TModel> GetOrDefault(string id) {
            TModel retval = default;

            try {
                retval = await Get(id);
            }
            catch (Exception e) {
                Serilog.Log.Error(e, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }

            return retval ?? CreateDefault(id);
        }

        public async Task Save(TModel model, string id) {
            var json = JsonSerializer.Serialize<TModel>(model,
                new JsonSerializerOptions() {
                    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                    IgnoreNullValues = true,
                    WriteIndented = true
                });

            var name = GetName(id);
            await File.WriteAllTextAsync(name, json);
        }

        public void DeleteFile(string id) {
            var name = GetName(id);
            File.Delete(name);
        }

        protected virtual TModel CreateDefault(string id) => new TModel();

        private async Task<TModel> Get(string id) {
            var name = GetName(id);
            var json = await File.ReadAllTextAsync(name);
            return JsonSerializer.Deserialize<TModel>(json);
        }

        private string GetName(string id) => Path.Combine(Given.PathData, Path.ChangeExtension($"{startOfName}{id}", "json"));
    }
}
