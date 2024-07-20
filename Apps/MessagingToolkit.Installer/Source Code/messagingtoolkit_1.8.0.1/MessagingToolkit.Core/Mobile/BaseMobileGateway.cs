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
using System.IO.Ports;
using System.Threading;
using System.IO;
using System.Reflection;
using System.Timers;
using System.Collections;
using System.Text.RegularExpressions;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

using MessagingToolkit.Core.Base;
using MessagingToolkit.Core.Log;
using MessagingToolkit.Core.Mobile.Feature;
using MessagingToolkit.Core.Properties;
using MessagingToolkit.Core.Mobile.Message;
using MessagingToolkit.Core.Mobile.Event;
using MessagingToolkit.Core.Mobile.PduLibrary;
using MessagingToolkit.Core.Service;
using MessagingToolkit.Core.Helper;
using MessagingToolkit.MMS;

namespace MessagingToolkit.Core.Mobile
{
    /// <summary>
    /// Base class for mobile gateway
    /// </summary>
    /// <typeparam name="T">Derived mobile gateway</typeparam>
    internal abstract class BaseMobileGateway<T> : BaseGateway<T>, IDisposable
    {
        #region ============= Private Constants ==================================================

        /// <summary>
        /// Close wait interval in milliseconds
        /// </summary>
        private const int CloseWaitInterval = 1000;

        /// <summary>
        /// Read time out for the gateway in milliseconds
        /// </summary>
        private const int DefaultReadTimeout = 3000;

        /// <summary>
        /// Write time out for the gateway in milliseconds
        /// </summary>
        private const int DefaultWriteTimeout = 3000;

        /// <summary>
        /// Reset profile command
        /// </summary>
        private const string ResetProfileCommand = "ATZ";

        /// <summary>
        /// Set to no echo command
        /// </summary>
        private const string NoEchoCommand = "ATE0";

        /// <summary>
        /// Set default operator selection
        /// </summary>
        private const string DefaultOperatorSelectionCommand = "AT+COPS=0,{0}";

        /// <summary>
        /// Maximum sent SMS for unlicensed copy
        /// </summary>
        private const int UnlicensedMaximumSms = 100;


        /// <summary>
        /// Maximum sent MMS for unlicensed copy
        /// </summary>
        private const int UnlicensedMaximumMms = 10;


        /// <summary>
        /// Minimum length to read from the port
        /// </summary>
        private const int MinimumInboundDataLength = 10;


        #endregion ================================================================================


        #region =========== Private Variables =====================================================

        /// <summary>
        /// Track whether Dispose has been called
        /// </summary> 
        private bool disposed = false;


        /// <summary>
        /// Echo mode. Default to false
        /// </summary>
        private bool echo = false;

        /// <summary>
        /// Device information. E.g model, manufacturer
        /// </summary>
        private DeviceInformation deviceInformation;


        /// <summary>
        /// Service center address
        /// </summary>
        private NumberInformation serviceCentreAddress;


        /// <summary>
        /// Command sync lock
        /// </summary>
        private object commandSyncLock;


        /// <summary>
        /// Reader synch lock
        /// </summary>
        private object readerSyncLock;

        /// <summary>
        /// Synchronous message sending
        /// </summary>
        private object messageQueueLock;


        /// <summary>
        /// Thread to read incoming data from the gateway
        /// </summary>
        /// <returns></returns>
        private Thread inboundDataReader;

        /// <summary>
        /// Connection watch dog
        /// </summary>
        private Thread watchDog;

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
        /// Data queue for incoming data
        /// </summary>
        private IncomingDataQueue incomingDataQueue;

        /// <summary>
        /// USSD response queue
        /// </summary>
        private UssdResponseQueue ussdResponseQueue;

        /// <summary>
        /// Message queue
        /// </summary>
        private PriorityQueue<IMessage, MessageQueuePriority> messageQueue;

        /// <summary>
        /// Delayed message queue
        /// </summary>
        private DelayedMessageQueue delayedMessageQueue;

        /// <summary>
        /// Statistics 
        /// </summary>
        private Statistics statistics;

        /// <summary>
        /// Network registration status
        /// </summary>
        private NetworkRegistration networkRegistration;

        /// <summary>
        /// Supported message indication settings
        /// </summary>
        private MessageIndicationSupport messageIndicationSupport;


        /// <summary>
        /// Phone storages
        /// </summary>
        private string[] phoneStorages;

        /// <summary>
        /// Flag to poll for new messages
        /// </summary>
        private bool pollNewMessages;

        /// <summary>
        /// Flag to indicate if the gateway is still connected
        /// </summary>
        private bool isGatewayConnected;


        /// <summary>
        /// Flag to indicate if message queue is enabled
        /// </summary>
        private bool isMessageQueueEnabled;


        /// <summary>
        /// Serial port
        /// </summary>
        private SerialPort port;


        /// <summary>
        /// Indicate if message indication is enabled
        /// </summary>
        private bool isMessageIndicationEnabled;

        /// <summary>
        /// Internal serial port stream
        /// </summary>
        private Stream internalSerialStream;

        #endregion ===============================================================================


        #region =========== Protected Variables ==================================================

        /// <summary>
        /// Mobile gateway configuration
        /// </summary>
        protected MobileGatewayConfiguration config;


        /// <summary>
        /// Execution engine for the gateway
        /// </summary>
        protected DelegateEngine delegateEngine;


        /// <summary>
        /// Message memory location
        /// </summary>
        protected MessageStorage messageStorage;

        /// <summary>
        /// License
        /// </summary>
        protected License license;

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
        /// Message received event
        /// </summary>
        public event MessageReceivedEventHandler MessageReceived;

        /// <summary>
        /// USSD response received event
        /// </summary>
        public event UssdResponseReceivedHandler UssdResponseReceived;

        /// <summary>
        /// Raw received message event
        /// </summary>
        protected event RawMessageReceivedEventHandler RawMessageReceived;


        /// <summary>
        /// Incoming call received
        /// </summary>
        public event IncomingCallEventHandler CallReceived;

        /// <summary>
        /// Occurs when there is a need to automatically disconnect incoming call
        /// </summary>
        private event IncomingCallEventHandler DisconnectCallReceived;

        /// <summary>
        /// Outgoing call made
        /// </summary>
        public event OutgoingCallEventHandler CallDialled;

        /// <summary>
        /// Gateway connected
        /// </summary>
        //public event ConnectedEventHandler GatewayConnected;

        /// <summary>
        /// Gateway disconnected
        /// </summary>
        public event DisconnectedEventHandler GatewayDisconnected;


        /// <summary>
        /// Occurs when watch dog failed
        /// </summary>
        public event WatchDogFailureEventHandler WatchDogFailed;

        #endregion ===============================================================================


        #region ================== Constructor ===================================================

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="config">The config.</param>
        protected BaseMobileGateway(MobileGatewayConfiguration config)
            : base()
        {
            // Keep the configuration
            this.config = config;

            // Perform initialization
            PerformSetup();

        }


        /// <summary>
        /// Initializes this instance.
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

            // Initialize the port
            //Port = new SerialPort();
            //Port.ReadTimeout = DefaultReadTimeout;
            //Port.WriteTimeout = DefaultWriteTimeout;

            // Create the execution engine
            delegateEngine = DelegateEngine.NewInstance();

            // Default to phone
            messageStorage = MessageStorage.Phone;

            // Create the queue
            incomingDataQueue = new IncomingDataQueue();
            ussdResponseQueue = new UssdResponseQueue();

            // Default to null
            inboundDataReader = null;
            watchDog = null;
            messageQueueProcessor = null;
            messagePoller = null;

            // Initialize the locks
            commandSyncLock = new object();
            readerSyncLock = new object();
            messageQueueLock = new object();

            // Create the message queue
            messageQueue = new PriorityQueue<IMessage, MessageQueuePriority>(3, PersistedMessageType.Sms | PersistedMessageType.Mms);
            messageQueue.PersistFolder = config.PersistenceFolder;
            messageQueue.Persist = config.PersistenceQueue;

            // Create the delayed message queue
            delayedMessageQueue = new DelayedMessageQueue(PersistedMessageType.Sms | PersistedMessageType.Mms);
            delayedMessageQueue.PersistFolder = config.PersistenceFolder;
            delayedMessageQueue.Persist = config.PersistenceQueue;

            // Create statistics class
            statistics = new Statistics();

            // Default to not supported
            BatchMessageMode = BatchMessageMode.NotSupported;

            // Receive a raw message
            if (this.RawMessageReceived == null)
            {
                RawMessageReceived += OnRawMessageReceived;
            }

            // Default to null
            phoneStorages = null;

            // Default to false;
            pollNewMessages = false;

            // Gateway supports all  features by default
            Attributes = GatewayAttribute.DeliveryReport | GatewayAttribute.FlashSms |
                GatewayAttribute.LongMessage | GatewayAttribute.ReceiveByPolling | GatewayAttribute.ReceiveByTrigger |
                GatewayAttribute.Send | GatewayAttribute.SmartSms | GatewayAttribute.WapPush;

            // Default to false
            isGatewayConnected = false;

            // Set to stopped
            Status = GatewayStatus.Stopped;

            // Initialize the license
            license = new License(config);

            // Message queue is enabled by default
            isMessageQueueEnabled = true;

            // Set the gateway id
            this.Id = config.GatewayId;

            // Default to false to not to listen for incoming USSD event
            this.EnableUssdEvent = false;

            // Event handler when call is received
            if (this.DisconnectCallReceived == null)
                this.DisconnectCallReceived += new IncomingCallEventHandler(BaseMobileGateway_DisconnectCallReceived);

            // Default to false
            this.PersistenceQueue = false;

            // Set to default
            this.networkRegistration = null;

            this.isMessageIndicationEnabled = false;
        }


        #endregion ================================================================================



        # region ============ Destructor ===========================================================

        /// <summary>
        /// Use C# destructor syntax for finalization code.
        /// This destructor will run only if the Dispose method
        /// does not get called.
        /// It gives your base class the opportunity to finalize.
        /// Do not provide destructors in types derived from this class.
        /// </summary>
        ~BaseMobileGateway()
        {
            // Do not re-create Dispose clean-up code here.
            // Calling Dispose(false) is optimal in terms of
            // readability and maintainability.
            Dispose(false);
        }

        #endregion ==================================================================================



        #region =========== Protected Properties ======================================================




        #endregion =====================================================================================


        #region =========== Public Properties ===========================================================

        /// <summary>
        /// Physical connection to the gateway
        /// </summary>
        /// <value>Serial port. See <see cref="SerialPort"/></value>
        public SerialPort Port
        {
            get
            {
                return this.port;
            }
        }

        /// <summary>
        /// Return the gateway configuration
        /// </summary>
        /// <value>Configuration object</value>
        public virtual MobileGatewayConfiguration Configuration
        {
            get
            {
                return config;
            }
        }

        /// <summary>
        /// Return the gateway connection status
        /// </summary>
        /// <value>true if connected, false if it is disconnected</value>
        public virtual bool Connected
        {
            get
            {
                if (Port != null)
                {
                    return Port.IsOpen;
                }
                return false;
            }
        }

        /// <summary>
        /// Read time out in milliseconds
        /// </summary>
        /// <value>Time out value</value>
        public virtual int ReadTimeout
        {
            get
            {
                if (Port == null) return DefaultReadTimeout;
                return Port.ReadTimeout;
            }
            set
            {
                if (Port != null)
                    Port.ReadTimeout = value;
            }
        }

        /// <summary>
        /// Write time out in milliseconds
        /// </summary>
        /// <value>Time out value</value>
        public virtual int WriteTimeout
        {
            get
            {
                if (Port == null) return DefaultWriteTimeout;
                return Port.WriteTimeout;
            }
            set
            {
                if (Port != null)
                    Port.WriteTimeout = value;
            }
        }


        /// <summary>
        /// Set gateway echo mode
        /// </summary>
        /// <value>Echo mode</value>
        public virtual bool Echo
        {
            get
            {
                return echo;
            }
            set
            {
                echo = value;

                EchoFeature feature = GetFeature<EchoFeature>(EchoFeature.NewInstance());
                feature.Echo = echo;
                try
                {
                    IContext context = Execute(feature);
                }
                catch (Exception ex)
                {
                    this.exception = ex;
                }
            }
        }

        /// <summary>
        /// Return the device information. E.g. model, manufacturer, etc
        /// </summary>
        /// <value>Device information</value>
        public virtual DeviceInformation DeviceInformation
        {
            get
            {
                /*
                if (deviceInformation != null)
                {
                    return deviceInformation;
                }
                else
                {
                */
                // Retrieve the device information from the gateway
                DeviceInformationFeature feature = GetFeature<DeviceInformationFeature>(DeviceInformationFeature.NewInstance());
                try
                {
                    IContext context = Execute(feature);
                    deviceInformation = feature.DeviceInformation;
                    return deviceInformation;
                }
                catch (Exception ex)
                {
                    this.exception = ex;
                    return new DeviceInformation();
                }
                //}
            }
        }


        /// <summary>
        /// Gateway service center address
        /// </summary>
        /// <value>Service center number</value>
        public virtual NumberInformation ServiceCentreAddress
        {
            get
            {
                if (serviceCentreAddress != null)
                {
                    return serviceCentreAddress;
                }
                else
                {
                    // Retrieve the sca information from the gateway
                    GetScaFeature feature = GetFeature<GetScaFeature>(GetScaFeature.NewInstance());
                    try
                    {
                        IContext context = Execute(feature);
                        serviceCentreAddress = (NumberInformation)context.GetResult();
                        return serviceCentreAddress;
                    }
                    catch (Exception ex)
                    {
                        this.exception = ex;
                        return new NumberInformation();
                    }
                }
            }
        }

        /// <summary>
        /// Memory location for reading/deleting, writing/sending short messages.
        /// See <see cref="MessageStorage"/>
        /// </summary>
        /// <value>Message memory</value>        
        public virtual MessageStorage MessageStorage
        {
            get
            {
                return messageStorage;
            }
            set
            {
                //if (value != null)
                //{
                messageStorage = value;

                // Set the memory location in the gateway            
                MessageStorageFeature feature = GetFeature<MessageStorageFeature>(MessageStorageFeature.NewInstance());
                try
                {
                    feature.MessageStorage = messageStorage;
                    IContext context = Execute(feature);
                }
                catch (Exception ex)
                {
                    this.exception = ex;
                }
                //}
            }
        }

