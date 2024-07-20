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
using System.IO;

using MessagingToolkit.Pdu.Ie;

namespace MessagingToolkit.Pdu
{
    /// <summary>
    /// SMS submit PDU
    /// </summary>
	public class SmsSubmitPdu:Pdu
	{
        // ==================================================
        // MESSAGE REFERENCE
        // ==================================================
        // usually just 0x00 to let MC supply
        private int messageReference = 0x00;

        // ==================================================
        // VALIDITY PERIOD
        // ==================================================
        // which one is used depends of validity period format (TP-VPF)
        private int validityPeriod = -1;
        private DateTime validityPeriodTimeStamp;

        /// <summary>
        /// Reply duplicate flag
        /// </summary>
        /// <value>Reply duplicate flag</value>
		virtual public int TpRd
		{	
			set
			{
				SetFirstOctetField(PduUtils.TpRdMask, value, new int[]{PduUtils.TpRdAcceptDuplicates, PduUtils.TpRdRejectDuplicates});
			}
			
		}
        /// <summary>
        /// Gets or sets the validity period format
        /// </summary>
        /// <value>Validity period format</value>
		virtual public int TpVpf
		{
			get
			{
				return GetFirstOctetField(PduUtils.TpVpfMask);
			}
			
			set
			{
                SetFirstOctetField(PduUtils.TpVpfMask, value, new int[] { PduUtils.TpVpfInteger, PduUtils.TpVpfNone, PduUtils.TpVpfTimestamp });
			}			
		}


        /// <summary>
        /// Gets or sets the reply path
        /// </summary>
        /// <value>Reply path</value>
        virtual public int TpRp
        {
            get
            {
                return GetFirstOctetField(PduUtils.TpRpMask);
            }

            set
            {
                SetFirstOctetField(PduUtils.TpRpMask, value, new int[] { PduUtils.TpRpWithRp, PduUtils.TpRpNoRp });
            }

        }

        /// <summary>
        /// Sets the tp status report request
        /// </summary>
        /// <value>Status report request</value>
		virtual public int TpSrr
		{
			set
			{
				SetFirstOctetField(PduUtils.TpSrrMask, value, new int[]{PduUtils.TpSrrNoReport, PduUtils.TpSrrReport});
			}
			
		}
        /// <summary>
        /// Gets or sets the message reference.
        /// </summary>
        /// <value>The message reference.</value>
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
        /// <summary>
        /// Gets or sets the validity period.
        /// </summary>
        /// <value>The validity period.</value>
		virtual public int ValidityPeriod
		{
			get
			{
				return validityPeriod;
			}
			
			set
			{
				this.validityPeriod = value;
			}
			
		}
        /// <summary>
        /// Gets or sets the validity timestamp.
        /// </summary>
        /// <value>The validity timestamp.</value>
		virtual public DateTime ValidityTimestamp
		{
            get
            {
                return validityPeriodTimeStamp;
            }
			set
			{
				this.validityPeriodTimeStamp = value;
			}			
		}

        /// <summary>
        /// Gets or sets the timezone.
        /// </summary>
        /// <value>The timezone.</value>
        virtual public string Timezone
        {
            get;
            set;
        }

        /// <summary>
        /// Determines whether [has tp rd].
        /// </summary>
        /// <returns>
        /// 	<c>true</c> if [has tp rd]; otherwise, <c>false</c>.
        /// </returns>
		public virtual bool HasTpRd()
		{
			return GetFirstOctetField(PduUtils.TpRdMask) == PduUtils.TpRdRejectDuplicates;
		}

        /// <summary>
        /// Determines whether [has tp VPF].
        /// </summary>
        /// <returns>
        /// 	<c>true</c> if [has tp VPF]; otherwise, <c>false</c>.
        /// </returns>
		public virtual bool HasTpVpf()
		{
            return GetFirstOctetField(PduUtils.TpVpfMask) != PduUtils.TpVpfNone;
		}

