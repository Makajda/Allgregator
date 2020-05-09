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

        internal async Task<TMined> GetOrDefault() {
            TMined retval = default;

            try {
                retval = await Get();
            }
            catch (Exception e) {
                Serilog.Log.Error(e, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }

            return retval ?? new TMined();
        }

        internal async Task Save(TMined mined) {
            var (entryName, fileName) = GetNames();

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

        internal void DeleteFile() {
            var (_, name) = GetNames();
            File.Delete(name);
        }

        private async Task<TMined> Get() {
            var (_, fileName) = GetNames();
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

        private (string EntryName, string FileName) GetNames() {
            string n = $"{name}Mined";
            var entryName = Path.ChangeExtension(n, "json");
            var fileName = Path.Combine(Given.PathData, Path.ChangeExtension(n, "zip"));
            return (entryName, fileName);
        }
    }
}
