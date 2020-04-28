﻿using Allgregator.Aux.Services;
using Allgregator.Rss.Common;
using Allgregator.Rss.Models;
using Allgregator.Rss.Repositories;
using Allgregator.Rss.Services;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Allgregator.Rss.ViewModels {
    internal class ChaptersViewModel : BindableBase {
        private readonly FactoryService factoryService;
        private readonly ViewsService viewsService;
        private readonly ChapterRepository chapterRepository;

        public ChaptersViewModel(
            FactoryService factoryService,
            ViewsService viewsService,
            ChapterRepository chapterRepository,
            IEventAggregator eventAggregator
            ) {
            this.factoryService = factoryService;
            this.viewsService = viewsService;
            this.chapterRepository = chapterRepository;

            eventAggregator.GetEvent<ChapterDeletedEvent>().Subscribe(ChapterDeleted);
            eventAggregator.GetEvent<ChapterAddedEvent>().Subscribe(ChapterAdded);
        }

        private ObservableCollection<ChapterViewModel> chapters;
        public ObservableCollection<ChapterViewModel> Chapters {
            get => chapters;
            set => SetProperty(ref chapters, value);
        }

        internal async Task Load() {
            var chapters = await chapterRepository.GetOrDefault();
            if (chapters != null) {
                Chapters = new ObservableCollection<ChapterViewModel>(
                    chapters.Select(n => factoryService.Resolve<Chapter, ChapterViewModel>(n)));
            }
        }

        private async void ChapterAdded(Chapter[] chapters) {
            foreach (var newChapter in chapters) {
                if (newChapter.Id == 0) {
                    newChapter.Id = chapterRepository.GetNewId(Chapters.Select(n => n.Chapter));
                }

                Chapters.Add(factoryService.Resolve<Chapter, ChapterViewModel>(newChapter));
            }

            await Save();
        }

        private async void ChapterDeleted(int id) {
            var chapter = Chapters.FirstOrDefault(n => n.Chapter.Id == id);
            if (chapter != null) {
                Chapters.Remove(chapter);
                chapter.IsActive = false;
                await Save();

                viewsService.RemoveMainViews(chapter.Chapter);
            }
        }

        private async Task Save() {
            try {
                await chapterRepository.Save(Chapters.Select(n => n.Chapter));
            }
            catch (Exception e) {
                Serilog.Log.Error(e, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }
        }
    }
}
