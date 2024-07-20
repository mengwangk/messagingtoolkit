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

using MessagingToolkit.Core;
using MessagingToolkit.Core.Mobile.Event;

namespace MessagingToolkit.Core.Mobile
{
    #region ==================== Public Enum ===============================================================

    /// <summary>
    /// Baud rate for the communication port
    /// </summary>
    public enum PortBaudRate
    {
        /// <summary>
        /// Sets the serial port baud rate to 110 bits per second 
        /// </summary>
        BitsPerSecond110 = 110,

        /// <summary>
        /// Sets the serial port baud rate to 300 bits per second 
        /// </summary>
        BitsPerSecond300 = 300,

        /// <summary>
        /// Sets the serial port baud rate to 1200 bits per second 
        /// </summary>
        BitsPerSecond1200 = 1200,

        /// <summary>
        /// Sets the serial port baud rate to 2400 bits per second 
        /// </summary>
        BitsPerSecond2400 = 2400,

        /// <summary>
        /// Sets the serial port baud rate to 4800 bits per second 
        /// </summary>
        BitsPerSecond4800 = 4800,

        /// <summary>
        /// Sets the serial port baud rate to 9600 bits per second 
        /// </summary>
        BitsPerSecond9600 = 9600,

        /// <summary>
        /// Sets the serial port baud rate to 14400 bits per second 
        /// </summary>
        BitsPerSecond14400 = 14400,

        /// <summary>
        /// Sets the serial port baud rate to 19200 bits per second 
        /// </summary>
        BitsPerSecond19200 = 19200,

        /// <summary>
        /// Sets the serial port baud rate to 38400 bits per second 
        /// </summary>
        BitsPerSecond38400 = 38400,

        /// <summary>
        /// Sets the serial port baud rate to 57600 bits per second 
        /// </summary>
        BitsPerSecond57600 = 57600,

        /// <summary>
        /// Sets the serial port baud rate to 115200 bits per second 
        /// </summary>
        BitsPerSecond115200 = 115200,

        /// <summary>
        /// Sets the serial port baud rate to 230400 bits per second 
        /// </summary>
        BitsPerSecond230400 = 230400,

        /// <summary>
        /// Sets the serial port baud rate to 460800 bits per second 
        /// </summary>
        BitsPerSecond460800 = 460800,
        
        /// <summary>
        /// Sets the serial port baud rate to 921600 bits per second 
        /// </summary>
        BitsPerSecond921600 = 921600,

        /// <summary>
        /// Sets the serial port baud rate to 7200000 bits per second 
        /// </summary>
        BitsPerSecond7200000 = 7200000,
    }

   
    /// <summary>
    /// Data bits for the communication port
    /// </summary>
    public enum PortDataBits
    {
        /// <summary>
        /// 4 bits
        /// </summary>
        Four = 4,
        /// <summary>
        /// 5 bits
        /// </summary>
        Five = 5,
        /// <summary>   
        /// 6 bits
        /// </summary>
        Six = 6,
        /// <summary>
        /// 7 bits
        /// </summary>
        Seven = 7,
        /// <summary>
        /// 8 bits
        /// </summary>
        Eight = 8
    }

    /// <summary>
    /// Parity bit for the communication port
    /// </summary>
    public enum PortParity
    {
        /// <summary>
        /// No parity check occurs
        /// </summary>
        None = Parity.None,
        /// <summary>
        /// Sets the parity bit so that the count of bits set is an odd number
        /// </summary>
        Odd = Parity.Odd,
        /// <summary>
        /// Sets the parity bit so that the count of bits set is an even number
        /// </summary>
        Even = Parity.Even,
        /// <summary>
        /// Leaves the parity bit set to 1
        /// </summary>
        Mark = Parity.Mark,
        /// <summary>
        /// Leaves the parity bit set to 0
        /// </summary>
        Space = Parity.Space
    }

    /// <summary>
    /// Standard number of stopbits per byte
    /// </summary>
    public enum PortStopBits
    {
        /// <summary>
        /// One stop bit is used
        /// </summary>
        One = StopBits.One,
        /// <summary>
        /// 1.5 stop bits are used
        /// </summary>
        OnePointFive = StopBits.OnePointFive,
        /// <summary>
        /// Two stop bits are used
        /// </summary>
        Two = StopBits.Two,
        /// <summary>
        /// No stop bits are used
        /// </summary>
        None = StopBits.None
    }

    /// <summary>
    /// Handshaking protocol for serial port transmission of data
    /// </summary>
    public enum PortHandshake
    {
        /// <summary>
        /// No control is used for the handshake
        /// </summary>
        None = Handshake.None,
        /// <summary>
        /// Both the Request-to-Send (RTS) hardware control 
        /// and the XON/XOFF software controls are used
        /// </summary>
        RequestToSendXOnXOff = Handshake.RequestToSendXOnXOff,
        /// <summary>
        /// The XON/XOFF software control protocol is used. 
        /// The XOFF control is sent to stop the transmission of data. 
        /// The XON control is sent to resume the transmission. 
        /// These software controls are used instead of Request to Send (RTS) 
        /// and Clear to Send (CTS) hardware controls
        /// </summary>
        XOnXOff = Handshake.XOnXOff,
        /// <summary>
        /// Request-to-Send (RTS) hardware flow control is used. 
        /// RTS signals that data is available for transmission. 
        /// If the input buffer becomes full, the RTS line will 
        /// be set to false. The RTS line will be set to true when 
        /// more room becomes available in the input buffer.
        /// </summary>
        RequestToSend = Handshake.RequestToSend
    }

    /// <summary>
    /// Action for long message
    /// </summary>
    public enum MessageSplitOption
    {
        /// <summary>
        /// Truncate the message
        /// </summary>
        Truncate = 0,
        /// <summary>
        /// Do a simple split
        /// </summary>
        SimpleSplit = 1,
        /// <summary>
        /// Do a formatted split
        /// </summary>
        //FormattedSplit = 2,
        /// <summary>
        /// Concatenate the message
        /// </summary>
        Concatenate = 3,
    }

    /// <summary>
    /// Message memory location
    /// </summary>
    public enum MessageStorage
    {
        /// <summary>
        /// SIM memory
        /// </summary>
        [StringValue("SM")]
        Sim = 1,
        /// <summary>
        /// Phone memory
        /// </summary>
        [StringValue("ME")]
        Phone = 2,
        /// <summary>
        /// Phone and SIM memory
        /// </summary>
        [StringValue("MT")]
        MobileTerminating,      
        /// <summary>
        /// Phone and SIM memory
        /// </summary>
        [StringValue("MTSMMESR")]
        MtSmMesr,
        /// <summary>
        /// Phone and SIM memory
        /// </summary>
        [StringValue("SMME")]
        SmMe,
        /// <summary>
        /// Phone and SIM memory
        /// </summary>
        [StringValue("SMSR")]
        SmSr,
        /// <summary>
        /// Phone and SIM memory
        /// </summary>
        [StringValue("SMMESR")]
        SmMeSr,
        /// <summary>
        /// SR
        /// </summary>
        [StringValue("SR")]
        Sr
    }

    /// <summary>
    /// Phone book storage
    /// </summary>
    public enum PhoneBookStorage
    {
        /// <summary>
        /// SIM storage
        /// </summary>
        [StringValue("SM")]
        Sim,
        /// <summary>
        /// Phone storage
        /// </summary>
        [StringValue("ME")]
        Phone
    }


