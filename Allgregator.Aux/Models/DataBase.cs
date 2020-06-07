using Prism.Mvvm;

namespace Allgregator.Aux.Models {
    public class DataBase<TMined> : BindableBase {
        private string name;
        public string Name {
            get { return name; }
            set { SetProperty(ref name, value); }
        }

        private TMined mined;
        public TMined Mined {
            get => mined;
            set => SetProperty(ref mined, value);
        }
    }
}
