using Allgregator.Aux.Common;
using System;
using System.IO;
using System.IO.Compression;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;

namespace Allgregator.Aux.Repositories {
    public class ZipRepositoryBase<TModel> where TModel : new() {
        private readonly Lazy<string> startOfName = new Lazy<string>(typeof(TModel).Name);
        public async Task<TModel> GetOrDefault(string id = null) {
            TModel retval = default;

            try {
                retval = await Get(id);
            }
            catch (Exception e) {
                Serilog.Log.Error(e, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }

            return retval ?? CreateDefault(id);
        }

        public async Task Save(TModel model, string id = null) {
            var (entryName, fileName) = GetNames(id);

            using var fileStream = new FileStream(fileName, FileMode.Create);
            using var archive = new ZipArchive(fileStream, ZipArchiveMode.Create);
            var entry = archive.CreateEntry(entryName);
            using var streamEntry = entry.Open();
            using var writer = new StreamWriter(streamEntry);
            var json = JsonSerializer.Serialize<TModel>(
                model,
                new JsonSerializerOptions() {
                    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                    IgnoreNullValues = true,
                    WriteIndented = false
                });

            await writer.WriteLineAsync(json);
        }

        public void DeleteFile(string id = null) {
            var (_, name) = GetNames(id);
            File.Delete(name);
        }

        protected virtual TModel CreateDefault(string id) => new TModel();

        private async Task<TModel> Get(string id) {
            var (_, fileName) = GetNames(id);
            TModel model = default;

            using var archive = ZipFile.OpenRead(fileName);
            if (archive.Entries.Count > 0) {
                var entry = archive.Entries[0];
                if (entry != null) {
                    using var stream = entry.Open();
                    using var reader = new StreamReader(entry.Open());
                    var json = await reader.ReadToEndAsync();
                    model = JsonSerializer.Deserialize<TModel>(json);
                }
            }

            return model;
        }

        private (string EntryName, string FileName) GetNames(string id) {
            string name = $"{startOfName.Value}{id}";
            var entryName = Path.ChangeExtension(name, "json");
            var fileName = Path.Combine(Given.PathData, Path.ChangeExtension(name, "zip"));
            return (entryName, fileName);
        }
    }
}
