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

        internal async Task Retrieve(Data data) {
            var mined = data.Mined;
            var cured = data.Cured;
            if (mined == null || cured == null || cured.Currencies == null) {
                return;
            }

            var startDate = cured.StartDate;
            var date = startDate.Date;
            var toDate = DateTimeOffset.Now.Date.AddDays(1);
            var dates = new List<DateTimeOffset>();

            if (mined.Terms != null) {
                var firstTerm = mined.Terms.FirstOrDefault();
                if (firstTerm != null && firstTerm.Date < date) {
                    date = firstTerm.Date.Date;
                }

                var lastTerm = mined.Terms.LastOrDefault();
                if (lastTerm != null) {
                    dates.Add(lastTerm.Date.Date);
                }

                foreach (var term in mined.Terms) {
                    if (date <= toDate && date < term.Date.Date) {
                        do {
                            dates.Add(date);
                            date = date.AddDays(1);
                        } while (date <= toDate && date < term.Date.Date);
                    }
                    else {
                        date = date.AddDays(1);
                    }
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
                retrieveService.SetCurrencies(cured.Currencies);
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