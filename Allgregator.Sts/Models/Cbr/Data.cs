using Allgregator.Aux.Models;

namespace Allgregator.Sts.Cbr.Models {
    public class Data : DataBase<Mined> {
        private Cured cured;
        public Cured Cured {
            get { return cured; }
            set { SetProperty(ref cured, value); }
        }
    }
}
