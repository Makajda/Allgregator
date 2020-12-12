using System.Windows;

namespace Allgregator
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void TopmostButton_Click(object sender, RoutedEventArgs e)
        {
            Topmost = !Topmost;
        }
    }
}
