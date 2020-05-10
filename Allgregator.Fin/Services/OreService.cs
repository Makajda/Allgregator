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

        internal async Task Retrieve(Mined mined, DateTimeOffset startDate, IEnumerable<string> currencies) {
            if (mined == null || currencies == null) {
                return;
            }

            // dates - недостающие даты
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
                retrieveService.SetCurrencies(currencies);
                var lastRetrieve = await Retrieve(dates, retrieveService.ProductionAsync);

                if (IsRetrieving) {
                    if (mined.Terms == null) {
                        mined.Terms = retrieveService.Items.OrderBy(n => n.Date).ToList();
                    }
                    else {
                        foreach (var item in retrieveService.Items) {
                            var term = mined.Terms.FirstOrDefault(n => n.Date == item.Date);
                            if (term == null) {
                                mined.Terms.Add(item);
                            }
                            else {
                                term.Values = item.Values;
                            }

                            mined.Terms = mined.Terms.OrderBy(n => n.Date).ToList();
                        }
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