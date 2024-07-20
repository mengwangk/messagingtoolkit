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
using System.Text.RegularExpressions;
using System.IO;

namespace MessagingToolkit.Core.Mobile.Message
{
    /// <summary>
    /// Provide encoding and decoding of Quoted-Printable.
    /// </summary>
    public class QuotedPrintable
    {
        private QuotedPrintable()
        {
        }

        /// <summary>
        /// // so including the = connection, the length will be 76
        /// </summary>
        private const int Rfc1521MaxCharsPerLine = 75;

        /// <summary>
        /// Return quoted printable string with 76 characters per line.
        /// </summary>
        /// <param name="textToEncode"></param>
        /// <returns></returns>
        public static string Encode(string textToEncode)
        {
            if (string.IsNullOrEmpty(textToEncode)) return textToEncode;
               
            return Encode(textToEncode, Rfc1521MaxCharsPerLine);
        }

        /// <summary>
        /// Encode8s the bit.
        /// </summary>
        /// <param name="textToEncode">The text to encode.</param>
        /// <returns></returns>
        public static string Encode8Bit(string textToEncode)
        {
            byte[] encodedBytes = Encoding.GetEncoding("iso-8859-1").GetBytes(textToEncode);
            return BitConverter.ToString(encodedBytes).Replace("-", string.Empty);            
        }

        private static string Encode(string textToEncode, int charsPerLine)
        {
            if (string.IsNullOrEmpty(textToEncode)) return textToEncode;

            if (charsPerLine <= 0)
                throw new ArgumentOutOfRangeException();

            return FormatEncodedString(EncodeString(textToEncode), charsPerLine);
        }
        /// <summary>
        /// Return quoted printable string, all in one line.
        /// </summary>
        /// <param name="textToEncode"></param>
        /// <returns></returns>
        public static string EncodeString(string textToEncode)
        {
            if (string.IsNullOrEmpty(textToEncode)) return textToEncode;

            byte[] bytes = Encoding.UTF8.GetBytes(textToEncode);
            StringBuilder builder = new StringBuilder();
            foreach (byte b in bytes)
            {
                if (b != 0)
                    if ((b < 32) || (b > 126))
                        builder.Append(String.Format("={0}", b.ToString("X2")));
                    else
                    {
                        switch (b)
                        {
                            case 13:
                                builder.Append("=0D");
                                break;
                            case 10:
                                builder.Append("=0A");
                                break;
                            case 61:
                                builder.Append("=3D");
                                break;
                            default:
                                builder.Append(Convert.ToChar(b));
                                break;
                        }
                    }
            }

            return builder.ToString();
        }

        private static string FormatEncodedString(string qpstr, int maxcharlen)
        {
            if (qpstr == null)
                throw new ArgumentNullException();

            StringBuilder builder = new StringBuilder();
            char[] charArray = qpstr.ToCharArray();
            int i = 0;
            foreach (char c in charArray)
            {
                builder.Append(c);
                i++;
                if (i == maxcharlen)
                {
                    builder.AppendLine("=");
                    i = 0;
                }
            }

            return builder.ToString();
        }

        static string HexDecoderEvaluator(Match m)
        {
            if (String.IsNullOrEmpty(m.Value))
                return null;

            CaptureCollection captures = m.Groups[3].Captures;
            byte[] bytes = new byte[captures.Count];

            for (int i = 0; i < captures.Count; i++)
            {
                bytes[i] = Convert.ToByte(captures[i].Value, 16);
            }

            return UTF8Encoding.UTF8.GetString(bytes);
        }

        static string HexDecoder(string line)
        {
            if (line == null)
                throw new ArgumentNullException();

            Regex re = new Regex("((\\=([0-9A-F][0-9A-F]))*)", RegexOptions.IgnoreCase);
            return re.Replace(line, new MatchEvaluator(HexDecoderEvaluator));
        }


        public static string Decode(string encodedText)
        {
            if (encodedText == null)
                throw new ArgumentNullException();

            if (encodedText == "")
                return "";

            using (StringReader sr = new StringReader(encodedText))
            {
                StringBuilder builder = new StringBuilder();
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    if (line.EndsWith("="))
                        builder.Append(line.Substring(0, line.Length - 1));
                    else
                        builder.Append(line);
                }

                return HexDecoder(builder.ToString());
            }
        }

        /// <summary>
        /// Check if the string needs to be quoted
        /// </summary>
        /// <param name="s">Value to check</param>
        /// <returns>true or false</returns>
        public static bool NeedQuotedPrintable(string s)
        {
            if (string.IsNullOrEmpty(s)) return false;

            UTF8Encoding byteConverter = new UTF8Encoding();
            byte[] buffer = byteConverter.GetBytes(s);
            foreach (byte b in buffer)
            {
                if ((b > 126) || (b < 33))
                    return true;
            }
            return false;

        }
    }
}
