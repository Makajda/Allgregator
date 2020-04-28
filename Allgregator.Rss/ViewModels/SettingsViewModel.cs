using Allgregator.Aux.Models;
using Allgregator.Aux.Services;
using Allgregator.Rss.Common;
using Allgregator.Rss.Models;
using Allgregator.Rss.Repositories;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;

namespace Allgregator.Rss.ViewModels {
    internal class SettingsViewModel : BindableBase {
        private readonly OpmlRepository opmlRepository;
        private readonly DialogService dialogService;
        private readonly IEventAggregator eventAggregator;

        public SettingsViewModel(
            Settings settings,
            OpmlRepository opmlRepository,
            IEventAggregator eventAggregator,
            DialogService dialogService
            ) {
            this.opmlRepository = opmlRepository;
            this.eventAggregator = eventAggregator;
            this.dialogService = dialogService;
            this.Settings = settings;

            AddChapterCommand = new DelegateCommand(AddChapter);
            ToOpmlCommand = new DelegateCommand(ToOpml);
            FromOpmlCommand = new DelegateCommand(FromOpml);
        }

        public DelegateCommand AddChapterCommand { get; private set; }
        public DelegateCommand ToOpmlCommand { get; private set; }
        public DelegateCommand FromOpmlCommand { get; private set; }
        public Settings Settings { get; private set; }

        private string addedName;
        public string AddedName {
            get { return addedName; }
            set { SetProperty(ref addedName, value); }
        }

        private void AddChapter() {
            eventAggregator.GetEvent<ChapterAddedEvent>().Publish(new Chapter[] { new Chapter() { Name = AddedName } });
            AddedName = null;
        }

        private async void ToOpml() {
            try {
                await opmlRepository.Export();
            }
            catch (Exception exception) {
                dialogService.Show(exception.Message);
            }
        }

        private async void FromOpml() {
            try {
                var (chapters, links) = await opmlRepository.Import();
                var str = $"+++ collections: {chapters},  RSS: {links}";
                dialogService.Show(str);
            }
            catch (Exception exception) {
                dialogService.Show(exception.Message);
            }
        }
    }
}
