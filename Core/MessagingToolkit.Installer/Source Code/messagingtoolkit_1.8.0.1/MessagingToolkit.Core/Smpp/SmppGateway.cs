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
using System.Net.Sockets;
using System.Collections;
using System.Threading;
using System.Timers;
using System.ComponentModel;
using System.Runtime.CompilerServices;

using MessagingToolkit.Core.Smpp.Packet.Request;
using MessagingToolkit.Core.Smpp.Utility;
using MessagingToolkit.Core.Smpp.Packet.Response;
using MessagingToolkit.Core.Smpp.Packet;
using MessagingToolkit.Core.Smpp.EventObjects;
using MessagingToolkit.Core.Log;
using MessagingToolkit.Core.Base;
using MessagingToolkit.Core.Service;

namespace MessagingToolkit.Core.Smpp 
{
	/// <summary>
	/// Wrapper class to provide asynchronous I/O for the SMPP library.  Note that most
	/// SMPP events have default handlers.  If the events are overridden by the caller by adding
	/// event handlers, it is the caller's responsibility to ensure that the proper response is
	/// sent.  For example: there is a default deliver_sm_resp implemented.  If you "listen" to
	/// the deliver_sm event, it is your responsibility to then send the deliver_sm_resp packet.
	/// </summary>
	public class SmppGateway : Component, ISmppGateway
	{

		/// <summary>
		/// Default port to 9999
		/// </summary>
		public const int DefaultPort = 9999;

		/// <summary>
		/// Default host to localhost
		/// </summary>
		public const string DefaultHost = "localhost";

		/// <summary>
		/// Default sleep time after socket failure
		/// </summary>
		public const int DefaultSleepTimeAfterSocketFailure = 10;

		/// <summary>
		/// Default retry count after socket failure
		/// </summary>
		public const int DefaultRetryAfterSocketFailure = 3;

		/// <summary>
		/// Store the last exception encountered
		/// </summary>
		protected Exception exception;

		private AsyncSocketClient asClient;
		private short _port;
		private BindingType _bindType;
		private string _host;
		private NpiType _npiType;
		private TonType _tonType; 
		private SmppVersionType _version;
		private string _addressRange;
		private string _password;
		private string _systemId;
		private string _systemType;
		private System.Timers.Timer timer;
		private int _enquireLinkInterval;
		private int _sleepTimeAfterSocketFailure;
		private bool _sentUnbindPacket = true;  //default to true since we start out unbound
		private string username;

		
		/// <summary>
		/// License
		/// </summary>
		protected License license;
		
		/// <summary>
		/// Required designer variable.
		/// </summary>
		protected Container components = null;



		#region properties
		
		/// <summary>
		/// The username to use for software validation.
		/// </summary>
		public string Username
		{
			set
			{
				username = value;
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
				return _sentUnbindPacket;
			}
		}
		
		/// <summary>
		/// The port on the SMSC to connect to.
		/// </summary>
		public short Port
		{
			get
			{
				return _port;
			}
			set
			{
				_port = value;
			}
		}
		
		/// <summary>
		/// The binding type(receiver, transmitter, or transceiver)to use 
		/// when connecting to the SMSC.
		/// </summary>
		public BindingType BindType
		{
			get
			{
				return _bindType;
			}
			set
			{
				_bindType = value;
			}
		
		}
		/// <summary>
		/// The system type to use when connecting to the SMSC.
		/// </summary>
		public string SystemType
		{
			get
			{
				return _systemType;
			}
			set
			{
				_systemType = value;
			}
		}
		/// <summary>
		/// The system ID to use when connecting to the SMSC.  This is, 
		/// in essence, a user name.
		/// </summary>
		public string SystemId
		{
			get
			{
				return _systemId;
			}
			set
			{
				_systemId = value;
			}
		}
		/// <summary>
		/// The password to use when connecting to an SMSC.
		/// </summary>
		public string Password
		{
			get
			{
				return _password;
			}
			set
			{
				_password = value;
			}
		}
		
