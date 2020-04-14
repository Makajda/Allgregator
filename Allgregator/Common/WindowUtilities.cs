using Allgregator.Models;
using Allgregator.Repositories.Rss;
using Allgregator.Views;
using DryIoc;
using Prism.DryIoc;
using Prism.Events;
using Prism.Ioc;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;

namespace Allgregator.Common {
    public static class WindowUtilities {
        public static Settings GetSettings() {
            var container = (App.Current as PrismApplication).Container;
            var settingsRepository = container.Resolve<SettingsRepository>();
            var settings = settingsRepository.Get();
            return settings;
        }

        public static MainWindow GetMainWindow() {
            var container = (App.Current as PrismApplication).Container;
            var window = container.Resolve<MainWindow>();
            var settings = container.Resolve<Settings>();
            SetWindowBoundsAndState(window, settings.MainWindowBounds, settings.MainWindowState);
            window.Closing += Window_Closing;
            return window;
        }

        private static void Window_Closing(object sender, CancelEventArgs e) {
            if (sender is MainWindow mainWindow) {
                var container = (App.Current as PrismApplication).Container;
                var eventAggregator = container.Resolve<IEventAggregator>();
                eventAggregator.GetEvent<WindowClosingEvent>().Publish(e);
                var settings = container.Resolve<Settings>();
                var settingsRepository = container.Resolve<SettingsRepository>();
                settings.MainWindowBounds = mainWindow.RestoreBounds;
                settings.MainWindowState = mainWindow.WindowState;
                try {
                    settingsRepository.Save(settings);
                }
                catch (Exception ex) { /*//TODO Log*/ }
            }
        }

        private static void SetWindowBoundsAndState(Window window, Rect bounds, WindowState state) {
            window.Left = double.IsInfinity(bounds.Left) ? 0d : bounds.Left;
            window.Top = double.IsInfinity(bounds.Top) ? 0d : bounds.Top;
            if (bounds.Width > double.Epsilon && !double.IsInfinity(bounds.Width))
                window.Width = bounds.Width;
            if (bounds.Height > double.Epsilon && !double.IsInfinity(bounds.Height))
                window.Height = bounds.Height;
            if (window.Left < 0d || window.Top < 0d
                || window.Width < double.Epsilon
                || window.Height < double.Epsilon
                || window.Width > SystemParameters.VirtualScreenWidth
                || window.Height > SystemParameters.VirtualScreenHeight) {
                window.Width = 700d;
                window.Height = 700d;
                window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                window.WindowState = WindowState.Normal;
            }
            else
                window.WindowState = state;
        }

        public static void Run(string name) {
            var myProcess = new Process();

            try {
                myProcess.StartInfo.UseShellExecute = true;
                myProcess.StartInfo.FileName = name;
                myProcess.Start();
            }
            catch (Exception) { }
        }
    }

    public class WindowClosingEvent : PubSubEvent<CancelEventArgs> { }

}
