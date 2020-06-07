using Allgregator.Aux.Repositories;
using Allgregator.Fin.Models;

namespace Allgregator.Fin.Repositories {
    public class CuredRepository : RepositoryBase<Cured> {
        public CuredRepository() : base(Module.Name) { }
        protected override Cured CreateDefault(int id = 0) {
            return new Cured() {
                Currencies = new[] {
                    "USD",
                    "EUR",
                    "GBP",
                    "CHF",
                    "CNY",
                    "UAH"
                }
            };
        }
    }
}
