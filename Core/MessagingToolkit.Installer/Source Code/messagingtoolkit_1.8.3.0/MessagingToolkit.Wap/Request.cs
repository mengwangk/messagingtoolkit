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

using MessagingToolkit.Wap.Log;
using MessagingToolkit.Wap.Wtp;
using MessagingToolkit.Wap.Wsp;
using MessagingToolkit.Wap.Wsp.Pdu;

namespace MessagingToolkit.Wap
{

    /// <summary>
    /// Base class for WSP GET/Post requests.
    /// </summary>
	public abstract class Request
    {
        private string url;
        private CWSPHeaders headers;

        /// <summary>
        /// Gets the WSP headers.
        /// </summary>
        /// <value>The WSP headers.</value>
		virtual internal CWSPHeaders WSPHeaders
		{
			get
			{
				return headers;
			}			
		}

        /// <summary>
        /// Initializes a new instance of the <see cref="Request"/> class.
        /// </summary>
        /// <param name="url">The URL.</param>
		protected internal Request(string url)
		{
			this.url = url;
			headers = new CWSPHeaders();
		}

        /// <summary>
        /// Set a request header.
        /// </summary>
        /// <param name="name">the header name</param>
        /// <param name="headerValue">The header value.</param>
		public virtual void SetHeader(string name, string headerValue)
		{
			headers.SetHeader(name, headerValue);
		}

        /// <summary>
        /// Get the URL for this request
        /// </summary>
        /// <returns></returns>
		public virtual string GetURL()
		{
			return url;
		}
	}
}