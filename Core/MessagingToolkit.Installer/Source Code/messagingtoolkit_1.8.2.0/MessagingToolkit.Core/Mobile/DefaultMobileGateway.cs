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


using MessagingToolkit.Core.Base;
using MessagingToolkit.Core.Log;
using MessagingToolkit.Core.Mobile.Feature;
using MessagingToolkit.Core.Properties;
using MessagingToolkit.Core.Mobile.Message;
using MessagingToolkit.Core.Mobile.Event;
using MessagingToolkit.Core.Mobile.PduLibrary;
using MessagingToolkit.Core.Service;

namespace MessagingToolkit.Core.Mobile
{
    /// <summary>
    /// Dummy implementation of the mobile gateway
    /// </summary>
    [System.Reflection.ObfuscationAttribute(Feature = "renaming", ApplyToMembers = true)]
    internal class DefaultMobileGateway : IMobileGateway
    {

        public DefaultMobileGateway()
        {
            // Initialize the logger
            Logger.UseSensibleDefaults("default.log", string.Empty, LogLevel.Error, LogNameFormat.NameDate);
            Logger.LogPrefix = LogPrefix.Dt;
        }

        #region IMobileGateway Members

        public string Id
        {
            get;
            set;
        }

        public GatewayStatus Status
        {
            get;
            set;
        }

        public DeviceInformation DeviceInformation
        {
            get
            {
                return new DeviceInformation();
            }
        }

        public MobileGatewayConfiguration Configuration
        {
            get
            {
                return MobileGatewayConfiguration.NewInstance();
            }
        }


//#if (!MONO)
//        public SerialPort Port
//#else
        public CustomSerialPort Port
//#endif
        {
            get
            {
//#if (!MONO)
//                return new SerialPort();
//#else
                return new CustomSerialPort();
//#endif
                
            }           
        }

        public bool Connected
        {
            get
            {
                return false;

            }
        }

        public int ReadTimeout
        {
            get
            {
                return 0;
            }
            set
            {
               
            }
        }

        public int WriteTimeout
        {
            get
            {
                return 0;
            }
            set
            {
                
            }
        }

        public bool Echo
        {
            get
            {
                return false;
            }
            set
            {
              
            }
        }

        public NumberInformation ServiceCentreAddress
        {
            get 
            {
                return new NumberInformation();
            }
        }

        public MessageStorage MessageStorage
        {
            get
            {
                return new MessageStorage();
            }
            set
            {
                
            }
        }

        public Statistics Statistics
        {
            get 
            {
                return new Statistics();
            }
        }

        public NetworkRegistration NetworkRegistration
        {
            get 
            {
                return new NetworkRegistration();
            }
        }

        public BatchMessageMode BatchMessageMode
        {
            get
            {
                return BatchMessageMode.Disabled; 
            }
            set
            {
               
            }
        }

        public SignalQuality SignalQuality
        {
            get 
            {
                return new SignalQuality(0, 0);
            }
        }

        public string[] SupportedCharacterSets
        {
            get 
            {
                return new string[] { };
            }
        }

        public string CurrentCharacterSet
        {
            get 
            {
                return string.Empty;
            }
        }

        public BatteryCharge BatteryCharge
        {
            get 
            {
                return new BatteryCharge(0, 0);
            }
        }

