using Allgregator.Aux.Common;
using Allgregator.Aux.ViewModels;
using Allgregator.Rss.Models;
using Prism.Commands;
using System;
using System.Windows;

namespace Allgregator.Rss.ViewModels {
    public class NewsViewModel : DataViewModelBase<Data> {
        public NewsViewModel() {
            OpenCommand = new DelegateCommand<Reco>(Open);
            MoveCommand = new DelegateCommand<Reco>(Move);
        }

        public DelegateCommand<Reco> OpenCommand { get; private set; }
        public DelegateCommand<Reco> MoveCommand { get; private set; }

        private void Open(Reco reco) {
            WindowUtilities.Run(reco.Uri);
            Move(reco);
        }

        private async void Move(Reco reco) {
            try {
                Window window = new() {
                    ShowActivated = true, ShowInTaskbar = false, Title = reco.ItemTitle + reco.Uri,
                    Width = 1400, Height = 1200, WindowStartupLocation = WindowStartupLocation.CenterScreen
                };
                Microsoft.Web.WebView2.Wpf.WebView2 browser = new();
                window.Content = browser;
                window.Show();
                await browser.EnsureCoreWebView2Async();
                browser.NavigateToString(reco.SummaryHtml);
                browser.ZoomFactor = 2.1;
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message);
            }

            //todo after sanction
            //if (Data.Mined != null && Data.Mined.NewRecos != null && Data.Mined.OldRecos != null) {
            //    Data.Mined.NewRecos.Remove(reco);
            //    Data.Mined.OldRecos.Insert(0, reco);
            //    if (Data.Mined.NewRecos.Count == 0) {
            //        Data.Mined.AcceptTime = Data.Mined.LastRetrieve;
            //    }

            //    Data.Mined.IsNeedToSave = true;
            //}
        }
    }
}

