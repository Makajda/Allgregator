using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace Allgregator.Aux.Common {
    public interface ISelected {
        bool IsSelected { get; set; }
    }
    public class Obsefy<T> : ObservableCollection<T> {
        public Obsefy() {
            Init();
        }

        public Obsefy(IEnumerable<T> collection) : base(collection) {
            Countfy = collection.Count();
            Init();
        }

        public Obsefy(List<T> list) : base(list) {
            Countfy = list.Count;
            Init();
        }

        private void Init() {
            CollectionChanged += (s, e) => Countfy = Count;
        }

        private T selected;
        public T Selected {
            get => selected;
            set {
                if (!EqualityComparer<T>.Default.Equals(selected, value) && selected is ISelected iSelected) {
                    if (selected != null) iSelected.IsSelected = false;
                    selected = value;
                    if (selected != null) iSelected.IsSelected = true;
                    OnPropertyChanged(new PropertyChangedEventArgs(nameof(Selected)));
                }
            }
        }

        private int countfy;
        public int Countfy {
            get => countfy;
            set {
                if (countfy != value) {
                    countfy = value;
                    OnPropertyChanged(new PropertyChangedEventArgs(nameof(Countfy)));
                }
            }
        }
    }
}
