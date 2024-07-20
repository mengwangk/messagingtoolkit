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
using MessagingToolkit.Wap.Wsp.Headers;

namespace MessagingToolkit.Wap.Wsp.Pdu
{
    /// <summary>
    /// WSP PDU
    /// </summary>
	public abstract class CWSPPDU
	{
        /// <summary> Table 34</summary>
        public const short PduTypeConnect = (0x01);
        public const short PduTypeConnectReply = (0x02);
        public const short PduTypeRedirect = (0x03);
        public const short PduTypeReply = (0x04);
        public const short PduTypeDisconnect = (0x05);
        public const short PduTypePush = (0x06);
        public const short PduTypeConfirmedPush = (0x07);
        public const short PduTypeSuspend = (0x08);
        public const short PduTypeResume = (0x09);
        public const short PduTypeGet = (0x40);
        public const short PduTypeGetOptions = (0x41);
        public const short PduTypeGetHead = (0x42);
        public const short PduTypeGetDelete = (0x43);
        public const short PduTypeGetTrace = (0x44);
        public const short PduTypePost = (0x60);
        public const short PduTypePostPut = (0x61);
        public const short PduTypeDataFragment = (0x80);
        protected internal CWSPCapabilities capabilities = new CWSPCapabilities();
        protected internal CWSPHeaders headers = new CWSPHeaders();
        private static WAPCodePage wapCodePage;

        /// <summary> 
        /// TID
        /// unint8
        /// in case of connectionless WSP PDUs (NOT IMPLEMENTED IN THIS RELEASE!)
        /// get it from S-Unit-MethodInvoke.req::TID
        /// or S-Unit-MethodResult.req::TID
        /// or S-Unit-Push.req::push ID
        /// </summary>
        protected internal short TID;

        /// <summary> PDU Type
        /// unint8
        /// </summary>
        protected internal short pduType;
        protected internal byte[] payload;

		virtual public short Type
		{
			get
			{
				return pduType;
			}
			
		}
		virtual public byte[] Payload
		{
			get
			{
				return payload;
			}
			
			set
			{
				this.payload = value;
			}
			
		}

        /// <summary>
        /// Initializes a new instance of the <see cref="CWSPPDU"/> class.
        /// </summary>
		public CWSPPDU()
		{
		}

        /// <summary>
        /// Initializes a new instance of the <see cref="CWSPPDU"/> class.
        /// </summary>
        /// <param name="payload">The payload.</param>
		public CWSPPDU(byte[] payload)
		{
			this.payload = payload;
		}
				
		//////////////////////////////// ABSTRACT METHOD ////////////////////////////

        /// <summary>
        /// Encodes the PDU according to WAP-230-WSP-20010705-A.
        /// See <a href="http://www.wapforum.org">www.wapforum.org</a> for more information.
        /// </summary>
        /// <returns></returns>
		public abstract byte[] ToByteArray();
		
			//////////////////////////////// CAPABILITIES/HEADERS ///////////////////////
		public virtual CWSPCapabilities GetCapabilities()
		{
			return capabilities;
		}
		
		public virtual void  SetCapabilities(CWSPCapabilities c)
		{
			capabilities = c;
		}
		
		public virtual CWSPHeaders GetHeaders()
		{
			return headers;
		}
		
		public virtual void  SetHeaders(CWSPHeaders h)
		{
			headers = h;
		}
		
		public override string ToString()
		{
			return BitArrayInputStream.GetBitString(this.ToByteArray());
		}

        /// <summary>
        /// Equalses the specified pdu.
        /// </summary>
        /// <param name="pdu">The pdu.</param>
        /// <returns></returns>
		public bool Equals(CWSPPDU pdu)
		{
			byte[] a1 = this.ToByteArray();
			byte[] a2 = pdu.ToByteArray();
			
			if (a1.Length != a2.Length)
			{
				return false;
			}
			else
			{
				for (int i = 0; i < a1.Length; i++)
				{
					if (a1[i] != a2[i])
					{
						Logger.LogThis("Can not decode received PDU - Byte " + i, LogLevel.Error);
						
						return false;
					}
				}
				
				return true;
			}
		}
		
