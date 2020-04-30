using Allgregator.Aux.Models;
using System.Text.Json.Serialization;

namespace Allgregator.Fin.Models {
    public class Chapter : ChapterBase {
        private Mined mined;
        [JsonIgnore]
        public Mined Mined {
            get { return mined; }
            set { SetProperty(ref mined, value); }
        }
    }
}
