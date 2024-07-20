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
    /// WSP data fragment
    /// </summary>
	public class CWSPDataFragment:CWSPPDU
	{
		private CWSPHeaders headers = new CWSPHeaders();

        /// <summary>
        /// Initializes a new instance of the <see cref="CWSPDataFragment"/> class.
        /// </summary>
        /// <param name="payload">The payload.</param>
		public CWSPDataFragment(byte[] payload)
		{
			this.pduType = PduTypeDataFragment;
			this.payload = payload;
		}

        /// <summary>
        /// Gets the type of the content.
        /// </summary>
        /// <returns></returns>
		public virtual string GetContentType()
		{
			return null;
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
			byte[] head = headers.Bytes;
			result.WriteUIntVar(head.Length);
			result.Write(head);
			result.Write(payload);
			
			//--------------------------------//
			return result.ToByteArray();
		}

        /// <summary>
        /// Gets the headers.
        /// </summary>
        /// <returns></returns>
		public override CWSPHeaders GetHeaders()
		{
			return headers;
		}
	}
}