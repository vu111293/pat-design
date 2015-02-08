// Check weather the process has exsisted in the
// current processes, if yes, loading the old process
// esle, create a new instance of PAT

// @author Ma Junwei,2010-11-26
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace PAT.GUI
{
    static class ProcessCheck
    {
        /// <summary>
        ///  the process name string must be checked appearon in the windows form title
        /// </summary>
        private static string _checkString;

        /// <summary>
        /// contains signatures for C++ dlls using interop
        /// </summary>
        internal static class NativeMethods
        {
            [DllImport("user32.dll")]
            public static extern bool ShowWindowAsync(IntPtr hWnd, int nCmdShow);

            [DllImport("user32.dll")]
            public static extern bool SetForegroundWindow(IntPtr hWnd);

            [DllImport("user32.dll")]
            public static extern bool EnumWindows(EnumWindowsProcDel lpEnumFunc,
                Int32 lParam);

            [DllImport("user32.dll")]
            public static extern int GetWindowThreadProcessId(IntPtr hWnd,
                ref Int32 lpdwProcessId);

            [DllImport("user32.dll")]
            public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString,
                Int32 nMaxCount);

            public const int SW_SHOWNORMAL = 1;

        }

        public delegate bool EnumWindowsProcDel(IntPtr hWnd, Int32 lParam);

        /// <summary>
        /// finding and showing of running window.
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="lParam"></param>
        /// <returns></returns>
        static private bool EnumWindowsProc(IntPtr hWnd, Int32 lParam)
        {
            int processId = 0;
            NativeMethods.GetWindowThreadProcessId(hWnd, ref processId);

            var caption = new StringBuilder(1024);
            NativeMethods.GetWindowText(hWnd, caption, 1024);

            if(processId == lParam && 
                (caption.ToString().IndexOf(_checkString,StringComparison.OrdinalIgnoreCase)!= -1))
            {
                NativeMethods.ShowWindowAsync(hWnd, NativeMethods.SW_SHOWNORMAL);
                NativeMethods.SetForegroundWindow(hWnd);
            }
            return true;
        }

        /// <summary>
        /// Find out if we have started the process, if there's a process
        /// in running, then return true, esle return false
        /// </summary>
        /// <param name="titleSnippet">
        /// the snippet of the title that the specific windows process has
        /// </param>
        /// <returns></returns>
        static public bool IsCreated(string titleSnippet)
        {
            _checkString = titleSnippet;
            
            // get the current processes named "PAT 3" in the local machine 
            // It had better pass the paramter "Application.ProductName"
            foreach(Process process in Process.GetProcessesByName("PAT 3"))
            {
                if(process.Id != Process.GetCurrentProcess().Id)
                {
                    NativeMethods.EnumWindows(new EnumWindowsProcDel(EnumWindowsProc), process.Id);
                    return true;
                }
            }
            return false;
        }
    }
}
