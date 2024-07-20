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

using MessagingToolkit.Core.Base;
using MessagingToolkit.Core.Log;
using MessagingToolkit.Core.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MessagingToolkit.Core.Mobile.Http
{
    /// <summary>
    /// Default HTTP gateway.
    /// </summary>
    public class DefaultHttpGateway : IHttpGateway
    {

        #region ====================== Methods =================================================

        /// <summary>
        /// Clears the log file content
        /// </summary>
        public void ClearLog() { }

        public List<DeviceMessage> GetMessages(GetMessageQuery query)
        {
            return new List<DeviceMessage>();
        }

        public List<DeviceMessage> GetMessages()
        {
            return new List<DeviceMessage>();
        }

        public bool Connect()
        {
            return false;
        }

        public bool Disconnect()
        {
            return false;
        }

        public bool Send(IMessage message)
        {
            return false;
        }

        public bool SendToQueue(IMessage message)
        {
            return false;
        }

        public MessageStatusInformation GetMessageStatus(string id)
        {
            return new MessageStatusInformation();
        }

        public int DeleteMessage(List<string> ids)
        {
            return 0;
        }

        public int DeleteMessage(string threadId)
        {
            return 0;
        }

        #endregion ===================== Methods ================================================




        #region ================================= events ========================================



        public event MessageEventHandler MessageSending
        {
            add { }
            remove { }
        }

        public event MessageEventHandler MessageSent
        {
            add { }
            remove { }
        }

        public event MessageEventHandler MessageDelivered
        {
            add { }
            remove { }
        }

        public event MessageErrorEventHandler MessageSendingFailed
        {
            add { }
            remove { }
        }

        public event NewMessageReceivedEventHandler MessageReceived
        {
            add { }
            remove { }
        }

        public event DisconnectedEventHandler GatewayDisconnected
        {
            add { }
            remove { }
        }


        #endregion ================================ events ========================================


        #region =========== Public Properties =====================================================

        public Statistics Statistics
        {
            get
            {
                return new Statistics();
            }
        }


        public HttpGatewayConfiguration Configuration
        {
            get
            {
                return HttpGatewayConfiguration.NewInstance();
            }
        }

        public GatewayAttribute Attributes
        {
            get
            {
                return GatewayAttribute.Send;
            }
            set
            {
            }
        }

        /// <summary>
        /// Return the license associated with this software
        /// </summary>
        /// <value>License</value>
        public virtual License License
        {
            get
            {
                HttpGatewayConfiguration config = HttpGatewayConfiguration.NewInstance();
                return new License(config);
            }
        }


        /// <summary>
        /// Gateway id
        /// </summary>
        /// <value>gateway id</value>
        public string Id
        {
            get;
            set;
        }

        /// <summary>
        /// Gateway status
        /// </summary>
        /// <value>Gateway status</value>
        public GatewayStatus Status
        {
            get;
            set;
        }


        /// <summary>
        /// Return the last exception encountered
        /// </summary>
        /// <value>Exception</value>
        public Exception LastError
        {
            get
            {
                return null;
            }
        }

        /// <summary>
        /// Set the logging level.
        /// </summary>
        /// <value>LogLevel enum. See <see cref="LogLevel"/></value>
        public virtual LogLevel LogLevel
        {
            get
            {
                return LogLevel.Error;
            }
            set
            {

            }
        }

        /// <summary>
        /// Log destination
        /// </summary>
        /// <value>Log destination. See <see cref="LogDestination"/></value>
        public virtual LogDestination LogDestination
        {
            get
            {
                return LogDestination.File;
            }
            set
            {

            }
        }

        /// <summary>
        /// Gets the log file.
        /// </summary>
        /// <value>The log file.</value>
        public virtual string LogFile
        {
            get
            {
                return string.Empty;
            }
        }

        public bool PersistenceQueue
        {
            get
            {
                return false;
            }
            set
            {
            }
        }

        public string PersistenceFolder
        {
            get
            {
                return string.Empty;
            }
            set
            {
            }
        }


        public virtual bool ValidateOutboundMessage
        {
            get
            {
                return false;
            }
            set
            {

            }
        }

        public bool PollNewMessages
        {
            get
            {
                return false;
            }
            set
            {

            }
        }

        public bool IsMessageQueueEnabled
        {
            get
            {
                return false;
            }
            set
            {
            }
        }

        #endregion ===========================================================================================


        #region =========== Public Method  ===================================================================

        public void Dispose() { }

        /// <summary>
        /// Reset the exception
        /// </summary>
        public void ClearError()
        {

        }

        public bool ClearQueue()
        {
            return false;
        }

        public int GetQueueCount()
        {
            return 0;
        }

        public DeviceBatteryInformation DeviceBatteryInformation
        {
            get
            {
                return new DeviceBatteryInformation();
            }

        }

        public DeviceNetworkInformation DeviceNetworkInformation
        {
            get
            {
                return new DeviceNetworkInformation();
            }
        }



        #endregion ===========================================================================================




    }
}