    /// <summary>
    /// Message type
    /// </summary>
    public enum MessageStatusType
    {
        /// <summary>
        /// Received and unread messages
        /// </summary>
        [StringValue("0")]
        ReceivedUnreadMessages,
        /// <summary>
        /// Received and read messages
        /// </summary>
        [StringValue("1")]
        ReceivedReadMessages,
        /// <summary>
        /// Stored unsent messages
        /// </summary>
        [StringValue("2")]
        StoredUnsentMessages,
        /// <summary>
        /// Stored sent messages 
        /// </summary>
        [StringValue("3")]
        StoredSentMessages,
        /// <summary>
        /// All messages
        /// </summary>
        [StringValue("4")]
        AllMessages
    }

    /// <summary>
    /// Message delete option
    /// </summary>
    public enum MessageDeleteOption
    {
        /// <summary>
        /// Delete the message by the message index
        /// </summary>
        [StringValue("0")]
        ByIndex = 0,
        /// <summary>
        /// Delete the preferred message store
        /// </summary>
        [StringValue("1")]
        PreferredMessageStore = 1,
        /// <summary>
        /// Delete the preferred message store and MO sent
        /// </summary>
        [StringValue("2")]
        PrefereredMessageStoreAndMoSent = 2,
        /// <summary>
        /// Delete the preferred message store and MO sent and unsent
        /// </summary>
        [StringValue("3")]
        PrefereredMessageStoreAndMoSentUnsent = 3,
        /// <summary>
        /// Delete all messages
        /// </summary>
        [StringValue("4")]
        AllMessages = 4
    }

    /// <summary>
    /// If it is a synchronous execution, then the
    /// thread is blocked until result is returned.
    /// If it is a asynchronous execution, then the thread
    /// is not blocked. 
    /// </summary>
    internal enum ExecutionBehavior
    {
        /// <summary>
        /// Synchronous execution
        /// </summary>
        Synchronous = 0,

        /// <summary>
        /// Asynchronous execution
        /// </summary>
        Asynchronous = 1
    }


    /// <summary>
    /// Response expected from the gateway
    /// </summary>
    internal enum Response
    {
        /// <summary>
        /// No respons expected, just return the result
        /// </summary>
        [StringValue("")]
        None,
        /// <summary>
        /// Error response
        /// </summary>
        [StringValue("ERROR\r\n")]
        Error,
        /// <summary>
        /// Successful response
        /// </summary>
        [StringValue(@"\bOK\b")]
        Ok,
        /// <summary>
        /// No carrier response
        /// </summary>
        [StringValue("NO CARRIER")]
        NoCarrier,

        /// <summary>
        /// CMS error response 
        /// </summary>
        /// <code>
        /// Error Code	Meaning 
        /// 0-127 		GSM 04.11 Annex E-2 values 
        /// 128-255 	GSM 03.40 section 9.2.3.22 values 
        /// 300 		Phone failure 
        /// 301 		SMS service of phone reserved 
        /// 302 		Operation not allowed 
        /// 303 		Operation not supported 
        /// 304 		Invalid PDU mode parameter 
        /// 305 		Invalid text mode parameter 
        /// 310 		SIM not inserted 
        /// 311 		SIM PIN necessary 
        /// 312 		PH-SIM PIN necessary 
        /// 313 		SIM failure 
        /// 314 		SIM busy 
        /// 315 		SIM wrong 
        /// 320 		Memory failure 
        /// 321 		Invalid memory index 
        /// 322 		Memory full 
        /// 330 		SMSC (message service center) address unknown 
        /// 331 		No network service 
        /// 332 		Network timeout 
        /// 500 		Unknown error 
        /// 512 		Manufacturer specific 
        ///</code>    
        [StringValue("\\+CMS ERROR: (\\d+)\r\n")]
        CmsError,
        /// <summary>
        /// CME error response
        /// </summary>
        [StringValue("\\+CME ERROR: (\\d+)\r\n")]
        CmeError,
        /// <summary>
        /// Response for SIM PIN required
        /// </summary>
        [StringValue("SIM PIN")]
        SimPinRequired,
        /// <summary>
        /// Response for SIM PIN required
        /// </summary>
        [StringValue("SIM PIN2")]
        SimPin2Required,
        /// <summary>
        /// Response for SIM busy
        /// </summary>
        [StringValue("BUSY")]
        Busy,
        /// <summary>
        /// Outgoing call
        /// </summary>
        [StringValue("RING")]
        Ring,
        /// <summary>
        /// Outgoing call
        /// </summary>
        [StringValue("+CLIP:")]
        IncomingCall,
        /// <summary>
        /// Incoming message
        /// </summary>
        [StringValue("+CMTI:")]
        IncomingMessageIndication,
        /// <summary>
        /// Incoming message
        /// </summary>
        [StringValue("+CMT:")]
        IncomingMessage,
        /// <summary>
        /// Incoming status report
        /// </summary>
        [StringValue("+CDSI:")]
        IncomingStatusReportIndication,
        /// <summary>
        /// Incoming status report
        /// </summary>
        [StringValue("+CDS:")]
        IncomingStatusReport,
        /// <summary>
        /// Ready response
        /// </summary>
        [StringValue("READY")]
        Ready,
        /// <summary>
        /// Command not supported response
        /// </summary>
        [StringValue("NOT SUPPORTED")]
        NotSupported
    }


