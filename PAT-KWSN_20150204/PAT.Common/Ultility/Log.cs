using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace PAT.Common.Ultility
{
    public static class Log
    {
        public static void d(string TAG, string message)
        {
            log("[{0}] {1}", TAG, message);
        }

        private static void log(string format, params object[] args)
        {
#if DEBUG
            Debug.WriteLine(format, args);
#else
            return;
#endif
        }
    }
}