		/// <summary>
		/// The host to bind this SMPP gateway to.
		/// </summary>
		public string Host
		{
			get
			{
				return _host;
			}
			set
			{
				_host = value;
			}
		}
		/// <summary>
		/// The number plan indicator that this SMPP gateway should use.  
		/// </summary>
		public NpiType NpiType 
		{
			get
			{
				return _npiType;
			}
			set 
			{
				_npiType = value;
			}
		}

		/// <summary>
		/// The type of number that this SMPP gateway should use.  
		/// </summary>
		public TonType TonType 
		{
			get
			{
				return _tonType;
			}
			set 
			{
				_tonType = value;
			}
		}

		/// <summary>
		/// The SMPP specification version to use.
		/// </summary>
		public SmppVersionType Version 
		{
			get
			{
				return _version;
			}
			set 
			{
				_version = value;
			}
		}

		/// <summary>
		/// The address range of this SMPP gateway.
		/// </summary>
		public string AddressRange 
		{
			get
			{
				return _addressRange;
			}
			set 
			{
				_addressRange = value;
			}
		}

		/// <summary>
		/// Set to the number of seconds that should elapse in between enquire_link 
		/// packets.  Setting this to anything other than 0 will enable the timer, setting 
		/// it to 0 will disable the timer.  Note that the timer is only started/stopped 
		/// during a bind/unbind.  Negative values are ignored.
		/// </summary>
		public int EnquireLinkInterval
		{
			get 
			{
				return _enquireLinkInterval;
			}

			set
			{
				if(value >= 0)
					_enquireLinkInterval = value;
			}
		}

		/// <summary>
		/// Sets the number of seconds that the system will wait before trying to rebind 
		/// after a total network failure(due to cable problems, etc).  Negative values are 
		/// ignored.
		/// </summary>
		public int SleepTimeAfterSocketFailure
		{
			get 
			{
				return _sleepTimeAfterSocketFailure;
			}

			set
			{
				if(value >= 0)
					_sleepTimeAfterSocketFailure = value;
			}
		}

		/// <summary>
		/// Number of retry after socket failure
		/// </summary>
		/// <value>The retry after socket failure.</value>      
		public int RetryAfterSocketFailure
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

		#endregion properties
		
		#region events
		/// <summary>
		/// Event called when the gateway receives a bind response.
		/// </summary>
		public event BindRespEventHandler OnBindResp;
		/// <summary>
		/// Event called when an error occurs.
		/// </summary>
		public event ErrorEventHandler OnError;
		/// <summary>
		/// Event called when the gateway is unbound.
		/// </summary>
		public event UnbindRespEventHandler OnUnboundResp;
		/// <summary>
		/// Event called when the connection is closed.
		/// </summary>
		public event ClosingEventHandler OnClose;
		/// <summary>
		/// Event called when an alert_notification comes in.
		/// </summary>
		public event AlertEventHandler OnAlert;
		/// <summary>
		/// Event called when a submit_sm_resp is received.
		/// </summary>
		public event SubmitSmRespEventHandler OnSubmitSmResp;
		/// <summary>
		/// Event called when a response to an enquire_link_resp is received.
		/// </summary>
		public event EnquireLinkRespEventHandler OnEnquireLinkResp;
		/// <summary>
		/// Event called when a submit_sm is received.
		/// </summary>
		public event SubmitSmEventHandler OnSubmitSm;
		/// <summary>
		/// Event called when a query_sm is received.
		/// </summary>
		public event QuerySmEventHandler OnQuerySm;
		/// <summary>
		/// Event called when a generic_nack is received.
		/// </summary>
		public event GenericNackEventHandler OnGenericNack;
		/// <summary>
		/// Event called when an enquire_link is received.
		/// </summary>
		public event EnquireLinkEventHandler OnEnquireLink;
		/// <summary>
		/// Event called when an unbind is received.
		/// </summary>
		public event UnbindEventHandler OnUnbind;
		/// <summary>
		/// Event called when the gateway receives a request for a bind.
		/// </summary>
		public event BindEventHandler OnBind;
		/// <summary>
		/// Event called when the gateway receives a cancel_sm.
		/// </summary>
		public event CancelSmEventHandler OnCancelSm;
		/// <summary>
		/// Event called when the gateway receives a cancel_sm_resp.
		/// </summary>
		public event CancelSmRespEventHandler OnCancelSmResp;
		/// <summary>
		/// Event called when the gateway receives a query_sm_resp.
		/// </summary>
		public event QuerySmRespEventHandler OnQuerySmResp;
		/// <summary>
		/// Event called when the gateway receives a data_sm.
		/// </summary>
		public event DataSmEventHandler OnDataSm;
		/// <summary>
		/// Event called when the gateway receives a data_sm_resp.
		/// </summary>
		public event DataSmRespEventHandler OnDataSmResp;
		/// <summary>
		/// Event called when the gateway receives a deliver_sm.
		/// </summary>
		public event DeliverSmEventHandler OnDeliverSm;
		/// <summary>
		/// Event called when the gateway receives a deliver_sm_resp.
		/// </summary>
		public event DeliverSmRespEventHandler OnDeliverSmResp;
		/// <summary>
		/// Event called when the gateway receives a replace_sm.
		/// </summary>
		public event ReplaceSmEventHandler OnReplaceSm;
		/// <summary>
		/// Event called when the gateway receives a replace_sm_resp.
		/// </summary>
		public event ReplaceSmRespEventHandler OnReplaceSmResp;
		/// <summary>
		/// Event called when the gateway receives a submit_multi.
		/// </summary>
		public event SubmitMultiEventHandler OnSubmitMulti;
		/// <summary>
		/// Event called when the gateway receives a submit_multi_resp.
		/// </summary>
		public event SubmitMultiRespEventHandler OnSubmitMultiResp;
		#endregion events
		
	

