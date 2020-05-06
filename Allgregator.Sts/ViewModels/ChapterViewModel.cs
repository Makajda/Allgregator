using Allgregator.Aux.Common;
using Allgregator.Aux.Models;
using Allgregator.Sts.Services;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Allgregator.Sts.ViewModels {
    internal class ChapterViewModel : ChapterViewModelBase {
        private readonly Settings settings;
        public ChapterViewModel(
            Settings settings,
            OreService oreService,
            IEventAggregator eventAggregator
            ) : base(eventAggregator) {
            OreService = oreService;
            this.settings = settings;
        }

        public OreService OreService { get; private set; }
        protected override int ChapterId => Given.StsChapter;
        protected override async Task Activate() {
            await LoadMined();
        }
        protected override async Task Deactivate() => await SaveMined();
        protected override void Run() { }
        protected override void WindowClosing(CancelEventArgs args) {
            if (IsActive) settings.CurrentChapterId = ChapterId;
            AsyncHelper.RunSync(async () => await SaveMined());
        }
        protected override Task Update() => Task.CompletedTask;

        private async Task LoadMined() {
        }

        private async Task SaveMined() {
        }
    }
}