    /// <summary>
    /// Possible error codes whenever there is a CMS (GSM network related) error
    /// received from the mobile gateway
    ///
    /// <code>
    /// CMS ERROR: 1	 Unassigned number 
    /// CMS ERROR: 8	 Operator determined barring
    /// CMS ERROR: 10	 Call bared
    /// CMS ERROR: 21	 Short message transfer rejected
    /// CMS ERROR: 27	 Destination out of service
    /// CMS ERROR: 28	 Unindentified subscriber
    /// CMS ERROR: 29	 Facility rejected
    /// CMS ERROR: 30	 Unknown subscriber
    /// CMS ERROR: 38	 Network out of order
    /// CMS ERROR: 41	 Temporary failure
    /// CMS ERROR: 42	 Congestion
    /// CMS ERROR: 47	 Recources unavailable
    /// CMS ERROR: 50	 Requested facility not subscribed
    /// CMS ERROR: 69	 Requested facility not implemented
    /// CMS ERROR: 81	 Invalid short message transfer reference value
    /// CMS ERROR: 95	 Invalid message unspecified
    /// CMS ERROR: 96	 Invalid mandatory information
    /// CMS ERROR: 97	 Message type non existent or not implemented
    /// CMS ERROR: 98	 Message not compatible with short message protocol
    /// CMS ERROR: 99	 Information element non-existent or not implemente
    /// CMS ERROR: 111	 Protocol error, unspecified
    /// CMS ERROR: 127	 Internetworking , unspecified
    /// CMS ERROR: 128	 Telematic internetworking not supported
    /// CMS ERROR: 129	 Short message type 0 not supported
    /// CMS ERROR: 130	 Cannot replace short message
    /// CMS ERROR: 143	 Unspecified TP-PID error
    /// CMS ERROR: 144	 Data code scheme not supported
    /// CMS ERROR: 145	 Message class not supported
    /// CMS ERROR: 159	 Unspecified TP-DCS error
    /// CMS ERROR: 160	 Command cannot be actioned
    /// CMS ERROR: 161	 Command unsupported
    /// CMS ERROR: 175	 Unspecified TP-Command error
    /// CMS ERROR: 176	 TPDU not supported
    /// CMS ERROR: 192	 SC busy
    /// CMS ERROR: 193	 No SC subscription
    /// CMS ERROR: 194	 SC System failure
    /// CMS ERROR: 195	 Invalid SME address
    /// CMS ERROR: 196	 Destination SME barred
    /// CMS ERROR: 197	 SM Rejected-Duplicate SM
    /// CMS ERROR: 198	 TP-VPF not supported
    /// CMS ERROR: 199	 TP-VP not supported
    /// CMS ERROR: 208	 D0 SIM SMS Storage full
    /// CMS ERROR: 209	 No SMS Storage capability in SIM
    /// CMS ERROR: 210	 Error in MS
    /// CMS ERROR: 211	 Memory capacity exceeded
    /// CMS ERROR: 212	 Sim application toolkit busy
    /// CMS ERROR: 213	 SIM data download error
    /// CMS ERROR: 255	 Unspecified error cause
    /// CMS ERROR: 300	 ME Failure
    /// CMS ERROR: 301	 SMS service of ME reserved
    /// CMS ERROR: 302	 Operation not allowed
    /// CMS ERROR: 303	 Operation not supported
    /// CMS ERROR: 304	 Invalid PDU mode parameter
    /// CMS ERROR: 305	 Invalid Text mode parameter
    /// CMS ERROR: 310	 SIM not inserted
    /// CMS ERROR: 311	 SIM PIN required
    /// CMS ERROR: 312	 PH-SIM PIN required
    /// CMS ERROR: 313	 SIM failure
    /// CMS ERROR: 314	 SIM busy
    /// CMS ERROR: 315	 SIM wrong
    /// CMS ERROR: 316	 SIM PUK required
    /// CMS ERROR: 317	 SIM PIN2 required
    /// CMS ERROR: 318	 SIM PUK2 required
    /// CMS ERROR: 320	 Memory failure
    /// CMS ERROR: 321	 Invalid memory index
    /// CMS ERROR: 322	 Memory full
    /// CMS ERROR: 330	 SMSC address unknown
    /// CMS ERROR: 331	 No network service
    /// CMS ERROR: 332	 Network timeout
    /// CMS ERROR: 340	 No +CNMA expected
    /// CMS ERROR: 500	 Unknown error
    /// CMS ERROR: 512	 User abort
    /// CMS ERROR: 513	 Unable to store
    /// CMS ERROR: 514	 Invalid Status
    /// CMS ERROR: 515	 Device busy or Invalid Character in string
    /// CMS ERROR: 516	 Invalid length
    /// CMS ERROR: 517	 Invalid character in PDU
    /// CMS ERROR: 518	 Invalid parameter
    /// CMS ERROR: 519	 Invalid length or character
    /// CMS ERROR: 520	 Invalid character in text
    /// CMS ERROR: 521	 Timer expired
    /// CMS ERROR: 522	 Operation temporary not allowed
    /// CMS ERROR: 532	 SIM not ready
    /// CMS ERROR: 534	 Cell Broadcast error unknown
    /// CMS ERROR: 535	 Protocol stack busy
    /// CMS ERROR: 538	 Invalid parameter
    /// </code>
    /// </summary>    
    public enum CmsErrorCode
    {
        [StringValue("CMS ERROR: 1	 Unassigned number")]
        _1,
        [StringValue("CMS ERROR: 8	 Operator determined barring")]
        _8,
        [StringValue("CMS ERROR: 10	 Call barred")]
        _10,
        [StringValue("CMS ERROR: 21	 Short message transfer rejected")]
        _21,
        [StringValue("CMS ERROR: 27	 Destination out of service")]
        _27,
        [StringValue("CMS ERROR: 28	 Unindentified subscriber")]
        _28,
        [StringValue("CMS ERROR: 29	 Facility rejected")]
        _29,
        [StringValue("CMS ERROR: 30	 Unknown subscriber")]
        _30,
        [StringValue("CMS ERROR: 38	 Network out of order")]
        _38,
        [StringValue("CMS ERROR: 41	 Temporary failure")]
        _41,
        [StringValue("CMS ERROR: 42	 Congestion")]
        _42,
        [StringValue("CMS ERROR: 47	 Recources unavailable")]
        _47,
        [StringValue("CMS ERROR: 50	 Requested facility not subscribed")]
        _50,
        [StringValue("CMS ERROR: 69	 Requested facility not implemented")]
        _69,
        [StringValue("CMS ERROR: 81	 Invalid short message transfer reference value")]
        _81,
        [StringValue("CMS ERROR: 95	 Invalid message unspecified")]
        _95,
        [StringValue("CMS ERROR: 96	 Invalid mandatory information")]
        _96,
        [StringValue("CMS ERROR: 97	 Message type non existent or not implemented")]
        _97,
        [StringValue("CMS ERROR: 98	 Message not compatible with short message protocol")]
        _98,
        [StringValue("CMS ERROR: 99	 Information element non-existent or not implemented")]
        _99,
        [StringValue("CMS ERROR: 111	 Protocol error, unspecified")]
        _111,
        [StringValue("CMS ERROR: 127	 Internetworking , unspecified")]
        _127,
        [StringValue("CMS ERROR: 128	 Telematic internetworking not supported")]
        _128,
        [StringValue("CMS ERROR: 129	 Short message type 0 not supported")]
        _129,
        [StringValue("CMS ERROR: 130	 Cannot replace short message")]
        _130,
        [StringValue("CMS ERROR: 143	 Unspecified TP-PID error")]
        _143,
        [StringValue("CMS ERROR: 144	 Data code scheme not supported")]
        _144,
        [StringValue("CMS ERROR: 145	 Message class not supported")]
        _145,
        [StringValue("CMS ERROR: 159	 Unspecified TP-DCS error")]
        _159,
        [StringValue("CMS ERROR: 160	 Command cannot be actioned")]
        _160,
        [StringValue("CMS ERROR: 161	 Command unsupported")]
        _161,
        [StringValue("CMS ERROR: 175	 Unspecified TP-Command error")]
        _175,
        [StringValue("CMS ERROR: 176	 TPDU not supported")]
        _176,
        [StringValue("CMS ERROR: 192	 SC busy")]
        _192,
        [StringValue("CMS ERROR: 193	 No SC subscription")]
        _193,
        [StringValue("CMS ERROR: 194	 SC System failure")]
        _194,
        [StringValue("CMS ERROR: 195	 Invalid SME address")]
        _195,
        [StringValue("CMS ERROR: 196	 Destination SME barred")]
        _196,
        [StringValue("CMS ERROR: 197	 SM Rejected-Duplicate SM")]
        _197,
        [StringValue("CMS ERROR: 198	 TP-VPF not supported")]
        _198,
        [StringValue("CMS ERROR: 199	 TP-VP not supported")]
        _199,
        [StringValue("CMS ERROR: 208	 D0 SIM SMS Storage full")]
        _208,
        [StringValue("CMS ERROR: 209	 No SMS Storage capability in SIM")]
        _209,
        [StringValue("CMS ERROR: 210	 Error in MS")]
        _210,
        [StringValue("CMS ERROR: 211	 Memory capacity exceeded")]
        _211,
        [StringValue("CMS ERROR: 212	 Sim application toolkit busy")]
        _212,
        [StringValue("CMS ERROR: 213	 SIM data download error")]
        _213,
        [StringValue("CMS ERROR: 255	 Unspecified error cause")]
        _255,
        [StringValue("CMS ERROR: 300	 ME Failure")]
        _300,
        [StringValue("CMS ERROR: 301	 SMS service of ME reserved")]
        _301,
        [StringValue("CMS ERROR: 302	 Operation not allowed")]
        _302,
        [StringValue("CMS ERROR: 303	 Operation not supported")]
        _303,
        [StringValue("CMS ERROR: 304	 Invalid PDU mode parameter")]
        _304,
        [StringValue("CMS ERROR: 305	 Invalid Text mode parameter")]
        _305,
        [StringValue("CMS ERROR: 310	 SIM not inserted")]
        _310,
        [StringValue("CMS ERROR: 311	 SIM PIN required")]
        _311,
        [StringValue("CMS ERROR: 312	 PH-SIM PIN required")]
        _312,
        [StringValue("CMS ERROR: 313	 SIM failure")]
        _313,
        [StringValue("CMS ERROR: 314	 SIM busy")]
        _314,
        [StringValue("CMS ERROR: 315	 SIM wrong")]
        _315,
        [StringValue("CMS ERROR: 316	 SIM PUK required")]
        _316,
        [StringValue("CMS ERROR: 317	 SIM PIN2 required")]
        _317,
        [StringValue("CMS ERROR: 318	 SIM PUK2 required")]
        _318,
        [StringValue("CMS ERROR: 320	 Memory failure")]
        _320,
        [StringValue("CMS ERROR: 321	 Invalid memory index")]
        _321,
        [StringValue("CMS ERROR: 322	 Memory full")]
        _322,
        [StringValue("CMS ERROR: 330	 SMSC address unknown")]
        _330,
        [StringValue("CMS ERROR: 331	 No network service")]
        _331,
        [StringValue("CMS ERROR: 332	 Network timeout")]
        _332,
        [StringValue("CMS ERROR: 340	 No +CNMA expected")]
        _340,
        [StringValue("CMS ERROR: 500	 Unknown error")]
        _500,
        [StringValue("CMS ERROR: 512	 User abort")]
        _512,
        [StringValue("CMS ERROR: 513	 Unable to store")]
        _513,
        [StringValue("CMS ERROR: 514	 Invalid Status")]
        _514,
        [StringValue("CMS ERROR: 515	 Device busy or Invalid Character in string")]
        _515,
        [StringValue("CMS ERROR: 516	 Invalid length")]
        _516,
        [StringValue("CMS ERROR: 517	 Invalid character in PDU")]
        _517,
        [StringValue("CMS ERROR: 518	 Invalid parameter")]
        _518,
        [StringValue("CMS ERROR: 519	 Invalid length or character")]
        _519,
        [StringValue("CMS ERROR: 520	 Invalid character in text")]
        _520,
        [StringValue("CMS ERROR: 521	 Timer expired")]
        _521,
        [StringValue("CMS ERROR: 522	 Operation temporary not allowed")]
        _522,
        [StringValue("CMS ERROR: 532	 SIM not ready")]
        _532,
        [StringValue("CMS ERROR: 534	 Cell Broadcast error unknown")]
        _534,
        [StringValue("CMS ERROR: 535	 Protocol stack busy")]
        _535,
        [StringValue("CMS ERROR: 538	 Invalid parameter")]
        _538
    }

