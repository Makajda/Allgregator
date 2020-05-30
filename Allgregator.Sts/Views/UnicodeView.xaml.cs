using Allgregator.Aux.Models;
using Allgregator.Sts.Model;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Allgregator.Sts.Views {
    /// <summary>
    /// Interaction logic for UnicodeView
    /// </summary>
    public partial class UnicodeView : UserControl {
        public UnicodeView(Settings settings) {
            InitializeComponent();

            this.settings = settings;
            areasView.SelectedIndex = settings.StsAreasIndex;
            symbolsView.SelectedIndex = settings.StsSymbolsIndex;
        }

        private readonly Settings settings;

        private void AreasView_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            symbolsView.Items.Clear();
            var areaObject = e.AddedItems.Count > 0 ? e.AddedItems[0] : null;
            if (areaObject is Area area && area != null) {
                foreach (var range in area.Ranges) {
                    for (var i = range.First; i <= range.Second; i++) {
                        symbolsView.Items.Add((char)i);
                    }
                }
            }

            settings.StsAreasIndex = areasView.SelectedIndex;
        }

        private void SymbolsView_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            var symbolObject = e.AddedItems.Count > 0 ? e.AddedItems[0] : null;
            if (symbolObject is char symbol) {
                result.Content = ((int)symbol).ToString("X");
            }
            else {
                result.Content = null;
            }

            settings.StsSymbolsIndex = symbolsView.SelectedIndex;
        }

        private void Result_Click(object sender, RoutedEventArgs e) {
            try {
                var text = result.Content == null ? string.Empty : result.Content.ToString();
                Clipboard.SetText(text);
            }
            catch (Exception) { }
        }
    }
}
