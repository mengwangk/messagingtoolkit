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
using System.IO;
using System.Timers;

using MessagingToolkit.Wap.Wtp.Pdu;
using MessagingToolkit.Wap.Log;
using MessagingToolkit.Wap.Helper;
using MessagingToolkit.Wap.Wsp;


namespace MessagingToolkit.Wap.Wtp
{


    /// <summary>
    /// This class implements a WTP state machine of one transaction according
    /// to the WTP specification by the WAP Forum. It uses CWTPManagement to send
    /// the PDUs. CWTPManagement receives PDUs from the remote, decodes them and
    /// calls #process(CWTPPDU pdu) of the corresponding tranaction (this class).
    /// <p>
    /// To be informed of thrown service primitives by this layer, you should
    /// implement the interface IWTPUpperLayer and give it with the constructor.
    /// </p><p>
    /// To construct a service primitive to be processed by this WTP-socket,
    /// use objects of the class CWTPEvent, that implement service primitives.
    /// </p>
    /// 	<p>
    /// 		<b>Notice</b>, that development is actually in progress.
    /// Most features are implemented but only for Initiator, not for a responder!
    /// (for a definition of these terms refer to the spec, section 3.2.)
    /// </p><p>
    /// 		<b>Section descriptions</b> used in this class refer to
    /// WTP Specification WAP-224-WTP-20010710-a
    /// by the <a href="http://www.wapforum.org">WAP Forum</a>
    /// 	</p>
    /// </summary>
    public class CWTPInitiator : IWTPTransaction
    {
        private class AcknowledgeActionListener
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="AcknowledgeActionListener"/> class.
            /// </summary>
            /// <param name="initiator">The initiator.</param>
            public AcknowledgeActionListener(CWTPInitiator initiator)
            {
                Init(initiator);
            }
            private void Init(CWTPInitiator initiator)
            {
                this.cwtpInitiator = initiator;
            }

            private CWTPInitiator cwtpInitiator;

            public CWTPInitiator Initiator
            {
                get
                {
                    return cwtpInitiator;
                }

            }
            public virtual void ActionPerformed(object eventSender, EventArgs e)
            {
                Initiator.acknowledgeTimer.Stop();

                if (Initiator.state == CWTPInitiator.StateResultRespWait)
                {
                    // check acknowledgement counter
                    if (!Initiator.uack)
                    {
                        // if user acknowledgement is turned off
                        Logger.LogThis("acknowledgement timer: exceeded " + (Initiator.aec + 1) + " time(s) --> cancel", LogLevel.Error);

                        CWTPAck send = new CWTPAck(Initiator.sendTID);
                        Initiator.wtpSocket.Send(send);
                        Initiator.waitTimer.Stop();
                        Initiator.waitTimer.Start();
                        Initiator.State = CWTPInitiator.StateWaitTimeout;
                    }
                    else if (Initiator.aec < CWTPInitiator.AecMax)
                    {

                        Logger.LogThis("acknowledgement timer: exceeded " + (Initiator.aec + 1) + " time(s).", LogLevel.Verbose);

                        Initiator.aec++;
                        Initiator.acknowledgeTimer.Stop();
                        Initiator.acknowledgeTimer.Start();
                        Initiator.State = CWTPInitiator.StateResultRespWait;
                    }
                    else if (Initiator.aec == CWTPInitiator.AecMax)
                    {
                        //abort if acknowledgement counter exceeds the maximum
                        Logger.LogThis("acknowledgement timer: exceeded " + (Initiator.aec + 1) + " time(s) --> cancel", LogLevel.Error);

                        CWTPAbort send = new CWTPAbort(Initiator.sendTID);
                        send.AbortReason = CWTPAbort.AbortReasonNoResponse;
                        Initiator.wtpSocket.Send(send);
                        Initiator.Close(CWTPAbort.AbortReasonNoResponse);
                        Initiator.aec = 0;
                        Initiator.State = CWTPInitiator.StateNull;
                    }
                }
            }
        }

        /// <summary>
        /// Retry action listener
        /// </summary>
        private class RetryActionListener
        {
            public RetryActionListener(CWTPInitiator initiator)
            {
                Init(initiator);
            }
            private void Init(CWTPInitiator initiator)
            {
                this.cwtpInitiator = initiator;
            }
            private CWTPInitiator cwtpInitiator;