		#region constructors
		
		/// <summary>
		/// Creates a default SMPP gateway, with port 9999, bindtype set to 
		/// transceiver, host set to localhost, NPI type set to ISDN, TON type 
		/// set to International, version set to 3.4, enquire link interval set 
		/// to 0(disabled), sleep time after socket failure set to 10 seconds, 
		/// and address range, password, system type and system ID set to null 
		///(no value).
		/// </summary>
		/// <param name="container">The container that will hold this 
		/// component.</param>
		public SmppGateway(IContainer container)
		{
			// Required for Windows.Forms Class Composition Designer support
			InitGateway();
			container.Add(this);
			InitializeComponent();
		}
		
		/// <summary>
		/// Creates a default SMPP gateway, with port 9999, bindtype set to 
		/// transceiver, host set to localhost, NPI type set to ISDN, TON type 
		/// set to International, version set to 3.4, enquire link interval set 
		/// to 0(disabled), sleep time after socket failure set to 10 seconds, 
		/// and address range, password, system type and system ID set to null 
		///(no value).
		/// </summary>
		public SmppGateway()
		{
			InitGateway();
			
			InitializeComponent();
		}


		/// <summary>
		/// Initializes a new instance of the <see cref="SmppGateway"/> class.
		/// </summary>
		/// <param name="config">The config.</param>
		public SmppGateway(SmppGatewayConfiguration config)
		{
			Port = config.Port;
			BindType = config.BindType;
			Host = config.Host;
			NpiType = config.NpiType;
			TonType = config.TonType;
			Version = config.Version;
			AddressRange = config.AddressRange;
			Password = config.Password;
			SystemId = config.SystemId;
			SystemType = config.SystemType;
			EnquireLinkInterval = config.EnquireLinkInterval;
			SleepTimeAfterSocketFailure = config.SleepTimeAfterSocketFailure;
			RetryAfterSocketFailure = config.RetryAfterSocketFailure;
			

			// Initialize the logger
			Logger.UseSensibleDefaults(config.LogFile, config.LogLocation, config.LogLevel, config.LogNameFormat);
            Logger.LogPrefix = LogPrefix.Dt;
						
			// Initialize the license
			license = new License(config);

			InitializeComponent();
		}

		#endregion constructors


