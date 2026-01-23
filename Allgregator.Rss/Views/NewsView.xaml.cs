using Allgregator.Rss.Models;
using Prism.Commands;
using System.Windows;
using System.Windows.Controls;

namespace Allgregator.Rss.Views {
    /// <summary>
    /// Interaction logic for NewsView
    /// </summary>
    public partial class NewsView : UserControl {
        public NewsView() {
            InitializeComponent();
            OpenCommand = new DelegateCommand<Reco>(Open);
        }

        public DelegateCommand<Reco> OpenCommand { get; private set; }

        private async void Open(Reco reco) {
            Window window = new() {
                ShowActivated = true, ShowInTaskbar = false, Title = reco.ItemTitle,
                Width = 1400, Height = 1000, WindowStartupLocation = WindowStartupLocation.CenterScreen
            };
            Microsoft.Web.WebView2.Wpf.WebView2 browser = new();
            window.Content = browser;
            window.Show();
            await browser.EnsureCoreWebView2Async();
            browser.NavigateToString(reco.SummaryHtml);
        }
    }
}
