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

using MessagingToolkit.Core.Mobile.PduLibrary;

namespace MessagingToolkit.Core.Mobile.Message
{
    /// <summary>
    /// EMS submit
    /// </summary>
    internal class EmsSubmit : SmsSubmit
    {
        #region ========================= Constructor =============================
        
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="pduCode">PDU code</param>
        public EmsSubmit(string pduCode): base(pduCode)
        {
            messageType = MessageTypeIndicator.MtiEmsSubmit;
            Decode(pduCode);
        }

        #endregion ================================================================


        #region ========================= Public Properties =======================

        /// <summary>
        /// User data header length
        /// </summary>
        /// <value>Data header length</value>
        public byte UserDataHeaderLength
        {
            get;
            set;
        }

        /// <summary>
        /// List of info elements
        /// </summary>
        /// <value>Info element list</value>
        public InfoElement[] InfoElements
        {
            get;
            set;
        }

        #endregion ================================================================

        #region ========================= Protected Method  =======================

        /// <summary>
        /// Return a list of info element based on the code
        /// </summary>
        /// <param name="infoElementCode">List of info element</param>
        /// <returns></returns>
        protected InfoElement[] GetInfoElement(string infoElementCode)
        {           
            List<InfoElement> result = new List<InfoElement>(1);            
            while (!(string.IsNullOrEmpty(infoElementCode)))
            {
                InfoElement infoElement = new InfoElement();
                infoElement.Identifier = PduUtils.GetByte(ref infoElementCode);
                infoElement.Length = PduUtils.GetByte(ref infoElementCode);
                infoElement.Data = PduUtils.GetString(ref infoElementCode, infoElement.Length * 2);
                result.Add(infoElement);
            }
            return result.ToArray();
        }

        #endregion ================================================================

        #region ========================= Public Method ===========================

        /// <summary>
        /// Decode the EMS
        /// </summary>
        /// <param name="pduCode">PDU code</param>
        public override void Decode(string pduCode)
        {
            serviceCenterNumberLength = PduUtils.GetByte(ref pduCode);
            serviceCenterNumberType = PduUtils.GetByte(ref pduCode);
            ServiceCenterNumber = PduUtils.GetAddress(PduUtils.GetString(ref pduCode, (serviceCenterNumberLength - 1) * 2));
            firstOctet = PduUtils.GetByte(ref pduCode);

            MessageReference = PduUtils.GetByte(ref pduCode);

            DestinationAddressLength = PduUtils.GetByte(ref pduCode);
            DestinationAddressType = PduUtils.GetByte(ref pduCode);
            DestinationAddressLength += Convert.ToByte(DestinationAddressLength % 2);
            DestinationAddress = PduUtils.GetAddress(PduUtils.GetString(ref pduCode, DestinationAddressLength));

            protocolIdentifier = PduUtils.GetByte(ref pduCode);
            // For flash message, it must be substracted
            int dcs = PduUtils.GetByte(ref pduCode);
            if (dcs >= 16) dcs -= 16;
            DataCodingScheme = (MessageDataCodingScheme)Enum.Parse(typeof(MessageDataCodingScheme), Convert.ToString(dcs));
            ValidityPeriod = (MessageValidPeriod)Enum.Parse(typeof(MessageValidPeriod), PduUtils.GetByte(ref pduCode).ToString());
            ContentLength = PduUtils.GetByte(ref pduCode);

            UserDataHeaderLength = PduUtils.GetByte(ref pduCode);
            InfoElements = GetInfoElement(PduUtils.GetString(ref pduCode, UserDataHeaderLength * 2));

            Content = PduUtils.GetString(ref pduCode, ContentLength * 2);
        }

        #endregion ================================================================
    }
}
