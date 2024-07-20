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
using MessagingToolkit.Wap.Wsp.Pdu;


namespace MessagingToolkit.Wap.Wsp
{
	
	/// <summary> 
    /// Objects of this type represent a result from a POST or GET method.
	/// </summary>	
	public class CWSPResult
	{
        private CWSPHeaders headers;
        private CWSPMethodManager methodManager;
        private string contentType;
        private byte[] payload;
        private int status;

        /// <summary>
        /// Gets or sets the type of the content.
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
        /// <summary>
        /// Gets or sets the headers.
        /// </summary>
        /// <value>The headers.</value>
		virtual public CWSPHeaders Headers
		{
			get
			{
				return headers;
			}
			
			set
			{
				this.headers = value;
			}
			
		}
        /// <summary>
        /// Gets or sets the payload.
        /// </summary>
        /// <value>The payload.</value>
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
        /// Gets the method manager.
        /// </summary>
        /// <value>The method manager.</value>
		virtual public CWSPMethodManager MethodManager
		{
			get
			{
				return methodManager;
			}			
		}


        /// <summary>
        /// Initializes a new instance of the <see cref="CWSPResult"/> class.
        /// </summary>
		private CWSPResult()
		{
		}

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="methodManager">The method manager.</param>
        /// <param name="headers">The headers.</param>
        /// <param name="contentType">Type of the content.</param>
        /// <param name="payload">The payload.</param>
		public CWSPResult(CWSPMethodManager methodManager, CWSPHeaders headers, string contentType, byte[] payload):base()
		{
			this.headers = headers;
			this.contentType = contentType;
			this.payload = payload;
			this.methodManager = methodManager;
		}

        /// <summary>
        /// Gets the status.
        /// </summary>
        /// <returns></returns>
		public virtual int GetStatus()
		{
			return status;
		}

        /// <summary>
        /// Sets the status.
        /// </summary>
        /// <param name="status">The status.</param>
		internal virtual void  SetStatus(int status)
		{
			this.status = status;
		}
	}
}