        public bool IsAcknowledgeRequired
        {
            get 
            {
                return false;
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

        public string[] PhoneBookStorages
        {
            get 
            {
                return new string[] { };
            }
        }

        public MessageMemoryStatus MessageMemoryStatus
        {
            get 
            {
                return new MessageMemoryStatus();
            }
        }

        public NetworkOperator NetworkOperator
        {
            get 
            {
                return new NetworkOperator(NetworkOperatorFormat.LongFormatAlphanumeric, string.Empty);
            }
        }

        public Subscriber[] Subscribers
        {
            get { return new Subscriber[] { }; }
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

        public bool Connect()
        {
            return false;
        }

        public bool Disconnect()
        {
            return false;
        }

        public bool Disable()
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

        public string Diagnose()
        {
            return string.Empty;
        }

        public List<MessageInformation> GetMessages(MessageStatusType messageType)
        {
            return new List<MessageInformation>();
        }


        public List<MessageInformation> GetMessages(MessageStatusType messageType, List<MessageStorage> storages)
        {
            return new List<MessageInformation>();
        }

        public List<MessageInformation> GetNotifications(NotificationType notificationType, NotificationStatus notificationStatus)
        {
            return new List<MessageInformation>(1);
        }

        public virtual bool GetMms(MessageInformation mmsNotification, ref Mms mms)
        {
            return false;
        }

        public MessageInformation GetMessage(int index)
        {
            return new MessageInformation();
        }

        public bool DeleteMessage(MessageDeleteOption messageDeleteOption, params int[] indexes)
        {
            return false;
        }

        public string SendCommand(string command)
        {
            return string.Empty;
        }

        public bool EnableNewMessageNotification(MessageNotification notificationFlag)
        {
            return false;
        }

        public bool EnableMessageNotifications()
        {
            return false;
        }

        public bool EnableMessageRouting()
        {
            return false;
        }

        public bool DisableMessageNotifications()
        {
            return false;
        }

        public bool DisableMessageRouting()
        {
            return false;
        }

        public bool EnableCallNotifications()
        {
            return false;
        }

        public bool DisableCallNotifications()
        {
            return false;
        }

        public bool EnableCLIR()
        {
            return false;
        }

        public bool DisableCLIR()
        {
            return false;
        }

        public bool EnableCOLP()
        {
            return false;
        }

        public bool DisableCOLP()
        {
            return false;
        }

        public bool SetCharacterSet(string characterSet)
        {
            return false;
        }

        public bool Dial(string phoneNumber)
        {
            return false;
        }

        public bool HangUp()
        {
            return false;
        }

        public bool Answer()
        {
            return false;
        }

        public bool SetServiceCentreAddress(NumberInformation address)
        {
            return false;
        }

        public bool AcknowledgeNewMessage()
        {
            return false;
        }

        public bool RequireAcknowledgeNewMessage(bool flag)
        {
            return false;
        }

        public PhoneBookEntry[] GetPhoneBook(string storage)
        {
            return new PhoneBookEntry[] { };
        }

        public PhoneBookEntry[] GetPhoneBook(PhoneBookStorage storage)
        {
            return new PhoneBookEntry[] { };
        }

        public MemoryStatusWithStorage GetPhoneBookMemoryStatus(string storage)
        {
            return new MemoryStatusWithStorage(string.Empty, 0, 0);
        }

        public MemoryStatusWithStorage GetPhoneBookMemoryStatus(PhoneBookStorage storage)
        {
            return new MemoryStatusWithStorage(string.Empty, 0, 0);
        }

        public vCard[] ExportPhoneBookTovCard(PhoneBookEntry[] phoneBookEntries)
        {
            return new vCard[]{};
        }

        public SupportedNetworkOperator[] GetSupportedNetworkOperators()
        {
            return new SupportedNetworkOperator[] { };
        }

        public bool AddPhoneBookEntry(PhoneBookEntry entry, string storage)
        {
            return false;
        }

        public bool AddPhoneBookEntry(PhoneBookEntry entry, PhoneBookStorage storage)
        {
            return false;
        }

        public bool DeletePhoneBook(string storage)
        {
            return false;
        }

        public bool DeletePhoneBook(PhoneBookStorage storage)
        {
            return false;
        }

        public bool DeletePhoneBookEntry(int index, string storage)
        {
            return false;
        }

        public bool DeletePhoneBookEntry(int index, PhoneBookStorage storage)
        {
            return false;
        }

        public PhoneBookEntry[] FindPhoneBookEntries(string searchText, string storage)
        {
            return new PhoneBookEntry[] { };
        }

        public PhoneBookEntry[] FindPhoneBookEntries(string searchText, PhoneBookStorage storage)
        {
            return new PhoneBookEntry[] { };
        }

        public List<int> SaveMessage(Sms message, MessageStatusType status)
        {
            return new List<int>();
        }

        public void Dispose() { }

        public string SendUssd(string ussdCommand)
        {
            return string.Empty;
        }

        public string SendUssd(string ussdCommand, bool interactive)
        {
            return string.Empty;
        }

        public UssdResponse SendUssd(UssdRequest ussdCommand)
        {
            return new UssdResponse();
        }


        public bool SendDtmf(string tones)
        {
            return false;
        }

       
        public bool SendDtmf(string tones, int duration)
        {
            return false;
        }

        public bool CancelUssdSession()
        {
            return true;
        }

        public bool SetMessageProtocol(MessageProtocol protocol)
        {
            return false;
        }
      
        public bool ClearQueue()
        {
            return false;
        }

        public int GetQueueCount()
        {
            return 0;
        }

        public bool ValidateConnection()
        {
            return false;
        }

        public bool InitializeDataConnection()
        {
            return false;
        }

        public event MessageEventHandler MessageSending
        {
            add {  }
            remove { }
        }

        public event MessageEventHandler MessageSent
        {
            add {  }
            remove {  }
        }

        public event MessageErrorEventHandler MessageSendingFailed
        {
            add {}
            remove {}
        }

        public event MessageReceivedEventHandler MessageReceived
        {
            add { }
            remove { }
        }

        public event UssdResponseReceivedHandler UssdResponseReceived
        {
            add { }
            remove { }
        }

        public event IncomingCallEventHandler CallReceived
        {
            add { }
            remove { }
        }


        public event OutgoingCallEventHandler CallDialled
        {
            add { }
            remove { }
        }

        /*
        public event ConnectedEventHandler GatewayConnected
        {
            add { }
            remove { }
        }
        */

        public event DisconnectedEventHandler GatewayDisconnected
        {
            add { }
            remove { }
        }

        public event WatchDogFailureEventHandler WatchDogFailed
        {
            add { }
            remove { }
        }
        #endregion

        #region IGateway Members

        public LogLevel LogLevel
        {
            get
            {
                return LogLevel.Error;
            }
            set
            {                
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

        public License License
        {
            get
            {
                MobileGatewayConfiguration config = MobileGatewayConfiguration.NewInstance();
                return new License(config);                
            }
        }

     
        public bool EnableUssdEvent
        {
            get
            {
                return false;
            }
            set
            {
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


        public bool EncodedUssdCommand
        {
            get
            {
                return false;
            }
            set
            {
            }
        }

        public LogDestination LogDestination
        {
            get
            {
                return LogDestination.File;
                
            }
            set
            {
               
            }
        }



        public string LogFile
        {
            get
            {
                return string.Empty;
            }
        }

        public Exception LastError
        {
            get 
            {
                return new Exception("Default mobile gateway");
            }
        }

        public void ClearError()
        {
           
        }


        /// <summary>
        /// Clears the log file content
        /// </summary>
        public void ClearLog() { }


        #endregion
    }
}
