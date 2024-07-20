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
using System.Timers;

using MessagingToolkit.Wap.Wtp.Pdu;
using MessagingToolkit.Wap.Log;
using MessagingToolkit.Wap.Helper;
using MessagingToolkit.Wap.Wsp;


namespace MessagingToolkit.Wap.Wtp
{

    /// <summary>
    /// WTP responder
    /// </summary>
	public class CWTPResponder : IWTPTransaction
	{
		private class AcknowledgeActionListener
		{
			public AcknowledgeActionListener(CWTPResponder responder)
			{
				Init(responder);
			}
			private void  Init(CWTPResponder responder)
			{
				this.wtpResponder = responder;
			}
			private CWTPResponder wtpResponder;

			public CWTPResponder Responder
			{
				get
				{
					return wtpResponder;
				}
				
			}
			public virtual void  ActionPerformed(object eventSender, EventArgs e)
			{
				Responder.acknowledgeTimer.Stop();
				if (Responder.state == CWTPResponder.StateInvokeRespWait)
				{
					// check acknowledgement counter
					if (Responder.aec < CWTPResponder.AecMax)
					{
						//Logger.LogThis("acknowledgement timer abgelaufen: aec < AEC_MAX", LogLevel.Verbose);
						
						Responder.aec++;
                        // Reset changed to Stop and Start again
						Responder.acknowledgeTimer.Stop();
                        Responder.acknowledgeTimer.Start();
						Responder.State = CWTPResponder.StateResultRespWait;
					}
					else if (Responder.aec == CWTPResponder.AecMax)
					{
						//abort if acknowledgement counter exceeds the maximum
						Logger.LogThis("acknowledgement timer abgelaufen: aec == AEC_MAX", LogLevel.Verbose);
						
						
						CWTPAbort send = new CWTPAbort(Responder.sendTID);
						send.AbortReason = CWTPAbort.AbortReasonNoResponse;
						Responder.wtpSocket.Send(send);
						Responder.Close(CWTPAbort.AbortReasonNoResponse);
						Responder.aec = 0;
						Responder.upperLayer.TrAbort(CWTPAbort.AbortReasonNoResponse);
						Responder.State = CWTPResponder.StateListen;
					}
					else if (!Responder.uack && Responder.classType == IWTPTransactionConstants.ClassType1)
					{
						Logger.LogThis("acknowledgement timer abgelaufen: uack == false classtype 1", LogLevel.Verbose);
						
						CWTPAck send = new CWTPAck(Responder.sendTID);
						Responder.wtpSocket.Send(send);
						Responder.waitTimer.Stop();
                        Responder.waitTimer.Start();
						Responder.State = CWTPResponder.StateWaitTimeout;
					}
					else if (!Responder.uack && Responder.classType == IWTPTransactionConstants.ClassType2)
					{
						Logger.LogThis("acknowledgement timer abgelaufen: uack == false classtype 2", LogLevel.Verbose);
						
						CWTPAck ack = new CWTPAck(Responder.sendTID);
						Responder.wtpSocket.Send(ack);
						Responder.State = CWTPResponder.StateResultWait;
					}
				}
				else if (Responder.state == CWTPResponder.StateResultWait)
				{
					Logger.LogThis("acknowledgement timer abgelaufen.", LogLevel.Verbose);
					
					CWTPAck ack = new CWTPAck(Responder.sendTID);
					Responder.wtpSocket.Send(ack);
					Responder.State = CWTPResponder.StateResultWait;
				}
			}
		}
		
        private class RetryActionListener
		{
			public RetryActionListener(CWTPResponder wtpResponder)
			{
				Init(wtpResponder);
			}
			private void  Init(CWTPResponder wtpResponder)
			{
				this.responder = wtpResponder;
			}
			private CWTPResponder responder;

			public CWTPResponder Responder
			{
				get
				{
					return responder;
				}
				
			}
			public virtual void  ActionPerformed(object eventSender, System.EventArgs e)
			{
				Responder.retryTimer.Stop();
				if (Responder.state == CWTPResponder.StateResultRespWait)
				{
					// check retransmission counter
					if (Responder.rcr < CWTPResponder.RcrMax)
					{
						//Logger.LogThis("retransmission timer " + Responder.rcr + " mal abgelaufen. Re-sending Result.", LogLevel.Verbose);
						
						Responder.rcr++;
						Responder.retryTimer.Stop();
                        Responder.retryTimer.Start();
						// re-send recent Result
						Responder.wtpSocket.Send(Responder.sentPDU);
						Responder.State = CWTPResponder.StateResultRespWait;
					}
					else if (Responder.rcr == CWTPResponder.RcrMax)
					{
						Logger.LogThis("retransmission timer " + Responder.rcr + " mal abgelaufen. Abbruch!", LogLevel.Verbose);
						
						// abort
						Responder.Close(CWTPAbort.AbortReasonUnknown);
						Responder.upperLayer.TrAbort(CWTPAbort.AbortReasonUnknown);
						Responder.rcr = 0;
						Responder.State = CWTPResponder.StateListen;
					}
				}
			}
		}
		
