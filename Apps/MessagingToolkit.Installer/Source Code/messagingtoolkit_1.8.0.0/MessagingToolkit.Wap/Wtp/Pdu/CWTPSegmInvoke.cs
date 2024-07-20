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


namespace MessagingToolkit.Wap.Wtp.Pdu
{

    /// <summary>
    /// Segment invoke
    /// </summary>
	public class CWTPSegmInvoke:CWTPPDU
	{
		private short sequenceNo;

        /// <summary>
        /// Initializes a new instance of the <see cref="CWTPSegmInvoke"/> class.
        /// </summary>
        /// <param name="sequenceNo">The sequence no.</param>
        /// <param name="payload">The payload.</param>
        /// <param name="tid">The tid.</param>
		public CWTPSegmInvoke(short sequenceNo, byte[] payload, int tid):base(payload, tid, PduTypeSegmInvoke)
		{
			this.sequenceNo = sequenceNo;
		}

        /// <summary>
        /// Encodes the PDU according to WAP-224-WTP-20010710-a
        /// </summary>
        /// <returns>encoded bytes</returns>
		public override byte[] ToByteArray()
		{
			BitArrayOutputStream result = new BitArrayOutputStream();
			result.Write(con);
			result.Write(pduType, 4);
			result.Write(gtr);
			result.Write(ttr);
			result.Write(rid);
			result.Write(tid, 16);
			result.Write(sequenceNo, 8);
			result.Write(payload);
			
			//    logger.debug(result.toString());
			return result.ToByteArray();
		}	
	}
}