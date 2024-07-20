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
using System.Xml.Serialization;

using MessagingToolkit.Core.Base;
using MessagingToolkit.Core.Mobile.Event;
using MessagingToolkit.Core.Log;

namespace MessagingToolkit.Core.Mobile
{
    /// <summary>
    /// Mobile gateway configuration class
    /// </summary>
    [System.Reflection.ObfuscationAttribute(Feature = "renaming", ApplyToMembers = true)]
    [Serializable]
    public class MobileGatewayConfiguration : BaseGatewayConfiguration, IConfiguration
    {

        #region =============== Private Constants ============================================================

        /// <summary>
        /// Gateway connected event
        /// </summary>
        public event ConnectedEventHandler GatewayConnected;


        /// <summary>
        /// Read time out for the gateway in milliseconds
        /// </summary>
        private const int DefaultReadTimeout = 3000;

        /// <summary>
        /// Write time out for the gateway in milliseconds
        /// </summary>
        private const int DefaultWriteTimeout = 3000;


        #endregion ===========================================================================================


        #region =============== Constructor ==================================================================

        /// <summary>
        /// Private constructor
        /// </summary>
        private MobileGatewayConfiguration()
            : base()
        {
            // Set default values
            PortName = string.Empty;
            BaudRate = PortBaudRate.BitsPerSecond115200;
            Handshake = PortHandshake.None;
            Parity = PortParity.None;
            DataBits = PortDataBits.Eight;
            StopBits = PortStopBits.One;
            RtsEnable = true;
            DtrEnable = true;
            DataConnectionTimeout = 0;
            SafeConnect = false;
            SafeDisconnect = false;
            ReadTimeOut = DefaultReadTimeout;
            WriteTimeOut = DefaultWriteTimeout;
        }


        #endregion ============================================================================================



        #region ============== Public Properties ==============================================================


        /// <summary>
        /// Communication port baud rate
        /// </summary>
        /// <value>PortBaudRate enum. See <see cref="PortBaudRate"/></value>
        [XmlAttribute]
        public PortBaudRate BaudRate
        {
            get;
            set;
        }

        /// <summary>
        /// Communication data bits
        /// </summary>
        /// <value>PortDataBits enum. See <see cref="PortDataBits"/></value>
        [XmlAttribute]
        public PortDataBits DataBits
        {
            get;
            set;
        }

        /// <summary>
        /// Communcation flow control
        /// </summary>
        /// <value>PortHandshake enum. See <see cref="PortHandshake"/></value>
        [XmlAttribute]
        public PortHandshake Handshake
        {
            get;
            set;
        }

        /// <summary>
        /// Communication port, e.g. COM1, COM2, COM3.
        /// </summary>
        /// <value>Port name</value>
        [XmlAttribute]
        public string PortName
        {
            get;
            set;
        }

        /// <summary>
        /// Port parity
        /// </summary>
        /// <value>PortParity enum. See <see cref="PortParity"/></value>
        [XmlAttribute]
        public PortParity Parity
        {
            get;
            set;
        }

        /// <summary>
        /// Port stop bits
        /// </summary>
        /// <value>PortStopBits enum. See <see cref="PortStopBits"/></value>
        [XmlAttribute]
        public PortStopBits StopBits
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether Ready To Send (RTS) signal is enabled during serial communication.
        /// </summary>
        /// <value>
        ///   <c>true</c> if RTS enabled; otherwise, <c>false</c>.
        /// </value>
        [XmlAttribute]
        public bool RtsEnable
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value that enables the Data Terminal Ready (DTR) signal during serial communication
        /// </summary>
        /// <value>
        ///   <c>true</c> if DTR is enabled; otherwise, <c>false</c>.
        /// </value>
        [XmlAttribute]
        public bool DtrEnable
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the data connection timeout.
        /// </summary>
        /// <value>
        /// The data connection timeout in milliseconds.
        /// </value>
        [XmlAttribute]
        public int DataConnectionTimeout
        {
            get;
            set;
        }

        /// <summary>
        /// Fix the bug as described in http://zachsaw.blogspot.com/2010/07/net-serialport-woes.html
        /// </summary>
        /// <value>
        /// Flag indicating to fix the serial port before opening. Default to false
        /// </value>
        [XmlAttribute]
        public bool SafeConnect
        {
            get;
            set;
        }

