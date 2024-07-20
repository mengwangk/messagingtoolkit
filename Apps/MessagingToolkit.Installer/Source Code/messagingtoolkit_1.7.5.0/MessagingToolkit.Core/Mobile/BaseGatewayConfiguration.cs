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
using System.Xml.Serialization;

using MessagingToolkit.Core.Base;

namespace MessagingToolkit.Core.Mobile
{
    /// <summary>
    /// Base class for all gateway configuration
    /// </summary>
    [System.Reflection.ObfuscationAttribute(Feature = "renaming", ApplyToMembers = true)]
    [Serializable]
    public class BaseGatewayConfiguration : BaseConfiguration
    {
        /// <summary>
        /// Number of times to retries if message sending failed
        /// </summary>
        private const int DefaultSendRetries = 3;

        /// <summary>
        /// Number of milliseconds to wait in between continuous message sending
        /// </summary>
        private const int DefaultSendWaitInterval = 200;


        /// <summary>
        /// Default command file
        /// </summary>
        private const string DefaultCommandFile = "command.txt";


        /// <summary>
        /// Watch dog interval to check connection status. Default to 60 seconds
        /// </summary>
        private const int DefaultWatchDogInterval = 60000;


        /// <summary>
        /// Default message polling interval
        /// </summary>
        private const int DefaultMessagePollingInterval = 10000;


        /// <summary>
        /// Default wait interval in milliseconds after reset
        /// </summary>
        private const int DefaultWaitIntervalAfterReset = 10000;

        /// <summary>
        /// Number of milliseconds to wait for the command response.
        /// This multiplies by the <see cref="DefaultCommandWaitRetryCount"/> gives the 
        /// number of milliseconds for the command time out.
        /// </summary>
        private const int DefaultCommandWaitInterval = 300;

        /// <summary>
        /// Number of retry count to wait for the command response.
        /// </summary>
        private const int DefaultCommandWaitRetryCount = 100;
                
        /// <summary>
        /// Initializes a new instance of the <see cref="BaseGatewayConfiguration"/> class.
        /// </summary>
        public BaseGatewayConfiguration()
            : base()
        {
            // Set default values
            Pin = string.Empty;
            InitializationString = string.Empty;

            // Set default values
            SendRetries = DefaultSendRetries;
            SendWaitInterval = DefaultSendWaitInterval;
            CommandWaitInterval = DefaultCommandWaitInterval;
            CommandWaitRetryCount = DefaultCommandWaitRetryCount;
            CommandFile = DefaultCommandFile;
            WatchDogInterval = DefaultWatchDogInterval;
            MessagePollingInterval = DefaultMessagePollingInterval;
            //WaitIntervalAfterReset = DefaultWaitIntervalAfterReset;
            DeleteReceivedMessage = false;
            ConcatenateMessage = true;
            DisablePinCheck = false;
            DisableWatchDog = false;
            CheckNetworkRegistrationOnStartup = true;
            GatewayId = string.Empty;
            ResetGatewayAfterTimeout = false;
            AutoDisconnectIncomingCall = false;

            ProviderAPN = string.Empty;
            ProviderAPNAccount = string.Empty;
            ProviderAPNPassword = string.Empty;
            ProviderMMSC = string.Empty;
            ProviderWAPGateway = string.Empty;
            DataCompressionControl = false;
            HeaderCompressionControl = false;
            InternetProtocol = InternetProtocol.IP;
            UserAgent = string.Empty;
            XWAPProfile = string.Empty;

            Prefixes = new List<string>(1);

            // Set to false by default
            PersistenceQueue = false;

            // Set to empty by default
            PersistenceFolder = string.Empty;

            // Default to numeric
            OperatorSelectionFormat = NetworkOperatorFormat.Numeric;

            // Default to false - not to skip operator selection
            SkipOperatorSelection = false;

            // Default to false
            EncodedUssdCommand = false;

            // Set to empty
            Model = string.Empty;
            DeviceName = string.Empty;
            
        }

        
        /// <summary>
        /// Gets or sets the gateway id.
        /// </summary>
        /// <value>The gateway id.</value>
        [XmlAttribute]
        public string GatewayId
        {
            get;
            set;
        }

