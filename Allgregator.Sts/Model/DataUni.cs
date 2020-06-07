using Prism.Mvvm;

namespace Allgregator.Sts.Model {
    public class DataUni : BindableBase {
        private MinedUni mined;
        public MinedUni Mined {
            get { return mined; }
            set { SetProperty(ref mined, value); }
        }
    }
}
