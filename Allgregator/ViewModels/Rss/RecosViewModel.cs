using Allgregator.Common;
using Allgregator.Models.Rss;
using Allgregator.Repositories.Rss;
using Prism;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.ComponentModel;
using System.Threading.Tasks;

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

            eventAggregator.GetEvent<ChapterChangedEvent>().Subscribe(ChangeChapter);
            eventAggregator.GetEvent<WindowClosingEvent>().Subscribe(async (cancelEventArgs) => await Save(cancelEventArgs));
            IsActiveChanged += async (s, e) => await Load();
        }

        public event EventHandler IsActiveChanged;
        public DelegateCommand<Reco> OpenCommand { get; private set; }
        public DelegateCommand<Reco> MoveCommand { get; private set; }

        private Chapter chapter;
        public Chapter Chapter {
            get => chapter;
            private set => SetProperty(ref chapter, value);
        }

        private bool isActive;
        public bool IsActive {
            get => isActive;
            set => SetProperty(ref isActive, value, () => IsActiveChanged?.Invoke(this, EventArgs.Empty));
        }

        private void Move(Reco reco) {
            var mined = Chapter.Mined;
            mined.NewRecos.Remove(reco);
            mined.OldRecos.Insert(0, reco);
            if (mined.NewRecos.Count == 0) {
                mined.AcceptTime = mined.LastRetrieve;
            }

            mined.IsNeedToSave = true;
        }

        private async void ChangeChapter(Chapter chapter) {
            await Save();
            Chapter = chapter;
            await Load();
        }

        private async Task Load() {
            if (IsActive && Chapter != null && !Chapter.IsMinedLoading && Chapter.Mined == null) {
                Chapter.IsMinedLoading = true;

                try {
                    Chapter.Mined = await minedRepository.Get(Chapter.Id);
                }
                catch (Exception e) { /*//TODO Log*/ }

                Chapter.IsMinedLoading = false;

                if (chapter.Mined == null) {
                    chapter.Mined = new Mined();
                }
            }
        }

        private async Task Save(CancelEventArgs cancelEventArgs = null) {
            if (Chapter != null && Chapter.Mined != null) {
                var mined = Chapter.Mined;
                if (mined.IsNeedToSave && !mined.IsSaving) {
                    mined.IsSaving = true;

                    try {
                        await minedRepository.Save(Chapter.Id, mined);
                        mined.IsNeedToSave = false;
                    }
                    catch (Exception e) { /*//TODO Log*/ }

                    mined.IsSaving = false;
                }
            }
        }
    }
}