		/// <summary>
		/// Sends a user-specified Pdu(see the base library for
		/// Pdu types).  This allows complete flexibility for sending Pdus.
		/// </summary>
		/// <param name="packet">The Pdu to send.</param>
		public bool SendPdu(PDU packet)
		{
			bool sendFailed = true;
			int retryCount = 0;
			
			while(sendFailed)
			{
				try
				{
					packet.ToMsbHexEncoding();
					asClient.Send(packet.PacketBytes);
					sendFailed = false;
				}
				catch(Exception exc)
				{
					if(OnError != null)
					{
						OnError(this, new CommonErrorEventArgs(exc));
					}

					this.exception = exc;

					//try to stay alive
					if((exc.Message.ToLower().IndexOf("socket is closed")>= 0 || 
						exc.Message.ToLower().IndexOf("unable to write data to the transport connection")>= 0))
					{
						System.Threading.Thread.Sleep(SleepTimeAfterSocketFailure * 1000);
						if (!Bind())
						{
							retryCount++;
							if (retryCount >= RetryAfterSocketFailure)
							{
								if (OnClose != null)
								{
									EventArgs e = new EventArgs();                                    
									OnClose(this, e);
								}
								sendFailed = false;
								return false; 
							}
						}                        
					}
					else	
					{
						//don't know what happened, but kick out
						sendFailed = false;
						return false;
					}
				}
			}
			return true;
		}

		/// <summary>
		/// Connects and binds the SMPP gateway to the SMSC, using the
		/// values that have been set in the constructor and through the
		/// properties.  This will also start the timer that sends enquire_link packets 
		/// at regular intervals, if it has been enabled.
		/// </summary>
		public bool Bind()
		{
			try
			{
				if(asClient != null)
					asClient.Disconnect();
			}
			catch
			{
				//drop it on the floor
			}

			//connect
			try 
			{
				asClient = new AsyncSocketClient(10240, null,
					new AsyncSocketClient.MessageHandler(ClientMessageHandler),
					new AsyncSocketClient.SocketClosingHandler(ClientCloseHandler),
					new AsyncSocketClient.ErrorHandler(ClientErrorHandler));

				asClient.Connect(Host, Port);

				SmppBind request = new SmppBind();
				request.SystemId = SystemId;
				request.Password = Password;
				request.SystemType = SystemType;
				request.InterfaceVersion = Version;
				request.AddressTon = TonType;
				request.AddressNpi = NpiType;
				request.AddressRange = AddressRange;
				request.BindType = BindType;
				

				SendPdu(request);
				_sentUnbindPacket = false;

				if(_enquireLinkInterval > 0)
				{
					if(timer == null)
					{
						timer = new System.Timers.Timer();
						timer.Elapsed += new ElapsedEventHandler(TimerElapsed);
					}

					if(timer != null)		//reset the old timer
					{
						timer.Stop();

						timer.Interval = EnquireLinkInterval * 1000;
						timer.Start();
					}
				}
			} 
			catch(Exception exc)
			{
				if(OnError != null)
				{
					OnError(this, new CommonErrorEventArgs(exc));
				}
				
				this.exception = exc;

				return false;
			}

			return true;
		}

		/// <summary>
		/// Unbinds the SMPP gateway from the SMSC then disconnects the socket
		/// when it receives the unbind response from the SMSC.  This will also stop the 
		/// timer that sends out the enquire_link packets if it has been enabled.  You need to 
		/// explicitly call this to unbind.; it will not be done for you.
		/// </summary>
		public bool Unbind()
		{
			if(timer != null)
				timer.Stop();
			
			if(!_sentUnbindPacket)
			{
				SmppUnbind request = new SmppUnbind();
				SendPdu(request);
				_sentUnbindPacket = true;
			}
			return true;
		}

		/// <summary>
		/// Clears the log file content
		/// </summary>
		public void ClearLog()
		{
			Logger.ClearLog();
		}

