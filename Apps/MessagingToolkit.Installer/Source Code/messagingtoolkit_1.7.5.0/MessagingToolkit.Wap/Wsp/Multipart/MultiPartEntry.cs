/// <summary> JWAP - A Java Implementation of the WAP Protocols
/// OSML - Open Source Messaging Library
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
using System.Net;
using System.Collections;
using System.IO;
using System.Text;

using MessagingToolkit.Wap.Wtp;
using MessagingToolkit.Wap.Log;
using MessagingToolkit.Wap.Helper;
using MessagingToolkit.Wap.Wsp;
using MessagingToolkit.Wap.Wsp.Headers;
using MessagingToolkit.Wap.Wsp.Pdu;

namespace MessagingToolkit.Wap.Wsp.Multipart
{

    /// <summary>
    /// This class represents a Multipart Entry according to WAP-230-WSP.
    /// </summary>
	public class MultiPartEntry
	{
        // Used for content-type encoding
        private static WAPCodePage wapCodePage;
        private string contentType;
        private byte[] data;
        private CWSPHeaders headers;

		virtual public IEnumerator HeaderNames
		{
			get
			{
				return headers.HeaderNames;
			}
			
		}
		virtual public string ContentType
		{
			get
			{
				return contentType;
			}
			
		}
		virtual public byte[] Payload
		{
			get
			{
				return data;
			}			
		}

		virtual public byte[] Bytes
		{
			get
			{
				MemoryStream outputStream = new MemoryStream();
				byte[] ct = wapCodePage.Encode("content-type", contentType);
				byte[] hd = headers.Bytes;
				
				byte[] byteArray;
				
                byteArray = Wsp.Headers.Encoding.UIntVar(ct.Length - 1 + hd.Length);
				outputStream.Write(byteArray, 0, byteArray.Length); // HeadersLen
				
                byteArray = Wsp.Headers.Encoding.UIntVar(data.Length);
                outputStream.Write(byteArray, 0, byteArray.Length); // DataLen
				
                outputStream.Write(ct, 1, ct.Length - 1); // Content-Type (without first octet)
				
				outputStream.Write(hd, 0, hd.Length);
				
				outputStream.Write(data, 0, data.Length); // data
				
				return outputStream.ToArray();
			}			
		}


        /// <summary>
        /// Initializes a new instance of the <see cref="MultiPartEntry"/> class.
        /// </summary>
		private MultiPartEntry()
		{
		}

        /// <summary>
        /// Construct a Multipart Entry.
        /// </summary>
        /// <param name="contentType">the content-type of the Multipart Entry</param>
        /// <param name="codePage">the codepage to use for header encoding or null to use
        /// the default (WAP) codepage</param>
        /// <param name="data">the payload</param>
		public MultiPartEntry(string contentType, CodePage codePage, byte[] data)
		{
			headers = (codePage == null)?new CWSPHeaders():new CWSPHeaders(codePage);
			this.contentType = contentType;
			this.data = data;
		}

        /// <summary>
        /// Construct a Multipart Entry using the WAP codepage for header encoding.
        /// </summary>
        /// <param name="contentType">the content-type of the Multipart Entry</param>
        /// <param name="data">the payload</param>
		public MultiPartEntry(string contentType, byte[] data):this(contentType, null, data)
		{
		}

        /// <summary>
        /// Add a header to the Multipart Entry.
        /// </summary>
        /// <param name="key">the header name</param>
        /// <param name="value">the header value</param>
		public virtual void AddHeader(string key, string value)
		{
			headers.AddHeader(key, value);
		}

        /// <summary>
        /// Gets the header.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
		public virtual string GetHeader(string key)
		{
			return headers.GetHeader(key);
		}

        /// <summary>
        /// Gets the headers.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
		public virtual IEnumerator GetHeaders(string key)
		{
			return headers.GetHeaders(key);
		}

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			sb.Append("MultipartEntry[content-type:").Append(contentType).Append("]\n");
			
			for (IEnumerator e = headers.HeaderNames; e.MoveNext(); )
			{
				string key = (string) e.Current;
				
				for (IEnumerator e2 = headers.GetHeaders(key); e2.MoveNext(); )
				{
					string val = (string) e2.Current;
					sb.Append("   ").Append(key).Append(':').Append(val).Append("\n");
				}
			}
			
			sb.Append(DebugUtils.HexDump("   Data: ", data));
			
			return sb.ToString();
		}

        /// <summary>
        /// Initializes the <see cref="MultiPartEntry"/> class.
        /// </summary>
		static MultiPartEntry()
		{
			wapCodePage = WAPCodePage.GetInstance();
		}
	}
}