        /// <summary>
        /// Handle scenario serial port base stream not closed when the device is unplug
        /// </summary>
        /// <value>
        /// Flag indicating to fix close the base stream when the device is unplug. Default to false
        /// </value>
        [XmlAttribute]
        public bool SafeDisconnect
        {
            get;
            set;
        }

        /// <summary>
        /// Serial port read time out in milliseconds
        /// </summary>
        /// <value>
        /// Value in milliseconds
        /// </value>
        [XmlAttribute]
        public int ReadTimeOut
        {
            get;
            set;
        }

        /// <summary>
        /// Serial port write time out in milliseconds
        /// </summary>
        /// <value>
        /// Value in milliseconds
        /// </value>
        [XmlAttribute]
        public int WriteTimeOut
        {
            get;
            set;
        }

        #endregion ============================================================================================



        #region ============== Protected method   ===============================================================
        /// <summary>
        /// Static factory to create the gateway configuration
        /// </summary>
        /// <returns>A new instance of gateway configuration</returns>
        internal void OnConnected(ConnectionEventArgs args)
        {
            if (this.GatewayConnected != null)
            {
                this.GatewayConnected.BeginInvoke(this, args, new AsyncCallback(this.AsyncCallback), null);
            }
        }

        #endregion ===========================================================================================


        #region ============================== Override ======================================================
        /// <summary>
        /// Equal comparer
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        protected bool Equals(MobileGatewayConfiguration other)
        {
            return BaudRate == other.BaudRate && DataBits == other.DataBits && Handshake == other.Handshake && string.Equals(PortName, other.PortName) && Parity == other.Parity && StopBits == other.StopBits && RtsEnable == other.RtsEnable && DtrEnable == other.DtrEnable && DataConnectionTimeout == other.DataConnectionTimeout && SafeConnect == other.SafeConnect && SafeDisconnect == other.SafeDisconnect && ReadTimeOut == other.ReadTimeOut && WriteTimeOut == other.WriteTimeOut;
        }

        /// <summary>
        /// Equal comparer
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((MobileGatewayConfiguration) obj);
        }