		/////////////////////////////////////////////////////////////////////////////////////////
		public static CWSPPDU GetPDU(byte[] bytes)
		{
			lock (typeof(CWSPPDU))
			{
				// This is a class, that helps us with decoding bits
				WSPDecoder bin = new WSPDecoder(bytes);
				
				//the PDU to be returned
				CWSPPDU end = null;
				
				//decode PDU type
				
				/** for connectionless mode: 1st byte is tid:
				short tid = bin.getUInt8(bytes[0]);
				short pduType = bin.getUInt8(bytes[1]);
				byte[] payload = null;
				int aktbyte = 2;
				*/
				short pduType = bin.UInt8;
				
				switch (pduType)
				{
					
					/** @todo ConfirmedPush PDU */
					case CWSPPDU.PduTypeConnect: 
						
						CWSPConnect pdu1 = new CWSPConnect();
						
						// decode Version
						pdu1.Version = bin.UInt8;
						
						// decode length of capabilities
						long caplength = bin.UIntVar;
						
						// decode length of headers
						long headlength = bin.UIntVar;
						
						// decode capabilities
						pdu1.SetCapabilities(GetCapabilities(bytes, bin.Seek(0), caplength));
						bin.Seek((int) caplength);
						
						// decode headers
						pdu1.SetHeaders(new CWSPHeaders(bin, (int) headlength, wapCodePage));
						end = pdu1;
						
						break;
					
					
					case CWSPPDU.PduTypeConnectReply: 
						
						// decode SessionID
						CWSPConnectReply pdu2 = new CWSPConnectReply(bin.UIntVar);
						
						// decode length of capabilities
						caplength = bin.UIntVar;
						
						// decode length of headers
						headlength = bin.UIntVar;
						
						// decode capabilities
						pdu2.SetCapabilities(GetCapabilities(bytes, bin.Seek(0), caplength));
						bin.Seek((int) caplength);
						
						// decode headers
						pdu2.SetHeaders(new CWSPHeaders(bin, (int) headlength, wapCodePage));
						end = pdu2;
						
						break;
					
					
					case CWSPPDU.PduTypeDataFragment: 
						
						// decode length of headers
						headlength = bin.UIntVar;
						
						// decode headers
						CWSPHeaders headers = new CWSPHeaders(bin, (int) headlength, wapCodePage);
						
						// the balance is payload
						byte[] payload = bin.GetBytes(bin.RemainingOctets);
						
						CWSPDataFragment pdu3 = new CWSPDataFragment(payload);
						pdu3.SetHeaders(headers);
						end = pdu3;
						
						break;
					
					
					case CWSPPDU.PduTypeDisconnect: 
						
						CWSPDisconnect pdu4 = new CWSPDisconnect(bin.UIntVar);
						end = pdu4;
						
						break;
					
					
					case CWSPPDU.PduTypeGet:  //5
						
						int urilength = (int) bin.UIntVar;
						string uri = bin.GetString(urilength);
						CWSPGet pdu5 = new CWSPGet(uri);
						pdu5.SetHeaders(new CWSPHeaders(bin, bin.RemainingOctets, wapCodePage));
						end = pdu5;
						
						break;
					
					
					case CWSPPDU.PduTypePost: 
						
						// decode length of URI
						urilength = (int) bin.UIntVar;
						
						// decode length of headers + contenttype
						headlength = bin.UIntVar;
						
						// decode URI
						uri = bin.GetString(urilength);
						
						// decode contenttype
						int cpos = bin.Seek(0);
						string contenttype = GetContentType(bin);
						headlength -= (bin.Seek(0) - cpos);
						
						// decode headers
						headers = new CWSPHeaders(bin, (int) headlength, wapCodePage);
						
						// the balance is payload
						payload = bin.GetBytes(bin.RemainingOctets);
						
						CWSPPost pdu6 = new CWSPPost(payload, contenttype, uri);
						pdu6.SetHeaders(headers);
						end = pdu6;
						
						break;
					
					
					case CWSPPDU.PduTypeRedirect: 
						
						CWSPRedirect pdu8 = new CWSPRedirect();
						
						// decode flags
						pdu8.SetFlags(bin.UInt8);
						
						// decode addresses
						pdu8.SetAddresses(GetAddresses(bytes, bin.Seek(0), bin.RemainingOctets));
						
						end = pdu8;
						
						break;
					
					
					case CWSPPDU.PduTypeReply: 
						
						// status
						long status = bin.UInt8;
						
						// length of header + contenttype
						headlength = bin.UIntVar;
						
						// decode contenttype
						cpos = bin.Seek(0);
						contenttype = GetContentType(bin);
						headlength -= (bin.Seek(0) - cpos);
						
						// decode headers
						headers = new CWSPHeaders(bin, (int) headlength, wapCodePage);
						
						// the balance is payload
						payload = bin.GetBytes(bin.RemainingOctets);
						
						CWSPReply pdu9 = new CWSPReply(status, payload, contenttype);
						pdu9.SetHeaders(headers);
						end = pdu9;
						
						break;
					
					
					case CWSPPDU.PduTypeResume: 
						
						// decode sessionID
						long sessionID = bin.UIntVar;
						
						// decode length of capabilities
						caplength = bin.UIntVar;
						
						// decode capabilities
						CWSPCapabilities capabilities = GetCapabilities(bytes, bin.Seek(0), caplength);
						bin.Seek((int) caplength);
						
						// decode headers
						headers = new CWSPHeaders(bin, bin.RemainingOctets, wapCodePage);
						
						CWSPResume pdu10 = new CWSPResume(sessionID);
						pdu10.SetCapabilities(capabilities);
						pdu10.SetHeaders(headers);
						end = pdu10;
						
						break;
					
					
					case CWSPPDU.PduTypeSuspend: 
						sessionID = bin.UIntVar;
						
						CWSPSuspend pdu11 = new CWSPSuspend(sessionID);
						end = pdu11;
						
						break;
					
					
					case CWSPPDU.PduTypePush: 
						
						CWSPPush pdu12 = new CWSPPush();
						headlength = bin.UIntVar;
						
						cpos = bin.Seek(0);
						contenttype = GetContentType(bin);
						headlength -= (bin.Seek(0) - cpos);
						
						Logger.LogThis("Push headers found. Len: " + headlength + "  content type:" + contenttype, LogLevel.Verbose);
						
						// decode headers
						pdu12.SetHeaders(new CWSPHeaders(bin, (int) headlength, wapCodePage));
						
						// decode payload
						pdu12.payload = bin.GetBytes(bin.RemainingOctets);
						end = pdu12;
						
						break;
					
					
					default: 
						
						// unnown PDU types
						throw new EWSPCorruptPDUException("Unknown PDU-Type! By the way: *is* it WSP? pduType=" + pduType);
					
				}
				
				// finally, set TID (for all types):
				//    end.TID=tid;
				//logger.debug(">>> decoded and re-encoded:");
				//logger.debug(end.toString());
				return end;
			}
		}
		
