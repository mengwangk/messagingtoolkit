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

using MessagingToolkit.Core.Log;
using MessagingToolkit.Core.Service;
using MessagingToolkit.Core.Smpp.Packet.Request;
using MessagingToolkit.Core.Smpp.Utility;
using MessagingToolkit.Core.Smpp.Packet.Response;
using MessagingToolkit.Core.Smpp.Packet;
using MessagingToolkit.Core.Smpp.EventObjects;

namespace MessagingToolkit.Core.Smpp
{
    /// <summary>
    /// Place holder SMPP gateway
    /// </summary>
    public class DefaultSmppGateway: ISmppGateway
    {

        #region Methods

        /// <summary>
        /// Connects and binds the SMPP gateway to the SMSC, using the
        /// values that have been set in the constructor and through the
        /// properties.  This will also start the timer that sends enquire_link packets 
        /// at regular intervals, if it has been enabled.
        /// </summary>
        public bool Bind() { return true; }

        /// <summary>
        /// Unbinds the SMPP gateway from the SMSC then disconnects the socket
        /// when it receives the unbind response from the SMSC.  This will also stop the 
        /// timer that sends out the enquire_link packets if it has been enabled.  You need to 
        /// explicitly call this to unbind.; it will not be done for you.
        /// </summary>
        public bool Unbind() { return true; }


        /// <summary>
        /// Sends a user-specified Pdu(see the base library for
        /// Pdu types).  This allows complete flexibility for sending Pdus.
        /// </summary>
        /// <param name="packet">The Pdu to send.</param>
        /// <returns></returns>
        public bool SendPdu(PDU packet) { return true; }

        /// <summary>
        /// Clears the log file content
        /// </summary>
        public void ClearLog() { }

        #endregion Methods

        #region Properties

        /// <summary>
        /// The username to use for software validation.
        /// </summary>
        public string Username 
        {
            set
            {
            }
        }

        /// <summary>
        /// Accessor to determine if we have sent the unbind packet out.  Once the packet is 
        /// sent, you can consider this object to be unbound.
        /// </summary>
        public bool SentUnbindPacket 
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// The port on the SMSC to connect to.
        /// </summary>
        public short Port 
        { 
            get; 
            set; 
        }

        /// <summary>
        /// The binding type(receiver, transmitter, or transceiver)to use 
        /// when connecting to the SMSC.
        /// </summary>
        public BindingType BindType 
        { 
            get; 
            set; 
        }

        /// <summary>
        /// The system type to use when connecting to the SMSC.
        /// </summary>
        public string SystemType 
        { 
            get; 
            set; 
        }

        /// <summary>
        /// The system ID to use when connecting to the SMSC.  This is, 
        /// in essence, a user name.
        /// </summary>
        public string SystemId 
        { 
            get; 
            set; 
        }

        /// <summary>
        /// The password to use when connecting to an SMSC.
        /// </summary>
        public string Password 
        { 
            get; 
            set; 
        }

        /// <summary>
        /// The host to bind this SMPP gateway to.
        /// </summary>
        public string Host 
        { 
            get; 
            set; 
        }

        /// <summary>
        /// The number plan indicator that this SMPP gateway should use.  
        /// </summary>
        public NpiType NpiType 
        { 
            get; 
            set; 
        }


        /// <summary>
        /// The type of number that this SMPP gateway should use.  
        /// </summary>
        public TonType TonType 
        { 
            get; 
            set; 
        }


        /// <summary>
        /// The SMPP specification version to use.
        /// </summary>
        public SmppVersionType Version 
        { 
            get; 
            set; 
        }

        /// <summary>
        /// The address range of this SMPP gateway.
        /// </summary>
        public string AddressRange 
        { 
            get; 
            set; 
        }


        /// <summary>
        /// Set to the number of seconds that should elapse in between enquire_link 
        /// packets.  Setting this to anything other than 0 will enable the timer, setting 
        /// it to 0 will disable the timer.  Note that the timer is only started/stopped 
        /// during a bind/unbind.  Negative values are ignored.
        /// </summary>
        public int EnquireLinkInterval 
        { 
            get; 
            set; 
        }


