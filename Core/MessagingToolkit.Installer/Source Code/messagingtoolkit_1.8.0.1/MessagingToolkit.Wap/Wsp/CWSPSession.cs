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
using System.Net;
using System.Collections;

using MessagingToolkit.Wap.Wtp;
using MessagingToolkit.Wap.Log;
using MessagingToolkit.Wap.Helper;
using MessagingToolkit.Wap.Wsp.Pdu;


namespace MessagingToolkit.Wap.Wsp
{


    /// <summary>
    /// This class implements the WSP state machine named "management entity"
    /// in the Wireless Session Protocol Specification by the <a href="http://www.wapforum.org">WAP-forum</a>.
    /// <br/>
    /// Using this class the programmer can use all methods of the WSP layer.
    /// To use the WAP-Stack with a class corresponding to HttpURLConnection
    /// please use WapURLConnection by calling<br/>
    /// 	<code>
    ///         URL example = new URL("wap://wap.nokia.com");
    ///         URLConnection con = example.getConnection();
    ///     </code>
    /// </summary>
	public class CWSPSession : IWTPUpperLayer
	{
        /// <summary>
        /// Gets a value indicating whether this <see cref="CWSPSession"/> is suspended.
        /// </summary>
        /// <value><c>true</c> if suspended; otherwise, <c>false</c>.</value>
		virtual public bool Suspended
		{
			get
			{
				return isSuspended;
			}
			
		}
        /// <summary>
        /// Gets the suspended code.
        /// </summary>
        /// <value>The suspended code.</value>
		virtual public short SuspendedCode
		{
			get
			{
				return suspendCode;
			}
			
		}
        /// <summary>
        /// Gets a value indicating whether this <see cref="CWSPSession"/> is disonnected.
        /// </summary>
        /// <value><c>true</c> if disonnected; otherwise, <c>false</c>.</value>
		virtual public bool Disonnected
		{
			get
			{
				return isDisconnected;
			}
			
		}
        /// <summary>
        /// Gets the disconnect code.
        /// </summary>
        /// <value>The disconnect code.</value>
		virtual public short DisconnectCode
		{
			get
			{
				return disconnectCode;
			}
			
		}
        /// <summary>
        /// Gets the WTP socket.
        /// </summary>
        /// <value>The WTP socket.</value>
		virtual public CWTPSocket WTPSocket
		{
			get
			{
				return socket;
			}
			
		}

		// SESSION states
		public const short StateNull = 0;
		public const short StateConnecting = 1;
		public const short StateConnectetd = 2;
		public const short StateSuspended = 3;
		public const short StateResuming = 4;
		
		// Abort reason code assignments
		// Table 35 in spec.
		public const short AbortProtoErr = (short) (0xE0);
		public const short AbortDisconnect = (short) (0xE1);
		public const short AbortSuspend = (short) (0xE2);
		public const short AbortResume = (short) (0xE3);
		public const short AbortCongestion = (short) (0xE4);
		public const short AbortConnectErr = (short) (0xE5);
		public const short AbortMRUExceeded = (short) (0xE6);
		public const short AbortMORExceeded = (short) (0xE7);
		public const short AbortPeerReq = (short) (0xE8);
		public const short AbortNetErr = (short) (0xE9);
		public const short AbortUserReq = (short) (0xEA);
		public const short AbortUserRfs = (short) (0xEB);
		public const short AbortUserPnd = (short) (0xEC);
		public const short AbortUserDcr = (short) (0xED);
		public const short AbortUserDcu = (short) (0xEE);
		public string[] states = new string[]{"STATE_NULL", "STATE_CONNECTING", "STATE_CONNECTED", "STATE_SUSPENDED", "STATE_RESUMING"};
		private bool isSuspended = false;
		private short suspendCode;
		private bool isDisconnected = false;
		private short disconnectCode;
		
		/// <summary> the actual session state</summary>
		private short state = StateNull;
		
		/// <summary> the acual transaction concering the session management</summary>
		private IWTPTransaction wtp;
		
