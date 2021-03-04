using Allgregator.Aux.Common;
using Allgregator.Aux.Models;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Windows;
using System.Windows.Input;

namespace Allgregator {
    public class MainWindowViewModel : BindableBase {
        private readonly IRegionManager regionManager;
        public MainWindowViewModel(
            IRegionManager regionManager
            ) {
            this.regionManager = regionManager;
        }

        private DelegateCommand loadedCommand; public ICommand LoadedCommand => loadedCommand ??= new DelegateCommand(Start);

        private string title = "Получение информации";
        public string Title {
            get { return title; }
            set { SetProperty(ref title, value); }
        }

        private void Start() {
            regionManager.Regions[Given.MenuRegion].SortComparison = new Comparison<object>(ComparisonReal);
        }

        private int ComparisonReal(object x, object y) {
            if (x is FrameworkElement xe && y is FrameworkElement ye && xe.DataContext is IComparison xc && ye.DataContext is IComparison yc) {
                if (xc.Weight < yc.Weight) return -1;
                if (xc.Weight > yc.Weight) return 1;
            }

            return 0;
        }
    }
}