    /// <summary>
    /// Possible error code when there is a CME (GSM equipment related) error
    /// received from the phone
    /// 
    /// <code>
    /// CME ERROR: 0	Phone failure   
    /// CME ERROR: 1	No connection to phone
    /// CME ERROR: 2	Phone adapter link reserved
    /// CME ERROR: 3	Operation not allowed
    /// CME ERROR: 4	Operation not supported
    /// CME ERROR: 5	PH_SIM PIN required
    /// CME ERROR: 6	PH_FSIM PIN required
    /// CME ERROR: 7	PH_FSIM PUK required
    /// CME ERROR: 10	SIM not inserted
    /// CME ERROR: 11	SIM PIN required
    /// CME ERROR: 12	SIM PUK required
    /// CME ERROR: 13	SIM failure
    /// CME ERROR: 14	SIM busy
    /// CME ERROR: 15	SIM wrong
    /// CME ERROR: 16	Incorrect password
    /// CME ERROR: 17	SIM PIN2 required
    /// CME ERROR: 18	SIM PUK2 required
    /// CME ERROR: 20	Memory full
    /// CME ERROR: 21	Invalid index
    /// CME ERROR: 22	Not found
    /// CME ERROR: 23	Memory failure
    /// CME ERROR: 24	Text string too long
    /// CME ERROR: 25	Invalid characters in text string
    /// CME ERROR: 26	Dial string too long
    /// CME ERROR: 27	Invalid characters in dial string
    /// CME ERROR: 30	No network service
    /// CME ERROR: 31	Network timeout
    /// CME ERROR: 32	Network not allowed, emergency calls only
    /// CME ERROR: 40	Network personalization PIN required
    /// CME ERROR: 41	Network personalization PUK required
    /// CME ERROR: 42	Network subset personalization PIN required
    /// CME ERROR: 43	Network subset personalization PUK required
    /// CME ERROR: 44	Service provider personalization PIN required
    /// CME ERROR: 45	Service provider personalization PUK required
    /// CME ERROR: 46	Corporate personalization PIN required
    /// CME ERROR: 47	Corporate personalization PUK required
    /// CME ERROR: 48	PH-SIM PUK required
    /// CME ERROR: 100	Unknown error
    /// CME ERROR: 103	Illegal MS
    /// CME ERROR: 106	Illegal ME
    /// CME ERROR: 107	GPRS services not allowed
    /// CME ERROR: 111	PLMN not allowed
    /// CME ERROR: 112	Location area not allowed
    /// CME ERROR: 113	Roaming not allowed in this location area
    /// CME ERROR: 126	Operation temporary not allowed
    /// CME ERROR: 132	Service operation not supported
    /// CME ERROR: 133	Requested service option not subscribed
    /// CME ERROR: 134	Service option temporary out of order
    /// CME ERROR: 148	Unspecified GPRS error
    /// CME ERROR: 149	PDP authentication failure
    /// CME ERROR: 150	Invalid mobile class
    /// CME ERROR: 256	Operation temporarily not allowed
    /// CME ERROR: 257	Call barred
    /// CME ERROR: 258	Phone is busy
    /// CME ERROR: 259	User abort
    /// CME ERROR: 260	Invalid dial string
    /// CME ERROR: 261	SS not executed
    /// CME ERROR: 262	SIM Blocked
    /// CME ERROR: 263	Invalid block
    /// CME ERROR: 772	SIM powered down
    /// </code> 
    /// </summary>
    public enum CmeErrorCode
    {
        [StringValue("CME ERROR: 0	Phone failure")]
        _0,
        [StringValue("CME ERROR: 1	No connection to phone")]
        _1,
        [StringValue("CME ERROR: 2	Phone adapter link reserved")]
        _2,
        [StringValue("CME ERROR: 3	Operation not allowed")]
        _3,
        [StringValue("CME ERROR: 4	Operation not supported")]
        _4,
        [StringValue("CME ERROR: 5	PH_SIM PIN required")]
        _5,
        [StringValue("CME ERROR: 6	PH_FSIM PIN required")]
        _6,
        [StringValue("CME ERROR: 7	PH_FSIM PUK required")]
        _7,
        [StringValue("CME ERROR: 10	SIM not inserted")]
        _10,
        [StringValue("CME ERROR: 11	SIM PIN required")]
        _11,
        [StringValue("CME ERROR: 12	SIM PUK required")]
        _12,
        [StringValue("CME ERROR: 13	SIM failure")]
        _13,
        [StringValue("CME ERROR: 14	SIM busy")]
        _14,
        [StringValue("CME ERROR: 15	SIM wrong")]
        _15,
        [StringValue("CME ERROR: 16	Incorrect password")]
        _16,
        [StringValue("CME ERROR: 17	SIM PIN2 required")]
        _17,
        [StringValue("CME ERROR: 18	SIM PUK2 required")]
        _18,
        [StringValue("CME ERROR: 20	Memory full")]
        _20,
        [StringValue("CME ERROR: 21	Invalid index")]
        _21,
        [StringValue("CME ERROR: 22	Not found")]
        _22,
        [StringValue("CME ERROR: 23	Memory failure")]
        _23,
        [StringValue("CME ERROR: 24	Text string too long")]
        _24,
        [StringValue("CME ERROR: 25	Invalid characters in text string")]
        _25,
        [StringValue("CME ERROR: 26	Dial string too long")]
        _26,
        [StringValue("CME ERROR: 27	Invalid characters in dial string")]
        _27,
        [StringValue("CME ERROR: 30	No network service")]
        _30,
        [StringValue("CME ERROR: 31	Network timeout")]
        _31,
        [StringValue("CME ERROR: 32	Network not allowed, emergency calls only")]
        _32,
        [StringValue("CME ERROR: 40	Network personalization PIN required")]
        _40,
        [StringValue("CME ERROR: 41	Network personalization PUK required")]
        _41,
        [StringValue("CME ERROR: 42	Network subset personalization PIN required")]
        _42,
        [StringValue("CME ERROR: 43	Network subset personalization PUK required")]
        _43,
        [StringValue("CME ERROR: 44	Service provider personalization PIN required")]
        _44,
        [StringValue("CME ERROR: 45	Service provider personalization PUK required")]
        _45,
        [StringValue("CME ERROR: 46	Corporate personalization PIN required")]
        _46,
        [StringValue("CME ERROR: 47	Corporate personalization PUK required")]
        _47,
        [StringValue("CME ERROR: 48	PH-SIM PUK required")]
        _48,
        [StringValue("CME ERROR: 100	Unknown error")]
        _100,
        [StringValue("CME ERROR: 103	Illegal MS")]
        _103,
        [StringValue("CME ERROR: 106	Illegal ME")]
        _106,
        [StringValue("CME ERROR: 107	GPRS services not allowed")]
        _107,
        [StringValue("CME ERROR: 111	PLMN not allowed")]
        _111,
        [StringValue("CME ERROR: 112	Location area not allowed")]
        _112,
        [StringValue("CME ERROR: 113	Roaming not allowed in this location area")]
        _113,
        [StringValue("CME ERROR: 126	Operation temporary not allowed")]
        _126,
        [StringValue("CME ERROR: 132	Service operation not supported")]
        _132,
        [StringValue("CME ERROR: 133	Requested service option not subscribed")]
        _133,
        [StringValue("CME ERROR: 134	Service option temporary out of order")]
        _134,
        [StringValue("CME ERROR: 148	Unspecified GPRS error")]
        _148,
        [StringValue("CME ERROR: 149	PDP authentication failure")]
        _149,
        [StringValue("CME ERROR: 150	Invalid mobile class")]
        _150,
        [StringValue("CME ERROR: 256	Operation temporarily not allowed")]
        _256,
        [StringValue("CME ERROR: 257	Call barred")]
        _257,
        [StringValue("CME ERROR: 258	Phone is busy")]
        _258,
        [StringValue("CME ERROR: 259	User abort")]
        _259,
        [StringValue("CME ERROR: 260	Invalid dial string")]
        _260,
        [StringValue("CME ERROR: 261	SS not executed")]
        _261,
        [StringValue("CME ERROR: 262	SIM Blocked")]
        _262,
        [StringValue("CME ERROR: 263	Invalid block")]
        _263,
        [StringValue("CME ERROR: 772	SIM powered down")]
        _772
    }