		/// <summary> the Layer below</summary>
		private CWTPSocket socket;
		
		/// <summary> Holds all pending CWSPMethodManagers of this session</summary>
		private ArrayList methods = ArrayList.Synchronized(new ArrayList(10));
		
		/// <summary> Holds all pending CWSPPushManagers of this session</summary>
		private ArrayList pushes = ArrayList.Synchronized(new ArrayList(10));

		private IWSPUpperLayer upperlayer;
		
		//////////////////////////////////////////////////////////////////////////////
		////////////////////////////// Protocol parameters and variables - sect. 7.1.3
		
		/// <summary> Maximum Receive Unit (sect. 7.1.3.1)</summary>
		private int MRU = 1400;
		
		/// <summary> Maximum Outstanding Method Requests (sect. 7.1.3.2)</summary>
		private int MOM = 1;
		
		/// <summary> Maximum Outstanding Push Requests (sect. 7.1.3.3)</summary>
		private int MOP = 1;
		
		/// <summary> 
        /// keeps track of the number of push transactions in process in the client
		/// (sect. 7.1.4.2)
		/// </summary>
		
		// pushs.size();
		
		/// <summary> saves the session identifier (sect. 7.1.4.3)
		/// We will get this from the ConnectReply by the Server
		/// </summary>
		private long sessionId = 0;
		
		/// <summary> do we use IWSPUpperLayer2 or </summary>
		private byte version = 0;

        /// <summary>
        /// Construct a new WSP Session.
        /// </summary>
        /// <param name="toAddress">address of the WAP gateway</param>
        /// <param name="toPort">WAP gateway port</param>
        /// <param name="upperLayer">WSP Upper Layer</param>
        /// <param name="verbose">verbose logging</param>
        /// <throws>  SocketException if the underlying WTP socket cannot be created </throws>
		public CWSPSession(IPAddress toAddress, int toPort, IWSPUpperLayer upperLayer, bool verbose):this(toAddress, toPort, null, CWTPSocket.DefaultPort, upperLayer, verbose)
		{
		}

        /// <summary>
        /// Construct a new WSP Session.
        /// </summary>
        /// <param name="toAddress">address of the WAP gateway</param>
        /// <param name="toPort">WAP gateway port</param>
        /// <param name="localAddress">local address to bind to (null to let the OS decide)</param>
        /// <param name="localPort">local port to bind to (use 0 to let the OS pick a free port)</param>
        /// <param name="upperLayer">WSP Upper Layer</param>
        /// <param name="verbose">verbose logging</param>
        /// <throws>  SocketException if the underlying WTP socket cannot be created </throws>
		public CWSPSession(IPAddress toAddress, int toPort, IPAddress localAddress, int localPort, IWSPUpperLayer upperLayer, bool verbose)
		{
			if ((upperLayer != null) && upperLayer is IWSPUpperLayer2)
			{
				this.version = 2;
			}
			else
			{
				this.version = 1;
			}
		
			this.upperlayer = upperLayer;
			socket = new CWTPSocket(toAddress, toPort, localAddress, localPort, this);
			
			// there can not be any session with the same peer address quadruplet!
		}

        /// <summary>
        /// Construct a new WSP session.
        /// </summary>
        /// <param name="toAddress">the address of the WAP gateway</param>
        /// <param name="toPort">WAP gateway port</param>
        /// <param name="upperLayer">WSP Upper Layer</param>
        /// <throws>  SocketException if the underlying WTP socket cannot be created </throws>
		public CWSPSession(IPAddress toAddress, int toPort, IWSPUpperLayer upperLayer):this(toAddress, toPort, upperLayer, false)
		{
		}


        /// <summary>
        /// Construct a new WSP session.
        /// </summary>
        /// <param name="address">The address.</param>
        /// <param name="upperLayer">WSP Upper Layer</param>
        /// <param name="verbose">verbose logging</param>
        /// <throws>  SocketException if the underlying WTP socket cannot be created </throws>
		public CWSPSession(CWSPSocketAddress address, IWSPUpperLayer upperLayer, bool verbose):this(address, null, upperLayer, verbose)
		{
		}


