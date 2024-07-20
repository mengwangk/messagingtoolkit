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
using System.IO;

using MessagingToolkit.Core.Mobile;
using MessagingToolkit.Core.Mobile.PduLibrary;

namespace MessagingToolkit.Core.Mobile.Message
{
    /// <summary>
    /// Wappush message
    /// 
    /// Full documentation can be found at 
    /// http://www.openmobilealliance.org/tech/affiliates/wap/wap-167-serviceind-20010731-a.pdf?doc=wap-167-serviceind-20010731-a.pdf
    /// </summary>
    [global::System.Serializable]
    public class Wappush: Sms
    {
        #region =============================== Constants ====================================================================
        
        /// <summary>
        /// Ports for the WDP information element, instructing the handset which 
        /// application to load on receving the message
        /// </summary>
        private static byte[] WdpDestinationPort = new byte[] { 0x0b, 0x84 };
        
        private static byte[] WdpSourcePort = new byte[] { 0x23, 0xf0 };

        /// <summary>
        /// Service indication action
        /// </summary>
        private ServiceIndication serviceIndication;

        /// <summary>
        /// Source port for WAP push
        /// </summary>
        private const int WappushSourcePort = 9200;

        /// <summary>
        /// Destination port for WAP push
        /// </summary>
        private const int WappushDestinationPort = 2948;

        #endregion ==================================================================================

        
        #region =============================== Private Variables ===================================



        #endregion ==================================================================================


        #region =============================== Public Properties ===================================


        /// <summary>
        /// Location URL
        /// </summary>
        /// <value>URL to go to</value>
        public string Href
        {
            get
            {
                return serviceIndication.Href;
            }
            set
            {
                serviceIndication.Href = value;
            }
        }

        /// <summary>
        /// Creation date
        /// </summary>
        /// <value>Creation date</value>
        public DateTime CreateDate
        {
            get
            {
                return serviceIndication.CreatedAt;
            }
            set
            {
                serviceIndication.CreatedAt = value;
            }
        }

        /// <summary>
        /// Expiry date
        /// </summary>
        /// <value>Expiry date</value>
        public DateTime ExpireDate
        {
            get
            {
                return serviceIndication.ExpiresAt;
            }
            set
            {
                serviceIndication.ExpiresAt = value;
            }
        }

        /// <summary>
        /// Signal strength
        /// </summary>
        /// <value></value>
        public ServiceIndicationAction Signal
        {
            get
            {
                return serviceIndication.Action;
            }
            set
            {
                serviceIndication.Action = value;
            }
        }


        #endregion ==================================================================================


        #region =============================== Constructor ========================================= 

        /// <summary>
        /// </summary>
        /// <param name="recipient"></param>
        /// <param name="href"></param>
        /// <param name="text"></param>
		public Wappush(string recipient, string href, string text)
		{       
			this.serviceIndication = new ServiceIndication(href, text, ServiceIndicationAction.SignalHigh);
            this.Content = text;
            this.DataCodingScheme = MessageDataCodingScheme.EightBits;
            this.SourcePort = WappushSourcePort;
            this.DestinationPort = WappushDestinationPort;
            this.DestinationAddress = recipient;      
		}


        #endregion ==================================================================================  


        
        #region =============================== Internal Methods =====================================


        /// <summary>
        /// Get the WAP push PDU
        /// </summary>
        /// <returns></returns>
        internal override byte[] GetPdu()
        {
            /*
            string msgBody = string.Empty;
            foreach (byte b in GetPduBytes())
            {
                string s = b.ToString("X");
                if (s.Length < 2)
                {
                    msgBody += "0" + s;
                }
                else
                {
                    msgBody += s;
                }                
            }
            return msgBody; 
            */
            return GetPduBytes();
        }

