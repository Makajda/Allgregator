﻿using Allgregator.Common;
using Allgregator.Models.Rss;
using System;
using System.IO;
using System.IO.Compression;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;

namespace Allgregator.Repositories.Rss {
    public class MinedRepository {
        public async Task<Mined> Get(int chapterId) {
            var (entryName, fileName) = GetNames(chapterId);

            try {
                using (var archive = ZipFile.OpenRead(fileName)) {
                    if (archive.Entries.Count > 0) {
                        var entry = archive.Entries[0];
                        if (entry != null) {
                            using (var stream = entry.Open()) {
                                using (var reader = new StreamReader(entry.Open())) {
                                    var json = await reader.ReadToEndAsync();
                                    return JsonSerializer.Deserialize<Mined>(json);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception) { }

            return new Mined();
        }

        public async Task Save(int chapterId, Mined mined) {
            var (entryName, fileName) = GetNames(chapterId);

            using (var fileStream = new FileStream(fileName, FileMode.Create)) {
                using (var archive = new ZipArchive(fileStream, ZipArchiveMode.Create)) {
                    var entry = archive.CreateEntry(entryName);
                    using (var streamEntry = entry.Open()) {
                        using (var writer = new StreamWriter(streamEntry)) {
                            var json = JsonSerializer.Serialize<Mined>(mined,
                                new JsonSerializerOptions() {
                                    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                                    IgnoreNullValues = true,
                                    WriteIndented = false
                                });

                            await writer.WriteLineAsync(json);
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
