using Allgregator.Common;
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

namespace Allgregator.ViewModels.Rss {
    public class LinksViewModel : BindableBase {
        private readonly ChapterRepository chapterRepository;
        private readonly DialogService dialogService;
        private readonly DetectionService detectionService;
        private readonly IEventAggregator eventAggregator;
        public LinksViewModel(
            ChapterRepository chapterRepository,
            DialogService dialogService,
            DetectionService detectionService,
            IEventAggregator eventAggregator
            ) {
            this.chapterRepository = chapterRepository;
            this.eventAggregator = eventAggregator;
            this.dialogService = dialogService;
            this.detectionService = detectionService;

            AddCommand = new DelegateCommand(Add);
            MoveCommand = new DelegateCommand<Link>(Move);
            DeleteCommand = new DelegateCommand<Link>(Delete);
            SelectionCommand = new DelegateCommand<Link>(Selected);

            eventAggregator.GetEvent<CurrentChapterChangedEvent>().Subscribe(chapter => Chapter = chapter);
        }

        public DelegateCommand AddCommand { get; private set; }
        public DelegateCommand<Link> MoveCommand { get; private set; }
        public DelegateCommand<Link> DeleteCommand { get; private set; }
        public DelegateCommand<Link> SelectionCommand { get; private set; }

        private Chapter chapter;
        public Chapter Chapter {
            get => chapter;
            private set => SetProperty(ref chapter, value);
        }

        private RssLinksView currentView;
        public RssLinksView CurrentView {
            get => currentView;
            private set => SetProperty(ref currentView, value);
        }

        private string address;
        public string Address {
            get { return address; }
            set { SetProperty(ref address, value); }
        }

        private IEnumerable<Link> links;
        public IEnumerable<Link> Links {
            get { return links; }
            set { SetProperty(ref links, value); }
        }

        private async void Move(Link link) {
            var chapters = await chapterRepository.GetOrDefault();
            dialogService.Show(chapters.Where(n => n.Id != Chapter.Id).Select(n => n.Name),
                name => {
                    var newChapter = chapters.FirstOrDefault(n => n.Name == name);
                    eventAggregator.GetEvent<LinkMovedEvent>().Publish((newChapter.Id, link));
                    Chapter.IsNeedToSaveLinks = true;
                    Chapter.Links.Remove(link);
                });
        }

        private void Delete(Link link) {
            dialogService.Show($"{link.Name}?", DeleteReal, 20, true);

            void DeleteReal() {
                Chapter.Links.Remove(link);
                Chapter.IsNeedToSaveLinks = true;
            }
        }

        private async void Add() {
            CurrentView = RssLinksView.DetectionView;
            var link = await detectionService.GetLink(Address);
            if (link != null) {
                Selected(link);
            }
            else {
                Links = await detectionService.GetLinks(Address);
                CurrentView = Links == null ? RssLinksView.NormalView : RssLinksView.SelectionView;
            }
        }

        private void Selected(Link link) {
            if (link.XmlUrl != null) {
                Address = null;
                Chapter.Links.Add(link);
                Chapter.IsNeedToSaveLinks = true;
            }

            CurrentView = RssLinksView.NormalView;
        }
    }
}
