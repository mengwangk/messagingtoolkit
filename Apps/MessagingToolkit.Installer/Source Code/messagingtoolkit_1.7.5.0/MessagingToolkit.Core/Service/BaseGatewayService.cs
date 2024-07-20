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
using System.Threading;
using System.Runtime.CompilerServices;

using MessagingToolkit.Core.Base;
using MessagingToolkit.Core.Properties;
using MessagingToolkit.Core.Mobile;
using MessagingToolkit.Core.Mobile.Message;
using MessagingToolkit.Core.Log;

namespace MessagingToolkit.Core.Service
{
    /// <summary>
    /// Base class for message gateway service
    /// </summary>
    /// <typeparam name="T">Gateway service type</typeparam>
    public abstract class BaseGatewayService<T> : BaseGateway<T>
    {
        #region =========== Private Constants =====================================================

        /// <summary>
        /// Watch dog interval. Default to 15 seconds
        /// </summary>
        private const int WatchDogInterval = 15000;

        /// <summary>
        /// Maximum gateways that can be added for unlicensed copy
        /// </summary>
        private const int UnlicensedMaximumGateway = 2;

        #endregion ============================================================================

        #region =========== Private Variables =====================================================

        /// <summary>
        /// Check the gateway connectivity
        /// </summary>
        private Thread watchDog;

        /// <summary>
        /// Flag to indicate if the gateway status must be checked
        /// </summary>
        private bool monitorService;


        /// <summary>
        /// Groups of destination numbers
        /// </summary>
        private List<Group> groups;


        #endregion ============================================================================

        #region ============= Constructor ====================================================

        /// <summary>
        /// Constructor
        /// </summary>
        protected BaseGatewayService()
            : base()
        {
            // Create an empty list
            Gateways = new List<IGateway>();

            // Default to false, do not monitor
            monitorService = false;

            // Create an empty group list
            groups = new List<Group>();
        }

        #endregion ===========================================================================


        #region ============= Public Properties ==============================================

        /// <summary>
        /// Gateway pool
        /// </summary>
        /// <value>List of gateways</value>
        public List<IGateway> Gateways
        {
            get;
            set;
        }

        /// <summary>
        /// Load balancer 
        /// </summary>
        /// <value>Load balancer</value>
        public LoadBalancer LoadBalancer
        {
            get;
            set;
        }

        /// <summary>
        /// Message router
        /// </summary>
        /// <value>Message router</value>
        public Router Router
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
        public bool MonitorService
        {
            get
            {
                return this.monitorService;
            }
            set
            {
                this.monitorService = value;

                try
                {
                    if (monitorService)
                    {                       
                        if (watchDog == null || !watchDog.IsAlive)
                        {
                            // Start the polling thread
                            watchDog = new Thread(new ThreadStart(this.WatchDog));
                            watchDog.IsBackground = true;
                            watchDog.Start();
                        }                        
                    }
                    else
                    {  
                        try
                        {
                            if (watchDog != null)
                            {
                                //watchDog.Interrupt();
                                //if (!watchDog.Join(500))
                                //{
                                    watchDog.Abort();
                                //}
                                watchDog = null;
                            }
                        }
                        catch (Exception e) { }                       
                    }
                }
                catch (Exception ex)
                {
                    this.exception = ex;
                }
            }
        }

        #endregion ============================================================================


        #region ============= Public Methods ==================================================

        /// <summary>
        /// Add a gateway
        /// </summary>
        /// <param name="gateway">Gateway instance</param>
        /// <returns>true if added successfully</returns>
        public virtual bool Add(IGateway gateway)
        {
            IMobileGateway mobileGateway = (IMobileGateway)gateway;
            if (!mobileGateway.Connected)
            {
                exception = new GatewayException(Resources.NotConnectedException);
                return false;
            }
            if (Gateways.Count == UnlicensedMaximumGateway)
            {
                // Check if license is valid
                if (!mobileGateway.License.Valid)
                {
                    exception = new GatewayException(string.Format(Resources.MaximumGatewayException, UnlicensedMaximumGateway));
                    return false;
                }
            }
            Gateways.Add(gateway);            
            return true;
        }