        private class WaitActionListener
		{
			public WaitActionListener(CWTPResponder wtpResponder)
			{
				Init(wtpResponder);
			}
			private void  Init(CWTPResponder wtpResponder)
			{
				this.responder = wtpResponder;
			}
			
            private CWTPResponder responder;

			public CWTPResponder Responder
			{
				get
				{
					return responder;
				}
				
			}
			public virtual void  ActionPerformed(object eventSender, System.EventArgs e)
			{
				Responder.waitTimer.Stop();
				if (Responder.state == CWTPResponder.StateWaitTimeout)
				{
					//Logger.LogThis("wait timeout", LogLevel.Verbose);
					
					Responder.State = CWTPResponder.StateListen;
					Responder.Close((short) 0x00);
				}
			}
		}
		virtual public int TID
		{						
			get
			{
				return sendTID;
			}
			
		}
		virtual public byte ClassType
		{
			get
			{
				return classType;
			}
			
			set
			{
				if (value == 1 | value == 2 | value == 0)
				{
					this.classType = value;
					return ;
				}
				else
				{
					throw new ArgumentException("Class Type has to be 1, 2 or 3");
				}
			}
			
		}
		private byte State
		{
			set
			{
				Logger.LogThis(">>> WTP Responder: " + states[value] + "<<<", LogLevel.Verbose);
			}
			
		}
		virtual public bool Aborted
		{
			get
			{
				return aborted;
			}
			
		}
		virtual public short AbortCode
		{
			get
			{
				return abortCode;
			}
			
		}
		
		private const byte StateListen = (0x00);
		private const byte StateTIDOkWait = (0x01);
		private const byte StateInvokeRespWait = (0x02);
		private const byte StateResultWait = (0x03);
		private const byte StateResultRespWait = (0x04);
		private const byte StateWaitTimeout = (0x05);
		
		private static readonly string[] states = new string[]{"LISTEN", "TIDOK WAIT", "INVOKE RESP WAIT", "RESULT WAIT", "RESLUT RESP WAIT", "WAIT TIMEOUT"};
			
		private byte state = (0x00);
		
		// which session does this transaction belong to?
		private IWTPUpperLayer upperLayer;
		
		// used to send and receive
		private CWTPSocket wtpSocket;
		
		// is this transaction aborted?
		private bool aborted = false;
		private short abortCode;
		
		/// <summary> 
        /// 5.3.1.7
		/// Class Type 1, 2 or 3
		/// </summary>
		private byte classType;
		
		/// <summary> 9.4.1
		/// ack interval
		/// this sets a bound for the amount of time to wait before sending an acknowledgement.
		/// </summary>
		private Timer acknowledgeTimer;
		private int acknowledgeInterval = 5000;
		
		/// <summary> 9.4.1
		/// retry interval
		/// This sets a bound for the amount of time to wait before re-transmitting a PDU.
		/// </summary>
		private Timer retryTimer;
		private int retryInterval = 10000;
		
		/// <summary> 9.4.1
		/// wait timeout interval (only class 2 initiator and class 1 responder)
		/// This sets a bound for the amount of time to wait before state information
		/// about a transaction is released.
		/// </summary>
		private Timer waitTimer;
		private int waitInterval = 5000;
		
		/// <summary> 9.4.2
		/// re-transmission counter
		/// acknowledgement expireation counter
		/// </summary>
		private int rcr = 0;
		private int aec = 0;
		
		public const int RcrMax = 3;
		public const int AecMax = 3;
		
		/// <summary> 9.4.3
		/// counter to generate unique TIDs
		/// uint16
		/// </summary>
		private static int genTID = 0;
		
		/// <summary> uint16</summary>
		private int sendTID;
		private int rcvTID;
		private int lastTID;
		
		/// <summary> recently sent PDU - hold it for retransmission</summary>
		private CWTPPDU sentPDU;
		
		private bool holdOn = false;
		private bool uack = false;


