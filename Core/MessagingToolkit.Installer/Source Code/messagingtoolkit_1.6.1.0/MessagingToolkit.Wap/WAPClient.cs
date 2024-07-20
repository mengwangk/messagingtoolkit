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
using System.Net;
using System.Collections;
using System.Threading;
using System.IO;

using MessagingToolkit.Wap.Log;
using MessagingToolkit.Wap.Wtp;
using MessagingToolkit.Wap.Wsp;
using MessagingToolkit.Wap.Wsp.Pdu;

namespace MessagingToolkit.Wap
{
    /// <summary>
    /// This class represents a WSP "User-Agent" which can be
    /// used for executing WSP <code>GET</code>
    /// and <code>POST</code> methods.
    /// <code>
    /// WAPClient client = new WAPClient("localhost", 9201);
    /// Request request = new GetRequest("http://localhost/");
    /// client.connect();
    /// Response response = client.execute(request);
    /// client.disconnect();
    /// </code>
    /// </summary>
    public class WAPClient
    {
        /// <summary>
        /// Default connect/disconnect timeout in milliseconds: 30000 
        /// </summary>
        public const long DefaultConnectTimeOut = 30000;

        /// <summary>
        /// Default execute timeout in milliseconds: 60000 
        /// </summary>
        public const long DefaultExecTimeout = 60000;


        /// <summary>
        /// Default content type
        /// </summary>
        private const string DefaultContentType = "application/unknown";


        /// <summary>
        /// Log file extension
        /// </summary>
        private const string LogFileExtension = ".log";

        /// <summary>
        /// Connected 
        /// </summary>
        private const string ConnectedResult = "CONNECTED";

        private IPAddress gatewayAddress;
        private IPAddress localAddr;
        private int gatewayPort;
        private int localPort;
        private CWSPSession session;
        private long disconnectTimeout;

        private object sessionLock = new object();

        private IWSPUpperLayer2 upperLayerImpl;

        private Hashtable pendingRequests;

        
        /// <summary>
        /// Initializes a new instance of the <see cref="WAPClient"/> class.
        /// </summary>
        private WAPClient()
        {
        }

        /// <summary>
        /// Check if the client is currently connected to the WAP gateway
        /// </summary>
        /// <value><c>true</c> if connected; otherwise, <c>false</c>.</value>
        /// <returns> true if the client is connected, false otherwise
        /// </returns>
        virtual public bool Connected
        {
            get
            {
                lock (this)
                {
                    return session != null;
                }
            }
        }

        /// <summary>
        /// Gets or sets the log level.
        /// </summary>
        /// <value>The log level.</value>
        virtual public LogLevel LogLevel
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
        /// Gets or sets the user agent.
        /// </summary>
        /// <value>
        /// The user agent.
        /// </value>
        virtual public string UserAgent
        {
            get;
            set;
        }



        /// <summary>
        /// Gets or sets the XWAP profile.
        /// </summary>
        /// <value>
        /// The XWAP profile.
        /// </value>
        virtual public string XWAPProfile
        {
            get;
            set;
        }


        /// <summary>
        /// Gets or sets the log name format.
        /// </summary>
        /// <value>The log name format.</value>
        virtual public LogNameFormat LogNameFormat
        {
            get
            {
                return Logger.LogNameFormat;
            }
            set
            {
                Logger.LogNameFormat = value;
            }
        }

        /// <summary>
        /// Gets or sets the log size max.
        /// </summary>
        /// <value>The log size max.</value>
        virtual public int LogSizeMax
        {
            get
            {
                return Logger.LogSizeMax;
            }
            set
            {
                Logger.LogSizeMax = value;
            }
        }

        /// <summary>
        /// Gets or sets the log quota format.
        /// </summary>
        /// <value>The log quota format.</value>
        virtual public LogQuotaFormat LogQuotaFormat
        {
            get
            {
                return Logger.LogQuotaFormat;
            }
            set
            {
                Logger.LogQuotaFormat = value;
            }
        }

        /// <summary>
        /// Gets or sets the execution time out.
        /// </summary>
        /// <value>
        /// The execution time out.
        /// </value>
        virtual public long ExecutionTimeOut
        {
            get;
            set;
        }

        /// <summary>
        /// Construct a new WAP Client
        /// </summary>
        /// <param name="wapGateway">hostname of the WAP gateway to use</param>
        /// <param name="port">port-number</param>
        /// <throws>UnknownHostException if the hostname cannot be resolved </throws>
        public WAPClient(string wapGateway, int port)
            : this(Dns.GetHostAddresses(wapGateway)[0], port)
        {
           
        }


