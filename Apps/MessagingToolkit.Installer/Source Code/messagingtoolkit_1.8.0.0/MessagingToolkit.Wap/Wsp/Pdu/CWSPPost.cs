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
using MessagingToolkit.Wap.Wsp.Headers;

namespace MessagingToolkit.Wap.Wsp.Pdu
{
    /// <summary>
    /// WSP Post
    /// </summary>
	public class CWSPPost:CWSPPDU
	{
        /// <summary> mult. octets</summary>
        private string uri;

        /// <summary> mult. octets</summary>
        private string contentType;

        // Used for encoding the content-type of POST requests
        private static CodePage wapCodePage;

		virtual public string Uri
		{
			get
			{
				return uri;
			}
			
			set
			{
				this.uri = value;
			}			
		}

        /// <summary>
        /// Initializes a new instance of the <see cref="CWSPPost"/> class.
        /// </summary>
        /// <param name="payload">The payload.</param>
        /// <param name="contentType">Type of the content.</param>
        /// <param name="uri">The URI.</param>
		public CWSPPost(byte[] payload, string contentType, string uri):base()
		{
			this.uri = uri;
			this.payload = payload;
			this.pduType = CWSPPDU.PduTypePost;
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
			byte[] cap = capabilities.Bytes;
			byte[] head = headers.Bytes;
			
			// Encode the content-type and strip the first octet
			byte[] tmp = wapCodePage.Encode("Content-Type", contentType == null?"octet/unspecified":contentType);
			byte[] ctype = new byte[tmp.Length - 1];
			Array.Copy(tmp, 1, ctype, 0, ctype.Length);
			
			byte[] uria = ByteHelper.GetBytes(uri);
			result.WriteUIntVar(uria.Length);
			result.WriteUIntVar(head.Length + ctype.Length);
			result.Write(uria);
			result.Write(ctype);
			result.Write(head);
			result.Write(payload);
			
			//--------------------------------//
			return result.ToByteArray();
		}
		
		public virtual string GetContentType()
		{
			return contentType;
		}
		
		public virtual void  SetContentType(string contentType)
		{
			this.contentType = contentType;
		}

		static CWSPPost()
		{
			wapCodePage = WAPCodePage.GetInstance();
		}
	}
}