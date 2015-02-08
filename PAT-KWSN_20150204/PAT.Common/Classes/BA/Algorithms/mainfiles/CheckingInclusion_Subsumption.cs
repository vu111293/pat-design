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
using InclusionAnti = cav2010.algorithms.InclusionAnti;
using FiniteAutomaton = cav2010.automata.FiniteAutomaton;
using FAState = cav2010.automata.FAState;
namespace cav2010.mainfiles
{
	/// <summary> </summary>
	/// <author>  Yu-Fang Chen
	/// 
	/// </author>
	public class CheckingInclusion_Subsumption
	{
		internal static long timeout = 0;
		
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
				System.Console.Error.WriteLine("Usage: main aut1.BA aut2.BA [timeout (sec)].");
				System.Console.Error.WriteLine("The program checks if Lang(aut1) <= Lang(aut2).");
				return ;
			}
			long ttime1;
			FiniteAutomaton aut1 = new FiniteAutomaton(args[0]);
			FiniteAutomaton aut2 = new FiniteAutomaton(args[1]);
			System.Console.Out.WriteLine("Aut1: # of Trans. " + aut1.trans + ", # of States " + aut1.states.size() + ".");
			System.Console.Out.WriteLine("Aut2: # of Trans. " + aut2.trans + ", # of States " + aut2.states.size() + ".");
			
			InclusionAnti inclusion = new InclusionAnti(aut1, aut2);
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
				if (inclusion.Included)
				{
					System.Console.Out.WriteLine("Included");
				}
				else
				{
					System.Console.Out.WriteLine("Not Included");
				}
				System.Console.Out.WriteLine("Time for the Subsumption algorithm(ms): " + ttime1 / 1000000 + ".");
			}
		}
		
		
		
		/// <summary>  Generate automata using Tabakov and Vardi's approach
		/// 
		/// </summary>
		/// <param name="num">
		/// </param>
		/// <returns> a random finite automaton
		/// </returns>
		/// <author>  Yu-Fang Chen
		/// </author>
		public static FiniteAutomaton genRandomTV(int size, float td, float fd, int alphabetSize)
		{
			FiniteAutomaton result = new FiniteAutomaton();
			//UPGRADE_ISSUE: The following fragment of code could not be parsed and was not converted. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1156'"
			TreeMap < Integer, FAState > st = new TreeMap < Integer, FAState >();
			
			td = td / size;
			System.Random r = new System.Random();
			
			for (int i = 0; i < size; i++)
			{
				st.put(i, result.createState());
				float rm = (float) r.NextDouble();
				if (fd > rm)
					result.F.add(st.get_Renamed(i));
			}
			result.setInitialState(st.get_Renamed(0));
			result.F.add(st.get_Renamed(0));
			for (int i = 0; i < size; i++)
			{
				for (int j = 0; j < size; j++)
				{
					for (int k = 0; k < alphabetSize; k++)
					{
						if (td > (float) r.NextDouble())
							result.addTransition(st.get_Renamed(i), st.get_Renamed(j), ("a" + k));
					}
				}
			}
			return result;
		}
	}
}