        /// <summary>
        /// Constructs a CWTPSocket using a DatagramSocket (UDP).
        /// Even implementing all parameters belonging to according to
        /// TR-Invoke service primitive (section 5.3.1),
        /// which are not temporary for one transaction.
        /// This socket can be used several times by service primitives.
        /// After all it has to be closed by calling <code>close()</code>
        /// </summary>
        /// <param name="wtpSocket">The WTP socket.</param>
        /// <param name="upperLayer">The upper layer.</param>
        /// <param name="initPDU">The init PDU.</param>
        /// <param name="ackType">Ack Type (section 5.3.1.5)</param>
        /// <param name="classtype">The classtype.</param>
        /// <seealso cref="close(short)">
        /// </seealso>
        public CWTPResponder(CWTPSocket wtpSocket, IWTPUpperLayer upperLayer, CWTPInvoke initPDU, bool ackType, byte classtype)
		{
			Init(wtpSocket, upperLayer, ackType, classtype);
			
			rcvTID = initPDU.GetTID();
			sendTID = rcvTID - 32768; // sendTID = rcvTID XOR 0x8000
			
			// process the invoke pdu
			try
			{
				Process(initPDU);
			}
			catch (EWTPAbortedException e)
			{
				Logger.LogThis("PDU processing aborted: " + e.Message, LogLevel.Error);
			}
		}

        /// <summary>
        /// Inits the specified WTP socket.
        /// </summary>
        /// <param name="wtpSocket">The WTP socket.</param>
        /// <param name="upperLayer">The upper layer.</param>
        /// <param name="ackType">if set to <c>true</c> [ack type].</param>
        /// <param name="classtype">The classtype.</param>
		private void  Init(CWTPSocket wtpSocket, IWTPUpperLayer upperLayer, bool ackType, byte classtype)
		{
			this.upperLayer = upperLayer;
			this.wtpSocket = wtpSocket;
			wtpSocket.AddTransaction(this);
			uack = ackType;
			ClassType = classtype;
			
			// initialize timer and add actionListener
			// see declaration of a_timer above
			acknowledgeTimer = new System.Timers.Timer();
			acknowledgeTimer.Elapsed += new System.Timers.ElapsedEventHandler(new AcknowledgeActionListener(this).ActionPerformed);
			acknowledgeTimer.Interval = acknowledgeInterval;
			
			// see declaration of r_timer above
			retryTimer = new System.Timers.Timer();
			retryTimer.Elapsed += new System.Timers.ElapsedEventHandler(new RetryActionListener(this).ActionPerformed);
			retryTimer.Interval = retryInterval;
			
			// see declaration of w_timer above
			waitTimer = new System.Timers.Timer();
			waitTimer.Elapsed += new System.Timers.ElapsedEventHandler(new WaitActionListener(this).ActionPerformed);
			waitTimer.Interval = waitInterval;
		}


