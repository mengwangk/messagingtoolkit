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

namespace MessagingToolkit.Barcode.Common
{
	
	/// <summary> 
    /// Superclass of classes encapsulating types ECIs, according to "Extended Channel Interpretations"
	/// 5.3 of ISO 18004. 
	/// </summary>
    internal abstract class ECI
	{
        private int val;
		

		virtual public int Value
		{
			get
			{
				return val;
			}
			
		}
		
	
		internal ECI(int value)
		{
			this.val = value;
		}

        /// <summary>
        /// Gets the ECI by value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        /// ECI representing ECI of given value, or null if it is legal but unsupported
        /// </returns>
        /// <throws>  IllegalArgumentException if ECI value is invalid </throws>
		public static ECI GetECIByValue(int value)
		{
			if (value < 0 || value > 999999)
			{
				throw new System.ArgumentException("Bad ECI value: " + value);
			}
			if (value < 900)
			{
				// Character set ECIs use 000000 - 000899
				return CharacterSetECI.GetCharacterSetECIByValue(value);
			}
			return null;
		}
	}
}