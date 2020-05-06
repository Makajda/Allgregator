using Prism.Mvvm;

namespace Allgregator.Fin.Models {
    public class Currency : BindableBase {
        public string Key { get; set; }

        private bool isOn = true;
        public bool IsOn {
            get { return isOn; }
            set { SetProperty(ref isOn, value); }
        }
    }
}
