using Allgregator.Common;
using Allgregator.Models.Rss;
using Allgregator.Repositories.Rss;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Allgregator.Services.Rss {
    public class ChapterService {
        private readonly LinkRepository linkRepository;
        private readonly MinedRepository minedRepository;
        public ChapterService(
            LinkRepository linkRepository,
            MinedRepository minedRepository
            ) {
            this.linkRepository = linkRepository;
            this.minedRepository = minedRepository;
        }

        public async Task Load(Chapter chapter, RssChapterViews currentView) {
            await LoadMined(chapter, currentView != RssChapterViews.LinksView);
            await LoadLinks(chapter, currentView == RssChapterViews.LinksView);
        }

        public async Task Load(Chapter chapter) {
            await LoadMined(chapter);
            await LoadLinks(chapter);
        }

        public async Task Save(Chapter chapter) {
            await SaveLinks(chapter);
            await SaveMined(chapter);
        }

        public async Task LinkMoved(Chapter chapter, (int Id, Link Link) obj) {
            if (obj.Id == chapter.Id) {
                await LoadLinks(chapter);
                chapter.Links.Add(obj.Link);
                await SaveLinks(chapter);
            }
        }

        private async Task LoadLinks(Chapter chapter, bool force = true) {
            if (chapter != null && chapter.Links == null && force) {
                var links = await linkRepository.GetOrDefault(chapter.Id);
                chapter.Links = new ObservableCollection<Link>(links);
            }
        }

        private async Task LoadMined(Chapter chapter, bool force = true) {
            if (chapter != null && chapter.Mined == null && force) {
                chapter.Mined = await minedRepository.GetOrDefault(chapter.Id);
            }
        }

        private async Task SaveLinks(Chapter chapter) {
            if (chapter != null && chapter.Links != null) {
                if (chapter.IsNeedToSaveLinks) {
                    try {
                        await linkRepository.Save(chapter.Id, chapter.Links);
                        chapter.IsNeedToSaveLinks = false;
                    }
                    catch (Exception e) { /*//TODO Log*/ }
                }
            }
        }

        private async Task SaveMined(Chapter chapter) {
            if (chapter != null && chapter.Mined != null) {
                if (chapter.IsNeedToSaveMined) {
                    try {
                        await minedRepository.Save(chapter.Id, chapter.Mined);
                        chapter.IsNeedToSaveMined = false;
                    }
                    catch (Exception e) { /*//TODO Log*/ }
                }
            }
        }
    }
}