        /// <summary>
        /// Remove a gateway
        /// </summary>
        /// <param name="gatewayId">Gateway id</param>
        /// <returns>true if removed successfully</returns>
        public virtual bool Remove(string gatewayId)
        {
            if (string.IsNullOrEmpty(gatewayId)) return false;
            foreach (IGateway gateway in Gateways)
            {
                if (gatewayId.Equals(gateway.Id, StringComparison.OrdinalIgnoreCase))
                {
                    IMobileGateway mobileGateway = (IMobileGateway)gateway;
                    mobileGateway.Disconnect();
                    mobileGateway = null;
                    break;
                }
            }
            Gateways.RemoveAll(delegate(IGateway gw) { return gatewayId.Equals(gw.Id, StringComparison.OrdinalIgnoreCase); });
            return true;
        }


        /// <summary>
        /// Remove all gateways
        /// </summary>
        /// <returns>true if removed successfully</returns>
        public virtual bool RemoveAll()
        {
            foreach (IGateway gateway in Gateways)
            {
                IMobileGateway mobileGateway = (IMobileGateway)gateway;
                mobileGateway.Disconnect();
                mobileGateway = null;
            }
            Gateways.Clear();

            try
            {
                if (watchDog != null && watchDog.IsAlive)
                {
                    //watchDog.Interrupt();
                    //if (!watchDog.Join(10))
                    //{
                        watchDog.Abort();
                    //}
                    watchDog = null;
                }
            }
            catch (Exception e) { }

            return true;
        }

        /// <summary>
        /// Shutdown the service
        /// </summary>
        /// <returns>true if shutdown successfully</returns>
        public virtual bool Shutdown()
        {
            return RemoveAll();
        }

        /// <summary>
        /// Find a gateway
        /// </summary>
        /// <param name="gatewayId">Gateway id</param>
        /// <param name="gateway">The found gateway</param>
        /// <returns>true if gateway is found</returns>
        public virtual bool Find(string gatewayId, out IGateway gateway)
        {
            gateway = null;
            if (string.IsNullOrEmpty(gatewayId)) return false;
            gateway = Gateways.Find(delegate(IGateway gw) { return gatewayId.Equals(gw.Id, StringComparison.OrdinalIgnoreCase); });
            if (gateway == null) return false;
            return true;
        }

        /// <summary>
        /// Send message
        /// </summary>
        /// <param name="message">Message</param>
        /// <returns>true if message is sent successfully to the gateway</returns>
        public virtual bool SendMessage(IMessage message)
        {
            Sms sms = (Sms)message;
            try
            {                                     
                List<String> recipients = ExpandGroup(sms.DestinationAddress);
			    if (recipients.Count() == 0) 
                {
                    IGateway gateway = RouteMessage(message);
                    if (gateway != null)
                    {
                        IMobileGateway mobileGateway = (IMobileGateway)gateway;
                        if (!mobileGateway.SendToQueue(message))
                        {
                            throw mobileGateway.LastError;
                        }
                    }                   
                }                       
			    else
			    {
				    List<IMessage> groupMessages = new List<IMessage>();
				    foreach (string phoneNumber in recipients)
				    {
                        Sms newMessage = ObjectClone.DeepCopy<Sms>(sms);
					    newMessage.DestinationAddress = phoneNumber;                           
					    groupMessages.Add(newMessage);
				    }
				    SendMessages(groupMessages);
				    return true;                                              
			    }
                
            }
            catch (Exception e)
            {
                Logger.LogThis(string.Format("Unable to send message to {0}. {1}", sms.DestinationAddress, e.Message) , LogLevel.Error);
                this.exception = e;
                return false;
            }
            return true;
        }

        /// <summary>
        /// Send message using a particular gateway
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="gatewayId">Gateway id</param>
        /// <returns>true if message is sent successfully to the gateway</returns>
        public virtual bool SendMessage(IMessage message, string gatewayId)
        {
            message.GatewayId = gatewayId;
            return SendMessage(message);         
        }


        /// <summary>
        /// Send a list of messages
        /// </summary>
        /// <param name="messages">Messages</param>
        /// <returns>Number of messages sent</returns>
        public int SendMessages(List<IMessage> messages)
        {
            int counter = 0;
            foreach (IMessage message in messages)
            {
                if (SendMessage(message))
                {
                    counter++;
                }
            }
            return counter;
        }