        /// <summary>
        /// Invoked by the run()-Method of the Management-Entity CWTPManagement.
        /// Processes given protocol data units
        /// according to state machine described in section 9.5
        /// <b>Notice:</b> Only WTP Initiator is implemented!
        /// </summary>
        /// <param name="pdu">the pdu to be processed in the state machine</param>
		public virtual void  Process(CWTPPDU pdu)
		{
			lock (this)
			{
				if (aborted)
				{
					throw new EWTPAbortedException(abortCode);
				}
				switch (state)
				{
					
					///////////////////// STATE LISTEN ///////////////////////////////////////
					case (0x00): 
						// invoke pdu in state listen
						if (pdu.PDUType == CWTPPDU.PduTypeInvoke)
						{
							if (this.classType == IWTPTransactionConstants.ClassType1 || this.classType == IWTPTransactionConstants.ClassType2)
							{
								if (true)
								{
									// TID OK
                                    acknowledgeTimer.Stop();
                                    acknowledgeTimer.Start();
									CWTPEvent initPacket = new CWTPEvent(pdu.Payload, CWTPEvent.TrInvokeInd);
									initPacket.Transaction = this;
									upperLayer.TrProcess(initPacket);
									State = StateInvokeRespWait;
								}
								else
								{
									// TID not OK
									CWTPAck ack = new CWTPAck(sendTID);
									ack.TveTok = true;
									wtpSocket.Send(ack);
									State = StateTIDOkWait;
								}
							}
							else if (classType == IWTPTransactionConstants.ClassType0)
							{
								CWTPEvent initPacket = new CWTPEvent(pdu.Payload, CWTPEvent.TrInvokeInd);
								initPacket.Transaction = this;
								upperLayer.TrProcess(initPacket);
								State = StateListen;
							}
						} // end invoke PDU in listen
						break;
						
						//////////////////// STATE TIDOK WAIT ////////////////////////////////////
					
					case (0x01): 
						if (pdu.PDUType == CWTPPDU.PduTypeAck && (classType == IWTPTransactionConstants.ClassType1 || classType == IWTPTransactionConstants.ClassType2) && (true))
						{
							CWTPEvent initPacket = new CWTPEvent(pdu.Payload, CWTPEvent.TrInvokeInd);
							initPacket.Transaction = this;
							upperLayer.TrProcess(initPacket);
                            acknowledgeTimer.Stop();
                            acknowledgeTimer.Start();
							State = StateInvokeRespWait;
						}
						else if (pdu.PDUType == CWTPPDU.PduTypeAbort)
						{
							short abortReason = ((CWTPAbort) pdu).AbortReason;
							Close(abortReason);
							upperLayer.TrAbort(abortReason);
							State = StateListen;
						}
						else if (pdu.PDUType == CWTPPDU.PduTypeInvoke)
						{
							if (pdu.GetRID())
							{
								CWTPAck ack = new CWTPAck(sendTID);
								ack.TveTok = true;
								wtpSocket.Send(ack);
								State = StateTIDOkWait;
							}
							else
							{
								// ignore
								State = StateTIDOkWait;
							}
						}
						break;
						
						///////////////////// STATE INVOKE RESP WAIT /////////////////////////////
					
					case (0x02): 
						if (pdu.PDUType == CWTPPDU.PduTypeAbort)
						{
							short abortReason = ((CWTPAbort) pdu).AbortReason;
							Close(abortReason);
							upperLayer.TrAbort(abortReason);
							State = StateListen;
						}
						else if (pdu.PDUType == CWTPPDU.PduTypeInvoke)
						{
							// ignore
							State = StateInvokeRespWait;
						}
						break;
						
						///////////////////// STATE RESULT WAIT //////////////////////////////////
					
					case (0x03): 
						if (pdu.PDUType == CWTPPDU.PduTypeInvoke)
						{
							if (pdu.GetRID())
							{
								if (true)
								{
									// resend Ack PDU
									State = StateResultWait;
								}
								else
								{
									// ignore
									State = StateResultWait;
								}
							}
							else
							{
								// ignore
								State = StateResultWait;
							}
						}
						else if (pdu.PDUType == CWTPPDU.PduTypeAbort)
						{
							short abortReason = ((CWTPAbort) pdu).AbortReason;
							Close(abortReason);
							upperLayer.TrAbort(abortReason);
							State = StateListen;
						}
						break;
						///////////////////// STATE RESULT RESP WAIT /////////////////////////////
					
					case (0x04): 
						if (pdu.PDUType == CWTPPDU.PduTypeAbort)
						{
							short abortReason = ((CWTPAbort) pdu).AbortReason;
							Close(abortReason);
							upperLayer.TrAbort(abortReason);
							State = StateListen;
						}
						else if (pdu.PDUType == CWTPPDU.PduTypeAck)
						{
							CWTPAck pduAck = (CWTPAck) pdu;
							if (pduAck.TveTok)
							{
								// ignore
								State = StateResultRespWait;
							}
							else
							{
								CWTPEvent p = new CWTPEvent(pdu.Payload, CWTPEvent.TrResultCnf);
								upperLayer.TrProcess(p);
								State = StateListen;
							}
						}
						break;
						///////////////////// STATE WAIT TIMEOUT /////////////////////////////////
					
					case (0x05): 
						if (pdu.PDUType == CWTPPDU.PduTypeInvoke)
						{
							CWTPInvoke invokepdu = (CWTPInvoke) pdu;
							if (invokepdu.GetRID())
							{
								CWTPAck ackpdu = new CWTPAck(sendTID);
								/** @todo input exitInfo TPI if available seite 56 */
								wtpSocket.Send(ackpdu);
								State = StateWaitTimeout;
							}
							else
							{
								// ignore
								State = StateWaitTimeout;
							}
						}
						else if (pdu.PDUType == CWTPPDU.PduTypeAck)
						{
							CWTPAck acki = (CWTPAck) pdu;
							if (acki.TveTok && acki.GetRID())
							{
								CWTPAck ackpdu = new CWTPAck(sendTID);
								/** @todo input exitInfo TPI if available seite 56 */
								wtpSocket.Send(ackpdu);
								State = StateWaitTimeout;
							}
						}
						else if (pdu.PDUType == CWTPPDU.PduTypeAbort)
						{
							short abortReason = ((CWTPAbort) pdu).AbortReason;
							Close(abortReason);
							upperLayer.TrAbort(abortReason);
							State = StateListen;
						}
						break;
					}
			}
		}
		
		
		/// <summary> Invoked by higher layers to process given service primitives
		/// according to state machine described in section 9.5.<br>
		/// <b>Notice:</b> Only WTP Initiator is implemented!
		/// 
		/// </summary>
		/// <param name="p">the Service Primitive to be processed
		/// </param>
		public virtual void  Process(CWTPEvent p)
		{
			lock (this)
			{
				if (aborted)
				{
					throw new EWTPAbortedException(abortCode);
				}
				switch (state)
				{
					
					///////////////////// STATE LISTEN ///////////////////////////////////////
					//case 0x00:
					//not possible
					//////////////////// STATE TIDOK WAIT ////////////////////////////////////
					//case 0x01:
					//not possible
					///////////////////// STATE INVOKE RESP WAIT /////////////////////////////
					case (0x02): 
						if (p.Type == CWTPEvent.TrInvokeRes)
						{
							if (classType == IWTPTransactionConstants.ClassType1)
							{
								/** @todo input exitinfo tpi if availabe */
								CWTPAck ack = new CWTPAck(sendTID);
								wtpSocket.Send(ack);
                                waitTimer.Stop();
                                waitTimer.Start();
								State = StateWaitTimeout;
							}
							else if (classType == IWTPTransactionConstants.ClassType2)
							{
                                acknowledgeTimer.Stop();
                                acknowledgeTimer.Start();
								State = StateResultWait;
							}
						}
						else if (p.Type == CWTPEvent.TrResultReq)
						{
							rcr = 0;
							sentPDU = new CWTPResult(p.UserData, sendTID);
							wtpSocket.Send(sentPDU);
                            retryTimer.Stop();
                            retryTimer.Start();
							State = StateResultRespWait;
						}
						break;
						///////////////////// STATE RESULT WAIT //////////////////////////////////
					
					case (0x03): 
						if (p.Type == CWTPEvent.TrResultReq)
						{
							rcr = 0;
							sentPDU = new CWTPResult(p.UserData, sendTID);
							wtpSocket.Send(sentPDU);
                            retryTimer.Stop();
                            retryTimer.Start();
							State = StateResultRespWait;
						}
						break;
						///////////////////// STATE RESULT RESP WAIT /////////////////////////////
						//case 0x04:
						//not possible
						///////////////////// STATE WAIT TIMEOUT /////////////////////////////////
						//case 0x05:
						//not possible
					}
			}
		}