            public CWTPInitiator Initiator
            {
                get
                {
                    return cwtpInitiator;
                }

            }
            public virtual void ActionPerformed(object eventSender, EventArgs e)
            {
                Initiator.retryTimer.Stop();

                if (Initiator.state == CWTPInitiator.StateResultWait)
                {
                    // check retransmission counter
                    if (Initiator.rcr < CWTPInitiator.RcrMax)
                    {
                        /** @todo Ack(TIDok) already sent? Page 53 */

                        Logger.LogThis("retransmission timer: exceeded " + (Initiator.rcr + 1) + " time(s) --> re-send", LogLevel.Verbose);

                        Initiator.rcr++;
                        Initiator.retryTimer.Stop();
                        Initiator.retryTimer.Start();


                        // re-send recent Invoke
                        Initiator.sentPDU.SetRID(true);
                        Initiator.wtpSocket.Send(Initiator.sentPDU);
                        Initiator.State = CWTPInitiator.StateResultWait;
                    }
                    else if (Initiator.rcr == CWTPInitiator.RcrMax)
                    {
                        Logger.LogThis("retransmission timer: exceeded " + (Initiator.rcr + 1) + " time(s) --> cancel", LogLevel.Error);

                        // abort
                        Initiator.Close(CWTPAbort.AbortReasonUnknown);
                        Initiator.upperLayer.TrAbort(CWTPAbort.AbortReasonUnknown);
                        Initiator.rcr = 0;
                        Initiator.State = CWTPInitiator.StateNull;
                    }
                }
            }
        }

        /// <summary>
        /// Wait action listener
        /// </summary>
        private class WaitActionListener
        {
            public WaitActionListener(CWTPInitiator initiator)
            {
                InitBlock(initiator);
            }

            private void InitBlock(CWTPInitiator initiator)
            {
                this.cwtpInitiator = initiator;
            }
            private CWTPInitiator cwtpInitiator;

            public CWTPInitiator Initiator
            {
                get
                {
                    return cwtpInitiator;
                }
            }

            public virtual void ActionPerformed(object event_sender, EventArgs e)
            {
                Initiator.waitTimer.Stop();

                if (Initiator.state == CWTPInitiator.StateWaitTimeout)
                {
                    //Logger.LogThis("wait timeout", LogLevel.Verbose);
                    Initiator.State = CWTPInitiator.StateNull;
                    Initiator.Close((short)0x00);
                }
            }
        }

        private class SegmentAckActionListener
        {
            public SegmentAckActionListener(CWTPInitiator initiator)
            {
                Init(initiator);
            }
            private void Init(CWTPInitiator initator)
            {
                this.cwtpInitiator = initator;
            }
            private CWTPInitiator cwtpInitiator;

