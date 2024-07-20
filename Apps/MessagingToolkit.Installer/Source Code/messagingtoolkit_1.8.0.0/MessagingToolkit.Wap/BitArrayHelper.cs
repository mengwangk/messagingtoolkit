//===============================================================================
// OSML - Open Source Messaging Library
//
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
using System.Linq;
using System.Text;
using System.Reflection;
using System.Collections;
using System.IO;
using System.Xml;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace MessagingToolkit.Wap
{
    /// <summary>
    /// Support class for the BitArray class.
    /// </summary>
    internal class BitArrayHelper
    {
        /// <summary>
        /// Sets the specified bit to true.
        /// </summary>
        /// <param name="bits">The BitArray to modify.</param>
        /// <param name="index">The bit index to set to true.</param>
        public static void Set(BitArray bits, Int32 index)
        {
            for (int increment = 0; index >= bits.Length; increment = +64)
            {
                bits.Length += increment;
            }


            bits.Set(index, true);
        }

        /// <summary>
        /// Returns a string representation of the BitArray object.
        /// </summary>
        /// <param name="bits">The BitArray object to convert to string.</param>
        /// <returns>A string representation of the BitArray object.</returns>
        public static string ToString(BitArray bits)
        {
            StringBuilder s = new StringBuilder();
            if (bits != null)
            {
                for (int i = 0; i < bits.Length; i++)
                {
                    if (bits[i] == true)
                    {
                        if (s.Length > 0)
                            s.Append(", ");
                        s.Append(i);
                    }
                }

                s.Insert(0, "{");
                s.Append("}");
            }
            else
                s.Insert(0, "null");

            return s.ToString();
        }
    }
}
