using Prism.Events;
using Prism.Ioc;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;

namespace Allgregator.Aux.Common {
    public static class WindowUtilities {
        public static Window CreateShell<View, ViewModel>(IContainerProvider container) where View : Window {
            var viewModel = container.Resolve<ViewModel>();
            var window = container.Resolve<View>();
            window.DataContext = viewModel;
            window.Closing += (s, e) => Window_Closing(s, e, container);
            window.KeyDown += (_, e) => { if (e.Key == Key.Escape) window.Close(); };

            var settings = container.Resolve<Settings>();
            SetWindowBoundsAndState(
                window,
                settings.MainWindowLeft,
                settings.MainWindowTop,
                settings.MainWindowWidth,
                settings.MainWindowHeight,
                settings.MainWindowState,
                settings.MainWindowTopmost);
            return window;
        }

        private static void Window_Closing(object sender, CancelEventArgs e, IContainerProvider container) {
            if (sender is Window window) {
                var eventAggregator = container.Resolve<IEventAggregator>();
                eventAggregator.GetEvent<WindowClosingEvent>().Publish(e);
                var settings = container.Resolve<Settings>();
                var settingsRepository = container.Resolve<SettingsRepository>();
                settings.MainWindowLeft = window.RestoreBounds.Left;
                settings.MainWindowTop = window.RestoreBounds.Top;
                settings.MainWindowWidth = window.RestoreBounds.Width;
                settings.MainWindowHeight = window.RestoreBounds.Height;
                settings.MainWindowState = window.WindowState;
                try {
                    settingsRepository.Save(settings);
                }
                catch (Exception ex) {
                    Serilog.Log.Error(ex, System.Reflection.MethodBase.GetCurrentMethod().Name);
                }
            }
        }

        private static void SetWindowBoundsAndState(Window window, double left, double top, double width, double height,
            WindowState state, bool topmost) {
            window.Topmost = topmost;
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
