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
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace cav2010.datastructure
{
	
	public class HashSet<T> : System.Collections.Generic.HashSet<T>
	{
		/// <summary> </summary>
        //private void  InitBlock()
        //{
        //    base(c);
        //}
		//UPGRADE_ISSUE: The following fragment of code could not be parsed and was not converted. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1156'"
		//< T > extends java.util.HashSet < T >
		private const long serialVersionUID = 8343382898871066790L;
		
		public HashSet()
		{
			//InitBlock();
		}
		
		//UPGRADE_ISSUE: The following fragment of code could not be parsed and was not converted. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1156'"
        public HashSet(Collection<T> c) : base(c)
		{
		    
		}
		
        //public HashSet(int initialCapacity):base(initialCapacity)
        //{
        //    //InitBlock();
        //}
		
        //public HashSet(int initialCapacity, float loadFactor):base(initialCapacity, loadFactor)
        //{
        //    //InitBlock();
        //}

		//UPGRADE_ISSUE: The following fragment of code could not be parsed and was not converted. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1156'"
		//Override
        //public override System.String ToString()
        //{
        //    System.Text.StringBuilder sb = new System.Text.StringBuilder();
        //    //System.Object[] objs = this..toArray();
			
        //    sb.Append("{");
        //    for (int i = 0; i < objs.Length; i++)
        //    {
        //        if (i == 0)
        //        {
        //            //UPGRADE_TODO: The equivalent in .NET for method 'java.lang.Object.toString' may return a different value. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1043'"
        //            sb.Append(objs[i].ToString());
        //        }
        //        else
        //        {
        //            //UPGRADE_TODO: The equivalent in .NET for method 'java.lang.Object.toString' may return a different value. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1043'"
        //            sb.Append(", " + objs[i].ToString());
        //        }
        //    }
        //    sb.Append("}");
			
        //    return sb.ToString();
        //}
	}
}