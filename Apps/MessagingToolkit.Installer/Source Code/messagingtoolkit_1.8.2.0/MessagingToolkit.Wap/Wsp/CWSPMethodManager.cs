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
using System.IO;

using MessagingToolkit.Wap.Wtp;
using MessagingToolkit.Wap.Log;
using MessagingToolkit.Wap.Helper;
using MessagingToolkit.Wap.Wsp.Pdu;

namespace MessagingToolkit.Wap.Wsp
{

    /// <summary>
    /// WSP method manager
    /// </summary>
    public class CWSPMethodManager : IWTPUpperLayer
    {
        private static readonly string[] states = new string[] { "STATE_NULL", "STATE_REQUESTING", "STATE_WAITING", "STATE_WAITING2", "STATE_COMPLETING" };
       
        // Method states
        private const short StateNull = 0;
        private const short StateRequesting = 1;
        private const short StateWaiting = 2;
        private const short StateWaiting2 = 3;
        private const short StateCompleting = 4;
        private byte version = 0;
        private short state = StateNull;
        private CWSPSession session;
        private IWTPTransaction wtp;
        private IWSPUpperLayer upperlayer;

        /// <summary>  
        /// In case we have segmented data we will buffer the data in this Stream
        /// </summary>
        private MemoryStream waitingResultContent = new MemoryStream();

        /// <summary>  
        /// In case we have segmented data we will prepare this result - waiting for the whole data
        /// </summary>
        private CWSPResult waitingResult;

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        private void Initialize()
        {
            waitingResult = new CWSPResult(this, null, null, null);
        }

        /// <summary>
        /// Sets the state.
        /// </summary>
        /// <value>The state.</value>
        private short State
        {           
            set
            {
                //Logger.LogThis(states[this.state] + " >>> " + states[value], LogLevel.Verbose);
                this.state = value;
            }
        }


        /// <summary>
        /// s-methodInvoke.req implemented in CWSPSession calls this constructor
        /// </summary>
        /// <param name="initpdu">The initpdu.</param>
        /// <param name="session">The session.</param>
        /// <param name="upperLayer">The upper layer.</param>
        public CWSPMethodManager(CWSPPDU initpdu, CWSPSession session, IWSPUpperLayer upperLayer)
        {
            Initialize();
            if ((upperLayer != null) && upperLayer is IWSPUpperLayer2)
            {
                this.version = 2;
            }
            else
            {
                this.version = 1;
            }

            this.session = session;
            this.upperlayer = upperLayer;
            State = StateRequesting;

            lock (this)
            {
                CWTPEvent initPacket = new CWTPEvent(initpdu.ToByteArray(), CWTPEvent.TrInvokeReq);
                wtp = session.WTPSocket.TrInvoke(this, initPacket, false, IWTPTransactionConstants.ClassType2);
            }
        }

        //////////////////////////////////////////////// from upper layer: S-*.req/res

        /// <summary>
        /// s-methodInvokeData.req
        /// </summary>
        /// <param name="payload">The payload.</param>
        /// <param name="moreData">if set to <c>true</c> [more data].</param>
        public virtual void SMethodInvokeData(byte[] payload, bool moreData)
        {
            lock (this)
            {
                if (state == StateRequesting)
                {
                    CWTPEvent initPacket = new CWTPEvent(payload, CWTPEvent.TrInvokeDataRes);
                    initPacket.MoreData = moreData;

                    /** @todo if request headers provided and MoreData == false*/
                }
            }
        }

        /// <summary>
        /// s-methodresultdata.res
        /// </summary>
       public virtual void SMthodResultData()
        {
            lock (this)
            {
                if (state == StateCompleting)
                {
                    State = StateNull;

                    CWTPEvent initPacket = new CWTPEvent(new byte[0], CWTPEvent.TrResultDataRes);

                    /** @todo initPacket.setExitInfo(ackHeaders); **/
                    try
                    {
                        wtp.Process(initPacket);
                    }
                    catch (EWTPAbortedException e)
                    {
                        Logger.LogThis("Event processing aborted: " + e.Message, LogLevel.Warn);
                        Abort(CWSPSession.AbortProtoErr);
                    }
                }
            }
        }

