using Allgregator.Aux.Services;
using Allgregator.Rss.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Allgregator.Rss.Services {
    internal class OreService : OreServiceBase {
        private readonly RetrieveService retrieveService;
        public OreService(
            RetrieveService retrieveService
            ) {
            this.retrieveService = retrieveService;
        }

        internal async Task Retrieve(Chapter chapter, DateTimeOffset cutoffTime) {
            if (chapter?.Linked?.Links == null || chapter.Mined == null) {
                return;
            }

            using (retrieveService) {
                retrieveService.CutoffTime = cutoffTime;
                var lastRetrieve = await Retrieve(chapter.Linked.Links, retrieveService.Production);

                if (IsRetrieving) {
                    var mined = chapter.Mined;
                    var newRecos = new List<Reco>();
                    var oldRecos = new List<Reco>();
                    var outdated = mined.OldRecos?.Where(n => n.PublishDate >= chapter.Mined.AcceptTime);

                    foreach (var reco in retrieveService.Items) {
                        if (reco.PublishDate > mined.AcceptTime) {
                            if (outdated?.FirstOrDefault(n => n.Equals(reco)) == null) {
                                newRecos.Add(reco);
                                continue;
                            }
                        }

                        oldRecos.Add(reco);
                    }

                    mined.NewRecos = new ObservableCollection<Reco>(newRecos.OrderByDescending(n => n.PublishDate));
                    mined.OldRecos = new ObservableCollection<Reco>(oldRecos.OrderByDescending(n => n.PublishDate));
                    mined.Errors = retrieveService.Errors.Count == 0 ? null : retrieveService.Errors.ToList();//cached;
                    mined.LastRetrieve = lastRetrieve;
                    mined.IsNeedToSave = true;
                    IsRetrieving = false;
                }
            }
        }
    }
}
