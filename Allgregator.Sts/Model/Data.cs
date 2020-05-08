using Prism.Mvvm;

namespace Allgregator.Sts.Model {
    public class Data : BindableBase {
        private Mined mined;
        public Mined Mined {
            get { return mined; }
            set { SetProperty(ref mined, value); }
        }
    }
}
