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
using System.Text;
using System.IO;
using System.Collections;
using System.Collections.Generic;

namespace MessagingToolkit.MMS
{
	
	/// <summary>
    ///  The MultimediaMessageResponse class provides methods to get the response returned by the MMSC.
	/// </summary>
    public class MultimediaMessageResponse
	{
        //private Hashtable hHeaders;
        private Dictionary<string,string> hHeaders;
        private int responseCode;
        private string responseMessage;
        private string contentType;
        private int contentLength;
        private byte[] buffer;

        /// <summary>
        /// Initializes a new instance of the <see cref="MultimediaMessageResponse"/> class.
        /// </summary>
        public MultimediaMessageResponse()
        {
            //hHeaders = Hashtable.Synchronized(new Hashtable());
            hHeaders = new Dictionary<string, string>();
        }
		

        /// <summary>
        /// Returns an all the field (key) contained in the response header
        /// </summary>
        /// <value>The headers list.</value>
		virtual public IEnumerator HeadersList
		{
			get
			{
				return hHeaders.Keys.GetEnumerator();
			}			
		}		
		
		protected internal virtual void  SetResponseCode(int responseCode)
		{
			this.responseCode = responseCode;
		}
		
		protected internal virtual void  SetResponseMessage(string responseMessage)
		{
			this.responseMessage = responseMessage;
		}
		
		protected internal virtual void  AddHeader(string hKey, string hValue)
		{
			hHeaders[hKey.ToUpper()] = hValue;
		}

        /// <summary>
        /// Gets the response code returned by the MMSC
        /// </summary>
        /// <returns></returns>
		public virtual int GetResponseCode()
		{
			return responseCode;
		}

        /// <summary>
        /// Gets the response message returned by the MMSC
        /// </summary>
        /// <returns></returns>
		public virtual string GetResponseMessage()
		{
			return responseMessage;
		}

        /// <summary>
        /// This method returns an header value contained in the response returned by the MMSC
        /// param key the name of the header field
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
		public virtual string GetHeaderValue(string key)
		{
			return (string) hHeaders[(string) key];
		}
		
		protected internal virtual void  SetContentType(string ct)
		{
			contentType = ct;
		}

        /// <summary>
        /// This method returns the content type of the response
        /// </summary>
        /// <returns></returns>
		public virtual string GetContentType()
		{
			return contentType;
		}
		
		protected internal virtual void  SetContentLength(int i)
		{
			contentLength = i;
		}

        /// <summary>
        /// This method returns the content length of the response
        /// </summary>
        /// <returns></returns>
		public virtual int GetContentLength()
		{
			return contentLength;
		}

        /// <summary>
        /// Sets the content.
        /// </summary>
        /// <param name="buf">The buf.</param>
		protected internal virtual void  SetContent(byte[] buf)
		{
			buffer = buf;
		}

        /// <summary>
        /// This method returns the content of the response
        /// </summary>
        /// <returns></returns>
		public virtual byte[] GetContent()
		{
			return buffer;
		}
	}
}