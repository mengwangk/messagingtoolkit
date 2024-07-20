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
    /// Device network information.
    /// </summary>
    [DataContract]
    public sealed class DeviceNetworkInformation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DeviceNetworkInformation"/> class.
        /// </summary>
        public DeviceNetworkInformation()
        {
            this.CdmaCellLocation = null;
            this.CellLocationType = string.Empty;
            this.ConnectionType = string.Empty;
            this.DeviceId = string.Empty;
            this.DeviceSoftwareVersion = string.Empty;
            this.GsmCellLocation = null;
            this.IPAddress = string.Empty;
            this.IsConnected = false;
            this.IsNetworkRoaming = false;
            this.NetworkCountryIso = string.Empty;
            this.NetworkOperator = string.Empty;
            this.NetworkOperatorName = string.Empty;
            this.NetworkType = string.Empty;
            this.PhoneNumber = string.Empty;
            this.PhoneType = string.Empty;
            this.SignalStrength = null;
            this.SimCountryIso = string.Empty;
            this.SimOperator = string.Empty;
            this.SimOperatorName = string.Empty;
            this.SimSerialNumber = string.Empty;
            this.SimState = string.Empty;
            this.SubscriberId = string.Empty;
            this.VoiceMailNumber = string.Empty;
        }
       
        /// <summary>
        /// Gets or sets the type of the cell location.
        /// </summary>
        /// <value>
        /// Current location type of the device. Can be either gsmCellLocation or cdmaCellLocation.
        /// </value>
        [DataMember(Name = "cellLocationType")]
        public string CellLocationType { get; set; }


        /// <summary>
        /// Gets or sets the GSM cell location.
        /// </summary>
        /// <value>
        /// GSM cell location information if cellLocationType is GSM.
        /// </value>
        [DataMember(Name = "gsmCellLocation")]
        public GsmCellLocation GsmCellLocation { get; set; }


        /// <summary>
        /// Gets or sets the cdma cell location.
        /// </summary>
        /// <value>
        /// CDMA cell location information if cellLocationType is CDMA.
        /// </value>
        [DataMember(Name = "cdmaCellLocation")]
        public CdmaCellLocation CdmaCellLocation { get; set; }

        /// <summary>
        /// Gets or sets the type of the connection.
        /// </summary>
        /// <value>
        /// Possible values are WIFI and MOBILE.
        /// </value>
        [DataMember(Name = "connectionType")]
        public string ConnectionType { get; set; }

        /// <summary>
        /// Gets or sets the device identifier.
        /// </summary>
        /// <value>
        /// Returns the unique device ID, for example, the IMEI for GSM and the MEID or ESN for CDMA phones.
        /// </value>
        [DataMember(Name = "deviceId")]
        public string DeviceId { get; set; }

        /// <summary>
        /// Gets or sets the device software version.
        /// </summary>
        /// <value>
        /// Returns the software version number for the device, for example, the IMEI/SV for GSM phones.
        /// </value>
        [DataMember(Name = "deviceSoftwareVersion")]
        public string DeviceSoftwareVersion { get; set; }

        /// <summary>
        /// Gets or sets the IP address.
        /// </summary>
        /// <value>
        /// IP address.
        /// </value>
        [DataMember(Name = "ipAddress")]
        public string IPAddress { get; set; }

        /// <summary>
        /// Gets or sets the voice mail number.
        /// </summary>
        /// <value>
        /// Returns the voice mail number.
        /// </value>
        [DataMember(Name = "voiceMailNumber")]
        public string VoiceMailNumber { get; set; }


        /// <summary>
        /// Gets or sets the subscriber identifier.
        /// </summary>
        /// <value>
        /// Returns the unique subscriber ID, for example, the IMSI for a GSM phone.
        /// </value>
        [DataMember(Name = "subscriberId")]
        public string SubscriberId { get; set; }

        /// <summary>
        /// Gets or sets the network country iso.
        /// </summary>
        /// <value>
        /// Returns the ISO country code equivalent of the current registered operator's MCC (Mobile Country Code).
        /// </value>
        [DataMember(Name = "networkCountryIso")]
        public string NetworkCountryIso { get; set; }

        /// <summary>
        /// Gets or sets the network operator.
        /// </summary>
        /// <value>
        /// Returns the numeric name (MCC+MNC) of current registered operator.
        /// </value>
        [DataMember(Name = "networkOperator")]
        public string NetworkOperator { get; set; }

        /// <summary>
        /// Gets or sets the name of the network operator.
        /// </summary>
        /// <value>
        /// Returns the alphabetic name of current registered operator.
        /// </value>
        [DataMember(Name = "networkOperatorName")]
        public string NetworkOperatorName { get; set; }

        /// <summary>
        /// Gets or sets the type of the network.
        /// </summary>
        /// <value>
        /// The network type for current data connection. Possible values are 1xRTT, CDMA, EDGE, EHRPD, EVDO_0, EVDO_A, EVDO_B, GPRS, HSDPA, HPSA, HSPAP, HSUPA, IDEN, LTE, UMTS, and UNKNOWN.
        /// </value>
        [DataMember(Name = "networkType")]
        public string NetworkType { get; set; }

        /// <summary>
        /// Gets or sets the phone number.
        /// </summary>
        /// <value>
        /// Phone number.
        /// </value>
        [DataMember(Name = "phoneNumber")]
        public string PhoneNumber { get; set; }

        /// <summary>
        /// Gets or sets the type of the phone.
        /// </summary>
        /// <value>
        /// Phone type. Possible valus are GSM, CDMA, SIP or NONE.
        /// </value>
        [DataMember(Name = "phoneType")]
        public string PhoneType { get; set; }

        /// <summary>
        /// Gets or sets the signal strength.
        /// </summary>
        /// <value>
        /// Signal strength. 
        /// </value>
        [DataMember(Name = "signalStrength")]
        public DeviceSignalStrength SignalStrength { get; set; }

        /// <summary>
        /// Gets or sets the sim country iso.
        /// </summary>
        /// <value>
        /// Returns the ISO country code equivalent for the SIM provider's country code.
        /// </value>
        [DataMember(Name = "simCountryIso")]
        public string SimCountryIso { get; set; }

        /// <summary>
        /// Gets or sets the sim country operator.
        /// </summary>
        /// <value>
        /// Returns the MCC+MNC (mobile country code + mobile network code) of the provider of the SIM.
        /// </value>
        [DataMember(Name = "simOperator")]
        public string SimOperator { get; set; }

        /// <summary>
        /// Gets or sets the name of the sim country operator.
        /// </summary>
        /// <value>
        /// Returns the Service Provider Name (SPN).
        /// </value>
        [DataMember(Name = "simOperatorName")]
        public string SimOperatorName { get; set; }


        /// <summary>
        /// Gets or sets the sim serial number.
        /// </summary>
        /// <value>
        /// Returns the serial number of the SIM, if applicable.
        /// </value>
        [DataMember(Name = "simSerialNumber")]
        public string SimSerialNumber { get; set; }


        /// <summary>
        /// Gets or sets the state of the sim.
        /// </summary>
        /// <value>
        /// Returns the state of the device SIM card. Possible values are ABSENT, NETWORK_LOCKED, PIN_REQUIRED, PUK_REQUIRED, READY, UNKNOWN.
        /// </value>
        [DataMember(Name = "simState")]
        public string SimState { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is network roaming.
        /// </summary>
        /// <value>
        /// Returns true if the device is considered roaming on the current network, for GSM purposes.
        /// </value>
        [DataMember(Name = "isNetworkRoaming")]
        public bool IsNetworkRoaming { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is connected.
        /// </summary>
        /// <value>
        /// Indicates if the phone is connected to a network (WIFI or mobile).
        /// </value>
        [DataMember(Name = "isConnected")]
        public bool IsConnected { get; set; }


        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        override public string ToString()
        {
            string str = String.Empty;
            str = String.Concat(str, "CellLocationType = ", CellLocationType, "\r\n");
            str = String.Concat(str, "GsmCellLocation = ", GsmCellLocation, "\r\n");
            str = String.Concat(str, "CdmaCellLocation = ", CdmaCellLocation, "\r\n");
            str = String.Concat(str, "ConnectionType = ", ConnectionType, "\r\n");
            str = String.Concat(str, "DeviceId = ", DeviceId, "\r\n");
            str = String.Concat(str, "DeviceSoftwareVersion = ", DeviceSoftwareVersion, "\r\n");
            str = String.Concat(str, "IPAddress = ", IPAddress, "\r\n");
            str = String.Concat(str, "VoiceMailNumber = ", VoiceMailNumber, "\r\n");
            str = String.Concat(str, "SubscriberId = ", SubscriberId, "\r\n");
            str = String.Concat(str, "NetworkCountryIso = ", NetworkCountryIso, "\r\n");
            str = String.Concat(str, "NetworkOperator = ", NetworkOperator, "\r\n");
            str = String.Concat(str, "NetworkOperatorName = ", NetworkOperatorName, "\r\n");
            str = String.Concat(str, "NetworkType = ", NetworkType, "\r\n");
            str = String.Concat(str, "PhoneNumber = ", PhoneNumber, "\r\n");
            str = String.Concat(str, "PhoneType = ", PhoneType, "\r\n");
            str = String.Concat(str, "SignalStrength = ", SignalStrength, "\r\n");
            str = String.Concat(str, "SimCountryIso = ", SimCountryIso, "\r\n");
            str = String.Concat(str, "SimOperator = ", SimOperator, "\r\n");
            str = String.Concat(str, "SimOperatorName = ", SimOperatorName, "\r\n");
            str = String.Concat(str, "SimSerialNumber = ", SimSerialNumber, "\r\n");
            str = String.Concat(str, "SimState = ", SimState, "\r\n");
            str = String.Concat(str, "IsNetworkRoaming = ", IsNetworkRoaming, "\r\n");
            str = String.Concat(str, "IsConnected = ", IsConnected, "\r\n");
            return str;
        }

    }
}