       /// <summary>
       /// S-MethodAbort.req
       /// </summary>
       /// <returns>S-MethodAbort.ind</returns>
        public virtual bool SMethodAbort()
        {
            lock (this)
            {
                if ((session.GetState() == CWSPSession.StateConnecting) || (session.GetState() == CWSPSession.StateConnectetd) || (session.GetState() == CWSPSession.StateResuming))
                {
                    if ((state == StateRequesting) || (state == StateWaiting) || (state == StateWaiting2) || (state == StateCompleting))
                    {
                        State = StateNull;
                        wtp.Abort(CWSPSession.AbortPeerReq);
                        SMethodAbortInd(CWSPSession.AbortPeerReq);

                        return true;
                    }
                }

                return false;
            }
        }

        /// <summary>
        /// s-methodresult.res
        /// </summary>
        /// <param name="exitInfoAckHeaders">The exit info ack headers.</param>
        public virtual void MethodResult(CWSPHeaders exitInfoAckHeaders)
        {
            lock (this)
            {
                if (session.GetState() == CWSPSession.StateConnectetd)
                {
                    if (state == StateCompleting)
                    {
                        State = StateNull;

                        CWTPEvent initPacket = new CWTPEvent(new byte[0], CWTPEvent.TrResultRes);

                        /** @todo initPacket.setExitInfo(ackHeaders); **/
                        try
                        {
                            wtp.Process(initPacket);
                        }
                        catch (EWTPAbortedException e)
                        {
                            Logger.LogThis("Event processing aborted: " + e.Message, LogLevel.Warn);
                            Abort(CWSPSession.AbortProtoErr);
                        }
                    }
                }
                // Remove the method from the session queue
                session.RemoveMethod(this);
            }
        }

        ////////////////////////////////////////////////// to upper Layer: S-*.ind/cnf
        public virtual void SMethodAbortInd(short reason)
        {
            lock (this)
            {
                //Logger.LogThis("s_methodAbort_ind - Reason: " + reason, LogLevel.Verbose);
            }
        }

       
        public virtual void SMethodInvokeDataCnf()
        {
            lock (this)
            {
                //Logger.LogThis("s_methodInvokeData_cnf", LogLevel.Verbose);
            }
        }

        public virtual void SMethodInvokeDataInd(CWSPHeaders headers, byte[] payload, string contentType, bool moreData)
        {
            lock (this)
            {
                //Logger.LogThis("s_methodInvokeData_cnf", LogLevel.Verbose);
            }
        }

        public virtual void SMethodInvokeCnf()
        {
            lock (this)
            {
                //Logger.LogThis("s_methodInvoke_cnf", LogLevel.Verbose);
            }
        }

        public virtual void SMethodResultInd(CWSPReply pdu, bool moreData)
        {
            lock (this)
            {
                CWSPHeaders headers = pdu.GetHeaders();
                byte[] payload = pdu.Payload;
                string contentType = pdu.GetContentType();

                
                //Logger.LogThis("s_methodResult_ind", LogLevel.Verbose);
                Logger.LogThis("  Data Length: " + payload.Length, LogLevel.Verbose);
                Logger.LogThis("  contentType: " + contentType, LogLevel.Verbose);
                Logger.LogThis("  moreData   : " + moreData, LogLevel.Verbose);
                Logger.LogThis("  version    : " + version, LogLevel.Verbose);

                if (payload.Length > 0)
                {
                   Logger.LogThis(" --- PAYLOAD ---\n" + DebugUtils.HexDump(payload), LogLevel.Verbose);
                }
                

                // Old interface...
                upperlayer.MethodResultInd(payload, contentType, moreData);

                if (version == 2)
                {
                    try
                    {

                        waitingResultContent.Write(payload, 0, payload.Length);

                        if (!moreData)
                        {
                            waitingResultContent.Flush();
                            CWSPResult result = new CWSPResult(this, headers, contentType, waitingResultContent.ToArray());
                            // Convert the WSP status into a HTTP status code
                            int status = pdu.GetStatus();
                            if (status == 0x50)
                            {
                                status = 416;
                            }
                            else if (status == 0x51)
                            {
                                status = 417;
                            }
                            else
                            {
                                if (status >= 0x60)
                                {
                                    status -= 0x10;
                                }
                                status = (status >> 4) * 100 + (status & 0x0f);
                            }
                            result.SetStatus(status);
                            ((IWSPUpperLayer2)upperlayer).MethodResultInd(result);
                            waitingResultContent.Close();
                            waitingResultContent = null;
                            waitingResultContent = new MemoryStream();
                        }
                        else
                        {
                            waitingResult = new CWSPResult(this, headers, contentType, null);
                        }
                    }
                    catch (System.IO.IOException e)
                    {
                        Logger.LogThis(e.ToString(), LogLevel.Error);
                    }
                }
            }
        }

