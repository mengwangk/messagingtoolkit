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

using MessagingToolkit.Wap.Wtp;
using MessagingToolkit.Wap.Log;
using MessagingToolkit.Wap.Helper;
using MessagingToolkit.Wap.Wsp.Pdu;


namespace MessagingToolkit.Wap.Wsp
{

    /// <summary>
    /// This class combines an InetAddress with a port number.
    /// </summary>
    public class CWSPSocketAddress
    {
        private IPAddress address;
        private int port;

        /// <summary>
        /// Gets the address.
        /// </summary>
        /// <value>The address.</value>
        virtual public IPAddress Address
        {
            get
            {
                return address;
            }

        }
        /// <summary>
        /// Gets the port.
        /// </summary>
        /// <value>The port.</value>
        virtual public int Port
        {
            get
            {
                return port;
            }

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CWSPSocketAddress"/> class.
        /// </summary>
        /// <param name="address">The address.</param>
        /// <param name="port">The port.</param>
        public CWSPSocketAddress(IPAddress address, int port)
        {
            if (address == null)
            {
                throw new NullReferenceException("Address may not be null");
            }
            this.address = address;
            this.port = port;
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return new StringBuilder(address.ToString()).Append(":").Append(port).ToString();
        }
    }
}