        /// <summary>
        /// RcvErrorPDU
        /// </summary>
        /// <param name="e">exception thrown by CWTPFactory</param>
		public virtual void  Process(EWTPCorruptPDUException e)
		{
			CWTPAbort abort = new CWTPAbort(sendTID);
			abort.AbortReason = CWTPAbort.AbortReasonProtoErr;
			wtpSocket.Send(abort);
			if (state != StateListen)
			{
				if (state != StateTIDOkWait)
				{
					upperLayer.TrAbort(CWTPAbort.AbortReasonProtoErr);
				}
				Close(CWTPAbort.AbortReasonProtoErr);
				State = StateListen;
			}
		}

        /// <summary>
        /// use this method to invoke a TR-ABORT.REQ by the upper Layer
        /// </summary>
		public virtual void  Abort()
		{
			Abort(CWTPAbort.AbortReasonUnknown);
		}

        /// <summary>
        /// use this method to invoke a TR-ABORT.REQ by the upper Layer
        /// </summary>
        /// <param name="abortReason">The abort reason.</param>
		public virtual void  Abort(short abortReason)
		{
			if (state == StateInvokeRespWait || state == StateResultWait || state == StateResultRespWait || state == StateWaitTimeout)
			{
				Close(abortReason);
				CWTPAbort abort = new CWTPAbort(sendTID);
				abort.AbortReason = abortReason;
				State = StateListen;
			}
		}

        /// <summary>
        /// Closes the specified reason code.
        /// </summary>
        /// <param name="reasonCode">The reason code.</param>
		public virtual void  Close(short reasonCode)
		{
			abortCode = reasonCode;
			aborted = true;
			retryTimer.Stop();
			waitTimer.Stop();
			acknowledgeTimer.Stop();
			State = StateListen;
			wtpSocket.RemoveTransaction(this);
		}
		
	}
}