        /// <summary>
        /// Construct a new WSP session.
        /// </summary>
        /// <param name="address">the address and port of the WAP gateway</param>
        /// <param name="localAddress">the local address and port or null</param>
        /// <param name="upperLayer">WSP Upper Layer</param>
        /// <param name="verbose">verbose logging</param>
        /// <throws>  SocketException if the underlying WTP socket cannot be created </throws>
		public CWSPSession(CWSPSocketAddress address, CWSPSocketAddress localAddress, IWSPUpperLayer upperLayer, bool verbose):this(address.Address, address.Port, localAddress == null?null:localAddress.Address, localAddress == null?CWTPSocket.DefaultPort:localAddress.Port, upperLayer, verbose)
		{
		}

		
		// --------  WSP service primitives - S-*.* -----------------------------------

        /// <summary>
        /// Establish a WSP connection.
        /// </summary>
	    public virtual void  SConnect()
		{
			lock (this)
			{
				Connect(null);
			}
		}

        /// <summary>
        /// Establish a WSP connection using WSP headers
        /// </summary>
        /// <param name="headers">the WSP headers to set or null</param>
	    public virtual void  Connect(CWSPHeaders headers)
		{
			lock (this)
			{
				if (state == StateNull)
				{
					AbortAllMethods(AbortDisconnect);
					AbortAllPushes(AbortDisconnect);
					
					// prepare WSP Connect PDU
					CWSPConnect pdu = new CWSPConnect();
					pdu.SetHeaders(headers);
					
					// prepare WTP Service Primitive
					CWTPEvent initPacket = new CWTPEvent(pdu.ToByteArray(), CWTPEvent.TrInvokeReq);
					
					// construct transaction with initPacket
					wtp = socket.TrInvoke(this, initPacket, false, IWTPTransactionConstants.ClassType2);
					SetState(StateConnecting);
				}
			}
		}

        /// <summary>
        /// S-Disconnect.req
        /// </summary>
		public virtual void  SDisconnect()
		{
			lock (this)
			{
				if (state == StateConnecting)
				{
					wtp.Abort(AbortDisconnect);
					AbortAllMethods(AbortDisconnect);
					SDisconnectInd(AbortUserReq);
					SetState(StateNull);
				}
				else if (state == StateConnectetd)
				{
					AbortAllMethods(AbortDisconnect);
					AbortAllPushes(AbortDisconnect);
					
					CWSPDisconnect pdu = new CWSPDisconnect(sessionId);
					CWTPEvent initPacket = new CWTPEvent(pdu.ToByteArray(), CWTPEvent.TrInvokeReq);
					wtp = socket.TrInvoke(this, initPacket, false, IWTPTransactionConstants.ClassType0);
					SDisconnectInd(AbortUserReq);
					SetState(StateNull);
				}
				else if (state == StateSuspended)
				{
					SDisconnectInd(AbortUserReq);
					SetState(StateNull);
				}
				else if (state == StateResuming)
				{
					wtp.Abort();
					AbortAllMethods(AbortDisconnect);
					SDisconnectInd(AbortUserReq);
					SetState(StateNull);
				}
			}
		}
		
		/// <summary> S-Suspend.req</summary>
		/// <returns> S-Suspend.ind
		/// </returns>
		public virtual bool SSuspend()
		{
			lock (this)
			{
				if (state == StateConnectetd)
				{
					AbortAllMethods(AbortSuspend);
					AbortAllPushes(AbortSuspend);
					
					CWSPSuspend pdu = new CWSPSuspend(this.sessionId);
					CWTPEvent initPacket = new CWTPEvent(pdu.ToByteArray(), CWTPEvent.TrInvokeReq);
					wtp = socket.TrInvoke(this, initPacket, false, IWTPTransactionConstants.ClassType0);
					SDisconnectInd(AbortUserReq);
					SetState(StateSuspended);
					
					return true;
				}
				else if (state == StateResuming)
				{
					wtp.Abort(AbortSuspend);
					AbortAllMethods(AbortSuspend);
					
					CWSPSuspend pdu = new CWSPSuspend(this.sessionId);
					CWTPEvent initPacket = new CWTPEvent(pdu.ToByteArray(), CWTPEvent.TrInvokeReq);
					wtp = socket.TrInvoke(this, initPacket, false, IWTPTransactionConstants.ClassType0);
					SDisconnectInd(AbortUserReq);
					SetState(StateNull);
					
					return true;
				}
				
				return false;
			}
		}

