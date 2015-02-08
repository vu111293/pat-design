#region Fireball License
//    Copyright (C) 2005  Sebastian Faltoni sebastian{at}dotnetfireball{dot}net
//
//    This library is free software; you can redistribute it and/or
//    modify it under the terms of the GNU Lesser General Public
//    License as published by the Free Software Foundation; either
//    version 2.1 of the License, or (at your option) any later version.
//
//    This library is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//    Lesser General Public License for more details.
//
//    You should have received a copy of the GNU Lesser General Public
//    License along with this library; if not, write to the Free Software
//    Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA

#endregion
#region Original License
// *****************************************************************************
// 
//  Copyright 2004, Weifen Luo
//  All rights reserved. The software and associated documentation 
//  supplied hereunder are the proprietary information of Weifen Luo
//  and are supplied subject to licence terms.
// 
//  WinFormsUI Library Version 1.0
// *****************************************************************************
#endregion


using System;
using System.Runtime.InteropServices;
using System.Threading;
using Fireball.Docking.Helpers;
using HookProc = Fireball.Docking.Helpers.PInvoke.HookProc;
using HookType = Fireball.Docking.Helpers.PInvoke.HookType;
namespace Fireball.Docking
{
	internal class HookEventArgs : EventArgs
	{
		public int HookCode;    // Hook code
		public IntPtr wParam;   // WPARAM argument
		public IntPtr lParam;   // LPARAM argument
	}

	internal class LocalWindowsHook
	{
        private static IntPtr SetWindowsHookEx(HookType code, HookProc func, IntPtr hInstance, int threadID)
        {
            if (Win32Helper.IsWindowsOS())
            {
                return PInvoke.SetWindowsHookEx(code, func, hInstance, threadID);
            }
            else
            {
                return IntPtr.Zero; ;
            }
        }

		private static int UnhookWindowsHookEx(IntPtr hhook)
        {
            if (Win32Helper.IsWindowsOS())
            {
                return PInvoke.UnhookWindowsHookEx(hhook);
            }
            else
            {
                return 0;
            }
        }

        private static int CallNextHookEx(IntPtr hhook, int code, IntPtr wParam, IntPtr lParam)
        {
            if (Win32Helper.IsWindowsOS())
            {
                return PInvoke.CallNextHookEx(hhook, code, wParam, lParam);
            }
            else
            {
                return 0;
            }
        }

		// Internal properties
		private IntPtr m_hhook = IntPtr.Zero;
		private HookProc m_filterFunc = null;
		private HookType m_hookType;

		// Event delegate
		public delegate void HookEventHandler(object sender, HookEventArgs e);

		// Event: HookInvoked 
		public event HookEventHandler HookInvoked;
		protected void OnHookInvoked(HookEventArgs e)
		{
			if (HookInvoked != null)
				HookInvoked(this, e);
		}

		// Class constructor(s)
		public LocalWindowsHook(HookType hook)
		{
			m_hookType = hook;
			m_filterFunc = new HookProc(this.CoreHookProc);
		}

		public LocalWindowsHook(HookType hook, HookProc func)
		{
			m_hookType = hook;
			m_filterFunc = func; 
		}        

		// Default filter function
		public int CoreHookProc(int code, IntPtr wParam, IntPtr lParam)
		{
			if (code < 0)
				return CallNextHookEx(m_hhook, code, wParam, lParam);

			// Let clients determine what to do
			HookEventArgs e = new HookEventArgs();
			e.HookCode = code;
			e.wParam = wParam;
			e.lParam = lParam;
			OnHookInvoked(e);

			// Yield to the next hook in the chain
			return CallNextHookEx(m_hhook, code, wParam, lParam);
		}

		// Install the hook
		public void Install()
		{
		    int threadId = System.AppDomain.GetCurrentThreadId(); // Kernel32.GetCurrentThreadId(); //Thread.CurrentThread.ManagedThreadId; //
			m_hhook = SetWindowsHookEx(m_hookType, m_filterFunc, IntPtr.Zero, threadId);
		}

		// Uninstall the hook
		public void Uninstall()
		{
			UnhookWindowsHookEx(m_hhook); 
		}
	}
}