        /// <summary>
        /// Send a list of messages using a specific gateway
        /// </summary>
        /// <param name="messages">Messages</param>
        /// <param name="gatewayId">Gateway id</param>
        /// <returns>Number of messages sent</returns>
        public int SendMessages(List<IMessage> messages, string gatewayId)
        {            
            int counter = 0;
            foreach (IMessage message in messages)
            {
                message.GatewayId = gatewayId;
                if (SendMessage(message))
                {
                    counter++;
                }
            }
            return counter;            
        }        

        /// <summary>
        /// Creates a destination group. A group can hold an unlimited number of
        /// recipients. Sending a message to a predefined group expands and sends the
        /// message to all numbers defined by the group
        /// </summary>
        /// <param name="groupName">Group name</param>
        /// <returns>true if group is created successfully</returns>
        public virtual bool CreateGroup(string groupName)
        {
            if (string.IsNullOrEmpty(groupName)) return false;
            this.groups.Add(new Group(groupName));
            return true;
        }

        /// <summary>
        /// Removes a group
        /// </summary>
        /// <param name="groupName">Group name</param>
        /// <returns>true if the removal was a success.</returns>
        public virtual bool RemoveGroup(string groupName)
        {
            if (string.IsNullOrEmpty(groupName)) return false;
            this.groups.RemoveAll(delegate(Group group) { return groupName.Equals(group.Name, StringComparison.OrdinalIgnoreCase); });
            return true;
        }

        /// <summary>
        /// Expands a group to its recipient numbers.
        /// </summary>
        /// <param name="groupName">Group name</param>
        /// <returns>
        /// A list of the numbers that this group represents. If the group is
        /// not defined, an empty list is returned.
        /// </returns>
        public virtual List<string> ExpandGroup(string groupName)
        {
            if (string.IsNullOrEmpty(groupName)) return new List<string>();
            foreach (Group group in this.groups)
            {
                if (groupName.Equals(group.Name, StringComparison.OrdinalIgnoreCase))
                {
                    return group.Numbers;
                }
            }
            return new List<string>();
        }