    /// <summary>
    /// Message validity period format
    /// </summary>
    public enum MessageValidityPeriodFormat
    {
        /// <summary>
        /// Relative format. b4=1 b3=0
        /// </summary>
        RelativeFormat = 16
    }


    /// <summary>
    /// Message status report request
    /// </summary>
    public enum MessageStatusReportRequest
    {
        /// <summary>
        /// SMS report request. Value = 00100000 = 0x20
        /// </summary>
        SmsReportRequest = 32,
        /// <summary>
        /// No SMS report request. Value = 00000000 = 0x00
        /// </summary>
        NoSmsReportRequest = 0
    }

    /// <summary>
    /// Message validity period
    /// </summary>
    public enum MessageValidPeriod
    {
        /// <summary>
        /// 1 hour
        /// </summary>
        OneHour = 1,
        /// <summary>
        /// 3 hours
        /// </summary>
        ThreeHours = 3,
        /// <summary>
        /// 6 hours
        /// </summary>
        SixHours = 6,
        /// <summary>
        /// 12 hours 
        /// </summary>
        TwelveHours = 12,
        /// <summary>
        /// 1 day
        /// </summary>
        OneDay = 24,
        /// <summary>
        /// 3 days
        /// </summary>
        ThreeDays = 72,
        /// <summary>
        /// 1 week
        /// </summary>
        OneWeek = 168,
        /// <summary>
        /// Maximum valid period
        /// </summary>
        Maximum = 255
    }

    /// <summary>
    /// Message type indicator
    /// </summary>
    public enum MessageTypeIndicator
    {
        /// <summary>
        /// Delivered SMS
        /// </summary>
        [StringValue("Received SMS")]
        MtiSmsDeliver = 0,
        /// <summary>
        /// Submitted SMS
        /// </summary>
        [StringValue("Submitted SMS")]
        MtiSmsSubmit = 1,
        /// <summary>
        /// SMS status report
        /// </summary>
        [StringValue("SMS Status Report")]
        MtiSmsStatusReport = 2,
        /// <summary>
        /// SMS command
        /// </summary>
        [StringValue("SMS Command")]
        MtiSmsCommand = 2,
        /// <summary>
        /// EMS received
        /// </summary>
        [StringValue("Received EMS")]
        MtiEmsReceived = 64,
        /// <summary>
        /// Submitted EMS 
        /// </summary>
        [StringValue("Submitted EMS")]
        MtiEmsSubmit = 65
    }

    /// <summary>
    /// Message content type indicator
    /// </summary>
    public enum MessageContentType
    {
        /// <summary>
        /// Normal message
        /// </summary>
        Normal,
        /// <summary>
        /// vCard
        /// </summary>
        vCard,
        /// <summary>
        /// vCalendar
        /// </summary>
        vCalendar,
        /// <summary>
        /// Wappush
        /// </summary>
        Wappush,
        /// <summary>
        /// MMS notification
        /// </summary>
        MmsNotification        
    }


    /// <summary>
    /// Message reply path
    /// </summary>
    public enum MessageReplyPath
    {
        /// <summary>
        /// No reply path
        /// </summary>
        NoReplyPath = 0,
        /// <summary>
        /// Reply path
        /// </summary>
        ReplyPath = 128
    }


    /// <summary>
    /// Message user data header indication
    /// </summary>
    public enum MessageDataHeaderIndication
    {
        /// <summary>
        /// User data header indication 
        /// </summary>
        UserDataHeaderIndication = 64,
        /// <summary>
        /// No user data header indication
        /// </summary>
        NoUserDataheaderIndication = 0
    }