        /// <summary>
        /// S-Resume.req
        /// </summary>
		public virtual void  SResume()
		{
			lock (this)
			{
				if (state == StateConnectetd)
				{
					AbortAllMethods(AbortUserReq);
					AbortAllPushes(AbortUserReq);
					
					// bind session to the new peer address quadruplet
					socket.Close();
					socket = new CWTPSocket(socket.RemoteAddress, socket.RemotePort, this);
					
					CWSPResume pdu = new CWSPResume(sessionId);
					CWTPEvent initPacket = new CWTPEvent(pdu.ToByteArray(), CWTPEvent.TrInvokeReq);
					wtp = socket.TrInvoke(this, initPacket, false, IWTPTransactionConstants.ClassType2);
					SetState(StateResuming);
				}
				else if (state == StateSuspended)
				{
					CWSPResume pdu = new CWSPResume(sessionId);
					CWTPEvent initPacket = new CWTPEvent(pdu.ToByteArray(), CWTPEvent.TrInvokeReq);
					wtp = socket.TrInvoke(this, initPacket, false, IWTPTransactionConstants.ClassType2);
					SetState(StateResuming);
				}
			}
		}

        /// <summary>
        /// Use this method to construct a POST-MethodInvoke.req.
        /// This method uses <code>methodInvoke(CWSPPDU pdu)</code>
        /// to send the constructed WSP-POST-PDU.
        /// </summary>
        /// <param name="data">The data to be POSTed</param>
        /// <param name="contentType">The MIME-ContentType of the data to be POSTed</param>
        /// <param name="uri">The URI.</param>
        /// <returns></returns>
		public virtual CWSPMethodManager SPost(byte[] data, string contentType, string uri)
		{
			lock (this)
			{
				CWSPPost pdu = new CWSPPost(data, contentType, uri);
				
				return SMethodInvoke(pdu);
			}
		}

        /// <summary>
        /// Use this method to construct a POST-MethodInvoke.req.
        /// This method uses <code>methodInvoke(CWSPPDU pdu)</code>
        /// to send the constructed WSP-POST-PDU.
        /// </summary>
        /// <param name="headers">The headers defined for the request</param>
        /// <param name="data">The data to be POSTed</param>
        /// <param name="contentType">The MIME-ContentType of the data to be POSTed</param>
        /// <param name="uri">the target URI to post to</param>
        /// <returns></returns>
		public virtual CWSPMethodManager SPost(CWSPHeaders headers, byte[] data, string contentType, string uri)
		{
			lock (this)
			{
				/** @todo hier müssen die Nachrichten aufgeteilt werden !!!! */
				CWSPPost pdu = new CWSPPost(data, contentType, uri);
				pdu.SetHeaders(headers);
				
				return SMethodInvoke(pdu);
			}
		}

        /// <summary>
        /// Use this method to construct a GET-MethodInvoke.req.
        /// This method uses <code>methodInvoke(CWSPPDU pdu)</code>
        /// to send the constructed WSP-GET-PDU.
        /// </summary>
        /// <param name="uri">The Unfied Resource Identifier of the resource to GET</param>
        /// <returns></returns>
		public virtual CWSPMethodManager SGet(string uri)
		{
			lock (this)
			{
				CWSPGet pdu = new CWSPGet(uri);
				
				return SMethodInvoke(pdu);
			}
		}

