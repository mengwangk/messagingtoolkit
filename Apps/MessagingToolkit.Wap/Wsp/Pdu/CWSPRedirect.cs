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
using MessagingToolkit.Wap.Wsp;


namespace MessagingToolkit.Wap.Wsp.Pdu
{
    /// <summary>
    /// WSP Redirect
    /// </summary>
	public class CWSPRedirect:CWSPPDU
	{
        /// <summary> 
        /// Table 6
        /// section 8.2.2.3
        /// </summary>
        public const short FlagPermanentRedirect = (0x80);
        public const short FlagReuseSecuritySession = (0x40);
        
        // uint8
        private short flags = (0x00);

        /// <summary> 
        /// Redirect address(es)
        /// usage of CWSPAddress
        /// </summary>
        private ArrayList addresses = ArrayList.Synchronized(new ArrayList(10));
		

		virtual public IPAddress[] InetAddresses
		{
			//public Vector getAddresses(){
			//  return addresses;
			//}
			
			get
			{
				IPAddress[] result = new IPAddress[addresses.Count];
				
				for (int i = 0; i < addresses.Count; i++)
				{
					result[i] = ((CWSPAddress) (addresses[i])).InetAddress;
				}
				
				return result;
			}
			
		}
        /// <summary>
        /// Gets the socket addresses.
        /// </summary>
        /// <value>The socket addresses.</value>
		virtual public CWSPSocketAddress[] SocketAddresses
		{
			get
			{
				CWSPSocketAddress[] result = new CWSPSocketAddress[addresses.Count];
				
				for (int i = 0; i < addresses.Count; i++)
				{
					result[i] = ((CWSPAddress) (addresses[i])).SocketAddress;
				}
				
				return result;
			}			
		}


        /// <summary>
        /// Initializes a new instance of the <see cref="CWSPRedirect"/> class.
        /// </summary>
		public CWSPRedirect():base()
		{
			pduType = CWSPPDU.PduTypeRedirect;
		}

        /// <summary>
        /// Encodes the PDU according to WAP-230-WSP-20010705-A.
        /// See <a href="http://www.wapforum.org">www.wapforum.org</a> for more information.
        /// </summary>
        /// <returns></returns>
		public override byte[] ToByteArray()
		{
			BitArrayOutputStream tmp = new BitArrayOutputStream();
			BitArrayOutputStream result = new BitArrayOutputStream();
			result.Write(pduType, 8);
			result.Write(flags, 8);
			
			if (!(addresses.Count == 0))
			{
				for (int i = 0; i < addresses.Count; i++)
				{
					tmp.Reset();
					
					CWSPAddress address = (CWSPAddress) addresses[i];
					result.Write(address.Bytes);
				}
			}
			
			return result.ToByteArray();
		}

        /// <summary>
        /// set flag according to sect. 8.2.2.3
        /// </summary>
        /// <param name="option">The option.</param>
        /// <param name="on_off">if set to <c>true</c> [on_off].</param>
		public virtual void  SetFlag(short option, bool onOff)
		{
			if (onOff)
			{
				if (!GetFlag(option))
				{
					flags = (short) (flags + option);
				}
			}
			else
			{
				if (GetFlag(option))
				{
					flags = (short) (flags - option);
				}
			}
		}

        /// <summary>
        /// is flag <code>option</code> set?
        /// </summary>
        /// <param name="option">The option.</param>
        /// <returns></returns>
		public virtual bool GetFlag(short option)
		{
			BitArray s = BitArrayOutputStream.GetBitSet(GetFlags());
		    return s.Get((int) (Math.Log(option) / Math.Log(2)));
		}

        /// <summary>
        /// return all flags as an byte
        /// </summary>
        /// <returns></returns>
		public virtual byte GetFlags()
		{
			BitArrayOutputStream m = new BitArrayOutputStream();
			m.Write(flags, 8);
			
			byte[] b = m.ToByteArray();
			
			return b[0];
		}

        /// <summary>
        /// Sets the flags.
        /// </summary>
        /// <param name="flags">The flags.</param>
		public virtual void SetFlags(short flags)
		{
			this.flags = flags;
		}

        /// <summary>
        /// adds a redirect address
        /// </summary>
        /// <param name="bearerTypeIncluded">if set to <c>true</c> [bearer type included].</param>
        /// <param name="portNumberIncluded">if set to <c>true</c> [port number included].</param>
        /// <param name="bearerType">Type of the bearer.</param>
        /// <param name="portNumber">The port number.</param>
        /// <param name="bearerAddressToUse">The bearer address to use.</param>
        /// <returns>
        /// boolean true = added
        /// false = NOT added (already added)
        /// </returns>
		public virtual bool AddAddress(bool bearerTypeIncluded, bool portNumberIncluded, short bearerType, int portNumber, string bearerAddressToUse)
		{
			CWSPAddress toAdd = new CWSPAddress(bearerTypeIncluded, portNumberIncluded, bearerType, portNumber, bearerAddressToUse);
			
			for (int i = 0; i < addresses.Count; i++)
			{
				CWSPAddress search = (CWSPAddress) addresses[i];
				
				if (search.Equals(toAdd))
				{
					return false;
				}
			}
			
			addresses.Add(toAdd);
			
			return true;
		}

        /// <summary>
        /// removes a redirect address
        /// </summary>
        /// <param name="bearerTypeIncluded">if set to <c>true</c> [bearer type included].</param>
        /// <param name="portNumberIncluded">if set to <c>true</c> [port number included].</param>
        /// <param name="bearerType">Type of the bearer.</param>
        /// <param name="portNumber">The port number.</param>
        /// <param name="bearerAddressToUse">The bearer address to use.</param>
        /// <returns>
        /// boolean true = removed
        /// false = address does not exist
        /// </returns>
		public virtual bool RemoveAddress(bool bearerTypeIncluded, bool portNumberIncluded, short bearerType, int portNumber, string bearerAddressToUse)
		{
			CWSPAddress toRemove = new CWSPAddress(bearerTypeIncluded, portNumberIncluded, bearerType, portNumber, bearerAddressToUse);
			
			for (int i = 0; i < addresses.Count; i++)
			{
				CWSPAddress search = (CWSPAddress) addresses[i];
				
				if (search.Equals(toRemove))
				{
					addresses.Remove(search);
					
					return true;
				}
			}
			
			return false;
		}

        /// <summary>
        /// Sets the addresses.
        /// </summary>
        /// <param name="addressList">The address list.</param>
		public virtual void SetAddresses(ArrayList addressList)
		{
			addresses = addressList;
		}
	}
}