        /// <summary>
        /// Determines whether [has tp SRR].
        /// </summary>
        /// <returns>
        /// 	<c>true</c> if [has tp SRR]; otherwise, <c>false</c>.
        /// </returns>
		public virtual bool HasTpSrr()
		{
			return GetFirstOctetField(PduUtils.TpSrrMask) == PduUtils.TpSrrReport;
		}

        /// <summary>
        /// Determines whether [has tp rp].
        /// </summary>
        /// <returns>
        /// 	<c>true</c> if [has tp rp]; otherwise, <c>false</c>.
        /// </returns>
        public virtual bool HasTpRp()
        {
            return GetFirstOctetField(PduUtils.TpRpMask) == PduUtils.TpRpWithRp;
        }

        /// <summary>
        /// Generate PDU information
        /// </summary>
        /// <returns>PDU information</returns>
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
			// protocol id
			sb.Append("TP-PID: " + PduUtils.ByteToPdu(ProtocolIdentifier) + " (" + PduUtils.ByteToBits((byte) ProtocolIdentifier) + ")");
			sb.Append("\r\n");
			// dcs
			sb.Append("TP-DCS: " + PduUtils.ByteToPdu(DataCodingScheme) + " (" + PduUtils.DecodeDataCodingScheme(this) + ") (" + PduUtils.ByteToBits((byte) DataCodingScheme) + ")");
			sb.Append("\r\n");
			// validity period
			switch (TpVpf)
			{
				
				case PduUtils.TpVpfInteger: 
					sb.Append("TP-VPF: " + ValidityPeriod + " hours");
					break;
				
				case PduUtils.TpVpfTimestamp: 
					sb.Append("TP-VPF: " + ValidityTimestamp.ToString("r") + this.Timezone);
					break;
				}
			sb.Append("\r\n");
			return sb.ToString();
		}

        /// <summary>
        /// Adds the reply path.
        /// </summary>
        /// <param name="address">The reply address</param>
        /// <param name="addressType">Type of the address</param>
        public void AddReplyPath(string address, int addressType)
        {
            if (string.IsNullOrEmpty(address)) return;
            this.TpRp = PduUtils.TpRpWithRp;
            MemoryStream baos = new MemoryStream();
            WriteBcdAddress(baos, address, addressType, address.Length);

            byte[] addressInfo = baos.ToArray();
            byte[] replyPath = new byte[addressInfo.Length + 1];
            replyPath[0] = Convert.ToByte(addressInfo.Length);
            Array.Copy(addressInfo, 0, replyPath, 1, addressInfo.Length);
            InformationElement infoElement = InformationElementFactory.CreateInformationElement(PduUtils.ReplyAddressElement, baos.ToArray());
            this.AddInformationElement(infoElement);
            baos.Close();
        }

        /// <summary>
        /// Writes the BCD address.
        /// </summary>
        /// <param name="baos">Memory stream</param>
        /// <param name="address">The address</param>
        /// <param name="addressType">Type of the address</param>
        /// <param name="addressLength">Length of the address</param>
        protected internal virtual void WriteBcdAddress(MemoryStream baos, string address, int addressType, int addressLength)
        {
            // BCD-style
            // ADDRESS LENGTH - either an octet count or semi-octet count
            baos.WriteByte((byte)addressLength);
            // ADDRESS TYPE
            baos.WriteByte((byte)addressType);
            // ADDRESS NUMBERS
            // if address.length is not even, pad the string an with F at the end
            if (address.Length % 2 == 1)
            {
                address = address + "F";
            }
            int digit = 0;
            for (int i = 0; i < address.Length; i++)
            {
                char c = address[i];
                if (i % 2 == 1)
                {
                    digit |= ((Convert.ToInt32(Convert.ToString(c), 16)) << 4);
                    baos.WriteByte((byte)digit);
                    // clear it
                    digit = 0;
                }
                else
                {
                    digit |= (Convert.ToInt32(Convert.ToString(c), 16) & 0x0F);
                }
            }
        }
	}
}