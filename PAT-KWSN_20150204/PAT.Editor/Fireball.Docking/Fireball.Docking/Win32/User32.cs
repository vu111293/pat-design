using System;
using System.Drawing;
//using System.Runtime.InteropServices;
using Fireball.Docking.Win32;
using Fireball.Docking.Helpers;

namespace Fireball.Docking
{
    internal class User32
    {
        public static bool DragDetect(IntPtr hWnd, Point pt)
        {
            if (Win32Helper.IsWindowsOS())
            {
                return PInvoke.DragDetect(hWnd, pt);
            }
            else
            {
                return false;
            }
        }


        public static IntPtr GetFocus()
        {
            if (Win32Helper.IsWindowsOS())
            {
                return PInvoke.GetFocus();
            }
            else
            {
                return IntPtr.Zero;
            }
        }

        public static void PostMessage(IntPtr hWnd, int Msg, uint wParam, uint lParam)
        {
            if (Win32Helper.IsWindowsOS())
            {
                PInvoke.PostMessage(hWnd, Msg, wParam, lParam);
            }
        }

        public static uint SendMessage(IntPtr hWnd, int Msg, uint wParam, uint lParam)
        {
            if (Win32Helper.IsWindowsOS())
            {
                return PInvoke.SendMessage(hWnd, Msg, wParam, lParam);
            }
            else
            {
                //TODO TKN: find suitable value to return
                return uint.MinValue;
            }
        }

        public static void TrackMouseEvent(ref TRACKMOUSEEVENTS tme)
        {
            if (Win32Helper.IsWindowsOS())
            {
                PInvoke.TrackMouseEvent(ref tme);
            }
        }

        public static IntPtr WindowFromPoint(POINT point)
        {
            if (Win32Helper.IsWindowsOS())
            {
                return PInvoke.WindowFromPoint(point);
            }
            else
            {
                return IntPtr.Zero;
            }
        }
	
	}
}