        /// <summary>
        /// Construct a new WAP Client
        /// </summary>
        /// <param name="wapGateway">the address of the WAP gateway to use</param>
        /// <param name="port">the WAP gateway port number</param>
        public WAPClient(IPAddress wapGateway, int port)
            : this(wapGateway, port, null, CWTPSocket.DefaultPort)
        {
        }

        /// <summary>
        /// Construct a new WAP Client
        /// </summary>
        /// <param name="wapGateway">the addresss of the WAP gateway to use</param>
        /// <param name="wapPort">the WAP gateway port number</param>
        /// <param name="localAddress">the local address to bind to</param>
        /// <param name="localPort">the local port to bind to (0 to let the OS pick a free port)</param>
        public WAPClient(IPAddress wapGateway, int wapPort, IPAddress localAddress, int localPort)
        {
            gatewayAddress = wapGateway;
            gatewayPort = wapPort;
            this.localAddr = localAddress;
            this.localPort = localPort;
            upperLayerImpl = new UpperLayerImpl(this);
            pendingRequests = Hashtable.Synchronized(new Hashtable());

            string assemblyName = typeof(WAPClient).Assembly.GetName().Name;
            string logFileName = assemblyName; 

            // Initialize the logger
            Logger.UseSensibleDefaults(logFileName, string.Empty, LogLevel.Error);
            Logger.LogPrefix = LogPrefix.Dt;

            ExecutionTimeOut = DefaultExecTimeout;
        }

        /// <summary>
        /// Execute a request. The client must be connected to the gateway.
        /// </summary>
        /// <param name="request">the request to execute</param>
        /// <returns>the response</returns>
        /// <throws>  SocketException if a timeout occurred </throws>
        /// <throws>  IllegalStateException if the client is not connected </throws>
        public virtual Response Execute(Request request)
        {
            return Execute(request, ExecutionTimeOut);
        }

        /// <summary>
        /// Execute a request. The client must be connected to the gateway.
        /// </summary>
        /// <param name="request">the request to execute</param>
        /// <param name="timeout">timeout in milliseconds</param>
        /// <returns>the response</returns>
        /// <throws>SocketException if a timeout occurred </throws>
        /// <throws>IllegalStateException if the client is not connected </throws>
        public virtual Response Execute(Request request, long timeout)
        {
            CWSPMethodManager mgr = null;

            lock (this)
            {
                if (session == null)
                {
                    throw new Exception("Not yet connected");
                }

                CWSPHeaders headers = request.WSPHeaders;
                if (headers.GetHeader("accept") == null)
                {
                    headers.SetHeader("accept", "*/*");
                }

                if (!string.IsNullOrEmpty(this.UserAgent))
                {
                    Logger.LogThis("Setting user-agent to  " + this.UserAgent, LogLevel.Verbose);
                    headers.SetHeader("user-agent", this.UserAgent);
                }
                else
                {
                    string uh = headers.GetHeader("user-agent");
                    if (uh == null)
                    {
                        headers.SetHeader("user-agent", "mstoolkit/1.1");
                    }
                    else if ("".Equals(uh))
                    {
                        headers.SetHeader("user-agent", null);
                    }
                }

                if (!string.IsNullOrEmpty(this.XWAPProfile))
                {
                    Logger.LogThis("Setting profile to  " + this.XWAPProfile, LogLevel.Verbose);
                    headers.SetHeader("PROFILE", this.XWAPProfile);
                }


                if (request is GetRequest)
                {
                    Logger.LogThis("Executing GET Request for URL " + request.GetURL(), LogLevel.Verbose);
                    mgr = session.SGet(headers, request.GetURL());
                }
                else if (request is PostRequest)
                {
                    Logger.LogThis("Executing POST Request for URL " + request.GetURL(), LogLevel.Verbose);

                    PostRequest post = (PostRequest)request;
                    mgr = session.SPost(post.WSPHeaders, post.RequestBody, post.ContentType, post.GetURL());
                }
            }

            // Wait until the method shows up in our hashtable
            Logger.LogThis("Waiting " + timeout + "ms for execute completion...", LogLevel.Verbose);

            Response response = (Response)WaitForCompletion(mgr, timeout);

            if (response == null)
            {
                throw new ConnectionException("Timeout executing request");
            }
            return response;
        }

