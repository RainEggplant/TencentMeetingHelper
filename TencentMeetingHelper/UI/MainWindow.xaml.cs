using System;
using System.Windows;

namespace TencentMeetingHelper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int desiredWidth;
        private int desiredHeight;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var dpiFactor = PresentationSource.FromVisual(this).CompositionTarget.TransformToDevice.M11;
            var screenSize = SystemParameters.WorkArea;
            desiredWidth = (int)(screenSize.Width * dpiFactor / 2);
            desiredHeight = (int)(screenSize.Height * dpiFactor);
        }

        private IntPtr GetWindowHandle()
        {
            var hWnd = WindowUtilty.GetTencentMeetingWindowHandle();
            if (hWnd == IntPtr.Zero)
            {
                MessageBox.Show("Cannot find the window of Tencent Meeting!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                StatusText.Text = "Missing window";
            }
            return hWnd;
        }

        private void SnapLeftButton_Click(object sender, RoutedEventArgs e)
        {
            var hWnd = GetWindowHandle();
            if (hWnd != IntPtr.Zero)
            {
                var result = WindowUtilty.SetWindowPosition(hWnd, 0, 0, desiredWidth, desiredHeight);
                StatusText.Text = result ? "Snap Left (successful)" : "Snap Left (failed)";
            }
        }

        private void SnapRightButton_Click(object sender, RoutedEventArgs e)
        {
            var hWnd = GetWindowHandle();
            if (hWnd != IntPtr.Zero) 
            {
                var result = WindowUtilty.SetWindowPosition(hWnd, desiredWidth + 1, 0, desiredWidth, desiredHeight);
                StatusText.Text = result ? "Snap Right (successful)" : "Snap Right (failed)";
            }
        }

        private void TopMostButton_Click(object sender, RoutedEventArgs e)
        {
            var hWnd = GetWindowHandle();
            if (hWnd != IntPtr.Zero)
            {
                var result = WindowUtilty.SetWindowTopMost(hWnd, true);
                StatusText.Text = result ? "Set TopMost (successful)" : "Set TopMost (failed)";
            }
        }


        private void NoTopMostButton_Click(object sender, RoutedEventArgs e)
        {
            var hWnd = GetWindowHandle();
            if (hWnd != IntPtr.Zero)
            {
                var result = WindowUtilty.SetWindowTopMost(hWnd, false);
                StatusText.Text = result ? "Unset TopMost (successful)" : "Unset TopMost (failed)";
            }
        }

        private void GitHubButton_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/RainEggplant/TencentMeetingHelper");
        }
    }
}
