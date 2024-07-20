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

using MessagingToolkit.Core.Base;

namespace MessagingToolkit.Core.Mobile
{
    /// <summary>
    /// IP based gateway configuration
    /// </summary>
    [System.Reflection.ObfuscationAttribute(Feature = "renaming", ApplyToMembers = true)]
    public class IPGatewayConfiguration: BaseGatewayConfiguration, IConfiguration
    {
        #region =============== Constructor ==================================================================

        /// <summary>
        /// Initializes a new instance of the <see cref="IPGatewayConfiguration"/> class.
        /// </summary>
        /// <param name="ipAddress">The ip address.</param>
        /// <param name="port">The port.</param>
       public IPGatewayConfiguration(string ipAddress, int port): base()
       {
           this.IPAddress = ipAddress;
           this.Port = port;
       }

       #endregion ============================================================================================


       #region ============== Public Properties ==============================================================

       /// <summary>
        /// Gets or sets the IP address.
        /// </summary>
        /// <value>The IP address.</value>
        public string IPAddress
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the port.
        /// </summary>
        /// <value>The port.</value>
        public int Port
        {
            get;
            private set;
        }

        #endregion ============================================================================================



        #region ============== Factory method   ===============================================================
        /// <summary>
        /// Static factory to create the gateway configuration
        /// </summary>
        /// <returns>A new instance of gateway configuration</returns>
        public static IPGatewayConfiguration NewInstance(string ipAddress, int port)
        {
            return new IPGatewayConfiguration(ipAddress, port);
        }

        #endregion ===========================================================================================


        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        override public string ToString()
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
            str = String.Concat(str, "Prefixes = ", Prefixes, "\r\n");
            str = String.Concat(str, "OperatorSelectionFormat = ", OperatorSelectionFormat, "\r\n");
            str = String.Concat(str, "SkipOperatorSelection = ", SkipOperatorSelection, "\r\n");
            str = String.Concat(str, "EncodedUssdCommand = ", EncodedUssdCommand, "\r\n");
            str = String.Concat(str, "CommandWaitInterval = ", CommandWaitInterval, "\r\n");
            str = String.Concat(str, "CommandWaitRetryCount = ", CommandWaitRetryCount, "\r\n");
            str = String.Concat(str, "IPAddress = ", IPAddress, "\r\n");
            str = String.Concat(str, "Port = ", Port, "\r\n");
            return str;
        }
    }
}
