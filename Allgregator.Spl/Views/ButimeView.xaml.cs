using Allgregator.Spl.Models;
using System.Windows;
using System.Windows.Controls;

namespace Allgregator.Spl.Views {
    public partial class ButimeView : UserControl {
        public ButimeView() {
            InitializeComponent();
        }

        private void OpenButton_Click(object sender, RoutedEventArgs e) {
            popupOpen.IsOpen = true;
            listOpen.ItemsSource = null;
            listOpen.ItemsSource = ((sender as FrameworkElement)?.DataContext as Butask)?.Butimes;
        }
    }
}