    /// <summary>
    /// Message data coding scheme
    /// </summary>
    public enum MessageDataCodingScheme
    {
        /// <summary>
        /// Data coding scheme not defined
        /// </summary>
        Undefined = -1,
        /// <summary>
        /// 0x00 = default alphabet is 7 bit ASCII
        /// </summary>
        DefaultAlphabet = 0,
        /// <summary>
        /// 11110010 = 0xF2 = Class2 SIM specific, 7 bit data coding in User Data
        /// </summary>
        SevenBits = 3,
        /// <summary>
        /// 11110110 = 0xF6 = Class2 SIM specific, 8 bit data coding in User Data
        /// </summary>
        EightBits = 4,
        /// <summary>
        /// 0X08 = 16bit Unicode UCS2 coding 
        /// </summary>
        Ucs2 = 8,
        /// <summary>
        /// 11110000 = 0xF0 = Class0 immediate display, 7 bit data coding in User Data
        /// </summary>
        Class0Ud7Bits = 240,   
        /// <summary>
        /// 11110001 = 0xF1 = Class1 ME (Mobile Equipment) specific, 7 bit data coding in User Data
        /// </summary>
        Class1Ud7Bits = 241,
        /// <summary>
        /// 11110010 = 0xF2 = Class2 SIM specific, 7 bit data coding in User Data
        /// </summary>
        Class2Ud7Bits = 242,
        /// <summary>
        /// 11110011= 0xF3 = Class3 TE (Terminate Equipment) specific, 7 bit data coding in User Data
        /// </summary>
        Class3Ud7Bits = 243,
        /// <summary>
        /// 11110100 = 0xF4 = Class0 immediate display, 8 bit data coding in User Data
        /// </summary>
        Class0Ud8Bits = 244,
        /// <summary>
        /// 11110101 = 0xF5 = Class1 ME (Mobile Equipment) specific, 8 bit data coding in User Data
        /// </summary>
        Class1Ud8Bits = 245,
        /// <summary>
        /// 11110110 = 0xF6 = Class2 SIM specific, 8 bit data coding in User Data
        /// </summary>
        Class2Ud8Bits = 246,
        /// <summary>
        /// 11110111= 0xF7 = Class3 TE (Terminate Equipment) specific, 8 bit data coding in User Data
        /// </summary>
        Class3Ud8Bits = 247,   
        /// <summary>
        /// Custom data coding
        /// </summary>
        Custom = 255
    }      

    /// <summary>
    /// Message status report status 
    /// 
    /// <code>
    /// 0x00	Short message delivered successfully
    /// 0x01	Forwarded, but status unknown
    /// 0x02	Replaced
    /// 0x20	Congestion, still trying
    /// 0x21	Recipient busy, still trying
    /// 0x22	No response recipient, still trying
    /// 0x23	Service rejected, still trying
    /// 0x24	QOS not available, still trying
    /// 0x25	Recipient error, still trying
    /// 0x40	RPC Error
    /// 0x41	Incompatible destination
    /// 0x42	Connection rejected
    /// 0x43	Not obtainable
    /// 0x44	QOS not available
    /// 0x45	No internetworking available
    /// 0x46	Message expired
    /// 0x47	Message deleted by sender
    /// 0x48	Message deleted by SMSC
    /// 0x49	Does not exist     
    /// </code>
    /// </summary>
    public enum MessageStatusReportStatus
    {
        /// <summary>
        /// Unknown status
        /// </summary>
        UnknownStatus = -1,
        /// <summary>
        /// Success
        /// </summary>
        Success = 0,
        /// <summary>
        /// Forwarded but status is unknown
        /// </summary>
        ForwardedStatusUnknown = 1,
        /// <summary>
        /// Replaced
        /// </summary>
        Replaced = 2,
        /// <summary>
        /// Retry due to congestion
        /// </summary>
        RetryCongestion = 32,
        /// <summary>
        /// Retry due to recipient busy
        /// </summary>
        RetryRecipientBusy = 33,
        /// <summary>
        /// Retry due to no response from recipient
        /// </summary>
        RetryNoResponseFromRecipient = 34,
        /// <summary>
        /// Retry due to service is rejected
        /// </summary>
        RetryServiceRejected = 35,
        /// <summary>
        /// Retry as QOS is not available
        /// </summary>
        RetryQosNotAvailable = 36,
        /// <summary>
        /// Retry due to recipient error
        /// </summary>
        RetryRecipientError = 37,       
        /// <summary>
        /// RPC error. Message is not send. Sending is stopped
        /// </summary>
        RpcError = 64,
        /// <summary>
        /// Incompatible destination number
        /// </summary>
        IncompatibleDestination = 65,
        /// <summary>
        /// Connection is rejected
        /// </summary>
        ConnectionRejected  =66,
        /// <summary>
        /// Not obtainable
        /// </summary>
        NotObtainable = 67,
        /// <summary>
        /// QOS is not available
        /// </summary>
        QosNotAvailable = 68,
        /// <summary>
        /// No inter-networking
        /// </summary>
        NoInternetWorking = 69,
        /// <summary>
        /// Message is expired
        /// </summary>
        MessageExpired  = 70,
        /// <summary>
        /// Message is deleted by sender
        /// </summary>
        MessageDeletedBySender = 71,
        /// <summary>
        /// Message is deleted by SMSC
        /// </summary>
        MessageDeletedBySmsc = 72,
        /// <summary>
        /// Does not exist
        /// </summary>
        DoesNotExist = 73,
        /// <summary>
        /// Not sent
        /// </summary>
        NotSendAbort = 96,
        /// <summary>
        /// No response from Short Message Entity
        /// </summary>
        NoResponseFromSme = 98
    }


    /// <summary>
    /// Message sending protocol
    /// </summary>
    /// <remarks>
    /// Depending on your gateway, it may support none, 1 only or both.
    /// It is set using the AT+CMGF command
    /// </remarks>
    public enum MessageProtocol
    {
        /// <summary>
        /// PDU mode. Value = 0
        /// </summary>
        PduMode = 0,
        /// <summary>
        /// Text mode. Value = 1
        /// </summary>
        TextMode = 1
    }


    /// <summary>
    /// Allowed values of the action attribute of the indication tag
    /// </summary>
    public enum ServiceIndicationAction
    {
        /// <summary>
        /// Not set
        /// </summary>
        NotSet,
        /// <summary>
        /// No signal
        /// </summary>
        SignalNone,
        /// <summary>
        /// Low signal
        /// </summary>
        SignalLow,
        /// <summary>
        /// Medium signal
        /// </summary>
        SignalMedium,
        /// <summary>
        /// High signal
        /// </summary>
        SignalHigh,
        /// <summary>
        /// Delete
        /// </summary>
        Delete
    }

    /// <summary>
    /// Address type
    /// </summary>
    public enum NumberType
    {
        /// <summary>
        /// Domestic number
        /// </summary>
        [StringValue("129")]
        Domestic,
        /// <summary>
        /// National numer
        /// </summary>
        [StringValue("131")]
        National,
        /// <summary>
        /// International number
        /// </summary>
        [StringValue("145")]
        International
    }

    /// <summary>
    /// Network registration status
    /// </summary>
    public enum NetworkRegistrationStatus
    {       
        /// <summary>
        /// Not registered
        /// </summary>
        [StringValue("0")]
        NotRegistered,
        /// <summary>
        /// Registered to home network
        /// </summary>
        [StringValue("1")]
        RegisteredHomeNetwork,
        /// <summary>
        /// Still searching for operator
        /// </summary>
        [StringValue("2")]
        SearchingNewOperator,
        /// <summary>
        /// Registration is denied
        /// </summary>
        [StringValue("3")]
        RegistrationDenied,
        /// <summary>
        /// Unknown error
        /// </summary>
        [StringValue("4")]
        Unknown,
        /// <summary>
        /// Registered to roaming network
        /// </summary>
        [StringValue("5")]
        RegisteredRoamingNetwork        
    }

    /// <summary>
    /// Network capability
    /// </summary>
    public enum NetworkCapability
    {
        /// <summary>
        /// Enable network registration 
        /// </summary>
        [StringValue("0")]
        EnabledNetworkRegistration,
        /// <summary>
        /// Disable network registration 
        /// </summary>
        [StringValue("1")]
        DisabledNetworkRegistration,
        /// <summary>
        /// Enable network registration and location information
        /// </summary>
        [StringValue("2")]
        EnabledNetworkRegistrationAndLocation
    }