        /// <summary>
        /// Generates the body of the SMS message
        /// </summary>
        /// <returns>byte array</returns>
        internal byte[] GetSmsBytes()
        {
            MemoryStream stream = new MemoryStream();
            byte[] wdpHeader = GetWdpHeaderBytes();
            stream.Write(wdpHeader, 0, wdpHeader.Length);

            byte[] pdu = GetPduBytes();
            stream.Write(pdu, 0, pdu.Length);

            return stream.ToArray();
        }

        /// <summary>
        /// Generates the PDU (Protocol Data Unit) comprising the encoded Service Indication
        /// and the WSP (Wireless Session Protocol) headers
        /// </summary>
        /// <returns>byte array comprising the PDU</returns>
        internal byte[] GetPduBytes()
        {
            byte[] body = serviceIndication.GetWbXmlBytes();

            byte[] headerBuffer = GetWspHeaderBytes((byte)body.Length);

            MemoryStream stream = new MemoryStream();
            stream.Write(headerBuffer, 0, headerBuffer.Length);
            stream.Write(body, 0, body.Length);

            return stream.ToArray();
        }

        /// <summary>
        /// Generates the WSP (Wireless Session Protocol) headers with the well known
        /// byte values specfic to a Service Indication
        /// </summary>
        /// <param name="contentLength">the length of the encoded Service Indication</param>
        /// <returns>byte array comprising the headers</returns>
        internal byte[] GetWspHeaderBytes(byte contentLength)
        {
            MemoryStream stream = new MemoryStream();

            stream.WriteByte(WirelessSessionProtocol.TransactionIdConnectionLess);
            stream.WriteByte(WirelessSessionProtocol.PduTypePush);

            MemoryStream headersStream = new MemoryStream();
            headersStream.Write(WirelessSessionProtocol.HeaderContentTypeApplicationVndWapSicUtf8,
                            0, WirelessSessionProtocol.HeaderContentTypeApplicationVndWapSicUtf8.Length);

            headersStream.WriteByte(WirelessSessionProtocol.HeaderApplicationType);
            headersStream.WriteByte(WirelessSessionProtocol.HeaderApplicationTypeXWapApplicationIdW2);

            //			headersStream.WriteByte(WSP.HEADER_CONTENTLENGTH);
            //			headersStream.WriteByte((byte)(contentLength + 128));
            //
            headersStream.Write(WirelessSessionProtocol.HeaderPushFlag, 0, WirelessSessionProtocol.HeaderPushFlag.Length);

            stream.WriteByte((byte)headersStream.Length);

            headersStream.WriteTo(stream);

            return stream.ToArray();
        }

        /// <summary>
        /// Generates the WDP (Wireless Datagram Protocol) or UDH (User Data Header) for the 
        /// SMS message. In the case comprising the Application Port information element
        /// indicating to the handset which application to start on receipt of the message
        /// </summary>
        /// <returns>byte array comprising the header</returns>
        internal byte[] GetWdpHeaderBytes()
        {
            MemoryStream stream = new MemoryStream();
            stream.WriteByte(WirelessDatagramProtocol.InformationElementIdentifierApplicationPort);
            stream.WriteByte((byte)(WdpDestinationPort.Length + WdpSourcePort.Length));
            stream.Write(WdpDestinationPort, 0, WdpDestinationPort.Length);
            stream.Write(WdpSourcePort, 0, WdpSourcePort.Length);

            MemoryStream headerStream = new MemoryStream();

            // write length of header
            headerStream.WriteByte((byte)stream.Length);

            stream.WriteTo(headerStream);
            return headerStream.ToArray();
        }


        #endregion ==================================================================================

        
        #region ============== Factory method   =====================================================


        /// <summary>
        /// Static factory to create the WAP push object
        /// </summary>
        /// <param name="recipient">Destination mobtel</param>
        /// <param name="hRef">Location URL</param>
        /// <param name="content">Content</param>
        /// <returns>A new instance of the SMS object</returns>
        public static Wappush NewInstance(string recipient, string hRef, string content)
        {
            return new Wappush(recipient, hRef, content);
         
        }

        #endregion ===================================================================================
    }
}
