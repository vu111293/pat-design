using System;
using System.Runtime.InteropServices;
using Fireball.Docking.Win32;
using System.Drawing;

namespace Fireball.Docking.Helpers
{
    internal class PInvoke
    {
        #region User32
        //[DllImport("User32.dll", CharSet=CharSet.Auto)]
        //public static extern bool AnimateWindow(IntPtr hWnd, uint dwTime, FlagsAnimateWindow dwFlags);

        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern bool DragDetect(IntPtr hWnd, Point pt);

        //[DllImport("user32.dll", CharSet=CharSet.Auto)]
        //public static extern int EnableWindow(IntPtr hwnd, bool bEnable);

        //[DllImport("User32.dll", CharSet=CharSet.Auto)]
        //public static extern IntPtr GetSysColorBrush(int index);

        //[DllImport("User32.dll", CharSet=CharSet.Auto)]
        //public static extern IntPtr GetDesktopWindow();

        //[DllImport("User32.dll", CharSet=CharSet.Auto)]
        //public static extern bool InvalidateRect(IntPtr hWnd, ref RECT rect, bool erase);

        //[DllImport("User32.dll", CharSet=CharSet.Auto)]
        //public static extern IntPtr LoadCursor(IntPtr hInstance, uint cursor);

        //[DllImport("User32.dll", CharSet = CharSet.Auto)]
        //public static extern IntPtr SetCursor(IntPtr hCursor);

        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr GetFocus();

        //[DllImport("User32.dll", CharSet = CharSet.Auto)]
        //public static extern IntPtr SetFocus(IntPtr hWnd);

        //[DllImport("User32.dll", CharSet=CharSet.Auto)]
        //public static extern bool ReleaseCapture();

        //[DllImport("User32.dll", CharSet=CharSet.Auto)]
        //public static extern bool WaitMessage();

        //[DllImport("User32.dll", CharSet=CharSet.Auto)]
        //public static extern bool TranslateMessage(ref MSG msg);

        //[DllImport("User32.dll", CharSet=CharSet.Auto)]
        //public static extern bool DispatchMessage(ref MSG msg);

        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern bool PostMessage(IntPtr hWnd, int Msg, uint wParam, uint lParam);

        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern uint SendMessage(IntPtr hWnd, int Msg, uint wParam, uint lParam);

        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern uint SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);

        //[DllImport("User32.dll", CharSet=CharSet.Auto)]
        //public static extern bool GetMessage(ref MSG msg, int hWnd, uint wFilterMin, uint wFilterMax);

        //[DllImport("User32.dll", CharSet=CharSet.Auto)]
        //public static extern bool PeekMessage(ref MSG msg, IntPtr hWnd, uint wFilterMin, uint wFilterMax, uint wFlag);

        //[DllImport("User32.dll", CharSet = CharSet.Auto)]
        //public static extern IntPtr BeginPaint(IntPtr hWnd, ref PAINTSTRUCT ps);

        //[DllImport("User32.dll", CharSet = CharSet.Auto)]
        //public static extern bool EndPaint(IntPtr hWnd, ref PAINTSTRUCT ps);

        //[DllImport("User32.dll", CharSet=CharSet.Auto)]
        //public static extern IntPtr GetDC(IntPtr hWnd);

        //[DllImport("User32.dll", CharSet=CharSet.Auto)]
        //public static extern IntPtr GetWindowDC(IntPtr hWnd);

        //[DllImport("user32.dll", CharSet=CharSet.Auto)]
        //public static extern bool LockWindowUpdate(IntPtr hWnd);

        //[DllImport("User32.dll", CharSet=CharSet.Auto)]
        //public static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);

        //[DllImport("User32.dll", CharSet = CharSet.Auto)]
        //public static extern int ShowWindow(IntPtr hWnd, short cmdShow);

        //[DllImport("User32.dll", CharSet=CharSet.Auto)]
        //public static extern bool MoveWindow(IntPtr hWnd, int x, int y, int width, int height, bool repaint);

        //[DllImport("User32.dll", CharSet = CharSet.Auto)]
        //public static extern int SetWindowPos(IntPtr hWnd, IntPtr hWndAfter, int X, int Y, int Width, int Height, FlagsSetWindowPos flags);

        //[DllImport("User32.dll", CharSet=CharSet.Auto)]
        //public static extern bool UpdateLayeredWindow(IntPtr hwnd, IntPtr hdcDst, ref POINT pptDst, ref SIZE psize, IntPtr hdcSrc, ref POINT pprSrc, Int32 crKey, ref BLENDFUNCTION pblend, Int32 dwFlags);