        public virtual void SMethodResultDataInd(byte[] responseBody, bool moreData)
        {
            lock (this)
            {
                if (version == 2)
                {
                    try
                    {                       
                        waitingResultContent.Write(responseBody, 0, responseBody.Length);

                        if (!moreData)
                        {
                            waitingResultContent.Flush();
                            waitingResult.Payload = waitingResultContent.ToArray();
                            ((IWSPUpperLayer2)upperlayer).MethodResultInd(waitingResult);
                            waitingResult = new CWSPResult(this, null, null, null);
                            waitingResultContent.Close();
                            waitingResultContent = null;
                            waitingResultContent = new MemoryStream();
                        }
                    }
                    catch (IOException e)
                    {
                        Logger.LogThis("IOException in s_methodResultData_ind: " +  e.Message, LogLevel.Warn);
                    }
                }
            }
        }

        /////////////////////////////////////////// from the layer below: TR-*.ind/cnf
        public virtual void TrProcess(CWTPEvent p)
        {
            //Logger.LogThis(CWTPEvent.types[p.Type] + " in " + states[state], LogLevel.Verbose);

            switch (p.Type)
            {

                case (byte)(0x03):  //--------------------------------------- TR_INVOKE_CNF

                    if ((session.GetState() == CWSPSession.StateConnecting) || (session.GetState() == CWSPSession.StateResuming))
                    {
                        Abort(CWSPSession.AbortProtoErr);
                    }
                    else if (session.GetState() == CWSPSession.StateConnectetd)
                    {
                        if (state == StateRequesting)
                        {
                            if (!p.MoreData)
                            {
                                // entire method completed
                                State = StateWaiting;
                                SMethodInvokeCnf();
                            }
                            else
                            {
                                State = StateRequesting;
                                SMethodInvokeCnf();
                            }
                        }
                    }

                    break;


                case (byte)(0x05):  //--------------------------------------- TR_RESULT_IND

                    if ((session.GetState() == CWSPSession.StateConnecting) || (session.GetState() == CWSPSession.StateResuming))
                    {
                        Abort(CWSPSession.AbortProtoErr);
                    }
                    else if (session.GetState() == CWSPSession.StateConnectetd)
                    {
                        if (state == StateWaiting)
                        {
                            try
                            {
                                CWSPPDU pdu = CWSPPDU.GetPDU(p.UserData);

                                /*
                                if (p.getUserData().length > session.getMRU()){
                                logger.warn("over!");
                                setState(STATE_NULL);
                                wtp.abort(session.ABORT_MRUEXCEEDED);
                                s_methodAbort_ind(session.ABORT_MRUEXCEEDED);
                                }
                                else
                                */
                                if (pdu.Type == CWSPPDU.PduTypeReply)
                                {
                                    CWSPReply pdu2 = (CWSPReply)pdu;

                                    if (p.MoreData)
                                    {
                                        State = StateWaiting2;

                                        CWTPEvent initPacket = new CWTPEvent(new byte[0], CWTPEvent.TrResultRes);

                                        try
                                        {
                                            wtp.Process(initPacket);
                                        }
                                        catch (EWTPAbortedException e)
                                        {
                                            Logger.LogThis("Event processing aborted: " + e.Message, LogLevel.Warn);
                                            Abort(CWSPSession.AbortProtoErr);
                                        }

                                        SMethodResultInd(pdu2, p.MoreData);
                                    }
                                    else
                                    {
                                        State = StateCompleting;
                                        SMethodResultInd(pdu2, p.MoreData);
                                    }
                                }
                                else
                                {
                                    State = StateNull;
                                    wtp.Abort(CWSPSession.AbortProtoErr);
                                    SMethodAbortInd(CWSPSession.AbortProtoErr);
                                }
                            }
                            catch (EWSPCorruptPDUException e)
                            {
                                Logger.LogThis("Corrupt PDU: " + e.Message, LogLevel.Warn);
                            }
                        }
                    }

                    break;


                case (byte)(0x0B):  //--------------------------------------- TR_INVOKEDATA_CNF

                    if (state == StateRequesting)
                    {
                        if (!p.MoreData)
                        {
                            // entire method completed
                            State = StateWaiting;
                            SMethodInvokeDataCnf();
                        }
                        else
                        {
                            State = StateRequesting;
                            SMethodInvokeDataCnf();
                        }
                    }

                    break;


                case (byte)(0x0D):  //--------------------------------------- TR_RESULTDATA_IND

                    if (state == StateWaiting2)
                    {
                        // if SDU size > MRU:
                        if (p.UserData.Length > session.GetMRU())
                        {
                            State = StateNull;
                            wtp.Abort(CWSPSession.AbortMRUExceeded);
                            SMethodAbortInd(CWSPSession.AbortMRUExceeded);
                        }
                        else
                        {
                            // if data fragment PDU
                            try
                            {
                                CWSPPDU pdu = CWSPPDU.GetPDU(p.UserData);

                                if ((pdu.Type == CWSPPDU.PduTypeDataFragment) && (p.FrameBoundary == true) && (p.MoreData == false))
                                {
                                    State = StateCompleting;

                                    CWSPDataFragment pdu3 = (CWSPDataFragment)pdu;
                                    SMethodInvokeDataInd(pdu3.GetHeaders(), pdu3.Payload, pdu3.GetContentType(), p.MoreData);
                                }
                            }
                            // if response body
                            catch (EWSPCorruptPDUException e)
                            {
                                if (p.MoreData)
                                {
                                    State = StateWaiting2;

                                    CWTPEvent initPacket = new CWTPEvent(new byte[0], CWTPEvent.TrResultDataRes);

                                    try
                                    {
                                        wtp.Process(initPacket);
                                    }
                                    catch (EWTPAbortedException e2)
                                    {
                                        Logger.LogThis("Event processing aborted: " + e2.Message, LogLevel.Warn);
                                        Abort(CWSPSession.AbortProtoErr);
                                    }

                                    SMethodResultDataInd(p.UserData, p.MoreData);
                                }
                                else if (p.FrameBoundary == false)
                                {
                                    State = StateCompleting;
                                    SMethodResultDataInd(p.UserData, p.MoreData);
                                }
                            }
                        }
                    }

                    break;
            } // end of switch
        }

        /// <summary>
        /// tr-abort.ind
        /// used by *WTP* layer to abort
        /// </summary>
        /// <param name="abortReason">The abort reason.</param>
        public virtual void TrAbort(short abortReason)
        {
            if ((session.GetState() == CWSPSession.StateConnecting) || (session.GetState() == CWSPSession.StateConnectetd) || (session.GetState() == CWSPSession.StateResuming))
            {
                if (state != StateNull)
                {
                    State = StateNull;

                    if (abortReason == CWSPSession.AbortDisconnect)
                    {
                        session.Disconnect();
                    }
                    else if (abortReason == CWSPSession.AbortSuspend)
                    {
                        session.Suspend();
                    }
                    else
                    {
                        SMethodAbortInd(abortReason);
                    }
                }
            }
        }

        //////////////////////////////////////////////////////////////// pseudo events

        /// <summary>
        /// used by CWSPSession - *WSP* layer
        /// </summary>
        /// <param name="reason">The reason.</param>
        public virtual void Abort(short reason)
        {
            if (state != StateNull)
            {
                State = StateNull;
                wtp.Abort(reason);
                SMethodAbortInd(reason);
            }
        }
        
    }
}
