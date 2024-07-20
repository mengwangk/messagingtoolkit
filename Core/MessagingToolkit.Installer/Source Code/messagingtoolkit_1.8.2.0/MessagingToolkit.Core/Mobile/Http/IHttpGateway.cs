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
using MessagingToolkit.Core.Service;
using MessagingToolkit.Core.Mobile.Message;

namespace MessagingToolkit.Core.Mobile.Http
{
    /// <summary>
    /// HTTP gateway interface to the mobile device.
    /// </summary>
    public interface IHttpGateway : IGateway, IDisposable
    {
        #region ========== Properties signatures =====================================================


        /// <summary>
        /// Return the gateway configuration
        /// </summary>
        /// <value>Configuration object</value>
        HttpGatewayConfiguration Configuration
        {
            get;
        }

        /// <summary>
        /// Statistics for incoming and outgoing messages
        /// </summary>
        /// <value>Statistics</value>
        Statistics Statistics
        {
            get;
        }


        /// <summary>
        /// Gateway attributes. See <see cref="GatewayAttribute"/>
        /// </summary>
        /// <value>Gateway attributes</value>
        GatewayAttribute Attributes
        {
            get;
            set;
        }

        /// <summary>
        /// Return the license associated with this software
        /// </summary>
        /// <value>License</value>
        License License
        {
            get;
        }


        /// <summary>
        /// If set to true, messages in queue or delayed queue are persisted
        /// </summary>
        /// <value><c>true</c> if persistence is required; otherwise, <c>false</c>.</value>
        bool PersistenceQueue
        {
            get;
            set;
        }


        /// <summary>
        /// Gets or sets the base persistence folder
        /// </summary>
        /// <value>Persistence folder</value>
        string PersistenceFolder
        {
            get;
            set;
        }


        /// <summary>
        /// Gets the device network information.
        /// </summary>
        /// <value>
        /// The device network information.
        /// </value>
        DeviceNetworkInformation DeviceNetworkInformation
        {
            get;
        }

        /// <summary>
        /// Gets the device battery information.
        /// </summary>
        /// <value>
        /// The device battery information.
        /// </value>
        DeviceBatteryInformation DeviceBatteryInformation
        {
            get;
        }


        /// <summary>
        /// Indicate if all outbound messages should be validated on their statuses.
        /// Should probably always set to true if you want to get the message status events raised.
        /// </summary>
        /// <value>
        /// <c>true</c> if need to validate outbound messages; otherwise, <c>false</c>.
        /// </value>
        bool ValidateOutboundMessage { get; set; }


        /// <summary>
        /// Polling for new unread messages and raise message received event.
        /// The polling interval can be specified using <see cref="HttpGatewayConfiguration"/>.
        /// Default to false
        /// </summary>
        /// <value></value>
        bool PollNewMessages
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value to define whether the message queue is enabled (sends out messages)
        /// </summary>
        /// <value>if true then the unsent messages in the queue will be sent immediately based on priority of the messages</value>
        bool IsMessageQueueEnabled
        {
            get;
            set;
        }


        /// <summary>
        /// Clear all the queued messages.
        /// </summary>
        /// <returns>true it is successful</returns>
        bool ClearQueue();

        /// <summary>
        /// Get number of queued items.
        /// </summary>
        /// <returns>Number of queued messages</returns>
        int GetQueueCount();

        #endregion ========================================================================================


        #region ========== Public Methods ================================================================

        /// <summary>
        /// Connect to the gateway
        /// </summary>
        /// <returns>true if success, false otherwise</returns>
        bool Connect();

        /// <summary>
        /// Disconnect from the gateway.
        /// </summary>
        /// <returns>true if success, false otherwise.</returns>
        bool Disconnect();

        /// <summary>
        /// Gets the messages using the specified query. Return an empty list if there is any errors.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns>List of messages.</returns>
        List<DeviceMessage> GetMessages(GetMessageQuery query);

        /// <summary>
        /// Gets all the messages. Return an empty list if there is any errors.
        /// </summary>
        /// <returns>List of messages.</returns>
        List<DeviceMessage> GetMessages();


        /// <summary>
        /// <para>
        /// Send message to the device through HTTP POST method. If the message is going to be sent immediately (not scheduled to be sent later) 
        /// the MessageSending event will NOT be triggered. 
        /// </para>
        /// 
        /// <para>
        /// The event MessageSent, MessageDelivered and MessageSendingFailed will be triggered depending on the sending status.
        /// </para>
        /// 
        /// </summary>
        /// <param name="message">Message to be sent.</param>
        /// <returns>true if successfully sent. If false then you need to check the last exception thrown</returns>
        bool Send(IMessage message);

        /// <summary>
        /// Gets the message status.
        /// </summary>
        /// <param name="id">The message id.</param>
        /// <returns>Message status</returns>
        MessageStatusInformation GetMessageStatus(string id);

        /// <summary>
        /// <para>
        /// Send message through message queue to the device through HTTP POST method. The message queue will be processed in the background
        /// and message events will be triggered.
        /// </para>
        /// 
        /// <para>
        /// The event MessagingSending,  MessageSent, MessageDelivered and MessageSendingFailed will be triggered depending on the sending status.
        /// </para>
        /// 
        /// </summary>
        /// <param name="message">Message to be sent.</param>
        /// <returns>true if successfully sent. If false then you need to check the last exception thrown</returns>
        bool SendToQueue(IMessage message);

        /// <summary>
        /// Deletes the message.
        /// </summary>
        /// <param name="ids">List of message ids.</param>
        /// <returns>Number of deleted messages.</returns>
        int DeleteMessage(List<string> ids);


        /// <summary>
        /// Deletes the message.
        /// </summary>
        /// <param name="threadId">The message thread id.</param>
        /// <returns>Number of deleted messages.</returns>
        int DeleteMessage(string threadId);


        #endregion =======================================================================================


        #region ========== Public Events =================================================================

        /// <summary>
        /// Message sending event
        /// </summary>
        event MessageEventHandler MessageSending;

        /// <summary>
        /// Message sent event
        /// </summary>
        event MessageEventHandler MessageSent;


        /// <summary>
        /// Message delivered event
        /// </summary>
        event MessageEventHandler MessageDelivered;


        /// <summary>
        /// Message sending failed event
        /// </summary>
        event MessageErrorEventHandler MessageSendingFailed;

        /// <summary>
        /// Message received event
        /// </summary>
        event NewMessageReceivedEventHandler MessageReceived;

        /// <summary>
        /// Occurs when gateway is disconnected.
        /// </summary>
        event DisconnectedEventHandler GatewayDisconnected;



        #endregion =======================================================================================

    }
}