        //[DllImport("User32.dll", CharSet=CharSet.Auto)]
        //public static extern bool GetWindowRect(IntPtr hWnd, ref RECT rect);

        //[DllImport("User32.dll", CharSet=CharSet.Auto)]
        //public static extern bool ClientToScreen(IntPtr hWnd, ref POINT pt);

        //[DllImport("User32.dll", CharSet=CharSet.Auto)]
        //public static extern bool ScreenToClient(IntPtr hWnd, ref POINT pt);

        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern bool TrackMouseEvent(ref TRACKMOUSEEVENTS tme);

        //[DllImport("User32.dll", CharSet=CharSet.Auto)]
        //public static extern bool SetWindowRgn(IntPtr hWnd, IntPtr hRgn, bool redraw);

        //[DllImport("User32.dll", CharSet=CharSet.Auto)]
        //public static extern ushort GetKeyState(int virtKey);

        //[DllImport("User32.dll", CharSet=CharSet.Auto)]
        //public static extern IntPtr GetParent(IntPtr hWnd);

        //[DllImport("user32.dll", CharSet = CharSet.Auto)]
        //public static extern int GetWindowLong(IntPtr hWnd, int Index);

        //[DllImport("user32.dll", CharSet = CharSet.Auto)]
        //public static extern int SetWindowLong(IntPtr hWnd, int Index, int Value);

        //[DllImport("User32.dll", CharSet=CharSet.Auto)]
        //public static extern bool DrawFocusRect(IntPtr hWnd, ref RECT rect);

        //[DllImport("User32.dll", CharSet=CharSet.Auto)]
        //public static extern bool HideCaret(IntPtr hWnd);

        //[DllImport("User32.dll", CharSet=CharSet.Auto)]
        //public static extern bool ShowCaret(IntPtr hWnd);

        //[DllImport("user32.dll", CharSet = CharSet.Auto)]
        //public static extern int ShowScrollBar(IntPtr hWnd, int wBar, int bShow);

        //[DllImport("user32.dll", CharSet = CharSet.Auto)]
        //public static extern bool SystemParametersInfo(SystemParametersInfoActions uAction, uint uParam, ref uint lpvParam, uint fuWinIni);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr WindowFromPoint(POINT point);

        //[DllImport("user32.dll", CharSet=CharSet.Auto)]
        //public static extern IntPtr DefWindowProc(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);
        #endregion User32

        #region DragHandlerBase
        public delegate IntPtr WndProcCallBack(IntPtr hwnd, int Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("User32.dll")]
        public static extern IntPtr SetWindowLong(IntPtr hWnd, int nIndex, WndProcCallBack wndProcCallBack);

        [DllImport("User32.dll")]
        public static extern IntPtr SetWindowLong(IntPtr hWnd, int nIndex, IntPtr wndFunc);

        [DllImport("User32.dll")]
        public static extern IntPtr CallWindowProc(IntPtr prevWndFunc, IntPtr hWnd, int iMsg, IntPtr wParam, IntPtr lParam);
        #endregion DragHandlerBase

        #region LocalWindowsHook
        // Hook Types  
        internal enum HookType : int
        {
            WH_JOURNALRECORD = 0,
            WH_JOURNALPLAYBACK = 1,
            WH_KEYBOARD = 2,
            WH_GETMESSAGE = 3,
            WH_CALLWNDPROC = 4,
            WH_CBT = 5,
            WH_SYSMSGFILTER = 6,
            WH_MOUSE = 7,
            WH_HARDWARE = 8,
            WH_DEBUG = 9,
            WH_SHELL = 10,
            WH_FOREGROUNDIDLE = 11,
            WH_CALLWNDPROCRET = 12,
            WH_KEYBOARD_LL = 13,
            WH_MOUSE_LL = 14
        }

        public delegate int HookProc(int code, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        public static extern IntPtr SetWindowsHookEx(HookType code, HookProc func, IntPtr hInstance, int threadID);

        [DllImport("user32.dll")]
        public static extern int UnhookWindowsHookEx(IntPtr hhook);

        [DllImport("user32.dll")]
        public static extern int CallNextHookEx(IntPtr hhook, int code, IntPtr wParam, IntPtr lParam);

        #endregion LocalWindowsHook
    }
}
