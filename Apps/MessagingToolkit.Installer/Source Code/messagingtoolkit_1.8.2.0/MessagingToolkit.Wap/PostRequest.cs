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

namespace MessagingToolkit.Wap
{
	
	/// <summary>
    /// Use this class for executing a WSP POST request
    /// </summary>
	public class PostRequest:Request
	{
        private byte[] requestBody;
        private string contentType;

        /// <summary>
        /// Return or set the request body for this WSP POST request
        /// </summary>
        /// <value>The request body.</value>
	    virtual public byte[] RequestBody
		{
			get
			{
				return requestBody;
			}
			
			set
			{
				this.requestBody = value;
			}			
		}

        /// <summary>
        /// Return or set the content-type
        /// </summary>
        /// <value>The type of the content.</value>
		virtual public string ContentType
		{
			get
			{
				return contentType;
			}
			
			set
			{
				this.contentType = value;
			}
			
		}
		/// <summary> Returns the length of the request-body</summary>
		virtual public long ContentLength
		{
			get
			{
				return requestBody == null?0:requestBody.Length;
			}			
		}

        /// <summary>
        /// Construct a new WSP POST request
        /// </summary>
        /// <param name="url">The URL.</param>
		public PostRequest(string url):base(url)
		{
		}
	}
}