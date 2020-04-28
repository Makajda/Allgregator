using Allgregator.Aux.Common;
using Allgregator.Fin.Models;
using System;
using System.IO;
using System.IO.Compression;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;

namespace Allgregator.Fin.Repositories {
    public class MinedRepository {
        internal async Task<Mined> GetOrDefault() {
            Mined retval = null;

            try {
                retval = await Get();
            }
            catch (Exception e) {
                Serilog.Log.Error(e, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }

            return retval ?? new Mined();
        }

        internal async Task Save(Mined mined) {
            var (entryName, fileName) = GetNames();

            using var fileStream = new FileStream(fileName, FileMode.Create);
            using var archive = new ZipArchive(fileStream, ZipArchiveMode.Create);
            var entry = archive.CreateEntry(entryName);
            using var streamEntry = entry.Open();
            using var writer = new StreamWriter(streamEntry);
            var json = JsonSerializer.Serialize<Mined>(
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

        private async Task<Mined> Get() {
            var (_, fileName) = GetNames();
            Mined mined = null;

            using var archive = ZipFile.OpenRead(fileName);
            if (archive.Entries.Count > 0) {
                var entry = archive.Entries[0];
                if (entry != null) {
                    using var stream = entry.Open();
                    using var reader = new StreamReader(entry.Open());
                    var json = await reader.ReadToEndAsync();
                    mined = JsonSerializer.Deserialize<Mined>(json);
                }
            }

            return mined;
        }

        private (string EntryName, string FileName) GetNames() {
            const string name = "finMined";
            var entryName = Path.ChangeExtension(name, "json");
            var fileName = Path.Combine(Given.PathData, Path.ChangeExtension(name, "zip"));
            return (entryName, fileName);
        }
    }
}
