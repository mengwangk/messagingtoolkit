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
    /// Capabilities allow the client to negotiate characteristics
    /// and extended behaviours of the protocol (see section 8.3 of the WSP spec.).
    /// </summary>
	public class CWSPCapabilities
	{
        /// <summary> Table 37</summary>
        public const short TypeClientSDUSize = (0x00);
        public const short TypeServerSDUSize = (0x01);
        public const short TypeProtocolOptions = (0x02);
        public const short TypeMethodMOR = (0x03);
        public const short TypePushMOR = (0x04);
        public const short TypeExtendedMethods = (0x05);
        public const short TypeHeaderCodePages = (0x06);
        public const short TypeAliases = (0x07);
        public const short TypeClientMessageSize = (0x08);
        public const short TypeServerMessageSize = (0x09);

        /// <summary> section 8.3.2.3</summary>
        public const short OptionConfPushFacility = (0x80);
        public const short OptionPushFacility = (0x40);
        public const short OptionSessionResumeFacility = (0x20);
        public const short OptionAckHeaders = (0x10);
        public const short OptionLargeDataTransfer = (0x08);

        /// <summary> section 8.3.2.1
        /// unintvar
        /// </summary>
        private long clientSDUSize = 1400;
        private bool bClientSDUSize = false;
        private long serverSDUSize = 1400;
        private bool bServerSDUSize = false;

        /// <summary> section 8.3.2.3
        /// one octet
        /// </summary>
        private long protocolOptions = 0x00;
        private bool bProtocolOptions = false;

        /// <summary> section 8.3.2.4
        /// uint8
        /// </summary>
        private long methodMOR = 1;
        private bool bMethodMOR = false;
        private long pushMOR = 1;
        private bool bPushMOR = false;

        /// <summary> 
        /// section 8.3.2.5
        /// uint8
        /// multiple octets
        /// </summary>
        private Hashtable extendedMethods = Hashtable.Synchronized(new Hashtable());

        /// <summary> 
        /// section 8.3.2.6
        /// uint8
        /// multiple octets
        /// </summary>
        private Hashtable headerCodePages = Hashtable.Synchronized(new Hashtable());

        /// <summary> 
        /// section 8.3.2.7
        /// multiple octets
        /// encoded as redirect address in sec. 8.2.2.3
        /// 
        /// BearerTypeIncluded, PortNumberIncluded, AddressLength
        /// </summary>
        internal ArrayList aliases = ArrayList.Synchronized(new ArrayList(10));

        /// <summary> 
        /// section 8.3.2.2
        /// unintvar
        /// </summary>
        private long clientMessageSize = 1400;
        private bool bClientMessageSize = false;
        private long serverMessageSize = 1400;
        private bool bServerMessageSize = false;
		

		virtual public long ClientSDUSize
		{
			get
			{
				return clientSDUSize;
			}
			
			/////////////////////////////////////////////////////////////////////////////
			//////////////////////////////// GET/SET ////////////////////////////////////
			
			set
			{
				this.clientSDUSize = value;
				this.bClientSDUSize = true;
			}
			
		}
		virtual public long ServerSDUSize
		{
			get
			{
				return serverSDUSize;
			}
			
			set
			{
				this.serverSDUSize = value;
				this.bServerSDUSize = true;
			}
			
		}
		virtual public byte ProtocolOptions
		{
			get
			{
				BitArrayOutputStream m = new BitArrayOutputStream();
				m.Write(protocolOptions, 8);
				
				byte[] b = m.ToByteArray();
				
				return b[0];
			}
			
		}
		virtual public long MethodMOR
		{
			get
			{
				return methodMOR;
			}
			
			set
			{
				this.methodMOR = value;
				this.bMethodMOR = true;
			}
			
		}
		virtual public long PushMOR
		{
			get
			{
				return pushMOR;
			}
			
			set
			{
				this.pushMOR = value;
				this.bPushMOR = true;
			}
			
		}
		virtual public Hashtable ExtendedMethods
		{
			get
			{
				return extendedMethods;
			}
			
		}
		virtual public Hashtable HeaderCodePages
		{
			get
			{
				return headerCodePages;
			}
			
		}
		virtual public ArrayList Aliases
		{
			get
			{
				return aliases;
			}
			
		}
		virtual public long ClientMessageSize
		{
			get
			{
				return clientMessageSize;
			}
			
			set
			{
				clientMessageSize = value;
				this.bClientMessageSize = true;
			}
			
		}
		virtual public long ServerMessageSize
		{
			get
			{
				return serverMessageSize;
			}
			
			set
			{
				serverMessageSize = value;
				this.bServerMessageSize = true;
			}
			
		}
        /// <summary>
        /// encode all capabilities in a byte array
        /// according to WAP-230-WSP-20010705-a section 8.3 "capability encoding"
        /// </summary>
        /// <value>The bytes.</value>
        /// <returns> The encoded bytes
        /// </returns>
		virtual public byte[] Bytes
		{
			get
			{
				BitArrayOutputStream result = new BitArrayOutputStream();
				BitArrayOutputStream tmp = new BitArrayOutputStream();
				byte[] tmp2;
				
				if (this.bClientSDUSize)
				{
					tmp.Write(TypeClientSDUSize + 0x80, 8);
					tmp.WriteUIntVar(clientSDUSize);
					tmp2 = tmp.ToByteArray();
					result.WriteUIntVar(tmp2.Length);
					result.Write(tmp2);
				}
				
				if (bServerSDUSize)
				{
					tmp.Reset();
					tmp.Write(TypeServerSDUSize + 0x80, 8);
					tmp.WriteUIntVar(serverSDUSize);
					tmp2 = tmp.ToByteArray();
					result.WriteUIntVar(tmp2.Length);
					result.Write(tmp2);
				}
				
				if (bClientMessageSize)
				{
					tmp.Reset();
					tmp.Write(TypeClientMessageSize + 0x80, 8);
					tmp.WriteUIntVar(clientMessageSize);
					tmp2 = tmp.ToByteArray();
					result.WriteUIntVar(tmp2.Length);
					result.Write(tmp2);
				}
				
				if (bServerMessageSize)
				{
					tmp.Reset();
					tmp.Write(TypeServerMessageSize + 0x80, 8);
					tmp.WriteUIntVar(serverMessageSize);
					tmp2 = tmp.ToByteArray();
					result.WriteUIntVar(tmp2.Length);
					result.Write(tmp2);
				}
				
				if (bProtocolOptions)
				{
					tmp.Reset();
					tmp.Write(TypeProtocolOptions + 0x80, 8);
					tmp.Write(protocolOptions, 8);
					tmp2 = tmp.ToByteArray();
					result.WriteUIntVar(tmp2.Length);
					result.Write(tmp2);
				}
				
				if (bMethodMOR)
				{
					tmp.Reset();
					tmp.Write(TypeMethodMOR + 0x80, 8);
					tmp.WriteUIntVar(methodMOR);
					tmp2 = tmp.ToByteArray();
					result.WriteUIntVar(tmp2.Length);
					result.Write(tmp2);
				}
				
				if (bPushMOR)
				{
					tmp.Reset();
					tmp.Write(TypePushMOR + 0x80, 8);
					tmp.WriteUIntVar(pushMOR);
					tmp2 = tmp.ToByteArray();
					result.WriteUIntVar(tmp2.Length);
					result.Write(tmp2);
				}
				
				if (!(extendedMethods.Count == 0))
				{
					IEnumerator keys = extendedMethods.Keys.GetEnumerator();
					
					while (keys.MoveNext())
					{
						tmp.Reset();
						
						object key = keys.Current;
						tmp.Write(TypeExtendedMethods + 0x80, 8);
						tmp.Write((long) ((System.Int64) key), 8);
						
						string value = (string) (extendedMethods[key]);
						tmp.Write(ByteHelper.GetBytes(value));
						tmp.Write(0x00, 8);
						tmp2 = tmp.ToByteArray();
						result.WriteUIntVar(tmp2.Length);
						result.Write(tmp2);
					}
				}
				
				if (!(headerCodePages.Count == 0))
				{
					IEnumerator keys = extendedMethods.Keys.GetEnumerator();
					
					while (keys.MoveNext())
					{
						tmp.Reset();
						
						object key = keys.Current;
						tmp.Write(TypeHeaderCodePages + 0x80, 8);
						tmp.Write((long) ((System.Int64) key), 8);
						
						string value = (string) (extendedMethods[key]);
						tmp.Write(ByteHelper.GetBytes(value));
						tmp.Write(0x00, 8);
						tmp2 = tmp.ToByteArray();
						result.WriteUIntVar(tmp2.Length);
						result.Write(tmp2);
					}
				}
				
				if (!(aliases.Count == 0))
				{
					for (int i = 0; i < aliases.Count; i++)
					{
						tmp.Reset();
						
						CWSPAddress address = (CWSPAddress) aliases[i];
						tmp.Write(TypeAliases + 0x80, 8);
						tmp.Write(address.Bytes);
						tmp2 = tmp.ToByteArray();
						result.WriteUIntVar(tmp2.Length);
						result.Write(tmp2);
					}
				}
				
				return result.ToByteArray();
			}
			
		}

        /// <summary>
        /// set a protocoloption
        /// use constans in this class beginning with "OPTION_"
        /// </summary>
        /// <param name="option">The option.</param>
        /// <param name="onOff">if set to <c>true</c> [on off].</param>
		public virtual void  SetProtocolOption(short option, bool onOff)
		{
			if (onOff)
			{
				if (!GetProtocolOption(option))
				{
					this.bProtocolOptions = true;
					protocolOptions += option;
				}
			}
			else
			{
				if (GetProtocolOption(option))
				{
					protocolOptions -= option;
				}
			}
		}

        /// <summary>
        /// is the Option <code>option</code> set?
        /// use constants in this class beginnig with "OPTION_"
        /// </summary>
        /// <param name="option">The option.</param>
        /// <returns></returns>
		public virtual bool GetProtocolOption(short option)
		{
			BitArray s = BitArrayOutputStream.GetBitSet(ProtocolOptions);
			
			return s.Get((int) (Math.Log(option) / Math.Log(2)));
		}

        /// <summary>
        /// Adds the extended method.
        /// </summary>
        /// <param name="pduType">Type of the pdu.</param>
        /// <param name="name">The name.</param>
        /// <returns></returns>
		public virtual bool AddExtendedMethod(long pduType, string name)
		{
			System.Int64 toAdd = (long) pduType;
			IEnumerator keys = extendedMethods.Keys.GetEnumerator();
			while (keys.MoveNext())
			{
				System.Int64 akt = (System.Int64) (keys.Current);
				
				if (toAdd.Equals(akt))
				{
					return false;
				}
			}
			
			extendedMethods[(long) pduType] = name;
			
			return true;
		}

        /// <summary>
        /// Removes the extended method.
        /// </summary>
        /// <param name="pduType">Type of the pdu.</param>
        /// <returns></returns>
		public virtual bool RemoveExtendedMethod(long pduType)
		{
			System.Int64 search = (long) pduType;
			IEnumerator keys = extendedMethods.Keys.GetEnumerator();
			
			while (keys.MoveNext())
			{
			    System.Int64 akt = (System.Int64) (keys.Current);
				
				if (search.Equals(akt))
				{
					extendedMethods.Remove(akt);
					
					return true;
				}
			}
			
			return false;
		}

        /// <summary>
        /// add a header code page
        /// </summary>
        /// <param name="pageCode">The page code.</param>
        /// <param name="name">The name.</param>
        /// <returns></returns>
		public virtual bool AddHeaderCodePage(long pageCode, string name)
		{
			System.Int64 toAdd = (long) pageCode;
			IEnumerator keys = headerCodePages.Keys.GetEnumerator();
			
			while (keys.MoveNext())
			{
				System.Int64 akt = (System.Int64) (keys.Current);
				
				if (toAdd.Equals(akt))
				{
					return false;
				}
			}
			
			headerCodePages[toAdd] = name;
			
			return true;
		}

        /// <summary>
        /// remove a header code page if it exists.
        /// </summary>
        /// <param name="pageCode">The page code.</param>
        /// <returns></returns>
		public virtual bool RemoveHeaderCodePage(long pageCode)
		{
			System.Int64 search = (long) pageCode;
			IEnumerator keys = headerCodePages.Keys.GetEnumerator();
			
			while (keys.MoveNext())
			{
				System.Int64 akt = (System.Int64) (keys.Current);
				
				if (search.Equals(akt))
				{
					headerCodePages.Remove(akt);
					
					return true;
				}
			}
			
			return false;
		}

        /// <summary>
        /// add a alias (alternate address of the sender)
        /// </summary>
        /// <param name="bearerTypeIncluded">if set to <c>true</c> [bearer type included].</param>
        /// <param name="portNumberIncluded">if set to <c>true</c> [port number included].</param>
        /// <param name="bearerType">Type of the bearer.</param>
        /// <param name="portNumber">The port number.</param>
        /// <param name="bearerAddressToUse">The bearer address to use.</param>
        /// <returns></returns>
		public virtual bool AddAlias(bool bearerTypeIncluded, bool portNumberIncluded, short bearerType, int portNumber, string bearerAddressToUse)
		{
			CWSPAddress toAdd = new CWSPAddress(bearerTypeIncluded, portNumberIncluded, bearerType, portNumber, bearerAddressToUse);
			
			for (int i = 0; i < aliases.Count; i++)
			{
				CWSPAddress search = (CWSPAddress) aliases[i];
				
				if (search.Equals(toAdd))
				{
					return false;
				}
			}
			
			aliases.Add(toAdd);
			
			return true;
		}

        /// <summary>
        /// Remove a alias using the values as given
        /// </summary>
        /// <param name="bearerTypeIncluded">if set to <c>true</c> [bearer type included].</param>
        /// <param name="portNumberIncluded">if set to <c>true</c> [port number included].</param>
        /// <param name="bearerType">Type of the bearer.</param>
        /// <param name="portNumber">The port number.</param>
        /// <param name="bearerAddressToUse">The bearer address to use.</param>
        /// <returns></returns>
		public virtual bool RemoveAlias(bool bearerTypeIncluded, bool portNumberIncluded, short bearerType, int portNumber, string bearerAddressToUse)
		{
			CWSPAddress toRemove = new CWSPAddress(bearerTypeIncluded, portNumberIncluded, bearerType, portNumber, bearerAddressToUse);
			
			for (int i = 0; i < aliases.Count; i++)
			{
				CWSPAddress search = (CWSPAddress) aliases[i];
				
				if (search.Equals(toRemove))
				{
					aliases.Remove(search);
					
					return true;
				}
			}
			
			return false;
		}
		
		
	}
}