        /// <summary>
        /// Adds a number to the specified group
        /// </summary>
        /// <param name="groupName">Group name</param>
        /// <param name="number">The number</param>
        /// <returns>
        /// true if the number is added. false if the group is not found.
        /// </returns>
        public virtual bool AddToGroup(string groupName, string number)
        {
            if (string.IsNullOrEmpty(groupName) || string.IsNullOrEmpty(number)) return false;
            foreach (Group group in this.groups)
            {
                if (group.Name.Equals(groupName, StringComparison.OrdinalIgnoreCase))
                {
                    group.AddNumber(number);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Removes a number from the specified group
        /// </summary>
        /// <param name="groupName">Group name</param>
        /// <param name="number">The number</param>
        /// <returns>
        /// true if the number was removed. False if the group or the number
        /// is not found.
        /// </returns>
        public virtual bool RemoveFromGroup(string groupName, string number)
        {
            if (string.IsNullOrEmpty(groupName) || string.IsNullOrEmpty(number)) return false;
            foreach (Group group in this.groups)
            {
                if (group.Name.Equals(groupName, StringComparison.OrdinalIgnoreCase))
                {
                    group.RemoveNumber(number);
                    return true;
                }
            }
            return false;
        }


        /// <summary>
        /// Read incoming messages from all gateways
        /// </summary>
        /// <param name="messageType">Message type. See <see cref="MessageStatusType"/></param>
        /// <returns>List of messages read</returns>
        public List<MessageInformation> ReadMessages(MessageStatusType messageType)
        {
            List<MessageInformation> messageList = new List<MessageInformation>();
            foreach (IGateway gateway in Gateways)
            {
                List<MessageInformation> tmpList = ReadMessages(messageType, gateway);
                messageList.AddRange(tmpList);
            }
            return messageList;
        }

        /// <summary>
        /// Read incoming messages from a particular gateway
        /// </summary>
        /// <param name="messageType">Message type. See <see cref="MessageStatusType"/></param>
        /// <param name="gatewayId">Gateway id</param>
        /// <returns>List of messages read</returns>
        public List<MessageInformation> ReadMessages(MessageStatusType messageType, string gatewayId)
        {
            IGateway gateway;
            if (Find(gatewayId, out gateway))
            {
                return ReadMessages(messageType, gateway);
            }
            return new List<MessageInformation>(0);
        }


        /// <summary>
        /// Read incoming messages from a particular gateway
        /// </summary>
        /// <param name="messageType">Message type. See <see cref="MessageStatusType"/></param>
        /// <param name="gateway">Gateway</param>
        /// <returns>The number of messages read</returns>
        public List<MessageInformation> ReadMessages(MessageStatusType messageType, IGateway gateway)
        {
            IMobileGateway mobileGateway = (IMobileGateway)gateway;
            if (mobileGateway.Connected)
            {
                if (
                    ((mobileGateway.Attributes & GatewayAttribute.ReceiveByPolling) == GatewayAttribute.ReceiveByPolling) ||
                     ((mobileGateway.Attributes & GatewayAttribute.ReceiveByTrigger) == GatewayAttribute.ReceiveByTrigger) 
                ) 
                {
                    return mobileGateway.GetMessages(messageType);
                }
            }
            return new List<MessageInformation>(0);
        }


        #endregion ================================================================================

        #region ============= Private Methods =====================================================

        /// <summary>
        /// Route the message
        /// </summary>
        /// <param name="message">Message to be sent</param>
        /// <returns>Gateway used for senting</returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        private IGateway RouteMessage(IMessage message)
        {
            //lock (this)
            //{
                return Router.Route(message);
            //}          
        }



        /// <summary>
        /// Monitor all the gateways, and restart them if they are stopped
        /// </summary>
        private void WatchDog()
        {
            while (true)
            {
                IGateway errorGateway = new DefaultMobileGateway();
                try
                {
                    Thread.Sleep(WatchDogInterval);
                    Logger.LogThis("Checking gateways. Number of gateways = " + this.Gateways.Count(), LogLevel.Info);                   
                    foreach (IGateway gateway in this.Gateways)
                    {
                        if (gateway != null)
                        {
                            if (gateway.Status == GatewayStatus.Restart)
                            {
                                errorGateway = gateway;
                                Logger.LogThis("Gateway " + gateway.Id + " is not active", LogLevel.Info);

                                // Try to restart it
                                IMobileGateway mobileGateway = (IMobileGateway)gateway;
                                //mobileGateway.Disconnect();
                                //mobileGateway.Status = GatewayStatus.Restart;

                                /*
                                if (!mobileGateway.Disconnect())
                                {
                                    Logger.LogThis("Error disconnect gateway " + mobileGateway.Id, LogLevel.Info);
                                    Logger.LogThis("Error:  " + mobileGateway.LastError.Message, LogLevel.Info);
                                    continue;
                                }
                                */
                                // Commented 27 Feb 2010
                                // mobileGateway.Connect();

                                // Retry to restart it again

                                 // Commented 27 Feb 2010
                                /*
                                if (mobileGateway.Status != GatewayStatus.Started)
                                {
                                    mobileGateway.Status = GatewayStatus.Restart;
                                    Logger.LogThis("Failed to restart gateway " + gateway.Id, LogLevel.Info);
                                    Logger.LogThis("Error:  " + mobileGateway.LastError.Message, LogLevel.Info);
                                }
                                else
                                {
                                    Logger.LogThis("Successfully restarted gateway " + gateway.Id, LogLevel.Info);
                                }
                                */
                            }
                        }
                    }
                }
                catch (ObjectDisposedException odEx)
                {
                    Logger.LogThis("Starting gateway " + errorGateway.Id, LogLevel.Error);
                    Logger.LogThis("Message:  "  + odEx.Message, LogLevel.Error);
                    break;
                }
                catch (ThreadInterruptedException tiEx)
                {
                    Logger.LogThis("Starting gateway " + errorGateway.Id, LogLevel.Error);
                    Logger.LogThis("Message:  " + tiEx.Message, LogLevel.Error);
                    break;
                }
                catch (Exception e)
                {
                    Logger.LogThis("Starting gateway " + errorGateway.Id, LogLevel.Error);
                    Logger.LogThis("Message:  " + e.Message, LogLevel.Error);
                    break;
                }
            }            
        }

        #endregion ================================================================================
    }
}