        /// <summary>
        /// Initialization string to be sent during connection
        /// </summary>
        /// <value>Initialization string</value>
        [XmlAttribute]
        public string InitializationString
        {
            get;
            set;
        }

        /// <summary>
        /// Personal Identification Number (PIN) set at the gateway
        /// </summary>
        /// <value>PIN</value>
        [XmlAttribute]
        public string Pin
        {
            get;
            set;
        }

        /// <summary>
        /// Gateway model. If you know the model, you should pass it in.
        /// E.g. Nokia, Sony, Motorola, Wavecomm
        /// </summary>
        /// <value></value>
        [XmlAttribute]
        public string Model
        {
            get;
            set;
        }

        /// <summary>
        /// Number of times to retries if message sending failed. Default to 3
        /// </summary>
        /// <value>Send retries</value>
        [XmlAttribute]
        public int SendRetries
        {
            get;
            set;
        }

        /// <summary>
        /// Send wait interval in milliseconds between consecutive message sending
        /// </summary>
        /// <value>Send wait interval</value>
        [XmlAttribute]
        public int SendWaitInterval
        {
            get;
            set;
        }

        /// <summary>
        /// Command file to read for diagnostic
        /// </summary>
        /// <value>Diagnostic command file</value>
        [XmlAttribute]
        public string CommandFile
        {
            get;
            set;
        }

        /// <summary>
        /// Watch dog interval in milliseconds
        /// </summary>
        /// <value>Watch dog interval</value>
        [XmlAttribute]
        public int WatchDogInterval
        {
            get;
            set;
        }

        /// <summary>
        /// Interval in milliseconds to poll for new messages.
        /// Default to 5 seconds
        /// </summary>
        /// <value>Message pollling interval in milliseconds</value>
        [XmlAttribute]
        public int MessagePollingInterval
        {
            get;
            set;
        }


        /// <summary>
        /// Gets or sets the wait interval after reset.
        /// </summary>
        /// <value>
        /// The wait interval after reset.
        /// </value>
        /*
        [XmlAttribute]
        public int WaitIntervalAfterReset
        {
            get;
            set;
        }
        */

        /// <summary>
        /// Flag to delete received message after message received event is raised.
        /// Default to false
        /// </summary>
        /// <value>Delete received message flag</value>
        [XmlAttribute]
        public bool DeleteReceivedMessage
        {
            get;
            set;
        }

        /// <summary>
        /// Concatenate multipart messages read from the gateway. Default to true
        /// </summary>
        /// <value>Concatenate message flag</value>
        [XmlAttribute]
        public bool ConcatenateMessage
        {
            get;
            set;
        }

        /// <summary>
        /// Flag to indicate if need to check SIM PIN status.
        /// Default to false
        /// </summary>
        /// <value>SIM status check flag</value>
        [XmlAttribute]
        public bool DisablePinCheck
        {
            get;
            set;
        }

        /// <summary>
        /// Flag to enable/disable the watch dog process.
        /// Default to false
        /// </summary>
        /// <value>Watch dog flag</value>
        [XmlAttribute]
        public bool DisableWatchDog
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether [check network registration on startup].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [check network registration on startup]; otherwise, <c>false</c>.
        /// </value>
        [XmlAttribute]
        public bool CheckNetworkRegistrationOnStartup
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether [reset gateway after timeout].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [reset gateway after timeout]; otherwise, <c>false</c>.
        /// </value>
        [XmlAttribute]
        public bool ResetGatewayAfterTimeout
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether to automatic disconnect incoming call.
        /// Default to false
        /// </summary>
        /// <value>
        /// 	<c>true</c> if auto disconnect incoming call; otherwise, <c>false</c>.
        /// </value>
        [XmlAttribute]
        public bool AutoDisconnectIncomingCall
        {
            get;
            set;
        }

        // ------------------- MMS related configurations ----------------------------------//