        /// <summary>
        /// Connect to the WAP gateway. Before requests can be executed, this method
        /// must be called.
        /// </summary>
        /// <throws>  SocketException if the connection could not be established </throws>
        /// <throws>  IllegalStateException if the client is already connected </throws>
        public virtual void Connect()
        {
            lock (this)
            {
                Connect(DefaultConnectTimeOut);
            }
        }
        /// <summary>
        /// Connect to the WAP gateway. Before requests can be executed, this method
        /// must be called.
        /// </summary>
        /// <param name="timeout">timeout in milliseconds</param>
        /// <throws>  SocketException if the connection could not be established </throws>
        ///   
        /// <throws>  IllegalStateException if the client is already connected </throws>
        public virtual void Connect(long timeout)
        {
            lock (this)
            {
                Connect(null, timeout);
            }
        }

        /// <summary>
        /// Connect to the WAP gateway. Before requests can be executed, this method
        /// must be called.
        /// </summary>
        /// <param name="headers">WSP headers used for connect or null
        /// objects. The headers will be encoded using the default WAP codepage.</param>
        /// <param name="timeout">timeout in milliseconds</param>
        /// <throws>SocketException if the connection could not be established </throws>
        /// <throws>IllegalStateException if the client is already connected </throws>
        public virtual void Connect(CWSPHeaders headers, long timeout)
        {
            lock (this)
            {
                if (session != null)
                {
                    throw new Exception("Already connected");
                }

                disconnectTimeout = timeout;
                pendingRequests.Clear();
                Logger.LogThis("Establishing WSP session with " + gatewayAddress.ToString() + ":" + gatewayPort, LogLevel.Verbose);
                session = new CWSPSession(gatewayAddress, gatewayPort, localAddr, localPort, upperLayerImpl, false);
                session.Connect(headers);
                object result = WaitForCompletion(sessionLock, timeout);

                if (result == null)
                {
                    CWSPSession ts = session;
                    session = null;
                    try
                    {
                        ts.SDisconnect();
                    }
                    catch (Exception unknown)
                    {
                        Logger.LogThis(unknown.Message, LogLevel.Error);
                    }
                    throw new ConnectionException("connect: Timeout occurred");
                }

                if (result != null)
                {
                    if (result is CWSPSocketAddress[])
                    {
                        // redirect received ...
                        CWSPSocketAddress[] addr = (CWSPSocketAddress[])result;
                        if (addr.Length > 0)
                        {
                            // Take the first address and try to reconnect...
                            gatewayAddress = addr[0].Address;
                            int p = addr[0].Port;
                            if (p > 0)
                            {
                                gatewayPort = p;
                            }
                            session = null;
                            Logger.LogThis("Redirect to " + gatewayAddress.ToString() + ":" + gatewayPort, LogLevel.Verbose);

                            Connect(headers, timeout);
                            return;
                        }
                    }
                    else if (!ConnectedResult.Equals(result))
                    {
                        CWSPSession ts = session;
                        session = null;
                        ts.SDisconnect();
                        if (result == null)
                        {
                            throw new ConnectionException("Timeout while establishing connection");
                        }
                        else if (!ConnectedResult.Equals(result))
                        {
                            throw new ConnectionException("Connection failed.");
                        }
                    }
                }
                Logger.LogThis("Connection established", LogLevel.Verbose);
            }
        }

        /// <summary>
        /// Disconnect from the WAP gateway. This releases used resources as well.
        /// </summary>
        public virtual void Disconnect()
        {
            lock (this)
            {
                if (session == null)
                {
                    return;
                }
                Logger.LogThis("Disconnecting client...", LogLevel.Verbose);
                CWSPSession ts = session;
                session = null;
                ts.SDisconnect();

                // Wait for confirmation
                object result = WaitForCompletion(sessionLock, disconnectTimeout);
                session = null;
                Logger.LogThis("Client disconnected...", LogLevel.Verbose);
            }
        }


        /// <summary>
        /// Reads the post data.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns></returns>
        private static byte[] ReadPostData(string input)
        {
            MemoryStream outputStream = new MemoryStream();
            Stream inputStream = null;
            if (input == null)
            {
                Console.Out.WriteLine("Reading post-data from input stream, hit EOF when done");
                inputStream = Console.OpenStandardInput();
            }
            else if ("-".Equals(input))
            {
                inputStream = Console.OpenStandardInput();
            }
            else
            {
                inputStream = new FileStream(input, FileMode.Open, FileAccess.Read);
            }
            byte[] buf = new byte[1024];
            int read = 0;
            while ((read = ByteHelper.ReadStream(inputStream, buf, 0, buf.Length)) > 0)
            {
                outputStream.Write(buf, 0, read);
            }
            inputStream.Close();
            return outputStream.ToArray();
        }

