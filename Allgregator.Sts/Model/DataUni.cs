using Prism.Mvvm;

namespace Allgregator.Sts.Model {
    public class DataUni : BindableBase {
        private MinedUni mined;
        public MinedUni Data {
            get => mined;
            set => SetProperty(ref mined, value);
        }
    }
}
