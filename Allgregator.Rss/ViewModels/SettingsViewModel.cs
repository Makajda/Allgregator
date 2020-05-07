using Allgregator.Aux.Common;
using Allgregator.Aux.Models;
using Allgregator.Aux.Services;
using Allgregator.Rss.Models;
using Allgregator.Rss.Repositories;
using Allgregator.Rss.Services;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Allgregator.Rss.ViewModels {
    internal class SettingsViewModel : BindableBase {
        private readonly OpmlRepository opmlRepository;
        private readonly ChapterRepository chapterRepository;
        private readonly RepoService repoService;
        private readonly ViewService viewService;
        private readonly DialogService dialogService;

        public SettingsViewModel(
            Settings settings,
            OpmlRepository opmlRepository,
            ChapterRepository chapterRepository,
            RepoService repoService,
            ViewService viewService,
            IRegionManager regionManager,
            IEventAggregator eventAggregator,
            DialogService dialogService
            ) {
            if (regionManager.Regions[Given.RegionMain].Context is Data data) {
                Data = data;
            }

            this.opmlRepository = opmlRepository;
            this.chapterRepository = chapterRepository;
            this.repoService = repoService;
            this.viewService = viewService;
            this.dialogService = dialogService;
            this.Settings = settings;

            AddChapterCommand = new DelegateCommand(AddChapter);
            DeleteChapterCommand = new DelegateCommand(DeleteChapter);
            ExportOpmlCommand = new DelegateCommand(ExportOpml);
            ImportOpmlCommand = new DelegateCommand(ImportOpml);

            eventAggregator.GetEvent<WindowClosingEvent>().Subscribe(SaveChapterName);
        }

        public DelegateCommand AddChapterCommand { get; private set; }
        public DelegateCommand DeleteChapterCommand { get; private set; }
        public DelegateCommand ExportOpmlCommand { get; private set; }
        public DelegateCommand ImportOpmlCommand { get; private set; }
        public Settings Settings { get; private set; }
        public Data Data { get; private set; }

        private string addedName;
        public string AddedName {
            get { return addedName; }
            set { SetProperty(ref addedName, value); }
        }

        private void AddChapter() {
            var chapters = chapterRepository.GetOrDefault().ToList();
            var chapter = chapterRepository.GetNewChapter(chapters, AddedName);
            chapters.Add(chapter);
            Save(chapters);
            viewService.AddModuleViews(new[] { chapter });
        }

        private void SaveChapterName(CancelEventArgs obj) {
            if (Data.IsNeedToSave) {
                var chapters = chapterRepository.GetOrDefault();
                var chapter = chapters.FirstOrDefault(n => n.Id == Data.Id);
                if (chapter != null) {
                    chapter.Name = string.IsNullOrEmpty(Data.Name) ? null : Data.Name;
                    Save(chapters);
                }
            }
        }

        private async void DeleteChapter() {
            await repoService.LoadLinks(Data);
            dialogService.Show($"{Data.Linked.Links.Count} addresses?", DeleteChapterReal, 20, true);

            void DeleteChapterReal() {
                var chapters = chapterRepository.GetOrDefault().Where(n => n.Id != Data.Id);
                Save(chapters);
                repoService.DeleteFiles(Data.Id);
                viewService.RemoveMainViews(Data.Id);
            }
        }

        private void Save(IEnumerable<Data> chapters) {
            try {
                chapterRepository.Save(chapters);
            }
            catch (Exception e) {
                Serilog.Log.Error(e, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }
        }

        private async void ExportOpml() {
            try {
                await opmlRepository.Export();
            }
            catch (Exception exception) {
                dialogService.Show(exception.Message);
            }
        }

        private async void ImportOpml() {
            try {
                var chapters = await opmlRepository.Import();
                if (chapters != null) {
                    viewService.AddModuleViews(chapters);

                    var str = $"added {chapters.Length} collections";
                    dialogService.Show(str);
                }
            }
            catch (Exception exception) {
                dialogService.Show(exception.Message);
            }
        }
    }
}
