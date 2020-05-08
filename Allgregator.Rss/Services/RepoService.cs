using Allgregator.Rss.Common;
using Allgregator.Rss.Models;
using Allgregator.Rss.Repositories;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Allgregator.Rss.Services {
    internal class RepoService {
        private readonly LinkedRepository linkedRepository;
        private readonly MinedRepository minedRepository;
        public RepoService(
            LinkedRepository linkedRepository,
            MinedRepository minedRepository
            ) {
            this.linkedRepository = linkedRepository;
            this.minedRepository = minedRepository;
        }

        internal async Task Load(Data data) {
            await LoadMined(data);
            await LoadLinks(data);
        }

        internal async Task Save(Data data) {
            await SaveLinks(data);
            await SaveMined(data);
        }

        internal async Task LinkMoved(Data data, (int Id, Link Link) obj) {
            if (obj.Id == data.Id) {
                await LoadLinks(data);
                if (data.Linked.Links == null) {
                    data.Linked.Links = new ObservableCollection<Link>();
                }

                data.Linked.Links.Add(obj.Link);
                await SaveLinks(data);
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

        internal async Task LoadLinks(Data data) {
            if (data.Linked == null) {
                data.Linked = await linkedRepository.GetOrDefault(data.Id);
                if (data.Linked.CurrentState == LinksStates.Detection) {
                    data.Linked.CurrentState = LinksStates.Normal;
                }
            }
        }

        private async Task LoadMined(Data data) {
            if (data.Mined == null) {
                data.Mined = await minedRepository.GetOrDefault(data.Id);
            }
        }

        private async Task SaveLinks(Data data) {
            if (data.Linked?.Links != null) {
                if (data.Linked.IsNeedToSave) {
                    try {
                        await linkedRepository.Save(data.Id, data.Linked);
                        data.Linked.IsNeedToSave = false;
                    }
                    catch (Exception e) {
                        Serilog.Log.Error(e, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    }
                }
            }
        }

        private async Task SaveMined(Data data) {
            if (data.Mined != null) {
                if (data.Mined.IsNeedToSave) {
                    try {
                        await minedRepository.Save(data.Id, data.Mined);
                        data.Mined.IsNeedToSave = false;
                    }
                    catch (Exception e) {
                        Serilog.Log.Error(e, System.Reflection.MethodBase.GetCurrentMethod().Name);
                    }
                }
            }
        }
    }
}
