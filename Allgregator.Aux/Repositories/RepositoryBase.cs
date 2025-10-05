using Allgregator.Aux.Common;
using Allgregator.Aux.Models;
using System;
using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;

namespace Allgregator.Aux.Repositories {
    public class RepositoryBase<TModel> where TModel : IWatchSave, new() {
        protected string startOfName;

        public void SetNames(string moduleName, string modelName = null) {
            this.startOfName = $"{moduleName}{(modelName ?? typeof(TModel).Name)}";
        }

        public async Task<TModel> GetOrDefault(int id = 0) {
            TModel retval = default;

            try {
                retval = await Get(id);
            }
            catch (Exception e) {
                Serilog.Log.Error(e, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }

            if (retval == null) {
                retval = CreateDefault(id);
            }

            retval.IsNeedToSave = false;
            return retval;
        }

        public virtual async Task Save(TModel model, int id = 0) {
            if (model.IsNeedToSave) {
                model.IsNeedToSave = false;
                var json = JsonSerializer.Serialize<TModel>(model,
                    new JsonSerializerOptions() {
                        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
                        WriteIndented = true
                    });

                var name = GetName(id);
                await File.WriteAllTextAsync(name, json);
            }
        }

        public virtual void DeleteFile(int id = 0) {
            var name = GetName(id);
            File.Delete(name);
        }

        protected virtual TModel CreateDefault(int id = 0) => new TModel();

        protected virtual async Task<TModel> Get(int id = 0) {
            var name = GetName(id);
            var json = await File.ReadAllTextAsync(name);
            return JsonSerializer.Deserialize<TModel>(json);
        }

        private string GetName(int id) => Path.Combine(Given.PathData,
            Path.ChangeExtension($"{startOfName}{(id == default ? null : id.ToString())}", "json"));
    }
}
