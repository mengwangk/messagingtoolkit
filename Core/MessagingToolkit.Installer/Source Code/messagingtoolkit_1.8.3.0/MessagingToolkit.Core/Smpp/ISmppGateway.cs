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
using MessagingToolkit.Core.Smpp.Packet.Request;
using MessagingToolkit.Core.Smpp.Utility;
using MessagingToolkit.Core.Smpp.Packet.Response;
using MessagingToolkit.Core.Smpp.Packet;
using MessagingToolkit.Core.Smpp.EventObjects;

namespace MessagingToolkit.Core.Smpp
{
	/// <summary>
	/// SMPPP gateway interface
	/// </summary>
	public interface ISmppGateway: IGateway
	{
		#region Methods
		
		/// <summary>
		/// Connects and binds the SMPP gateway to the SMSC, using the
		/// values that have been set in the constructor and through the
		/// properties.  This will also start the timer that sends enquire_link packets 
		/// at regular intervals, if it has been enabled.
		/// </summary>
		bool Bind();

		/// <summary>
		/// Unbinds the SMPP gateway from the SMSC then disconnects the socket
		/// when it receives the unbind response from the SMSC.  This will also stop the 
		/// timer that sends out the enquire_link packets if it has been enabled.  You need to 
		/// explicitly call this to unbind.; it will not be done for you.
		/// </summary>
		bool Unbind();


		/// <summary>
		/// Sends a user-specified Pdu(see the base library for
		/// Pdu types).  This allows complete flexibility for sending Pdus.
		/// </summary>
		/// <param name="packet">The Pdu to send.</param>
		/// <returns></returns>
		bool SendPdu(PDU packet);

		#endregion Methods

		#region Properties

		/// <summary>
		/// The username to use for software validation.
		/// </summary>
		string Username { set; }
		
		/// <summary>
		/// Accessor to determine if we have sent the unbind packet out.  Once the packet is 
		/// sent, you can consider this object to be unbound.
		/// </summary>
		bool SentUnbindPacket { get ;}
				
		/// <summary>
		/// The port on the SMSC to connect to.
		/// </summary>
		short Port { get; set; }	
		
		/// <summary>
		/// The binding type(receiver, transmitter, or transceiver)to use 
		/// when connecting to the SMSC.
		/// </summary>
		BindingType BindType { get ;set; }
		
		/// <summary>
		/// The system type to use when connecting to the SMSC.
		/// </summary>
		string SystemType { get; set; }
		
		/// <summary>
		/// The system ID to use when connecting to the SMSC.  This is, 
		/// in essence, a user name.
		/// </summary>
		string SystemId { get ;set; }

		/// <summary>
		/// The password to use when connecting to an SMSC.
		/// </summary>
		string Password { get; set; }
		
		/// <summary>
		/// The host to bind this SMPP gateway to.
		/// </summary>
		string Host { get; set; }

		/// <summary>
		/// The number plan indicator that this SMPP gateway should use.  
		/// </summary>
		NpiType NpiType {get; set;}


		/// <summary>
		/// The type of number that this SMPP gateway should use.  
		/// </summary>
		TonType TonType {get; set;}


		/// <summary>
		/// The SMPP specification version to use.
		/// </summary>
		SmppVersionType Version  { get; set;}

		/// <summary>
		/// The address range of this SMPP gateway.
		/// </summary>
		string AddressRange { get; set;}


		/// <summary>
		/// Set to the number of seconds that should elapse in between enquire_link 
		/// packets.  Setting this to anything other than 0 will enable the timer, setting 
		/// it to 0 will disable the timer.  Note that the timer is only started/stopped 
		/// during a bind/unbind.  Negative values are ignored.
		/// </summary>
		int EnquireLinkInterval { get; set; }


		/// <summary>
		/// Sets the number of seconds that the system will wait before trying to rebind 
		/// after a total network failure(due to cable problems, etc).  Negative values are 
		/// ignored.
		/// </summary>
		int SleepTimeAfterSocketFailure { get; set;}