    /// <summary>
    /// Message indication mode
    /// </summary>
    public enum MessageIndicationMode
    {
        /// <summary>
        /// Buffer all indications
        /// </summary>
        DoNotForward ,
        /// <summary>
        /// No indications when the DTE-DCE link is reserved (online data mode)
        /// </summary>
        SkipWhenReserved,
        /// <summary>
        /// Buffer indications when the DTE-DCE link is reserved (online data mode)
        /// and flush them to the DTE when the reservation ended
        /// </summary>
        BufferAndFlush,
        /// <summary>
        /// Always forward
        /// </summary>
        ForwardAlways
    }

    /// <summary>
    /// SMS deliver message indication
    /// </summary>
    public enum ReceivedMessageIndication
    {
        /// <summary>
        /// No received message indications are routed to the DTE
        /// </summary>
        Disabled,
        /// <summary>
        /// Indications of received message is routed to the DTE using result code +CMTI
        /// </summary>
        RouteMemoryLocation,
        /// <summary>
        /// Received message is routed to DTE using result code +CMT, class 2 is +CMTI
        /// </summary>
        RouteMessage,
        /// <summary>
        /// Class 3 messages is routed to DTE using result code +CMT, other +CMTI
        /// </summary>
        RouteSpecial
    }

    /// <summary>
    /// Cell broadcast message (CBM) indication
    /// </summary>
    public enum CellBroadcastMessageIndication
    {
        /// <summary>
        /// No received message indications are routed to the DTE
        /// </summary>
        Disabled,
        /// <summary>
        /// Indications of received message is routed to the DTE using result code +CBMI
        /// </summary>
        RouteMemoryLocation,
        /// <summary>
        /// Received message is routed to DTE using result code +CBM
        /// </summary>
        RouteMessage,
        /// <summary>
        /// Class 3 messages is routed to DTE using result code +CBM, other +CBMI
        /// </summary>
        RouteSpecial
    }

    /// <summary>
    /// Status report message indication
    /// </summary>
    public enum StatusReportMessageIndication
    {
        /// <summary>
        /// No status report
        /// </summary>
        Disabled,
        /// <summary>
        /// Status report is routed to DTE using +CDS
        /// </summary>
        RouteMessage,
        /// <summary>
        ///  Status report is routed to DTE using +CDSI
        /// </summary>
        RouteMemoryLocation
    }

    /// <summary>
    /// Message buffer indication
    /// </summary>
    public enum MessageBufferIndication
    {
        /// <summary>
        /// Buffer of indications is flushed to DTE when mode = 1 or 2
        /// </summary>
        Flush,
        /// <summary>
        /// Buffer of indications is cleared when mode = 1 or 2 is entered
        /// </summary>
        Clear
    }

    /// <summary>
    /// Enumeration representing the different message classes.
    /// </summary>
	public enum MessageClasses
    {
        /// <summary>
        /// Default option to set no message class meaning.
        /// </summary>
        None,
        /// <summary>
        /// Class 0 - Immediate display (alert).
        /// </summary>
        Flash,       
        /// <summary>
        /// Class 1 - ME specific
        /// </summary>
        Me,
        /// <summary>
        /// Classs 2 - SIM specific
        /// </summary>
        Sim,
        /// <summary>
        /// Class 3 - TE specific
        /// </summary>
        Te
    }

    /// <summary>
    /// Message sending mode. If enabled then several messages
    /// can be sent consecutively.
    /// </summary>
    public enum BatchMessageMode
    {
        /// <summary>
        /// Disable batch SMS
        /// </summary>
        Disabled,
        /// <summary>
        /// Enable temporary 
        /// </summary>
        Temporary,
        /// <summary>
        /// Enable permanently
        /// </summary>
        Enabled,
        /// <summary>
        /// Not supported
        /// </summary>
        NotSupported
    }

    /// <summary>
    /// Flag to turn on/off a capability
    /// </summary>
    public enum CapabilityMode
    {
        /// <summary>
        /// Off mode
        /// </summary>
        [StringValue("0")]
        Off,
        /// <summary>
        /// On mode
        /// </summary>
        [StringValue("1")]
        On
    }

    /// <summary>
    /// Operator status
    /// </summary>
    public enum NetworkOperatorStatus
    {
        /// <summary>
        /// Unknown status
        /// </summary>
        Unknown,
        /// <summary>
        /// Available
        /// </summary>
        Available,
        /// <summary>
        /// Current
        /// </summary>
        Current,
        /// <summary>
        /// Forbidden
        /// </summary>
        Forbidden
    }

    /// <summary>
    /// USSD response code action
    /// </summary>
    public enum UssdResponseAction
    {
        /// <summary>
        /// No further user action required
        /// </summary>
        [StringValue("0")]
        NoActionRequired,
        /// <summary>
        /// Further user action required
        /// </summary>
        [StringValue("1")]
        FurtherActionRequired,
        /// <summary>
        /// USSD terminated by network
        /// </summary>
        [StringValue("2")]
        TerminatedByNetwork,
        /// <summary>
        /// Operation not supported
        /// </summary>
        [StringValue("4")]
        OperationNotSupported
    }

    /// <summary>
    /// Possible values of Internet protocols
    /// </summary>
    public enum InternetProtocol
    {
        /// <summary>
        /// Internet Protocol
        /// </summary>
        [StringValue("IP")]
        IP,
        /// <summary>
        /// Point to Point protocol
        /// </summary>
        [StringValue("PPP")]
        PPP,
        /// <summary>
        /// Internet Protocol v6
        /// </summary>
        [StringValue("IPV6")]
        IPV6,
    }

    /// <summary>
    /// Operator format
    /// </summary>
    public enum NetworkOperatorFormat
    {
        /// <summary>
        /// Long format and alphanumeric
        /// </summary>
        LongFormatAlphanumeric,
        /// <summary>
        /// Short format and alphanumeric
        /// </summary>
        ShortFormatAlphanumeric,
        /// <summary>
        /// Numeric
        /// </summary>
        Numeric
    }

    /// <summary>
    /// Network operator selection mode
    /// </summary>
    public enum NetworkOperatorSelectionMode
    {
        /// <summary>
        /// Automatic 
        /// </summary>
        Automatic,
        /// <summary>
        /// Manual
        /// </summary>
        Manual,
        /// <summary>
        /// Deregister from network
        /// </summary>
        DeregisterFromNetwork,
        /// <summary>
        /// No register and deregister
        /// </summary>
        NoRegisterDeregister,
        /// <summary>
        /// Try manual register, then automatic
        /// </summary>
        ManualThenAutomatic
    }

    /// <summary>
    /// Message notification flag
    /// </summary>
    [Flags]
    public enum MessageNotification
    {
        /// <summary>
        /// Enable notification for received message
        /// </summary>
        ReceivedMessage = 1,
        /// <summary>
        /// Enable notification for status report
        /// </summary>
        StatusReport = 2,
    }

    /// <summary>
    /// MMS address type
    /// </summary>
    public enum MmsAddressType
    {
        /// <summary>
        /// Phone number of type PLMN (Public Land Mobile Network)
        /// </summary>
        PhoneNumber,
        /// <summary>
        /// IPv4
        /// </summary>
        IPv4,
        /// <summary>
        /// IPv6
        /// </summary>
        IPv6,
        /// <summary>
        /// Email
        /// </summary>
        Email
    }
    

    /// <summary>
    /// Feature command type
    /// </summary>
    internal enum FeatureCommandType
    {
        /// <summary>
        /// AT command based feature. E.g. SMS
        /// </summary>
        AT,
        /// <summary>
        /// Data feature. E.g. MMS
        /// </summary>
        Data
    }

