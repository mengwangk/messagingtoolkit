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
using System.Collections;
using System.IO;

namespace MessagingToolkit.Pdu.WapPush
{
    /// <summary>
    /// WAP push service indication data generator
    /// </summary>
	public class WapSiUserDataGenerator
	{
        //===============================================    
        //  WAP SI User Data generation
        //===============================================        
        //  1 Visit GMail now! 2 now! now! now! 3 now! now! now! 4 now! now! now! 5 now! now! now! 6 now! now! now! 7 now! now! now! 8 now! now! now! 9 now! now! now!    
        //                                  dcs    udl udhl port            concat           wsp header                 si data        href                                  created               expires            signal      indicationText
        //  PDU1: 0051000C91360936310732 00 F5  FF 8B  0C   05 04 0B84 23F0 08 04 7A5B 02 01 29 06 06 03 AE 81 EA 8D CA 02056A00 45 C6 0E036D61696C2E676F6F676C6500850300                                             08    01 03 3120566973697420474D61696C206E6F77212032206E6F7721206E6F7721206E6F77212033206E6F7721206E6F7721206E6F77212034206E6F7721206E6F7721206E6F77212035206E 6F7721206E6F7721206E6F77212036206E6F
        //        0051000C91360936310732 00 F4  FF 8C  0B   05 04 0B84 23F0 00 03 A8   02 01 01 06 06 03 AE 81 EA 8D CA 02056A00 45 C6 0E036D61696C2E676F6F676C6500850300 0A C30720080523170534 10 C30720080524170534 08    01 03 3120566973697420474D61696C206E6F77212032206E6F7721206E6F7721206E6F77212033206E6F7721206E6F7721206E6F77212034206E6F7721206E6F7721206E6F77212035206E
        //  PDU2: 0051000C91360936310732 00 F5  FF 4F  0C   05 04 0B84 23F0 08 04 7A5B 02 02                                      7721206E6F7721206E6F77212037206E6F7721206E6F7721206E6F77212038206E6F7721206E6F7721206E6F77212039206E6F7721206E6F7721206E6F7721000101
        //        0051000C91360936310732 00 F4  FF 60  0B   05 04 0B84 23F0 00 03 A8   02 02 6F7721206E6F7721206E6F77212036206E6F 7721206E6F7721206E6F77212037206E6F7721206E6F7721206E6F77212038206E6F7721206E6F7721206E6F77212039206E6F7721206E6F7721206E6F7721000101
        private WapSiPdu pdu;

        private MemoryStream baos = new MemoryStream();

		public WapSiPdu WapSiPdu
		{
            get
            {
                return this.pdu;
            }
			set
			{
				this.pdu = value;
			}			
		}
				
		public byte[] GenerateWapSiUDBytes()
		{
			try
			{
				baos = new MemoryStream();
				// write wsp header
				WriteWspHeader();
				// write si data
				WriteWapSiData();
				return baos.ToArray();
			}
			catch (Exception e)
			{
                throw e;
			}
		}
		
		private void  WriteWspHeader()
		{
			// WSP header bytes
			// Push Identifier, this can be an arbitrary number??
			// maybe it should be part of the pdu        
			baos.WriteByte((byte) 0x01);
			// WSP PDU Type = Push
			baos.WriteByte((byte) 0x06);
			// NOTE: for now leave these hard coded
			//       ideally these should be editable from the WapSiPdu/OutboundWapSIMessage
			// Total header length
			baos.WriteByte((byte) 0x04);
			// Length of content type (3 bytes)
			baos.WriteByte((byte) 0x03);
			// value for content-type (0x2E = application/vnd.wap.sic)
			baos.WriteByte((byte) (0x2E | 0x80));
			// accept-charset header (0x01 = accept-charset)
			baos.WriteByte((byte) (0x01 | 0x80));
			// accept-charset value (0x6A = utf-8)
			baos.WriteByte((byte) (WapPushUtils.WbXmlCharsetUtf8 | 0x80));
		}
		
		private void  WriteWapSiData()
		{
			// SI bytes
			// Version of WBXML ( 0x02 = 1.2)
			baos.WriteByte((byte) WapPushUtils.WbXmlVersion12);
			// SI identifier ( 0x05 = SI 1.0)
			baos.WriteByte((byte) WapPushUtils.WbXmlSi10PublicIdentifier);
			// charset (0x6A = UTF-8)
			baos.WriteByte((byte) WapPushUtils.WbXmlCharsetUtf8);
			// Table string length (0)
			baos.WriteByte((byte) 0x00);
			// 0x45 = WBXML coding for tag <SI> with content only (No attributes)
			baos.WriteByte((byte) WapPushUtils.WbXmlSitagContentNoAttributes);
			// 0xC6 = WBXML coding for tag <indication> with content and attributes
			baos.WriteByte((byte) WapPushUtils.WbXMlIndicationTagContentAndAttributes);
			// href attribute
			WriteHrefAttribute(pdu.Url);
			// created attribute
			DateTimeOffset tempAux = pdu.CreateDate;
			WriteCreatedAttribute(ref tempAux);
			// expire attribure
			DateTimeOffset tempAux2 = pdu.ExpireDate;
			WriteExpiresAttribute(ref tempAux2);
			// action attribute
			WriteActionAttribute((int)pdu.WapSignal);
			// si-id attribute
			WriteSiIdAttribute(pdu.SiId);
			// class attribute
			// writeClassAttribute(baos);
			// WBXML coding for finishing <indication> open tag
			baos.WriteByte((byte) WapPushUtils.WbXmlCloseTag);
			// indication text
			WriteText(pdu.IndicationText);
			// WBXML coding for </si> closing tag
			baos.WriteByte((byte) WapPushUtils.WbXmlCloseTag);
			// WBXML coding for </indication> closing tag
			baos.WriteByte((byte) WapPushUtils.WbXmlCloseTag);
		}
		