        /// <summary>
        /// Use this method to construct a GET-MethodInvoke.req.
        /// This method uses <code>methodInvoke(CWSPPDU pdu)</code>
        /// to send the constructed WSP-GET-PDU.
        /// </summary>
        /// <param name="headers">The headers that are defined for the request</param>
        /// <param name="uri">The Unfied Resource Identifier of the resource to GET</param>
        /// <returns></returns>
		public virtual CWSPMethodManager SGet(CWSPHeaders headers, string uri)
		{
			lock (this)
			{
				CWSPGet pdu = new CWSPGet(uri);
				pdu.SetHeaders(headers);
				
				return SMethodInvoke(pdu);
			}
		}

        /// <summary>
        /// S-MethodInvoke.req
        /// To construct a POST- or GET-Request please use
        /// <code>get(String uri)</code> or
        /// <code>post(byte[] data, String contentType)</code>
        /// instead of this method.
        /// </summary>
        /// <param name="pdu">The GET - or POST-PDU to be sent.</param>
        /// <returns></returns>
	    public virtual CWSPMethodManager SMethodInvoke(CWSPPDU pdu)
		{
			lock (this)
			{
				if ((state != StateNull) && (state != StateSuspended))
				{
					CWSPMethodManager m = null;
					lock (methods.SyncRoot)
					{
						m = new CWSPMethodManager(pdu, this, upperlayer);
						methods.Add(m);
					}
					return m;
				}
				else
				{
					return null;
				}
			}
		}

        /// <summary>
        /// Removes the method.
        /// </summary>
        /// <param name="m">The m.</param>
		public virtual void  RemoveMethod(CWSPMethodManager m)
		{
			lock (methods.SyncRoot)
			{
				methods.Remove(m);
			}
		}



        /// <summary>
        /// Ss the connect CNF.
        /// </summary>
		private void  SConnectCnf()
		{
			lock (this)
			{
				//Logger.LogThis("s-connect.ind", LogLevel.Verbose);
				upperlayer.ConnectCnf();
			}
		}

        /// <summary>
        /// Ss the suspend ind.
        /// </summary>
        /// <param name="reason">The reason.</param>
		private void  SSuspendInd(short reason)
		{
			lock (this)
			{
				//Logger.LogThis("s-suspend.ind", LogLevel.Verbose);
				isSuspended = true;
				suspendCode = reason;
				upperlayer.SuspendInd(reason);
			}
		}

        /// <summary>
        /// Ss the resume CNF.
        /// </summary>
		private void  SResumeCnf()
		{
			lock (this)
			{
				//Logger.LogThis("s-resume.ind", LogLevel.Verbose);
				isSuspended = false;
				suspendCode = 0;
				upperlayer.ResumeCnf();
			}
		}

        /// <summary>
        /// Ss the disconnect ind.
        /// </summary>
        /// <param name="reason">The reason.</param>
		private void  SDisconnectInd(short reason)
		{
			lock (this)
			{
				//Logger.LogThis("s-disconnect.ind", LogLevel.Verbose);
				isDisconnected = true;
				disconnectCode = reason;
				upperlayer.DisconnectInd(reason);
				socket.Close();
			}
		}

        /// <summary>
        /// Ss the disconnect ind.
        /// </summary>
        /// <param name="redirectInfo">The redirect info.</param>
		private void  SDisconnectInd(IPAddress[] redirectInfo)
		{
			lock (this)
			{
				//Logger.LogThis("s-disconnect.ind - redirected", LogLevel.Verbose);
				isDisconnected = true;
				upperlayer.DisconnectInd(redirectInfo);
				socket.Close();
			}
		}

