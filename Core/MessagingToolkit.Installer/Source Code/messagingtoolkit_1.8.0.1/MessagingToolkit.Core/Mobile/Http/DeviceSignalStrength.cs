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
using System.Runtime.Serialization;
using System.Text;

namespace MessagingToolkit.Core.Mobile.Http
{
    /// <summary>
    /// Device signal strength.
    /// </summary>
    [DataContract]
    public sealed class DeviceSignalStrength
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="DeviceSignalStrength"/> class.
        /// </summary>
        public DeviceSignalStrength()
        {
            this.CdmaDbm = -1;
            this.CdmaEcio = -1;
            this.EvdoDbm = -1;
            this.EvdoEcio = -1;
            this.EvdoSnr = -1;
            this.GsmBitErrorRate = -1;
            this.GsmSignalStrength = -1;
            this.IsGsm = false;
        }

        /// <summary>
        /// Gets or sets the cdma DBM.
        /// </summary>
        /// <value>
        /// Get the CDMA RSSI value in dBm.
        /// </value>
        [DataMember(Name = "cdmaDbm")]
        public int CdmaDbm { get; set; }

        /// <summary>
        /// Gets or sets the cdma ecio.
        /// </summary>
        /// <value>
        /// Get the CDMA Ec/Io value in dB*10.
        /// </value>
        [DataMember(Name = "cdmaEcio")]
        public int CdmaEcio { get; set; }

        /// <summary>
        /// Gets or sets the evdo DBM.
        /// </summary>
        /// <value>
        ///Get the EVDO RSSI value in dBm.
        /// </value>
        [DataMember(Name = "evdoDbm")]
        public int EvdoDbm { get; set; }

        /// <summary>
        /// Gets or sets the evdo ecio.
        /// </summary>
        /// <value>
        /// Get the EVDO Ec/Io value in dB*10.
        /// </value>
        [DataMember(Name = "evdoEcio")]
        public int EvdoEcio { get; set; }

        /// <summary>
        /// Gets or sets the evdo SNR.
        /// </summary>
        /// <value>
        /// Get the signal to noise ratio. Valid values are 0-8. 8 is the highest.
        /// </value>
        [DataMember(Name = "evdoSnr")]
        public int EvdoSnr { get; set; }

        /// <summary>
        /// Gets or sets the GSM bit error rate.
        /// </summary>
        /// <value>
        /// Get the GSM bit error rate (0-7, 99) as defined in TS 27.007 8.5.
        /// </value>
        [DataMember(Name = "gsmBitErrorRate")]
        public int GsmBitErrorRate { get; set; }

        /// <summary>
        /// Gets or sets the GSM signal strength.
        /// </summary>
        /// <value>
        ///  Get the GSM Signal Strength, valid values are (0-31, 99) as defined in TS 27.007 8.5.
        /// </value>
        [DataMember(Name = "gsmSignalStrength")]
        public int GsmSignalStrength { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is GSM.
        /// </summary>
        /// <value>
        ///  true if this is for GSM.
        /// </value>
        [DataMember(Name = "isGsm")]
        public bool IsGsm { get; set; }


        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        override public string ToString()
        {
            string str = String.Empty;
            str = String.Concat(str, "CdmaDbm = ", CdmaDbm, "\r\n");
            str = String.Concat(str, "CdmaEcio = ", CdmaEcio, "\r\n");
            str = String.Concat(str, "EvdoDbm = ", EvdoDbm, "\r\n");
            str = String.Concat(str, "EvdoEcio = ", EvdoEcio, "\r\n");
            str = String.Concat(str, "EvdoSnr = ", EvdoSnr, "\r\n");
            str = String.Concat(str, "GsmBitErrorRate = ", GsmBitErrorRate, "\r\n");
            str = String.Concat(str, "GsmSignalStrength = ", GsmSignalStrength, "\r\n");
            str = String.Concat(str, "IsGsm = ", IsGsm, "\r\n");
            return str;
        }
    }
}
