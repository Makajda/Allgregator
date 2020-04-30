using Prism.Mvvm;
using System.Text.Json.Serialization;

namespace Allgregator.Rss.Models {
    public class Chapter : BindableBase {//todo разделить на два класса
        public int Id { get; internal set; }

        private string name;//todo связать с ChapterBase для правки
        public string Name {
            get { return name; }
            set { SetProperty(ref name, value); }
        }

        private Linked linked;
        [JsonIgnore]
        public Linked Linked {
            get { return linked; }
            set { SetProperty(ref linked, value); }
        }

        private Mined mined;
        [JsonIgnore]
        public Mined Mined {
            get { return mined; }
            set { SetProperty(ref mined, value); }
        }
    }
}
