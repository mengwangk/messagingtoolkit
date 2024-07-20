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
using System.Collections;
using System.Threading;
using System.Net.Sockets;

using MessagingToolkit.Wap.Wtp.Pdu;
using MessagingToolkit.Wap.Log;
using MessagingToolkit.Wap.Helper;
using MessagingToolkit.Wap.Wsp;
using MessagingToolkit.Wap;

namespace MessagingToolkit.Wap.Wtp
{
    /// <summary>
    /// This class interfaces to the lower layer, the UDP Layer.
    /// It uses a datagram socket here.
    /// Per WSP session (defined by local port + address and remote port + address)
    /// we need one object of type CWTPSocket. We may need more than one transaction
    /// (CWTPTransaction) per session. CWTPSocket listens in a endless thread
    /// for new datagrams on the DatagramSocket. If it gets one, it uses
    /// CWTPFactory to decode the PDU. Then it associates the PDU with a transaction
    /// by comparing the Transaction Identifier.
    /// The constructor is private. Use #getInstance(CWTPTransaction t) to
    /// get an object for a specific transaction. This is because we want to check,
    /// if there already exists a CWTPManagement for the session the transaction
    /// belongs to.
    /// </summary>
    public class CWTPSocket : ThreadManager
    {
        /// <summary>
        /// WAP Default client port (49200)  
        /// </summary>
        public const int DefaultPort = 49200;

        /// <summary> the underliing Layer, a DatagramSocket (UDP)</summary>
        private UdpSocket socket;

        /// <summary> the upper Layer, modelling the session, WSP</summary>
        private IWTPUpperLayer upperLayer;

        // remote port and address
        private int toPort;
        private IPAddress toAddress;

        // Used to synchronize reader-thread with close() method
        private object threadLock = new object();
        private bool isRunning;

        /// <summary> Holds all transactions belonging to this management entity and
        /// their corresponding Transaction IDs wrapped in an Integer
        /// </summary>
        private Hashtable transactions = Hashtable.Synchronized(new Hashtable());


        /// <summary>
        /// Gets the local address.
        /// </summary>
        /// <value>The local address.</value>
        virtual public IPAddress LocalAddress
        {
            get
            {
                //return Dns.GetHostEntry(Dns.GetHostName()).AddressList[0];
                return Dns.GetHostAddresses(Dns.GetHostName())[0];
            }

        }
        /// <summary>
        /// Gets the local port.
        /// </summary>
        /// <value>The local port.</value>
        virtual public int LocalPort
        {
            get;
            set;

        }
        /// <summary>
        /// Gets the remote address.
        /// </summary>
        /// <value>The remote address.</value>
        virtual public IPAddress RemoteAddress
        {
            get
            {
                return toAddress;
            }
        }