        /// <summary>
        /// Get hash code
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (int) BaudRate;
                hashCode = (hashCode*397) ^ (int) DataBits;
                hashCode = (hashCode*397) ^ (int) Handshake;
                hashCode = (hashCode*397) ^ (PortName != null ? PortName.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (int) Parity;
                hashCode = (hashCode*397) ^ (int) StopBits;
                hashCode = (hashCode*397) ^ RtsEnable.GetHashCode();
                hashCode = (hashCode*397) ^ DtrEnable.GetHashCode();
                hashCode = (hashCode*397) ^ DataConnectionTimeout;
                hashCode = (hashCode*397) ^ SafeConnect.GetHashCode();
                hashCode = (hashCode*397) ^ SafeDisconnect.GetHashCode();
                hashCode = (hashCode*397) ^ ReadTimeOut;
                hashCode = (hashCode*397) ^ WriteTimeOut;
                return hashCode;
            }
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            string str = String.Empty;
            str = String.Concat(str, "GatewayId = ", GatewayId, "\r\n");
            str = String.Concat(str, "InitializationString = ", InitializationString, "\r\n");
            str = String.Concat(str, "Pin = ", Pin, "\r\n");
            str = String.Concat(str, "Model = ", Model, "\r\n");
            str = String.Concat(str, "SendWaitInterval = ", SendWaitInterval, "\r\n");
            str = String.Concat(str, "CommandFile = ", CommandFile, "\r\n");
            str = String.Concat(str, "WatchDogInterval = ", WatchDogInterval, "\r\n");
            str = String.Concat(str, "DeleteReceivedMessage = ", DeleteReceivedMessage, "\r\n");
            str = String.Concat(str, "ConcatenateMessage = ", ConcatenateMessage, "\r\n");
            str = String.Concat(str, "DisablePinCheck = ", DisablePinCheck, "\r\n");
            str = String.Concat(str, "DisableWatchDog = ", DisableWatchDog, "\r\n");
            str = String.Concat(str, "CheckNetworkRegistrationOnStartup = ", CheckNetworkRegistrationOnStartup, "\r\n");
            str = String.Concat(str, "ResetGatewayAfterTimeout = ", ResetGatewayAfterTimeout, "\r\n");
            str = String.Concat(str, "AutoDisconnectIncomingCall = ", AutoDisconnectIncomingCall, "\r\n");
            str = String.Concat(str, "DeviceName = ", DeviceName, "\r\n");
            str = String.Concat(str, "ProviderMMSC = ", ProviderMMSC, "\r\n");
            str = String.Concat(str, "ProviderAPN = ", ProviderAPN, "\r\n");
            str = String.Concat(str, "ProviderWAPGateway = ", ProviderWAPGateway, "\r\n");
            str = String.Concat(str, "ProviderAPNAccount = ", ProviderAPNAccount, "\r\n");
            str = String.Concat(str, "ProviderAPNPassword = ", ProviderAPNPassword, "\r\n");
            str = String.Concat(str, "DataCompressionControl = ", DataCompressionControl, "\r\n");
            str = String.Concat(str, "HeaderCompressionControl = ", HeaderCompressionControl, "\r\n");
            str = String.Concat(str, "InternetProtocol = ", InternetProtocol, "\r\n");
            str = String.Concat(str, "UserAgent = ", UserAgent, "\r\n");
            str = String.Concat(str, "XWAPProfile = ", XWAPProfile, "\r\n");
            str = String.Concat(str, "SupportHttpCharsetHeader = ", SupportHttpCharsetHeader, "\r\n");
            str = String.Concat(str, "UseHttpTransportForMMS = ", UseHttpTransportForMMS, "\r\n");
            str = String.Concat(str, "Prefixes = ", Prefixes, "\r\n");
            str = String.Concat(str, "OperatorSelectionFormat = ", OperatorSelectionFormat, "\r\n");
            str = String.Concat(str, "SkipOperatorSelection = ", SkipOperatorSelection, "\r\n");
            str = String.Concat(str, "EncodedUssdCommand = ", EncodedUssdCommand, "\r\n");
            str = String.Concat(str, "CommandWaitInterval = ", CommandWaitInterval, "\r\n");
            str = String.Concat(str, "CommandWaitRetryCount = ", CommandWaitRetryCount, "\r\n");
            str = String.Concat(str, "BaudRate = ", BaudRate, "\r\n");
            str = String.Concat(str, "DataBits = ", DataBits, "\r\n");
            str = String.Concat(str, "Handshake = ", Handshake, "\r\n");
            str = String.Concat(str, "PortName = ", PortName, "\r\n");
            str = String.Concat(str, "Parity = ", Parity, "\r\n");
            str = String.Concat(str, "StopBits = ", StopBits, "\r\n");
            str = String.Concat(str, "RtsEnable = ", RtsEnable, "\r\n");
            str = String.Concat(str, "DtrEnable = ", DtrEnable, "\r\n");
            str = String.Concat(str, "DataConnectionTimeout = ", DataConnectionTimeout, "\r\n");
            str = String.Concat(str, "SafeConnect = ", SafeConnect, "\r\n");
            str = String.Concat(str, "SafeDisconnect = ", SafeDisconnect, "\r\n");
            str = String.Concat(str, "ReadTimeOut = ", ReadTimeOut, "\r\n");
            str = String.Concat(str, "WriteTimeOut = ", WriteTimeOut, "\r\n");
            return str;
        }

        #endregion ----------------------------------------------------------------------------------------------------------------------

        /// <summary>
        /// Asynchronous callback method
        /// </summary>
        /// <param name="param">Result</param>
        private void AsyncCallback(IAsyncResult param)
        {
            System.Runtime.Remoting.Messaging.AsyncResult result = (System.Runtime.Remoting.Messaging.AsyncResult)param;
            if (result.AsyncDelegate is ConnectedEventHandler)
            {
                Logger.LogThis("Ending async ConnectedEventHandler call", LogLevel.Verbose, this.GatewayId);
                ((ConnectedEventHandler)result.AsyncDelegate).EndInvoke(result);
            }
            else
            {
                Logger.LogThis("Warning: AsyncCallback got unknown delegate: " + result.AsyncDelegate.GetType().ToString(), LogLevel.Verbose, this.GatewayId);
            }
        }

        #region ============== Factory method   ===============================================================
        /// <summary>
        /// Static factory to create the gateway configuration
        /// </summary>
        /// <returns>A new instance of gateway configuration</returns>
        public static MobileGatewayConfiguration NewInstance()
        {
            return new MobileGatewayConfiguration();
        }

        #endregion ===========================================================================================

    }
}
