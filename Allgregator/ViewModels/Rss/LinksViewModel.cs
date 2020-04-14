using Allgregator.Common;
using Allgregator.Models.Rss;
using Allgregator.Services;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;

namespace Allgregator.ViewModels.Rss {
    public class LinksViewModel : BindableBase {
        DialogService dialogService;
        public LinksViewModel(
            DialogService dialogService,
            IEventAggregator eventAggregator
            ) {
            this.dialogService = dialogService;

            MoveCommand = new DelegateCommand<Link>(Move);
            DeleteCommand = new DelegateCommand<Link>(Delete);

            eventAggregator.GetEvent<ChapterChangedEvent>().Subscribe((chapter) => Chapter = chapter);
        }

        public DelegateCommand<Link> MoveCommand { get; private set; }
        public DelegateCommand<Link> DeleteCommand { get; private set; }

        private Chapter chapter;
        public Chapter Chapter {
            get => chapter;
            private set => SetProperty(ref chapter, value);
        }

        private void Move(Link link) {
            //todo
        }

        private void Delete(Link link) {
            dialogService.Show($"{link.Name}?", DeleteReal, link, 20, true);
        }

        private void DeleteReal(object olink) {
            if (olink is Link link && link != null) {
                Chapter.Links.Remove(link);
                Chapter.IsNeedToSaveLinks = true;
            }
        }
    }
}
