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
    /// This Class represents an Abort PDU.
    /// According to the WTP specification in section 8 this PDU
    /// can be encoded into a byte array.
    /// <br/><br/>
    /// 4 bytes are used. You can not input payload of upper layers.
    /// To encode the PDU call toByteArray().
    /// <br/><br/>
    /// There are to ways of creation: <b>Either</b> you construct a Object
    /// manually by calling the constructor <b>or</b> you use CWTPFactory
    /// to decode a byte Array.
    /// </summary>
	public class CWTPAbort:CWTPPDU
	{
        /// <summary>
        /// Gets or sets the type of the abort.
        /// </summary>
        /// <value>The type of the abort.</value>
		virtual public long AbortType
		{			
			//////////////////////////////// SET/GET ////////////////////////////////////
			
			get
			{
				return abortType;
			}
			
			set
			{
				this.abortType = value;
			}			
		}

        /// <summary>
        /// Gets or sets the abort reason.
        /// </summary>
        /// <value>The abort reason.</value>
		virtual public short AbortReason
		{
			get
			{
				return (short) abortReason;
			}
			
			set
			{
				this.abortReason = value;
			}			
		}

		public const short AbortTypeProvider = (0x00);
		public const short AbortTypeUser = (0x01);
		public const short AbortReasonUnknown = (0x00);
		public const short AbortReasonProtoErr = (0x01);
		public const short AbortReasonInvalidTId = (0x02);
		public const short AbortReasonNotImplementedCl2 = (0x03);
		public const short AbortReasonNotImplementedSar = (0x04);
		public const short AbortReasonNotImplementedUack = (0x05);
		public const short AbortReasonWtpVersionNone = (0x07);
		public const short AbortReasonCpTempExceeded = (0x07);
		public const short AbortReasonNoResponse = (0x08);
		public const short AbortReasonMessageTooLarge = (0x09);
		public const short AbotReasonNotImplementedEsar = (0x0A);
		
		/// <summary> 
        /// Abort Type
		/// use static constants in CWTPAbort
		/// </summary>
		private long abortType;
		
		/// <summary> 
        /// Abort Reason
		/// use static constants in CWTPAbort
		/// </summary>
		private long abortReason;

        /// <summary>
        /// Defaults:<br/>
        /// abortType = ABORT_TYPE_USER;<br/>
        /// abortReason = ABORT_REASON_UNKNOWN;
        /// </summary>
        /// <param name="TID">the Transaction ID according to the spec.</param>
		public CWTPAbort(int TID):base(TID, PduTypeAbort)
		{
			abortType = AbortTypeUser;
			abortReason = AbortReasonUnknown;
		}

        /// <summary>
        /// encodes the PDU according to the WTP spec
        /// </summary>
        /// <returns>encoded bytes</returns>
		public override byte[] ToByteArray()
		{
			BitArrayOutputStream result = new BitArrayOutputStream();
			result.Write(con);
			result.Write(pduType, 4);
			result.Write(abortType, 3);
			result.Write(tid, 16);
			result.Write(abortReason, 8);
			
			//    logger.debug(result.toString());
			return result.ToByteArray();
		}
		
		/////////////////////////////////////////////////////////////////////////////
		//////////////////////////////// HELPERS ////////////////////////////////////

        /// <summary>
        /// constructs a string representation of the object
        /// including all fields.
        /// </summary>
        /// <returns>
        /// The constructed String with debug information
        /// </returns>
		public override string ToString()
		{
			string result = "";
			result += ("CON:         " + con + Environment.NewLine + "PDUType:     " + pduType + Environment.NewLine + "AbortType:   " + abortType + Environment.NewLine + "AbortReason: " + abortReason + System.Environment.NewLine + "TID:         " + tid + System.Environment.NewLine + System.Environment.NewLine + "ENCODED:" + System.Environment.NewLine + BitArrayOutputStream.GetBitString(ToByteArray()));
			
			return result;
		}		
		
	}
}