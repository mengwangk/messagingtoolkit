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
using MessagingToolkit.Core.Mobile.Event;
using MessagingToolkit.Core.Log;

namespace MessagingToolkit.Core.Mobile.Http
{
    /// <summary>
    /// Configuration for the HTTP mobile gateway.
    /// </summary>
    [System.Reflection.ObfuscationAttribute(Feature = "renaming", ApplyToMembers = true)]
    [Serializable]
    public class HttpGatewayConfiguration : BaseConfiguration, IConfiguration
    {
        /// <summary>
        /// The default port.
        /// </summary>
        private const int DefaultPort = 1688;


        #region =============== Private Constants ============================================================

        /// <summary>
        /// Gateway connected event
        /// </summary>
        public event ConnectedEventHandler GatewayConnected;


        #endregion ===========================================================================================




        #region =============== Constructor ==================================================================

        /// <summary>
        /// Private constructor.
        /// </summary>
        private HttpGatewayConfiguration()
            : base()
        {
            // Set default values
            this.PersistenceQueue = false;
            this.PersistenceFolder = string.Empty;
            this.GatewayId = string.Empty;
            this.IPAddress = string.Empty;
            this.Port = DefaultPort;
            this.MarkReadMessage = true;
            this.DeleteReceivedMessage = false;
        }


        #endregion ============================================================================================



        #region ============== Public Properties ==============================================================

        /// <summary>
        /// If set to true, messages in queue or delayed queue are persisted
        /// </summary>
        /// <value><c>true</c> if messages will be persisted; otherwise, <c>false</c>.</value>
        [XmlAttribute]
        internal bool PersistenceQueue
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the persistence folder. Default to the current folder
        /// </summary>
        /// <value>The persistence folder.</value>
        [XmlAttribute]
        internal string PersistenceFolder
        {
            get;
            set;
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
        /// IP address of the mobile devices.
        /// </summary>
        /// <value>IP address.</value>
        [XmlAttribute]
        public string IPAddress
        {
            get;
            set;
        }

        /// <summary>
        /// Port used
        /// </summary>
        /// <value>Port. Default to 1688.</value>
        [XmlAttribute]
        public int Port
        {
            get;
            set;
        }


        /// <summary>
        /// Gets or sets the name of the user.
        /// </summary>
        /// <value>
        /// The name of the user.
        /// </value>
        [XmlAttribute]
        public string UserName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        /// <value>
        /// The password.
        /// </value>
        [XmlAttribute]
        public string Password
        {
            get;
            set;
        }

        /// <summary>
        /// Mark the unread messages as read after receiving.
        /// </summary>
        /// <value>
        /// true to mark the messages as read, false means no action.
        /// </value>
        [XmlAttribute]
        public bool MarkReadMessage
        {
            get;
            set;
        }

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


        #endregion ===========================================================================================


        #region ============== Public method   ===============================================================

        /// <summary>
        /// Override ToString method
        /// </summary>
        /// <returns>A string representation of the configuration values.</returns>
        override public string ToString()
        {
            string str = String.Empty;
            str = String.Concat(str, "LogFile = ", LogFile, "\r\n");
            str = String.Concat(str, "LicenseKey = ", LicenseKey, "\r\n");
            str = String.Concat(str, "LogLevel = ", LogLevel, "\r\n");
            str = String.Concat(str, "LogQuotaFormat = ", LogQuotaFormat, "\r\n");
            str = String.Concat(str, "LogSizeMax = ", LogSizeMax, "\r\n");
            str = String.Concat(str, "LogNameFormat = ", LogNameFormat, "\r\n");
            str = String.Concat(str, "LogLocation = ", LogLocation, "\r\n");
            str = String.Concat(str, "DebugMode = ", DebugMode, "\r\n");
            str = String.Concat(str, "SendRetries = ", SendRetries, "\r\n");
            str = String.Concat(str, "MessagePollingInterval = ", MessagePollingInterval, "\r\n");
            str = String.Concat(str, "PersistenceQueue = ", PersistenceQueue, "\r\n");
            str = String.Concat(str, "PersistenceFolder = ", PersistenceFolder, "\r\n");
            str = String.Concat(str, "GatewayId = ", GatewayId, "\r\n");
            str = String.Concat(str, "IPAddress = ", IPAddress, "\r\n");
            str = String.Concat(str, "Port = ", Port, "\r\n");
            str = String.Concat(str, "UserName = ", UserName, "\r\n");
            str = String.Concat(str, "Password = ", Password, "\r\n");
            str = String.Concat(str, "MarkReadMessage = ", MarkReadMessage, "\r\n");
            str = String.Concat(str, "DeleteReceivedMessage = ", DeleteReceivedMessage, "\r\n");
            return str;
        }

        #endregion ===========================================================================================

        #region ============== Factory method   ===============================================================

        /// <summary>
        /// Static factory to create the mobile HTTP gateway configuration.
        /// </summary>
        /// <returns>A new instance of mobile HTTP configuration</returns>
        public static HttpGatewayConfiguration NewInstance()
        {
            return new HttpGatewayConfiguration();
        }

        #endregion ===========================================================================================

    }
}
