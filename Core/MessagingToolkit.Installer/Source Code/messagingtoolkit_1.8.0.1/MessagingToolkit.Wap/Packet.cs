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
    /// Class to manage packets
    /// </summary>
    internal class Packet
    {
        private byte[] data;
        private int length;
        private IPEndPoint ipEndPoint;

        int port = -1;
        IPAddress address = null;

        /// <summary>
        /// Constructor for the packet
        /// </summary>	
        /// <param name="data">The buffer to store the data</param>	
        /// <param name="data">The length of the data sent</param>	
        /// <returns>A new packet to receive data of the specified length</returns>	
        public Packet(byte[] data, int length)
        {
            if (length > data.Length)
                throw new ArgumentException("illegal length");

            this.data = data;
            this.length = length;
            this.ipEndPoint = null;
        }

        /// <summary>
        /// Constructor for the packet
        /// </summary>	
        /// <param name="data">The data to be sent</param>	
        /// <param name="data">The length of the data to be sent</param>	
        /// <param name="data">The IP of the destination point</param>	
        /// <returns>A new packet with the data, length and ipEndPoint set</returns>
        public Packet(byte[] data, int length, IPEndPoint ipendpoint)
        {
            if (length > data.Length)
                throw new ArgumentException("illegal length");

            this.data = data;
            this.length = length;
            this.ipEndPoint = ipendpoint;
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
        /// Gets and sets the address
        /// </summary>			
        /// <returns>The int value of the address</returns>
        public IPAddress Address
        {
            get
            {
                return address;
            }
            set
            {
                address = value;
                if (this.ipEndPoint == null)
                {
                    if (Port >= 0 && Port <= 0xFFFF)
                        this.ipEndPoint = new IPEndPoint(value, Port);
                }
                else
                    this.ipEndPoint.Address = value;
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
                return port;
            }
            set
            {
                if (value < 0 || value > 0xFFFF)
                    throw new ArgumentException("Port out of range:" + value);

                port = value;
                if (this.ipEndPoint == null)
                {
                    if (Address != null)
                        this.ipEndPoint = new System.Net.IPEndPoint(Address, value);
                }
                else
                    this.ipEndPoint.Port = value;
            }
        }

        /// <summary>
        /// Gets and sets the length of the data
        /// </summary>			
        /// <returns>The int value of the length</returns>
        public int Length
        {
            get
            {
                return this.length;
            }
            set
            {
                if (value > data.Length)
                    throw new ArgumentException("illegal length");

                this.length = value;
            }
        }

        /// <summary>
        /// Gets and sets the byte array that contains the data
        /// </summary>			
        /// <returns>The byte array that contains the data</returns>
        public byte[] Data
        {
            get
            {
                return this.data;
            }

            set
            {
                this.data = value;
            }
        }
    }
}