        /// <summary>
        /// Ss the disconnect ind.
        /// </summary>
        /// <param name="redirectInfo">The redirect info.</param>
		private void  SDisconnectInd(CWSPSocketAddress[] redirectInfo)
		{
			lock (this)
			{
				//Logger.LogThis("s-disconnect.ind - redirected", LogLevel.Verbose);
				isDisconnected = true;
				if (version == 2)
				{
					((IWSPUpperLayer2) upperlayer).DisconnectInd(redirectInfo);
				}
				else
				{
					IPAddress[] redirectInfo2 = new IPAddress[redirectInfo.Length];
					for (int i = 0; i < redirectInfo.Length; i++)
					{
						redirectInfo2[i] = redirectInfo[i].Address;
					}
					upperlayer.DisconnectInd(redirectInfo2);
				}
				socket.Close();
			}
		}
				
		//////////////////////////////////////// implementing IWTPUpperLayer  - TR-*.*
		
		/// <summary> process all TR-*.ind and TR-*.cnf service primitives except TR-Abort
		/// (call tr-abort(short abortReason) to indicate a Abort by TR).
		/// </summary>
		/// <param name="p">The WTP Service primitive
		/// </param>
		public virtual void  TrProcess(CWTPEvent p)
		{
			lock (this)
			{
				try
				{
					CWSPPDU pdu = null;
					
					if ((p.UserData != null) && (p.UserData.Length != 0))
					{
						pdu = CWSPPDU.GetPDU(p.UserData);
					}
					
					
					//Logger.LogThis(CWTPEvent.types[p.Type] + " in " + states[state], LogLevel.Verbose);
					
					
					switch (p.Type)
					{
						
						case (byte) (0x01):  //--------------------------------------------- TR-INVOKE.IND
							
							switch (state)
							{
								
								case StateConnecting: 
									
									if ((p.Transaction.ClassType == IWTPTransactionConstants.ClassType1) && (pdu.Type == CWSPPDU.PduTypeConfirmedPush))
									{
										p.Transaction.Abort(AbortProtoErr);
									}
									
									break;
								
								
								case StateConnectetd: 
									
									if ((p.Transaction.ClassType == IWTPTransactionConstants.ClassType0) && (pdu.Type == CWSPPDU.PduTypeDisconnect))
									{
										AbortAllMethods(AbortDisconnect);
										AbortAllPushes(AbortDisconnect);
										SDisconnectInd(AbortDisconnect);
										SetState(StateNull);
									}
									else if ((p.Transaction.ClassType == IWTPTransactionConstants.ClassType0) && (pdu.Type == CWSPPDU.PduTypePush))
									{
										/** @todo s_push_ind() */
									}
									else if ((p.Transaction.ClassType == IWTPTransactionConstants.ClassType1) && (pdu.Type == CWSPPDU.PduTypeConfirmedPush))
									{
										/** @todo start new push transaction with this event */
									}
									
									break;
								
								
								case StateSuspended: 
									
									if ((p.Transaction.ClassType == IWTPTransactionConstants.ClassType0) && (pdu.Type == CWSPPDU.PduTypeDisconnect))
									{
										SDisconnectInd(AbortDisconnect);
										SetState(StateNull);
									}
									else if ((p.Transaction.ClassType == IWTPTransactionConstants.ClassType1) && (pdu.Type == CWSPPDU.PduTypeConfirmedPush))
									{
										p.Transaction.Abort(AbortSuspend);
									}
									
									break;
								
								
								case StateResuming: 
									
									if ((p.Transaction.ClassType == IWTPTransactionConstants.ClassType0) && (pdu.Type == CWSPPDU.PduTypeDisconnect))
									{
										wtp.Abort(AbortDisconnect);
										AbortAllMethods(AbortDisconnect);
										SDisconnectInd(AbortDisconnect);
										SetState(StateNull);
									}
									else if ((p.Transaction.ClassType == IWTPTransactionConstants.ClassType1) && (pdu.Type == CWSPPDU.PduTypeConfirmedPush))
									{
										p.Transaction.Abort(AbortProtoErr);
									}
									break;
								}
							
							break;
						
						
						case (byte) (0x03):  //--------------------------------------------- TR-INVOKE.CNF
							
							switch (state)
							{
								
								case StateConnecting: 
									
									//ignore
									break;
								
								
								case StateConnectetd: 
									break;
								
								
								case StateSuspended: 
									
									// ignore
									break;
								
								
								case StateResuming: 
									// ignore
									break;
								}
							
							break;
						
						
						case (byte) (0x05):  //--------------------------------------------- TR-RESULT.IND
							
							switch (state)
							{
								
								case StateConnecting: 
									
									if (p.UserData.Length > MRU)
									{
										wtp.Abort(AbortMRUExceeded);
										AbortAllMethods(AbortConnectErr);
										SDisconnectInd(AbortMRUExceeded);
										SetState(StateNull);
									}
									else if (pdu.Type == CWSPPDU.PduTypeConnectReply)
									{
										CWSPConnectReply pdu2 = (CWSPConnectReply) pdu;
										SetState(StateConnectetd);
										
										CWTPEvent initPacket = new CWTPEvent(new byte[0], CWTPEvent.TrResultRes);
										wtp.Process(initPacket);
										sessionId = pdu2.ServerSessionID;
										SConnectCnf();
									}
									else if (pdu.Type == CWSPPDU.PduTypeRedirect)
									{
										CWTPEvent initPacket = new CWTPEvent(new byte[0], CWTPEvent.TrResultRes);
										wtp.Process(initPacket);
										AbortAllMethods(AbortConnectErr);
										
										CWSPRedirect pdu2 = (CWSPRedirect) pdu;
										if (version == 2)
										{
											SDisconnectInd(pdu2.SocketAddresses);
										}
										else
										{
											SDisconnectInd(pdu2.InetAddresses);
										}
										SetState(StateNull);
									}
									else if (pdu.Type == CWSPPDU.PduTypeReply)
									{
										CWTPEvent initPacket = new CWTPEvent(new byte[0], CWTPEvent.TrResultRes);
										wtp.Process(initPacket);
										AbortAllMethods(AbortConnectErr);
										
										CWSPReply pdu2 = (CWSPReply) pdu;
										SDisconnectInd(pdu2.GetStatus());
										SetState(StateNull);
									}
									else
									{
										wtp.Abort(AbortProtoErr);
										AbortAllMethods(AbortConnectErr);
										SDisconnectInd(AbortProtoErr);
										SetState(StateNull);
									}
									
									break;
								
								
								case StateConnectetd: 
									break;
								
								
								case StateSuspended: 
									break;
								
								
								case StateResuming: 
									
									if (p.UserData.Length > MRU)
									{
										wtp.Abort(AbortMRUExceeded);
										AbortAllMethods(AbortSuspend);
										SSuspendInd(AbortMRUExceeded);
										SetState(StateSuspended);
									}
									else if (pdu.Type == CWSPPDU.PduTypeReply)
									{
										CWSPReply pdu2 = (CWSPReply) pdu;
										
										if (pdu2.GetStatus() == CWSPReply._200OkSuccess)
										{
											CWTPEvent initPacket = new CWTPEvent(new byte[0], CWTPEvent.TrResultRes);
											wtp.Process(initPacket);
											SResumeCnf();
											SetState(StateConnectetd);
										}
										else
										{
											CWTPEvent initPacket = new CWTPEvent(new byte[0], CWTPEvent.TrResultRes);
											wtp.Process(initPacket);
											AbortAllMethods(AbortDisconnect);
											
											CWSPReply pdu3 = (CWSPReply) pdu;
											SDisconnectInd(pdu3.GetStatus());
											SetState(StateNull);
										}
									}
									else
									{
										wtp.Abort(AbortProtoErr);
										AbortAllMethods(AbortSuspend);
										SSuspendInd(AbortProtoErr);
										SetState(StateSuspended);
									}
									break;
								}
							
							break;
						
						
						case (byte) (0x07):  //--------------------------------------------- TR-RESULT.CNF
							
							switch (state)
							{
								
								case StateConnecting: 
									break;
								
								
								case StateConnectetd: 
									break;
								
								
								case StateSuspended: 
									break;
								
								case StateResuming: 
									break;
								}
							
							break;
						}
				}
				catch (EWSPCorruptPDUException e)
				{
					Logger.LogThis("Corrupt PDU: " + e.Message, LogLevel.Error);
				}
				catch (EWTPAbortedException e2)
				{
					Logger.LogThis("Aborted: " + e2.Message, LogLevel.Error);
				}
			}
		}
		
