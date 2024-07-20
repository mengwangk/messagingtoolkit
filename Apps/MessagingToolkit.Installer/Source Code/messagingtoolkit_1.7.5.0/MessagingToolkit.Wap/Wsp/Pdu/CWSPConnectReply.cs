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
	
	public class CWSPConnectReply:CWSPPDU
	{
        /// <summary>
        /// Server sessionid
        /// </summary>
        private long serverSessionID;

        /// <summary>
        /// Gets or sets the server session ID.
        /// </summary>
        /// <value>The server session ID.</value>
		virtual public long ServerSessionID
		{
			get
			{
				return serverSessionID;
			}
			
			set
			{
				this.serverSessionID = value;
			}			
		}		
		
		public CWSPConnectReply(long serverSessionID):base()
		{
			this.serverSessionID = serverSessionID;
			pduType = CWSPPDU.PduTypeConnectReply;
		}
		
		public override byte[] ToByteArray()
		{
			BitArrayOutputStream result = new BitArrayOutputStream();
			result.Write(pduType, 8);
			result.WriteUIntVar(serverSessionID);
			
			byte[] cap = capabilities.Bytes;
			byte[] head = headers.Bytes;
			result.WriteUIntVar(cap.Length);
			result.WriteUIntVar(head.Length);
			result.Write(cap);
			result.Write(head);
			
			return result.ToByteArray();
		}
	}
}