        /// <summary>
        /// Sets the number of seconds that the system will wait before trying to rebind 
        /// after a total network failure(due to cable problems, etc).  Negative values are 
        /// ignored.
        /// </summary>
        public int SleepTimeAfterSocketFailure 
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
                SmppGatewayConfiguration config = SmppGatewayConfiguration.NewInstance();
                return new License(config);
            }
        }

        #endregion properties

        #region events

        /// <summary>
        /// Event called when the gateway receives a bind response.
        /// </summary>
        public event BindRespEventHandler OnBindResp
        {
            add { }
            remove { }
        }

        /// <summary>
        /// Event called when an error occurs.
        /// </summary>
        public event ErrorEventHandler OnError
        {
            add { }
            remove { }
        }

        /// <summary>
        /// Event called when the gateway is unbound.
        /// </summary>
        public event UnbindRespEventHandler OnUnboundResp
        {
            add { }
            remove { }
        }

        /// <summary>
        /// Event called when the connection is closed.
        /// </summary>
        public event ClosingEventHandler OnClose
        {
            add { }
            remove { }
        }

        /// <summary>
        /// Event called when an alert_notification comes in.
        /// </summary>
        public event AlertEventHandler OnAlert
        {
            add { }
            remove { }
        }

        /// <summary>
        /// Event called when a submit_sm_resp is received.
        /// </summary>
        public event SubmitSmRespEventHandler OnSubmitSmResp
        {
            add { }
            remove { }
        }

        /// <summary>
        /// Event called when a response to an enquire_link_resp is received.
        /// </summary>
        public event EnquireLinkRespEventHandler OnEnquireLinkResp
        {
            add { }
            remove { }
        }

        /// <summary>
        /// Event called when a submit_sm is received.
        /// </summary>
        public event SubmitSmEventHandler OnSubmitSm
        {
            add { }
            remove { }
        }

        /// <summary>
        /// Event called when a query_sm is received.
        /// </summary>
        public event QuerySmEventHandler OnQuerySm
        {
            add { }
            remove { }
        }

        /// <summary>
        /// Event called when a generic_nack is received.
        /// </summary>
        public event GenericNackEventHandler OnGenericNack
        {
            add { }
            remove { }
        }


        /// <summary>
        /// Event called when an enquire_link is received.
        /// </summary>
        public event EnquireLinkEventHandler OnEnquireLink
        {
            add { }
            remove { }
        }

        /// <summary>
        /// Event called when an unbind is received.
        /// </summary>
        public event UnbindEventHandler OnUnbind
        {
            add { }
            remove { }
        }

        /// <summary>
        /// Event called when the gateway receives a request for a bind.
        /// </summary>
        public event BindEventHandler OnBind
        {
            add { }
            remove { }
        }

        /// <summary>
        /// Event called when the gateway receives a cancel_sm.
        /// </summary>
        public event CancelSmEventHandler OnCancelSm
        {
            add { }
            remove { }
        }

        /// <summary>
        /// Event called when the gateway receives a cancel_sm_resp.
        /// </summary>
        public event CancelSmRespEventHandler OnCancelSmResp
        {
            add { }
            remove { }
        }

        /// <summary>
        /// Event called when the gateway receives a query_sm_resp.
        /// </summary>
        public event QuerySmRespEventHandler OnQuerySmResp
        {
            add { }
            remove { }
        }

        /// <summary>
        /// Event called when the gateway receives a data_sm.
        /// </summary>
        public event DataSmEventHandler OnDataSm
        {
            add { }
            remove { }
        }

        /// <summary>
        /// Event called when the gateway receives a data_sm_resp.
        /// </summary>
        public event DataSmRespEventHandler OnDataSmResp
        {
            add { }
            remove { }
        }

        /// <summary>
        /// Event called when the gateway receives a deliver_sm.
        /// </summary>
        public event DeliverSmEventHandler OnDeliverSm
        {
            add { }
            remove { }
        }

        /// <summary>
        /// Event called when the gateway receives a deliver_sm_resp.
        /// </summary>
        public event DeliverSmRespEventHandler OnDeliverSmResp
        {
            add { }
            remove { }
        }

        /// <summary>
        /// Event called when the gateway receives a replace_sm.
        /// </summary>
        public event ReplaceSmEventHandler OnReplaceSm
        {
            add { }
            remove { }
        }

        /// <summary>
        /// Event called when the gateway receives a replace_sm_resp.
        /// </summary>
        public event ReplaceSmRespEventHandler OnReplaceSmResp
        {
            add { }
            remove { }
        }

        /// <summary>
        /// Event called when the gateway receives a submit_multi.
        /// </summary>
        public event SubmitMultiEventHandler OnSubmitMulti
        {
            add { }
            remove { }
        }

        /// <summary>
        /// Event called when the gateway receives a submit_multi_resp.
        /// </summary>
        public event SubmitMultiRespEventHandler OnSubmitMultiResp
        {
            add { }
            remove { }
        }

        #endregion events


        #region =========== Public Properties =============================================================

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
                return LogLevel.Error ;
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


        #endregion ===========================================================================================


        #region =========== Public Method  ===================================================================

        /// <summary>
        /// Reset the exception
        /// </summary>
        public void ClearError()
        {
            
        }

        #endregion ===========================================================================================

    }
}