        /// <summary>
        /// Gets the remote port.
        /// </summary>
        /// <value>The remote port.</value>
        virtual public int RemotePort
        {
            get
            {
                return toPort;
            }
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="CWTPSocket"/> class.
        /// </summary>
        /// <param name="toAddress">To address.</param>
        /// <param name="toPort">To port.</param>
        /// <param name="upperLayer">The upper layer.</param>
        public CWTPSocket(IPAddress toAddress, int toPort, IWTPUpperLayer upperLayer)
            : this(toAddress, toPort, null, DefaultPort, upperLayer)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CWTPSocket"/> class.
        /// </summary>
        /// <param name="toAddress">To address.</param>
        /// <param name="toPort">To port.</param>
        /// <param name="localAddress">The local address.</param>
        /// <param name="localPort">The local port.</param>
        /// <param name="upperLayer">The upper layer.</param>
        public CWTPSocket(IPAddress toAddress, int toPort, System.Net.IPAddress localAddress, int localPort, IWTPUpperLayer upperLayer)
        {
            this.LocalPort = localPort;
            if (localAddress == null)
            {
                socket = new UdpSocket(localPort);
            }
            else
            {
                //socket = new UdpSocket(new IPEndPoint(Dns.GetHostEntry(Dns.GetHostName()).AddressList[0], localPort));
                socket = new UdpSocket(new IPEndPoint(Dns.GetHostAddresses(Dns.GetHostName())[0], localPort));
            }

            //socket.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            //socket.Client.ExclusiveAddressUse = false;
                       
            //socket.Ttl = 1000;            
            this.upperLayer = upperLayer;
            this.toAddress = toAddress;
            this.toPort = toPort;
            this.Name = "CWTPSocket-" + toAddress.ToString() + ":" + toPort;
            isRunning = true;
            this.Start();
        }

        /// <summary>
        /// Invoke
        /// </summary>
        /// <param name="upperLayer">The upper layer.</param>
        /// <param name="initPacket">The init packet.</param>
        /// <param name="ackType">if set to <c>true</c> [ack type].</param>
        /// <param name="classType">Type of the class.</param>
        /// <returns></returns>
        public virtual CWTPInitiator TrInvoke(IWTPUpperLayer upperLayer, CWTPEvent initPacket, bool ackType, byte classType)
        {
            return new CWTPInitiator(this, upperLayer, initPacket, ackType, classType);
        }

        /// <summary>
        /// Sends the specified pdu.
        /// </summary>
        /// <param name="pdu">The pdu.</param>
        public virtual bool Send(CWTPPDU pdu)
        {
            byte[] sendBytes = pdu.ToByteArray();

            try
            {
                if (socket == null)
                {
                    return false;
                }
                //Logger.LogThis("Sending " + CWTPPDU.Types[pdu.PDUType] + ", TID: " + pdu.GetTID(), LogLevel.Verbose);
                Logger.LogThis("\n" + DebugUtils.HexDump("Sent data: ", sendBytes), LogLevel.Verbose);
                UdpSocket.Send(socket, new Packet(sendBytes, sendBytes.Length, new IPEndPoint(toAddress, toPort)));
            }
            catch (Exception e)
            {
                Logger.LogThis("Exception while sending: " + e.Message, LogLevel.Error);
            }
            return true;
        }

        /// <summary> This method in an (endless) loop receives DatagramPackets (UDP)
        /// by the DatagramSocket layer below.
        /// <br>
        /// Stopping: The loop can be interrupted by calling interrupt().
        /// If you would like to stop the whole socket you better
        /// should call close(). It also closes the DatagramSocket below.
        /// </summary>
        override public void Run()
        {
            while (isRunning)
            {
                //Logger.LogThis("WTP-Layer listening...", LogLevel.Verbose);

                /** @todo MRU capability negotiation
                * The folowing number (1400) should not be hard coded!
                * It is the MRU that is negotiated via WSP.
                * This is part of the WSP CAPABILITY NEGOTIATION Task!
                */
                Packet input = new Packet(new byte[1400], 1400);

                try
                {
                    while (isRunning)
                    {
                        try
                        {
                            UdpSocket.Receive(socket, out input);

                            break;
                        }
                        catch (Exception ie)
                        {
                            // timeout from read...
                            //Logger.LogThis(ie.Message, LogLevel.Error);
                        }
                    }
                }
                catch (IOException e)
                {
                    if (isRunning)
                    {
                        Logger.LogThis("IOException from socket.receive(): " + e.Message, LogLevel.Error);
                    }
                }

                // Have we been stopped? 
                if (!isRunning)
                {
                    break;
                }

                // is the remote host allowed to communicate with this session?
                // if not, ignore it!
                //if (toAddress.equals(in.getAddress()) && toPort == in.getPort()){
                // decode the bytes
                CWTPPDU pdu = null;
                IWTPTransaction transact = null;

                try
                {
                    // uses now the actual data length from the DatagramPacket
                    // instead of the length of the  byte[] buffer
                    pdu = CWTPPDU.Decode(input.Data, input.Length);

                    //Logger.LogThis("Received WTP PDU: " + CWTPPDU.Types[pdu.PDUType] + " TID: " + pdu.GetTID() + " | " + (pdu.GetTID() + 32768), LogLevel.Verbose);
                    
                }
                catch (EWTPCorruptPDUException e)
                {
                    // if the TID is available process in state machine
                    // else: ignore
                    if (e.TidAvailable)
                    {
                        transact = GetTransaction(e.TID + 32768);

                        if (transact != null)
                        {
                            // process in state machine
                            transact.Process(e);
                        }
                    }
                }

                //if (pdu == null) break;

                // associate the PDU with the corresponding transaction
                transact = GetTransaction(pdu.GetTID() + 32768);

                if (transact == null)
                {
                    // there is no transaction with this TID
                    //logger.debug("new Transaction");
                    // if rcvInvoke start new transaction:
                    if (pdu.PDUType == CWTPPDU.PduTypeInvoke)
                    {
                        CWTPInvoke pdu2 = (CWTPInvoke)pdu;

                        if ((pdu2.GetTCL() == IWTPTransactionConstants.ClassType1) || (pdu2.GetTCL() == IWTPTransactionConstants.ClassType2))
                        {
                            CWTPResponder resp = new CWTPResponder(this, upperLayer, pdu2, pdu2.GetUpFlag(), pdu2.GetTCL());
                        }
                    }

                    // if Ack PDU with TIDve flag set, send abort PDU
                    if (pdu.PDUType == CWTPPDU.PduTypeAck)
                    {
                        if (((CWTPAck)pdu).TveTok)
                        {
                            Send(new CWTPAbort(CWTPAbort.AbortReasonInvalidTId));
                        }
                    }
                    else if (pdu.pduType == CWTPPDU.PduTypeNegAck)
                    {
                        if (((CWTPAck)pdu).TveTok)
                        {
                            Send(new CWTPAbort(CWTPAbort.AbortReasonInvalidTId));
                        }
                    }

                    // else: ignore
                }
                else
                {
                    // pdu is associated with transaction
                    try
                    {
                        //logger.debug("PDU associated");
                        // process in state machine of transaction
                        transact.Process(pdu);
                    }
                    catch (EWTPAbortedException e3)
                    {
                        Logger.LogThis("Transaction aborted: " + e3.Message, LogLevel.Warn);
                        transact = GetTransaction(pdu.GetTID());

                        if (transact != null)
                        {
                            RemoveTransaction(transact);
                        }
                    }
                }
            }
            // Notify close()...
            lock (threadLock)
            {
                Monitor.PulseAll(threadLock);
            }
        }

        /// <summary> Close the socket. Closes the underliing DatagramSocket (UDP)</summary>
        public virtual void Close()
        {
            Logger.LogThis("Close(): Closing socket", LogLevel.Verbose);
            isRunning = false;            

            //socket.Client.Shutdown(SocketShutdown.Both);
            //socket.Client.Close();           
            
            socket.Close();
            socket = null;
            if (ThreadManager.Current() != this)
            {
                lock (threadLock)
                {
                    try
                    {
                        Logger.LogThis("Close(): waiting for thread to finish", LogLevel.Verbose);
                        Monitor.Wait(threadLock, TimeSpan.FromMilliseconds(5000)); // Wait max 5 secs for reader thread to terminate...
                        Logger.LogThis("Close(): done", LogLevel.Verbose);
                    }
                    catch (ThreadInterruptedException e)
                    {
                        Logger.LogThis(string.Format("Unable to close socket: {0}", e.Message), LogLevel.Error);
                    }
                }
            }
        }


        /// <summary>
        /// Adds the transaction.
        /// </summary>
        /// <param name="transaction">The transaction.</param>
        /// <returns></returns>
        public virtual bool AddTransaction(IWTPTransaction transaction)
        {
            Int32 tid = (Int32)transaction.TID;

            lock (transactions.SyncRoot)
            {
                if (transactions.ContainsKey(tid))
                {
                    return false;
                }

                transactions[tid] = transaction;
            }

            return true;
        }

        /// <summary>
        /// Gets the transaction.
        /// </summary>
        /// <param name="TID">The TID.</param>
        /// <returns></returns>
        public virtual IWTPTransaction GetTransaction(int TID)
        {
            Int32 tid = (Int32)TID;

            return (IWTPTransaction)transactions[tid];
        }

        /// <summary>
        /// Removes the transaction.
        /// </summary>
        /// <param name="t">The t.</param>
        /// <returns></returns>
        public virtual bool RemoveTransaction(IWTPTransaction transaction)
        {
            Int32 tid = (System.Int32)transaction.TID;

            object tempObject;
            tempObject = transactions[tid];
            transactions.Remove(tid);
            return tempObject != null;
        }
       
    }
}