		#region internal methods		
		/// <summary>
		/// Callback method to handle received messages.  The AsyncSocketClient
		/// library calls this; don't call it yourself.
		/// </summary>
		/// <param name="client">The client to receive messages from.</param>
		internal void ClientMessageHandler(AsyncSocketClient client)
		{
			try 
			{
				Queue responseQueue = new PduFactory().GetPduQueue(client.Buffer);
				ThreadPool.QueueUserWorkItem(new WaitCallback(ProcessPduQueue), responseQueue);
			} 
			catch(Exception exception)
			{
				if(OnError != null)
				{
					CommonErrorEventArgs e = new CommonErrorEventArgs(exception);
					OnError(this, e);
				}
				this.exception = exception;
			}
		}

		/// <summary>
		/// Callback method to handle socket closing.
		/// </summary>
		/// <param name="client">The client to receive messages from.</param>
		internal void ClientCloseHandler(AsyncSocketClient client)
		{
			//fire off a closing event
			if(OnClose != null)
			{
				System.EventArgs e = new System.EventArgs();
				OnClose(this, e);
			}
		}

		/// <summary>
		/// Callback method to handle errors.
		/// </summary>
		/// <param name="client">The client to receive messages from.</param>
		/// <param name="exception">The generated exception.</param>
		internal void ClientErrorHandler(AsyncSocketClient client,
			Exception exception)
		{
			//fire off an error handler
			if(OnError != null)
			{
				CommonErrorEventArgs e = new CommonErrorEventArgs(exception);
				OnError(this, e);
			}
			this.exception = exception;
		}
		#endregion internal methods

		#region private methods
		
				/// <summary>
		/// Goes through the packets in the queue and fires events for them.  Called by the
		/// threads in the ThreadPool.
		/// </summary>
		/// <param name="queueStateObj">The queue of byte packets.</param>
		private void ProcessPduQueue(object queueStateObj)
		{
			Queue responseQueue = queueStateObj as Queue;

			foreach (PDU response in responseQueue)
			{
				//based on each Pdu, fire off an event
				if(response != null)
					FireEvents(response);
			}
		}
		
		/// <summary>
		/// Sends out an enquire_link packet.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="ea"></param>
		private void TimerElapsed(object sender, ElapsedEventArgs ea)
		{
			SendPdu(new SmppEnquireLink());
		}

