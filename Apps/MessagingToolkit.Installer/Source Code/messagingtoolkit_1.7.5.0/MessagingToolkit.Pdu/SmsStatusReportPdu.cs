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
    /// SMS status report PDU
    /// </summary>
	public class SmsStatusReportPdu:Pdu
	{

        // ==================================================
        // MESSAGE REFERENCE
        // ==================================================
        // usually just 0x00 to let MC supply
        private int messageReference = 0x00;

        // ==================================================
        // STATUS
        // ==================================================
        private int status = 0x00;

        // ==================================================
        // TIMESTAMP
        // ==================================================
        private DateTime timestamp;

        // ==================================================
        // DISCHARGE TIME
        // ==================================================
        private DateTime dischargeTime;
		

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
		virtual public int MessageReference
		{
			get
			{
				return messageReference;
			}
			
			set
			{
				messageReference = value;
			}
			
		}
		virtual public int Status
		{
			get
			{
				return status;
			}
			
			set
			{
				this.status = value;
			}			
		}

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

		virtual public DateTime DischargeTime
		{
			get
			{
				return dischargeTime;
			}
			
			set
			{
				this.dischargeTime = value;
			}			
		}

		// can only create via the factory
		internal SmsStatusReportPdu()
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
			// message reference
			sb.Append("Message Reference: " + PduUtils.ByteToPdu(MessageReference));
			sb.Append("\r\n");
			
			// destination address
            if (!string.IsNullOrEmpty(Address))
			{
				sb.Append("Destination Address: [Length: " + Address.Length + " (" + PduUtils.ByteToPdu((byte) Address.Length) + ")");
				sb.Append(", Type: " + PduUtils.ByteToPdu(AddressType) + " (" + PduUtils.ByteToBits((byte) AddressType) + ")");
				sb.Append(", Address: " + Address);
				sb.Append("]");
			}
			else
			{
				sb.Append("Destination Address: [Length: 0");
				sb.Append(", Type: " + PduUtils.ByteToPdu(AddressType) + " (" + PduUtils.ByteToBits((byte) AddressType) + ")");
				sb.Append("]");
			}
			
			sb.Append("\r\n");
			// timestamp
			sb.Append("TP-SCTS: " + Timestamp.ToString("r") + this.Timezone);
			sb.Append("\r\n");
			// discharge time
			sb.Append("Discharge Time: " + DischargeTime.ToString("r") + this.Timezone);
			sb.Append("\r\n");
			// status
			sb.Append("Status: " + PduUtils.ByteToPdu(Status));
			sb.Append("\r\n");
			return sb.ToString();
		}
	}
}