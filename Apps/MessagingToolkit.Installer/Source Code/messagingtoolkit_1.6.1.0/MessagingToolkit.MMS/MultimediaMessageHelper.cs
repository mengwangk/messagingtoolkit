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
using System.Text;
using System.IO;
using System.Collections.Generic;


namespace MessagingToolkit.MMS
{
    /// <summary>
    /// Helper class to store information about encodings
    /// with a preamble
    /// </summary>
    public class PreambleInfo
    {
        protected Encoding encoding;
        protected byte[] preamble;

        /// <summary>
        /// Property Encoding (Encoding).
        /// </summary>
        public Encoding Encoding
        {
            get { return this.encoding; }
        }


        /// <summary>
        /// Property Preamble (byte[]).
        /// </summary>
        public byte[] Preamble
        {
            get { return this.preamble; }
        }
        
        /// <summary>
        /// Constructor with preamble and encoding
        /// </summary>
        /// <param name="encoding"></param>
        /// <param name="preamble"></param>
        public PreambleInfo(Encoding encoding, byte[] preamble)
        {
            this.encoding = encoding;
            this.preamble = preamble;
        }
    }

    /// <summary>
    /// Helper class
    /// </summary>
    public class MultimediaMessageHelper
    {
        // The list of encodings with a preamble,
        // sorted longest preamble first.
        protected static SortedList<int, PreambleInfo> preambles = null;

        // Maximum length of all preamles
        protected static int maxPreambleLength = 0;       

        /// <summary>
        /// Converts a string to an array of bytes
        /// </summary>
        /// <param name="sourceString">The string to be converted</param>
        /// <returns>The new array of bytes</returns>
        public static byte[] GetBytes(string sourceString)
        {
            //if (IsUnicode(sourceString))
                //return Encoding.UTF8.GetBytes(sourceString);
                //return Encoding.GetEncoding("UTF-16BE").GetBytes(sourceString);      
            //else
                return Encoding.UTF8.GetBytes(sourceString);
               //return Encoding.ASCII.GetBytes(sourceString);
        }


        /// <summary>
        /// Converts a byte array to string
        /// </summary>
        /// <param name="sourceBytes">The source bytes.</param>
        /// <returns>The new array of bytes</returns>
        public static string GetString(byte[] sourceBytes)
        {
            return Encoding.UTF8.GetString(sourceBytes);           
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


        /***
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
        ***/


        /// <summary>
        /// Read the contents of a text file as a string. Scan for a preamble first.
        /// If a preamble is found, the corresponding encoding is used.
        /// If no preamble is found, the supplied defaultEncoding is used.
        /// </summary>
        /// <param name="filename">The name of the file to read</param>
        /// <param name="defaultEncoding">The encoding to use if no preamble present</param>
        /// <param name="usedEncoding">The actual encoding used</param>
        /// <returns>The contents of the file as a string</returns>
        public static string ReadAllText(string filename, Encoding defaultEncoding, out Encoding usedEncoding)
        {
            // Read the contents of the file as an array of bytes
            byte[] bytes = File.ReadAllBytes(filename);

            // Detect the encoding of the file:
            usedEncoding = DetectEncoding(bytes);

            // If none found, use the default encoding.
            // Otherwise, determine the length of the encoding markers in the file
            int offset;
            if (usedEncoding == null)
            {
                offset = 0;
                usedEncoding = defaultEncoding;
            }
            else
            {
                offset = usedEncoding.GetPreamble().Length;
            }
            // Now interpret the bytes according to the encoding,
            // skipping the preample (if any)
            return usedEncoding.GetString(bytes, offset, bytes.Length - offset);
        }



        /// <summary>
        /// Detect the encoding in an array of bytes.
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns>The encoding found, or null</returns>
        public static Encoding DetectEncoding(byte[] bytes)
        {
            // Scan for encodings if we haven't done so
            if (preambles == null)
                ScanEncodings();

            // Try each preamble in turn
            foreach (PreambleInfo info in preambles.Values)
            {
                // Match all bytes in the preamble
                bool match = true;
                if (bytes.Length >= info.Preamble.Length)
                {
                    for (int i = 0; i < info.Preamble.Length; i++)
                    {
                        if (bytes[i] != info.Preamble[i])
                        {
                            match = false;
                            break;
                        }
                    }
                    if (match)
                    {
                        return info.Encoding;
                    }
                }
            }
            return Encoding.ASCII;
        }



        /// <summary>
        /// Detect the encoding of a file. Reads just enough of
        /// the file to be able to detect a preamble.
        /// </summary>
        /// <param name="filename">The path name of the file</param>
        /// <returns>The encoding detected, or null if no preamble found</returns>
        public static Encoding DetectEncoding(string filename)
        {
            // Scan for encodings if we haven't done so
            if (preambles == null)
                ScanEncodings();
            using (FileStream stream = File.OpenRead(filename))
            {
                // Never read more than the length of the file
                // or the maximum preamble length
                long n = stream.Length;
                // No bytes? No encoding!
                if (n == 0)
                    return null;
                // Read the minimum amount necessary
                if (n > maxPreambleLength)
                    n = maxPreambleLength;
                byte[] bytes = new byte[n];
                stream.Read(bytes, 0, (int)n);

                // Detect the encoding from the byte array
                return DetectEncoding(bytes);
            }
        }


        /// <summary>
        /// Loop over all available encodings and store those
        /// with a preamble in the _preambles list.
        /// The list is sorted by preamble length,
        /// longest preamble first. This prevents
        /// a short preamble 'masking' a longer one
        /// later in the list.
        /// </summary>
        protected static void ScanEncodings()
        {
            // Create a new sorted list of preambles
            preambles = new SortedList<int, PreambleInfo>();
            // Loop over all encodings
            foreach (EncodingInfo encodingInfo in Encoding.GetEncodings())
            {
                // Do we have a preamble?
                byte[] preamble = encodingInfo.GetEncoding().GetPreamble();
                if (preamble.Length > 0)
                {
                    // Add it to the collection, inversely sorted by preamble length
                    // (and code page, to keep the keys unique)
                    preambles.Add(-(preamble.Length * 1000000 + encodingInfo.CodePage),
                       new PreambleInfo(encodingInfo.GetEncoding(), preamble));

                    // Update the maximum preamble length if this one's longer
                    if (preamble.Length > maxPreambleLength)
                    {
                        maxPreambleLength = preamble.Length;
                    }
                }
            }
        }
    }
}


