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
using MessagingToolkit.Core.Mobile.Message;
using MessagingToolkit.Core.Mobile.Event;
using MessagingToolkit.Core.Service;

namespace MessagingToolkit.Core.Mobile
{

    /// <summary>
    /// This is the interface that is used as the access
    /// point to the mobile gateway
    /// </summary>
    public interface IMobileGateway: IGateway, IDisposable
    {

        #region ========== Properties signatures =====================================================
               

        /// <summary>
        /// Return the device information
        /// </summary>
        /// <value>Device information</value>
        DeviceInformation DeviceInformation
        {
            get;
        }
       
        /// <summary>
        /// Return the gateway configuration
        /// </summary>
        /// <value>Configuration object</value>
        MobileGatewayConfiguration Configuration
        {
            get;            
        }

        /// <summary>
        /// Physical connection to the gateway
        /// </summary>
        /// <value>Serial port. See <see cref="SerialPort"/></value>
//#if (!MONO)
//        SerialPort Port
//#else
        CustomSerialPort Port
//#endif
        {
            get;           
        }

        /// <summary>
        /// Return the gateway connection status
        /// </summary>
        /// <value>true if connected, false if it is disconnected</value>
        bool Connected
        {
            get;
        }

        /// <summary>
        /// Read time out in milliseconds
        /// </summary>
        /// <value>Time out value</value>
        int ReadTimeout
        {
            get;
            set;
        }

        /// <summary>
        /// Write time out in milliseconds
        /// </summary>
        /// <value>Time out value</value>
        int WriteTimeout
        {
            get;
            set;
        }

        /// <summary>
        /// Set the echo mode
        /// </summary>
        /// <value>Echo mode</value>
        bool Echo
        {
            get;
            set;
        }


        /// <summary>
        /// Service center number information
        /// </summary>
        /// <value>Gateway service center address</value>
        NumberInformation ServiceCentreAddress
        {
            get;
        }


