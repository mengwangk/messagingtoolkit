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
using System.Reflection;
using System.Collections;
using System.IO;
using System.Xml;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace MessagingToolkit.Wap
{
    /// <summary>
    /// Support class used to extend System.Net.Sockets.UdpClient class functionality
    /// </summary>
    internal class UdpSocket : UdpClient
    {
        public int port = -1;
        public IPEndPoint ipEndPoint = null;
        public string host = null;


        /// <summary>
        /// Initializes a new instance of the UdpClientSupport class, and binds it to the local port number provided.
        /// </summary>
        /// <param name="port">The local port number from which you intend to communicate.</param>
        public UdpSocket(int port)
            : base(port)
        {
            this.port = port;
        }

        /// <summary>
        /// Initializes a new instance of the UdpClientSupport class.
        /// </summary>
        public UdpSocket()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the UdpClientSupport class,
        /// and binds it to the specified local endpoint.
        /// </summary>
        /// <param name="IP">An IPEndPoint that respresents the local endpoint to which you bind the UDP connection.</param>
        public UdpSocket(IPEndPoint IP)
            : base(IP)
        {
            this.ipEndPoint = IP;
            this.port = this.ipEndPoint.Port;
        }

        /// <summary>
        /// Initializes a new instance of the UdpClientSupport class,
        /// and and establishes a default remote host.
        /// </summary>
        /// <param name="host">The name of the remote DNS host to which you intend to connect.</param>
        /// <param name="port">The remote port number to which you intend to connect. </param>
        public UdpSocket(string host, int port)
            : base(host, port)
        {
            this.host = host;
            this.port = port;
        }

        /// <summary>
        /// Returns a UDP datagram that was sent by a remote host.
        /// </summary>
        /// <param name="client">UDP client instance to use to receive the datagram</param>
        /// <param name="packet">Instance of the recieved datagram packet</param>
        public static void Receive(UdpClient client, out Packet packet)
        {
            IPEndPoint remoteIpEndPoint =
                new IPEndPoint(IPAddress.Any, 0);

            Packet tempPacket;
            try
            {
                byte[] dataIn = client.Receive(ref remoteIpEndPoint);
                tempPacket = new Packet(dataIn, dataIn.Length);
                tempPacket.IPEndPoint = remoteIpEndPoint;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            packet = tempPacket;
        }

        /// <summary>
        /// Sends a UDP datagram to the host at the specified remote endpoint.
        /// </summary>
        /// <param name="client">Client to use as source for sending the datagram</param>
        /// <param name="packet">Packet containing the datagram data to send</param>
        public static void Send(UdpClient client, Packet packet)
        {            
            if (client != null && packet != null)
                client.Send(packet.Data, packet.Length, packet.IPEndPoint);
        }


        /// <summary>
        /// Gets and sets the address of the IP
        /// </summary>			
        /// <returns>The IP address</returns>
        public IPEndPoint IPEndPoint
        {
            get
            {
                return this.ipEndPoint;
            }
            set
            {
                this.ipEndPoint = value;
            }
        }

        /// <summary>
        /// Gets and sets the port
        /// </summary>			
        /// <returns>The int value of the port</returns>
        public int Port
        {
            get
            {
                return this.port;
            }
            set
            {
                if (value < 0 || value > 0xFFFF)
                    throw new ArgumentException("Port out of range:" + value);

                this.port = value;
            }
        }


        /// <summary>
        /// Gets the address of the IP
        /// </summary>			
        /// <returns>The IP address</returns>
        public IPAddress GetIPEndPointAddress()
        {
            if (this.ipEndPoint == null)
                return null;
            else
                return (this.ipEndPoint.Address == null) ? null : this.ipEndPoint.Address;
        }

    }
}
