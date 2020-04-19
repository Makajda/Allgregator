﻿using Allgregator.Common;
using Allgregator.Models.Rss;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;

namespace Allgregator.Repositories.Rss {
    public class ChapterRepository {
        private const string nameFile = "chapters.json";

        public async Task<IEnumerable<Chapter>> GetOrDefault() {
            try {
                return await Get();
            }
            catch (Exception e) {
                Serilog.Log.Error(e, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }

            return CreateDefault();
        }

        public async Task Save(IEnumerable<Chapter> chapters) {
            var name = Path.Combine(Given.PathData, nameFile);
            var json = JsonSerializer.Serialize<IEnumerable<Chapter>>(chapters.OrderBy(n => n.Name),
                new JsonSerializerOptions() {
                    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                    IgnoreNullValues = true,
                    WriteIndented = true
                });

            await File.WriteAllTextAsync(name, json);
        }

        public int GetNewId(IEnumerable<Chapter> chapters) {
            var newId = 1;
            foreach (var chapter in chapters.OrderBy(n => n.Id)) {
                var id = chapter.Id;
                if (id != newId) break;
                newId++;
            }

            return newId;
        }

        private async Task<IEnumerable<Chapter>> Get() {
            var name = Path.Combine(Given.PathData, nameFile);
            var json = await File.ReadAllTextAsync(name);
            return JsonSerializer.Deserialize<IEnumerable<Chapter>>(json);
        }

        private IEnumerable<Chapter> CreateDefault() {
            return new List<Chapter>() {
                new Chapter() { Id = 10001, Name = "Try" }
            };
        }
    }
}
