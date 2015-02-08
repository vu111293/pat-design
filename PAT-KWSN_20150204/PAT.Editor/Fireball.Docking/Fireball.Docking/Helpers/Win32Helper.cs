using System;
using System.Drawing;
using System.Windows.Forms;

namespace Fireball.Docking
{
	internal class Win32Helper
	{
		public static Control ControlAtPoint(Point pt)
		{
			Win32.POINT pt32;
			pt32.x = pt.X;
			pt32.y = pt.Y;

			return Control.FromChildHandle(User32.WindowFromPoint(pt32));
		}

		public static uint MakeLong(int low, int high)
		{
			return (uint)((high << 16) + low);
		}

        public static bool IsWindowsOS()
        {
            return (Environment.OSVersion.Platform == PlatformID.Win32NT || Environment.OSVersion.Platform == PlatformID.Win32Windows);
        }
	}
}
