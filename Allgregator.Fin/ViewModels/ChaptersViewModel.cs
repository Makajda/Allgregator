using Allgregator.Fin.Services;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Allgregator.Fin.ViewModels {
    internal class ChaptersViewModel : BindableBase {
        public ChaptersViewModel(
            OreService oreService
            ) {
            OreService = oreService;
        }
        public OreService OreService { get; private set; }

        private bool isActive;
        public bool IsActive {
            get => isActive;
            set => SetProperty(ref isActive, value);
        }
    }
}
