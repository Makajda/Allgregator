using Allgregator.Aux.Common;
using Prism.Mvvm;
using Prism.Regions;

namespace Allgregator.Aux.ViewModels {
    public class DataViewModelBase<TData> : BindableBase, INavigationAware {
        private TData data;
        public TData Data {
            get => data;
            set => SetProperty(ref data, value);
        }

        public void OnNavigatedTo(NavigationContext navigationContext) {
            if (navigationContext.Parameters.TryGetValue(Given.DataParameter, out TData data)) {
                Data = data;
            }
        }
        public bool IsNavigationTarget(NavigationContext navigationContext) {
            return true;
        }
        public void OnNavigatedFrom(NavigationContext navigationContext) { }
    }
}