        /// <summary>
        /// Memory location for reading/deleting, writing/sending short messages.
        /// See <see cref="MessageStorage"/>
        /// </summary>
        /// <value>Message memory</value>
        MessageStorage MessageStorage
        {
            get;
            set;
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
        /// Get the network registration status
        /// </summary>
        /// <value>Network registration status</value>
        NetworkRegistration NetworkRegistration
        {
            get;
        }

        /// <summary>
        /// Determine if need to batch send the message
        /// </summary>
        /// <value>Can be disabled, temporary enabled, or disabled </value>
        BatchMessageMode BatchMessageMode
        {
            get;
            set;
        }

        /// <summary>
        /// Return the signal quality of the gateway
        /// </summary>
        /// <value>Signal quality</value>
        SignalQuality SignalQuality
        {
            get;
        }

        /// <summary>
        /// Get all supported character sets
        /// </summary>
        /// <value>List of supported character sets</value>
        string[] SupportedCharacterSets
        {
            get;
        }

        /// <summary>
        /// Get current character set
        /// </summary>
        /// <value>Current character set</value>
        string CurrentCharacterSet
        {
            get;
        }

        /// <summary>
        /// Get battery charging information
        /// </summary>
        /// <value>Battery charge</value>
        BatteryCharge BatteryCharge
        {
            get;
        }

        /// <summary>
        /// Check if needs to acknowledge routed messages
        /// </summary>
        /// <value>true if acknowledgement is required</value>
        bool IsAcknowledgeRequired
        {
            get;
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
        string[] PhoneBookStorages
        {
            get;
        }

        /// <summary>
        /// Get message memory status. The message memory location is
        /// specified using <see cref="MessageStorage"/>
        /// </summary>
        /// <returns>Memory status</returns>
        MessageMemoryStatus MessageMemoryStatus
        {
            get;
        }

        /// <summary>
        /// Get current network operator
        /// </summary>
        /// <value>Network operator</value>
        NetworkOperator NetworkOperator
        {
            get;
        }

        /// <summary>
        /// Retrieve subscriber information from the gateway
        /// </summary>
        /// <value>A list of subscriber instances</value>
        Subscriber[] Subscribers
        {
            get;
        }

        /// <summary>
        /// Polling for new unread messages and raise message received event.
        /// The polling interval can be specified using <see cref="MobileGatewayConfiguration"/>.
        /// Default to false
        /// </summary>
        /// <value></value>
        bool PollNewMessages
        {
            get;
            set;
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
        /// Enables the incoming USSD event.
        /// Once enabled, SendUssd command will return immediately, and you have
        /// to use the Ussd received event to get the response.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if enable incoming ussd event; otherwise, <c>false</c>.
        /// </value>
        /// <returns>true if successful, false otherwise</returns>
        bool EnableUssdEvent
        {
            get;
            set;
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
        /// Gets or sets a value indicating whether the USSD command and response should be encoded.
        /// </summary>
        /// <value>
        ///   <c>true</c> if USSD command should be encoded; otherwise, <c>false</c>.
        /// </value>
        bool EncodedUssdCommand
        {
            get;
            set;
        }

       #endregion ========================================================================================


        #region ========== Public Methods ================================================================


        /// <summary>
        /// Connect to the gateway
        /// </summary>
        /// <returns>true if success, false otherwise</returns>
        bool Connect();

        /// <summary>
        /// Disconnect from the gateway
        /// </summary>
        /// <returns>true if success, false otherwise</returns>
        bool Disconnect();


        /// <summary>
        /// Disables this instance.
        /// </summary>
        /// <returns>true if successful, false otherwise</returns>
        bool Disable();


        /// <summary>
        /// Send message synchronously. 
        /// Since it is synchronous, message sending events will not be raised
        /// 
        /// Message can be 
        /// - SMS, 
        /// - EMS, 
        /// - Flash SMS
        /// - vCard,
        /// - vCalendar
        /// - Wappush
        /// - Ringtone, 
        /// - MMS      
        /// </summary>
        /// <param name="message">Message to be sent</param>
        /// <returns>true if successfully sent. If false then you need to check the last exception thrown</returns>
        bool Send(IMessage message);

              
        /// <summary>
        /// Send message asynchronously.
        /// Message sending events will be raised
        /// 
        /// Message can be 
        /// - SMS, 
        /// - EMS, 
        /// - Flash SMS
        /// - vCard,
        /// - vCalendar
        /// - Wappush
        /// - Ringtone, 
        /// - MMS     
        /// </summary>
        /// <param name="message">Message to be sent</param>
        /// <returns>true if successfully sent. If false then you need to check the last exception thrown</returns>
        bool SendToQueue(IMessage message);

     
        /// <summary>
        /// Check and verify the capabilities of the gateway
        /// </summary>
        /// <remarks>
        /// This is useful if we need to determine the capabilities of new
        /// gateway. Run this if you need to do troubleshooting
        /// </remarks>
        /// <returns>The diagnostic results</returns>
        string Diagnose();

        /// <summary>
        /// Retrieve message from the gateway. Set the memory location
        /// using <see cref="MessageStorage"/>
        /// </summary>
        /// <param name="messageType">Message type. See <see cref="MessageStatusType"/></param>
        /// <returns>
        /// A list of messages
        /// </returns>
        List<MessageInformation> GetMessages(MessageStatusType messageType);

        /// <summary>
        /// Retrieve message from the gateway based on the list of storages that are passed in
        /// </summary>
        /// <param name="messageType">Message type. See <see cref="MessageStatusType"/></param>
        /// <param name="storages">The storages.</param>
        /// <returns>
        /// A list of messages
        /// </returns>
        List<MessageInformation> GetMessages(MessageStatusType messageType, List<MessageStorage> storages);

        /// <summary>
        /// Gets the notifications.
        /// </summary>
        /// <param name="notificationType">Type of the notification.</param>
        /// <param name="notificationStatus">The notification status.</param>
        /// <returns>List of messages fulfilled the conditions</returns>
        List<MessageInformation> GetNotifications(NotificationType notificationType, NotificationStatus notificationStatus);

        /// <summary>
        /// Gets the MMS.
        /// </summary>
        /// <param name="mmsNotification">The MMS notification.</param>
        /// <param name="mms">The MMS.</param>
        /// <returns>The MMS instance if found</returns>
        bool GetMms(MessageInformation mmsNotification, ref Mms mms);

        /// <summary>
        /// Retrieve message from the gateway using index. Set the message location
        /// using <see cref="MessageStorage"/>
        /// </summary>
        /// <param name="index">Message index</param>
        /// <returns>Message information</returns>
        MessageInformation GetMessage(int index);
               

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
        bool DeleteMessage(MessageDeleteOption messageDeleteOption, params int[] indexes);
        
        /// <summary>
        /// Send command directly to the gateway
        /// </summary>
        /// <param name="command">Command string</param>
        /// <returns>Execution results</returns>
        string SendCommand(string command);

        /// <summary>
        /// Enable new message notification
        /// </summary>
        /// <param name="notificationFlag">Notification flag</param>
        /// <returns>true if can be set</returns>
        bool EnableNewMessageNotification(MessageNotification notificationFlag);

        /// <summary>
        /// Enable message notifications
        /// </summary>
        /// <returns>true if message notifications can be enabled</returns>
        bool EnableMessageNotifications();


        /// <summary>
        /// Enable message routing, meaning any messages received at the gateway
        /// are routed to the application without saving them
        /// </summary>
        /// <returns>true if enabled successfully</returns>
        bool EnableMessageRouting();

        /// <summary>
        /// Disable message notifications
        /// </summary>
        /// <returns>Disable message notifications</returns>
        bool DisableMessageNotifications();

        /// <summary>
        /// Disable message routing
        /// </summary>
        /// <returns>true if can be set successfully</returns>
        bool DisableMessageRouting();
        
        /// <summary>
        /// Enable calling line identity presentation
        /// </summary>
        /// <returns>true if successful, false otherwise</returns>
        bool EnableCallNotifications();

        /// <summary>
        /// Disable  calling line identity presentation
        /// </summary>
        /// <returns>true if successful, false otherwise</returns>
        bool DisableCallNotifications();

        /// <summary>
        /// Enable calling line identity restriction
        /// </summary>
        /// <returns>true if successful, false otherwise</returns>
        bool EnableCLIR();

        /// <summary>
        /// Disable calling line identity restriction
        /// </summary>
        /// <returns>true if successful, false otherwise</returns>
        bool DisableCLIR();

        /// <summary>
        /// Enable connected line identification presentation
        /// </summary>
        /// <returns>true if successful, false otherwise</returns>
        bool EnableCOLP();

        /// <summary>
        /// Disable connected line identification presentation
        /// </summary>
        /// <returns>true if successful, false otherwise</returns>
        bool DisableCOLP();

        /// <summary>
        /// Set current character set
        /// </summary>
        /// <param name="characterSet">Character set</param>
        /// <returns>true if successfull set</returns>
        bool SetCharacterSet(string characterSet);

        /// <summary>
        /// Dial a phone number
        /// </summary>
        /// <param name="phoneNumber">Phone number</param>
        /// <returns>true if successful</returns>
        bool Dial(string phoneNumber);

        /// <summary>
        /// Hang up the call in progress
        /// </summary>
        /// <returns>true if successful</returns>
        bool HangUp();

        /// <summary>
        /// Anwser the call
        /// </summary>
        /// <returns>true if can be answered</returns>
        bool Answer();

        /// <summary>
        /// Set service centre address
        /// </summary>
        /// <param name="address">Service centre address</param>
        /// <returns>true if successfully set</returns>
        bool SetServiceCentreAddress(NumberInformation address);


        /// <summary>
        /// Acknowledge a routed message if <see cref="IsAcknowledgeRequired"/> is true
        /// </summary>
        /// <returns>true if acknowledged successfully</returns>
        bool AcknowledgeNewMessage();


        /// <summary>
        /// Set the flag acknowledge new message. True if need to acknowledge
        /// </summary>
        /// <param name="flag">Acknowledge flag</param>
        /// <returns>true if successfully set</returns>
        bool RequireAcknowledgeNewMessage(bool flag);


        /// <summary>
        /// Get a list of phone book entries from the specified storage
        /// </summary>
        /// <param name="storage">Phone book storage</param>
        /// <returns>A list of phone book entries</returns>
        PhoneBookEntry[] GetPhoneBook(string storage);

        /// <summary>
        /// Get a list of phone book entries from the specified storage
        /// </summary>
        /// <param name="storage">Phone book storage enumeration. See <see cref="PhoneBookStorage"/></param>
        /// <returns>A list of phone book entries</returns>
        PhoneBookEntry[] GetPhoneBook(PhoneBookStorage storage);

        /// <summary>
        /// Retrieve phone book memory storage status
        /// </summary>
        /// <param name="storage">Phone book storage</param>
        /// <returns>Memory status</returns>
        MemoryStatusWithStorage GetPhoneBookMemoryStatus(string storage);


        /// <summary>
        /// Retrieve phone book memory storage status
        /// </summary>
        /// <param name="storage">Phone book storage enumeration. See <see cref="PhoneBookStorage"/></param>
        /// <returns>Memory status</returns>
        MemoryStatusWithStorage GetPhoneBookMemoryStatus(PhoneBookStorage storage);


        /// <summary>
        /// Export phone book entries to vCard
        /// </summary>
        /// <param name="phoneBookEntries">Phone book entries to be exported</param>
        /// <returns>An array of vCard instances</returns>
        vCard[] ExportPhoneBookTovCard(PhoneBookEntry[] phoneBookEntries);

        /// <summary>
        /// Get a list of supported network operators
        /// </summary>
        /// <returns>A list of network operators</returns>
        SupportedNetworkOperator[] GetSupportedNetworkOperators();

        /// <summary>
        /// Add phone book entry to the specified storage
        /// </summary>
        /// <param name="entry">Phone book entry</param>
        /// <param name="storage">Phone book storage</param>
        /// <returns>true if successful</returns>
        bool AddPhoneBookEntry(PhoneBookEntry entry, string storage);

        /// <summary>
        /// Add phone book entry to the specified storage
        /// </summary>
        /// <param name="entry">Phone book entry</param>
        /// <param name="storage">Phone book storage. See <see cref="PhoneBookStorage"/></param>
        /// <returns>true if successful</returns>
        bool AddPhoneBookEntry(PhoneBookEntry entry, PhoneBookStorage storage);

        /// <summary>
        /// Delete a specific phone book
        /// </summary>
        /// <param name="storage">Phone book storage</param>
        /// <returns>true if successful</returns>
        bool DeletePhoneBook(string storage);

        /// <summary>
        /// Delete a specific phone book
        /// </summary>
        /// <param name="storage">Phone book storage. See <see cref="PhoneBookStorage"/></param>
        /// <returns>true if successful</returns>
        bool DeletePhoneBook(PhoneBookStorage storage);

        /// <summary>
        /// Delete a specific phone book entry
        /// </summary>
        /// <param name="index">Phone book entry index</param>
        /// <param name="storage">Phone book storage</param>
        /// <returns>true if successful</returns>
        bool DeletePhoneBookEntry(int index, string storage);

        /// <summary>
        /// Delete a specific phone book entry
        /// </summary>
        /// <param name="index">Phone book entry index</param>
        /// <param name="storage">Phone book storage. See <see cref="PhoneBookStorage"/></param>
        /// <returns>true if successful</returns>
        bool DeletePhoneBookEntry(int index, PhoneBookStorage storage);

        /// <summary>
        /// Search for phone book entries from the specified storage
        /// </summary>
        /// <param name="searchText">Text to search for</param>
        /// <param name="storage">Phone book storage</param>
        /// <returns>true if successful</returns>
        PhoneBookEntry[] FindPhoneBookEntries(string searchText, string storage);

        /// <summary>
        /// Search for phone book entries from the specified storage
        /// </summary>
        /// <param name="searchText">Text to search for</param>
        /// <param name="storage">Phone book storage. See <see cref="PhoneBookStorage"/></param>
        /// <returns>true if successful</returns>
        PhoneBookEntry[] FindPhoneBookEntries(string searchText, PhoneBookStorage storage);

        /// <summary>
        /// Save message to gateway
        /// </summary>
        /// <param name="message">Message content</param>
        /// <param name="status">Message status</param>
        /// <returns>true if successful</returns>
        List<int> SaveMessage(Sms message, MessageStatusType status);

        /// <summary>
        /// Send USSD command to the network
        /// </summary>
        /// <param name="ussdCommand">USSD command</param>
        /// <returns>Execution result. If the result is empty, check the last exception for error</returns>
        string SendUssd(string ussdCommand);


        /// <summary>
        /// Sends the USSD command
        /// </summary>
        /// <param name="ussdCommand">The USSD command.</param>
        /// <param name="interactive">if set to <c>true</c> then the command is interactive</param>
        /// <returns>true if successful, false otherwise</returns>
        string SendUssd(string ussdCommand, bool interactive);

        /// <summary>
        /// Sends the USSD.
        /// </summary>
        /// <param name="ussdCommand">The USSD command.</param>
        /// <returns>USSD response</returns>
        UssdResponse SendUssd(UssdRequest ussdCommand);


        /// <summary>
        /// Sends the DTMF.
        /// </summary>
        /// <param name="tones">The DTMF tones.</param>
        /// <returns>true if successful, false otherwise</returns>
        bool SendDtmf(string tones);

        /// <summary>
        /// Sends the DTMF.
        /// </summary>
        /// <param name="tones">The DTMF tones.</param>
        /// <param name="duration">The duration in milliseconds. Set to 0 to use the default value.</param>
        /// <returns>true if successful, false otherwise</returns>
        bool SendDtmf(string tones, int duration);

        /// <summary>
        /// Cancels the USSD session.
        /// </summary>
        /// <returns>true if successful, false otherwise</returns>
        bool CancelUssdSession();


        /// <summary>
        /// Protocol for message sending. It can be either PDU or text mode
        /// </summary>
        /// <param name="protocol">Protocol. See <see cref="MessageProtocol"/></param>
        /// <returns>true if it is set successfully</returns>
        bool SetMessageProtocol(MessageProtocol protocol);


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


        /// <summary>
        /// Check if the gateway connection is still valid
        /// </summary>
        /// <returns>true if connection is valid. Otherwise returns false</returns>
        bool ValidateConnection();


        /// <summary>
        /// Initializes the data connection. 
        /// This is normally called one time for MMS service.
        /// <para>
        /// 
        /// </para>
        /// </summary>
        /// <returns>true </returns>
        bool InitializeDataConnection();


        /**** Not implemented yet *****/

        /// <summary>
        /// Get supported DTMF
        /// </summary>
        /// <returns>Supported DTMF</returns>
        //char[] GetSupportedDtmfs();

        /// <summary>
        /// Send DTMF
        /// </summary>
        /// <param name="dtmfString">DTMF string</param>
        /// <returns>true if successful</returns>
        //bool SendDtmf(string dtmfString);

        /*
        bool ExportPhoneBookToXml(PhoneBookEntry[] phoneBookEntries);
        bool ImportPhoneBookFromXml(string xmlFileName);
        bool ExportMessagesToXml(MessageInformation[] messages);
        bool ImportMessagesFromXml(string xmlFileName);
        */
        
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
        /// Message sending failed event
        /// </summary>
        event MessageErrorEventHandler MessageSendingFailed;

        /// <summary>
        /// Message received event
        /// </summary>
        event MessageReceivedEventHandler MessageReceived;

        /// <summary>
        /// USSD response received event
        /// </summary>
        event UssdResponseReceivedHandler UssdResponseReceived;

        /// <summary>
        /// Outgoing call received event
        /// </summary>
        event OutgoingCallEventHandler CallDialled;

        /// <summary>
        /// Incoming call received event
        /// </summary>
        event IncomingCallEventHandler CallReceived;
        
        /// <summary>
        /// Occurs when gateway is connected.
        /// </summary>
        //event ConnectedEventHandler GatewayConnected;

        /// <summary>
        /// Occurs when gateway is disconnected.
        /// </summary>
        event DisconnectedEventHandler GatewayDisconnected;
        
        /// <summary>
        /// Occurs when watch dog failed
        /// </summary>
        event WatchDogFailureEventHandler WatchDogFailed;

        #endregion =======================================================================================
    }
}