		/// <summary> TR-Abort.ind</summary>
		/// <param name="abortReason">The abort reason
		/// </param>
		public virtual void  TrAbort(short abortReason)
		{
			//Logger.LogThis("WSP Session: TR-ABORT.REQ in " + states[state], LogLevel.Verbose);
						
			if (state == StateConnecting)
			{
				AbortAllMethods(AbortConnectErr);
				SDisconnectInd(abortReason);
				SetState(StateNull);
			}
			else if (state == StateSuspended)
			{
				// ignore
			}
			else if (state == StateResuming)
			{
				if (abortReason == AbortDisconnect)
				{
					AbortAllMethods(AbortDisconnect);
					SDisconnectInd(AbortDisconnect);
					SetState(StateNull);
				}
				else
				{
					AbortAllMethods(AbortSuspend);
					SSuspendInd(abortReason);
					SetState(StateSuspended);
				}
			}
		}
		
		//////////////////////////////////////////////////////////////////////////////
		//////////////////////////////////////////////////////////// WSP pseudo events
		public virtual void  Disconnect()
		{
			socket.Close();
			
			if (state == StateConnecting)
			{
				wtp.Abort(AbortDisconnect);
				AbortAllMethods(AbortDisconnect);
				SDisconnectInd(AbortDisconnect);
				SetState(StateNull);
			}
			else if (state == StateConnectetd)
			{
				AbortAllMethods(AbortDisconnect);
				AbortAllPushes(AbortDisconnect);
				SDisconnectInd(AbortDisconnect);
				SetState(StateNull);
			}
			else if (state == StateSuspended)
			{
				SDisconnectInd(AbortDisconnect);
				SetState(StateNull);
			}
			else if (state == StateResuming)
			{
				wtp.Abort(AbortDisconnect);
				AbortAllMethods(AbortDisconnect);
				SDisconnectInd(AbortDisconnect);
				SetState(StateNull);
			}
		}
		