		/// <summary>
		/// Fires an event off based on what Pdu is sent in.
		/// </summary>
		/// <param name="response">The response to fire an event for.</param>
		private void FireEvents(PDU response)
		{
			//here we go...
			if(response is SmppBindResp)
			{
				if(OnBindResp != null)
				{
					OnBindResp(this, new BindRespEventArgs((SmppBindResp)response));
				}
			} 
			else if(response is SmppUnbindResp)
			{
				//disconnect
				asClient.Disconnect();
				if(OnUnboundResp != null)
				{
					OnUnboundResp(this, new UnbindRespEventArgs((SmppUnbindResp)response));
				}
			} 
			else if(response is SmppAlertNotification)
			{
				if(OnAlert != null)
				{
					OnAlert(this, new AlertEventArgs((SmppAlertNotification)response));
				}
			}	
			else if(response is SmppSubmitSmResp)
			{
				if(OnSubmitSmResp != null)
				{
					OnSubmitSmResp(this,
						new SubmitSmRespEventArgs((SmppSubmitSmResp)response));
				}
			}
			else if(response is SmppEnquireLinkResp)
			{
				if(OnEnquireLinkResp != null)
				{
					OnEnquireLinkResp(this, new EnquireLinkRespEventArgs((SmppEnquireLinkResp)response));
				}
			}
			else if(response is SmppSubmitSm)
			{
				if(OnSubmitSm != null)
				{
					OnSubmitSm(this, new SubmitSmEventArgs((SmppSubmitSm)response));
				}
				else
				{
					//default a response
					SmppSubmitSmResp pdu = new SmppSubmitSmResp();
					pdu.SequenceNumber = response.SequenceNumber;
					pdu.MessageId = System.Guid.NewGuid().ToString().Substring(0, 10);
					pdu.CommandStatus = 0;
	
					SendPdu(pdu);
				}
			}
			else if(response is SmppQuerySm)
			{
				if(OnQuerySm != null)
				{
					OnQuerySm(this, new QuerySmEventArgs((SmppQuerySm)response));
				}
				else
				{
					//default a response
					SmppQuerySmResp pdu = new SmppQuerySmResp();
					pdu.SequenceNumber = response.SequenceNumber;
					pdu.CommandStatus = 0;
	
					SendPdu(pdu);
				}
			}
			else if(response is SmppGenericNack)
			{
				if(OnGenericNack != null)
				{
					OnGenericNack(this, new GenericNackEventArgs((SmppGenericNack)response));
				}
			}
			else if(response is SmppEnquireLink)
			{
				if(OnEnquireLink != null)
				{
					OnEnquireLink(this, new EnquireLinkEventArgs((SmppEnquireLink)response));
				}
				
				//send a response back
				SmppEnquireLinkResp pdu = new SmppEnquireLinkResp();
				pdu.SequenceNumber = response.SequenceNumber;
				pdu.CommandStatus = 0;

				SendPdu(pdu);
			}
			else if(response is SmppUnbind)
			{
				if(OnUnbind != null)
				{
					OnUnbind(this, new UnbindEventArgs((SmppUnbind)response));
				}
				else
				{
					//default a response
					SmppUnbindResp pdu = new SmppUnbindResp();
					pdu.SequenceNumber = response.SequenceNumber;
					pdu.CommandStatus = 0;
	
					SendPdu(pdu);
				}
			}
			else if(response is SmppBind)
			{
				if(OnBind != null)
				{
					OnBind(this, new BindEventArgs((SmppBind)response));
				}
				else
				{
					//default a response
					SmppBindResp pdu = new SmppBindResp();
					pdu.SequenceNumber = response.SequenceNumber;
					pdu.CommandStatus = 0;
					pdu.SystemId = "Generic";
	
					SendPdu(pdu);
				}
			}
			else if(response is SmppCancelSm)
			{
				if(OnCancelSm != null)
				{
					OnCancelSm(this, new CancelSmEventArgs((SmppCancelSm)response));
				}
				else
				{
					//default a response
					SmppCancelSmResp pdu = new SmppCancelSmResp();
					pdu.SequenceNumber = response.SequenceNumber;
					pdu.CommandStatus = 0;
	
					SendPdu(pdu);
				}
			}
			else if(response is SmppCancelSmResp)
			{
				if(OnCancelSmResp != null)
				{
					OnCancelSmResp(this, new CancelSmRespEventArgs((SmppCancelSmResp)response));
				}
			}
			else if(response is SmppCancelSmResp)
			{
				if(OnCancelSmResp != null)
				{
					OnCancelSmResp(this, new CancelSmRespEventArgs((SmppCancelSmResp)response));
				}
			}
			else if(response is SmppQuerySmResp)
			{
				if(OnQuerySmResp != null)
				{
					OnQuerySmResp(this, new QuerySmRespEventArgs((SmppQuerySmResp)response));
				}
			}
			else if(response is SmppDataSm)
			{
				if(OnDataSm != null)
				{
					OnDataSm(this, new DataSmEventArgs((SmppDataSm)response));
				}
				else
				{
					//default a response
					SmppDataSmResp pdu = new SmppDataSmResp();
					pdu.SequenceNumber = response.SequenceNumber;
					pdu.CommandStatus = 0;
					pdu.MessageId = "Generic";
	
					SendPdu(pdu);
				}
			}
			else if(response is SmppDataSmResp)
			{
				if(OnDataSmResp != null)
				{
					OnDataSmResp(this, new DataSmRespEventArgs((SmppDataSmResp)response));
				}
			}
			else if(response is SmppDeliverSm)
			{
				if(OnDeliverSm != null)
				{
					OnDeliverSm(this, new DeliverSmEventArgs((SmppDeliverSm)response));
				}
				else
				{
					//default a response
					SmppDeliverSmResp pdu = new SmppDeliverSmResp();
					pdu.SequenceNumber = response.SequenceNumber;
					pdu.CommandStatus = 0;
	
					SendPdu(pdu);
				}
			}
			else if(response is SmppDeliverSmResp)
			{
				if(OnDeliverSmResp != null)
				{
					OnDeliverSmResp(this, new DeliverSmRespEventArgs((SmppDeliverSmResp)response));
				}
			}
			else if(response is SmppReplaceSm)
			{
				if(OnReplaceSm != null)
				{
					OnReplaceSm(this, new ReplaceSmEventArgs((SmppReplaceSm)response));
				}
				else
				{
					//default a response
					SmppReplaceSmResp pdu = new SmppReplaceSmResp();
					pdu.SequenceNumber = response.SequenceNumber;
					pdu.CommandStatus = 0;
	
					SendPdu(pdu);
				}
			}
			else if(response is SmppReplaceSmResp)
			{
				if(OnReplaceSmResp != null)
				{
					OnReplaceSmResp(this, new ReplaceSmRespEventArgs((SmppReplaceSmResp)response));
				}
			}
			else if(response is SmppSubmitMulti)
			{
				if(OnSubmitMulti != null)
				{
					OnSubmitMulti(this, new SubmitMultiEventArgs((SmppSubmitMulti)response));
				}
				else
				{
					//default a response
					SmppSubmitMultiResp pdu = new SmppSubmitMultiResp();
					pdu.SequenceNumber = response.SequenceNumber;
					pdu.CommandStatus = 0;
	
					SendPdu(pdu);
				}
			}
			else if(response is SmppSubmitMultiResp)
			{
				if(OnSubmitMultiResp != null)
				{
					OnSubmitMultiResp(this, new SubmitMultiRespEventArgs((SmppSubmitMultiResp)response));
				}
			}
		}
	
