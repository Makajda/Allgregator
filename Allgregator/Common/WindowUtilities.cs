using System.Windows;

namespace Allgregator.Common {
    public static class WindowUtilities {
        public static void SetWindowBoundsAndState(Window window, Rect bounds, WindowState state) {
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
    }
}
