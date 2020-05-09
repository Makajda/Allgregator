using Allgregator.Aux.Common;
using System;
using System.IO;
using System.IO.Compression;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;

namespace Allgregator.Aux.Repositories {
    public class MinedRepositoryBase<TMined> where TMined : new() {
        protected string name;

        public async Task<TMined> GetOrDefault(int id = 0) {
            TMined retval = default;

            try {
                retval = await Get(id);
            }
            catch (Exception e) {
                Serilog.Log.Error(e, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }

            return retval ?? new TMined();
        }

        public async Task Save(TMined mined, int id = 0) {
            var (entryName, fileName) = GetNames(id);

            using var fileStream = new FileStream(fileName, FileMode.Create);
            using var archive = new ZipArchive(fileStream, ZipArchiveMode.Create);
            var entry = archive.CreateEntry(entryName);
            using var streamEntry = entry.Open();
            using var writer = new StreamWriter(streamEntry);
            var json = JsonSerializer.Serialize<TMined>(
                mined,
                new JsonSerializerOptions() {
                    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                    IgnoreNullValues = true,
                    WriteIndented = false
                });

            await writer.WriteLineAsync(json);
        }

        public void DeleteFile(int id = 0) {
            var (_, name) = GetNames(id);
            File.Delete(name);
        }

        private async Task<TMined> Get(int id = 0) {
            var (_, fileName) = GetNames(id);
            TMined mined = default;

            using var archive = ZipFile.OpenRead(fileName);
            if (archive.Entries.Count > 0) {
                var entry = archive.Entries[0];
                if (entry != null) {
                    using var stream = entry.Open();
                    using var reader = new StreamReader(entry.Open());
                    var json = await reader.ReadToEndAsync();
                    mined = JsonSerializer.Deserialize<TMined>(json);
                }
            }

            return mined;
        }

        private (string EntryName, string FileName) GetNames(int id) {
            string n = id == 0 ? $"{name}Mined" : $"{name}Mined{id}";
            var entryName = Path.ChangeExtension(n, "json");
            var fileName = Path.Combine(Given.PathData, Path.ChangeExtension(n, "zip"));
            return (entryName, fileName);
        }
    }
}
