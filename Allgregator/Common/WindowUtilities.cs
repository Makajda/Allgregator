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
            var settings = settingsRepository.GetOrDefault();
            return settings;
        }

        public static MainWindow GetMainWindow() {
            var container = (App.Current as PrismApplication).Container;
            var window = container.Resolve<MainWindow>();
            var settings = container.Resolve<Settings>();
            SetWindowBoundsAndState(window, settings.MainWindowLeft, settings.MainWindowTop, settings.MainWindowWidth, settings.MainWindowHeight, settings.MainWindowState);
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
                settings.MainWindowLeft = mainWindow.RestoreBounds.Left;
                settings.MainWindowTop = mainWindow.RestoreBounds.Top;
                settings.MainWindowWidth = mainWindow.RestoreBounds.Width;
                settings.MainWindowHeight = mainWindow.RestoreBounds.Height;
                settings.MainWindowState = mainWindow.WindowState;
                try {
                    settingsRepository.Save(settings);
                }
                catch (Exception ex) { /*//TODO Log*/ }
            }
        }

        private static void SetWindowBoundsAndState(Window window, double left, double top, double width, double height, WindowState state) {
            window.Left = double.IsInfinity(left) ? 0d : left;
            window.Top = double.IsInfinity(top) ? 0d : top;
            if (width > double.Epsilon && !double.IsInfinity(width))
                window.Width = width;
            if (height > double.Epsilon && !double.IsInfinity(height))
                window.Height = height;
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
