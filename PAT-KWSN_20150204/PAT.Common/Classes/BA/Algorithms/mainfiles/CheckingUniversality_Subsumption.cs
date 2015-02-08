/* Written by Chen Yu-Fang and Hong Chih-Duo                              */
/* Copyright (c) 2010  Academia Sinica	                                  */
/*                                                                        */
/* This program is free software; you can redistribute it and/or modify   */
/* it under the terms of the GNU General Public License as published by   */
/* the Free Software Foundation; either version 2 of the License, or      */
/* (at your option) any later version.                                    */
/*                                                                        */
/* This program is distributed in the hope that it will be useful,        */
/* but WITHOUT ANY WARRANTY; without even the implied warranty of         */
/* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the          */
/* GNU General Public License for more details.                           */
/*                                                                        */
/* You should have received a copy of the GNU General Public License      */
/* along with this program; if not, write to the Free Software            */
/* Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA*/
using System;
//UPGRADE_TODO: The package 'java.lang.management' could not be found. If it was not included in the conversion, there may be compiler issues. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1262'"
using java.lang.management;
using UniversalityAnti = cav2010.algorithms.UniversalityAnti;
using FiniteAutomaton = cav2010.automata.FiniteAutomaton;
namespace cav2010.mainfiles
{
	
	/// <summary> </summary>
	/// <author>  Yu-Fang Chen
	/// 
	/// </author>
	
	/// <summary>Get CPU time in nanoseconds. </summary>
	public class CheckingUniversality_Subsumption
	{
		internal static long timeout = 100000;
		
		/// <summary>Get CPU time in nanoseconds. </summary>
		static public long getCpuTime(long id)
		{
			ThreadMXBean bean = ManagementFactory.getThreadMXBean();
			if (!bean.isThreadCpuTimeSupported())
				return 0L;
			else
				return bean.getThreadCpuTime(id);
		}
		
		[STAThread]
		public static void  Main(System.String[] args)
		{
			if (args.Length == 3)
			{
				timeout = System.Int32.Parse(args[2]) * 1000;
			}
			if (args.Length < 2)
			{
				System.Console.Error.WriteLine("Usage: main aut.BA [timeout (sec)].");
				System.Console.Error.WriteLine("The program checks if the BA aut is universal.");
				return ;
			}
			long ttime1;
			FiniteAutomaton aut = new FiniteAutomaton(args[0]);
			System.Console.Out.WriteLine("# of Trans. " + aut.trans + ", # of States " + aut.states.size() + ".");
			
			UniversalityAnti inclusion = new UniversalityAnti(aut);
			inclusion.Start();
			try
			{
				inclusion.Join(timeout);
			}
			catch (System.Threading.ThreadInterruptedException e)
			{
			}
			
			if (inclusion.IsAlive)
			{
				System.Console.Out.WriteLine("Timeout");
			}
			else
			{
				ttime1 = inclusion.RunTime;
				if (inclusion.Universal)
				{
					System.Console.Out.WriteLine("Universal");
				}
				else
				{
					System.Console.Out.WriteLine("Not Universal");
				}
				System.Console.Out.WriteLine("Time for the Subsumption algorithm(ms): " + ttime1 / 1000000 + ".");
			}
		}
	}
}