		private void  WriteHrefAttribute(string url)
		{
			// write href attribute        
			if ((url == null) || (url.Trim().Equals("")))
			{
                throw new Exception("Invalid URL: '" + url + "'");
			}
			// scan the start of the href for the protocol bytes
			bool protocolFound = false;
			foreach (string protocol in WapPushUtils.GetProtocols())
			{
				if (url.StartsWith(protocol))
				{
					// write associated byte for this protocol
					baos.WriteByte(Convert.ToByte(WapPushUtils.GetProtocolByteFor(protocol)));
					protocolFound = true;
					url = url.Substring(protocol.Length);
					break;
				}
			}
			if (protocolFound == false)
			{
				// if no match use 0x0B (unknown)
				baos.WriteByte((byte) WapPushUtils.WbXmlHrefUnknown);
			}
			// write string start
			baos.WriteByte((byte) WapPushUtils.WbXMlStringStart);
			// move one and add character a time and lookahead for the domain strings
			for (int i = 0, lastPosition = 0; i < url.Length; i++)
			{
				foreach (string domain in WapPushUtils.GetDomains())
				{
					// if next characters match the domain
					if (i + domain.Length > url.Length)
					{
						// write remainder
						string currentPortion = url.Substring(lastPosition, (url.Length) - (lastPosition));
						byte[] tempSbyteArray = Encoding.GetEncoding("UTF-8").GetBytes(currentPortion);
						baos.Write(tempSbyteArray, 0, tempSbyteArray.Length);
						// make everything end
						i = i + domain.Length;
						break;
					}
					if (url.Substring(i, (i + domain.Length) - (i)).Equals(domain, StringComparison.OrdinalIgnoreCase))
					{
						// write current string portion of the url
						if (lastPosition < i)
						{
							string currentPortion = url.Substring(lastPosition, (i) - (lastPosition));
							byte[] tempSbyteArray = Encoding.GetEncoding("UTF-8").GetBytes(currentPortion);
                            baos.Write(tempSbyteArray, 0, tempSbyteArray.Length);
						}
						// write domain byte
						baos.WriteByte((byte) WapPushUtils.WbXMlStringEnd);
						baos.WriteByte(Convert.ToByte(WapPushUtils.GetDomainByteFor(domain)));
						baos.WriteByte((byte) WapPushUtils.WbXMlStringStart);
						// move index and lastPosition
						i = i + domain.Length;
						lastPosition = i;
						// skip to main loop
						break;
					}
				}
			}
			baos.WriteByte((byte) WapPushUtils.WbXMlStringEnd);
		}
		
		private void  WriteCreatedAttribute(ref DateTimeOffset createDate)
		{
			if (createDate != null)
			{
				// write created indicator
				// write created date info
				baos.WriteByte((byte) WapPushUtils.PushCreated);
				WriteDate(ref createDate);
			}
		}
	
        private void  WriteExpiresAttribute(ref DateTimeOffset expireDate)
		{
			if (expireDate != null)
			{
				// write expires indicator
				// write expires date info
				baos.WriteByte((byte) WapPushUtils.PushExpires);
				WriteDate(ref expireDate);
			}
		}
		
		private void  WriteSiIdAttribute(string siId)
		{
			if ((siId != null) && (siId.Trim().Equals("")))
			{
				baos.WriteByte((byte) WapPushUtils.PushSiId);
				WriteText(siId);
			}
		}
		
		//    private void writeClassAttribute(WapSiPdu pdu)
		//    {
		//        // what is this supposed to be?
		//        
		//    }
		private void  WriteActionAttribute(int wapSignal)
		{
			if (wapSignal != WapPushUtils.PushSignalMedium)
			{
				// only write if not medium since this is the default if nothing is there
				baos.WriteByte((byte) wapSignal);
			}
		}
		
		//===============================================    
		//  UTILITIES
		//===============================================    
		private void  WriteText(string text)
		{
			try
			{
				baos.WriteByte((byte) WapPushUtils.WbXMlStringStart);
				// this should depend on the value of the encoding in the WSP header
				// possible values: utf-8, utf-16, ??
				byte[] tempSbyteArray = Encoding.GetEncoding("UTF-8").GetBytes(text);
				baos.Write(tempSbyteArray, 0, tempSbyteArray.Length);
				baos.WriteByte((byte) WapPushUtils.WbXMlStringEnd);
			}
			catch (Exception e)
			{
                throw e;
			}
		}

        private void  WriteDate(ref DateTimeOffset date)
		{
			// sample 19990625152315 (7 octets) represents date "1999-06-25 15:23:15"
			// sample 20990603       (4 octets) represents date "2099-06-30 00:00:00"
			
            string dateData = date.ToString("yyyyMMddHHmmss");
			// scan from the last octet to start and remove all trailing 00s
			for (int i = 6; i >= 0; i--)
			{
				if (dateData.EndsWith("00"))
				{
					dateData = dateData.Substring(0, (i * 2) - (0));
				}
				else
				{
					break;
				}
			}
			// generate the byte[] from remaining date data
			byte[] dataBytes = PduUtils.PduToBytes(dateData);
			// mark opaque data
			baos.WriteByte((byte) WapPushUtils.WbXMlOpaqueData);
			// write octet length
			baos.WriteByte((byte) dataBytes.Length);
			// write data			
            baos.Write(dataBytes, 0, dataBytes.Length);
		}
	}
}