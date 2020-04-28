using Allgregator.Aux.Common;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;

namespace Allgregator {
    public class MainWindowViewModel : BindableBase {
        public MainWindowViewModel(
            IEventAggregator eventAggregator
            ) {

            //todo temp here
            SettingsCommand = new DelegateCommand(() => eventAggregator.GetEvent<CurrentChapterChangedEvent>().Publish(Given.SettingsChapter));
        }

        public DelegateCommand SettingsCommand { get; private set; }

        private string title = "Получение информации";
        public string Title {
            get { return title; }
            set { SetProperty(ref title, value); }
        }
    }
}
