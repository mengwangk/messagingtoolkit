//===============================================================================
// Copyright © TWIT88.COM.  All rights reserved.
//
// This file is part of Open Source Messaging Library.
//
// Open Source Messaging Library is free software: you can redistribute it 
// and/or modify it under the terms of the GNU General Public License version 3.
//
// Open Source Messaging Library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this software.  If not, see <http://www.gnu.org/licenses/>.
//===============================================================================

using System;
using System.Collections.Generic;

namespace MessagingToolkit.Barcode.Common
{
	
	/// <summary> 
    /// Collections class
   	/// </summary>
    internal sealed class Collections
	{
		
		private Collections()
		{
		}

        /// <summary>
        /// Sorts its argument (destructively) using insert sort; in the context of this package
        /// insertion sort is simple and efficient given its relatively small inputs.
        /// </summary>
        /// <param name="vector">vector to sort</param>
        /// <param name="comparator">comparator to define sort ordering</param>
		public static void  InsertionSort<T>(List<T> vector, Comparator comparator)
		{
			int max = vector.Count;
			for (int i = 1; i < max; i++)
			{
				T value = vector[i];
				int j = i - 1;
				T valueB;
				while (j >= 0 && comparator.Compare((valueB = vector[j]), value) > 0)
				{
					vector[j + 1] = valueB;
					j--;
				}
				vector[j + 1] = value;
			}
		}
	}
}