		/// <summary>
		/// Initializes the SMPP gateway with some default values.
		/// </summary>
		private void InitGateway()
		{
			Port = DefaultPort;
			BindType = BindingType.BindAsTransceiver;
			Host = DefaultHost;
			NpiType = NpiType.ISDN;
			TonType = TonType.International; 
			Version = SmppVersionType.Version3_4;
			AddressRange = string.Empty;
			Password = string.Empty;
			SystemId = string.Empty;
			SystemType = string.Empty;
			EnquireLinkInterval = 0;    // Disable the timer
			SleepTimeAfterSocketFailure = DefaultSleepTimeAfterSocketFailure;
			RetryAfterSocketFailure = DefaultRetryAfterSocketFailure;

			// Initialize the logger
			Logger.UseSensibleDefaults(BaseConfiguration.DefaultLogFileName, string.Empty, LogLevel.Error, LogNameFormat.NameDate);
            Logger.LogPrefix = LogPrefix.Dt;

			// Set the logging level from the configuration
			Logger.LogLevel = LogLevel.Error;
		}
		#endregion private methods
		
		#region Component Designer generated code
		
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		protected void InitializeComponent()
		{
			components = new System.ComponentModel.Container();
		}
		#endregion
		
		/// <summary>
		/// Disposes of this component.  Called by the framework; do not call it 
		/// directly.
		/// </summary>
		/// <param name="disposing">This is set to false during garbage collection but 
		/// true during a disposal.</param>
		protected override void Dispose(bool disposing)
		{
			if(disposing)
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			
			try
			{
				if(!_sentUnbindPacket)
					Unbind();
			}
			catch
			{
				//drop it on the floor
			}
			base.Dispose(disposing);
		}



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
				return exception;
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
				return Logger.LogLevel;
			}
			set
			{
				Logger.LogLevel = value;
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
				return Logger.LogWhere;
			}
			set
			{
				Logger.LogWhere = value;
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
				return Logger.LogPath;
			}
		}


		#endregion ===========================================================================================


		#region =========== Public Method  ===================================================================

		/// <summary>
		/// Reset the exception
		/// </summary>
		public void ClearError()
		{
			exception = null;
		}

		#endregion ===========================================================================================

	}
}
