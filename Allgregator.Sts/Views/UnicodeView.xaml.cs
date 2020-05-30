using Allgregator.Sts.Model;
using Allgregator.Sts.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Threading;

namespace Allgregator.Sts.Views {
    /// <summary>
    /// Interaction logic for UnicodeView
    /// </summary>
    public partial class UnicodeView : UserControl {
        public UnicodeView() {
            InitializeComponent();

            cvs = Resources["cvs"] as CollectionViewSource;
        }

        private CollectionViewSource cvs;

        private void CollectionViewSource_Filter(object sender, FilterEventArgs e) {
            if (!string.IsNullOrWhiteSpace(filter.Text) && e.Item is Symbol item) {
                if (item.Name.Contains(filter.Text)) {
                    e.Accepted = true;
                }
                else {
                    e.Accepted = false;
                }
            }
        }

        private void ButtonFilter_Click(object sender, RoutedEventArgs e) {
            //await Task.Run(() => Dispatcher.Invoke(() => cvs.View.Refresh(), DispatcherPriority.Normal));
            var vm = DataContext as UnicodeViewModel;
            //var area = vm.Data.Mined.Areas.FirstOrDefault();
            var ranges = vm.Data.Mined.Areas.SelectMany(n => n.Ranges);
            //if (area != null && area.Ranges.Count > 0) {
            //var range = area.Ranges[0];
            var symbols = new List<Symbol>();
            var sym = new List<int>();
            foreach (var range in ranges) {
                for (var i = range.Begin; i <= range.End; i++) {
                    if (!sym.Contains(i)) {
                        sym.Add(i);
                        //    var name = i.ToString();
                        //    if (!symbols.Any(n => n.Name == name)) {
                        //        symbols.Add(new Symbol() { Char = (char)i, Name = name });
                        //    }
                    }
                }

                s.ItemsSource = symbols;
                //}

            }
        }
    }
}
