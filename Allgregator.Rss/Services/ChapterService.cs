using Allgregator.Repositories.Rss;
using Allgregator.Rss.Common;
using Allgregator.Rss.Models;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Allgregator.Rss.Services {
    internal class ChapterService {
        private readonly LinkedRepository linkedRepository;
        private readonly MinedRepository minedRepository;
        public ChapterService(
            LinkedRepository linkedRepository,
            MinedRepository minedRepository
            ) {
            this.linkedRepository = linkedRepository;
            this.minedRepository = minedRepository;
        }

        internal async Task Load(Chapter chapter, bool force = true) {
            await LoadMined(chapter);
            await LoadLinks(chapter, force);
        }

        internal async Task Save(Chapter chapter) {
            await SaveLinks(chapter);
            await SaveMined(chapter);
        }

        internal async Task LinkMoved(Chapter chapter, (int Id, Link Link) obj) {
            if (obj.Id == chapter.Id) {
                await LoadLinks(chapter);
                if (chapter.Linked.Links == null) {
                    chapter.Linked.Links = new ObservableCollection<Link>();
                }

                chapter.Linked.Links.Add(obj.Link);
                await SaveLinks(chapter);
            }
        }

        internal void DeleteFiles(int id) {
            try {
                linkedRepository.DeleteFile(id);
            }
            catch (Exception e) {
                Serilog.Log.Error(e, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }

            try {
                minedRepository.DeleteFile(id);
            }
            catch (Exception e) {
                Serilog.Log.Error(e, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }
        }

        private async Task LoadLinks(Chapter chapter, bool force = true) {
            if (chapter != null && chapter.Linked == null && force) {
                chapter.Linked = await linkedRepository.GetOrDefault(chapter.Id);
                if (chapter.Linked.CurrentState == LinksStates.Detection || chapter.Linked.CurrentState == LinksStates.Chapter) {
                    chapter.Linked.CurrentState = LinksStates.Normal;
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
                    catch (Exception e) {
                        Serilog.Log.Error(e, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    }
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
                    catch (Exception e) {
                        Serilog.Log.Error(e, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    }
                }
            }
        }
    }
}
