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
    /// This is merely a clone of <code>Comparator</code> since it is not available in
	/// CLDC 1.1 / MIDP 2.0.
	/// </summary>
	public interface Comparator
	{
		
		int Compare(System.Object o1, System.Object o2);
	}
}