        /// <summary>
        /// Get the network registration status
        /// </summary>
        /// <value>Network registration status</value>
        public virtual NetworkRegistration NetworkRegistration
        {
            get
            {
                //if (networkRegistration != null)
                //{
                //    return networkRegistration;
                //}
                //else
                //{
                // Set the memory location in the gateway            
                NetworkRegistrationFeature feature = GetFeature<NetworkRegistrationFeature>(NetworkRegistrationFeature.NewInstance());
                try
                {
                    IContext context = Execute(feature);
                    networkRegistration = (NetworkRegistration)context.GetResult();
                }
                catch (Exception ex)
                {
                    this.exception = ex;
                }

                return networkRegistration;

                //}
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
        /// Determine if need to batch send the message
        /// </summary>
        /// <value>Can be disabled, temporary enabled, or disabled </value>
        public virtual BatchMessageMode BatchMessageMode
        {
            get;
            set;
        }


        /// <summary>
        /// Return the signal quality of the gateway
        /// </summary>
        /// <value>Signal quality</value>
        public virtual SignalQuality SignalQuality
        {
            get
            {
                // Get the signal quality     
                SignalQualityFeature feature = GetFeature<SignalQualityFeature>(SignalQualityFeature.NewInstance());
                try
                {
                    IContext context = Execute(feature);
                    return (SignalQuality)context.GetResult();
                }
                catch (Exception ex)
                {
                    this.exception = ex;
                }
                return new SignalQuality(0, 0);
            }
        }

        /// <summary>
        /// Get all supported character sets
        /// </summary>
        /// <value>List of supported character sets</value>
        public virtual string[] SupportedCharacterSets
        {
            get
            {
                // Get a list of supported character sets
                GetSupportedCharacterSetsFeature feature = GetFeature<GetSupportedCharacterSetsFeature>(GetSupportedCharacterSetsFeature.NewInstance());
                try
                {
                    IContext context = Execute(feature);
                    return (string[])context.GetResult();
                }
                catch (Exception ex)
                {
                    this.exception = ex;
                }
                return new string[] { };
            }
        }

        /// <summary>
        /// Get current character set
        /// </summary>
        /// <value>Current character set</value>
        public virtual string CurrentCharacterSet
        {
            get
            {
                // Get current character set
                GetCurrentCharacterSetFeature feature = GetFeature<GetCurrentCharacterSetFeature>(GetCurrentCharacterSetFeature.NewInstance());
                try
                {
                    IContext context = Execute(feature);
                    return (string)context.GetResult();
                }
                catch (Exception ex)
                {
                    this.exception = ex;
                }
                return string.Empty;
            }
        }

        /// <summary>
        /// Get battery charging information
        /// </summary>
        /// <value>Battery charge</value>
        public virtual BatteryCharge BatteryCharge
        {
            get
            {
                // Get current battery charging information
                GetBatteryChargeFeature feature = GetFeature<GetBatteryChargeFeature>(GetBatteryChargeFeature.NewInstance());
                try
                {
                    IContext context = Execute(feature);
                    return (BatteryCharge)context.GetResult();
                }
                catch (Exception ex)
                {
                    this.exception = ex;
                }
                return new BatteryCharge(0, 0);
            }
        }



        /// <summary>
        /// Check if needs to acknowledge routed messages
        /// </summary>
        /// <value>true if acknowledgement is required</value>
        public virtual bool IsAcknowledgeRequired
        {
            get
            {
                GetMessageServiceFeature feature = GetFeature<GetMessageServiceFeature>(GetMessageServiceFeature.NewInstance());
                try
                {
                    IContext context = Execute(feature);
                    MessageService messageService = (MessageService)context.GetResult();
                    return (messageService.Service == 1);
                }
                catch (Exception ex)
                {
                    this.exception = ex;
                }
                return false;
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


        /// <summary>
        /// Get a list of phone storages.
        /// E.g 
        /// MC - missed call
        /// RC - received call
        /// ON - own number
        /// DC - dialled number
        /// EN - emergency number
        /// FD - SIM fixed dialling phone book
        /// LD - SIM last dialling phone book
        /// ME - ME phone book
        /// MT - combined ME and SIM phone book
        /// SM - SIM phone book
        /// TA - TA phone book
        /// </summary>
        /// <value>A list of phone book storages</value>
        public virtual string[] PhoneBookStorages
        {
            get
            {
                if (phoneStorages != null) return phoneStorages;

                GetPhoneBookStorageFeature feature = GetFeature<GetPhoneBookStorageFeature>(GetPhoneBookStorageFeature.NewInstance());
                try
                {
                    IContext context = Execute(feature);
                    phoneStorages = (string[])context.GetResult();
                    return phoneStorages;
                }
                catch (Exception ex)
                {
                    this.exception = ex;
                }
                return new string[] { };
            }
        }


        /// <summary>
        /// Get message memory status. The message memory location is
        /// specified using <see cref="MessageStorage"/>
        /// </summary>
        /// <returns>Memory status</returns>
        public virtual MessageMemoryStatus MessageMemoryStatus
        {
            get
            {
                GetMessageStorageStatusFeature feature = GetFeature<GetMessageStorageStatusFeature>(GetMessageStorageStatusFeature.NewInstance());
                try
                {
                    feature.MessageStorage = this.MessageStorage;
                    IContext context = Execute(feature);
                    MessageMemoryStatus status = (MessageMemoryStatus)context.GetResult();
                    return status;
                }
                catch (Exception ex)
                {
                    this.exception = ex;
                }
                return new MessageMemoryStatus();
            }
        }

        /// <summary>
        /// Get current network operator
        /// </summary>
        /// <value>Network operator</value>
        public virtual NetworkOperator NetworkOperator
        {
            get
            {
                GetNetworkOperatorFeature feature = GetFeature<GetNetworkOperatorFeature>(GetNetworkOperatorFeature.NewInstance());
                try
                {
                    IContext context = Execute(feature);
                    NetworkOperator networkOperator = (NetworkOperator)context.GetResult();
                    return networkOperator;
                }
                catch (Exception ex)
                {
                    this.exception = ex;
                }
                return new NetworkOperator(NetworkOperatorFormat.LongFormatAlphanumeric, string.Empty);
            }
        }

        /// <summary>
        /// Retrieve subscriber information from the gateway
        /// </summary>
        /// <value>A list of subscriber instances</value>
        public virtual Subscriber[] Subscribers
        {
            get
            {
                GetSubscriberFeature feature = GetFeature<GetSubscriberFeature>(GetSubscriberFeature.NewInstance());
                try
                {
                    IContext context = Execute(feature);
                    Subscriber[] subscribers = (Subscriber[])context.GetResult();
                    return subscribers;
                }
                catch (Exception ex)
                {
                    this.exception = ex;
                }
                return new Subscriber[] { };

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
                                //messagePoller.Interrupt();
                                //if (!messagePoller.Join(500))
                                //{
                                messagePoller.Abort();
                                //}
                                messagePoller = null;
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
        /// Enables the incoming USSD event.
        /// Once enabled, SendUssd command will return immediately, and you have
        /// to use the Ussd received event to get the response.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [enable incoming ussd event]; otherwise, <c>false</c>.
        /// </value>
        /// <returns></returns>
        public virtual bool EnableUssdEvent
        {
            get;
            set;
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
                    List<IMessage> messages = this.messageQueue.Load(PersistedMessageType.Sms | PersistedMessageType.Mms);
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
        /// Gets or sets a value indicating whether the USSD command and response should be encoded.
        /// </summary>
        /// <value>
        ///   <c>true</c> if USSD command should be encoded; otherwise, <c>false</c>.
        /// </value>
        public virtual bool EncodedUssdCommand
        {
            get
            {
                return config.EncodedUssdCommand;
            }
            set
            {
                config.EncodedUssdCommand = value;
            }
        }

        #endregion ================================================================================

        #region =========== Internal Properties====================================================


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
        /// Object for reading mode synching
        /// </summary>
        /// <value>Reader object</value>
        internal object ReaderSyncLock
        {
            get
            {
                return readerSyncLock;
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

        /// <summary>
        /// Gets or sets the name of the data connection entry.
        /// </summary>
        /// <value>The name of the data connection entry.</value>
        internal string RasEntryName
        {
            get;
            set;
        }

        #endregion ================================================================================


        #region =========== Private Methods =======================================================


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

                // Call the appropriate methods to clean up
                // unmanaged resources here.
                // If disposing is false,
                // only the following code is executed.
                // If the port is already open, close it

                if (VerifyPort(Port))
                {
                    if (Port != null && Port.IsOpen)
                    {
                        Disconnect();
                    }
                }


                if (Port != null)
                {
                    if (config.SafeDisconnect)
                    {
                        SerialPortHelper.SafeDisconnect(Port, internalSerialStream);
                    }
                    else
                    {
                        try
                        {
                            Port.Close();
                        }
                        catch (Exception ex)
                        {
                            Logger.LogThis("Error closing port.", LogLevel.Error, this.Id);
                            Logger.LogThis(ex.Message, LogLevel.Error, this.Id);
                            Logger.LogThis(ex.StackTrace, LogLevel.Error, this.Id);
                        }


                        try
                        {
                            Port.Dispose();
                        }
                        catch (Exception ex)
                        {
                            Logger.LogThis("Error disposing port.", LogLevel.Error, this.Id);
                            Logger.LogThis(ex.Message, LogLevel.Error, this.Id);
                            Logger.LogThis(ex.StackTrace, LogLevel.Error, this.Id);
                        }
                    }
                    this.port = null;
                }

                // Note disposing has been done.
                disposed = true;

            }
        }

       // private void LoadPersistenceQueue()
        //{
            // Commented Dec 22nd 2014
            //this.delayedMessageQueue.Persist = this.Configuration.PersistenceQueue;
           // this.delayedMessageQueue.PersistFolder = this.Configuration.PersistenceFolder;
           // this.messageQueue.Persist = this.Configuration.PersistenceQueue;
            //this.messageQueue.PersistFolder = this.Configuration.PersistenceFolder;
            /*
            if (this.messageQueue.Persist)
            {
                List<IMessage> messages = this.messageQueue.Load(PersistedMessageType.Sms | PersistedMessageType.Mms);
                foreach (IMessage message in messages)
                {
                    SendToQueue(message);
                }

            }
            */
       // }

        #endregion ================================================================================

        #region =========== Public Methods ========================================================

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

            // ------ Commented - July 14 2012 ------------
            GC.SuppressFinalize(this);
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

                // Check if the port exists                
                CheckPort();

                // Open the port
                OpenPort();

                // Set to true
                isGatewayConnected = true;

                // Start the reader thread
                StartDataReader();

                // Connect to gateway
                ConnectGateway();

                // Configure the gateway
                ConfigureGateway();

                // Start the watch dog
                StartWatchDog();

                // Start the message queue processor
                StartMessageQueue();

                // Start the delayed message queue
                StartDelayedMessageQueue();

                // Call the set up method
                OnConnected();

                // Clear the queue
                incomingDataQueue.Clear();

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
                catch (Exception e) { }

                return false;
            }

            return true;
        }

        /// <summary>
        /// Disconnect from the gateway
        /// </summary>
        /// <returns>true if success, false otherwise</returns>
        public virtual bool Disconnect()
        {
            Status = GatewayStatus.Stopping;

            // Terminate the data reader
            StopDataReader();

            // Stop the watch dog
            StopWatchDog();

            // Stop the message queue
            StopMessageQueue();

            // Stop delayed message queue
            StopDelayedMessageQueue();

            // Stop message poller
            StopMessagePoller();

            // Clear the queue
            incomingDataQueue.Clear();

            // Close the port
            if (!ClosePort()) return false;

            // Call the disconnect method
            OnDisconnected();

            Status = GatewayStatus.Stopped;

            // Raise the event
            OnGatewayDisconnected();


            // Initialize all variables
            PerformSetup();

            return true;
        }


        /// <summary>
        /// Temporary disconnect from the gateway
        /// </summary>
        /// <returns>true if success, false otherwise</returns>
        public virtual bool Disable()
        {
            //Status = GatewayStatus.Stopping;

            // Terminate the data reader
            StopDataReader();

            // Stop the watch dog
            StopWatchDog();

            // Stop the message queue
            //StopMessageQueue();

            // Stop message poller
            StopMessagePoller();

            // Clear the queue
            incomingDataQueue.Clear();

            // Close the port
            if (!ClosePort()) return false;

            // Call the disconnect method
            //OnDisconnected();

            //Status = GatewayStatus.Stopped;

            return true;
        }

        /// <summary>
        /// Send message
        /// </summary>
        /// <param name="message">Message object</param>
        /// <returns>true if sending is successful</returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public virtual bool Send(IMessage message)
        {
            try
            {
                if (message == null) return false;

                if (message.ScheduledDeliveryDate != null && message.ScheduledDeliveryDate > DateTime.Now)
                {
                    // Set the message identifier
                    if (string.IsNullOrEmpty(message.Identifier))
                    {
                        message.Identifier = GatewayHelper.GenerateUniqueIdentifier(); 
                    }

                    Logger.LogThis(string.Format("Send to delayed message queue. ID is [{0}]. Scheduled date is [{1}]", message.Identifier, message.ScheduledDeliveryDate.ToString()), LogLevel.Verbose, this.Id);
                    delayedMessageQueue.QueueMessage(message);
                    return true; 
                }

                for (int i = 1; i <= config.SendRetries; i++)
                {
                    try
                    {
                        if (message is Mms)
                        {
                            Mms mms = (Mms)message;
                            SendMms(mms);
                        }
                        else
                        {
                            // Set message sendingmode
                            SetMessageBatchMode();
                            Sms sms = (Sms)message;
                            SendMessage(sms);
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
                        // Sleep for longer time upon exceptions
                        Thread.Sleep(1000);
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
        /// Send message to queue
        /// </summary>
        /// <param name="message">Message object</param>
        /// <returns>true if sending is successful</returns>
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
        /// Send a list of commands to the gateway to determine its capabilities.
        /// Useful for troubleshooting
        /// </summary>
        /// <remarks>
        /// Besides running those predefined commands, extra commands specified by putting a
        /// file called "diagnose.cmd" in the executable directory
        /// </remarks>
        /// <returns>Diagnostic results</returns>
        public virtual string Diagnose()
        {
            string[] diagnosticsCommand = Resources.DiagnosticCommand.Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
            List<string> commandList = diagnosticsCommand.ToList<string>();
            string currentDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string commandFile = currentDirectory + Path.DirectorySeparatorChar + config.CommandFile;
            if (File.Exists(commandFile))
            {
                string[] fileContent = File.ReadAllLines(commandFile, Encoding.UTF8);
                List<string> fileContentList = fileContent.ToList<string>();
                commandList.AddRange(fileContentList);
            }
            string executionOutput = string.Empty;
            // Execute the command 1 by 1 and return the result
            ExecuteFeature feature = GetFeature<ExecuteFeature>(ExecuteFeature.NewInstance());
            foreach (string command in commandList)
            {
                feature.Request = command;
                IContext context = Execute(feature);
                executionOutput += command + "\n";
                executionOutput += context.GetResultString() + "\n";

            }
            return executionOutput;
        }

        /// <summary>
        /// Retrieve message from the gateway. Set the memory location
        /// using <see cref="MessageStorage"/>.
        /// If there are messages in the gateway, but an empty list is returned, check
        /// the last error returned for any exceptions
        /// </summary>
        /// <param name="messageType">Message type. See <see cref="MessageStatusType"/></param>
        /// <returns>A list of messages</returns>
        public virtual List<MessageInformation> GetMessages(MessageStatusType messageType)
        {
            // Get the messages from the gateway              
            GetMessageFeature feature = GetFeature<GetMessageFeature>(GetMessageFeature.NewInstance());
            feature.ConcatenateMessage = config.ConcatenateMessage;
            feature.Sort = false;
            try
            {
                feature.MessageStatusType = messageType;
                IContext context = Execute(feature);
                List<MessageInformation> messages = (List<MessageInformation>)context.GetResult();
                return messages;
            }
            catch (Exception ex)
            {
                this.exception = ex;
            }
            // Return an empty list
            return new List<MessageInformation>();
        }


        /// <summary>
        /// Gets the messages.
        /// </summary>
        /// <param name="messageType">Type of the message.</param>
        /// <param name="storages">The storages.</param>
        /// <returns></returns>
        public List<MessageInformation> GetMessages(MessageStatusType messageType, List<MessageStorage> storages)
        {
            // Get the messages from the gateway              
            GetMessageFeature feature = GetFeature<GetMessageFeature>(GetMessageFeature.NewInstance());
            feature.ConcatenateMessage = config.ConcatenateMessage;
            feature.Sort = false;
            MessageStorage currentStorage = this.MessageStorage;
            try
            {
                List<MessageInformation> allMessages = new List<MessageInformation>();
                foreach (MessageStorage storage in storages)
                {
                    this.MessageStorage = storage;
                    feature.MessageStatusType = messageType;
                    IContext context = Execute(feature);
                    List<MessageInformation> messages = (List<MessageInformation>)context.GetResult();
                    allMessages.AddRange(messages);
                }
                return allMessages;
            }
            catch (Exception ex)
            {
                this.exception = ex;
            }
            finally
            {
                this.MessageStorage = currentStorage;
            }
            // Return an empty list
            return new List<MessageInformation>();
        }


        /// <summary>
        /// Retrieve message from the gateway using the index. Set the memory location
        /// using <see cref="MessageStorage"/>.
        /// If the message content is the PDU code, turn the debugging mode on for
        /// further troubleshooting
        /// </summary>
        /// <param name="index">Message index</param>
        /// <returns>Message information</returns>
        public virtual MessageInformation GetMessage(int index)
        {
            // Get the messages from the gateway              
            GetMessageByIndexFeature feature = GetFeature<GetMessageByIndexFeature>(GetMessageByIndexFeature.NewInstance());
            try
            {
                feature.Index = index;
                IContext context = Execute(feature);
                return (MessageInformation)context.GetResult();
            }
            catch (Exception ex)
            {
                this.exception = ex;
            }
            // Return an empty list
            return new MessageInformation();
        }

        /// <summary>
        /// Gets the MMS notifications.
        /// </summary>
        /// <param name="notificationType">Type of the notification.</param>
        /// <param name="notificationStatus">The notification status.</param>
        /// <returns>
        /// List of messages fulfilled the conditions
        /// </returns>
        public virtual List<MessageInformation> GetNotifications(NotificationType notificationType, NotificationStatus notificationStatus)
        {
            //int destinationPort = MmsConstants.MmsNotificationDestinationPort;
            //string pattern = MmsConstants.ContentTypeWapMmsMessage;

            if (notificationType == NotificationType.Mms)
            {
                if (notificationStatus == NotificationStatus.None)
                {
                    List<MessageInformation> messages = GetMessages(MessageStatusType.AllMessages);

                    return messages.FindAll(
                        delegate(MessageInformation msg)
                        {
                            return (msg.GetType() == typeof(MmsNotification));
                            //return (msg.DestinationPort == destinationPort && !string.IsNullOrEmpty(msg.Content) && msg.Content.Contains(pattern));
                        }
                        );
                }
                else if (notificationStatus == NotificationStatus.New)
                {
                    List<MessageInformation> messages = GetMessages(MessageStatusType.ReceivedUnreadMessages);
                    return messages.FindAll(
                      delegate(MessageInformation msg)
                      {
                          return (msg.GetType() == typeof(MmsNotification));
                          //return (msg.DestinationPort == destinationPort && !string.IsNullOrEmpty(msg.Content) && msg.Content.Contains(pattern));
                      }
                      );
                }
            }
            return new List<MessageInformation>(1);
        }

        /// <summary>
        /// Gets the MMS.
        /// </summary>
        /// <param name="mmsNotification">The MMS notification.</param>
        /// <param name="mms">The MMS.</param>
        /// <returns>The MMS object if found</returns>
        public virtual bool GetMms(MessageInformation mmsNotification, ref Mms mms)
        {
            try
            {
                for (int i = 0; i <= config.SendRetries; i++)
                {
                    try
                    {
                        if (RetrieveMms(mmsNotification, ref mms))
                        {
                            return true;
                        }
                    }
                    catch (Exception e)
                    {
                        Logger.LogThis("Retrieving message: " + e.Message, LogLevel.Info, this.Id);
                        Logger.LogThis("Trace: " + e.StackTrace, LogLevel.Info, this.Id);
                        if (i == config.SendRetries)
                        {
                            throw e;

                        }
                        // Sleep for longer time upon exceptions
                        Thread.Sleep(1000);
                    }
                }
            }
            catch (Exception ex)
            {
                // Assign to the internal variable
                this.exception = ex;
            }

            return false;
        }


        /// <summary>
        /// Delete message from the gateway. If delete by index, then
        /// index must be passed in. The message to be deleted can be from phone or SIM, set 
        /// using <see cref="MessageStorage"/>
        /// </summary>
        /// <param name="messageDeleteOption">Message deletion option. See <see cref="MessageDeleteOption"/></param>
        /// <param name="indexes">Message index</param>
        /// <returns>
        /// true if deletion is successul, false if failed. You can check the last error for any exceptions thrown
        /// </returns>
        public virtual bool DeleteMessage(MessageDeleteOption messageDeleteOption, params int[] indexes)
        {
            // If deletion by index, then the index must be passed in
            if (messageDeleteOption == MessageDeleteOption.ByIndex && (indexes == null || indexes.Length == 0))
            {
                this.exception = new GatewayException(Resources.MessageIndexNotFound);
                return false;
            }

            // Get the messages from the gateway              
            DeleteMessageFeature feature = GetFeature<DeleteMessageFeature>(DeleteMessageFeature.NewInstance());
            try
            {
                feature.MessageDeleteOption = messageDeleteOption;
                if (messageDeleteOption != MessageDeleteOption.ByIndex)
                {
                    IContext context = Execute(feature);
                }
                else
                {
                    foreach (int index in indexes)
                    {
                        feature.MessageIndex = index;
                        IContext context = Execute(feature);
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                this.exception = ex;
            }
            return false;
        }

        /// <summary>
        /// Send command directly to the gateway
        /// </summary>
        /// <param name="command">Command string</param>
        /// <returns>Execution results</returns>
        public virtual string SendCommand(string command)
        {
            // Run the command           
            ExecuteFeature feature = GetFeature<ExecuteFeature>(ExecuteFeature.NewInstance());
            try
            {
                feature.Request = command;
                IContext context = Execute(feature);
                return context.GetResultString();
            }
            catch (Exception ex)
            {
                this.exception = ex;
            }
            // Return an empty list
            return StringEnum.GetStringValue(Response.Error).Trim();
        }

        /// <summary>
        /// Enable new message notification
        /// </summary>
        /// <param name="notificationFlag">Notification flag</param>
        /// <returns>true if can be set</returns>
        public virtual bool EnableNewMessageNotification(MessageNotification notificationFlag)
        {
            try
            {
                MessageIndicationSupport supportedIndications = GetSupportedMessageIndications();
                MessageIndicationMode bufferAndFlush = MessageIndicationMode.BufferAndFlush;
                if (!supportedIndications.SupportsMode(bufferAndFlush))
                {
                    if (!supportedIndications.SupportsMode(MessageIndicationMode.SkipWhenReserved))
                    {
                        if (!supportedIndications.SupportsMode(MessageIndicationMode.ForwardAlways))
                        {
                            Logger.LogThis("The phone does not support any of the required message indication modes.", LogLevel.Info, this.Id);
                            return false;
                        }
                        bufferAndFlush = MessageIndicationMode.ForwardAlways;
                    }
                    else
                    {
                        bufferAndFlush = MessageIndicationMode.SkipWhenReserved;
                    }
                }
                ReceivedMessageIndication routeMessage = ReceivedMessageIndication.Disabled;
                if ((notificationFlag & MessageNotification.ReceivedMessage) == MessageNotification.ReceivedMessage)
                {
                    routeMessage = ReceivedMessageIndication.RouteMemoryLocation;
                    if (!supportedIndications.SupportsDeliverSetting(routeMessage))
                    {
                        Logger.LogThis("The phone does not support notification for standard SMS (SMS-DELIVER) messages.", LogLevel.Info, this.Id);
                        routeMessage = ReceivedMessageIndication.RouteMessage;
                        if (!supportedIndications.SupportsDeliverSetting(routeMessage))
                        {
                            Logger.LogThis("The phone does not support routing of standard SMS (SMS-DELIVER) messages.", LogLevel.Info, this.Id);
                            return false;
                        }
                    }
                }

                // Disable cell broadcast
                CellBroadcastMessageIndication disabled = CellBroadcastMessageIndication.Disabled;

                StatusReportMessageIndication statusReport = StatusReportMessageIndication.Disabled;
                if ((notificationFlag & MessageNotification.StatusReport) == MessageNotification.StatusReport)
                {
                    statusReport = StatusReportMessageIndication.RouteMemoryLocation;
                    if (!supportedIndications.SupportsStatusReportSetting(statusReport))
                    {
                        // For status report if not supported then check if can just route the message
                        statusReport = StatusReportMessageIndication.RouteMessage;
                        if (!supportedIndications.SupportsStatusReportSetting(statusReport))
                        {
                            Logger.LogThis("Attention: The phone does not support notification about new status reports. As a fallback it will be disabled.", LogLevel.Info, this.Id);
                            statusReport = StatusReportMessageIndication.Disabled;
                        }
                    }
                }
                MessageBufferIndication flush = MessageBufferIndication.Flush;
                if (!supportedIndications.SupportsBufferSetting(flush))
                {
                    if (!supportedIndications.SupportsBufferSetting(MessageBufferIndication.Clear))
                    {
                        Logger.LogThis("The phone does not support any of the required buffer settings.", LogLevel.Info, this.Id);
                        return false;
                    }
                    flush = MessageBufferIndication.Clear;
                }
                MessageIndicationSettings settings = new MessageIndicationSettings(bufferAndFlush, routeMessage, disabled, statusReport, flush);
                SetMessageIndications(settings);
                isMessageIndicationEnabled = true;
                return true;
            }
            catch (Exception ex)
            {
                this.exception = ex;
            }
            return false;
        }


        /// <summary>
        /// Enable message indications for received messages
        /// </summary>
        /// <returns>true if successful</returns>
        public virtual bool EnableMessageNotifications()
        {
            try
            {
                MessageIndicationSupport supportedIndications = GetSupportedMessageIndications();
                MessageIndicationMode bufferAndFlush = MessageIndicationMode.BufferAndFlush;
                if (!supportedIndications.SupportsMode(bufferAndFlush))
                {
                    if (!supportedIndications.SupportsMode(MessageIndicationMode.SkipWhenReserved))
                    {
                        if (!supportedIndications.SupportsMode(MessageIndicationMode.ForwardAlways))
                        {
                            Logger.LogThis("The phone does not support any of the required message indication modes.", LogLevel.Info, this.Id);
                            return false;
                        }
                        bufferAndFlush = MessageIndicationMode.ForwardAlways;
                    }
                    else
                    {
                        bufferAndFlush = MessageIndicationMode.SkipWhenReserved;
                    }
                }
                ReceivedMessageIndication routeMemoryLocation = ReceivedMessageIndication.RouteMemoryLocation;
                if (!supportedIndications.SupportsDeliverSetting(routeMemoryLocation))
                {
                    Logger.LogThis("The phone does not support notification for standard SMS (SMS-DELIVER) messages.", LogLevel.Info, this.Id);
                    return false;
                }

                CellBroadcastMessageIndication disabled = CellBroadcastMessageIndication.Disabled;
                StatusReportMessageIndication statusReport = StatusReportMessageIndication.RouteMemoryLocation;
                if (!supportedIndications.SupportsStatusReportSetting(statusReport))
                {
                    // For status report if not supported then check if can just route the message
                    statusReport = StatusReportMessageIndication.RouteMessage;
                    if (!supportedIndications.SupportsStatusReportSetting(statusReport))
                    {
                        Logger.LogThis("Attention: The phone does not support notification about new status reports. As a fallback it will be disabled.", LogLevel.Info, this.Id);
                        statusReport = StatusReportMessageIndication.Disabled;
                    }
                }
                MessageBufferIndication flush = MessageBufferIndication.Flush;
                if (!supportedIndications.SupportsBufferSetting(flush))
                {
                    if (!supportedIndications.SupportsBufferSetting(MessageBufferIndication.Clear))
                    {
                        Logger.LogThis("The phone does not support any of the required buffer settings.", LogLevel.Info, this.Id);
                        return false;
                    }
                    flush = MessageBufferIndication.Clear;
                }
                MessageIndicationSettings settings = new MessageIndicationSettings(bufferAndFlush, routeMemoryLocation, disabled, statusReport, flush);
                SetMessageIndications(settings);
                isMessageIndicationEnabled = true;
                return true;
            }
            catch (Exception ex)
            {
                this.exception = ex;
            }
            return false;
        }


        /// <summary>
        /// Disable message notifications
        /// </summary>
        /// <returns>true if can be set successfully</returns>
        public virtual bool DisableMessageNotifications()
        {
            isMessageIndicationEnabled = false;
            if (
                ResetMessageIndications(new MessageIndicationSettings(MessageIndicationMode.DoNotForward, 0, 0, 0, 0)) ||
                ResetMessageIndications(new MessageIndicationSettings(MessageIndicationMode.BufferAndFlush, 0, 0, 0, 0)) ||
                ResetMessageIndications(new MessageIndicationSettings(MessageIndicationMode.SkipWhenReserved, 0, 0, 0, 0)) ||
                ResetMessageIndications(new MessageIndicationSettings(MessageIndicationMode.ForwardAlways, 0, 0, 0, 0))
                )
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Disable message routing
        /// </summary>
        /// <returns>true if can be set successfully</returns>
        public virtual bool DisableMessageRouting()
        {
            isMessageIndicationEnabled = false;
            return DisableMessageNotifications();
        }

        /// <summary>
        /// Enable message routing, meaning any messages received at the gateway
        /// are routed to the application without saving them
        /// </summary>
        /// <returns>true if enabled successfully</returns>
        public virtual bool EnableMessageRouting()
        {
            try
            {
                MessageIndicationSupport supportedIndications = GetSupportedMessageIndications();
                MessageIndicationMode bufferAndFlush = MessageIndicationMode.BufferAndFlush;
                if (!supportedIndications.SupportsMode(bufferAndFlush))
                {
                    if (!supportedIndications.SupportsMode(MessageIndicationMode.SkipWhenReserved))
                    {
                        if (!supportedIndications.SupportsMode(MessageIndicationMode.ForwardAlways))
                        {
                            Logger.LogThis("The phone does not support any of the required message indication modes.", LogLevel.Info, this.Id);
                            return false;
                        }
                        bufferAndFlush = MessageIndicationMode.ForwardAlways;
                    }
                    else
                    {
                        bufferAndFlush = MessageIndicationMode.SkipWhenReserved;
                    }
                }
                ReceivedMessageIndication routeMessage = ReceivedMessageIndication.RouteMessage;
                if (!supportedIndications.SupportsDeliverSetting(routeMessage))
                {
                    Logger.LogThis("The phone does not support routing of standard SMS (SMS-DELIVER) messages.", LogLevel.Info, this.Id);
                    return false;
                }
                CellBroadcastMessageIndication disabled = CellBroadcastMessageIndication.Disabled;
                StatusReportMessageIndication statusReport = StatusReportMessageIndication.RouteMessage;
                if (!supportedIndications.SupportsStatusReportSetting(statusReport))
                {
                    Logger.LogThis("Attention: The phone does not support routing of new status reports. As a fallback it will be disabled.", LogLevel.Info, this.Id);
                    statusReport = StatusReportMessageIndication.Disabled;
                }
                MessageBufferIndication flush = MessageBufferIndication.Flush;
                if (!supportedIndications.SupportsBufferSetting(flush))
                {
                    if (!supportedIndications.SupportsBufferSetting(MessageBufferIndication.Clear))
                    {
                        Logger.LogThis("The phone does not support any of the required buffer settings.", LogLevel.Info, this.Id);
                    }
                    flush = MessageBufferIndication.Clear;
                }
                MessageIndicationSettings settings = new MessageIndicationSettings(bufferAndFlush, routeMessage, disabled, statusReport, flush);
                SetMessageIndications(settings);
                isMessageIndicationEnabled = true;
                return true;
            }
            catch (Exception ex)
            {
                this.exception = ex;
            }
            return false;
        }

        /// <summary>
        /// Enable calling line identity presentation
        /// </summary>
        /// <returns></returns>
        public bool EnableCallNotifications()
        {
            ClipFeature feature = GetFeature<ClipFeature>(ClipFeature.NewInstance());
            try
            {
                feature.Mode = CapabilityMode.On;
                IContext context = Execute(feature);
                return true;
            }
            catch (Exception ex)
            {
                this.exception = ex;
            }
            return false;
        }

        /// <summary>
        /// Disable  calling line identity presentation
        /// </summary>
        /// <returns></returns>
        public bool DisableCallNotifications()
        {
            ClipFeature feature = GetFeature<ClipFeature>(ClipFeature.NewInstance());
            try
            {
                feature.Mode = CapabilityMode.Off;
                IContext context = Execute(feature);
                return true;
            }
            catch (Exception ex)
            {
                this.exception = ex;
            }
            return false;
        }

        /// <summary>
        /// Enable calling line identity restriction
        /// </summary>
        /// <returns></returns>
        public bool EnableCLIR()
        {
            ClirFeature feature = GetFeature<ClirFeature>(ClirFeature.NewInstance());
            try
            {
                feature.Mode = CapabilityMode.On;
                IContext context = Execute(feature);
                return true;
            }
            catch (Exception ex)
            {
                this.exception = ex;
            }
            return false;
        }

        /// <summary>
        /// Disable calling line identity restriction
        /// </summary>
        /// <returns></returns>
        public bool DisableCLIR()
        {
            ClirFeature feature = GetFeature<ClirFeature>(ClirFeature.NewInstance());
            try
            {
                feature.Mode = CapabilityMode.Off;
                IContext context = Execute(feature);
                return true;
            }
            catch (Exception ex)
            {
                this.exception = ex;
            }
            return false;
        }


        /// <summary>
        /// Enable connected line identification presentation
        /// </summary>
        /// <returns>true if successful, false otherwise</returns>
        public bool EnableCOLP()
        {
            ColpFeature feature = GetFeature<ColpFeature>(ColpFeature.NewInstance());
            try
            {
                feature.Mode = CapabilityMode.On;
                IContext context = Execute(feature);
                return true;
            }
            catch (Exception ex)
            {
                this.exception = ex;
            }
            return false;
        }

        /// <summary>
        /// Disable connected line identification presentation
        /// </summary>
        /// <returns>true if successful, false otherwise</returns>
        public bool DisableCOLP()
        {
            ColpFeature feature = GetFeature<ColpFeature>(ColpFeature.NewInstance());
            try
            {
                feature.Mode = CapabilityMode.Off;
                IContext context = Execute(feature);
                return true;
            }
            catch (Exception ex)
            {
                this.exception = ex;
            }
            return false;
        }

        /// <summary>
        /// Set current character set
        /// </summary>
        /// <param name="characterSet">Character set</param>
        /// <returns>true if successfull set</returns>
        public virtual bool SetCharacterSet(string characterSet)
        {
            SetCharacterSetFeature feature = GetFeature<SetCharacterSetFeature>(SetCharacterSetFeature.NewInstance());
            try
            {
                feature.CharacterSet = characterSet;
                IContext context = Execute(feature);
                return true;
            }
            catch (Exception ex)
            {
                this.exception = ex;
            }
            return false;
        }

        /// <summary>
        /// Set service centre address
        /// </summary>
        /// <param name="address">Service centre address</param>
        /// <returns>true if successfully set</returns>
        public virtual bool SetServiceCentreAddress(NumberInformation address)
        {
            SetServiceCentreAddressFeature feature = GetFeature<SetServiceCentreAddressFeature>(SetServiceCentreAddressFeature.NewInstance());
            try
            {
                feature.ServiceCentreAddress = address;
                IContext context = Execute(feature);
                return true;
            }
            catch (Exception ex)
            {
                this.exception = ex;
            }
            return false;
        }

        /// <summary>
        /// Acknowledge a routed message if <see cref="IsAcknowledgeRequired"/> is true
        /// </summary>
        /// <returns>true if acknowledged successfully</returns>
        public virtual bool AcknowledgeNewMessage()
        {
            AcknowledgeMessageFeature feature = GetFeature<AcknowledgeMessageFeature>(AcknowledgeMessageFeature.NewInstance());
            try
            {
                feature.Acknowledge = true;
                IContext context = Execute(feature);
                return true;
            }
            catch (Exception ex)
            {
                this.exception = ex;
            }
            return false;
        }

        /// <summary>
        /// Set the flag acknowledge new message. True if need to acknowledge
        /// </summary>
        /// <param name="flag">Acknowledge flag</param>
        /// <returns>true if successfully set</returns>
        public virtual bool RequireAcknowledgeNewMessage(bool flag)
        {
            SetMessageServiceFeature feature = GetFeature<SetMessageServiceFeature>(SetMessageServiceFeature.NewInstance());
            try
            {
                feature.AcknowledgeRequired = flag;
                IContext context = Execute(feature);
                return true;
            }
            catch (Exception ex)
            {
                this.exception = ex;
            }
            return false;
        }


        /// <summary>
        /// Dial a phone number
        /// </summary>
        /// <param name="phoneNumber">Phone number</param>
        /// <returns>true if successful</returns>
        public virtual bool Dial(string phoneNumber)
        {
            if (string.IsNullOrEmpty(phoneNumber)) return false;

            DialFeature feature = GetFeature<DialFeature>(DialFeature.NewInstance());
            try
            {
                feature.PhoneNumber = phoneNumber;
                IContext context = Execute(feature);

                // Raise outgoing call event
                OnCallDialled(phoneNumber);

                // Increment counter
                this.statistics.OutgoingCall++;

                return true;
            }
            catch (Exception ex)
            {
                this.exception = ex;
            }
            return false;
        }

        /// <summary>
        /// Hang up the call in progress
        /// </summary>
        /// <returns>true if successful</returns>
        public virtual bool HangUp()
        {
            HangUpFeature feature = GetFeature<HangUpFeature>(HangUpFeature.NewInstance());
            try
            {
                IContext context = Execute(feature);
                return true;
            }
            catch (Exception ex)
            {
                this.exception = ex;
            }
            return false;
        }


        /// <summary>
        /// Anwser the call
        /// </summary>
        /// <returns>true if can be answered</returns>
        public virtual bool Answer()
        {
            AnswerCallFeature feature = GetFeature<AnswerCallFeature>(AnswerCallFeature.NewInstance());
            try
            {
                IContext context = Execute(feature);
                return true;
            }
            catch (Exception ex)
            {
                this.exception = ex;
            }
            return false;
        }

        /// <summary>
        /// Get a list of phone book entries from the specified storage
        /// </summary>
        /// <param name="storage">Phone book storage</param>
        /// <returns>A list of phone book entries</returns>
        public virtual PhoneBookEntry[] GetPhoneBook(string storage)
        {
            try
            {
                // Set phone book storage  
                SetPhoneBookStorageFeature storagefeature = GetFeature<SetPhoneBookStorageFeature>(SetPhoneBookStorageFeature.NewInstance());
                storagefeature.Storage = storage;
                IContext context = Execute(storagefeature);

                // Read the phone book size
                IFeature feature = GetFeature<GetPhoneBookSizeFeature>(GetPhoneBookSizeFeature.NewInstance());
                context = Execute(feature);
                PhoneBookSize phoneBookSize = (PhoneBookSize)context.GetResult();

                // Read the phone book entries
                GetPhoneBookEntriesFeature phoneBookFeature = GetFeature<GetPhoneBookEntriesFeature>(GetPhoneBookEntriesFeature.NewInstance());
                phoneBookFeature.PhoneBookSize = phoneBookSize;
                context = Execute(phoneBookFeature);
                PhoneBookEntry[] phoneBookEntries = (PhoneBookEntry[])context.GetResult();
                return phoneBookEntries;
            }
            catch (Exception ex)
            {
                this.exception = ex;
            }

            return new PhoneBookEntry[] { };
        }

        /// <summary>
        /// Get a list of phone book entries from the specified storage
        /// </summary>
        /// <param name="storage">Phone book storage enumeration. See <see cref="PhoneBookStorage"/></param>
        /// <returns>A list of phone book entries</returns>
        public virtual PhoneBookEntry[] GetPhoneBook(PhoneBookStorage storage)
        {
            return GetPhoneBook(StringEnum.GetStringValue(storage));
        }


        /// <summary>
        /// Retrieve phone book memory storage status
        /// </summary>
        /// <param name="storage">Phone book storage</param>
        /// <returns>Memory status</returns>
        public virtual MemoryStatusWithStorage GetPhoneBookMemoryStatus(string storage)
        {
            try
            {
                // Set phone book storage  
                SetPhoneBookStorageFeature storagefeature = GetFeature<SetPhoneBookStorageFeature>(SetPhoneBookStorageFeature.NewInstance());
                storagefeature.Storage = storage;
                IContext context = Execute(storagefeature);

                GetPhoneBookMemoryStatusFeature feature = GetFeature<GetPhoneBookMemoryStatusFeature>(GetPhoneBookMemoryStatusFeature.NewInstance());
                context = Execute(feature);
                return (MemoryStatusWithStorage)context.GetResult();
            }
            catch (Exception ex)
            {
                this.exception = ex;
            }
            return new MemoryStatusWithStorage(string.Empty, 0, 0);
        }


        /// <summary>
        /// Retrieve phone book memory storage status
        /// </summary>
        /// <param name="storage">Phone book storage enumeration. See <see cref="PhoneBookStorage"/></param>
        /// <returns>Memory status</returns>
        public virtual MemoryStatusWithStorage GetPhoneBookMemoryStatus(PhoneBookStorage storage)
        {
            return GetPhoneBookMemoryStatus(StringEnum.GetStringValue(storage));
        }

        /// <summary>
        /// Export phone book entries to vCard
        /// </summary>
        /// <param name="phoneBookEntries">Phone book entries to be exported</param>
        /// <returns>An array of vCard instances</returns>
        public virtual vCard[] ExportPhoneBookTovCard(PhoneBookEntry[] phoneBookEntries)
        {
            vCard[] vCards = new vCard[phoneBookEntries.Count()];
            int count = 0;
            foreach (PhoneBookEntry entry in phoneBookEntries)
            {
                vCard vCard = vCard.NewInstance();
                PhoneNumber p = new PhoneNumber();
                HomeWorkTypes homeWorkTypes = HomeWorkTypes.Work;
                PhoneTypes phoneTypes = PhoneTypes.Cell;
                string givenName = entry.Text.Replace("/H", string.Empty);
                givenName = givenName.Replace("/M", string.Empty);
                vCard.GivenName = givenName;
                p.Number = entry.Number;
                p.HomeWorkType = homeWorkTypes;
                p.PhoneType = phoneTypes;
                vCard.Phones.Add(p);
                vCards[count++] = vCard;
            }
            return vCards;
        }

        /// <summary>
        /// Get a list of supported network operators
        /// </summary>
        /// <returns>A list of network operators</returns>
        public virtual SupportedNetworkOperator[] GetSupportedNetworkOperators()
        {
            try
            {
                GetSupportedNetworkOperatorsFeature feature = GetFeature<GetSupportedNetworkOperatorsFeature>(GetSupportedNetworkOperatorsFeature.NewInstance());
                IContext context = Execute(feature);
                return (SupportedNetworkOperator[])context.GetResult();
            }
            catch (Exception ex)
            {
                this.exception = ex;
            }
            return new SupportedNetworkOperator[] { };
        }

        /// <summary>
        /// Add phone book entry to the specified storage
        /// </summary>
        /// <param name="entry">Phone book entry</param>
        /// <param name="storage">Phone book storage</param>
        /// <returns>true if successful</returns>
        public virtual bool AddPhoneBookEntry(PhoneBookEntry entry, string storage)
        {
            try
            {
                // Set phone book storage  
                SetPhoneBookStorageFeature storagefeature = GetFeature<SetPhoneBookStorageFeature>(SetPhoneBookStorageFeature.NewInstance());
                storagefeature.Storage = storage;
                IContext context = Execute(storagefeature);

                // Read the phone book size
                AddPhoneBookEntryFeature feature = GetFeature<AddPhoneBookEntryFeature>(AddPhoneBookEntryFeature.NewInstance());
                feature.PhoneBookEntry = entry;
                context = Execute(feature);
                return true;
            }
            catch (Exception ex)
            {
                this.exception = ex;
            }

            return false;
        }

        /// <summary>
        /// Add phone book entry to the specified storage
        /// </summary>
        /// <param name="entry">Phone book entry</param>
        /// <param name="storage">Phone book storage. See <see cref="PhoneBookStorage"/></param>
        /// <returns>true if successful</returns>
        public virtual bool AddPhoneBookEntry(PhoneBookEntry entry, PhoneBookStorage storage)
        {
            return AddPhoneBookEntry(entry, StringEnum.GetStringValue(storage));
        }

        /// <summary>
        /// Delete a specific phone book
        /// </summary>
        /// <param name="storage">Phone book storage</param>
        /// <returns>true if successful</returns>
        public virtual bool DeletePhoneBook(string storage)
        {
            try
            {
                // Get the phone book entries
                PhoneBookEntry[] phoneBookEntries = GetPhoneBook(storage);

                // Delete the phone book entry one by one             
                DeletePhoneBookEntryFeature feature = GetFeature<DeletePhoneBookEntryFeature>(DeletePhoneBookEntryFeature.NewInstance());
                foreach (PhoneBookEntry phoneBookEntry in phoneBookEntries)
                {
                    feature.Index = phoneBookEntry.Index;
                    IContext context = Execute(feature);
                }
                return true;
            }
            catch (Exception ex)
            {
                this.exception = ex;
            }

            return false;
        }

        /// <summary>
        /// Delete a specific phone book
        /// </summary>
        /// <param name="storage">Phone book storage. See <see cref="PhoneBookStorage"/></param>
        /// <returns>true if successful</returns>
        public virtual bool DeletePhoneBook(PhoneBookStorage storage)
        {
            return DeletePhoneBook(StringEnum.GetStringValue(storage));
        }

        /// <summary>
        /// Delete a specific phone book entry
        /// </summary>
        /// <param name="index">Phone book entry index</param>
        /// <param name="storage">Phone book storage</param>
        /// <returns>true if successful</returns>
        public virtual bool DeletePhoneBookEntry(int index, string storage)
        {
            try
            {
                // Set phone book storage  
                SetPhoneBookStorageFeature storagefeature = GetFeature<SetPhoneBookStorageFeature>(SetPhoneBookStorageFeature.NewInstance());
                storagefeature.Storage = storage;
                IContext context = Execute(storagefeature);

                // Delete the phone book entry by index     
                DeletePhoneBookEntryFeature feature = GetFeature<DeletePhoneBookEntryFeature>(DeletePhoneBookEntryFeature.NewInstance());
                feature.Index = index;
                context = Execute(feature);
                return true;
            }
            catch (Exception ex)
            {
                this.exception = ex;
            }

            return false;
        }

        /// <summary>
        /// Delete a specific phone book entry
        /// </summary>
        /// <param name="index">Phone book entry index</param>
        /// <param name="storage">Phone book storage. See <see cref="PhoneBookStorage"/></param>
        /// <returns>true if successful</returns>
        public virtual bool DeletePhoneBookEntry(int index, PhoneBookStorage storage)
        {
            return DeletePhoneBookEntry(index, StringEnum.GetStringValue(storage));
        }

        /// <summary>
        /// Search for phone book entries from the specified storage
        /// </summary>
        /// <param name="searchText">Text to search for</param>
        /// <param name="storage">Phone book storage</param>
        /// <returns>true if successful</returns>
        public virtual PhoneBookEntry[] FindPhoneBookEntries(string searchText, string storage)
        {
            try
            {
                // Set phone book storage  
                SetPhoneBookStorageFeature storagefeature = GetFeature<SetPhoneBookStorageFeature>(SetPhoneBookStorageFeature.NewInstance());
                storagefeature.Storage = storage;
                IContext context = Execute(storagefeature);

                // Find phone book entries     
                FindPhoneBookEntriesFeature feature = GetFeature<FindPhoneBookEntriesFeature>(FindPhoneBookEntriesFeature.NewInstance());
                feature.FindText = searchText;
                context = Execute(feature);
                return (PhoneBookEntry[])context.GetResult();
            }
            catch (Exception ex)
            {
                this.exception = ex;
            }
            return new PhoneBookEntry[] { };
        }

        /// <summary>
        /// Search for phone book entries from the specified storage
        /// </summary>
        /// <param name="searchText">Text to search for</param>
        /// <param name="storage">Phone book storage. See <see cref="PhoneBookStorage"/></param>
        /// <returns>true if successful</returns>
        public virtual PhoneBookEntry[] FindPhoneBookEntries(string searchText, PhoneBookStorage storage)
        {
            return FindPhoneBookEntries(searchText, StringEnum.GetStringValue(storage));
        }

        /// <summary>
        /// Save message to gateway. Message storage is specified using <see cref="MessageStorage"/>
        /// property
        /// </summary>
        /// <param name="message">Message content</param>
        /// <param name="status">Message status</param>
        /// <returns>true if successful</returns>
        public virtual List<int> SaveMessage(Sms message, MessageStatusType status)
        {
            try
            {
                // Save the message
                SaveMessageFeature feature = GetFeature<SaveMessageFeature>(SaveMessageFeature.NewInstance());
                feature.MessageStatus = status;
                feature.Message = message;
                IContext context = Execute(feature);
                return (List<int>)context.GetResult(); 
            }
            catch (Exception ex)
            {
                this.exception = ex;
            }
            return new List<int>();
        }

        /// <summary>
        /// Send USSD command to the network
        /// </summary>
        /// <param name="ussdCommand">USSD command</param>
        /// <returns>Execution result. If the result is empty, check the last exception for error</returns>
        public virtual string SendUssd(string ussdCommand)
        {
            return SendUssd(ussdCommand, false);
        }

        /// <summary>
        /// Sends the USSD command
        /// </summary>
        /// <param name="ussdCommand">The ussd command.</param>
        /// <param name="interactive">if set to <c>true</c> [interactive].</param>
        /// <returns></returns>
        public virtual string SendUssd(string ussdCommand, bool interactive)
        {
            try
            {
                UssdFeature feature = GetFeature<UssdFeature>(UssdFeature.NewInstance());
                feature.Content = ussdCommand;
                feature.Interactive = interactive;
                feature.UssdResponseQueue = this.ussdResponseQueue;
                feature.EnableUssdEvent = this.EnableUssdEvent;
                feature.Encoded = this.EncodedUssdCommand;
                IContext context = Execute(feature);
                if (!this.EnableUssdEvent)
                {
                    UssdResponse ussdResponse = (UssdResponse)context.GetResult();
                    return ussdResponse.Content;
                }
                else
                {
                    return context.GetResultString();
                }
            }
            catch (Exception ex)
            {
                this.exception = ex;
            }
            return string.Empty;
        }

        /// <summary>
        /// Sends the USSD command
        /// </summary>
        /// <param name="ussdCommand">The ussd command.</param>
        /// <returns></returns>
        public virtual UssdResponse SendUssd(UssdRequest ussdCommand)
        {
            try
            {
                UssdFeature feature = GetFeature<UssdFeature>(UssdFeature.NewInstance());
                feature.Request = ussdCommand;
                feature.UssdResponseQueue = this.ussdResponseQueue;
                feature.EnableUssdEvent = this.EnableUssdEvent;
                feature.Encoded = this.EncodedUssdCommand;
                IContext context = Execute(feature);
                if (!this.EnableUssdEvent)
                {
                    UssdResponse ussdResponse = (UssdResponse)context.GetResult();
                    return ussdResponse;
                }
                else
                {
                    return new UssdResponse();
                }
            }
            catch (Exception ex)
            {
                this.exception = ex;
            }
            return new UssdResponse();
        }

        /// <summary>
        /// Cancels the USSD session.
        /// </summary>
        /// <returns>true if successful, false otherwise</returns>
        public virtual bool CancelUssdSession()
        {
            CancelUssdSessionFeature feature = GetFeature<CancelUssdSessionFeature>(CancelUssdSessionFeature.NewInstance());
            try
            {
                IContext context = Execute(feature);
                return true;
            }
            catch (Exception ex)
            {
                this.exception = ex;
            }
            return false;
        }


        /// <summary>
        /// Sends the DTMF.
        /// </summary>
        /// <param name="tones">The DTMF tones.</param>
        /// <returns>true if successful, false otherwise</returns>
        public virtual bool SendDtmf(string tones)
        {
            SendDtmfFeature feature = GetFeature<SendDtmfFeature>(SendDtmfFeature.NewInstance());
            try
            {
                feature.Tones = tones;
                IContext context = Execute(feature);
                return true;
            }
            catch (Exception ex)
            {
                this.exception = ex;
            }
            return false;
        }

        /// <summary>
        /// Sends the DTMF.
        /// </summary>
        /// <param name="tones">The DTMF tones.</param>
        /// <param name="duration">The duration in milliseconds. Set to 0 to use the default value.</param>
        /// <returns>true if successful, false otherwise</returns>
        public virtual bool SendDtmf(string tones, int duration)
        {
            SendDtmfFeature feature = GetFeature<SendDtmfFeature>(SendDtmfFeature.NewInstance());
            try
            {
                feature.Tones = tones;
                feature.Duration = duration;
                IContext context = Execute(feature);
                return true;
            }
            catch (Exception ex)
            {
                this.exception = ex;
            }
            return false;
        }

        /// <summary>
        /// Protocol for message sending. It can be either PDU or text mode
        /// </summary>
        /// <param name="protocol">Protocol. See <see cref="MessageProtocol"/></param>
        /// <returns>true if it is set successfully</returns>
        public virtual bool SetMessageProtocol(MessageProtocol protocol)
        {
            if (protocol == MessageProtocol.PduMode)
            {
                try
                {
                    SetPduProtocolFeature feature = GetFeature<SetPduProtocolFeature>(SetPduProtocolFeature.NewInstance());
                    IContext context = Execute(feature);
                    return true;
                }
                catch (Exception ex)
                {
                    this.exception = ex;
                }
                return false;
            }
            else
            {
                try
                {
                    SetTextProtocolFeature feature = GetFeature<SetTextProtocolFeature>(SetTextProtocolFeature.NewInstance());
                    IContext context = Execute(feature);
                    return true;
                }
                catch (Exception ex)
                {
                    this.exception = ex;
                }
                return false;
            }
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
        /// Check if the gateway connection is still valid
        /// </summary>
        /// <exception cref="GatewayException">If the connection is not valid, throw GatewayException</exception>
        public virtual bool ValidateConnection()
        {
            if (!isGatewayConnected)
            {
                return false;
            }

            if (!Connected)
            {
                return false;
            }
            return true;
        }



        /// <summary>
        /// Initializes the data connection.
        /// This is normally called one time for data related service like MMS
        /// </summary>
        /// <returns>
        /// true if the PDP connection already exist or is created successfully. If false is returned, check the last error thrown.
        /// </returns>
        public virtual bool InitializeDataConnection()
        {
            try
            {
                if (string.IsNullOrEmpty(config.DeviceName))
                {
                    throw new GatewayException(Resources.DeviceNameNotProvidedException);
                }
                // Set the PDP IP connection
                SetPdpIpConnectionFeature feature = GetFeature<SetPdpIpConnectionFeature>(SetPdpIpConnectionFeature.NewInstance());
                feature.APN = config.ProviderAPN;
                feature.DataCompressionControl = config.DataCompressionControl;
                feature.HeaderCompressionControl = config.HeaderCompressionControl;
                feature.InternetProtocol = config.InternetProtocol;
                IContext context = Execute(feature);
                PdpConnection pdpConnection = feature.PdpConnection;

                // Activate GPRS
                SetGprsAttachFeature gprsFeature = GetFeature<SetGprsAttachFeature>(SetGprsAttachFeature.NewInstance());
                context = Execute(gprsFeature);

                // Create and get the RAS connection entry name
                SetRasConnectionFeature rasFeature = GetFeature<SetRasConnectionFeature>(SetRasConnectionFeature.NewInstance());
                rasFeature.Configuration = this.config;
                rasFeature.PdpConnection = pdpConnection;
                context = Execute(rasFeature);
                this.RasEntryName = rasFeature.RasEntryName;

                return true;
            }
            catch (Exception ex)
            {
                this.exception = ex;
            }
            return false;
        }


        /// <summary>
        /// Get supported DTMF
        /// </summary>
        /// <returns>Supported DTMF</returns>
        /*
        public virtual char[] GetSupportedDtmfs()
        {
            return null;
        }
        */

        /// <summary>
        /// Send DTMF
        /// </summary>
        /// <param name="dtmfString">DTMF string</param>
        /// <returns>true if successful</returns>
        /*
        public virtual bool SendDtmf(string dtmfString)
        {
            return true;
        }
        */


        #endregion ======================================================================================


        #region =========== Protected Methods ===========================================================

        /// <summary>
        /// Set up the feature to be used
        /// </summary>
        /// <param name="feature">Feature</param>
        /// <returns>The feature to be run</returns>
        protected F GetFeature<F>(F feature) where F : IFeature
        {
            feature.Port = Port;
            feature.IncomingDataQueue = incomingDataQueue;

            // Set the common properties
            feature.WaitForResponseInterval = config.CommandWaitInterval;
            feature.WaitForResponseRetryCount = config.CommandWaitRetryCount;

            return feature;
        }

        /// <summary>
        /// Execute the feature
        /// </summary>
        /// <param name="feature">Feature</param>
        /// <returns>Execution result</returns>
        protected IContext Execute(IFeature feature)
        {
            lock (CommandSyncLock)
            {
                // Validate the connection
                if (feature.CommandType == FeatureCommandType.AT)
                {
                    if (!ValidateConnection())
                    {
                        // Try if reconnect
                        Disconnect();
                        if (!Connect())
                            throw new GatewayException(Resources.NotConnectedException, this.LastError);
                    }
                }
                else if (feature.CommandType == FeatureCommandType.Data)
                {
                    // Do nothing at this moment
                    //Disable();
                }

                if (Port != null)
                {
                    Port.DiscardInBuffer();
                    Port.DiscardOutBuffer();
                }

                try
                {
                    IContext context = delegateEngine.Run(feature);
                    LastExecution = DateTime.Now;
                    return context;
                }
                catch (Exception ex)
                {
                    // Check if a reset is required
                    if (feature.RequiredReset && config.ResetGatewayAfterTimeout)
                    {
                        // Disconnect the gateway and reconnect     
                        Logger.LogThis("Timeout occurred. Resetting the gateway", LogLevel.Info);
                        Disconnect();
                        if (!Connect())
                            throw new GatewayException(Resources.NotConnectedException, this.LastError);
                    }

                    // If in debug mode, display the dialog
                    if (config.DebugMode)
                    {
                        MessageBox.Show(string.Format("Message: {0}\nStack trace: {1}", ex.Message, ex.StackTrace), Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                    throw ex;
                }
            }
        }

        /// <summary>
        /// Retrieve the SIM card status. Status can be 
        /// 1. PIN required
        /// 2. Busy
        /// 3. Ready
        /// </summary>
        /// <returns></returns>
        protected Response GetSimStatus()
        {
            // Create the feature      
            GetSimStatusFeature feature = GetFeature<GetSimStatusFeature>(GetSimStatusFeature.NewInstance());
            try
            {
                IContext context = Execute(feature);
                return (Response)context.GetResult();
            }
            catch (Exception ex)
            {
                this.exception = ex;
            }
            // Default to ready
            return Response.Ready;
        }

        /// <summary>
        /// Get supported message indication
        /// </summary>
        /// <returns>Supported message indication settings</returns>
        protected MessageIndicationSupport GetSupportedMessageIndications()
        {
            if (messageIndicationSupport != null)
            {
                return messageIndicationSupport;
            }
            else
            {
                // Query the setting from gateway                
                GetSupportedMessageIndicationFeature feature = GetFeature<GetSupportedMessageIndicationFeature>(GetSupportedMessageIndicationFeature.NewInstance());
                try
                {
                    IContext context = Execute(feature);
                    messageIndicationSupport = (MessageIndicationSupport)context.GetResult();
                    return messageIndicationSupport;
                }
                catch (Exception ex)
                {
                    this.exception = ex;
                }

                return new MessageIndicationSupport();
            }
        }

        /// <summary>
        /// Set message indications
        /// </summary>
        /// <param name="setting"></param>
        protected void SetMessageIndications(MessageIndicationSettings setting)
        {
            // Query the setting from gateway                
            SetMessageIndicationsFeature feature = GetFeature<SetMessageIndicationsFeature>(SetMessageIndicationsFeature.NewInstance());
            feature.MessageIndicationSettings = setting;
            IContext context = Execute(feature);
        }

        /// <summary>
        /// Resets the message indications.
        /// </summary>
        /// <param name="setting">The setting.</param>
        /// <returns></returns>
        protected bool ResetMessageIndications(MessageIndicationSettings setting)
        {
            try
            {
                // Query the setting from gateway                
                SetMessageIndicationsFeature feature = GetFeature<SetMessageIndicationsFeature>(SetMessageIndicationsFeature.NewInstance());
                feature.MessageIndicationSettings = setting;
                IContext context = Execute(feature);
                return true;
            }
            catch (Exception ex)
            {
                this.exception = ex;
            }
            return false;
        }

        /// <summary>
        /// Set message batch mode
        /// </summary>
        protected void SetMessageBatchMode()
        {
            if (BatchMessageMode == Mobile.BatchMessageMode.NotSupported) return;

            BatchSmsFeature feature = GetFeature<BatchSmsFeature>(BatchSmsFeature.NewInstance());
            feature.BatchMessageMode = BatchMessageMode;
            try
            {
                IContext context = Execute(feature);
            }
            catch (Exception ex)
            {
                this.exception = ex;
            }

        }

        /// <summary>
        /// Echo off
        /// </summary>
        protected virtual void EchoOff()
        {
            ExecuteFeature feature = GetFeature<ExecuteFeature>(ExecuteFeature.NewInstance());
            IContext context;
            // Set to no echo           
            feature.Request = NoEchoCommand;
            context = Execute(feature);
        }

        /// <summary>
        /// Initialize up the gateway
        /// </summary>
        protected virtual bool Synchronize()
        {
            ExecuteFeature feature = GetFeature<ExecuteFeature>(ExecuteFeature.NewInstance());
            IContext context;

            // Set the profile to stored profile 0            
            feature.Request = ResetProfileCommand;
            context = Execute(feature);
            string response = context.GetResultString();
            if (string.IsNullOrEmpty(response)) return false;
            Regex expectedResponse = new Regex(StringEnum.GetStringValue(Response.Ok));
            if (!expectedResponse.IsMatch(response)) return false;
            return true;
        }

        /// <summary>
        /// Initialize up the gateway
        /// </summary>
        protected virtual void Reset()
        {
        }

        /// <summary>
        /// Initialize up the gateway
        /// </summary>
        protected virtual void Initialize()
        {
            if (!config.SkipOperatorSelection)
            {
                ExecuteFeature feature = GetFeature<ExecuteFeature>(ExecuteFeature.NewInstance());
                IContext context;

                // Default operator selection mode            
                feature.Request = string.Format(DefaultOperatorSelectionCommand, (int)config.OperatorSelectionFormat);
                context = Execute(feature);
            }
        }


        /// <summary>
        /// Retrieve supported message storage information for reading, writing,
        /// and maybe receiving messages.
        /// Storage can be ME (mobile equipment), SM (SIM memory)
        /// </summary>
        /// <returns></returns>
        protected virtual MessageStorageInfo GetMessageStorageInfo()
        {
            GetMessageStorageFeature feature = GetFeature<GetMessageStorageFeature>(GetMessageStorageFeature.NewInstance());
            try
            {
                IContext context = Execute(feature);
                return (MessageStorageInfo)context.GetResult();
            }
            catch (Exception ex)
            {
                this.exception = ex;
            }
            MessageStorageInfo emptyStorageInfo = new MessageStorageInfo();
            emptyStorageInfo.ReadStorages = new string[0];
            emptyStorageInfo.WriteStorages = new string[0];
            emptyStorageInfo.ReceiveStorages = new string[0];
            return emptyStorageInfo;
        }


        /// <summary>
        /// Method to call during connection setup
        /// </summary>
        protected abstract void OnConnected();

        /// <summary>
        /// Method to call after connection is disconnected
        /// </summary>
        protected abstract void OnDisconnected();

        #endregion =========================================================================


        #region =========== Private Events  ================================================

        /// <summary>
        /// Port data received event
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">Data received</param>
        private void Port_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            if (e.EventType == SerialData.Chars)
            {
                lock (ReaderSyncLock)
                {
                    Monitor.PulseAll(ReaderSyncLock);
                }
            }
        }

        #endregion =========================================================================

        #region =========== Private Methods ================================================


        /// <summary>
        /// Read data from gateway
        /// </summary>
        private void InBoundDataReader()
        {
            MessageIndicationHandlers messageIndicationHandlers = new MessageIndicationHandlers();
            CallIndicationHandlers callIndicationHandlers = new CallIndicationHandlers();
            string data = string.Empty;
            while (Connected)
            {
                try
                {
                    lock (ReaderSyncLock)
                    {
                        if (Port.BytesToRead <= 0) Monitor.Wait(ReaderSyncLock);
                        if (!Connected) break;
                        string portData = Port.ReadExisting();
                        //Logger.LogThis("Port data: " + portData, LogLevel.Verbose, this.Id);
                        data += portData;
                        if (!string.IsNullOrEmpty(data))
                        {
                            //Logger.LogThis("Received: " + data, LogLevel.Verbose, this.Id);

                            // If too short and not ends with \n, and it is an indication then continue
                            // Modified Oct 15 2013
                            //if (data.Trim().StartsWith("+") && data.Length <= MinimumInboundDataLength && !data.EndsWith("\n")) continue;
                            if (data.Trim().StartsWith("+") && !data.EndsWith("\n")) continue;

                            // Check if it is an unsolicited message
                            bool isIncompleteMessage = false;
                            bool isIncompleteCall = false;
                            string unhandledData = string.Empty;
                            if (CheckUnsolicitedMessage(messageIndicationHandlers, ref data, ref isIncompleteMessage, ref unhandledData))
                            {
                                //Logger.LogThis("The incoming message indication is processed.", LogLevel.Verbose, this.Id);
                                //Logger.LogThis("Data is : " + data, LogLevel.Verbose, this.Id);
                                if (!string.IsNullOrEmpty(data) && !isIncompleteMessage)
                                {
                                    // Need to add into incoming data queue
                                    EnqueueData(ref data);
                                }
                                if (!string.IsNullOrEmpty(unhandledData))
                                {
                                    // Need to add into incoming data queue
                                    EnqueueData(ref unhandledData);
                                }
                            }
                            else if (CheckUnsolicitedCall(callIndicationHandlers, ref data, ref isIncompleteCall, ref unhandledData))
                            {
                                //Logger.LogThis("The incoming call indication is processed.", LogLevel.Verbose, this.Id);
                                if (!string.IsNullOrEmpty(data) && !isIncompleteCall)
                                {
                                    // Need to add into incoming data queue
                                    EnqueueData(ref data);
                                }
                                if (!string.IsNullOrEmpty(unhandledData))
                                {
                                    // Need to add into incoming data queue
                                    EnqueueData(ref unhandledData);
                                }
                            }
                            else
                            {
                                if (!string.IsNullOrEmpty(data))
                                    EnqueueData(ref data);
                            }
                        }
                    }
                }
                catch (ThreadInterruptedException tiEx)
                {
                    if (!Connected) break;
                }
                catch (ThreadAbortException taEx)
                {
                    if (!Connected) break;
                    //break;
                }
                catch (Exception e)
                {
                    // Log the error
                    Logger.LogThis("Reading incoming data: " + e.Message, LogLevel.Error, this.Id);
                    Logger.LogThis("Data: " + data, LogLevel.Error, this.Id);
                    Logger.LogThis("Stack trace: " + e.StackTrace, LogLevel.Error, this.Id);

                    // Clear the queue


                    if (!Connected) break;
                }
            }
        }

        /// <summary>
        /// Encode the data
        /// </summary>
        /// <param name="data">Data to be enqueued</param>
        private void EnqueueData(ref string data)
        {
            string[] lines = data.Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            bool appendCrLf = false;
            if (data.EndsWith("\r\n")) appendCrLf = true;
            if (lines.Length > 1)
            {
                // Multiple lines received
                int lineCount = 0;
                foreach (string line in lines)
                {
                    lineCount++;
                    if (!string.IsNullOrEmpty(line))
                    {
                        string input = line;
                        if (lineCount != lines.Length)
                        {
                            incomingDataQueue.Enqueue(input + "\r\n");
                        }
                        else
                        {
                            if (appendCrLf)
                                incomingDataQueue.Enqueue(input + "\r\n");
                            else
                                incomingDataQueue.Enqueue(input);
                        }
                    }
                    else
                    {
                        /*
                        string trimText = line.Trim();
                        if (string.IsNullOrEmpty(trimText)) continue; 
                        */
                        if (lineCount != lines.Length)
                        {
                            incomingDataQueue.Enqueue("\r\n");
                        }
                    }
                }
            }
            else
            {
                incomingDataQueue.Enqueue(data);
            }
            data = string.Empty;
        }

        /// <summary>
        /// Check for unsolicited call
        /// </summary>
        /// <param name="handlers">Unsolicited call handlers</param>
        /// <param name="input">The input.</param>
        /// <param name="isIncompleteCall">if set to <c>true</c> [is incomplete call].</param>
        /// <param name="unhandledData">The unhandled data.</param>
        /// <returns></returns>
        private bool CheckUnsolicitedCall(CallIndicationHandlers handlers, ref string input, ref bool isIncompleteCall, ref string unhandledData)
        {
            string[] lines = input.Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
            bool appendCrLf = false;
            bool prefixCrLf = false;

            if (input.EndsWith("\r\n")) appendCrLf = true;
            if (input.StartsWith("\r\n")) prefixCrLf = true;

            string data = string.Empty;
            List<string> rawData = new List<string>();
            bool containsUnsolicitedCall = false;

            int lineCount = 0;
            foreach (string line in lines)
            {
                lineCount++;
                if (!string.IsNullOrEmpty(line))
                {
                    data += line;
                    if (handlers.IsUnsolicitedCall(data))
                    {
                        // Is unsolicited call, raise the call received event
                        string description;
                        string call = data;

                        IIndicationObject callInformation =
                            handlers.HandleUnsolicitedCall(ref call, out description);

                        Logger.LogThis("Unsolicited call: " + data, LogLevel.Verbose, this.Id);
                        Logger.LogThis(description, LogLevel.Verbose, this.Id);

                        // Raise event
                        ProcessCallReceived(callInformation);

                        // Reset
                        data = string.Empty;

                        isIncompleteCall = false;
                        containsUnsolicitedCall = true;
                    }
                    else if (handlers.IsIncompleteUnsolicitedCall(data))
                    {
                        // Need to check for next line
                        if (lineCount != lines.Length)
                        {
                            data += "\r\n";
                        }
                        else
                        {
                            if (prefixCrLf)
                                data = "\r\n" + data;

                            if (appendCrLf)
                                data += "\r\n";
                        }
                        Logger.LogThis("Incomplete unsolicited call: " + data, LogLevel.Verbose, this.Id);
                        containsUnsolicitedCall = true;
                        isIncompleteCall = true;
                    }
                    else
                    {
                        // Need to check for next line
                        if (lineCount != lines.Length)
                        {
                            if (lineCount == 1 && prefixCrLf)
                                rawData.Add("\r\n");

                            rawData.Add(data + "\r\n");
                        }
                        else
                        {
                            if (prefixCrLf && lines.Length == 1)
                                rawData.Add("\r\n");

                            if (appendCrLf)
                                rawData.Add(data + "\r\n");
                            else
                                rawData.Add(data);
                        }
                        data = string.Empty;
                    }
                }
            }

            if (!containsUnsolicitedCall) return false;
            if (isIncompleteCall)
            {
                foreach (string line in rawData)
                {
                    unhandledData += line;
                }
                return containsUnsolicitedCall;
            }

            if (!string.IsNullOrEmpty(data))
            {
                string[] unprocessedList = data.Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
                appendCrLf = false;
                prefixCrLf = false;

                if (data.StartsWith("\r\n")) prefixCrLf = true;
                if (data.EndsWith("\r\n")) appendCrLf = true;

                lineCount = 0;
                foreach (string line in unprocessedList)
                {
                    lineCount++;
                    if (!string.IsNullOrEmpty(line))
                    {
                        if (lineCount != lines.Length)
                        {
                            if (lineCount == 1 && prefixCrLf)
                                rawData.Add("\r\n");

                            rawData.Add(line + "\r\n");
                        }
                        else
                        {
                            if (prefixCrLf && lines.Length == 1)
                                rawData.Add("\r\n");

                            if (appendCrLf)
                                rawData.Add(line + "\r\n");
                            else
                                rawData.Add(line);
                        }
                    }
                }
                input = string.Empty;
                foreach (string line in rawData)
                {
                    input += line;
                }
            }
            else
            {
                input = string.Empty;
                foreach (string line in rawData)
                {
                    unhandledData += line;
                }
            }

            return containsUnsolicitedCall;
        }

        /// <summary>
        /// Check for unsolicited message
        /// </summary>
        /// <param name="handlers">Unsolicited message handlers</param>
        /// <param name="input">The input.</param>
        /// <param name="isInCompletedMessage">if set to <c>true</c> [is in completed message].</param>
        /// <param name="unhandledData">The unhandled data.</param>
        /// <returns></returns>
        private bool CheckUnsolicitedMessage(MessageIndicationHandlers handlers, ref string input, ref bool isInCompletedMessage, ref string unhandledData)
        {
            string[] lines = input.Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
            bool appendCrLf = false;
            bool prefixCrLf = false;

            if (input.EndsWith("\r\n")) appendCrLf = true;
            if (input.StartsWith("\r\n")) prefixCrLf = true;

            string data = string.Empty;
            List<string> rawData = new List<string>();
            bool containsUnsolicitedMessage = false;
            int lineCount = 0;

            foreach (string line in lines)
            {
                lineCount++;
                if (!string.IsNullOrEmpty(line))
                {
                    data += line;
                    if (handlers.IsUnsolicitedMessage(data))
                    {
                        // Is unsolicited message, raise the message received event
                        string description;
                        string message = data;

                        IIndicationObject messageIndicationObject =
                            handlers.HandleUnsolicitedMessage(ref message, out description);

                        Logger.LogThis("Unsolicited message: " + data, LogLevel.Verbose, this.Id);
                        Logger.LogThis(description, LogLevel.Verbose, this.Id);

                        // Raise event
                        ProcessMessageReceived(messageIndicationObject);

                        // Reset
                        data = string.Empty;

                        isInCompletedMessage = false;
                        containsUnsolicitedMessage = true;
                    }
                    else if (handlers.IsIncompleteUnsolicitedMessage(data))
                    {
                        // Need to check for next line
                        if (lineCount != lines.Length)
                        {
                            data += "\r\n";
                        }
                        else
                        {
                            if (prefixCrLf)
                                data = "\r\n" + data;

                            if (appendCrLf)
                                data += "\r\n";
                        }
                        Logger.LogThis("Incomplete unsolicited message: " + data, LogLevel.Verbose, this.Id);
                        containsUnsolicitedMessage = true;
                        isInCompletedMessage = true;
                    }
                    else
                    {
                        // Need to check for next line
                        if (lineCount != lines.Length)
                        {
                            if (lineCount == 1 && prefixCrLf)
                                rawData.Add("\r\n");

                            rawData.Add(data + "\r\n");
                        }
                        else
                        {
                            if (prefixCrLf && lines.Length == 1)
                                rawData.Add("\r\n");

                            if (appendCrLf)
                                rawData.Add(data + "\r\n");
                            else
                                rawData.Add(data);
                        }
                        data = string.Empty;
                    }
                }
            }

            if (!containsUnsolicitedMessage) return false;
            if (isInCompletedMessage)
            {
                foreach (string line in rawData)
                {
                    unhandledData += line;
                }
                return containsUnsolicitedMessage;
            }

            if (!string.IsNullOrEmpty(data))
            {
                string[] unprocessedList = data.Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
                appendCrLf = false;
                prefixCrLf = false;
                if (data.StartsWith("\r\n")) prefixCrLf = true;
                if (data.EndsWith("\r\n")) appendCrLf = true;
                lineCount = 0;
                foreach (string line in unprocessedList)
                {
                    lineCount++;
                    if (!string.IsNullOrEmpty(line))
                    {
                        if (lineCount != lines.Length)
                        {
                            if (lineCount == 1 && prefixCrLf)
                                rawData.Add("\r\n");

                            rawData.Add(line + "\r\n");
                        }
                        else
                        {
                            if (prefixCrLf && lines.Length == 1)
                                rawData.Add("\r\n");

                            if (appendCrLf)
                                rawData.Add(line + "\r\n");
                            else
                                rawData.Add(line);
                        }
                    }
                }
                input = string.Empty;
                foreach (string line in rawData)
                {
                    input += line;
                }
            }
            else
            {
                input = string.Empty;
                foreach (string line in rawData)
                {
                    unhandledData += line;
                }
            }

            return containsUnsolicitedMessage;
        }


        /// <summary>
        /// Watch dog to check the connection status
        /// </summary>
        private void WatchDog()
        {
            while (Connected)
            {
                try
                {
                    Thread.Sleep(config.WatchDogInterval);
                    if (!Connected) break;

                    // If there are command executed previously then skip the check
                    TimeSpan timespan = DateTime.Now - LastExecution;
                    if (timespan.TotalMilliseconds < config.WatchDogInterval) continue;
                    Logger.LogThis("Sending watch dog message", LogLevel.Verbose, this.Id);
                    WatchDogFeature feature = GetFeature<WatchDogFeature>(WatchDogFeature.NewInstance());
                    IContext context = Execute(feature);
                    if (string.IsNullOrEmpty(context.GetResultString()))
                    {
                        isGatewayConnected = false;
                        Status = GatewayStatus.Restart;

                        // 30 Jan 2012
                        OnWatchDogFailed();

                        // Raise the event
                        //OnGatewayDisconnected();
                    }
                    else
                    {
                        isGatewayConnected = true;
                        Status = GatewayStatus.Started;

                        // Check gateway status
                        CheckGatewayStatus(feature);
                    }
                }
                catch (ThreadInterruptedException tiEx)
                {
                    if (!Connected)
                    {
                        //break;
                        isGatewayConnected = false;
                        Status = GatewayStatus.Restart;

                        // Raise the event
                        //OnGatewayDisconnected();

                    }
                }
                catch (ThreadAbortException taEx)
                {
                    //if (!Connected)
                    //{                       
                    isGatewayConnected = false;
                    Status = GatewayStatus.Restart;

                    // Raise the event
                    //OnGatewayDisconnected();

                    break;
                    //}
                }
                catch (Exception e)
                {
                    //break;                  
                    isGatewayConnected = false;
                    Status = GatewayStatus.Restart;

                    // Raise the event
                    this.exception = e;
                    OnWatchDogFailed();
                }
            }
            if (!Connected)
            {
                isGatewayConnected = false;
                Status = GatewayStatus.Restart;
            }
        }


        /// <summary>
        /// Process message from message queue
        /// </summary>
        private void ProcessMessageQueue()
        {
            IMessage message = null;
            while (true)
            {
                try
                {
                    lock (messageQueueLock)
                    {
                        if (messageQueue.Count == 0 || !IsMessageQueueEnabled)
                            Monitor.Wait(messageQueueLock);
                    }

                    //if (!Connected) break;

                    // Set message sending mode
                    //SetMessageBatchMode();
                    bool isMessageModeSet = false;

                    while (messageQueue.Count > 0 && IsMessageQueueEnabled)
                    {
                        // Modified Oct 11 2014
                        //PriorityQueueItem<IMessage, MessageQueuePriority> item = messageQueue.Dequeue();
                        PriorityQueueItem<IMessage, MessageQueuePriority> item = messageQueue.Peek();
                        
                        message = item.Value;

                        // Raise message sending event
                        OnMessageSending(message);

                        // Try sending the message
                        for (int i = 1; i <= config.SendRetries; i++)
                        {
                            //bool isProcessed = false;
                            try
                            {
                                if (message is Mms)
                                {
                                    Mms mms = (Mms)message;
                                    SendMms(mms);   
                                    isMessageModeSet = false;
                                }
                                else
                                {
                                    if (!isMessageModeSet)
                                    {
                                        // Set message sending mode
                                        SetMessageBatchMode();
                                        isMessageModeSet = true;
                                    }
                                    Sms sms = (Sms)message;
                                    SendMessage(sms);
                                }

                                // Raise message sent event                                                                       
                                OnMessageSent(message);

                                // Remove from queue - commented Oct 11 2014
                                //messageQueue.Dequeue();
                                //isProcessed = true;
                                messageQueue.Dequeue(item);

                                if (messageQueue.Count > 0)
                                {
                                    // Sleep 200 ms before next send
                                    Thread.Sleep(config.SendWaitInterval);
                                }

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
                                    // Raise message sent event
                                    OnMessageSendingFailed(message, e);

                                    // Remove from queue - commented Oct 11 2014
                                    // messageQueue.Dequeue();
                                    //isProcessed = true;
                                    messageQueue.Dequeue(item);

                                }
                                // Sleep for longer time upon exception
                                Thread.Sleep(1000);
                            }
                        }
                    }
                    //} //lock
                }
                catch (ThreadInterruptedException tiEx)
                {
                    //if (!Connected) break;
                    break;
                }
                catch (ThreadAbortException taEx)
                {
                    //if (!Connected) break;
                    break;
                }
                catch (Exception e)
                {
                    // Log it  
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
        /// Send out the message
        /// </summary>
        /// <param name="message">Message instance</param>
        private void SendMessage(Sms message)
        {
            try
            {
                Logger.LogThis("Sending message to " + message.DestinationAddress, LogLevel.Verbose, this.Id);
                Logger.LogThis("Content: " + message.Content, LogLevel.Verbose, this.Id);

                if (string.IsNullOrEmpty(message.GatewayId))
                {
                    message.GatewayId = this.Id;
                }

                // For testing - Oct 11 2014
                //if (!string.IsNullOrEmpty(message.Content))
                //{
                //    return;
                //}

                if (!license.Valid)
                {
                    if (this.statistics.OutgoingSms >= UnlicensedMaximumSms)
                        throw new GatewayException(Resources.LicenseException);

                    if (message.GetType() == typeof(Sms))
                    {
                        if (!message.Content.StartsWith(Resources.UnlicensedMessagePrefix))
                        {
                            // Prefix the message
                            message.Content = Resources.UnlicensedMessagePrefix + message.Content;
                        }
                    }
                }

                // If service center address is not set, then try to get from the gateway
                if (string.IsNullOrEmpty(message.ServiceCenterNumber))
                {
                    message.ServiceCenterNumber = ServiceCentreAddress.Number;
                }

                if (message.RawMessage)
                {
                    SendRawSmsFeature feature = GetFeature<SendRawSmsFeature>(SendRawSmsFeature.NewInstance());
                    feature.Message = message;
                    IContext context = Execute(feature);

                    //if (message.ReferenceNo != null && message.ReferenceNo.Count == 0)
                    message.Indexes = (List<int>)context.GetResult();

                }
                else if (message.SaveSentMessage)
                {
                    SendSmsFromStorageFeature feature = GetFeature<SendSmsFromStorageFeature>(SendSmsFromStorageFeature.NewInstance());
                    feature.Message = message;
                    IContext context = Execute(feature);
                    //if (message.ReferenceNo != null && message.ReferenceNo.Count == 0)
                    message.Indexes = (List<int>)context.GetResult();
                }
                else
                {
                    SendSmsFeature feature = GetFeature<SendSmsFeature>(SendSmsFeature.NewInstance());
                    feature.Message = message;
                    IContext context = Execute(feature);
                    //if (message.ReferenceNo != null && message.ReferenceNo.Count == 0)
                    message.Indexes = (List<int>)context.GetResult();
                }

                // Increase the statistics counter
                if (message.Indexes != null && message.Indexes.Count > 0)
                {
                    statistics.OutgoingSms++;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        /// <summary>
        /// Sends the MMS.
        /// </summary>
        /// <param name="message">The MMS message.</param>
        private void SendMms(Mms message)
        {
            try
            {
                if (Logger.LogLevel == LogLevel.Verbose)
                {
                    foreach (MultimediaMessageAddress address in message.To)
                    {
                        Logger.LogThis("To: " + address.ToString(), LogLevel.Verbose, this.Id);
                    }
                    Logger.LogThis("Subject: " + message.Subject, LogLevel.Verbose, this.Id);
                }

                if (string.IsNullOrEmpty(message.GatewayId))
                {
                    message.GatewayId = this.Id;
                }

                if (!license.Valid)
                {
                    if (this.statistics.OutgoingMms >= UnlicensedMaximumMms)
                        throw new GatewayException(Resources.LicenseException);
                }

                SendMmsFeature feature = GetFeature<SendMmsFeature>(SendMmsFeature.NewInstance());
                feature.Message = message;
                feature.Configuration = this.config;
                feature.Gateway = (IMobileGateway)this;
                feature.RasEntryName = this.RasEntryName;
                feature.UserAgent = this.config.UserAgent;
                feature.XWAPProfile = this.config.XWAPProfile;
                feature.ConnectionTimeout = this.config.DataConnectionTimeout;
                IContext context = Execute(feature);

                // Increase the statistics counter
                if (!string.IsNullOrEmpty(message.MessageId))
                {
                    statistics.OutgoingMms++;
                }
                else
                {
                    throw new GatewayException(Resources.NoMessageIDException);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        /// <summary>
        /// Sends the MMS.
        /// </summary>
        /// <param name="messageInformation">The message information.</param>
        /// <param name="message">The MMS message.</param>
        /// <returns>true if MMS can be retrieved, otherwise return false</returns>
        private bool RetrieveMms(MessageInformation messageInformation, ref Mms message)
        {
            try
            {
                RetrieveMmsFeature feature = GetFeature<RetrieveMmsFeature>(RetrieveMmsFeature.NewInstance());
                feature.MessageInformation = messageInformation;
                feature.Configuration = this.config;
                feature.Gateway = (IMobileGateway)this;
                feature.RasEntryName = this.RasEntryName;
                feature.UserAgent = this.config.UserAgent;
                feature.XWAPProfile = this.config.XWAPProfile;
                feature.ConnectionTimeout = this.config.DataConnectionTimeout;
                IContext context = Execute(feature);
                message = feature.Message;
                // Increase the statistics counter
                if (!string.IsNullOrEmpty(message.MessageId))
                {
                    statistics.IncomingMms++;

                    if (string.IsNullOrEmpty(message.GatewayId))
                    {
                        message.GatewayId = this.Id;
                    }

                    return true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return false;
        }

        /// <summary>
        /// Wait for network registration successfully
        /// </summary>
        private void WaitForNetworkRegistration()
        {
            while (true)
            {
                NetworkRegistration status = this.NetworkRegistration;

                if (status.Status == NetworkRegistrationStatus.RegisteredHomeNetwork ||
                    status.Status == NetworkRegistrationStatus.RegisteredRoamingNetwork)
                {
                    break;
                }
                else if (status.Status == NetworkRegistrationStatus.SearchingNewOperator)
                {
                    // Set to null so that query from gateway again
                    networkRegistration = null;
                }
                else
                {
                    throw new GatewayException(Resources.NotRegisteredToNetworkException);
                }
                // Sleep for 5 seconds before checking again
                Thread.Sleep(5000);
            }
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
        /// Message received.
        /// </summary>
        /// <param name="sender">Sender object</param>
        /// <param name="messageIndicationObject">Message indication object</param>
        private void OnRawMessageReceived(object sender, IIndicationObject messageIndicationObject)
        {
            // Fix USSD event not triggered error - 1 Nov 2011
            if (messageIndicationObject is UssdResponse)
            {
                ProcessUssdResponse((UssdResponse)messageIndicationObject);
                return;
            }

            //this.statistics.IncomingSms++;  // Increase the counter
            //if (this.MessageReceived != null)
            //{

            bool requiredToReset = false;
            MessageStorage currentStorage = this.MessageStorage;
            try
            {
                //this.isMessageIndicationEnabled = true;
                if (messageIndicationObject is MemoryLocation)
                {
                    string currentStorageValue = StringEnum.GetStringValue(currentStorage);
                    MemoryLocation memoryLocation = (MemoryLocation)messageIndicationObject;
                    MessageStorage newMessageStorage = (MessageStorage)StringEnum.Parse(typeof(MessageStorage), memoryLocation.Storage);
                    string newStorageLocation = StringEnum.GetStringValue(newMessageStorage);
                    if (!currentStorageValue.Equals(newStorageLocation, StringComparison.OrdinalIgnoreCase))
                    {
                        this.MessageStorage = newMessageStorage;
                        Logger.LogThis(string.Format("Current storage location [{0}]. New storage location [{1}]", currentStorageValue, newStorageLocation), LogLevel.Verbose, this.Id);
                        requiredToReset = true;
                    }
                    MemoryLocation msgLocation = messageIndicationObject as MemoryLocation;
                    if (msgLocation.NotificationType == MessageNotification.StatusReport)
                    {
                        CheckInboundStatusReport(msgLocation);
                    }
                    else
                    {
                        CheckInboundMessages();
                    }
                }
                else if (messageIndicationObject is ReceivedMessage)
                {
                    //if (!this.isMessageIndicationEnabled && !this.PollNewMessages) return;

                    MessageInformation message = null;
                    ReceivedMessage receivedMessage = (ReceivedMessage)messageIndicationObject;
                    PduFactory pduFactory = PduFactory.NewInstance();
                    message = pduFactory.Decode(receivedMessage.Data);
                    message.MessageStatusType = MessageStatusType.ReceivedUnreadMessages;
                    message.Index = -1; // No index here      

                    message.GatewayId = this.Id;

                    if (!license.Valid)
                    {
                        if (!string.IsNullOrEmpty(message.Content) && !message.Content.StartsWith(Resources.UnlicensedMessagePrefix))
                        {
                            message.Content = Resources.UnlicensedMessagePrefix + message.Content;
                        }
                    }

                    if (this.MessageReceived != null)
                    {
                        MessageReceivedEventArgs e = new MessageReceivedEventArgs(message);
                        this.MessageReceived.BeginInvoke(this, e, new AsyncCallback(this.AsyncCallback), null);
                    }
                    else
                    {
                        Logger.LogThis("Warning: No event handlers for MessageReceived event, message is ignored.", LogLevel.Verbose, this.Id);
                    }

                    this.statistics.IncomingSms++;
                }
                else
                {
                    Logger.LogThis("Unknown indication type", LogLevel.Verbose, this.Id);
                    return;
                }
            }
            catch (Exception ex)
            {
                Logger.LogThis("Error processing received message: " + ex.Message, LogLevel.Info, this.Id);
            }
            finally
            {
                if (requiredToReset)
                {
                    this.MessageStorage = currentStorage;
                }
            }

            /** Commented 19 Jan 2012
            try
            {
                MessageInformation message = null;
                if (messageIndicationObject is MemoryLocation)
                {
                    bool requiredToReset = false;
                    MessageStorage currentStorage = this.MessageStorage;
                    string currentStorageValue = StringEnum.GetStringValue(currentStorage);
                    try
                    {
                        MemoryLocation memoryLocation = (MemoryLocation)messageIndicationObject;
                        this.MessageStorage = (MessageStorage)StringEnum.Parse(typeof(MessageStorage), memoryLocation.Storage);
                        string newStorageLocation = StringEnum.GetStringValue(this.MessageStorage);
                        if (!currentStorageValue.Equals(newStorageLocation, StringComparison.OrdinalIgnoreCase))
                        {
                            Logger.LogThis(string.Format("Current storage location [{0}]. New storage location [{1}]", currentStorageValue, newStorageLocation), LogLevel.Verbose, this.Id);
                            requiredToReset = true;
                        }
                        message = GetMessage(memoryLocation.Index);
                        message.MessageStatusType = MessageStatusType.ReceivedUnreadMessages;
                        message.Index = memoryLocation.Index;
                        //message.Indexes.Add(memoryLocation.Index);
                    }
                    finally
                    {
                        if (requiredToReset) 
                        {
                            this.MessageStorage = currentStorage;
                        }
                    }
                }
                else if (messageIndicationObject is ReceivedMessage)
                {
                    ReceivedMessage receivedMessage = (ReceivedMessage)messageIndicationObject;
                    PduFactory pduFactory = PduFactory.NewInstance();
                    message = pduFactory.Decode(receivedMessage.Data);
                    message.MessageStatusType = MessageStatusType.ReceivedUnreadMessages;
                    message.Index = -1; // No index here                        
                }                  
                else
                {
                    Logger.LogThis("Unknown indication type", LogLevel.Verbose, this.Id);
                    return;
                }
                message.GatewayId = this.Id;

                if (!license.Valid)
                {                        
                    if (!string.IsNullOrEmpty(message.Content) && !message.Content.StartsWith(Resources.UnlicensedMessagePrefix))
                    {
                        message.Content = Resources.UnlicensedMessagePrefix + message.Content;
                    }
                }
                  
                MessageReceivedEventArgs e = new MessageReceivedEventArgs(message);
                this.MessageReceived.BeginInvoke(this, e, new AsyncCallback(this.AsyncCallback), null);

                // Check if need to delete the message automatically
                if (config.DeleteReceivedMessage && message != null && message.Index > 0)
                {
                    //this.DeleteMessage(MessageDeleteOption.ByIndex, message.Index);
                    foreach (int index in message.Indexes)
                    {
                        Logger.LogThis("Delete message with index " + index, LogLevel.Verbose);
                        this.DeleteMessage(MessageDeleteOption.ByIndex, index);
                    }
                }                    
            }
            catch (Exception ex)
            {
                Logger.LogThis("Error processing received message: " + ex.Message, LogLevel.Info, this.Id);
            }

            **/
            //}
            //else
            //{
            //Logger.LogThis("Warning: No event handlers for MessageReceived event, message is ignored.", LogLevel.Verbose, this.Id);
            //} 

        }

        /// <summary>
        /// Processes the ussd response.
        /// </summary>
        /// <param name="ussdResponse">The ussd response.</param>
        private void ProcessUssdResponse(UssdResponse ussdResponse)
        {
            ussdResponse.GatewayId = this.Id;

            if (this.EncodedUssdCommand)
            {
                try
                {
                    byte[] responseEncodedSeptets = MessagingToolkit.Pdu.PduUtils.PduToBytes(ussdResponse.Content);
                    byte[] responseUnencodedSeptets = MessagingToolkit.Pdu.PduUtils.EncodedSeptetsToUnencodedSeptets(responseEncodedSeptets);
                    ussdResponse.Content = MessagingToolkit.Pdu.PduUtils.UnencodedSeptetsToString(responseUnencodedSeptets);
                }
                catch (Exception ex)
                {
                    Logger.LogThis("Error decoding USSD response.", LogLevel.Error, this.Id);
                    Logger.LogThis(ex.ToString(), LogLevel.Error, this.Id);

                    if (config.DebugMode)
                    {
                        MessageBox.Show(string.Format("Message: {0}\nStack trace: {1}", ex.Message, ex.StackTrace), Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            if (this.EnableUssdEvent)
            {
                if (this.UssdResponseReceived != null)
                {
                    // Raise the USSD response event               
                    UssdReceivedEventArgs e = new UssdReceivedEventArgs(ussdResponse);
                    this.UssdResponseReceived.BeginInvoke(this, e, new AsyncCallback(this.AsyncCallback), null);
                }
            }
            else
            {
                // Put the response into a FIFO list
                ussdResponseQueue.Add(ussdResponse);
            }
        }

        /// <summary>
        /// Process received call
        /// </summary>
        /// <param name="e">Call indication object</param>
        private void ProcessCallReceived(IIndicationObject e)
        {
            this.statistics.IncomingCall++;
            if (this.CallReceived != null)
            {
                CallInformation callInformation = (CallInformation)e;
                callInformation.GatewayId = this.Id;
                if (!string.IsNullOrEmpty(callInformation.Number))  // Raise event if calling number is not empty
                {
                    IncomingCallEventArgs incomingCallEventArgs = new IncomingCallEventArgs(callInformation);
                    this.CallReceived.BeginInvoke(this, incomingCallEventArgs, new AsyncCallback(this.AsyncCallback), null);
                }
            }

            // If set to true, disconnect the incoming call automatically
            if (config.AutoDisconnectIncomingCall && this.DisconnectCallReceived != null)
            {
                CallInformation callInformation = (CallInformation)e;
                IncomingCallEventArgs incomingCallEventArgs = new IncomingCallEventArgs(callInformation);
                this.DisconnectCallReceived.BeginInvoke(this, incomingCallEventArgs, new AsyncCallback(this.AsyncCallback), null);
            }
        }


        /// <summary>
        /// Dial a call
        /// </summary>
        /// <param name="phoneNumber">Phone number</param>
        private void OnCallDialled(string phoneNumber)
        {
            if (this.CallDialled != null)
            {
                Logger.LogThis("Firing async CallDialled event.", LogLevel.Verbose, this.Id);
                NumberType numberType = NumberType.Domestic;
                if (phoneNumber.StartsWith("+")) numberType = NumberType.International;
                CallInformation callInformation = new CallInformation(phoneNumber, numberType, DateTime.Now, this.Id);
                OutgoingCallEventArgs e = new OutgoingCallEventArgs(callInformation);
                this.CallDialled.BeginInvoke(this, e, new AsyncCallback(this.AsyncCallback), null);
            }
        }


        /// <summary>
        /// Process received message
        /// </summary>
        /// <param name="e">Message indication object</param>
        private void ProcessMessageReceived(IIndicationObject e)
        {
            if (this.RawMessageReceived != null)
            {
                this.RawMessageReceived.BeginInvoke(this, e, new AsyncCallback(this.AsyncCallback), null);
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
                if (result.AsyncDelegate is MessageReceivedEventHandler)
                {
                    Logger.LogThis("Ending async MessageReceivedEventHandler call", LogLevel.Verbose, this.Id);
                    ((MessageReceivedEventHandler)result.AsyncDelegate).EndInvoke(result);
                }
                else if (result.AsyncDelegate is RawMessageReceivedEventHandler)
                {
                    Logger.LogThis("Ending async RawMessageReceivedEventHandler call", LogLevel.Verbose, this.Id);
                    ((RawMessageReceivedEventHandler)result.AsyncDelegate).EndInvoke(result);
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
                else if (result.AsyncDelegate is IncomingCallEventHandler)
                {
                    Logger.LogThis("Ending async IncomingCallEventHandler call", LogLevel.Verbose, this.Id);
                    ((IncomingCallEventHandler)result.AsyncDelegate).EndInvoke(result);
                }
                else if (result.AsyncDelegate is OutgoingCallEventHandler)
                {
                    Logger.LogThis("Ending async OutgoingCallEventHandler call", LogLevel.Verbose, this.Id);
                    ((OutgoingCallEventHandler)result.AsyncDelegate).EndInvoke(result);
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
                else if (result.AsyncDelegate is UssdResponseReceivedHandler)
                {
                    Logger.LogThis("Ending async UssdResponseReceivedHandler call", LogLevel.Verbose, this.Id);
                    ((UssdResponseReceivedHandler)result.AsyncDelegate).EndInvoke(result);
                }
                else if (result.AsyncDelegate is WatchDogFailureEventHandler)
                {
                    Logger.LogThis("Ending async WatchDogFailureEventHandler call", LogLevel.Verbose, this.Id);
                    ((WatchDogFailureEventHandler)result.AsyncDelegate).EndInvoke(result);
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

        private void CheckInboundStatusReport(MemoryLocation memoryLocation)
        {
            if (this.isMessageIndicationEnabled || this.PollNewMessages)
            {
                GetMessageByIndexFeature feature = GetFeature<GetMessageByIndexFeature>(GetMessageByIndexFeature.NewInstance());
                MessageInformation msg = null;
                try
                {
                    feature.Index = memoryLocation.Index;
                    IContext context = Execute(feature);
                    msg = context.GetResult() as MessageInformation;
                }
                catch (Exception ex)
                {
                    this.exception = ex;
                }

                // Check for echo
                CheckGatewayStatus(feature);

                // Raise the events
                if (msg != null)
                {
                    if (!license.Valid)
                    {
                        if (!string.IsNullOrEmpty(msg.Content) && !msg.Content.StartsWith(Resources.UnlicensedMessagePrefix))
                        {
                            msg.Content = Resources.UnlicensedMessagePrefix + msg.Content;
                        }
                    }

                    msg.GatewayId = this.Id;
                    this.statistics.IncomingSms++;
                    if (this.MessageReceived != null)
                    {
                        MessageReceivedEventArgs e = new MessageReceivedEventArgs(msg);
                        this.MessageReceived.BeginInvoke(this, e, new AsyncCallback(this.AsyncCallback), null);
                    }
                    else
                    {
                        Logger.LogThis("Warning: No event handlers for MessageReceived event, message is ignored.", LogLevel.Verbose, this.Id);
                    }

                    // Check if need to delete the message automatically
                    if (config.DeleteReceivedMessage)
                    {
                        foreach (int index in msg.Indexes)
                        {
                            Logger.LogThis("Delete message with index " + index, LogLevel.Verbose);
                            this.DeleteMessage(MessageDeleteOption.ByIndex, index);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Checks for inbound messages.
        /// </summary>
        private void CheckInboundMessages()
        {
            if (this.isMessageIndicationEnabled || this.PollNewMessages)
            {
                GetMessageFeature feature = GetFeature<GetMessageFeature>(GetMessageFeature.NewInstance());
                feature.ConcatenateMessage = true;
                feature.Sort = false;
                feature.MessageStatusType = MessageStatusType.AllMessages;
                IContext context = Execute(feature);
                List<MessageInformation> messages = (List<MessageInformation>)context.GetResult();

                // Check for echo
                CheckGatewayStatus(feature);

                List<MessageInformation> tmpMessageList = messages.FindAll(
                    delegate(MessageInformation msg)
                    {
                        return (msg.MessageStatusType == MessageStatusType.ReceivedUnreadMessages && msg.TotalPiece == msg.TotalPieceReceived);
                    }
                );

                // Raise the events
                foreach (MessageInformation msg in tmpMessageList)
                {
                    if (!license.Valid)
                    {
                        if (!string.IsNullOrEmpty(msg.Content) && !msg.Content.StartsWith(Resources.UnlicensedMessagePrefix))
                        {
                            msg.Content = Resources.UnlicensedMessagePrefix + msg.Content;
                        }
                    }

                    msg.GatewayId = this.Id;
                    this.statistics.IncomingSms++;
                    if (this.MessageReceived != null)
                    {
                        MessageReceivedEventArgs e = new MessageReceivedEventArgs(msg);
                        this.MessageReceived.BeginInvoke(this, e, new AsyncCallback(this.AsyncCallback), null);
                    }
                    else
                    {
                        Logger.LogThis("Warning: No event handlers for MessageReceived event, message is ignored.", LogLevel.Verbose, this.Id);
                    }

                    // Check if need to delete the message automatically
                    if (config.DeleteReceivedMessage)
                    {
                        foreach (int index in msg.Indexes)
                        {
                            Logger.LogThis("Delete message with index " + index, LogLevel.Verbose);
                            this.DeleteMessage(MessageDeleteOption.ByIndex, index);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Poll for new unread messages
        /// </summary>
        private void MessagePolling()
        {
            while (Connected)
            {
                try
                {
                    // Check for inbound messages
                    CheckInboundMessages();

                    // Sleep and wait for next run
                    Thread.Sleep(config.MessagePollingInterval);
                }
                catch (ThreadInterruptedException tiEx)
                {
                    if (!Connected) break;
                }
                catch (ThreadAbortException taEx)
                {
                    //if (!Connected) break;
                    break;
                }
                catch (Exception e)
                {
                    Logger.LogThis("Error polling new messages: " + e.Message, LogLevel.Error, this.Id);
                    if (!Connected) break;

                    // Sleep and wait for next run
                    Thread.Sleep(config.MessagePollingInterval);

                }
            }
        }

        /// <summary>
        /// Verify if the port exits
        /// </summary>
        /// <param name="port">Serial port</param>
        /// <returns>
        /// true if the port exists, otherwise returns false
        /// </returns>
        private bool VerifyPort(SerialPort port)
        {
            if (port == null || !port.IsOpen) return false;

            string[] portNames = SerialPort.GetPortNames();
            if (!portNames.Contains(port.PortName)) return false;

            try
            {
                port.DtrEnable = false;
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Checks the port.
        /// </summary>
        private void CheckPort()
        {
            // Check if the port exists                
            string[] portNames = SerialPort.GetPortNames();
            bool isPortExists = false;
            foreach (string portName in portNames)
            {
                if (portName.ToUpper() == config.PortName.ToUpper())
                {
                    isPortExists = true;
                    break;
                }
            }
            if (!isPortExists)
            {
                throw new GatewayException(string.Format(Resources.PortNotExistException, config.PortName));
            }
        }

        /// <summary>
        /// Opens the port.
        /// </summary>
        private void OpenPort()
        {
            // If the port is already open, close it
            if (Port != null && Port.IsOpen)
            {
                Disconnect();
            }

            if (config.SafeConnect)
            {
                SerialPortHelper.Execute(config.PortName);
            }

            // Create the serial port
            port = new SerialPort();
            Port.ReadTimeout = DefaultReadTimeout;
            Port.WriteTimeout = DefaultWriteTimeout;

            // Set the port properties
            Port.PortName = config.PortName;
            Port.Parity = (Parity)Enum.Parse(typeof(Parity), config.Parity.ToString());
            Port.Handshake = (Handshake)Enum.Parse(typeof(Handshake), config.Handshake.ToString());
            Port.StopBits = (StopBits)Enum.Parse(typeof(StopBits), config.StopBits.ToString());
            Port.DataBits = (int)Enum.Parse(typeof(PortDataBits), config.DataBits.ToString());
            Port.BaudRate = (int)Enum.Parse(typeof(PortBaudRate), config.BaudRate.ToString());

            //Port.Encoding = Encoding.GetEncoding(0x4e4);
            Port.DataReceived += new SerialDataReceivedEventHandler(this.Port_DataReceived);

            // Connect to the port
            Port.Open();

            // Get the serial port stream
            this.internalSerialStream = Port.BaseStream;

            // Suppress the finalizer to prevent ObjectDisposedException
            // See http://social.msdn.microsoft.com/Forums/en-US/netfxbcl/thread/8a1825d2-c84b-4620-91e7-3934a4d47330
            // See http://social.msdn.microsoft.com/Forums/en-US/Vsexpressvb/thread/e43128ed-2979-422d-9731-2c206bc5bb69
            // See http://connect.microsoft.com/VisualStudio/feedback/ViewFeedback.aspx?FeedbackID=140018
            // See http://connect.microsoft.com/VisualStudio/feedback/ViewFeedback.aspx?FeedbackID=426766
            GC.SuppressFinalize(Port.BaseStream);

            // Set to true
            Port.DtrEnable = config.DtrEnable;
            Port.RtsEnable = config.RtsEnable;

        }

        /// <summary>
        /// Starts the data reader.
        /// </summary>
        private void StartDataReader()
        {
            if (inboundDataReader != null && inboundDataReader.IsAlive)
            {
                // Stop the thread first
                StopDataReader();
            }

            // Start the reader thread
            inboundDataReader = new Thread(new ThreadStart(this.InBoundDataReader));
            inboundDataReader.IsBackground = true;
            inboundDataReader.Start();
        }

        /// <summary>
        /// Stops the data reader.
        /// </summary>
        private void StopDataReader()
        {
            try
            {
                if (inboundDataReader != null)
                {
                    //inboundDataReader.Abort();                    
                    //inboundDataReader.Interrupt();
                    //if (!inboundDataReader.Join(500))
                    //{
                    inboundDataReader.Abort();
                    //}                  
                    inboundDataReader = null;
                }
            }
            catch (Exception e) { }
        }

        /// <summary>
        /// Connects to the gateway.
        /// </summary>
        private void ConnectGateway()
        {
            ExecuteFeature feature = GetFeature<ExecuteFeature>(ExecuteFeature.NewInstance());
            IContext context;
            // Any initialization string
            if (!string.IsNullOrEmpty(config.InitializationString))
            {
                feature.Request = config.InitializationString;
                context = Execute(feature);
            }

            // Synchronize the gateway
            Reset();
            if (!Synchronize())
            {
                throw new GatewayException(Resources.InvalidComPort);
            }
            EchoOff();

            bool checkNetworkRegistration = false;
            if (!config.DisablePinCheck)
            {
                while (true)
                {
                    // Get the sim status
                    Response simStatus = GetSimStatus();

                    if (simStatus == Response.Ready)
                    {
                        break;
                    }
                    else if (simStatus == Response.SimPinRequired || simStatus == Response.SimPin2Required)
                    {
                        // Need to send the PIN                        
                        if (string.IsNullOrEmpty(config.Pin))
                        {
                            throw new GatewayException(Resources.SimPinNotSetException);
                        }
                        // Enter the PIN
                        Logger.LogThis("Entering the PIN, and wait for 5 seconds...", LogLevel.Info, this.Id);

                        EnterPinFeature enterPinFeature = GetFeature<EnterPinFeature>(EnterPinFeature.NewInstance());
                        enterPinFeature.Pin = config.Pin;
                        context = Execute(enterPinFeature);
                        Thread.Sleep(5000);
                        checkNetworkRegistration = true;
                    }
                    else if (simStatus == Response.Busy)
                    {
                        Logger.LogThis("SIM busy. Sleep for 1 seconds before retrying..", LogLevel.Warn, this.Id);
                        // Sleep for 1 second
                        Thread.Sleep(1000);
                    }
                }
            }
            EchoOff();
            Initialize();
            EchoOff();

            // Check and wait for network registration if necessary
            if (checkNetworkRegistration && config.CheckNetworkRegistrationOnStartup) WaitForNetworkRegistration();
        }


        /// <summary>
        /// Configures the gateway.
        /// </summary>
        private void ConfigureGateway()
        {
            // Set verbose error - for CME related error
            IContext context;
            SetVerboseErrorFeature setVerboseErrorFeature = GetFeature<SetVerboseErrorFeature>(SetVerboseErrorFeature.NewInstance());
            context = Execute(setVerboseErrorFeature);

            // Get storage location
            GetMessageStorageLocationFeature storageLocationFeature = GetFeature<GetMessageStorageLocationFeature>(GetMessageStorageLocationFeature.NewInstance());
            context = Execute(storageLocationFeature);
            string[] locations = (string[])context.GetResult();
            if (locations != null && locations.Length > 0)
            {
                object obj = StringEnum.Parse(typeof(MessageStorage), locations[0]);
                if (obj != null)
                    messageStorage = (MessageStorage)obj;     // Fix on 2 Nov 2011
                //messageStorage = MessageStorage;
            }

            // Set message protocol to PDU
            SetMessageProtocol(MessageProtocol.PduMode);

            // By default disable message notification
            DisableMessageNotifications();

        }


        /// <summary>
        /// Starts the watch dog.
        /// </summary>
        private void StartWatchDog()
        {
            if (!config.DisableWatchDog)
            {
                if (watchDog != null && watchDog.IsAlive)
                {
                    // Stop it
                    StopWatchDog();
                }

                // Start the watch dog thread
                watchDog = new Thread(new ThreadStart(this.WatchDog));
                watchDog.IsBackground = true;
                watchDog.Start();
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
        /// Stops the watch dog.
        /// </summary>
        private void StopWatchDog()
        {
            try
            {
                if (watchDog != null)
                {
                    watchDog.Abort();
                    /*(
                    watchDog.Interrupt();
                    if (!watchDog.Join(500))
                    {
                        watchDog.Abort();
                    } 
                    */
                    watchDog = null;
                }
            }
            catch (Exception e) { }
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
                    /*
                    messageQueueProcessor.Interrupt();
                    if (!messageQueueProcessor.Join(500))
                    {
                        messageQueueProcessor.Abort();
                    }*/

                    messageQueueProcessor = null;
                }
            }
            catch (Exception e) { }
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
                    /*
                    delayedMessageProcessor.Interrupt();
                    if (!delayedMessageProcessor.Join(500))
                    {
                        delayedMessageProcessor.Abort();
                    }*/

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
                    /*
                    messagePoller.Interrupt();
                    if (!messagePoller.Join(500))
                    {
                        messagePoller.Abort();
                    }
                    */
                    messagePoller = null;
                }
            }
            catch (Exception e) { }
        }

        /// <summary>
        /// Closes the port.
        /// </summary>
        /// <returns></returns>
        private bool ClosePort()
        {
            // If the port is already open, close it
            // Need to wait so that the port is closed properly
            //if (Port.IsOpen)
            if (Port != null && Port.IsOpen)
            {
                try
                {
                    if (!VerifyPort(Port))
                    {
                        throw new GatewayException(string.Format(Resources.GatewayDisconnectedException, Port.PortName));
                    }
                    Port.Close();
                    Port.DataReceived -= new SerialDataReceivedEventHandler(this.Port_DataReceived);

                    try
                    {
                        Port.Dispose();
                    }
                    catch (Exception) { }

                    Thread.Sleep(CloseWaitInterval);
                }
                catch (Exception e)
                {
                    this.exception = e;
                    return false;

                }
            }

            if (config.SafeDisconnect)
            {
                SerialPortHelper.SafeDisconnect(Port, internalSerialStream);
            }
            port = null;

            return true;
        }

        /// <summary>
        /// Called when gateway is connected.
        /// </summary>        
        private void OnGatewayConnected()
        {
            //if (this.GatewayConnected != null)
            //{
            ConnectionEventArgs args = new ConnectionEventArgs(this.Id);
            this.config.OnConnected(args);
            //this.GatewayConnected.BeginInvoke(this, args, new AsyncCallback(this.AsyncCallback), null);
            //}
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
        /// Called when watch dog failed
        /// </summary>
        private void OnWatchDogFailed()
        {
            if (this.WatchDogFailed != null)
            {
                WatchDogEventArgs args = new WatchDogEventArgs(this.Id);
                args.Error = this.LastError;
                this.WatchDogFailed.BeginInvoke(this, args, new AsyncCallback(this.AsyncCallback), null);
            }
        }

        /// <summary>
        /// Checks the gateway status.
        /// </summary>
        /// <param name="feature">The feature.</param>
        private void CheckGatewayStatus(IFeature feature)
        {
            if (feature.HasEcho)
            {
                Reset();
                Synchronize();
                EchoOff();
            }
        }


        /// <summary>
        /// Handles the DisconnectCallReceived event of the BaseMobileGateway control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MessagingToolkit.Core.Mobile.Event.IncomingCallEventArgs"/> instance containing the event data.</param>
        private void BaseMobileGateway_DisconnectCallReceived(object sender, IncomingCallEventArgs e)
        {
            string phoneNumber = e.CallInformation.Number;
            if (string.IsNullOrEmpty(phoneNumber)) phoneNumber = "[unknown]";
            Logger.LogThis(string.Format("Automatic disconnect incoming call from {0}", phoneNumber), LogLevel.Verbose, this.Id);
            HangUp();
        }
        #endregion =========================================================================



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
            str = String.Concat(str, "Port = ", Port, "\r\n");
            str = String.Concat(str, "Configuration = ", Configuration, "\r\n");
            str = String.Concat(str, "Connected = ", Connected, "\r\n");
            str = String.Concat(str, "ReadTimeout = ", ReadTimeout, "\r\n");
            str = String.Concat(str, "WriteTimeout = ", WriteTimeout, "\r\n");
            str = String.Concat(str, "Echo = ", Echo, "\r\n");
            str = String.Concat(str, "DeviceInformation = ", DeviceInformation, "\r\n");
            str = String.Concat(str, "ServiceCentreAddress = ", ServiceCentreAddress, "\r\n");
            str = String.Concat(str, "MessageStorage = ", MessageStorage, "\r\n");
            str = String.Concat(str, "NetworkRegistration = ", NetworkRegistration, "\r\n");
            str = String.Concat(str, "Statistics = ", Statistics, "\r\n");
            str = String.Concat(str, "BatchMessageMode = ", BatchMessageMode, "\r\n");
            str = String.Concat(str, "SignalQuality = ", SignalQuality, "\r\n");
            str = String.Concat(str, "SupportedCharacterSets = ", SupportedCharacterSets, "\r\n");
            str = String.Concat(str, "CurrentCharacterSet = ", CurrentCharacterSet, "\r\n");
            str = String.Concat(str, "BatteryCharge = ", BatteryCharge, "\r\n");
            str = String.Concat(str, "IsAcknowledgeRequired = ", IsAcknowledgeRequired, "\r\n");
            str = String.Concat(str, "IsMessageQueueEnabled = ", IsMessageQueueEnabled, "\r\n");
            str = String.Concat(str, "PhoneBookStorages = ", PhoneBookStorages, "\r\n");
            str = String.Concat(str, "MessageMemoryStatus = ", MessageMemoryStatus, "\r\n");
            str = String.Concat(str, "NetworkOperator = ", NetworkOperator, "\r\n");
            str = String.Concat(str, "Subscribers = ", Subscribers, "\r\n");
            str = String.Concat(str, "PollNewMessages = ", PollNewMessages, "\r\n");
            str = String.Concat(str, "Attributes = ", Attributes, "\r\n");
            str = String.Concat(str, "License = ", License, "\r\n");
            str = String.Concat(str, "EnableUssdEvent = ", EnableUssdEvent, "\r\n");
            str = String.Concat(str, "PersistenceQueue = ", PersistenceQueue, "\r\n");
            str = String.Concat(str, "PersistenceFolder = ", PersistenceFolder, "\r\n");
            str = String.Concat(str, "EncodedUssdCommand = ", EncodedUssdCommand, "\r\n");
            return str;
        }
    }
}
