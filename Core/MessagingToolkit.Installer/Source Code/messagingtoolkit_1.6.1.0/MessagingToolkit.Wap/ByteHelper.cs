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
    /// Helper class
    /// </summary>
    internal class ByteHelper
    {
        /// <summary>
        /// Converts a string to an array of bytes
        /// </summary>
        /// <param name="sourceString">The string to be converted</param>
        /// <returns>The new array of bytes</returns>
        public static byte[] GetBytes(string sourceString)
        {
            return UTF8Encoding.UTF8.GetBytes(sourceString);
            //if (IsUnicode(sourceString))
            //    return UTF8Encoding.UTF8.GetBytes(sourceString);
            //else
            //    return ASCIIEncoding.ASCII.GetBytes(sourceString);
        }

       
        /// <summary>
        /// Converts an array of bytes to an array of chars
        /// </summary>
        /// <param name="byteArray">The array of bytes to convert</param>
        /// <returns>The new array of chars</returns>
        public static char[] ToCharArray(byte[] byteArray)
        {
            return UTF8Encoding.UTF8.GetChars(byteArray);
            //Encoding enc = GetEncoding(byteArray);            
            //return enc.GetChars(byteArray);
        }


        /// <summary>
        /// Reads a number of characters from the current source Stream and writes the data to the target array at the specified index.
        /// </summary>
        /// <param name="sourceStream">The source Stream to read from.</param>
        /// <param name="target">Contains the array of characteres read from the source Stream.</param>
        /// <param name="start">The starting index of the target array.</param>
        /// <param name="count">The maximum number of characters to read from the source Stream.</param>
        /// <returns>
        /// The number of characters read. The number will be less than or equal to count depending on the data available in the source Stream. Returns -1 if the end of the stream is reached.
        /// </returns>
        public static Int32 ReadStream(Stream sourceStream, byte[] target, int start, int count)
        {
            // Returns 0 bytes if not enough space in target
            if (target.Length == 0)
                return 0;

            byte[] receiver = new byte[target.Length];
            int bytesRead = sourceStream.Read(receiver, start, count);

            // Returns -1 if EOF
            if (bytesRead == 0)
                return -1;

            for (int i = start; i < start + bytesRead; i++)
                target[i] = (byte)receiver[i];

            return bytesRead;
        }

        /// <summary>
        /// Determines whether the specified text is Unicode.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns>
        /// 	<c>true</c> if the specified text is Unicode; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsUnicode(string text)
        {
            /*
            foreach (char chr in input)
            {
                string str = chr.ToString();
                Encoding code = Encoding.GetEncoding("GB18030");
                if (code.GetByteCount(str) == 2)
                {
                    return false;
                }
            }
            return true;
            */


            int i = 0;
            for (i = 1; i <= text.Length; i++)
            {
                int code = Convert.ToInt32(Convert.ToChar(text.Substring(i - 1, 1)));
                if (code < 0 || code > 255)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Gets the encoding.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        public static Encoding GetEncoding(byte[] data)
        {
            // Default to ASCII
            Encoding enc = Encoding.ASCII;
            if ((data[0] == 0xef && data[1] == 0xbb && data[2] == 0xbf))
            {
                enc = Encoding.UTF8;
            }
            else if (data[0] == 0xff && data[1] == 0xfe)
            {
                // ucs-2le, ucs-4le, and ucs-16le
                enc = Encoding.Unicode;

            }
            else if (data[0] == 0xfe && data[1] == 0xff)
            {
                // utf-16 and ucs-2
                enc = Encoding.Unicode;
            }
            else if (data[0] == 0 && data[1] == 0 && data[2] == 0xfe && data[3] == 0xff)
            {
                // ucs-4
                enc = Encoding.Unicode;
            }
            return enc;
        } 
    
    }

}
