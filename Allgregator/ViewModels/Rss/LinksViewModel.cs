﻿using Allgregator.Common;
using Allgregator.Models.Rss;
using Allgregator.Repositories.Rss;
using Allgregator.Services;
using Allgregator.Services.Rss;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Allgregator.ViewModels.Rss {
    public class LinksViewModel : BindableBase {
        private readonly ChapterRepository chapterRepository;
        private readonly DialogService dialogService;
        private readonly IEventAggregator eventAggregator;
        string savedName;

        public LinksViewModel(
            Chapter chapter,
            ChapterRepository chapterRepository,
            DialogService dialogService,
            DetectionService detectionService,
            IEventAggregator eventAggregator
            ) {
            Chapter = chapter;
            this.chapterRepository = chapterRepository;
            this.eventAggregator = eventAggregator;
            this.dialogService = dialogService;

            AddCommand = new DelegateCommand(async () => await detectionService.SetAddress(Chapter.Linked));
            MoveCommand = new DelegateCommand<Link>(Move);
            DeleteCommand = new DelegateCommand<Link>(Delete);
            SelectionCommand = new DelegateCommand<Link>(link => detectionService.Selected(Chapter.Linked, link));
            ToChapterCommand = new DelegateCommand(ToChapter);
            FromChapterCommand = new DelegateCommand(FromChapter);
            DeleteChapterCommand = new DelegateCommand(DeleteChapter);

            eventAggregator.GetEvent<WindowClosingEvent>().Subscribe(e => AsyncHelper.RunSync(SaveChapterName));
        }

        public DelegateCommand AddCommand { get; private set; }
        public DelegateCommand<Link> MoveCommand { get; private set; }
        public DelegateCommand<Link> DeleteCommand { get; private set; }
        public DelegateCommand<Link> SelectionCommand { get; private set; }
        public DelegateCommand ToChapterCommand { get; private set; }
        public DelegateCommand FromChapterCommand { get; private set; }
        public DelegateCommand DeleteChapterCommand { get; private set; }
        public Chapter Chapter { get; private set; }

        private async void Move(Link link) {
            var chapters = await chapterRepository.GetOrDefault();
            dialogService.Show(chapters.Where(n => n.Id != Chapter.Id).Select(n => n.Name),
                name => {
                    var newChapter = chapters.FirstOrDefault(n => n.Name == name);
                    eventAggregator.GetEvent<LinkMovedEvent>().Publish((newChapter.Id, link));
                    Chapter.Linked.IsNeedToSave = true;
                    Chapter.Linked.Links.Remove(link);
                });
        }

        private void Delete(Link link) {
            dialogService.Show($"{link.Name}?", DeleteReal, 20, true);

            void DeleteReal() {
                Chapter.Linked.Links.Remove(link);
                Chapter.Linked.IsNeedToSave = true;
            }
        }

        private void ToChapter() {
            savedName = Chapter.Name ?? string.Empty;
            Chapter.Linked.CurrentState = RssLinksStates.Chapter;
        }

        private async void FromChapter() {
            Chapter.Linked.CurrentState = RssLinksStates.Normal;
            await SaveChapterName();
        }

        private async Task SaveChapterName() {
            if (savedName != null && Chapter.Name != savedName) {
                var chapters = await chapterRepository.GetOrDefault();
                var chapter = chapters.FirstOrDefault(n => n.Id == Chapter.Id);
                if (chapter != null) {
                    chapter.Name = string.IsNullOrEmpty(Chapter.Name) ? null : Chapter.Name;
                    var saved = await SaveChapters(chapters);
                    if (saved) {
                        savedName = default;
                    }
                }
            }
        }

        private void DeleteChapter() {
            if (Chapter?.Linked?.Links != null && Chapter.Linked.Links.Count > 0) {
                dialogService.Show($"{Chapter.Linked.Links.Count} addresses?", DeleteChapterReal, 20, true);
            }
            else {
                DeleteChapterReal();
            }

            void DeleteChapterReal() {
                eventAggregator.GetEvent<ChapterDeletedEvent>().Publish(Chapter.Id);
            }
        }

        private async Task<bool> SaveChapters(IEnumerable<Chapter> chapters) {
            try {
                await chapterRepository.Save(chapters);
                return true;
            }
            catch (Exception e) {
                /*//TODO Log*/
            }

            return false;
        }
    }
}
