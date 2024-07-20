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
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;

#if (!NO_WINFORMS)
using System.Windows.Forms;
#endif


using MessagingToolkit.Core.Base;
using MessagingToolkit.Core.Helper;
using MessagingToolkit.Core.Log;
using MessagingToolkit.Core.Mobile.Http.Feature;
using MessagingToolkit.Core.Properties;
using MessagingToolkit.Core.Service;
using MessagingToolkit.Core.Mobile.Event;
using MessagingToolkit.Core.Mobile.Http.Event;

namespace MessagingToolkit.Core.Mobile.Http
{
    /// <summary>
    /// Mobile HTTP gateway.
    /// </summary>
     [System.Reflection.ObfuscationAttribute(Feature = "renaming", ApplyToMembers = true)]
    internal class HttpGateway : BaseGateway<HttpGateway>, IHttpGateway, IDisposable
    {
        #region =========== Protected Variables ==================================================

        private const string NetworkInfoUrlPath = "services/api/status/network";

        private const string BatteryInfoUrlPath = "services/api/status/battery";

        private const string MessagesUrlPath = "services/api/messaging";

        private const string MessageStatusUrlPath = "services/api/messaging/status";

        /// <summary>
        /// Maximum sent message for unlicensed copy.
        /// </summary>
        private const int UnlicensedMaximumMessage = 100;

        #endregion ===============================================================================

        #region =========== Protected Variables ==================================================

        /// <summary>
        /// License.
        /// </summary>
        protected License license;

        /// <summary>
        /// HTTP gateway configuration.
        /// </summary>
        protected HttpGatewayConfiguration config;

        #endregion ===============================================================================

        #region =========== Private Variables =====================================================

        /// <summary>
        /// Execution engine for the gateway.
        /// </summary>
        private HttpDelegateEngine delegateEngine;

        /// <summary>
        /// Track whether Dispose has been called
        /// </summary> 
        private bool disposed = false;

        /// <summary>
        /// Statistics 
        /// </summary>
        private Statistics statistics;

        /// <summary>
        /// Command sync lock
        /// </summary>
        private object commandSyncLock;

        /// <summary>
        /// Message queue
        /// </summary>
        private PriorityQueue<IMessage, MessageQueuePriority> messageQueue;

        /// <summary>
        /// Delayed message queue
        /// </summary>
        private DelayedMessageQueue delayedMessageQueue;


        /// <summary>
        /// Outbound message queue
        /// </summary>
        private OutboundMessageQueue outboundMessageQueue;

        /// <summary>
        /// Flag to indicate if the library will always check the status of outbound message. Should set to true.
        /// </summary>
        private bool validateOutboundMessageStatus;

        /// <summary>
        /// Thread to process the message queue
        /// </summary>
        private Thread outboundMessageQueueProcessor;

        /// <summary>
        /// Thread to process the message queue
        /// </summary>
        private Thread messageQueueProcessor;

        /// <summary>
        /// Thread to process delayed message
        /// </summary>
        private Thread delayedMessageProcessor;

        /// <summary>
        /// Thread to poll for new unread messages
        /// </summary>
        private Thread messagePoller;

        /// <summary>
        /// Outbound queue lock
        /// </summary>
        private object outboundQueueLock;


        /// <summary>
        /// Synchronous message sending
        /// </summary>
        private object messageQueueLock;


        /// <summary>
        /// Flag to indicate if message queue is enabled
        /// </summary>
        private bool isMessageQueueEnabled;

        /// <summary>
        /// Flag to indicate if the gateway is still connected
        /// </summary>
        private bool isGatewayConnected;


        /// <summary>
        /// Flag to poll for new messages
        /// </summary>
        private bool pollNewMessages;


        #endregion ===============================================================================


        #region ================== Event =========================================================

        /// <summary>
        /// Message sending event
        /// </summary>
        public event MessageEventHandler MessageSending;

        /// <summary>
        /// Message sending failed event
        /// </summary>
        public event MessageErrorEventHandler MessageSendingFailed;

        /// <summary>
        /// Message sent event
        /// </summary>
        public event MessageEventHandler MessageSent;

        /// <summary>
        /// Message delivered event
        /// </summary>
        public event MessageEventHandler MessageDelivered;

        /// <summary>
        /// Message received event
        /// </summary>
        public event NewMessageReceivedEventHandler MessageReceived;

        /// <summary>
        /// Gateway disconnected
        /// </summary>
        public event DisconnectedEventHandler GatewayDisconnected;

        #endregion ===============================================================================


        #region ========================================= constructors ===========================

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpGateway"/> class.
        /// </summary>
        /// <param name="config">The configuration.</param>
        public HttpGateway(HttpGatewayConfiguration config)
            : base()
        {
            this.config = config;
            PerformSetup();
        }


        /// <summary>
        /// Performs the setup.
        /// </summary>
        protected void PerformSetup()
        {
            // Initialize the logger
            Logger.UseSensibleDefaults(config.LogFile, config.LogLocation, config.LogLevel, config.LogNameFormat);
            Logger.LogPrefix = LogPrefix.Dt;

            // Set the logging level from the configuration
            Logger.LogLevel = config.LogLevel;
            Logger.LogNameFormat = config.LogNameFormat;
            Logger.LogQuotaFormat = config.LogQuotaFormat;
            Logger.LogSizeMax = config.LogSizeMax;


            // Initialize the license
            license = new License(config);

            // Create statistics class
            statistics = new Statistics();

            // Default to null
            outboundMessageQueueProcessor = null;
            messageQueueProcessor = null;
            messagePoller = null;

            // Create the message queue
            outboundMessageQueue = new OutboundMessageQueue();

            // Create statistics class
            statistics = new Statistics();

            // Gateway supports all  features by default
            Attributes = GatewayAttribute.DeliveryReport | GatewayAttribute.LongMessage | GatewayAttribute.ReceiveByPolling | GatewayAttribute.Send;

            // Default to false;
            pollNewMessages = false;

            // Default to false
            isGatewayConnected = false;

            // Set to stopped
            Status = GatewayStatus.Stopped;

            // Message queue is enabled by default
            isMessageQueueEnabled = true;

            // Set the gateway id
            this.Id = config.GatewayId;

            // Initialize the locks
            commandSyncLock = new object();
            outboundQueueLock = new object();

            messageQueueLock = new object();

            // Create the message queue
            messageQueue = new PriorityQueue<IMessage, MessageQueuePriority>(3, PersistedMessageType.Http);
            messageQueue.PersistFolder = config.PersistenceFolder;

            // Create the delayed message queue
            delayedMessageQueue = new DelayedMessageQueue(PersistedMessageType.Http);
            delayedMessageQueue.PersistFolder = config.PersistenceFolder;

            // Create the execution engine
            delegateEngine = HttpDelegateEngine.NewInstance();

            // Default to false
            this.PersistenceQueue = false;
        }

