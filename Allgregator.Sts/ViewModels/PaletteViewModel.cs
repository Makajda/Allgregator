using Allgregator.Aux.Models;
using Allgregator.Aux.ViewModels;
using Allgregator.Sts.Model;
using Prism.Commands;
using System;
using System.Windows;
using System.Windows.Media;

namespace Allgregator.Sts.ViewModels {
    public class PaletteViewModel : DataViewModelBase<DataBase<MinedBase<PaletteColor>>> {
        public PaletteViewModel(Settings settings) {
            R = settings.StsPaletteR = 99;
            G = settings.StsPaletteG = 99;
            B = settings.StsPaletteB = 99;
            CopyCommand = new DelegateCommand(Copy);
        }

        public DelegateCommand CopyCommand { get; private set; }

        private string result;
        public string Result {
            get => result;
            set => SetProperty(ref result, value);
        }

        private Brush resultBrush;
        public Brush ResultBrush {
            get => resultBrush;
            set => SetProperty(ref resultBrush, value);
        }

        private byte r;
        public byte R {
            get => r;
            set => SetProperty(ref r, value, SetResult);
        }

        private byte g;
        public byte G {
            get => g;
            set => SetProperty(ref g, value, SetResult);
        }

        private byte b;
        public byte B {
            get => b;
            set => SetProperty(ref b, value, SetResult);
        }

        private void SetResult() {
            Result = $"{R:X}{G:X}{B:X}";
            ResultBrush = new SolidColorBrush(Color.FromRgb(R, G, B));//todo
        }

        private void Copy() {
            try {
                Clipboard.SetText(Result);
            }
            catch (Exception e) {
                Serilog.Log.Error(e, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }
        }
    }
}
