using Allgregator.Common;
using Allgregator.Models.Rss;
using Allgregator.Repositories.Rss;
using Prism;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;

namespace Allgregator.ViewModels.Rss {
    public class LinksViewModel : BindableBase, IActiveAware {
        private readonly LinkRepository linksRepository;
        private readonly IRegionManager regionManager;

        public LinksViewModel(
            LinkRepository linksRepository,
            IRegionManager regionManager,
            IEventAggregator eventAggregator
            ) {
            this.linksRepository = linksRepository;
            this.regionManager = regionManager;

            eventAggregator.GetEvent<ChapterChangedEvent>().Subscribe(ChangeChapter);
            eventAggregator.GetEvent<WindowClosingEvent>().Subscribe(async (cancelEventArgs) => await Save(cancelEventArgs));
            IsActiveChanged += async (s, e) => await Load();
        }

        public event EventHandler IsActiveChanged;

        private Chapter chapter;
        public Chapter Chapter {
            get => chapter;
            private set => SetProperty(ref chapter, value);
        }

        private bool isActive;
        public bool IsActive {
            get => isActive;
            set => SetProperty(ref isActive, value, () => IsActiveChanged?.Invoke(this, EventArgs.Empty));
        }

        private async void ChangeChapter(Chapter chapter) {
            await Save();
            Chapter = chapter;
            await Load();
        }

        private async Task Load() {
            if (IsActive && Chapter != null && Chapter.Links == null) {
                try {
                    Chapter.Links = new ObservableCollection<Link>(await linksRepository.Get(Chapter.Id));
                }
                catch (Exception) {
                    /*//TODO Log*/
                    Chapter.Links = new ObservableCollection<Link>() {
                        new Link() {
                            HtmlUrl = "http://feeds.bbci.co.uk/news/health/rss.xml",
                            Name = "BBC News - Health",
                            XmlUrl = "http://feeds.bbci.co.uk/news/health/rss.xml"
                        },
                        new Link() {
                            HtmlUrl = "http://feeds.skynews.com/feeds/rss/business.xml",
                            Name = "Business News - Markets reports and financial news from Sky",
                            XmlUrl = "http://feeds.skynews.com/feeds/rss/business.xml"
                        },
                        new Link() {
                            HtmlUrl = "http://rss.cnn.com/rss/edition_technology.rss",
                            Name = "CNN.com - Technology",
                            XmlUrl = "http://rss.cnn.com/rss/edition_technology.rss"
                        },
                        new Link() {
                            HtmlUrl = "http://feeds.foxnews.com/foxnews/sports",
                            Name = "FOX News",
                            XmlUrl = "http://feeds.foxnews.com/foxnews/sports"
                        },
                        new Link() {
                            HtmlUrl = "http://feeds.reuters.com/news/artsculture",
                            Name = "Reuters: Arts",
                            XmlUrl = "http://feeds.reuters.com/news/artsculture"
                        }
                    };
                }
            }
        }

        private async Task Save(CancelEventArgs cancelEventArgs = null) {
            if (Chapter != null && Chapter.Links != null) {
                try {
                    //todo needSave await linksRepository.Save(Chapter.Id, Chapter.Links);
                }
                catch (Exception) { /*//TODO Log*/ }
            }
        }
    }
}

