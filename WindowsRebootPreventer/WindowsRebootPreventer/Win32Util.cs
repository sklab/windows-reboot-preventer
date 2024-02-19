using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WindowsRebootPreventer
{
    public static class Win32Util
    {
        #region [Load Win32API]

        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr FindWindow(IntPtr ZeroOnly, string lpWindowName);

        [DllImport("user32.dll")]
        static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, int uFlags);

        [DllImport("user32.dll")]
        public static extern bool PostMessage(IntPtr hWnd, UInt32 Msg, Int32 wParam, uint lParam);

        #endregion

        #region [Constants]

        public const int WM_NULL = 0x0000; // A message that has no effect
        public const int WM_MOVE = 0x0003; // Window movement
        public const int WM_SIZE = 0x0005; // Window size change
        public const int WM_ACTIVATE = 0x0006; // Window activation/deactivation
        public const int WM_SETTEXT = 0x000C; // Set the window title or control text
        public const int WM_GETTEXT = 0x000D; // Get the window title or control text
        public const int WM_GETTEXTLENGTH = 0x000E; // Get the size of the window title or control text
        public const int WM_SETFONT = 0x0030; // Set the control font
        public const int WM_GETFONT = 0x0031; // Get the control font
        public const int WM_NOTIFY = 0x004E; // Notification from common controls
        public const int WM_CONTEXTMENU = 0x007B; // Notification to display the context menu
        public const int WM_GETICON = 0x007F; // Get the window icon
        public const int WM_KEYDOWN = 0x0100; // A non-system key was pressed
        public const int WM_KEYUP = 0x0101; // A non-system key that was pressed was released
        public const int WM_CHAR = 0x0102; // Character input from the keyboard
        public const int WM_COMMAND = 0x0111; // Menu item selection/control notification
        public const int WM_SYSCOMMAND = 0x0112; // System menu item selection
        public const int WM_MENUSELECT = 0x011F; // A menu item was selected
        public const int WM_LBUTTONDOWN = 0x0201; // Pressed the left mouse button
        public const int WM_LBUTTONUP = 0x0202; // Released the left mouse button
        public const int WM_LBUTTONDBLCLK = 0x0203; // Double-clicked the left mouse button
        public const int WM_RBUTTONDOWN = 0x0204; // Pressed the right mouse button
        public const int WM_RBUTTONUP = 0x0205; // Released the right mouse button
        public const int WM_RBUTTONDBLCLK = 0x0206; // Double-clicked the right mouse button
        public const int WM_MBUTTONDOWN = 0x0207; // Pressed the middle mouse button
        public const int WM_MBUTTONUP = 0x0208; // Released the middle mouse button
        public const int WM_MBUTTONDBLCLK = 0x0209; // Double-clicked the middle mouse button
        public const int WM_DROPFILES = 0x0233; // Files were dropped
        public const int WM_CUT = 0x0300; // Cut the text of the edit control
        public const int WM_COPY = 0x0301; // Copy the text of the edit control
        public const int WM_PASTE = 0x0302; // Paste the text of the edit control
        public const int WM_CLEAR = 0x0303; // Delete the text of the edit control
        public const int WM_UNDO = 0x0304; // Undo the previous operation of the edit control
        public const int WM_USER = 0x0400; // The beginning of the application-defined message

        /// <summary>
        /// Retains the current size (ignores the cx and cy parameters).
        /// </summary>
        /// <see cref="https://learn.microsoft.com/ja-jp/windows/win32/api/winuser/nf-winuser-setwindowpos"/>
        public const int SWP_NOSIZE = 0x0001;

        /// <summary>
        /// Retains the current Z order (ignores the hWndInsertAfter parameter).
        /// </summary>
        public const int SWP_NOZORDER = 0x0004;

        #endregion

        #region [Methods]

        /// <summary>
        /// Get Windows by caption
        /// </summary>
        /// <param name="caption"></param>
        /// <returns>A handle to the window</returns>
        public static IntPtr GetWindow(string caption)
        {
            const int INTERVAL = 100;

            int timeout = 3 * 1000;
            IntPtr hWnd;

            while ((hWnd = FindWindow(IntPtr.Zero, caption)) == IntPtr.Zero)
            {
                Thread.Sleep(INTERVAL);
                timeout -= INTERVAL;
                if (timeout < 0) return IntPtr.Zero;
            }

            return hWnd;
        }

        /// <summary>
        /// Set window position by window handle
        /// </summary>
        /// <param name="hWnd">A handle to the window</param>
        /// <param name="x">The new position of the left side of the window, in client coordinates.</param>
        /// <param name="y">The new position of the top of the window, in client coordinates.</param>
        public static void SetWindowPosition(IntPtr hWnd, int x, int y)
        {
            if (IntPtr.Zero == hWnd) return;
            SetWindowPos(hWnd, IntPtr.Zero, x, y, 0, 0, SWP_NOZORDER | SWP_NOSIZE);
        }

        /// <summary>
        /// Set window position by window caption
        /// </summary>
        /// <param name="caption">A handle to the window</param>
        /// <param name="x">The new position of the left side of the window, in client coordinates.</param>
        /// <param name="y">The new position of the top of the window, in client coordinates.</param>
        public static void SetWindowPosition(string caption, int x, int y)
        {
            IntPtr hWnd = GetWindow(caption);
            SetWindowPosition(hWnd, x, y);
        }

        #endregion

    }
}
