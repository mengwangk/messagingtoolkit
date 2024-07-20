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

namespace MessagingToolkit.Wap.Wsp.Headers
{
    /// <summary>
    /// This class contains static methods used to encode primitive data types used
    /// for encoding WSP data units
    /// </summary>
    public class Encoding
    {
        private static readonly byte[] EmptyValue = new byte[0];
        private static readonly byte[] EmptyString = new byte[] { 0 };


        /// <summary>
        /// Private constructor
        /// </summary>
        private Encoding()
        {
        }

        public static byte[] TextString(string value)
        {
            byte[] data = null;
            if (value == null)
            {
                data = EmptyValue;
            }
            else if ("".Equals(value))
            {
                data = EmptyString;
            }
            else
            {
                byte[] b = ByteHelper.GetBytes(value);

                if (b[0] > 127)
                {
                    data = new byte[b.Length + 2];
                    data[0] = 127;
                    Array.Copy(b, 0, data, 1, b.Length);
                }
                else
                {
                    data = new byte[b.Length + 1];
                    Array.Copy(b, 0, data, 0, b.Length);
                }
            }
            return data;
        }

        public static byte[] ExtensionMedia(string value)
        {
            byte[] data = null;

            if (value == null)
            {
                data = EmptyValue;
            }
            else
            {
                byte[] b = ByteHelper.GetBytes(value);
                data = new byte[b.Length + 1];
                Array.Copy(b, 0, data, 0, b.Length);
            }
            return data;
        }

        public static byte[] TokenText(string value)
        {
            int len = 1;
            byte[] data = null;

            if (value != null)
            {
                len += value.Length;
                data = new byte[len];

                byte[] sb = ByteHelper.GetBytes(value);
                Array.Copy(sb, 0, data, 0, len - 1);
            }
            else
            {
                data = EmptyString;
            }

            return data;
        }

        public static byte[] ShortInteger(short value)
        {
            byte[] data = null;
            data = new byte[1];
            data[0] = (byte)((Math.Abs(value) & 0xff) | 0x80);
            return data;
        }

        public static byte[] LongInteger(long value)
        {
            byte[] data = null;
            value = Math.Abs(value);

            int octets = (int)Math.Ceiling(Convert.ToString(value, 2).Length / 8.0);
            data = new byte[octets + 1];
            data[0] = (byte)octets;

            while (octets > 0)
            {
                data[octets] = (byte)(value & 0xff);
                value >>= 8;
                octets--;
            }
            return data;
        }

        public static byte[] UIntVar(int value)
        {
            byte[] data = null;
            byte[] b = new byte[5];
            int i = 5;

            do
            {
                int x = value & 0x7f;
                value >>= 7;

                if (i != 5)
                {
                    x |= 0x80;
                }

                b[--i] = (byte)(x & 0xff);
            }
            while ((value > 0) && (i > 0));

            data = new byte[5 - i];
            Array.Copy(b, i, data, 0, data.Length);

            return data;
        }

        public static byte[] QuotedString(string value)
        {
            byte[] data = null;

            if (value == null)
            {
                data = new byte[] { (byte)(0x22), (byte)(0x00) };
            }
            else
            {
                byte[] b = ByteHelper.GetBytes(value);
                data = new byte[b.Length + 2];
                data[0] = (byte)(0x22);
                Array.Copy(b, 0, data, 1, b.Length);
            }

            return data;
        }

        public static byte[] DateValue(ref DateTime date)
        { 
            long secs = date.Ticks / 1000;
            return LongInteger(secs);
        }

        public static byte[] QualityFactor(float value)
        {
            int qf = (int)(value * 1000) % 1000;

            if ((qf % 10) != 0)
            {
                qf += 100;
            }
            else
            {
                qf = (qf / 10) + 1;
            }

            return UIntVar(qf);
        }

        public static byte[] VersionValue(string version)
        {
            // TODO: Try to extract major and minor from string so that
            // a shortInteger can be used instead of a text-string...
            return TextString(version);
        }

        public static byte[] VersionValue(int major, int minor)
        {
            if ((major < 1) || (major > 7) || (minor < 0) || (minor > 14))
            {
                StringBuilder sb = new StringBuilder(major).Append('.').Append(minor);
                return TextString(sb.ToString());
            }

            if (minor == 0)
            {
                minor = 15;
            }

            short i = (short)(((major & 0x0F) << 8) | (minor & 0x0f));

            return ShortInteger(i);
        }

        public static byte[] ValueLength(byte[] tt)
        {
            return ValueLength((tt == null) ? 0 : tt.Length);
        }

        public static byte[] ValueLength(int i)
        {
            byte[] ret = null;

            if (i == 0)
            {
                ret = EmptyValue;
            }
            else if (i <= 30)
            {
                ret = new byte[] { (byte)i };
            }
            else
            {
                byte[] ui = UIntVar(i);
                ret = new byte[ui.Length + 1];
                ret[0] = 31;
                Array.Copy(ui, 0, ret, 1, ui.Length);
            }

            return ret;
        }

        public static byte[] EncodeHeader(short wk, byte[] data)
        {
            byte[] kb = ShortInteger(wk);
            byte[] ret = new byte[data.Length + kb.Length];
            Array.Copy(kb, 0, ret, 0, kb.Length);
            Array.Copy(data, 0, ret, kb.Length, data.Length);

            return ret;
        }

        public static byte[] EncodeHeader(string key, byte[] data)
        {
            byte[] kb = TextString(key);
            byte[] ret = new byte[kb.Length + data.Length];
            Array.Copy(kb, 0, ret, 0, kb.Length);
            Array.Copy(data, 0, ret, kb.Length, data.Length);
            return ret;
        }

        public static byte[] IntegerValue(long l)
        {
            return (l < 0x80) ? Encoding.ShortInteger((short)l) : Encoding.LongInteger(l);
        }

        public static byte[] UriValue(string value)
        {
            return TextString((value == null) ? null : value.Trim());
        }
    }
}
