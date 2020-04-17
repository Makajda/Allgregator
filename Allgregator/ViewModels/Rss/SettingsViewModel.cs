using Allgregator.Models;
using Allgregator.Repositories.Rss;
using Allgregator.Services;
using Prism.Commands;
using Prism.Mvvm;
using System;

namespace Allgregator.ViewModels.Rss {
    public class SettingsViewModel : BindableBase {
        private readonly OpmlRepository opmlRepository;
        private readonly DialogService dialogService;
        public SettingsViewModel(
            Settings settings,
            OpmlRepository opmlRepository,
            DialogService dialogService
            ) {
            this.opmlRepository = opmlRepository;
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
                var str = $"+ collections: {chapters},  RSS: {links}";
                dialogService.Show(str);
            }
            catch (Exception exception) {
                dialogService.Show(exception.Message);
            }
        }
    }
}
