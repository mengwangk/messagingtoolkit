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
    /// Information element
    /// </summary>
    public class InformationElement
    {
        private byte identifier;
        private byte[] data;

        public int Identifier
        {
            get
            {
                return (identifier & 0xFF);
            }

        }
        public int Length
        {
            get
            {
                return data.Length;
            }

        }
        public byte[] Data
        {
            get
            {
                return data;
            }
        }       

        // iei
        // iel (implicit length of data)
        // ied (raw ie data)
        internal InformationElement(byte id, byte[] ieData)
        {
            Initialize(id, ieData);
        }

        internal InformationElement()
        {
        }

        // for outgoing messages
        internal virtual void Initialize(byte id, byte[] ieData)
        {
            this.identifier = id;
            this.data = ieData;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(GetType().Name + "[");
            sb.Append(PduUtils.ByteToPdu(identifier));
            sb.Append(", ");
            sb.Append(PduUtils.ByteToPdu(data.Length));
            sb.Append(", ");
            sb.Append(PduUtils.BytesToPdu(data));
            sb.Append("]");
            return sb.ToString();
        }
    }
}