using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Allgregator.Aux.ViewModels {
    public class SimpleChapterViewModelBase : ChapterViewModelBase {
        public SimpleChapterViewModelBase() : base(null, null) {

        }
        protected override Task Activate() {
            throw new NotImplementedException();
        }

        protected override Task Deactivate() {
            throw new NotImplementedException();
        }

        protected override Task Update() {
            throw new NotImplementedException();
        }
    }
}
