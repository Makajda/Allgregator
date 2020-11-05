using Allgregator.Aux.Repositories;
using Allgregator.Rss.Common;
using Allgregator.Rss.Models;
using Allgregator.Rss.Repositories;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Allgregator.Rss.Services {
    internal class RepoService {
        private readonly LinkedRepository linkedRepository;
        private readonly RepositoryBase<Mined> minedRepository;
        public RepoService(
            LinkedRepository linkedRepository,
            ZipRepositoryBase<Mined> minedRepository
            ) {
            this.linkedRepository = linkedRepository;
            this.minedRepository = minedRepository;
            minedRepository.SetNames(Module.Name);
        }

        internal async Task Load(Data data) {
            await LoadMined(data);
            await LoadLinks(data);
        }

        internal async Task Save(Data data) {
            await SaveLinks(data);
            await SaveMined(data);
        }

        internal async Task LinkMoved(Data data, MoveRecord moveRecord) {
            if (moveRecord.Id == data.Id) {
                await LoadLinks(data);
                if (data.Linked.Links == null) {
                    data.Linked.Links = new ObservableCollection<Link>();
                }

                data.Linked.Links.Add(moveRecord.Link);
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
                try {
                    await linkedRepository.Save(data.Linked, data.Id);
                }
                catch (Exception e) {
                    Serilog.Log.Error(e, System.Reflection.MethodBase.GetCurrentMethod().Name);
                }
            }
        }

        private async Task SaveMined(Data data) {
            if (data.Mined != null) {
                try {
                    await minedRepository.Save(data.Mined, data.Id);
                }
                catch (Exception e) {
                    Serilog.Log.Error(e, System.Reflection.MethodBase.GetCurrentMethod().Name);
                }
            }
        }
    }
}