        /// <summary>
        /// Gets or sets the name of the device.
        /// </summary>
        /// <value>The name of the device.</value>
        [XmlAttribute]
        public string DeviceName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the provider MMSC.
        /// </summary>
        /// <value>The provider MMSC.</value>
        [XmlAttribute]
        public string ProviderMMSC
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the provider APN.
        /// </summary>
        /// <value>The provider APN.</value>
        [XmlAttribute]
        public string ProviderAPN
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the provider WAP gateway.
        /// </summary>
        /// <value>The provider WAP gateway.</value>
        [XmlAttribute]
        public string ProviderWAPGateway
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the provider APN account.
        /// </summary>
        /// <value>The provider APN account.</value>
        [XmlAttribute]
        public string ProviderAPNAccount
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the provider APN password.
        /// </summary>
        /// <value>The provider APN password.</value>
        [XmlAttribute]
        public string ProviderAPNPassword
        {
            get;
            set;
        }


        /// <summary>
        /// Gets or sets a value indicating whether data compression control is on or off
        /// </summary>
        /// <value>
        /// 	<c>true</c> if data compression control is on; otherwise, <c>false</c>.
        /// </value>
        [XmlAttribute]
        public bool DataCompressionControl
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether header compression control is on or off
        /// </summary>
        /// <value>
        /// 	<c>true</c> if header compression control is on; otherwise, <c>false</c>.
        /// </value>
        [XmlAttribute]
        public bool HeaderCompressionControl
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the internet protocol.
        /// </summary>
        /// <value>
        /// The internet protocol.
        /// </value>
        [XmlAttribute]
        public InternetProtocol InternetProtocol
        {
            get;
            set;
        }


        /// <summary>
        /// Gets or sets the user agent.
        /// </summary>
        /// <value>
        /// The user agent.
        /// </value>
        [XmlAttribute]
        public string UserAgent
        {
            get;
            set;
        }


        /// <summary>
        /// Gets or sets the XWAP profile.
        /// </summary>
        /// <value>
        /// The XWAP profile.
        /// </value>
        [XmlAttribute]
        public string XWAPProfile
        {
            get;
            set;
        }


        /// <summary>
        /// Gets or sets the prefixes. This is used by the number prefix router.
        /// </summary>
        /// <value>List of prefixes</value>
        [XmlAttribute]
        public List<string> Prefixes
        {
            get;
            set;
        }

        
        


        /// <summary>
        /// If set to true, messages in queue or delayed queue are persisted
        /// </summary>
        /// <value><c>true</c> if messages will be persisted; otherwise, <c>false</c>.</value>
        [XmlAttribute]
        public bool PersistenceQueue
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the persistence folder. Default to the current folder
        /// </summary>
        /// <value>The persistence folder.</value>
        [XmlAttribute]
        public string PersistenceFolder
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the operator selection format.
        /// </summary>
        /// <value>The operator selection format.</value>
        [XmlAttribute]
        public NetworkOperatorFormat OperatorSelectionFormat
        {
            get;
            set;
        }


        /// <summary>
        /// Gets or sets a value indicating whether to skip operation selection.
        /// </summary>
        /// <value>
        ///   <c>true</c> if skip operator selection; otherwise, <c>false</c>.
        /// </value>
        [XmlAttribute]
        public bool SkipOperatorSelection
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the USSD command and response should be encoded.
        /// </summary>
        /// <value>
        ///   <c>true</c> if USSD command should be encoded; otherwise, <c>false</c>.
        /// </value>
        [XmlAttribute]
        public virtual bool EncodedUssdCommand
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating the command wait interval in milliseconds.
        /// This multiplies by the <see cref="CommandWaitRetryCount"/> gives the 
        /// number of milliseconds for the command time out.
        /// </summary>
        /// <value>
        ///   Number of milliseconds. Default to 300.
        /// </value>
        [XmlAttribute]
        public virtual int CommandWaitInterval
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating the command wait retry count.
        /// This multiplies by the <see cref="CommandWaitInterval"/> gives the 
        /// number of milliseconds for the command time out.
        /// </summary>
        /// <value>
        ///   Default to 100.
        /// </value>
        [XmlAttribute]
        public virtual int CommandWaitRetryCount
        {
            get;
            set;
        }

    }
}
