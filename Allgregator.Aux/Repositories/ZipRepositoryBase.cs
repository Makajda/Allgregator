using Allgregator.Aux.Common;
using Allgregator.Aux.Models;
using System.IO;
using System.IO.Compression;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;

namespace Allgregator.Aux.Repositories {
    public abstract class ZipRepositoryBase<TModel> : RepositoryBase<TModel> where TModel : IWatchSave, new() {
        public ZipRepositoryBase(string moduleName, string modelName = null) : base(moduleName, modelName) { }
        public override async Task Save(TModel model, int id = 0) {
            if (model.IsNeedToSave) {
                model.IsNeedToSave = false;
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
        }

        public override void DeleteFile(int id = 0) {
            var (_, name) = GetNames(id);
            File.Delete(name);
        }

        protected override async Task<TModel> Get(int id = 0) {
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

        private (string EntryName, string FileName) GetNames(int id) {
            string name = $"{startOfName}{(id == default ? null : id.ToString())}";
            var entryName = Path.ChangeExtension(name, "json");
            var fileName = Path.Combine(Given.PathData, Path.ChangeExtension(name, "zip"));
            return (entryName, fileName);
        }
    }
}