	    private static CWSPCapabilities GetCapabilities(byte[] bytes, int offset, long count)
		{
			lock (typeof(CWSPPDU))
			{
				CWSPCapabilities result = new CWSPCapabilities();
				
				if (count <= 0)
				{
					return result;
				}
				
				try
				{
					/** @todo decode capabilities */
				}
				catch (System.IndexOutOfRangeException e)
				{
					// ignore and return null
					return result;
				}
				
				return result;
			}
		}
		
	    private static ArrayList GetAddresses(byte[] bytes, int offset, long count)
		{
			lock (typeof(CWSPPDU))
			{
				// This is a class, that helps us with decoding bits
				BitArrayInputStream bin = new BitArrayInputStream();
				ArrayList result = ArrayList.Synchronized(new ArrayList(10));
				
				if (count <= 0)
				{
					return result;
				}
				
				int aktbyte = offset;
				
				while (aktbyte < (count + offset))
				{
					BitArray b = BitArrayInputStream.GetBitSet(bytes[aktbyte]);
					bool bearerTypeIncluded = b.Get(0);
					bool portNumberIncluded = b.Get(1);
					short addresslen = bin.GetByte(bytes[aktbyte], 2, 6);
					short bearerType = bin.GetUInt8(bytes[++aktbyte]);
					int portNumber = bin.GetUInt16(bytes[++aktbyte], bytes[++aktbyte]);
					string address = "";
					
					if (addresslen != 4)
					{
						Logger.LogThis("Unknown bearer type when decoding addresses!", LogLevel.Warn);
					}
					else
					{
						// the ipv4 address is encoded in 4 bytes
						// get each byte and concat the ipv4 address as a string 
						for (int i = 0; i < addresslen; i++)
						{
							short ip = bin.GetUInt8(bytes[++aktbyte]);
							address = address + ip;
							
							if (i < (addresslen - 1))
							{
								address = address + ".";
							}
						}
					}
					
					//String address = new String(bytes, ++aktbyte, addresslen);
					CWSPAddress a = new CWSPAddress(bearerTypeIncluded, portNumberIncluded, bearerType, portNumber, address);
					aktbyte += addresslen;
					result.Add(a);
				}
				
				return result;
			}
		}
		
		public static string GetContentType(WSPDecoder dc)
		{
			lock (typeof(CWSPPDU))
			{
				int hs = CWSPHeaders.GetHeaderValueSize(dc);
				byte[] header = dc.GetBytes(hs);
				return wapCodePage.DecodeContentType(header);
			}
		}

		static CWSPPDU()
		{			
			wapCodePage = WAPCodePage.GetInstance(1, 2);
		}
	}
}