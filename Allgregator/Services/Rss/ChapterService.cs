using Allgregator.Common;
using Allgregator.Models.Rss;
using Allgregator.Repositories.Rss;
using System;
using System.Threading.Tasks;

namespace Allgregator.Services.Rss {
    public class ChapterService {
        private readonly LinkedRepository linkedRepository;
        private readonly MinedRepository minedRepository;
        public ChapterService(
            LinkedRepository linkedRepository,
            MinedRepository minedRepository
            ) {
            this.linkedRepository = linkedRepository;
            this.minedRepository = minedRepository;
        }

        public async Task Load(Chapter chapter, RssChapterViews currentView) {
            await LoadMined(chapter);
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
                chapter.Linked.Links.Add(obj.Link);
                await SaveLinks(chapter);
            }
        }

        private async Task LoadLinks(Chapter chapter, bool force = true) {
            if (chapter != null && chapter.Linked == null && force) {
                chapter.Linked = await linkedRepository.GetOrDefault(chapter.Id);
                if (chapter.Linked.CurrentState == RssLinksStates.Detection || chapter.Linked.CurrentState == RssLinksStates.Chapter) {
                    chapter.Linked.CurrentState = RssLinksStates.Normal;
                }
            }
        }

        private async Task LoadMined(Chapter chapter) {
            if (chapter != null && chapter.Mined == null) {
                chapter.Mined = await minedRepository.GetOrDefault(chapter.Id);
            }
        }

        private async Task SaveLinks(Chapter chapter) {
            if (chapter?.Linked?.Links != null) {
                if (chapter.Linked.IsNeedToSave) {
                    try {
                        await linkedRepository.Save(chapter.Id, chapter.Linked);
                        chapter.Linked.IsNeedToSave = false;
                    }
                    catch (Exception e) { /*//TODO Log*/ }
                }
            }
        }

        private async Task SaveMined(Chapter chapter) {
            if (chapter?.Mined != null) {
                if (chapter.Mined.IsNeedToSave) {
                    try {
                        await minedRepository.Save(chapter.Id, chapter.Mined);
                        chapter.Mined.IsNeedToSave = false;
                    }
                    catch (Exception e) { /*//TODO Log*/ }
                }
            }
        }
    }
}
