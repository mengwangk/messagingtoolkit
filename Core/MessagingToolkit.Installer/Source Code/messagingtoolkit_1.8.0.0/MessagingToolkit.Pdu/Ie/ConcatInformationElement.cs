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

namespace MessagingToolkit.Pdu.Ie
{
    /// <summary>
    /// Concatenate message information element
    /// </summary>
    public class ConcatInformationElement : InformationElement
    {
        private const int ConcatIeLength16Bit = 6;
        private const int ConcateIeLength8Bit = 5;
        public const int Concat8BitRef = 0x00;
        public const int Concat16BitRef = 0x08;

        private static int defaultConcatType = Concat8BitRef;
        private static int defaultConcatLength = ConcateIeLength8Bit;

        public static int DefaultConcatLength
        {
            get
            {
                return defaultConcatLength;
            }

        }
        public static int DefaultConcatType
        {
            get
            {
                return defaultConcatType;
            }

            set
            {
                switch (value)
                {

                    case Concat8BitRef:
                        defaultConcatType = Concat8BitRef;
                        defaultConcatLength = ConcateIeLength8Bit;
                        break;

                    case Concat16BitRef:
                        defaultConcatType = Concat16BitRef;
                        defaultConcatLength = ConcatIeLength16Bit;
                        break;

                    default:
                        throw new Exception("Invalid Concat type");

                }
            }
        }

        virtual public int MpRefNo
        {
            get
            {
                // this is 8-bit in 0x00 and 16-bit in 0x08 
                byte[] data = Data;
                if (Identifier == Concat8BitRef)
                {
                    return (data[0] & (0xFF));
                }
                else if (Identifier == Concat16BitRef)
                {
                    return ((data[0] << 8) | data[1]) & (0xFFFF);
                }
                throw new Exception("Invalid identifier");
            }

            set
            {
                // this is 8-bit in 0x00 and 16-bit in 0x08 
                byte[] data = Data;
                if (Identifier == Concat8BitRef)
                {
                    data[0] = (byte)(value & (0xFF));
                }
                else if (Identifier == Concat16BitRef)
                {
                    data[0] = (byte)((PduUtils.URShift(value, 8)) & (0xFF));
                    data[1] = (byte)((value) & (0xFF));
                }
                else
                {
                    throw new Exception("Invalid identifier");
                }
            }
        }

        virtual public int MpMaxNo
        {
            get
            {
                byte[] data = Data;
                if (Identifier == Concat8BitRef)
                {
                    return (data[1] & (0xFF));
                }
                else if (Identifier == Concat16BitRef)
                {
                    return (data[2] & (0xFF));
                }
                throw new Exception("Invalid identifier");
            }

            set
            {
                byte[] data = Data;
                if (Identifier == Concat8BitRef)
                {
                    data[1] = (byte)(value & 0xFF);
                }
                else if (Identifier == Concat16BitRef)
                {
                    data[2] = (byte)(value & 0xFF);
                }
                else
                {
                    throw new Exception("Invalid identifier");
                }
            }
        }

        virtual public int MpSeqNo
        {
            get
            {
                byte[] data = Data;
                if (Identifier == Concat8BitRef)
                {
                    return (data[2] & (0xFF));
                }
                else if (Identifier == Concat16BitRef)
                {
                    return (data[3] & (0xFF));
                }
                throw new Exception("Invalid identifier");
            }

            set
            {
                byte[] data = Data;
                if (Identifier == Concat8BitRef)
                {
                    data[2] = (byte)(value & (0xFF));
                }
                else if (Identifier == Concat16BitRef)
                {
                    data[3] = (byte)(value & (0xFF));
                }
                else
                {
                    throw new Exception("Invalid identifier");
                }
            }
        }


        internal ConcatInformationElement(byte identifier, byte[] data): base(identifier, data)
        {
            if (Identifier == Concat8BitRef)
            {
                // iei
                // iel
                // ref
                // max
                // seq
                if (data.Length != 3)
                {
                    throw new Exception("Invalid data length in: " + GetType().Name);
                }
            }
            else if (Identifier == Concat16BitRef)
            {
                // iei
                // iel
                // ref(2 bytes)
                // max
                // seq
                if (data.Length != 4)
                {
                    throw new Exception("Invalid data length in: " + GetType().Name);
                }
            }
            else
            {
                throw new Exception("Invalid identifier in data in: " + GetType().Name);
            }
            Validate();
        }

        internal ConcatInformationElement(int identifier, int mpRefNo, int mpMaxNo, int mpSeqNo): base()
        {
            byte[] data = null;
            switch (identifier)
            {
                case Concat8BitRef:
                    data = new byte[3];
                    data[0] = (byte)(mpRefNo & 0xFF);
                    data[1] = (byte)(mpMaxNo & 0xFF);
                    data[2] = (byte)(mpSeqNo & 0xFF);
                    break;

                case Concat16BitRef:
                    data = new byte[4];
                    data[0] = (byte)(PduUtils.URShift((mpRefNo & 0xFF00), 8));
                    data[1] = (byte)(mpRefNo & 0xFF);
                    data[2] = (byte)(mpMaxNo & 0xFF);
                    data[3] = (byte)(mpSeqNo & 0xFF);
                    break;

                default:
                    throw new Exception("Invalid identifier for " + GetType().Name);

            }
            Initialize((byte)(identifier & 0xFF), data);
            Validate();
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(base.ToString());
            sb.Append("[MpRefNo: ");
            sb.Append(MpRefNo);
            sb.Append(", MpMaxNo: ");
            sb.Append(MpMaxNo);
            sb.Append(", MpSeqNo: ");
            sb.Append(MpSeqNo);
            sb.Append("]");
            return sb.ToString();
        }

        private void Validate()
        {
            if (MpMaxNo == 0)
            {
                throw new Exception("mpMaxNo must be > 0");
            }
            if (MpSeqNo == 0)
            {
                throw new Exception("mpSeqNo must be > 0");
            }
        }
    }
}