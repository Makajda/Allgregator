using Allgregator.Aux.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Allgregator.Aux.Services {
    public class RetrieveServiceBase<Tin, Tout> : IDisposable {
        protected readonly object syncItems = new object();
        protected readonly object syncErrors = new object();

        public List<Tout> Items { get; } = new List<Tout>();
        public List<Error> Errors { get; } = new List<Error>();

        public void Dispose() {
            Items.Clear();
            Errors.Clear();
        }

        public virtual Task ProductionAsync(Tin arg) => Task.CompletedTask;
        public virtual void Production(Tin arg) { }
    }
}
