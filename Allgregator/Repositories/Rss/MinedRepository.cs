using Allgregator.Common;
using Allgregator.Models.Rss;
using Newtonsoft.Json;
using System;
using System.IO;
using System.IO.Compression;

namespace Allgregator.Repositories.Rss {
    public class MinedRepository {
        public Mined GetMined(int chapterId) {
            var (entryName, fileName) = GetNames(chapterId);

            try {
                using (var archive = ZipFile.OpenRead(fileName)) {
                    if (archive.Entries.Count > 0) {
                        var entry = archive.Entries[0];
                        if (entry != null) {
                            using (var stream = entry.Open()) {
                                using (var reader = new StreamReader(entry.Open())) {
                                    var json = reader.ReadToEnd();
                                    return JsonConvert.DeserializeObject<Mined>(json);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception) { }

            return new Mined();
        }

        public void Save(int chapterId, Mined mined) {
            var (entryName, fileName) = GetNames(chapterId);

            using (var fileStream = new FileStream(fileName, FileMode.Open)) {
                using (var archive = new ZipArchive(fileStream, ZipArchiveMode.Create)) {
                    var entry = archive.CreateEntry(entryName);
                    using (var streamEntry = entry.Open()) {
                        using (var writer = new StreamWriter(streamEntry)) {
                            var json = JsonConvert.SerializeObject(mined, Formatting.Indented);
                            writer.WriteLine(json);
                        }
                    }
                }
            }
        }

        private (string, string) GetNames(int chapterId) {
            const string nameFile = "mined{0}";
            var name = string.Format(nameFile, chapterId);
            var entryName = Path.ChangeExtension(name, "json");
            var fileName = Path.Combine(Given.PathData, Path.ChangeExtension(name, "zip"));
            return (entryName, fileName);
        }
    }
}