        #endregion ========================================================================================



        #region =========== Public Properties =============================================================

        /// <summary>
        /// Indicate if all outbound messages should be validated on their statuses.
        /// Should probably always set to true if you want to get the message status events raised.
        /// </summary>
        /// <value>
        /// <c>true</c> if need to validate outbound messages; otherwise, <c>false</c>.
        /// </value>
        public virtual bool ValidateOutboundMessage
        {
            get
            {
                return validateOutboundMessageStatus;
            }
            set
            {
                validateOutboundMessageStatus = value;

                try
                {
                    if (validateOutboundMessageStatus)
                    {
                        if (outboundMessageQueueProcessor == null || !outboundMessageQueueProcessor.IsAlive)
                        {
                            // Start the polling thread
                            outboundMessageQueueProcessor = new Thread(new ThreadStart(this.OutboundMessageChecker));
                            outboundMessageQueueProcessor.IsBackground = true;
                            outboundMessageQueueProcessor.Start();
                        }
                    }
                    else
                    {
                        // Stop the message poller
                        try
                        {
                            if (outboundMessageQueueProcessor != null)
                            {
                                outboundMessageQueueProcessor.Abort();
                                outboundMessageQueueProcessor = null;
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


        /// <summary>
        /// Polling for new unread messages and raise message received event.
        /// The polling interval can be specified using <see cref="MobileGatewayConfiguration"/>.
        /// Default to false
        /// </summary>
        /// <value></value>
        public virtual bool PollNewMessages
        {
            get
            {
                return pollNewMessages;
            }
            set
            {
                pollNewMessages = value;

                try
                {
                    if (pollNewMessages)
                    {
                        if (messagePoller == null || !messagePoller.IsAlive)
                        {
                            // Start the polling thread
                            messagePoller = new Thread(new ThreadStart(this.MessagePolling));
                            messagePoller.IsBackground = true;
                            messagePoller.Start();
                        }
                    }
                    else
                    {
                        // Stop the message poller
                        try
                        {
                            if (messagePoller != null)
                            {
                                messagePoller.Abort();
                                messagePoller = null;
                            }
                        }
                        catch (Exception) { }
                    }
                }
                catch (Exception ex)
                {
                    this.exception = ex;
                }
            }
        }

        /// <summary>
        /// Gateway attributes. See <see cref="GatewayAttribute"/>
        /// </summary>
        /// <value>Gateway attributes</value>
        public virtual GatewayAttribute Attributes
        {
            get;
            set;
        }


        /// <summary>
        /// Return the license associated with this software
        /// </summary>
        /// <value>License</value>
        public virtual License License
        {
            get
            {
                return this.license;
            }
        }

        /// <summary>
        /// Return the gateway configuration
        /// </summary>
        /// <value>Configuration object</value>
        public virtual HttpGatewayConfiguration Configuration
        {
            get
            {
                return config;
            }
        }

        /// <summary>
        /// Statistics property
        /// </summary>
        /// <value>Statistics</value>
        public virtual Statistics Statistics
        {
            get
            {
                return statistics;
            }
        }


        /// <summary>
        /// If set to true, messages in queue or delayed queue are persisted
        /// </summary>
        /// <value><c>true</c> if persistence is required; otherwise, <c>false</c>.</value>
        public virtual bool PersistenceQueue
        {
            get
            {
                return this.Configuration.PersistenceQueue;
            }
            set
            {
                this.Configuration.PersistenceQueue = value;
                this.messageQueue.Persist = value;
                this.delayedMessageQueue.Persist = value;

                if (this.messageQueue.Persist)
                {
                    List<IMessage> messages = this.messageQueue.Load(PersistedMessageType.Http);
                    foreach (IMessage message in messages)
                    {
                        SendToQueue(message);
                    }
                }

            }
        }


        /// <summary>
        /// Gets or sets the base persistence folder
        /// </summary>
        /// <value>Persistence folder</value>
        public virtual string PersistenceFolder
        {
            get
            {
                return this.Configuration.PersistenceFolder;
            }
            set
            {
                this.Configuration.PersistenceFolder = value;
                this.messageQueue.PersistFolder = value;
                this.delayedMessageQueue.PersistFolder = value;
            }
        }


        /// <summary>
        /// Gets the device network information.
        /// </summary>
        /// <value>
        /// The device network information.
        /// </value>
        public DeviceNetworkInformation DeviceNetworkInformation
        {
            get
            {
                // Retrieve the device network information from the gateway
                GetNetworkInfoFeature feature = GetFeature<GetNetworkInfoFeature>(GetNetworkInfoFeature.NewInstance());
                try
                {
                    feature.Uri = NetworkInfoUrlPath;
                    IContext context = Execute(feature);
                    DeviceNetworkInformation networkInfo = (DeviceNetworkInformation)context.GetResult();
                    return networkInfo;
                }
                catch (Exception ex)
                {
                    this.exception = ex;
                    return new DeviceNetworkInformation();
                }
            }
        }

        /// <summary>
        /// Gets the device battery information.
        /// </summary>
        /// <value>
        /// The device battery information.
        /// </value>
        public DeviceBatteryInformation DeviceBatteryInformation
        {
            get
            {
                // Retrieve the device battery information from the gateway
                GetBatteryInfoFeature feature = GetFeature<GetBatteryInfoFeature>(GetBatteryInfoFeature.NewInstance());
                try
                {
                    feature.Uri = BatteryInfoUrlPath;
                    IContext context = Execute(feature);
                    DeviceBatteryInformation batteryInfo = (DeviceBatteryInformation)context.GetResult();
                    return batteryInfo;
                }
                catch (Exception ex)
                {
                    this.exception = ex;
                    return new DeviceBatteryInformation();
                }
            }
        }

        /// <summary>
        /// Gets or sets a value to define whether the message queue is enabled (sends out messages)
        /// </summary>
        /// <value>if true then the unsent messages in the queue will be sent immediately based on priority of the messages</value>
        public virtual bool IsMessageQueueEnabled
        {
            get
            {
                return isMessageQueueEnabled;
            }
            set
            {
                this.isMessageQueueEnabled = value;
                if (isMessageQueueEnabled)
                {
                    lock (messageQueueLock)
                    {
                        Monitor.PulseAll(messageQueueLock);
                    }
                }
            }
        }


        #endregion ===========================================================================================


        #region =========== Public Method  ===================================================================


        /// <summary>
        /// Implement IDisposable.
        /// Do not make this method virtual.
        ///  A derived class should not be able to override this method.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);

            // This object will be cleaned up by the Dispose method.
            // Therefore, you should call GC.SupressFinalize to
            // take this object off the finalization queue
            // and prevent finalization code for this object
            // from executing a second time.

            GC.SuppressFinalize(this);
        }


        /// <summary>
        /// Disconnect from the gateway.
        /// </summary>
        /// <returns>true if success, false otherwise.</returns>
        public bool Disconnect()
        {
            Status = GatewayStatus.Stopping;

            // Stop the message queue
            StopMessageQueue();

            // Stop delayed message queue
            StopDelayedMessageQueue();

            // Stop message poller
            StopMessagePoller();

            // Stop outbound message checker
            StopOutboundMessageChecker();

            Status = GatewayStatus.Stopped;

            // Raise the event
            OnGatewayDisconnected();

            // Reinitialize all the variables
            PerformSetup();

            return true;
        }


        /// <summary>
        /// Connect to the gateway
        /// </summary>
        /// <returns>true if success, false otherwise</returns>
        public virtual bool Connect()
        {
            try
            {
                Status = GatewayStatus.Starting;

                // Set to true
                isGatewayConnected = true;

                // Start the message queue processor
                StartMessageQueue();

                // Start the delayed message queue
                StartDelayedMessageQueue();

                // Set to true by default
                ValidateOutboundMessage = true;

                // Start the delayed message queue
                // StartDelayedMessageQueue();

                Status = GatewayStatus.Started;

                // Raise the event
                OnGatewayConnected();
            }
            catch (Exception ex)
            {
                // Assign to the internal variable
                this.exception = ex;

                // Set to false
                isGatewayConnected = false;

                Status = GatewayStatus.Failed;

                try
                {
                    // Call the disconnect method just in case the port is already open
                    Disconnect();
                }
                catch (Exception) { }

                return false;
            }

            return true;
        }

        /// <summary>
        /// Gets the messages using the specified query. Return an empty list if there is any errors.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns>
        /// List of messages.
        /// </returns>
        public List<DeviceMessage> GetMessages(GetMessageQuery query)
        {
            GetMessagesFeature feature = GetFeature<GetMessagesFeature>(GetMessagesFeature.NewInstance());
            try
            {
                feature.Uri = MessagesUrlPath;
                feature.Query = query;
                IContext context = Execute(feature);
                List<DeviceMessage> messages = (List<DeviceMessage>)context.GetResult();
                return messages;
            }
            catch (Exception ex)
            {
                this.exception = ex;
                return new List<DeviceMessage>();
            }
        }

        /// <summary>
        /// Gets all the messages. Return an empty list if there is any errors.
        /// </summary>
        /// <returns>
        /// List of messages.
        /// </returns>
        public List<DeviceMessage> GetMessages()
        {
            // Retrieve the device battery information from the gateway
            GetMessagesFeature feature = GetFeature<GetMessagesFeature>(GetMessagesFeature.NewInstance());
            try
            {
                feature.Uri = MessagesUrlPath;
                IContext context = Execute(feature);
                List<DeviceMessage> messages = (List<DeviceMessage>)context.GetResult();
                return messages;
            }
            catch (Exception ex)
            {
                this.exception = ex;
                return new List<DeviceMessage>();
            }
        }


        /// <summary>
        /// <para>
        /// Send message to the device through HTTP POST method. If the message is going to be sent immediately (not scheduled to be sent later)
        /// the MessageSending event will NOT be triggered.
        /// </para>
        /// <para>
        /// The MessageSent, MessageDelivered and MessageSendingFailed events will be triggered.
        /// </para>
        /// </summary>
        /// <param name="message">Message to be sent.</param>
        /// <returns>
        /// true if successfully sent. If false then you need to check the last exception thrown
        /// </returns>
        public virtual bool Send(IMessage message)
        {
            try
            {
                if (message == null) return false;

                // Set the message identifier
                if (string.IsNullOrEmpty(message.Identifier))
                {
                    message.Identifier = GatewayHelper.GenerateUniqueIdentifier();
                }

                if (message.ScheduledDeliveryDate != null && message.ScheduledDeliveryDate > DateTime.Now)
                {
                    Logger.LogThis(string.Format("Send to delayed message queue. Scheduled date is [{0}]", message.ScheduledDeliveryDate.ToString()), LogLevel.Verbose, this.Id);
                    delayedMessageQueue.QueueMessage(message);
                    return true;
                }

                for (int i = 1; i <= config.SendRetries; i++)
                {
                    try
                    {
                        if (message is PostMessage)
                        {
                            PostMessage postMessage = (PostMessage)message;
                            SendMessage(postMessage);
                        }
                        break;
                    }
                    catch (Exception e)
                    {
                        Logger.LogThis("Sending message: " + e.Message, LogLevel.Info, this.Id);
                        Logger.LogThis("Trace: " + e.StackTrace, LogLevel.Info, this.Id);
                        if (i == config.SendRetries)
                        {
                            throw e;

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Assign to the internal variable
                this.exception = ex;
                return false;
            }
            return true;
        }


        /// <summary>
        /// Gets the message status.
        /// </summary>
        /// <param name="id">The message id.</param>
        /// <returns>
        /// Message status
        /// </returns>
        public virtual MessageStatusInformation GetMessageStatus(string id)
        {
            GetMessageStatusFeature feature = GetFeature<GetMessageStatusFeature>(GetMessageStatusFeature.NewInstance());
            try
            {
                feature.Uri = MessagesUrlPath;
                feature.Id = id;
                IContext context = Execute(feature);
                MessageStatusInformation statusInfo = (MessageStatusInformation)context.GetResult();
                return statusInfo;
            }
            catch (Exception ex)
            {
                this.exception = ex;
                return new MessageStatusInformation();
            }
        }



        /// <summary>
        /// <para>
        /// Send message through message queue to the device through HTTP POST method. The message queue will be processed in the background
        /// and message events will be triggered.
        /// </para>
        /// <para>
        /// The event MessagingSending,  MessageSent, MessageDelivered and MessageSendingFailed will be triggered depending on the sending status.
        /// </para>
        /// </summary>
        /// <param name="message">Message to be sent.</param>
        /// <returns>
        /// true if successfully sent. If false then you need to check the last exception thrown
        /// </returns>
        public virtual bool SendToQueue(IMessage message)
        {
            try
            {
                if (message == null) return false;

                // Set the message identifier
                if (string.IsNullOrEmpty(message.Identifier))
                {
                    message.Identifier = GatewayHelper.GenerateUniqueIdentifier();
                }

                if (message.ScheduledDeliveryDate != null && message.ScheduledDeliveryDate > DateTime.Now)
                {
                    Logger.LogThis(string.Format("Send to delayed message queue. ID is [{0}]. Scheduled date is [{1}]", message.Identifier, message.ScheduledDeliveryDate.ToString()), LogLevel.Verbose, this.Id);
                    delayedMessageQueue.QueueMessage(message);
                    return true;
                }

                // Put the message into the queue                
                messageQueue.Enqueue(message, message.QueuePriority);
                lock (messageQueueLock)
                {
                    Monitor.PulseAll(messageQueueLock);
                }
            }
            catch (Exception ex)
            {
                // Assign to the internal variable
                this.exception = ex;
                return false;
            }
            return true;
        }

        /// <summary>
        /// Clear all the messages queued
        /// </summary>
        /// <returns>truf it is successful</returns>
        public virtual bool ClearQueue()
        {
            lock (messageQueueLock)
            {
                messageQueue.Clear();
            }
            return true;
        }

        /// <summary>
        /// Get all messages in the queue
        /// </summary>
        /// <returns>List of messages in queue</returns>
        public virtual int GetQueueCount()
        {
            return messageQueue.Count();
        }


        /// <summary>
        /// Deletes the message.
        /// </summary>
        /// <param name="ids">List of message ids.</param>
        /// <returns>
        /// Number of deleted messages. -1 if there is error.
        /// </returns>
        public virtual int DeleteMessage(List<String> ids)
        {
            DeleteMessageByIdFeature feature = GetFeature<DeleteMessageByIdFeature>(DeleteMessageByIdFeature.NewInstance());
            try
            {
                feature.Uri = MessagesUrlPath;
                feature.Ids = ids;
                IContext context = Execute(feature);
                DeleteMessageResponse response = (DeleteMessageResponse)context.GetResult();
                return response.Count;
            }
            catch (Exception ex)
            {
                this.exception = ex;
                return -1;
            }
            
        }

        /// <summary>
        /// Deletes the message.
        /// </summary>
        /// <param name="threadId">The message thread id.</param>
        /// <returns>
        /// Number of deleted messages.
        /// </returns>
        public int DeleteMessage(string threadId)
        {
            DeleteMessageByThreadIdFeature feature = GetFeature<DeleteMessageByThreadIdFeature>(DeleteMessageByThreadIdFeature.NewInstance());
            try
            {
                feature.Uri = MessagesUrlPath;
                feature.ThreadId = threadId;
                IContext context = Execute(feature);
                DeleteMessageResponse response = (DeleteMessageResponse)context.GetResult();
                return response.Count;
            }
            catch (Exception ex)
            {
                this.exception = ex;
                return -1;
            }
        }

        #endregion ===========================================================================================

        #region =========== Protected Methods ===========================================================

        /// <summary>
        /// Set up the feature to be used
        /// </summary>
        /// <param name="feature">Feature</param>
        /// <returns>The feature to be run</returns>
        private F GetFeature<F>(F feature) where F : IHttpFeature
        {
            feature.IPAddress = this.Configuration.IPAddress;
            feature.Port = this.Configuration.Port;
            feature.UserName = this.Configuration.UserName;
            feature.Password = this.Configuration.Password;
            return feature;
        }

        /// <summary>
        /// Execute the feature
        /// </summary>
        /// <param name="feature">Feature</param>
        /// <returns>Execution result</returns>
        private IContext Execute(IHttpFeature feature)
        {
            lock (CommandSyncLock)
            {
                try
                {
                    IContext context = delegateEngine.Run(feature);
                    LastExecution = DateTime.Now;
                    return context;
                }
                catch (Exception ex)
                {
                    // If in debug mode, display the dialog
                    if (config.DebugMode)
                    {
#if (!NO_WINFORMS)
                        MessageBox.Show(string.Format("Message: {0}\nStack trace: {1}", ex.Message, ex.StackTrace), Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
#endif
                    }
                    Logger.LogThis(string.Format("Error executing command: [{0}]", ex.Message), LogLevel.Error, this.Id);
                    throw ex;
                }
            }
        }

        /// <summary>
        /// Poll for new unread messages.
        /// </summary>
        private void MessagePolling()
        {
            while (true)
            {
                try
                {
                    GetMessagesFeature feature = GetFeature<GetMessagesFeature>(GetMessagesFeature.NewInstance());
                    GetMessageQuery query = new GetMessageQuery();
                    query.Status = (int)MessageReadStatus.Unread;
                    feature.Uri = MessagesUrlPath;
                    feature.Query = query;
                    IContext context = Execute(feature);
                    List<DeviceMessage> unreadMessages = (List<DeviceMessage>)context.GetResult();

                    List<string> ids = new List<string>(1);
                    foreach (DeviceMessage message in unreadMessages)
                    {
                        // Read the receive event
                        OnMessageReceived(message);

                        // Increase by 1
                        statistics.IncomingSms++;

                        ids.Add(message.Id);
                    }

                    if (config.DeleteReceivedMessage)
                    {
                        // Delete the messages
                        DeleteMessageByIdFeature deleteFeature = GetFeature<DeleteMessageByIdFeature>(DeleteMessageByIdFeature.NewInstance());
                        deleteFeature.Uri = MessagesUrlPath;
                        deleteFeature.Ids = ids;
                        context = Execute(deleteFeature);
                        DeleteMessageResponse response = (DeleteMessageResponse)context.GetResult();
                        if (response != null && response.Count > 0)
                        {
                            Logger.LogThis(string.Format("Deleted received messages from device. Total message count = {0}", response.Count), LogLevel.Info, this.Id);
                        }
                    }
                    else
                    {
                        // If no deletion, check if need to mark as read
                        if (config.MarkReadMessage)
                        {
                            // Mark all the messages as read
                            UpdateMessageToReadFeature updateFeature = GetFeature<UpdateMessageToReadFeature>(UpdateMessageToReadFeature.NewInstance());
                            updateFeature.Uri = MessagesUrlPath;
                            updateFeature.Ids = ids;
                            context = Execute(updateFeature);
                            PutMessageResponse response = (PutMessageResponse)context.GetResult();
                            if (response != null && response.Count > 0)
                            {
                                Logger.LogThis(string.Format("Updated unread mesages to read status. Total message count = {0}", response.Count), LogLevel.Info, this.Id);
                            }
                        }
                    }

                    // Sleep and wait for next run
                    Thread.Sleep(config.MessagePollingInterval);
                }
                catch (ThreadInterruptedException)
                {
                    break;
                }
                catch (ThreadAbortException)
                {
                    break;
                }
                catch (Exception e)
                {
                    Logger.LogThis("Error polling new messages: " + e.Message, LogLevel.Error, this.Id);

                    // Sleep and wait for next run
                    Thread.Sleep(config.MessagePollingInterval);
                }
            }
        }


        #endregion ===========================================================================================

        #region =========== Private Method  ===================================================================


        /// <summary>
        /// Dispose(bool disposing) executes in two distinct scenarios.
        /// If disposing equals true, the method has been called directly
        /// or indirectly by a user's code. Managed and unmanaged resources
        /// can be disposed.
        /// If disposing equals false, the method has been called by the
        /// runtime from inside the finalizer and you should not reference
        /// other objects. Only unmanaged resources can be disposed.
        /// </summary>
        /// <param name="disposing"></param>
        private void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!this.disposed)
            {
                // If disposing equals true, dispose all managed
                // and unmanaged resources.
                if (disposing)
                {
                    // Dispose managed resources.
                }

                // Note disposing has been done.
                disposed = true;

            }
        }



        /// <summary>
        /// Send out the message
        /// </summary>
        /// <param name="message">Message instance</param>
        /// <exception cref="MessagingToolkit.Core.GatewayException"></exception>
        private void SendMessage(PostMessage message)
        {
            try
            {
                Logger.LogThis("Sending message to " + message.To, LogLevel.Verbose, this.Id);
                Logger.LogThis("Content: " + message.Message, LogLevel.Verbose, this.Id);

                if (string.IsNullOrEmpty(message.GatewayId))
                {
                    message.GatewayId = this.Id;
                }

                if (!license.Valid)
                {
                    if (this.statistics.OutgoingSms >= UnlicensedMaximumMessage)
                        throw new GatewayException(Resources.LicenseException);

                    if (!message.Message.StartsWith(Resources.UnlicensedMessagePrefix))
                    {
                        // Prefix the message
                        message.Message = Resources.UnlicensedMessagePrefix + message.Message;
                    }
                }

                PostMessageFeature feature = GetFeature<PostMessageFeature>(PostMessageFeature.NewInstance());
                feature.Uri = MessagesUrlPath;
                feature.Message = message;
                IContext context = Execute(feature);
                PostMessageResponse response = (PostMessageResponse)context.GetResult();

                // Increase the statistics counter
                if (response.IsSuccessful && response.Message != null && !string.IsNullOrEmpty(response.Message.Id))
                {
                    message.MessageIdFromDevice = response.Message.Id;
                    statistics.OutgoingSms++;

                    // Add it to the outbound message queue if necessary
                    if (this.ValidateOutboundMessage)
                    {
                        this.outboundMessageQueue.Enqueue(message);
                        lock (outboundQueueLock)
                        {
                            Monitor.PulseAll(outboundQueueLock);
                        }
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(response.Description))
                        throw new GatewayException(response.Description);
                    else
                        throw new GatewayException(Resources.UnknownDeviceMessageSendingException);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        /// <summary>
        /// Thread to check the statuses of the messages which have been HTTP posted to the device.
        /// </summary>
        private void OutboundMessageChecker()
        {
            PostMessage message = null;
            while (true)
            {
                try
                {
                    lock (outboundQueueLock)
                    {
                        if (outboundMessageQueue.Count == 0 || !this.ValidateOutboundMessage)
                            Monitor.Wait(outboundQueueLock);
                    }

                    while (outboundMessageQueue.Count > 0 && this.ValidateOutboundMessage)
                    {
                        message = outboundMessageQueue.Peek();
                        try
                        {
                            // Check the message status
                            GetMessageStatusFeature feature = GetFeature<GetMessageStatusFeature>(GetMessageStatusFeature.NewInstance());
                            feature.Uri = MessageStatusUrlPath;
                            feature.Id = message.MessageIdFromDevice;
                            IContext context = Execute(feature);
                            MessageStatusInformation statusInfo = (MessageStatusInformation)context.GetResult();

                            if (StringEnum.GetStringValue(MessageDeliveryStatus.Delivered).Equals(statusInfo.Status, StringComparison.InvariantCultureIgnoreCase))
                            {
                                OnMessageDelivered(message);
                            }
                            else if (StringEnum.GetStringValue(MessageDeliveryStatus.Sent).Equals(statusInfo.Status, StringComparison.InvariantCultureIgnoreCase))
                            {
                                OnMessageSent(message);
                            }
                            else if (StringEnum.GetStringValue(MessageDeliveryStatus.Failed).Equals(statusInfo.Status, StringComparison.InvariantCultureIgnoreCase))
                            {
                                OnMessageSendingFailed(message, new GatewayException(string.Format(Resources.MessageSendingException, statusInfo.ErrorDescription)));

                            }
                            outboundMessageQueue.Dequeue();

                            // If status <> Delivered or Failed, requeue for checking again
                            if (!StringEnum.GetStringValue(MessageDeliveryStatus.Delivered).Equals(statusInfo.Status, StringComparison.InvariantCultureIgnoreCase) &&
                                !StringEnum.GetStringValue(MessageDeliveryStatus.Failed).Equals(statusInfo.Status, StringComparison.InvariantCultureIgnoreCase)
                                )
                            {
                                outboundMessageQueue.Enqueue(message);
                                if (outboundMessageQueue.Count == 1)
                                {
                                    // If this is the only one in the queue, sleep for longer time before retrying
                                    Thread.Sleep(5000);
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            Logger.LogThis("Error checking message status: " + e.Message, LogLevel.Info, this.Id);
                            Logger.LogThis("Trace: " + e.StackTrace, LogLevel.Info, this.Id);
                            if (e.GetBaseException() != null)
                            {
                                Logger.LogThis("Base exception: " + e.GetBaseException().StackTrace, LogLevel.Info, this.Id);
                            }
                            if (e.InnerException != null)
                            {
                                Logger.LogThis("Inner exception: " + e.InnerException.StackTrace, LogLevel.Info, this.Id);
                            }

                            // Sleep for longer time upon exception
                            Thread.Sleep(3000);
                        }
                    }
                }
                catch (ThreadInterruptedException)
                {
                    break;
                }
                catch (ThreadAbortException)
                {
                    break;
                }
                catch (Exception e)
                {
                    // Log it  
                    Logger.LogThis("Outbound message checker: " + e.Message, LogLevel.Warn, this.Id);
                    Logger.LogThis("Stack trace: " + e.StackTrace, LogLevel.Error, this.Id);
                    break;
                }
            }
        }

        /// <summary>
        /// Process message from message queue
        /// </summary>
        private void ProcessMessageQueue()
        {
            PostMessage message = null;
            while (true)
            {
                try
                {
                    lock (messageQueueLock)
                    {
                        if (messageQueue.Count == 0 || !IsMessageQueueEnabled)
                            Monitor.Wait(messageQueueLock);
                    }

                    while (messageQueue.Count > 0 && IsMessageQueueEnabled)
                    {
                        PriorityQueueItem<IMessage, MessageQueuePriority> item = messageQueue.Peek();
                        message = item.Value as PostMessage;

                        // Raise message sending event
                        OnMessageSending(message);

                        // Try sending the message
                        for (int i = 1; i <= config.SendRetries; i++)
                        {
                            try
                            {
                                SendMessage(message);
                                messageQueue.Dequeue(item);
                                break;
                            }
                            catch (Exception e)
                            {
                                Logger.LogThis("Failed to send message: " + e.Message, LogLevel.Info, this.Id);
                                Logger.LogThis("Trace: " + e.StackTrace, LogLevel.Info, this.Id);
                                if (e.GetBaseException() != null)
                                {
                                    Logger.LogThis("Base exception: " + e.GetBaseException().StackTrace, LogLevel.Info, this.Id);
                                }
                                if (e.InnerException != null)
                                {
                                    Logger.LogThis("Inner exception: " + e.InnerException.StackTrace, LogLevel.Info, this.Id);
                                }
                                if (i == config.SendRetries)
                                {
                                    // Error in sending SMS, raise the event and pass the exception
                                    OnMessageSendingFailed(message, e);

                                    messageQueue.Dequeue(item);

                                }
                                // Sleep for longer time upon exception
                                Thread.Sleep(1000);
                            }
                        }
                    }
                }
                catch (ThreadInterruptedException)
                {
                    break;
                }
                catch (ThreadAbortException)
                {
                    break;
                }
                catch (Exception e)
                {
                    Logger.LogThis("Processing message queue: " + e.Message, LogLevel.Warn, this.Id);
                    Logger.LogThis("Stack trace: " + e.StackTrace, LogLevel.Error, this.Id);
                    break;
                }
            }
        }


        /// <summary>
        /// Process message from delayed message queue
        /// </summary>
        private void ProcessDelayedMessageQueue()
        {
            IMessage message = null;
            while (true)
            {
                // Load persistence
                //LoadPersistenceQueue();
                try
                {
                    while (delayedMessageQueue.CountDue() > 0)
                    {
                        message = delayedMessageQueue.DueMessage;
                        Logger.LogThis(string.Format("Delayed message due for sending. Id is [{0}]", message.Identifier), LogLevel.Verbose, this.Id);
                        SendToQueue(message);
                    }
                    // Sleep 5 seconds before the next check
                    Thread.Sleep(5000);
                }
                catch (ThreadInterruptedException)
                {
                    break;
                }
                catch (ThreadAbortException)
                {
                    break;
                }
                catch (Exception e)
                {
                    // Log it  
                    Logger.LogThis("Processing delayed message queue: " + e.Message, LogLevel.Warn, this.Id);
                    Logger.LogThis("Stack trace: " + e.StackTrace, LogLevel.Error, this.Id);
                    Thread.Sleep(10000);
                }
            }
        }

        /// <summary>
        /// Starts the message queue.
        /// </summary>
        private void StartMessageQueue()
        {
            if (messageQueueProcessor != null && messageQueueProcessor.IsAlive)
            {
                // If the message queue is already started, return
                return;
            }

            messageQueueProcessor = new Thread(new ThreadStart(this.ProcessMessageQueue));
            messageQueueProcessor.IsBackground = true;
            messageQueueProcessor.Start();

        }

        /// <summary>
        /// Starts the delayed message queue.
        /// </summary>
        private void StartDelayedMessageQueue()
        {
            if (delayedMessageProcessor != null && delayedMessageProcessor.IsAlive)
            {
                // If the message queue is already started, return
                return;
            }

            delayedMessageProcessor = new Thread(new ThreadStart(this.ProcessDelayedMessageQueue));
            delayedMessageProcessor.IsBackground = true;
            delayedMessageProcessor.Start();
        }



        /// <summary>
        /// Stops the message queue.
        /// </summary>
        private void StopMessageQueue()
        {
            try
            {
                if (messageQueueProcessor != null)
                {
                    messageQueueProcessor.Abort();
                    messageQueueProcessor = null;
                }
            }
            catch (Exception) { }
        }

        /// <summary>
        /// Stops the delayed message queue.
        /// </summary>
        private void StopDelayedMessageQueue()
        {
            try
            {
                if (delayedMessageProcessor != null)
                {
                    delayedMessageProcessor.Abort();
                    delayedMessageProcessor = null;
                }
            }
            catch (Exception e) { }
        }



        /// <summary>
        /// Stops the message poller.
        /// </summary>
        private void StopMessagePoller()
        {
            try
            {
                if (messagePoller != null)
                {
                    messagePoller.Abort();
                    messagePoller = null;
                }
            }
            catch (Exception) { }
        }

        /// <summary>
        /// Stops the outbound message check
        /// </summary>
        private void StopOutboundMessageChecker()
        {
            try
            {
                if (outboundMessageQueueProcessor != null)
                {
                    outboundMessageQueueProcessor.Abort();
                    outboundMessageQueueProcessor = null;
                }
            }
            catch (Exception) { }
        }

        /// <summary>
        /// Message sending started
        /// </summary>
        /// <param name="message">Message</param>
        private void OnMessageSending(IMessage message)
        {
            if (this.MessageSending != null)
            {
                Logger.LogThis("Firing async MessageSending event.", LogLevel.Verbose, this.Id);
                MessageEventArgs e = new MessageEventArgs(message);
                this.MessageSending.BeginInvoke(this, e, new AsyncCallback(this.AsyncCallback), null);
            }
        }

        /// <summary>
        /// Message sending is successful
        /// </summary>
        /// <param name="message">Message</param>
        private void OnMessageSent(IMessage message)
        {
            if (this.MessageSent != null)
            {
                Logger.LogThis("Firing async MessageSent event.", LogLevel.Verbose, this.Id);
                MessageEventArgs e = new MessageEventArgs(message);
                this.MessageSent.BeginInvoke(this, e, new AsyncCallback(this.AsyncCallback), null);
            }
        }

        /// <summary>
        /// Message is delivered 
        /// </summary>
        /// <param name="message">Message</param>
        private void OnMessageDelivered(IMessage message)
        {
            if (this.MessageDelivered != null)
            {
                Logger.LogThis("Firing async MessageDelivered event.", LogLevel.Verbose, this.Id);
                MessageEventArgs e = new MessageEventArgs(message);
                this.MessageDelivered.BeginInvoke(this, e, new AsyncCallback(this.AsyncCallback), null);
            }
        }

        /// <summary>
        /// Message sending failed
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="ex">Exception</param>
        private void OnMessageSendingFailed(IMessage message, Exception ex)
        {
            if (this.MessageSendingFailed != null)
            {
                Logger.LogThis("Firing async MessageSendingFailed event.", LogLevel.Verbose, this.Id);
                MessageErrorEventArgs e = new MessageErrorEventArgs(message, ex);
                this.MessageSendingFailed.BeginInvoke(this, e, new AsyncCallback(this.AsyncCallback), null);
            }
        }

        /// <summary>
        /// Message is received.
        /// </summary>
        /// <param name="message">Message</param>
        private void OnMessageReceived(DeviceMessage message)
        {
            if (this.MessageReceived != null)
            {
                Logger.LogThis("Firing async MessageReceived event.", LogLevel.Verbose, this.Id);
                NewMessageReceivedEventArgs e = new NewMessageReceivedEventArgs(message);
                this.MessageReceived.BeginInvoke(this, e, new AsyncCallback(this.AsyncCallback), null);
            }
        }

        /// <summary>
        /// Called when gateway is connected.
        /// </summary>        
        private void OnGatewayConnected()
        {
            ConnectionEventArgs args = new ConnectionEventArgs(this.Id);
            this.config.OnConnected(args);
        }

        /// <summary>
        /// Called when gateway is disconnected.
        /// </summary>
        private void OnGatewayDisconnected()
        {
            if (this.GatewayDisconnected != null)
            {
                ConnectionEventArgs args = new ConnectionEventArgs(this.Id);
                this.GatewayDisconnected.BeginInvoke(this, args, new AsyncCallback(this.AsyncCallback), null);
            }
        }


        /// <summary>
        /// Asynchronous callback method
        /// </summary>
        /// <param name="param">Result</param>
        private void AsyncCallback(IAsyncResult param)
        {
            try
            {
                System.Runtime.Remoting.Messaging.AsyncResult result = (System.Runtime.Remoting.Messaging.AsyncResult)param;
                if (result.AsyncDelegate is NewMessageReceivedEventHandler)
                {
                    Logger.LogThis("Ending async NewMessageReceivedEventHandler call", LogLevel.Verbose, this.Id);
                    ((NewMessageReceivedEventHandler)result.AsyncDelegate).EndInvoke(result);
                }
                else if (result.AsyncDelegate is MessageEventHandler)
                {
                    Logger.LogThis("Ending async MessageEventHandler call", LogLevel.Verbose, this.Id);
                    ((MessageEventHandler)result.AsyncDelegate).EndInvoke(result);
                }
                else if (result.AsyncDelegate is MessageErrorEventHandler)
                {
                    Logger.LogThis("Ending async MessageErrorEventHandler call", LogLevel.Verbose, this.Id);
                    ((MessageErrorEventHandler)result.AsyncDelegate).EndInvoke(result);
                }
                else if (result.AsyncDelegate is ConnectedEventHandler)
                {
                    Logger.LogThis("Ending async ConnectedEventHandler call", LogLevel.Verbose, this.Id);
                    ((ConnectedEventHandler)result.AsyncDelegate).EndInvoke(result);
                }
                else if (result.AsyncDelegate is DisconnectedEventHandler)
                {
                    Logger.LogThis("Ending async DisconnectedEventHandler call", LogLevel.Verbose, this.Id);
                    ((DisconnectedEventHandler)result.AsyncDelegate).EndInvoke(result);
                }
                else if (result.AsyncDelegate is EventHandler)
                {
                    Logger.LogThis("Ending async EventHandler call", LogLevel.Verbose, this.Id);
                    ((EventHandler)result.AsyncDelegate).EndInvoke(result);
                }
                else
                {
                    Logger.LogThis("Warning: AsyncCallback got unknown delegate: " + result.AsyncDelegate.GetType().ToString(), LogLevel.Verbose, this.Id);
                }
            }
            catch (Exception ex)
            {
                Logger.LogThis("Error: AsyncCallback encountered error: " + ex.Message, LogLevel.Warn, this.Id);
            }
        }


        /*
        private void LoadPersistenceQueue()
        {
            // Commented Dec 22nd 2014
            //this.delayedMessageQueue.Persist = this.Configuration.PersistenceQueue;
            //this.delayedMessageQueue.PersistFolder = this.Configuration.PersistenceFolder;
            //this.messageQueue.Persist = this.Configuration.PersistenceQueue;
            //this.messageQueue.PersistFolder = this.Configuration.PersistenceFolder;

            if (this.messageQueue.Persist)
            {
                List<IMessage> messages = this.messageQueue.Load(PersistedMessageType.Http);
                foreach (IMessage message in messages)
                {
                    SendToQueue(message);
                }
            }
        }
        */

        #endregion ===========================================================================================


        #region =========== Internal Properties================================================================


        /// <summary>
        /// Object for command mode synching
        /// </summary>
        /// <value>Command object</value>
        internal object CommandSyncLock
        {
            get
            {
                return commandSyncLock;
            }
        }


        /// <summary>
        /// Last execution time
        /// </summary>
        /// <value>Time in milliseconds</value>
        internal DateTime LastExecution
        {
            get;
            set;
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
            str = String.Concat(str, "Id = ", Id, "\r\n");
            str = String.Concat(str, "Status = ", Status, "\r\n");
            str = String.Concat(str, "LastError = ", LastError, "\r\n");
            str = String.Concat(str, "LogLevel = ", LogLevel, "\r\n");
            str = String.Concat(str, "LogDestination = ", LogDestination, "\r\n");
            str = String.Concat(str, "LogFile = ", LogFile, "\r\n");
            str = String.Concat(str, "ValidateOutboundMessage = ", ValidateOutboundMessage, "\r\n");
            str = String.Concat(str, "PollNewMessages = ", PollNewMessages, "\r\n");
            str = String.Concat(str, "Attributes = ", Attributes, "\r\n");
            str = String.Concat(str, "License = ", License, "\r\n");
            str = String.Concat(str, "Configuration = ", Configuration, "\r\n");
            str = String.Concat(str, "Statistics = ", Statistics, "\r\n");
            str = String.Concat(str, "PersistenceQueue = ", PersistenceQueue, "\r\n");
            str = String.Concat(str, "PersistenceFolder = ", PersistenceFolder, "\r\n");
            str = String.Concat(str, "DeviceNetworkInformation = ", DeviceNetworkInformation, "\r\n");
            str = String.Concat(str, "DeviceBatteryInformation = ", DeviceBatteryInformation, "\r\n");
            str = String.Concat(str, "IsMessageQueueEnabled = ", IsMessageQueueEnabled, "\r\n");
            return str;
        }
    }
}
