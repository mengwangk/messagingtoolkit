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
using System.Net;
using System.Collections;
using System.IO;

using MessagingToolkit.Wap.Log;
using MessagingToolkit.Wap.Wtp;
using MessagingToolkit.Wap.Wsp;
using MessagingToolkit.Wap.Wsp.Pdu;
using MessagingToolkit.Wap.Helper;

namespace MessagingToolkit.Wap
{

    /// <summary>
    /// This class represents a Response on a WSP GET or POST request
    /// </summary>
    public class Response
	{
        /// <summary>
        /// Returns the HTTP status code
        /// </summary>
        /// <value>The status.</value>
		virtual public int Status
		{			
			get
			{
				return status;
			}
			
		}
        /// <summary>
        /// Returns the status-text corresponding to the response-status, e.g.
        /// for status-code 200 this method returns "OK".
        /// </summary>
        /// <value>The status text.</value>
		virtual public string StatusText
		{
			get
			{
				string ret = codes.Code2Str(Status);
				if (ret == null)
				{
					ret = "Unknown";
				}
				return ret;
			}
			
		}
        /// <summary>
        /// Determine if the response status signals success (HTTP 2xx).
        /// </summary>
        /// <value><c>true</c> if success; otherwise, <c>false</c>.</value>
		virtual public bool Success
		{
			get
			{
				return (status >= 200 && status < 300);
			}			
		}

        /// <summary>
        /// Gets the response body.
        /// </summary>
        /// <value>The response body.</value>
		virtual public byte[] ResponseBody
		{
			/*
			* Returns the response-body
			*/
			
			get
			{
				return responseBody;
			}
			
		}
        /// <summary>
        /// Returns the response-body as input stream
        /// </summary>
        /// <value>The response body as input stream.</value>
		virtual public Stream ResponseBodyAsInputStream
		{
			get
			{
				return new MemoryStream(responseBody == null?new byte[0]:responseBody);
			}
			
		}
        /// <summary>
        /// Get a list of all header names for this response.
        /// </summary>
        /// <value>The header names.</value>
        /// <returns> an enumeration of Strings
        /// </returns>
		virtual public IEnumerator HeaderNames
		{
			get
			{
				return headers == null? EmptyEnum:headers.HeaderNames;
			}
			
		}
        /// <summary>
        /// Get the response content-type
        /// </summary>
        /// <value>The type of the content.</value>
		virtual public string ContentType
		{
			get
			{
				return contentType;
			}
			
		}
        /// <summary>
        /// Get the response content-length
        /// </summary>
        /// <value>The length of the content.</value>
		virtual public long ContentLength
		{
			get
			{
				string cl = GetHeader("content-length");
				return cl == null?0:System.Int64.Parse(cl);
			}
			
		}

		private static readonly IEnumerator EmptyEnum = ArrayList.Synchronized(new ArrayList(10)).GetEnumerator();
		
		private int status;
		private byte[] responseBody;
		private CWSPHeaders headers;
		private string contentType;
		private static TransTable codes;

        /// <summary>
        /// Initializes a new instance of the <see cref="Response"/> class.
        /// </summary>
        /// <param name="result">The result.</param>
		internal Response(CWSPResult result)
		{
			status = result.GetStatus();
			responseBody = result.Payload;
			headers = result.Headers;
			contentType = result.ContentType;
		}

        /// <summary>
        /// Get the value of a response-header
        /// </summary>
        /// <param name="name">the header name</param>
        /// <returns>the header value or null if not present</returns>
		public virtual string GetHeader(string name)
		{
			return headers == null?null:headers.GetHeader(name);
		}

        /// <summary>
        /// Get a list of all values for a header
        /// </summary>
        /// <param name="header">the header name</param>
        /// <returns>an enumeration of strings</returns>
		public virtual IEnumerator GetHeaders(string header)
		{
			return headers == null?EmptyEnum:headers.GetHeaders(header);
		}

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
		public override string ToString()
		{
			StringBuilder buf = new StringBuilder();
			buf.Append("Status          : ").Append(Status).Append(" - ").Append(StatusText);
			buf.Append("\nContentType     : ").Append(ContentType);
			buf.Append("\nContentLength   : ").Append(ContentLength);
			buf.Append("\nHeaders         :");
			for (IEnumerator e = HeaderNames; e.MoveNext(); )
			{
				string key = (string) e.Current;
				buf.Append("\n   ").Append(key).Append(": ").Append(GetHeader(key));
			}
			return buf.ToString();
		}

        /// <summary>
        /// Initializes the <see cref="Response"/> class.
        /// </summary>
		static Response()
		{
			codes = TransTable.GetTable("http-status-codes");
		}
	}
}