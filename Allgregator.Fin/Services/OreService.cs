using Allgregator.Aux.Services;
using Allgregator.Fin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Allgregator.Fin.Services {
    internal class OreService : OreServiceBase {
        private readonly RetrieveService retrieveService;
        public OreService(
            RetrieveService retrieveService
            ) {
            this.retrieveService = retrieveService;
        }

        internal async Task Retrieve(Mined mined, DateTimeOffset startDate) {
            if (mined == null || mined.Currencies == null) {
                return;
            }

            // dates - недостающие даты на основании имеющихся mined.Currensies
            var date = startDate.Date;
            var toDate = DateTimeOffset.Now.Date;
            var dates = new List<DateTimeOffset>();

            if (mined.Terms != null) {
                foreach (var term in mined.Terms) {
                    while (date <= toDate && date < term.Date) {
                        dates.Add(date);
                        date = date.AddDays(1);
                    }

                    date = date.AddDays(1);
                }
            }

            while (date <= toDate) {
                dates.Add(date);
                date = date.AddDays(1);
            }

            if (dates.Count == 0) {
                dates.Add(toDate);
            }

            using (retrieveService) {
                retrieveService.SetNames(mined.Currencies.Where(n => n.IsOn).Select(n => n.Key));
                var lastRetrieve = await Retrieve(dates, retrieveService.ProductionAsync);

                if (IsRetrieving) {
                    if (mined.Terms == null) {
                        mined.Terms = retrieveService.Items.OrderBy(n => n.Date).ToList();
                    }
                    else {
                        mined.Terms = mined.Terms.Union(retrieveService.Items).OrderBy(n => n.Date).ToList();
                    }
                    mined.Errors = retrieveService.Errors.Count == 0 ? null : retrieveService.Errors.ToList();//cached;
                    mined.LastRetrieve = lastRetrieve;
                    mined.IsNeedToSave = true;
                    IsRetrieving = false;
                }
            }
        }
    }
}