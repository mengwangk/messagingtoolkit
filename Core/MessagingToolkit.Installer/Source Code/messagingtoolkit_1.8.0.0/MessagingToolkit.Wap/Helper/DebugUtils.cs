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
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Collections;
using System.IO;
using System.Configuration;
using System.Reflection;
using System.Xml;

using MessagingToolkit.Wap.Log;

namespace MessagingToolkit.Wap.Helper
{
	
	/// <summary> 
    /// This class contains various static utility methods.
	/// </summary>	
	public class DebugUtils
	{
        /// <summary>
        /// Construct a String containing a hex-dump of a byte array
        /// </summary>
        /// <param name="bytes">the data to dump</param>
        /// <returns>a string containing the hexdump</returns>
        public static string HexDump(byte[] bytes)
        {
            return HexDump(null, bytes);
        }



        /// <summary>
        /// Construct a String containing a hex-dump of a byte array
        /// </summary>
        /// <param name="label">the label of the hexdump or null</param>
        /// <param name="bytes">the data to dump</param>
        /// <returns>a string containing the hexdump</returns>
        public static string HexDump(string label, byte[] bytes)
        {
            int modulo = 16;
            int brk = modulo / 2;
            int indent = (label == null) ? 0 : label.Length;

            StringBuilder sb = new StringBuilder(indent + 1);

            while (indent > 0)
            {
                sb.Append(" ");
                indent--;
            }

            string ind = sb.ToString();

            if (bytes == null)
            {
                return null;
            }

            sb = new StringBuilder(bytes.Length * 4);

            StringBuilder cb = new StringBuilder(16);
            bool nl = true;
            int i = 0;

            for (i = 1; i <= bytes.Length; i++)
            {
                // start of line?
                if (nl)
                {
                    nl = false;

                    if (i > 1)
                    {
                        sb.Append(ind);
                    }
                    else if (label != null)
                    {
                        sb.Append(label);
                    }

                    string ha = Convert.ToString(i - 1, 16);

                    for (int j = ha.Length; j <= 8; j++)
                    {
                        sb.Append("0");
                    }

                    sb.Append(ha).Append(" ");
                }

                sb.Append(" ");

                int c = (bytes[i - 1] & 0xFF);
                string hx = System.Convert.ToString(c, 16).ToUpper();

                if (hx.Length == 1)
                {
                    sb.Append("0");
                }

                sb.Append(hx);
                cb.Append((c < 0x21 || c > 0x7e) ? '.' : (char)(c));

                if ((i % brk) == 0)
                {
                    sb.Append(" ");
                }

                if ((i % modulo) == 0)
                {
                    sb.Append("|").Append(cb).Append("|\n");
                    nl = true;
                    cb = new StringBuilder(16);
                }
            }

            int mod = i % modulo;
            if (mod != 1)
            {
                if (mod == 0)
                {
                    mod = modulo;
                }
                while (mod <= modulo)
                {
                    sb.Append("   ");

                    if ((mod % brk) == 0)
                    {
                        sb.Append(" ");
                    }

                    mod++;
                }

                sb.Append("|").Append(cb).Append("|\n");
            }

            return sb.ToString();
        }
	}
}