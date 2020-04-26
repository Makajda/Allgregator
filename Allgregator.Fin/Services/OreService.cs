using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Text;

namespace Allgregator.Fin.Services {
    internal class OreService : BindableBase {

        private bool isRetrieving;
        public bool IsRetrieving {
            get { return isRetrieving; }
            set { SetProperty(ref isRetrieving, value); }
        }
    }
}
