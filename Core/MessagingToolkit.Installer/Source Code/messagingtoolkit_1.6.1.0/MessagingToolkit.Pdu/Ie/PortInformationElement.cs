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
    /// Port information element
    /// </summary>
    public class PortInformationElement : InformationElement
    {
        public const int Port16Bit = 0x05;

        virtual public int DestinationPort
        {
            get
            {
                // first 2 bytes of data
                byte[] data = Data;
                return (((data[0] & 0xFF) << 8) | (data[1] & 0xFF));
            }

        }
        virtual public int SourcePort
        {
            get
            {
                // next 2 bytes of data
                byte[] data = Data;
                return (((data[2] & 0xFF) << 8) | (data[3] & 0xFF));
            }
        }
        
        internal PortInformationElement(byte id, byte[] data)
            : base(id, data)
        {
            if (Identifier != Port16Bit)
            {
                throw new SystemException("Invalid identifier " + Identifier + " in data in: " + GetType().Name);
            }
            // iei
            // iel
            // dest(2 bytes)
            // src (2 bytes)
            if (data.Length != 4)
            {
                throw new SystemException("Invalid data length in: " + GetType().Name);
            }
        }

        internal PortInformationElement(int identifier, int destPort, int srcPort): base()
        {
            byte[] data = null;
            switch (identifier)
            {
                case Port16Bit:
                    data = new byte[4];
                    data[0] = (byte)(PduUtils.URShift((destPort & 0xFF00), 8));
                    data[1] = (byte)(destPort & 0xFF);
                    data[2] = (byte)(PduUtils.URShift((srcPort & 0xFF00), 8));
                    data[3] = (byte)(srcPort & 0xFF);
                    break;
                default:
                    throw new SystemException("Invalid identifier for " + GetType().Name);

            }
            Initialize((byte)(identifier & 0xFF), data);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(base.ToString());
            sb.Append("[Dst Port: ");
            sb.Append(DestinationPort);
            sb.Append(", Src Port: ");
            sb.Append(SourcePort);
            sb.Append("]");
            return sb.ToString();
        }
    }
}