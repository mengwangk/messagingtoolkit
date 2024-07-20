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
using System.Collections;

namespace MessagingToolkit.Pdu
{
    /// <summary>
    /// SMS delivered PDU
    /// </summary>
	public class SmsDeliveryPdu:Pdu
	{
        // ==================================================
        // TIMESTAMP
        // ==================================================
        private DateTime timestamp;		

		virtual public DateTime Timestamp
		{
			get
			{
				return this.timestamp;
			}
			
			set
			{
				this.timestamp = value;
			}			
		}

        virtual public string Timezone
        {
            get;
            set;
        }

		virtual public int TpMms
		{		
			set
			{
				CheckTpMti(new int[]{PduUtils.TpMtiSmsDeliver, PduUtils.TpMtiSmsStatusReport});
				
                // for SMS-DELIVER and SMS-STATUS-REPORT only
				SetFirstOctetField(PduUtils.TpMmsMask, value, new int[]{PduUtils.TpMmsMoreMessages, PduUtils.TpMmsNoMessages});
			}			
		}

		virtual public int TpSri
		{
			set
			{
				SetFirstOctetField(PduUtils.TpSriMask, value, new int[]{PduUtils.TpSriNoReport, PduUtils.TpSriReport});
			}			
		}

		// can only create via the factory
		internal SmsDeliveryPdu()
		{
		}
		
		
		public virtual bool HasTpMms()
		{
			CheckTpMti(new int[]{PduUtils.TpMtiSmsDeliver, PduUtils.TpMtiSmsStatusReport});
			// for SMS-DELIVER and SMS-STATUS-REPORT only
			return GetFirstOctetField(PduUtils.TpMmsMask) == PduUtils.TpMmsMoreMessages;
		}
		
		public virtual bool HasTpSri()
		{
			return GetFirstOctetField(PduUtils.TpSriMask) == PduUtils.TpSriReport;
		}
		
		protected internal override string PduSubclassInfo()
		{
			StringBuilder sb = new StringBuilder();
			
			// originator address		
			if (!string.IsNullOrEmpty(Address))
			{
				sb.Append("Originator Address: [Length: " + Address.Length + " (" + PduUtils.ByteToPdu((byte) Address.Length) + ")");
				sb.Append(", Type: " + PduUtils.ByteToPdu(AddressType) + " (" + PduUtils.ByteToBits((byte) AddressType) + ")");
				sb.Append(", Address: " + Address);
				sb.Append("]");
			}
			else
			{
				sb.Append("Originator Address: [Length: 0");
				sb.Append(", Type: " + PduUtils.ByteToPdu(AddressType) + " (" + PduUtils.ByteToBits((byte) AddressType) + ")");
				sb.Append("]");
			}
			
			sb.Append("\n");
			// protocol id
			sb.Append("TP-PID: " + PduUtils.ByteToPdu(ProtocolIdentifier) + " (" + PduUtils.ByteToBits((byte) ProtocolIdentifier) + ")");
			sb.Append("\n");
			// dcs
			sb.Append("TP-DCS: " + PduUtils.ByteToPdu(DataCodingScheme) + " (" + PduUtils.DecodeDataCodingScheme(this) + ") (" + PduUtils.ByteToBits((byte) DataCodingScheme) + ")");
			sb.Append("\n");
			// timestamp
			sb.Append("TP-SCTS: " + Timestamp.ToString("r") + this.Timezone);
			sb.Append("\n");
			return sb.ToString();
		}
	}
}