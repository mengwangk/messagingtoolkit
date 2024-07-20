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
using System.Net;
using System.IO;

using MessagingToolkit.Wap.Wtp;
using MessagingToolkit.Wap.Log;
using MessagingToolkit.Wap.Helper;
using MessagingToolkit.Wap.Wsp.Pdu;


namespace MessagingToolkit.Wap.Wsp
{

    /// <summary>
    /// This class can be used to extract WSP Primitives from a byte array.
    /// The decoder maintains a pointer (index) into the byte array
    /// which is increased when calling one of the getter-methods.
    /// E.g. calling getUint8() to read a octet will increase the index by one.
    /// </summary>
    public class WSPDecoder
    {
        virtual public byte Octet
        {
            get
            {
                return data[offset++];
            }

        }
        virtual public short UInt8
        {
            get
            {
                return (short)(data[offset++] & 0xff); // Make it unsigned
            }

        }
        virtual public int UInt16
        {
            get
            {
                return (UInt8 << 8) | UInt8;
            }

        }
        virtual public int UInt32
        {
            get
            {
                return (UInt16 << 16) | UInt16;
            }

        }
        virtual public long UIntVar
        {
            get
            {
                int octet = 0;
                long result = 0;

                do
                {
                    octet = Octet;
                    result <<= 7;
                    result |= (octet & 0x7f);
                }
                while ((octet & 0x80) != 0);

                return result;
            }

        }
        virtual public string CString
        {
            get
            {
                MemoryStream outStream = new MemoryStream();

                for (int i = Octet; i != 0; i = Octet)
                {
                    outStream.WriteByte((byte)i);
                }

                string str = new string(ByteHelper.ToCharArray(outStream.ToArray()));

                return ("".Equals(str) ? null : str);
            }

        }
        virtual public string TextString
        {
            get
            {
                int o = Octet;
                if (o != 127)
                {
                    Seek(-1);
                }
                return CString;
            }

        }
        virtual public bool EOF
        {
            get
            {
                return offset >= data.Length;
            }

        }
        virtual public int RemainingOctets
        {
            get
            {
                int remain = data.Length - offset;
                return remain < 0 ? 0 : remain;
            }

        }
        /// <summary> Returns a short- or long-integer </summary>
        virtual public long IntegerValue
        {
            get
            {
                long len = UInt8;
                long ret = 0;
                if ((len & 0x80) != 0)
                {
                    // Short-Integer
                    ret = len & 0x7f;
                }
                else
                {
                    Seek(-1);
                    ret = LongInteger;
                }
                return ret;
            }

        }
        virtual public long ValueLength
        {
            get
            {
                long len = 0;
                len = UInt8;
                if (len == 31)
                {
                    len = UIntVar;
                }
                return len;
            }

        }
        virtual public long LongInteger
        {
            get
            {
                long len = UInt8;
                long ret = 0;
                while (len > 0)
                {
                    len--;
                    ret |= ((long)UInt8 << (int)(len * 8));
                }
                return ret;
            }

        }
        virtual public DateTime DateValue
        {
            get
            {
                long delta = LongInteger;
                return new DateTime(delta * 1000);
            }

        }
        private byte[] data;
        private int offset;

        /// <summary>
        /// Initializes a new instance of the <see cref="WSPDecoder"/> class.
        /// </summary>
        private WSPDecoder()
        {
        }

        /// <summary>
        /// Construct a new WSP Decoder
        /// </summary>
        /// <param name="data">the data to operate on</param>
        public WSPDecoder(byte[] data)
        {
            this.data = data;
        }

        public virtual byte[] GetBytes(int count)
        {
            byte[] ret = new byte[count];
            Array.Copy(data, offset, ret, 0, ret.Length);
            offset += count;

            return ret;
        }

        /// <summary> Modify the array index</summary>
        /// <param name="offset">the offset (might be negative to seek backwards)
        /// </param>
        /// <returns> the new position of the index
        /// </returns>
        public virtual int Seek(int offset)
        {
            this.offset += offset;
            if (this.offset < 0)
            {
                this.offset = 0;
            }
            return this.offset;
        }

        public virtual int Pos(int position)
        {
            int last = offset;
            offset = position;
            return last;
        }

        public virtual int Pos()
        {
            return offset;
        }

        public virtual string GetString(int length)
        {
            return new string(ByteHelper.ToCharArray(data), offset, length);
        }

        public virtual short GetShortInteger()
        {
            return (short)(UInt8 & 0x7f);
        }


        /// <summary>isShortInteger tests if the next value is a short int.</summary>
        public virtual bool IsShortInteger()
        {
            return (data[offset] < 0); // n.b. bytes are signed
        }

        /// <summary>isString tests if the next value is a string.</summary>
        public virtual bool IsString()
        {
            return (data[offset] >= 0x20 && data[offset] < 0x80);
        }
    }
}
