﻿using Allgregator.Aux.Common;
using System;
using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;

namespace Allgregator.Aux.Repositories {
    public abstract class RepositoryBase<TModel> where TModel : new() {
        protected readonly Lazy<string> startOfName;

        public RepositoryBase() {
            startOfName = new Lazy<string>($"{ModuleName}{typeof(TModel).Name}");
        }

        protected abstract string ModuleName { get; }

        public async Task<TModel> GetOrDefault(int id = 0) {
            TModel retval = default;

            try {
                retval = await Get(id);
            }
            catch (Exception e) {
                Serilog.Log.Error(e, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }

            return retval ?? CreateDefault(id);
        }

        public virtual async Task Save(TModel model, int id = 0) {
            var json = JsonSerializer.Serialize<TModel>(model,
                new JsonSerializerOptions() {
                    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                    IgnoreNullValues = true,
                    WriteIndented = true
                });

            var name = GetName(id);
            await File.WriteAllTextAsync(name, json);
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