    /// <summary>
    /// Notification type. At this moment only MMS 
    /// notification is supported
    /// </summary>
    public enum NotificationType
    {
        /// <summary>
        /// New MMS received notification
        /// </summary>
        Mms
    }

    /// <summary>
    /// Notification status
    /// </summary>
    public enum NotificationStatus
    {
        /// <summary>
        /// No status
        /// </summary>
        None,
        /// <summary>
        /// New status
        /// </summary>
        New
    }

    /// <summary>
    /// Attachment type
    /// </summary>
    public enum AttachmentType
    {
        /// <summary>
        /// Text attachment
        /// </summary>
        Text,
        /// <summary>
        /// Image attachment
        /// </summary>
        Image,
        /// <summary>
        /// Audio attachment
        /// </summary>
        Audio
    }

    /// <summary>
    /// Content type
    /// </summary>
    public enum ContentType
    {
        /// <summary>
        /// HTML text content type
        /// </summary>
        [StringValue("text/html")]
        TextHtml,

        /// <summary>
        /// Plain text content type
        /// </summary>
        [StringValue("text/plain")]
        TextPlain,

        /// <summary>
        /// WML text content type
        /// </summary>
        [StringValue("text/vnd.wap.wml")]
        TextWml,

        /// <summary>
        /// GIF image content type
        /// </summary>
        [StringValue("image/gif")]
        ImageGif,

        /// <summary>
        /// JPEG image content type
        /// </summary>
        [StringValue("image/jpeg")]
        ImageJpeg,

        /// <summary>
        /// TIFF image content type
        /// </summary>
        [StringValue("image/tiff")]
        ImageTiff,

        /// <summary>
        /// PNG image content type
        /// </summary>
        [StringValue("image/png")]
        ImagePng,

        /// <summary>
        /// WBMP image content type
        /// </summary>
        [StringValue("image/vnd.wap.wbmp")]
        ImageWbmp,

        /// <summary>
        /// AMR audio content type
        /// </summary>
        [StringValue("audio/amr")]
        AudioAmr,

        /// <summary>
        /// IMelody content type
        /// </summary>
        [StringValue("text/x-imelody")]
        AudioIMelody,

        /// <summary>
        /// MIDI content type
        /// </summary>
        [StringValue("audio/midi")]
        AudioMidi,

        /// <summary>
        /// SMIL content type
        /// </summary>
        [StringValue("application/smil")]
        ApplicationSmil,
        
        /// <summary>
        /// vCalendar
        /// </summary>
        [StringValue("text/x-vCalendar")]
        vCalendar,

        /// <summary>
        /// vCard
        /// </summary>
        [StringValue("text/x-vCard")]
        vCard
    }

    #endregion ===============================================================================================

    #region ================= Public Constants ===============================================================

    /// <summary>
    /// Wireless session protocol (WSP) 
    /// </summary>
    public static class WirelessSessionProtocol
    {
        /// <summary>
        /// Connectionless transaction id for WSP
        /// </summary>
        public static byte TransactionIdConnectionLess = 0x25;

        /// <summary>
        /// PDU type for WAP push
        /// </summary>
        public static byte PduTypePush = 0x06;

        /// <summary>
        /// Header content length
        /// </summary>
        public static byte HeaderContentLength = 0x8D;

        /// <summary>
        /// Header content type for WAP push
        /// </summary>
        public static byte[] HeaderContentTypeApplicationVndWapSicUtf8 = new byte[] { 0x03, 0xAE, 0x81, 0xEA };

        /// <summary>
        /// Header application type
        /// </summary>
        public static byte HeaderApplicationType = 0xaf;

        /// <summary>
        /// Header application type for WAP
        /// </summary>
        public static byte HeaderApplicationTypeXWapApplicationIdW2 = 0x82;

        /// <summary>
        /// Header push flag
        /// </summary>
        public static byte[] HeaderPushFlag = new byte[] { 0xB4, 0x84 };
    }


    /// <summary>
    /// Well known values used when generating a WDP (Wireless Datagram Protocol) header
    /// </summary>
    public static class WirelessDatagramProtocol
    {
        /// <summary>
        /// Information element identifier application port
        /// </summary>
        public static byte InformationElementIdentifierApplicationPort = 0x05;
    }


    /// <summary>
    /// Series of well known constants and static byte values used when encoding
    /// a document to WBXML
    /// </summary>
    public static class WbXml
    {
        /// <summary>
        /// Null value
        /// </summary>
        public static byte Null = 0x00;

        /// <summary>
        /// Version information
        /// </summary>
        public static byte Version11 = 0x01;

        /// <summary>
        /// Version information
        /// </summary>
        public static byte Version12 = 0x02;

        /// <summary>
        /// Charset of UTF-8
        /// </summary>
        public static byte CharsetUtf8 = 0x6A;

        /// <summary>
        /// Token end tag
        /// </summary>
        public static byte TagTokenEnd = 0x01;

        /// <summary>
        /// Token inline string
        /// </summary>
        public static byte TokenInlineStringFollows = 0x03;

        /// <summary>
        /// Token opaque data
        /// </summary>
        public static byte TokenOpaqueDataFollows = 0xC3;


        /// <summary>
        /// Set tag token indications
        /// </summary>
        /// <param name="token">Token</param>
        /// <param name="hasAttributes">Attribute indicator</param>
        /// <param name="hasContent">Content indicator</param>
        /// <returns>Tag token</returns>
        public static byte SetTagTokenIndications(byte token, bool hasAttributes, bool hasContent)
        {
            if (hasAttributes)
                token |= 0xC0;
            if (hasContent)
                token |= 0x40;

            return token;
        }
    }

    /// <summary>
    /// Persistence queue constants
    /// </summary>
    internal static class PersistenceQueue
    {
        public static string QueueFolder = "queue";
        public static string SmsFolder = "sms";
        public static string MmsFolder = "mms";
        public static string DelayedQueueFolder = "delay";
    }

    #endregion =================================================================================================


    #region ================= Public Delegates =================================================================

    /// <summary>
    /// Message event handler
    /// </summary>
    public delegate void MessageEventHandler(object sender, MessageEventArgs e);

    /// <summary>
    /// Message error event handler
    /// </summary>
    public delegate void MessageErrorEventHandler(object sender, MessageErrorEventArgs e);

    /// <summary>
    /// Message received event handler
    /// </summary>
    public delegate void MessageReceivedEventHandler(object sender, MessageReceivedEventArgs e);

    /// <summary>
    /// USSD response event handler
    /// </summary>
    public delegate void UssdResponseReceivedHandler(object sender, UssdReceivedEventArgs e);

    /// <summary>
    /// Raw message event handler
    /// </summary>
    internal delegate void RawMessageReceivedEventHandler(object sender, IIndicationObject e);

    /// <summary>
    /// Incoming call event handler
    /// </summary>
    public delegate void IncomingCallEventHandler(object sender, IncomingCallEventArgs e);


    /// <summary>
    /// Outgoing call event handler
    /// </summary>
    public delegate void OutgoingCallEventHandler(object sender, OutgoingCallEventArgs e);

    /// <summary>
    /// Gateway connected event handler
    /// </summary>
    public delegate void ConnectedEventHandler(object sender, ConnectionEventArgs e);

    /// <summary>
    /// Gateway disconnected event handler
    /// </summary>
    public delegate void DisconnectedEventHandler(object sender, ConnectionEventArgs e);


    /// <summary>
    /// Watchdog failure event handler
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="MessagingToolkit.Core.Mobile.Event.WatchDogEventArgs"/> instance containing the event data.</param>
    public delegate void WatchDogFailureEventHandler(object sender, WatchDogEventArgs e);


    #endregion =================================================================================================
    
}
