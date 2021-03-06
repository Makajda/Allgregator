﻿using Prism.Mvvm;
using System.Text.Json.Serialization;

namespace Allgregator.Aux.Models {
    public class DataBase<TMined> : BindableBase {
        private string title;
        public string Title {
            get { return title; }
            set { SetProperty(ref title, value); }
        }

        private TMined mined;
        public TMined Mined {
            get => mined;
            set => SetProperty(ref mined, value);
        }

        private bool isSettings;
        [JsonIgnore]
        public bool IsSettings {
            get { return isSettings; }
            set { SetProperty(ref isSettings, value); }
        }
    }
}
