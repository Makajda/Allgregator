using Allgregator.Sts.Model;
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

        private async void ButtonFilter_Click(object sender, RoutedEventArgs e) {
            await Task.Run(() => Dispatcher.Invoke(() => cvs.View.Refresh(), DispatcherPriority.Normal));
        }
    }
}