		/// <summary>
		/// Return the license associated with this software
		/// </summary>
		/// <value>License</value>
		License License
		{
			get;
		}

		#endregion properties
		
		#region events
		
		/// <summary>
		/// Event called when the gateway receives a bind response.
		/// </summary>
		event BindRespEventHandler OnBindResp;
		
		/// <summary>
		/// Event called when an error occurs.
		/// </summary>
		event ErrorEventHandler OnError;
		
		/// <summary>
		/// Event called when the gateway is unbound.
		/// </summary>
		event UnbindRespEventHandler OnUnboundResp;
		
		/// <summary>
		/// Event called when the connection is closed.
		/// </summary>
		event ClosingEventHandler OnClose;
		
		/// <summary>
		/// Event called when an alert_notification comes in.
		/// </summary>
		event AlertEventHandler OnAlert;
		
		/// <summary>
		/// Event called when a submit_sm_resp is received.
		/// </summary>
		event SubmitSmRespEventHandler OnSubmitSmResp;
		
		/// <summary>
		/// Event called when a response to an enquire_link_resp is received.
		/// </summary>
		event EnquireLinkRespEventHandler OnEnquireLinkResp;
		
		/// <summary>
		/// Event called when a submit_sm is received.
		/// </summary>
		event SubmitSmEventHandler OnSubmitSm;
		
		/// <summary>
		/// Event called when a query_sm is received.
		/// </summary>
		event QuerySmEventHandler OnQuerySm;
		
		/// <summary>
		/// Event called when a generic_nack is received.
		/// </summary>
		event GenericNackEventHandler OnGenericNack;
		
		/// <summary>
		/// Event called when an enquire_link is received.
		/// </summary>
		event EnquireLinkEventHandler OnEnquireLink;
		
		/// <summary>
		/// Event called when an unbind is received.
		/// </summary>
		event UnbindEventHandler OnUnbind;
		
		/// <summary>
		/// Event called when the gateway receives a request for a bind.
		/// </summary>
		event BindEventHandler OnBind;
		
		/// <summary>
		/// Event called when the gateway receives a cancel_sm.
		/// </summary>
		event CancelSmEventHandler OnCancelSm;
		
		/// <summary>
		/// Event called when the gateway receives a cancel_sm_resp.
		/// </summary>
		event CancelSmRespEventHandler OnCancelSmResp;
		
		/// <summary>
		/// Event called when the gateway receives a query_sm_resp.
		/// </summary>
		event QuerySmRespEventHandler OnQuerySmResp;
		
		/// <summary>
		/// Event called when the gateway receives a data_sm.
		/// </summary>
		event DataSmEventHandler OnDataSm;
		
		/// <summary>
		/// Event called when the gateway receives a data_sm_resp.
		/// </summary>
		event DataSmRespEventHandler OnDataSmResp;
		
		/// <summary>
		/// Event called when the gateway receives a deliver_sm.
		/// </summary>
		event DeliverSmEventHandler OnDeliverSm;
		
		/// <summary>
		/// Event called when the gateway receives a deliver_sm_resp.
		/// </summary>
		event DeliverSmRespEventHandler OnDeliverSmResp;
		
		/// <summary>
		/// Event called when the gateway receives a replace_sm.
		/// </summary>
		event ReplaceSmEventHandler OnReplaceSm;
		
		/// <summary>
		/// Event called when the gateway receives a replace_sm_resp.
		/// </summary>
		event ReplaceSmRespEventHandler OnReplaceSmResp;
		
		/// <summary>
		/// Event called when the gateway receives a submit_multi.
		/// </summary>
		event SubmitMultiEventHandler OnSubmitMulti;
		
		/// <summary>
		/// Event called when the gateway receives a submit_multi_resp.
		/// </summary>
		event SubmitMultiRespEventHandler OnSubmitMultiResp;
		
		#endregion events

	}
}
