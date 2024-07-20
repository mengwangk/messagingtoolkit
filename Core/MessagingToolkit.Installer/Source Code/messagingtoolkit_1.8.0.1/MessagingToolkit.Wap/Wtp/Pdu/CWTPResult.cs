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
    /// This Class represents an Result PDU.
    /// According to the WTP specification in section 8 this PDU
    /// can be encoded into a byte array.
    /// <br/><br/>
    /// The first 3 bytes of the PDU are used for the WTP Layer.
    /// After this the payload follows - e.g. the bytes of a upper Layer.
    /// To encode the PDU call toByteArray().
    /// <br/><br/>
    /// There are to ways of creation: <b>Either</b> you construct a Object
    /// manually by calling the constructor <b>or</b> you use CWTPFactory
    /// to decode a byte Array.
    /// </summary>
	public class CWTPResult:CWTPPDU
	{
        /// <summary>
        /// Initializes a new instance of the <see cref="CWTPResult"/> class.
        /// </summary>
        /// <param name="payload">The Bytes belonging to the layer above</param>
        /// <param name="TID">the Transaction ID according to the spec</param>
		public CWTPResult(byte[] payload, int tid):base(payload, tid, PduTypeResult)
		{
		}

        /// <summary>
        /// Encodes the PDU according to the WTP spec
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
			result.Write(payload);
			
			return result.ToByteArray();
		}
		
		
		//////////////////////////////// HELPERS ////////////////////////////////////

        /// <summary>
        /// constructs a string representation of the object
        /// invluding all fields.
        /// </summary>
        /// <returns>
        /// The constructed String with debug information
        /// </returns>
		public override string ToString()
		{
			string result = "";
			result += ("CON:      " + con + System.Environment.NewLine + "pduType:  " + pduType + System.Environment.NewLine + "GTR:      " + gtr + System.Environment.NewLine + "TTR:      " + ttr + System.Environment.NewLine + "RID:      " + rid + System.Environment.NewLine + "TID:      " + tid + System.Environment.NewLine + System.Environment.NewLine + "ENCODED:" + System.Environment.NewLine + BitArrayOutputStream.GetBitString(ToByteArray()));
			
			return result;
		}	
	
	}
}