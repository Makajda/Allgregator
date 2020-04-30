﻿using Allgregator.Aux.Common;
using Allgregator.Rss.Models;
using Prism.Commands;
using Prism.Mvvm;

namespace Allgregator.Rss.ViewModels {
    internal class RecosViewModel : BindableBase {
        public RecosViewModel(
            Data data
            ) {
            Data = data;

            OpenCommand = new DelegateCommand<Reco>(Open);
            MoveCommand = new DelegateCommand<Reco>(Move);
        }

        public DelegateCommand<Reco> OpenCommand { get; private set; }
        public DelegateCommand<Reco> MoveCommand { get; private set; }
        public Data Data { get; private set; }

        private void Open(Reco reco) {
            WindowUtilities.Run(reco.Uri);
            Move(reco);
        }

        private void Move(Reco reco) {
            if (Data.Mined != null) {
                Data.Mined.NewRecos.Remove(reco);
                Data.Mined.OldRecos.Insert(0, reco);
                if (Data.Mined.NewRecos.Count == 0) {
                    Data.Mined.AcceptTime = Data.Mined.LastRetrieve;
                }

                Data.Mined.IsNeedToSave = true;
            }
        }
    }
}

