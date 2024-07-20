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
using MessagingToolkit.Wap.Wsp;


namespace MessagingToolkit.Wap.Wsp.Pdu
{

    /// <summary>
    /// WSP address
    /// </summary>
	public class CWSPAddress
	{
        /// <summary> 
        /// Bearer Types according to WAP-259-WDP-20010614-a
        /// only two of them implemented here!
        /// </summary>
        public const int BEARER_TYPE_GSM_CSD = 0x0A;
        public const int BEARER_TYPE_GSM_GPRS = 0x0B;
        public bool bearerTypeIncluded; // 1st bit
        public bool portNumberIncluded; // 2nd bit
        public long addressLen; // 6bits
        public long bearerType; // uint8
        public long portNumber; // uint16
        public short bearerAddressToUse1; // multiple octets
        public short bearerAddressToUse2;
        public short bearerAddressToUse3;
        public short bearerAddressToUse4;

        /// <summary>
        /// get the encoded byte array of the address
        /// </summary>
        /// <value>The bytes</value>
		virtual public byte[] Bytes
		{
			get
			{
				BitArrayOutputStream output = new BitArrayOutputStream();
				output.Write(bearerTypeIncluded);
				output.Write(portNumberIncluded);
				output.Write(addressLen, 6);
				output.Write(bearerType, 8);
				output.Write(portNumber, 16);
				output.Write(bearerAddressToUse1, 8);
				output.Write(bearerAddressToUse2, 8);
				output.Write(bearerAddressToUse3, 8);
				output.Write(bearerAddressToUse4, 8);
				
				return output.ToByteArray();
			}			
		}

        /// <summary>
        /// Gets a value indicating whether [bearer type included].
        /// </summary>
        /// <value><c>true</c> if [bearer type included]; otherwise, <c>false</c>.</value>
		virtual public bool BearerTypeIncluded
		{			
			get
			{
				return bearerTypeIncluded;
			}			
		}

        /// <summary>
        /// Gets a value indicating whether [port number included].
        /// </summary>
        /// <value><c>true</c> if [port number included]; otherwise, <c>false</c>.</value>
		virtual public bool PortNumberIncluded
		{
			get
			{
				return portNumberIncluded;
			}
			
		}
        /// <summary>
        /// Gets the address len.
        /// </summary>
        /// <value>The address len.</value>
		virtual public long AddressLen
		{
			get
			{
				return addressLen;
			}
			
		}
        /// <summary>
        /// Gets the type of the bearer.
        /// </summary>
        /// <value>The type of the bearer.</value>
		virtual public long BearerType
		{
			get
			{
				return bearerType;
			}
			
		}
        /// <summary>
        /// Gets the port number.
        /// </summary>
        /// <value>The port number.</value>
		virtual public long PortNumber
		{
			get
			{
				return portNumber;
			}
			
		}
        /// <summary>
        /// Gets the inet address.
        /// </summary>
        /// <value>The inet address.</value>
		virtual public IPAddress InetAddress
		{
			get
			{
				IPAddress ret = null;
				string addr = BearerAddressToUse;
				try
				{
					//ret = Dns.GetHostEntry(addr).AddressList[0];
                    ret = Dns.GetHostAddresses(addr)[0];
				}
				catch (Exception e)
				{
					Logger.LogThis(addr + ": Unable to convert to InetAddress: " + e.Message, LogLevel.Warn);
				}
				return ret;
			}
			
		}
        /// <summary>
        /// Gets the socket address.
        /// </summary>
        /// <value>The socket address.</value>
		virtual public CWSPSocketAddress SocketAddress
		{
			get
			{
				IPAddress addr = InetAddress;
				CWSPSocketAddress ret = null;
				if (addr != null)
				{
					ret = new CWSPSocketAddress(addr, (int) portNumber);
				}
				return ret;
			}
			
		}
        /// <summary>
        /// Gets the bearer address to use.
        /// </summary>
        /// <value>The bearer address to use.</value>
		virtual public string BearerAddressToUse
		{
			get
			{
				StringBuilder sb = new StringBuilder();
				sb.Append(bearerAddressToUse1).Append(".").Append(bearerAddressToUse2).Append(".").Append(bearerAddressToUse3).Append(".").Append(bearerAddressToUse4);
				return sb.ToString();
			}			
		}

        /// <summary>
        /// Initializes a new instance of the <see cref="CWSPAddress"/> class.
        /// </summary>
        /// <param name="bearerTypeIncluded">if set to <c>true</c> [bearer type included].</param>
        /// <param name="portNumberIncluded">if set to <c>true</c> [port number included].</param>
        /// <param name="bearerType">Type of the bearer.</param>
        /// <param name="portNumber">The port number.</param>
        /// <param name="bearerAddressToUse_IP">The bearer address to use_ IP.</param>
		public CWSPAddress(bool bearerTypeIncluded, bool portNumberIncluded, short bearerType, int portNumber, string bearerAddressToUse_IP)
		{
			this.bearerTypeIncluded = bearerTypeIncluded;
			this.portNumberIncluded = portNumberIncluded;
			this.addressLen = 4;
			this.bearerType = bearerType;
			this.portNumber = portNumber;
			
			Tokenizer t = new Tokenizer(bearerAddressToUse_IP, ".", false);
			bearerAddressToUse1 = System.Int16.Parse(t.NextToken());
			bearerAddressToUse2 = System.Int16.Parse(t.NextToken());
			bearerAddressToUse3 = System.Int16.Parse(t.NextToken());
			bearerAddressToUse4 = System.Int16.Parse(t.NextToken());
		}

        /// <summary>
        /// copmpares all fields of the address
        /// </summary>
        /// <param name="a">The <see cref="System.Object"/> to compare with this instance.</param>
        /// <returns>
        /// 	<c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
		public  override bool Equals(object a)
		{
			CWSPAddress b = (CWSPAddress) a;
			
			if ((this.bearerTypeIncluded == b.bearerTypeIncluded) && (this.portNumberIncluded == b.portNumberIncluded) && (this.addressLen == b.addressLen) && (this.bearerType == b.bearerType) && (this.portNumber == b.portNumber) && (this.bearerAddressToUse1 == b.bearerAddressToUse1) && (this.bearerAddressToUse2 == b.bearerAddressToUse2) && (this.bearerAddressToUse3 == b.bearerAddressToUse3) && (this.bearerAddressToUse4 == b.bearerAddressToUse4))
			{
				return true;
			}
			else
			{
				return false;
			}
		}

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}