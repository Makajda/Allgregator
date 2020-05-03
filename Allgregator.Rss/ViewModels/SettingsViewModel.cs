using Allgregator.Aux.Common;
using Allgregator.Aux.Models;
using Allgregator.Aux.Services;
using Allgregator.Rss.Common;
using Allgregator.Rss.Models;
using Allgregator.Rss.Repositories;
using Allgregator.Rss.Services;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using System;

namespace Allgregator.Rss.ViewModels {
    internal class SettingsViewModel : BindableBase {
        private readonly OpmlRepository opmlRepository;
        private readonly DialogService dialogService;
        private readonly IEventAggregator eventAggregator;//todo

        public SettingsViewModel(
            Settings settings,
            OpmlRepository opmlRepository,
            ViewService viewService,
            IRegionManager regionManager,
            IEventAggregator eventAggregator,
            DialogService dialogService
            ) {
            if (regionManager.Regions[Given.MainRegion].Context is Data data) {
                Data = data;
            }

            this.opmlRepository = opmlRepository;
            this.eventAggregator = eventAggregator;
            this.dialogService = dialogService;
            this.Settings = settings;

            AddChapterCommand = new DelegateCommand(AddChapter);
            //DeleteChapterCommand = new DelegateCommand(DeleteChapter);
            ToOpmlCommand = new DelegateCommand(ToOpml);
            FromOpmlCommand = new DelegateCommand(FromOpml);
            //todo eventAggregator.GetEvent<ChapterDeletedEvent>().Subscribe(ChapterDeleted);
            //todo eventAggregator.GetEvent<ChapterAddedEvent>().Subscribe(ChapterAdded);
            //todo internal class ChapterDeletedEvent : PubSubEvent<int> { }
            //todo internal class ChapterAddedEvent : PubSubEvent<Chapter[]> { }
            //eventAggregator.GetEvent<WindowClosingEvent>().Subscribe(e => AsyncHelper.RunSync(SaveChapterName));
        }

        public DelegateCommand AddChapterCommand { get; private set; }
        public DelegateCommand DeleteChapterCommand { get; private set; }
        public DelegateCommand ToOpmlCommand { get; private set; }
        public DelegateCommand FromOpmlCommand { get; private set; }
        public Settings Settings { get; private set; }
        public Data Data { get; private set; }

        private string addedName;
        public string AddedName {
            get { return addedName; }
            set { SetProperty(ref addedName, value); }
        }

        private void AddChapter() {
            //todo eventAggregator.GetEvent<ChapterAddedEvent>().Publish(new Chapter[] { new Chapter() { Name = AddedName } });
            AddedName = null;
        }

        //private async Task SaveChapterName() {
        //todo if (savedName != null && Chapter.Name != savedName) {
        //    var chapters = await chapterRepository.GetOrDefault();
        //    var chapter = chapters.FirstOrDefault(n => n.Id == Chapter.Id);
        //    if (chapter != null) {
        //        chapter.Name = string.IsNullOrEmpty(Chapter.Name) ? null : Chapter.Name;
        //        try {
        //            await chapterRepository.Save(chapters);
        //            savedName = null;
        //        }
        //        catch (Exception e) {
        //            Serilog.Log.Error(e, System.Reflection.MethodBase.GetCurrentMethod().Name);
        //        }
        //    }
        //}
        //}

        //private void DeleteChapter() {
        //    if (Data.Linked?.Links != null && Data.Linked.Links.Count > 0) {
        //        dialogService.Show($"{Data.Linked.Links.Count} addresses?", DeleteChapterReal, 20, true);
        //    }
        //    else {
        //        DeleteChapterReal();
        //    }

        //    void DeleteChapterReal() {
        //        //todo eventAggregator.GetEvent<ChapterDeletedEvent>().Publish(id);
        //    }
        //}

        //private async void ChapterAdded(Chapter[] chapters) {
        //    foreach (var newChapter in chapters) {
        //        if (newChapter.Id == 0) {
        //            newChapter.Id = chapterRepository.GetNewId(Chapters.Select(n => n.Chapter));
        //        }

        //        Chapters.Add(factoryService.Resolve<Chapter, ChapterViewModel>(newChapter));
        //    }

        //    await Save();
        //}

        //private async void ChapterDeleted(int id) {
        //    var chapter = Chapters.FirstOrDefault(n => n.Chapter.Id == id);
        //    if (chapter != null) {
        //        Chapters.Remove(chapter);
        //        chapter.IsActive = false;
        //        await Save();

        //        viewsService.RemoveMainViews(chapter.Chapter);
        //    }
        //}

        //private async Task Save() {
        //    try {
        //        await chapterRepository.Save(Chapters.Select(n => n.Chapter));
        //    }
        //    catch (Exception e) {
        //        Serilog.Log.Error(e, System.Reflection.MethodBase.GetCurrentMethod().Name);
        //    }
        //}


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
