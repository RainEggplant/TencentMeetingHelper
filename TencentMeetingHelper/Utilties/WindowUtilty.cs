using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace TencentMeetingHelper
{
    public static class WindowUtilty
    {
        private static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
        private static readonly IntPtr HWND_NOTOPMOST = new IntPtr(-2);
        private const UInt32 SWP_NOSIZE = 0x0001;
        private const UInt32 SWP_NOMOVE = 0x0002;
        private const UInt32 SWP_NOOWNERZORDER = 0x0200;

        public static IntPtr GetTencentMeetingWindowHandle()
        {
            var windows = FindWindowsWithText("腾讯会议");
            var hWnd = IntPtr.Zero;
            foreach (var w in windows)
            {
                var childHWnd = FindWindowEx(w, IntPtr.Zero, "TXGuiFoundation", null);
                if (childHWnd != IntPtr.Zero)
                {
                    hWnd = w;
                    break;
                }
            }
            return hWnd;
        }

        /// <summary> Find all windows that contain the given title text </summary>
        /// <param name="titleText"> The text that the window title must contain. </param>
        public static IEnumerable<IntPtr> FindWindowsWithText(string titleText)
        {
            return FindWindows(delegate (IntPtr wnd, IntPtr param)
            {
                return GetWindowText(wnd).Contains(titleText);
            });
        }

        public static bool SetWindowPosition(IntPtr hWnd, int x, int y, int width, int height)
        {
            return SetWindowPos(hWnd, IntPtr.Zero, x, y, width, height, SWP_NOOWNERZORDER);
        }

        public static bool SetWindowTopMost(IntPtr hWnd, bool topMost)
        {
            var hWndInsertAfter = topMost ? HWND_TOPMOST : HWND_NOTOPMOST;
            return SetWindowPos(hWnd, hWndInsertAfter, 0, 0, 0, 0, SWP_NOSIZE | SWP_NOMOVE);
        }

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern IntPtr FindWindowEx(IntPtr parentHandle, IntPtr childAfter, string lclassName, string windowTitle);

        [DllImport("user32.dll", EntryPoint = "SetWindowPos")]
        private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, uint wFlags);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder strText, int maxCount);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern int GetWindowTextLength(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern bool EnumWindows(EnumWindowsProc enumProc, IntPtr lParam);

        // Delegate to filter which windows to include 
        private delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

        /// <summary> Get the text for the window pointed to by hWnd </summary>
        private static string GetWindowText(IntPtr hWnd)
        {
            int size = GetWindowTextLength(hWnd);
            if (size > 0)
            {
                var builder = new StringBuilder(size + 1);
                GetWindowText(hWnd, builder, builder.Capacity);
                return builder.ToString();
            }

            return String.Empty;
        }

        /// <summary> Find all windows that match the given filter </summary>
        /// <param name="filter"> A delegate that returns true for windows
        ///    that should be returned and false for windows that should
        ///    not be returned </param>
        private static IEnumerable<IntPtr> FindWindows(EnumWindowsProc filter)
        {
            IntPtr found = IntPtr.Zero;
            List<IntPtr> windows = new List<IntPtr>();

            EnumWindows(delegate (IntPtr hWnd, IntPtr param)
            {
                if (filter(hWnd, param))
                {
                    // only add the windows that pass the filter
                    windows.Add(hWnd);
                }

                // but return true here so that we iterate all windows
                return true;
            }, IntPtr.Zero);

            return windows;
        }
    }
}
