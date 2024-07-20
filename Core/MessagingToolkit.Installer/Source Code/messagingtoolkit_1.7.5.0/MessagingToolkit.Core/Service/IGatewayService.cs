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
using System.Collections.Specialized;

using MessagingToolkit.Core.Base;
using MessagingToolkit.Core.Mobile.Message;
using MessagingToolkit.Core.Mobile;

namespace MessagingToolkit.Core.Service
{
    /// <summary>
    /// Public interface of the message gateway service
    /// </summary>
    public interface IGatewayService: IGateway
    {
        #region ============= Public Properties================================================

        /// <summary>
        /// Gateway pool
        /// </summary>
        /// <value>List of gateways</value>
        List<IGateway> Gateways
        {
            get;
            set;
        }

        /// <summary>
        /// Load balancer 
        /// </summary>
        /// <value>Load balancer</value>
        LoadBalancer LoadBalancer
        {
            get;
            set;
        }

        /// <summary>
        /// Message router
        /// </summary>
        /// <value>Message router</value>
        Router Router
        {
            get;
            set;
        }

        /// <summary>
        /// If set to true, a watch dog thread is started to check all
        /// the gateways are started, and will try to restart the gateway
        /// if it is closed or disconnected
        /// </summary>
        /// <value>Default is false</value>
        bool MonitorService
        {
            get;
            set;
        }

        #endregion ============================================================================


        #region ============= Public Methods ==================================================

        /// <summary>
        /// Add a gateway
        /// </summary>
        /// <param name="gateway">Gateway instance</param>
        /// <returns>true if added successfully</returns>
        bool Add(IGateway gateway);

        /// <summary>
        /// Remove a gateway
        /// </summary>
        /// <param name="gatewayId">Gateway id</param>
        /// <returns>true if removed successfully</returns>
        bool Remove(string gatewayId);


        /// <summary>
        /// Remove all gateways
        /// </summary>
        /// <returns>true if removed successfully</returns>
        bool RemoveAll();

        /// <summary>
        /// Shutdown the service
        /// </summary>
        /// <returns>true if shutdown successfully</returns>
        bool Shutdown();

        /// <summary>
        /// Find a gateway
        /// </summary>
        /// <param name="gatewayId">Gateway id</param>
        /// <param name="gateway">The found gateway</param>
        /// <returns>true if gateway is found</returns>
        bool Find(string gatewayId, out IGateway gateway);

        /// <summary>
        /// Send message
        /// </summary>
        /// <param name="message">Message</param>
        /// <returns>true if message is sent successfully to the gateway</returns>
        bool SendMessage(IMessage message);

        /// <summary>
        /// Send message using a particular gateway
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="gatewayId">Gateway id</param>
        /// <returns>true if message is sent successfully to the gateway</returns>
        bool SendMessage(IMessage message, string gatewayId);

                
        /// <summary>
        /// Send a list of messages
        /// </summary>
        /// <param name="messages">Messages</param>
        /// <returns>Number of messages sent</returns>
        int SendMessages(List<IMessage> messages);


        /// <summary>
        /// Send a list of messages using a specific gateway
        /// </summary>
        /// <param name="messages">Messages</param>
        /// <param name="gatewayId">Gateway id</param>
        /// <returns>Number of messages sent</returns>
        int SendMessages(List<IMessage> messages, string gatewayId);
        

        /// <summary>
        /// Creates a destination group. A group can hold an unlimited number of
	    /// recipients. Sending a message to a predefined group expands and sends the
	    /// message to all numbers defined by the group
        /// </summary>
        /// <param name="groupName">Group name</param>
        /// <returns>true if group is created successfully</returns>
        bool CreateGroup(string groupName);

        /// <summary>
        /// Removes a group
        /// </summary>
        /// <param name="groupName">Group name</param>
        /// <returns>true if the removal was a success.</returns>
        bool RemoveGroup(string groupName);

        /// <summary>
        /// Expands a group to its recipient numbers.
        /// </summary>
        /// <param name="groupName">Group name</param>
        /// <returns>
        /// A list of the numbers that this group represents. If the group is
        /// not defined, an empty list is returned.
        /// </returns>
        List<string> ExpandGroup(string groupName);

        /// <summary>
        /// Adds a number to the specified group
        /// </summary>
        /// <param name="groupName">Group name</param>
        /// <param name="number">The number</param>
        /// <returns>
        /// true if the number is added. false if the group is not found.
        /// </returns>
        bool AddToGroup(string groupName, string number);

        /// <summary>
        /// Removes a number from the specified group
        /// </summary>
        /// <param name="groupName">Group name</param>
        /// <param name="number">The number</param>
        /// <returns>
        /// true if the number was removed. False if the group or the number
        /// is not found.
        /// </returns>
        bool RemoveFromGroup(string groupName, string number);


        /// <summary>
        /// Read incoming messages from all gateways
        /// </summary>
        /// <param name="messageType">Message type. See <see cref="MessageStatusType"/></param>
        /// <returns>List of messages read</returns>
        List<MessageInformation> ReadMessages(MessageStatusType messageType);
     

        /// <summary>
        /// Read incoming messages from a particular gateway
        /// </summary>
        /// <param name="messageType">Message type. See <see cref="MessageStatusType"/></param>
        /// <param name="gatewayId">Gateway id</param>
        /// <returns>List of messages read</returns>
        List<MessageInformation> ReadMessages(MessageStatusType messageType, string gatewayId);

        /// <summary>
        /// Read incoming messages from a particular gateway
        /// </summary>
        /// <param name="messageType">Message type. See <see cref="MessageStatusType"/></param>
        /// <param name="gateway">Gateway</param>
        /// <returns>The number of messages read</returns>
        List<MessageInformation> ReadMessages(MessageStatusType messageType, IGateway gateway);


        #endregion ==========================================================================

    }
}
