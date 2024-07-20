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
    /// WSP disconnect
    /// </summary>
	public class CWSPDisconnect:CWSPPDU
	{
        /// <summary> uintvar</summary>
        private long serverSessionID;

        /// <summary>
        /// Gets the server session ID.
        /// </summary>
        /// <value>The server session ID.</value>
		virtual public long ServerSessionID
		{
			get
			{
				return serverSessionID;
			}			
		}

        /// <summary>
        /// Initializes a new instance of the <see cref="CWSPDisconnect"/> class.
        /// </summary>
        /// <param name="serverSessionID">The server session ID.</param>
		public CWSPDisconnect(long serverSessionID):base()
		{
			this.serverSessionID = serverSessionID;
			pduType = CWSPPDU.PduTypeDisconnect;
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
			result.WriteUIntVar(serverSessionID);
			
			//--------------------------------//
			return result.ToByteArray();
		}
	}
}