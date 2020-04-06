﻿using Allgregator.Common;
using Allgregator.Models.Rss;
using Allgregator.Repositories.Rss;
using Prism;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.ComponentModel;

namespace Allgregator.ViewModels.Rss {
    public class RecosViewModel : BindableBase, IActiveAware {
        private readonly MinedRepository minedRepository;

        public RecosViewModel(
            MinedRepository minedRepository,
            IEventAggregator eventAggregator
            ) {
            this.minedRepository = minedRepository;
            OpenCommand = new DelegateCommand<Reco>((reco) => WindowUtilities.Run(reco.Uri.ToString()));
            MoveCommand = new DelegateCommand<Reco>(Move);

            eventAggregator.GetEvent<ChapterChangedEvent>().Subscribe((chapter) => Chapter = chapter);
            eventAggregator.GetEvent<WindowClosingEvent>().Subscribe(SaveMined);
        }

        public event EventHandler IsActiveChanged;
        public DelegateCommand<Reco> OpenCommand { get; private set; }
        public DelegateCommand<Reco> MoveCommand { get; private set; }

        private Chapter chapter;
        public Chapter Chapter {
            get => chapter;
            private set {
                SaveMined();
                SetProperty(ref chapter, value);
                LoadMined();
            }
        }

        private bool isActive;
        public bool IsActive {
            get => isActive;
            set => SetProperty(ref isActive, value, OnIsActiveChanged);
        }

        private void OnIsActiveChanged() {
            IsActiveChanged?.Invoke(this, EventArgs.Empty);
            LoadMined();
        }

        private void Move(Reco reco) {
            Chapter.Mined.NewRecos.Remove(reco);
            Chapter.Mined.OldRecos.Insert(0, reco);
            Chapter.Mined.IsNeedToSave = true;
        }

        private void LoadMined() {
            if (IsActive) {
                if (Chapter != null) {
                    if (Chapter.Mined == null) Chapter.Mined = minedRepository.Get(Chapter.Id);
                }
            }
        }

        private void SaveMined(CancelEventArgs cancelEventArgs = null) {
            if (Chapter != null) {
                if (Chapter.Mined != null) {
                    var mined = Chapter.Mined;
                    if (mined.IsNeedToSave) {
                        if (mined.NewRecos != null && mined.NewRecos.Count == 0) {
                            mined.AcceptTime = mined.LastRetrieve;
                        }

                        minedRepository.Save(Chapter.Id, mined);
                        Chapter.Mined.IsNeedToSave = false;
                    }
                }
            }
        }
    }
}