        /// <summary>
        /// Waits for completion.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="timeout">The timeout.</param>
        /// <returns></returns>
        private object WaitForCompletion(object key, long timeout)
        {

            object obj = null;
            long startAt = 0;

            if (timeout > 0)
            {
                startAt = (DateTime.Now.Ticks - 621355968000000000) / 10000;
            }
            while (obj == null)
            {
                if (timeout > 0 && (startAt + timeout) < (DateTime.Now.Ticks - 621355968000000000) / 10000)
                {
                    Logger.LogThis("Timeout occurred", LogLevel.Verbose);
                    break;
                }
                lock (pendingRequests)
                {
                    object tempObject;
                    tempObject = pendingRequests[key];
                    pendingRequests.Remove(key);
                    obj = tempObject;
                    if (obj == null)
                    {
                        try
                        {
                            Monitor.Wait(pendingRequests, TimeSpan.FromMilliseconds(timeout));
                        }
                        catch (ThreadInterruptedException e)
                        {
                            Logger.LogThis("Interrupted", LogLevel.Warn);
                        }
                    }
                }
            }
            return obj;
        }

        /// <summary>
        /// Completes the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        private void Complete(object key, object value)
        {
            lock (pendingRequests)
            {
                pendingRequests[key] = value;
                Monitor.PulseAll(pendingRequests);
            }
        }


        // -----------------------------------------------------------
        /// <summary>
        /// Upper layer implementation
        /// </summary>
        private class UpperLayerImpl : IWSPUpperLayer2
        {
            private WAPClient wapClient;

            /// <summary>
            /// Initializes a new instance of the <see cref="UpperLayerImpl"/> class.
            /// </summary>
            /// <param name="client">The client.</param>
            public UpperLayerImpl(WAPClient client)
            {
                Initalize(client);
            }

            /// <summary>
            /// Initalizes the specified client.
            /// </summary>
            /// <param name="client">The client.</param>
            private void Initalize(WAPClient client)
            {
                this.wapClient = client;
            }

            /// <summary>
            /// Gets the client.
            /// </summary>
            /// <value>The client.</value>
            public WAPClient Client
            {
                get
                {
                    return wapClient;
                }

            }
                        
            /// <summary>
            /// Connects the CNF.
            /// </summary>
            public virtual void ConnectCnf()
            {
                Client.Complete(Client.sessionLock, WAPClient.ConnectedResult);
            }

            /// <summary>
            /// Disconnects the ind.
            /// </summary>
            /// <param name="reason">The reason.</param>
            public virtual void DisconnectInd(short reason)
            {
                //Logger.LogThis("s_disconnect_ind(" + reason + ")", LogLevel.Verbose);
                Client.Complete(Client.sessionLock, "DISCONNECTED: " + reason);
                Client.session = null;
            }

            /// <summary>
            /// Disconnects the ind.
            /// </summary>
            /// <param name="redirectInfo">The redirect info.</param>
            public virtual void DisconnectInd(CWSPSocketAddress[] redirectInfo)
            {
                Client.Complete(Client.sessionLock, redirectInfo);
            }

            /// <summary>
            /// Methods the result ind.
            /// </summary>
            /// <param name="result">The result.</param>
            public virtual void MethodResultInd(CWSPResult result)
            {
                Response response = new Response(result);
                CWSPMethodManager mgr = result.MethodManager;
                mgr.MethodResult(null);
                Client.Complete(mgr, response);
            }

            /// <summary>
            /// Suspends the ind.
            /// </summary>
            /// <param name="reason">The reason.</param>
            public virtual void SuspendInd(short reason)
            {
                //Logger.LogThis("s_suspend_ind(" + reason + ")", LogLevel.Verbose);

            }

            /// <summary>
            /// Resumes the CNF.
            /// </summary>
            public virtual void ResumeCnf()
            {
                //Logger.LogThis("s_resume_cnf()", LogLevel.Verbose);
            }

            /// <summary>
            /// Disconnects the ind.
            /// </summary>
            /// <param name="redirectInfo">The redirect info.</param>
            public virtual void DisconnectInd(IPAddress[] redirectInfo)
            {
            }

            /// <summary>
            /// Methods the result ind.
            /// </summary>
            /// <param name="payload">The payload.</param>
            /// <param name="contentType">Type of the content.</param>
            /// <param name="moreData">if set to <c>true</c> [more data].</param>
            public virtual void MethodResultInd(byte[] payload, string contentType, bool moreData)
            {
            }
        }
    }
}
