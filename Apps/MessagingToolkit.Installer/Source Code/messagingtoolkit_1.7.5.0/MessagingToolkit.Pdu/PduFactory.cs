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

using MessagingToolkit.Pdu.WapPush;
using MessagingToolkit.Pdu.Ie;

namespace MessagingToolkit.Pdu
{
    /// <summary>
    /// PDU factory
    /// </summary>
	public class PduFactory
	{
        /// <summary>
        /// Create the a new SMS submit PDU
        /// </summary>
        /// <returns>A new SMS submit PDU</returns>
		public static SmsSubmitPdu NewSmsSubmitPdu()
		{
			// apply defaults
			int additionalFields = PduUtils.TpRdAcceptDuplicates | PduUtils.TpVpfInteger;
			return NewSmsSubmitPdu(additionalFields);
		}
		
		public static SmsSubmitPdu NewSmsSubmitPdu(int additionalFields)
		{
			// remove any TP_MTI values
			int firstOctet = PduUtils.TpMtiSmsSubmit | additionalFields;
			return (SmsSubmitPdu) CreatePdu(firstOctet);
		}

        /// <summary>
        /// News the wap si pdu.
        /// </summary>
        /// <returns></returns>
		public static WapSiPdu NewWapSiPdu()
		{
			// apply defaults
			int additionalFields = PduUtils.TpRdAcceptDuplicates | PduUtils.TpVpfInteger;
			return NewWapSiPdu(additionalFields);
		}

        /// <summary>
        /// News the wap si pdu.
        /// </summary>
        /// <param name="additionalFields">The additional fields.</param>
        /// <returns></returns>
		public static WapSiPdu NewWapSiPdu(int additionalFields)
		{
			// remove any TP_MTI values
			WapSiPdu pdu;
			int firstOctet = PduUtils.TpMtiSmsSubmit | additionalFields;
			int messageType = GetFirstOctetField(firstOctet, PduUtils.TpMtiMask);
			switch (messageType)
			{				
				case PduUtils.TpMtiSmsSubmit: 
					pdu = new WapSiPdu();
					break;
				
				default:
                    throw new Exception("Invalid TP-MTI value: " + messageType);				
			}
			pdu.FirstOctet = firstOctet;
			return pdu;
		}


        /// <summary>
        /// News the SMS delivery pdu.
        /// NOTE: this is only for testing since Incoming Message Pdus
        /// are created directly using createPdu()
        /// </summary>
        /// <returns></returns>
		public static SmsSubmitPdu NewSmsDeliveryPdu()
		{
			int firstOctet = PduUtils.TpMtiSmsDeliver | PduUtils.TpMmsMoreMessages;
			return (SmsSubmitPdu) CreatePdu(firstOctet);
		}

        /// <summary>
        /// News the SMS delivery pdu.
        /// </summary>
        /// <param name="additionalFields">The additional fields.</param>
        /// <returns></returns>
		public static SmsDeliveryPdu NewSmsDeliveryPdu(int additionalFields)
		{
			// remove any TP_MTI values
			int firstOctet = PduUtils.TpMtiSmsDeliver | additionalFields;
			return (SmsDeliveryPdu) CreatePdu(firstOctet);
		}

        /// <summary>
        /// Gets the first octet field.
        /// </summary>
        /// <param name="firstOctet">The first octet.</param>
        /// <param name="fieldName">Name of the field.</param>
        /// <returns></returns>
		private static int GetFirstOctetField(int firstOctet, int fieldName)
		{
			return firstOctet & ~ fieldName;
		}
		
		
        /// <summary>
        /// Creates the pdu.
        /// used to determine what Pdu to use based on the first octet
        /// this is the only way to instantiate a Pdu object
        /// </summary>
        /// <param name="firstOctet">The first octet.</param>
        /// <returns></returns>
		public static Pdu CreatePdu(int firstOctet)
		{
			Pdu pdu = null;
			int messageType = GetFirstOctetField(firstOctet, PduUtils.TpMtiMask);
            switch (messageType)
			{				
				case PduUtils.TpMtiSmsDeliver: 
					pdu = new SmsDeliveryPdu();
					break;
				
				case PduUtils.TpMtiSmsStatusReport: 
					pdu = new SmsStatusReportPdu();
					break;
				
				case PduUtils.TpMtiSmsSubmit: 
					pdu = new SmsSubmitPdu();
					break;
				
				default:
                    throw new Exception("Invalid TP-MTI value: " + messageType);
				
			}
			// once set, you can't change it
			// this method is only available in this package
			pdu.FirstOctet = firstOctet;
			return pdu;
		}
	}
}