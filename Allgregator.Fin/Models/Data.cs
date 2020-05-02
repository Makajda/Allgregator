using Prism.Mvvm;

namespace Allgregator.Fin.Models {
    public class Data : BindableBase {
        private Mined mined;
        public Mined Mined {
            get { return mined; }
            set { SetProperty(ref mined, value); }
        }
    }
}
