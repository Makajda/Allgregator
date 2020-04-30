using Prism.Mvvm;

namespace Allgregator.Aux.Models {
    public class Chapter : BindableBase {
        public int Id { get; set; }
        public string Spec { get; set; }

        private string name;
        public string Name {
            get => name;
            set => SetProperty(ref name, value);
        }

    }
}
