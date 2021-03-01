using Allgregator.Spl.Models;
using System.Windows;
using System.Windows.Controls;

namespace Allgregator.Spl.Views {
    public partial class ButimeView : UserControl {
        public ButimeView() {
            InitializeComponent();
        }

        private void OpenButton_Click(object sender, RoutedEventArgs e) {
            listOpen.ItemsSource = null;
            var source = ((sender as FrameworkElement)?.DataContext as Butask)?.Butimes;
            if (source != null && source.Count > 0) {
                popupOpen.IsOpen = true;
                listOpen.ItemsSource = source;
            }
        }
    }
}
