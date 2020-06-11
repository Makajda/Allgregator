using Allgregator.Aux.Common;
using Allgregator.Aux.Models;
using Allgregator.Aux.ViewModels;
using Allgregator.Sts.Model;
using Prism.Commands;
using Prism.Events;
using System;
using System.ComponentModel;
using System.Windows;

namespace Allgregator.Sts.ViewModels {
    public class PaletteViewModel : DataViewModelBase<DataBase<MinedBase<PaletteColor>>> {
        private readonly Settings settings;
        public PaletteViewModel(Settings settings, IEventAggregator eventAggregator) {
            this.settings = settings;
            R = settings.StsPaletteR;
            G = settings.StsPaletteG;
            B = settings.StsPaletteB;
            CopyCommand = new DelegateCommand(Copy);
            eventAggregator.GetEvent<WindowClosingEvent>().Subscribe(WindowClosing);
        }

        public DelegateCommand CopyCommand { get; private set; }

        private PaletteColor selectedColor;
        public PaletteColor SelectedColor {
            get => selectedColor;
            set => SetProperty(ref selectedColor, value, () => { R = selectedColor.R; G = selectedColor.G; B = selectedColor.B; });
        }

        private byte r;
        public byte R {
            get => r;
            set => SetProperty(ref r, value);
        }

        private byte g;
        public byte G {
            get => g;
            set => SetProperty(ref g, value);
        }

        private byte b;
        public byte B {
            get => b;
            set => SetProperty(ref b, value);
        }

        private void Copy() {
            try {
                Clipboard.SetText($"{R:X2}{G:X2}{B:X2}");
            }
            catch (Exception e) {
                Serilog.Log.Error(e, System.Reflection.MethodBase.GetCurrentMethod().Name);
            }
        }

        private void WindowClosing(CancelEventArgs obj) {
            settings.StsPaletteR = R;
            settings.StsPaletteG = G;
            settings.StsPaletteB = B;
        }
    }
}