            public CWTPInitiator Initiator
            {
                get
                {
                    return cwtpInitiator;
                }

            }
            public virtual void ActionPerformed(object eventSender, EventArgs e)
            {
                Initiator.segmentAckTimer.Stop();

                if (Initiator.segmentSended > Initiator.segmentSendedMax)
                {
                    Logger.LogThis("wait_ack timeout (" + Initiator.segmentSended + "), abort.", LogLevel.Verbose);

                    Initiator.segmentSended = 1;
                    Initiator.State = CWTPInitiator.StateNull;
                    Initiator.Close((short)0x00);
                }
                else
                {
                    try
                    {
                        if (Initiator.state == CWTPInitiator.StateWaitAck)
                        {
                            if (!Initiator.wtpSocket.Send(Initiator.segment))
                            {
                                Logger.LogThis("wait_ack timeout, abort.", LogLevel.Verbose);
                                Initiator.segmentSended = 1;
                                Initiator.State = CWTPInitiator.StateNull;
                                Initiator.Close((short)0x00);
                            }
                            else
                            {
                                /*
                                Logger.LogThis("wait_ack timeout (" + Initiator.segmentSended + "), abort.", LogLevel.Verbose);
                                Initiator.segmentSended = 1;
                                Initiator.State = CWTPInitiator.StateNull;
                                Initiator.Close((short)0x00);
                                */
                               
                                Logger.LogThis("wait_ack timeout (" + Initiator.segmentSended + "), re-sending segment.", LogLevel.Verbose);
                                Initiator.segment.SetRID(true);
                                Initiator.segmentSended++;
                                Initiator.segmentAckTimer.Stop();
                                Initiator.segmentAckTimer.Start();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.LogThis("wait_ack timeout (" + Initiator.segmentSended + "), abort.", LogLevel.Verbose);
                        Initiator.segmentSended = 1;
                        Initiator.State = CWTPInitiator.StateNull;
                        Initiator.Close((short)0x00);
                    }
                }
            }
        }

        virtual public int TID
        {
            /*
            public boolean getAckType(){
            return uack;
            }
            public void setAckType(boolean ackType){
            uack = ackType;
            }
			
			
            public IWTPUpperLayer getUpperLayer(){
            return upperLayer;
            }
            */

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
                if ((value == 1) | (value == 2) | (value == 0))
                {
                    this.classType = value;

                    return;
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
                //if (debug){
                //  log.log(0, this, "" + sendTID + ": " + states[this.state] + " >>> " + states[state]);
                //}
                this.state = value;
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

        private const byte StateNull = (0x00);
        private const byte StateResultWait = (0x01);
        private const byte StateResultRespWait = (0x02);
        private const byte StateWaitTimeout = (0x03);
        private const byte StateWaitAck = (0x04);
        private static readonly string[] States = new string[] { "NULL", "RESULT WAIT", "RESULT RESP WAIT", "WAIT TIMEOUT", "WAIT ACK" };
        public const int RcrMax = 3;
        public const int AecMax = 3;

        /// <summary> 9.4.3
        /// counter to generate unique TIDs
        /// uint16
        /// </summary>
        private static int genTID = -1;
        private byte state = 0x00;

        // which session does this transaction belong to?
        private IWTPUpperLayer upperLayer;

        // used to send and receive
        private CWTPSocket wtpSocket;

        // is this transaction aborted?
        private bool aborted = false;
        private short abortCode;

        /// <summary> 5.3.1.7
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

        /// <summary> 7.14.3
        /// While segmentation each segment has to be acknowledged. This timer
        /// waits before resending a segment.
        /// </summary>
        private Timer segmentAckTimer;
        private int segmentAckInterval = 8000;
        private CWTPPDU segment;
        private int segmentSended = 1;
        private int segmentSendedMax = 4;

        /// <summary> 9.4.2
        /// re-transmission counter
        /// acknowledgement expireation counter
        /// </summary>
        private int rcr = 0;
        private int aec = 0;

        /// <summary> uint16</summary>
        private int sendTID;
        private int rcvTID;
        private int lastTID;

        /// <summary> recently sent PDU - hold it for retransmission</summary>
        private CWTPPDU sentPDU;
        private bool holdOn = false;
        private bool uack = false;

        /// <summary> next segment to send while in STATE_WAIT_ACK</summary>
        private int segmentIndex = 0;
        private MemoryStream segdata;
        private int segend;
        private short seglast;
        private int segTID;

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
        /// <param name="initPacket">The init packet.</param>
        /// <param name="ackType">Ack Type (section 5.3.1.5)</param>
        /// <param name="classType">Class Type (0, 1 or 2) (section 5.3.1.8)</param>
        /// <seealso cref="close(short)">
        /// </seealso>
        /// <throws>  IllegalArgumentException </throws>
        /// <throws>  SocketException </throws>
        public CWTPInitiator(CWTPSocket wtpSocket, IWTPUpperLayer upperLayer, CWTPEvent initPacket, bool ackType, byte classType)
        {
            Initialize(wtpSocket, upperLayer, ackType, classType);

            sendTID = CWTPInitiator.GenerateNewTID();
            rcvTID = sendTID - 32768; // rcvTID = sendTID XOR 0x8000

            // process the init service primitive
            try
            {
                Process(initPacket);
            }
            catch (EWTPAbortedException e)
            {
                Logger.LogThis("Event processing aborted: " + e.Message, LogLevel.Warn);
            }
        }

        /// <summary>
        /// Inits the specified WTP_ socket.
        /// </summary>
        /// <param name="wtp_Socket">The WTP_ socket.</param>
        /// <param name="upper_Layer">The upper_ layer.</param>
        /// <param name="ackType">if set to <c>true</c> [ack type].</param>
        /// <param name="classType">Type of the class.</param>
        private void Initialize(CWTPSocket wtpSocket, IWTPUpperLayer upperLayer, bool ackType, byte classType)
        {
            this.upperLayer = upperLayer;
            this.wtpSocket = wtpSocket;
            uack = ackType;
            ClassType = classType;

            // initialize timer and add actionListener
            // see declaration of a_timer above
            acknowledgeTimer = new Timer();
            acknowledgeTimer.Elapsed += new ElapsedEventHandler(new AcknowledgeActionListener(this).ActionPerformed);
            acknowledgeTimer.Interval = acknowledgeInterval;

            // see declaration of r_timer above
            retryTimer = new Timer();
            retryTimer.Elapsed += new ElapsedEventHandler(new RetryActionListener(this).ActionPerformed);
            retryTimer.Interval = retryInterval;

            // see declaration of w_timer above
            waitTimer = new Timer();
            waitTimer.Elapsed += new ElapsedEventHandler(new WaitActionListener(this).ActionPerformed);
            waitTimer.Interval = waitInterval;

            // see declaration of w_timer above
            segmentAckTimer = new Timer();
            segmentAckTimer.Elapsed += new ElapsedEventHandler(new SegmentAckActionListener(this).ActionPerformed);
            segmentAckTimer.Interval = segmentAckInterval;
        }


        /// <summary>
        /// Invoked by the run()-Method of the Management-Entity CWTPManagement.
        /// Processes given protocol data units
        /// according to state machine described in section 9.5
        /// <b>Notice:</b> Only WTP Initiator is implemented!
        /// </summary>
        /// <param name="pdu">the pdu to be processed in the state machine</param>
        public virtual void Process(CWTPPDU pdu)
        {
            lock (this)
            {
                if (aborted)
                {
                    throw new EWTPAbortedException(abortCode);
                }


                //Logger.LogThis("" + sendTID + ": " + CWTPPDU.Types[pdu.PDUType] + " in " + States[state] + " class " + classType + " holdOn " + holdOn, LogLevel.Verbose);


                switch (state)
                {

                    ///////////////////// NULL //////////////////////////////////////////////
                    case (0x00):

                        // because only initiator is implemented,
                        // we do not need to react on pdu-events in NULL state
                        break;

                    ///////////////////// RESULT WAIT ///////////////////////////////////////

                    case (0x01):

                        // RcvAck in RESULT WAIT
                        if (pdu.PDUType == CWTPPDU.PduTypeAck)
                        {
                            if (((CWTPAck)pdu).TveTok && ((classType == 1) || (classType == 2)) && (rcr < RcrMax))
                            {
                                CWTPAck send = new CWTPAck(((CWTPAck)pdu).GetTID() & 0x7fff);
                                send.TveTok = true;
                                wtpSocket.Send(send);
                                rcr++;
                                retryTimer.Stop();
                                retryTimer.Start();
                                State = StateResultWait;
                            }
                            else if (((CWTPAck)pdu).TveTok && ((classType == 1) || (classType == 2)))
                            {
                                State = StateResultWait;
                            }
                            else if ((classType == 2) && !holdOn)
                            {
                                retryTimer.Stop();
                                upperLayer.TrProcess(new CWTPEvent(pdu.Payload, CWTPEvent.TrInvokeCnf));
                                this.holdOn = true;
                                State = CWTPInitiator.StateResultWait;
                            }
                            else if ((classType == 2) && holdOn)
                            {
                                State = StateResultWait;
                            }
                            else if (classType == 1)
                            {
                                retryTimer.Stop();
                                upperLayer.TrProcess(new CWTPEvent(pdu.Payload, CWTPEvent.TrInvokeCnf));
                                State = StateNull;
                            }
                        }
                        // RcvAbort in RESULT WAIT
                        else if (pdu.PDUType == CWTPPDU.PduTypeAbort)
                        {
                            short abortReason = ((CWTPAbort)pdu).AbortReason;
                            Close(abortReason);
                            upperLayer.TrAbort(abortReason);
                            State = StateNull;
                        }
                        // RcvResult in RESULT WAIT
                        else if (pdu.PDUType == CWTPPDU.PduTypeResult && classType == 2)
                        {
                            CWTPResult res = (CWTPResult)pdu;
                            byte nextState = StateResultRespWait;

                            // Stop timer
                            retryTimer.Stop();

                            // Check for segmentation indication
                            if (!res.GetTTR())
                            {
                                Logger.LogThis("Not last packet", LogLevel.Verbose);
                                segend = 0; seglast = 0;
                                segdata = new System.IO.MemoryStream(64 * 1024); // 64kbyte segs
                                segTID = pdu.GetTID();
                                try
                                {
                                    segdata.Write(res.payload, 0, res.payload.Length);
                                }
                                catch (IOException e)
                                {
                                    Logger.LogThis("Exception writing to ByteArrayOutputStream: " + e.Message, LogLevel.Warn);
                                }
                                nextState = StateResultWait; // We will wait for more to come
                            }
                            State = nextState;

                            // if !holdOn generate TR_Invoke.cnf
                            if (!holdOn)
                            {
                                upperLayer.TrProcess(new CWTPEvent(pdu.Payload, CWTPEvent.TrInvokeCnf));
                            }

                            if (!res.GetTTR() && res.GetGTR())
                            {
                                byte[] payload = new byte[2];
                                CWTPAck ack = new CWTPAck(sendTID);
                                payload[0] = (0x03 << 3) | 0x01; // psn, one byte length
                                payload[1] = 0; // Implicit sequence number 0
                                ack.SetCON(true); // headers follow ;)
                                ack.Payload = payload;
                                //Logger.LogThis("sending ack:" + ack.ToString(), LogLevel.Verbose);

                                wtpSocket.Send(ack);
                            }

                            if (nextState != StateResultWait)
                            {
                                // generate tr_result.ind
                                upperLayer.TrProcess(new CWTPEvent(pdu.Payload, CWTPEvent.TrResultInd));
                            }
                            // restart ack timer
                            acknowledgeTimer.Stop();
                            acknowledgeTimer.Start();
                        }
                        else if (pdu.PDUType == CWTPPDU.PduTypeSegmResult)
                        {
                            Logger.LogThis("Segmented result", LogLevel.Verbose);
                            bool sendAck = false;
                            bool sendToUpper = false;
                            CWTPSegmResult sres = (CWTPSegmResult)pdu;

                            waitTimer.Stop();
                            waitTimer.Start();
                            acknowledgeTimer.Stop();
                            acknowledgeTimer.Start();

                            if (sres.GetTID() != segTID)
                            {
                                Logger.LogThis("Dumping out-of sync segment", LogLevel.Warn);
                                return;
                            }
                            if (seglast + 1 == sres.PSN)
                            {
                                // we are in line with stuff
                                seglast = sres.PSN;
                                try
                                {
                                    segdata.Write(sres.Payload, 0, sres.Payload.Length);
                                }
                                catch (IOException e)
                                {
                                    Logger.LogThis("Exception writing to ByteArrayOutputStream: " + e.Message, LogLevel.Warn);
                                }
                                if (sres.GetGTR())
                                {
                                    Logger.LogThis("Last packet of packet group", LogLevel.Verbose);
                                    // ==> ttr must be 0  -> end of group
                                    sendAck = true;
                                }
                                else if (sres.GetTTR())
                                {
                                    Logger.LogThis("Last packet of message", LogLevel.Verbose);
                                    // EOF :)
                                    sendAck = true;
                                    sendToUpper = true;
                                    State = StateResultRespWait;
                                } //else not last packet...
                            }
                            else
                            {
                                Logger.LogThis("LOST PACKET - last " + seglast + " current psn " + sres.PSN, LogLevel.Warn);
                                // @todo: send NACK
                            }
                            if (sendAck)
                            {
                                byte[] payload = new byte[2];
                                CWTPAck send = new CWTPAck(sendTID);
                                payload[0] = (0x03 << 3) | 0x01; // psn, one byte length
                                payload[1] = (byte)(0x7f & sres.PSN);
                                send.SetCON(true); // headers follow ;)
                                send.Payload = payload;

                                //Logger.LogThis("sending ack:" + send.ToString(), LogLevel.Verbose);

                                wtpSocket.Send(send);
                            }
                            if (sendToUpper)
                            {
                                byte[] payload = segdata.ToArray();
                                upperLayer.TrProcess(new CWTPEvent(payload, CWTPEvent.TrResultInd));
                                segdata = null;
                                segend = 0;
                            }
                        }
                        break;

                    ///////////////////// RESULT RESP WAIT //////////////////////////////////

                    case (0x02):

                        // RcvAbort in RESULT RESP WAIT
                        if (pdu.PDUType == CWTPPDU.PduTypeAbort)
                        {
                            short abortReason = ((CWTPAbort)pdu).AbortReason;
                            Close(abortReason);
                            upperLayer.TrAbort(abortReason);
                            State = StateNull;
                        }
                        // RcvResult in RESULT RESP WAIT
                        else if (pdu.PDUType == CWTPPDU.PduTypeResult)
                        {
                            //Logger.LogThis("blp", LogLevel.Verbose);
                            State = StateResultRespWait;

                            // RcvSegmResult in RESULT RESP WAIT
                        }
                        else if (pdu.PDUType == CWTPPDU.PduTypeSegmResult)
                        {
                            //Logger.LogThis("XXXXXX blpseg", LogLevel.Verbose);

                            State = StateResultRespWait;
                        }

                        break;

                    ///////////////////// WAIT TIMEOUT //////////////////////////////////////

                    case (0x03):

                        // RcvAbort in WAIT TIMEOUT
                        if (pdu.PDUType == CWTPPDU.PduTypeAbort)
                        {
                            short abortReason = ((CWTPAbort)pdu).AbortReason;
                            Close(abortReason);
                            upperLayer.TrAbort(abortReason);
                            State = StateNull;
                        }
                        // RcvResult in WAIT TIMEOUT
                        else if (pdu.PDUType == CWTPPDU.PduTypeResult)
                        {
                            if (pdu.GetRID())
                            {
                                /** @todo input TPI exitinfo if available - page 54 **/
                                CWTPAck send = new CWTPAck(sendTID);
                                wtpSocket.Send(send);
                                State = StateWaitTimeout;
                            }
                            else
                            {
                                State = StateWaitTimeout;
                            }
                        }

                        break;

                    ///////////////////// WAIT ACK WHILE SEGMENTATION SENDING ////////////////

                    case (0x04):

                        // rcvAck in STATE_WAIT_ACK
                        if (pdu.PDUType == CWTPPDU.PduTypeAck)
                        {
                            //re-initialize re-sending counter
                            segmentAckTimer.Stop();
                            segmentSended = 1;
                            segment = null;

                            // send next segment
                            pdu = (CWTPPDU)(sentPDU.Segments[segmentIndex]);
                            wtpSocket.Send(pdu);

                            segmentIndex++;

                            // last segment sent
                            if (sentPDU.Segments.Count == segmentIndex)
                            {
                                segmentIndex = 0;

                                if ((classType == 1) | (classType == 2))
                                {
                                    // start timer to re-send whole message
                                    retryTimer.Stop();
                                    retryTimer.Start();
                                    State = StateResultWait;
                                }
                                else if (classType == 0)
                                {
                                    State = StateNull;
                                    Close((short)0x00);
                                }
                            }
                            // not last segment
                            else
                            {
                                // save segment for re-sending
                                segment = pdu;

                                // start timer to re-send segment
                                segmentAckTimer.Stop();
                                segmentAckTimer.Start();
                            }
                        }

                        break;
                }
            }
        }

        /// <summary>
        /// Invoked by higher layers to process given service primitives
        /// according to state machine described in section 9.5.<br/>
        /// 	<b>Notice:</b> Only WTP Initiator is implemented!
        /// </summary>
        /// <param name="p">the Service Primitive to be processed</param>
        public virtual void Process(CWTPEvent p)
        {
            lock (this)
            {
                if (aborted)
                {
                    throw new EWTPAbortedException(abortCode);
                }


                //Logger.LogThis("" + sendTID + ": " + CWTPEvent.types[p.Type] + " in " + States[state], LogLevel.Verbose);


                switch (state)
                {

                    ///////////////////// STATE NULL /////////////////////////////////////////
                    case (0x00):

                        // TR-INVOKE.REQ
                        // some things do do when receiving a TR-Invoke
                        // were already done in the constructor - so please see there!
                        if (p.Type == CWTPEvent.TrInvokeReq)
                        {
                            if (((classType == 1) | (classType == 2)) && uack)
                            {
                                CWTPInvoke send = new CWTPInvoke(p.UserData, sendTID, classType);
                                wtpSocket.AddTransaction(this);
                                wtpSocket.Send(send);
                                sentPDU = send;
                                this.rcr = 0;
                                retryTimer.Stop();
                                retryTimer.Start();
                                State = StateResultWait;
                            }
                            else if (((classType == 1) | (classType == 2)) && !uack)
                            {
                                CWTPInvoke send = new CWTPInvoke(p.UserData, sendTID, classType);
                                wtpSocket.AddTransaction(this);
                                wtpSocket.Send(send);
                                sentPDU = send;
                                this.rcr = 0;

                                // do we have to segmentate?
                                if (!send.GetTTR())
                                {
                                    State = StateWaitAck;
                                }
                                else
                                {
                                    retryTimer.Stop();
                                    retryTimer.Start();
                                    State = StateResultWait;
                                }
                            }
                            else if (classType == 0)
                            {
                                CWTPInvoke send = new CWTPInvoke(p.UserData, sendTID, classType);
                                wtpSocket.AddTransaction(this);
                                wtpSocket.Send(send);

                                // do we have to segmentate?
                                if (!send.GetTTR())
                                {
                                    sentPDU = send;
                                    State = StateWaitAck;
                                }
                                else
                                {
                                    State = StateNull;
                                    Close((short)0x00);
                                }
                            }
                        }
                        //end TR-INVOKE.REQ

                        break;

                    //////////////////// STATE RESULT WAIT ///////////////////////////////////

                    case (0x01):
                        break;

                    ///////////////////// STATE RESULT RESP WAIT /////////////////////////////

                    case (0x02):

                        // TR-Result.res
                        if (p.Type == CWTPEvent.TrResultRes)
                        {
                            if (p.ExitInfo.Length != 0)
                            {
                                CWTPAck send = new CWTPAck(sendTID);

                                /** @todo input TPI exitinfo into "send" - page 54 top **/
                                wtpSocket.Send(send);

                                waitTimer.Stop();
                                waitTimer.Start();
                                State = StateWaitTimeout;
                            }
                            else
                            {
                                CWTPAck send = new CWTPAck(sendTID);
                                wtpSocket.Send(send);
                                waitTimer.Stop();
                                waitTimer.Start();
                                State = StateWaitTimeout;
                            }
                        }
                        // end TR-Result.res

                        break;

                    ///////////////////// STATE WAIT TIMEOUT /////////////////////////////////

                    case (0x03):
                        break;
                }
            }
        }

        public virtual void Process(EWTPCorruptPDUException e)
        {
            if (state != StateNull)
            {
                CWTPAbort send = new CWTPAbort(e.TID);
                send.AbortReason = CWTPAbort.AbortReasonProtoErr;
                wtpSocket.Send(send);
                Close(CWTPAbort.AbortReasonProtoErr);
                upperLayer.TrAbort(CWTPAbort.AbortReasonProtoErr);
                State = StateNull;
            }
        }

        /// <summary> use this method to invoke a TR-ABORT.REQ by the upper Layer</summary>
        public virtual void Abort()
        {
            Abort(CWTPAbort.AbortReasonUnknown);
        }

        /// <summary> use this method to invoke a TR-ABORT.REQ by the upper Layer</summary>
        public virtual void Abort(short abortReason)
        {
            // TR-ABORT.REQ
            if ((state == 0x01) || (state == 0x02) || (state == 0x03))
            {

                Logger.LogThis("" + sendTID + ": TR-ABORT.REQ Reason: " + abortReason, LogLevel.Verbose);


                CWTPAbort send = new CWTPAbort(sendTID);
                send.AbortReason = abortReason;
                wtpSocket.Send(send);
                Close(abortReason);
            }
        }

        public virtual void Close(short reasonCode)
        {
            abortCode = reasonCode;
            aborted = true;
            retryTimer.Stop();
            waitTimer.Stop();
            acknowledgeTimer.Stop();
            State = StateNull;
            wtpSocket.RemoveTransaction(this);
        }


        /// <summary>
        /// Static method, that generates a new (unique) Transaction ID
        /// by incrementing genTID
        /// </summary>
        /// <returns>A new unique Transaction ID</returns>
        private static int GenerateNewTID()
        {
            lock (typeof(CWTPInitiator))
            {
                if (genTID == -1)
                {
                    Random r = new Random();
                    genTID = Math.Abs(r.Next() % 255);
                }

                int result = genTID;

                if (genTID == 255)
                {
                    genTID = 0;
                }
                else
                {
                    ++genTID;
                }

                return result;
            }
        }

    }
}