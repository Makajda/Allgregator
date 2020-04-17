﻿using Allgregator.Common;
using Allgregator.Models;
using Allgregator.Services.Rss;
using Allgregator.ViewModels;
using Allgregator.Views;
using Allgregator.Views.Rss;
using Prism.DryIoc;
using Prism.Ioc;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Reflection;
using System.Windows;

namespace Allgregator {
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : PrismApplication {
        protected override Window CreateShell() => WindowUtilities.GetMainWindow();

        protected override void OnInitialized() {
            base.OnInitialized();

            var regionManager = Container.Resolve<IRegionManager>();
            regionManager.RegisterViewWithRegion(Given.MenuRegion, typeof(ChaptersView));

            var region = regionManager.Regions[Given.MainRegion];
            region.Add(Container.Resolve<RecosView>((typeof(bool), true)), RssChapterViews.NewsView.ToString());
            region.Add(Container.Resolve<RecosView>((typeof(bool), false)), RssChapterViews.OldsView.ToString());
            region.Add(Container.Resolve<LinksView>(), RssChapterViews.LinksView.ToString());
            region.Add(Container.Resolve<SettingsView>(), Given.SettingsView);
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry) {
            containerRegistry.RegisterInstance<Settings>(WindowUtilities.GetSettings());
            containerRegistry.RegisterSingleton<ChapterService>();
            containerRegistry.RegisterForNavigation<MainWindow, MainWindowViewModel>();
        }

        protected override void ConfigureServiceLocator() {
            base.ConfigureServiceLocator();
            ViewModelLocationProvider.SetDefaultViewTypeToViewModelTypeResolver((viewType) => {
                var viewName = viewType.FullName.Replace(".Views.", ".ViewModels.");
                var viewAssemblyName = viewType.GetTypeInfo().Assembly.FullName;
                var viewModelName = $"{viewName}Model, {viewAssemblyName}";
                return Type.GetType(viewModelName);
            });
        }
    }
}
