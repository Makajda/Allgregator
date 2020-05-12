using Prism.Mvvm;

namespace Allgregator.Fin.Models {
    public class Data : BindableBase {
        private Mined mined;
        public Mined Mined {
            get { return mined; }
            set { SetProperty(ref mined, value); }
        }

        private Cured cured;
        public Cured Cured {
            get { return cured; }
            set { SetProperty(ref cured, value); }
        }
    }
}
