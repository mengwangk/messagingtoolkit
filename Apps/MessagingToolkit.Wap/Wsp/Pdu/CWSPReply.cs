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
	
	public class CWSPReply:CWSPPDU
	{
		/// <summary> Table 36</summary>
		public const short _100Continue = (0x10);
		public const short _101SwitchingProtocols = (0x11);
		public const short _200OkSuccess = (0x20);
		public const short _201Created = (0x21);
		public const short _202Accepted = (0x22);
		public const short _203NonAuthoritativeInformation = (0x23);
		public const short _204NoContent = (0x24);
		public const short _205ResetContent = (0x25);
		public const short _206PartialContent = (0x26);
		public const short _300MultipleChoices = (0x30);
		public const short _301MovedPermanently = (0x31);
		public const short _302MovedTemporarily = (0x32);
		public const short _303SeeOther = (0x33);
		public const short _304NotModified = (0x34);
		public const short _305UseProxy = (0x35);
		public const short _307TemporaryRedirect = (0x37);
		public const short _400BadRequest = (0x40);
		public const short _401Unauthorized = (0x41);
		public const short _402PaymentRequired = (0x42);
		public const short _403Forbidden = (0x43);
		public const short _404NotFound = (0x44);
		public const short _405MethodNotAllowed = (0x45);
		public const short _406NotAcceptable = (0x46);
		public const short _407ProxyAuthenticationRequired = (0x47);
		public const short _408RequestTimeout = (0x48);
		public const short _409Conflict = (0x49);
		public const short _410Gone = (0x4A);
		public const short _411LengthRequired = (0x4B);
		public const short _412PreconditionFailed = (0x4C);
		public const short _413RequestEntityTooLarge = (0x4D);
		public const short _414RequestURITooLarge = (0x4E);
		public const short _415UnsupportedMediaType = (0x4F);
		public const short _416RequestedRangeNotSatisfiable = (0x50);
		public const short _417ExpectationFailed = (0x51);
		public const short _500InternalServerError = (0x60);
		public const short _501NotImplemented = (0x61);
		public const short _502BadGateway = (0x62);
		public const short _503ServiceUnavailable = (0x63);
		public const short _504GatewayTimeout = (0x64);
		public const short _505HTTPVersionNotSupported = (0x65);
		
		/// <summary> status
		/// uint8
		/// use constants in this class beginning with "_"
		/// </summary>
		private long status;
		
		/// <summary> ContentType
		/// mult. octets
		/// </summary>
		private string contentType;

        /// <summary>
        /// Initializes a new instance of the <see cref="CWSPReply"/> class.
        /// </summary>
        /// <param name="status">The status.</param>
        /// <param name="payload">The payload.</param>
        /// <param name="contentType">Type of the content.</param>
		public CWSPReply(long status, byte[] payload, string contentType)
		{
			this.pduType = CWSPPDU.PduTypeReply;
			this.status = status;
			this.payload = payload;
			this.contentType = contentType;
		}

        /// <summary>
        /// Encodes the PDU according to WAP-230-WSP-20010705-A.
        /// See <a href="http://www.wapforum.org">www.wapforum.org</a> for more information.
        /// </summary>
        /// <returns></returns>
		public override byte[] ToByteArray()
		{
			BitArrayOutputStream result = new BitArrayOutputStream();
			result.Write(pduType, 8);
			
			//--------------------------------//
			result.Write(status, 8);
			
			byte[] cap = capabilities.Bytes;
			byte[] head = headers.Bytes;
			byte[] ctype = ByteHelper.GetBytes(contentType);
			result.WriteUIntVar(head.Length + ctype.Length + 1);
			result.Write(ctype);
			result.Write(0x00, 8);
			result.Write(head);
			result.Write(payload);
			
			//--------------------------------//
			return result.ToByteArray();
		}

        /// <summary>
        /// Gets the status.
        /// </summary>
        /// <returns></returns>
		public virtual byte GetStatus()
		{
			BitArrayOutputStream m = new BitArrayOutputStream();
			m.Write(status, 8);
			
			byte[] b = m.ToByteArray();
			
			return b[0];
		}

        /// <summary>
        /// use constants in this class!
        /// </summary>
        /// <param name="status">The status.</param>
		public virtual void SetStatus(short status)
		{
			this.status = status;
		}

        /// <summary>
        /// Sets the type of the content.
        /// </summary>
        /// <param name="type">The type.</param>
		public virtual void  SetContentType(string type)
		{
			contentType = type;
		}

        /// <summary>
        /// Gets the type of the content.
        /// </summary>
        /// <returns></returns>
		public virtual string GetContentType()
		{
			return contentType;
		}

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
		public override string ToString()
		{
			BitArrayInputStream bin = new BitArrayInputStream();
			
			return "Status:      " + status + System.Environment.NewLine + "ContentType: " + contentType + System.Environment.NewLine + "encoded: " + System.Environment.NewLine + BitArrayInputStream.GetBitString(this.ToByteArray());
		}
	}
}