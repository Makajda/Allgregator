using Prism.Mvvm;

namespace Allgregator {
    public class MainWindowViewModel : BindableBase {
        private string title = "Получение информации";
        public string Title {
            get { return title; }
            set { SetProperty(ref title, value); }
        }
    }
}