		public virtual void  Suspend()
		{
			if (state == StateConnecting)
			{
				wtp.Abort(AbortDisconnect);
				AbortAllMethods(AbortDisconnect);
				SDisconnectInd(AbortSuspend);
				SetState(StateNull);
			}
			else if (state == StateConnectetd)
			{
				// resume facility enabled!
				AbortAllMethods(AbortSuspend);
				AbortAllPushes(AbortSuspend);
				SDisconnectInd(AbortSuspend);
				SetState(StateSuspended);
			}
			else if (state == StateResuming)
			{
				wtp.Abort(AbortSuspend);
				AbortAllMethods(AbortSuspend);
				SSuspendInd(AbortSuspend);
				SetState(StateSuspended);
			}
		}
		
		//////////////////////////////////////////////////////////////////////////////
		////////////////////////////////////////////////////////////////////// HELPERS
		private void  AbortAllMethods(short reason)
		{
			while (methods.Count > 0)
			{
				((CWSPMethodManager) methods[0]).Abort(reason);
				methods.RemoveAt(0);
			}
		}
		
		private void  AbortAllPushes(short reason)
		{
			while (pushes.Count > 0)
			{
				((CWSPPushManager) pushes[0]).Abort(reason);
				pushes.RemoveAt(0);
			}
		}

        /// <summary>
        /// sets the state of the state machine
        /// </summary>
        /// <param name="state">the state to set</param>
		private void  SetState(short state)
		{			
			//Logger.LogThis(states[this.state] + " >>> " + states[state], LogLevel.Verbose);			
			
			this.state = state;
		}
		
		public virtual short GetState()
		{
			return state;
		}
		
		public virtual int GetMRU()
		{
			return MRU;
		}
		
	}
}