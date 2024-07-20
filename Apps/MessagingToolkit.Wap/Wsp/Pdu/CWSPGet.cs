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

    /// <summary>
    /// WSP Get PDU
    /// </summary>
	public class CWSPGet:CWSPPDU
	{
        /// <summary> mult. octets</summary>
        private string uri;

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
        /// Initializes a new instance of the <see cref="CWSPGet"/> class.
        /// </summary>
        /// <param name="uri">The URI.</param>
		public CWSPGet(string uri):base()
		{
			this.uri = uri;
			this.pduType = CWSPPDU.PduTypeGet;
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
			byte[] uria = ByteHelper.GetBytes(uri);
			result.WriteUIntVar(uria.Length);
			result.Write(uria);
			result.Write(headers.Bytes);
			
			//--------------------------------//
			return result.ToByteArray();
		}
	}
}