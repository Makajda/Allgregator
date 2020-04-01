using Prism.Mvvm;

namespace Allgregator.ViewModels {
    public class MainWindowViewModel : BindableBase {

        public MainWindowViewModel() {
            Title = "Работает";
        }

        private string title;
        public string Title {
            get { return title; }
            set { SetProperty(ref title, value); }
        }
    }
}
