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
	
	
	/// <summary> This Class represents an Acknowledgement PDU.
	/// According to the WTP specification in section 8 this PDU
	/// can be encoded into a byte array.
	/// <br><br>
	/// 3 bytes are used. You can not input payload of upper layers.
	/// To encode the PDU call toByteArray().
	/// <br><br>
	/// There are to ways of creation: <b>Either</b> you construct a Object
	/// manually by calling the constructor <b>or</b> you use CWTPFactory
	/// to decode a byte Array.
	/// </summary>
	public class CWTPAck:CWTPPDU
	{
		virtual public bool TveTok
		{
			/////////////////////////////////////////////////////////////////////////////
			//////////////////////////////// GET/SET ////////////////////////////////////
			
			get
			{
				return tvetok;
			}
			
			set
			{
				this.tvetok = value;
			}
			
		}
		/// <summary> Tve/Tok Flag
		/// responder -> initiator: "do you have outstanding transactions with this TID?"
		/// initiator -> resopnder: "I have outstanding transaction(s) with this TID!"
		/// </summary>
		private bool tvetok;
		
		/// <param name="TID">the Transaction ID according to the spec
		/// </param>
		public CWTPAck(int TID):base(TID, PduTypeAck)
		{
			tvetok = false;
		}
		
		/// <summary> encodes the PDU according to the WTP spec
		/// 
		/// </summary>
		/// <returns> encoded bytes
		/// </returns>
		public override byte[] ToByteArray()
		{
			BitArrayOutputStream result = new BitArrayOutputStream();
			result.Write(con);
			result.Write(pduType, 4);
			result.Write(tvetok);
			result.Write(res1);
			result.Write(rid);
			result.Write(tid, 16);
			
			if (payload != null)
			{
				result.Write(payload);
			}
			
			//    logger.debug(result.toString());
			return result.ToByteArray();
		}
		
		/////////////////////////////////////////////////////////////////////////////
		//////////////////////////////// HELPERS ////////////////////////////////////
		
		/// <summary> constructs a string representation of the object
		/// invluding all fields.
		/// 
		/// </summary>
		/// <returns> The constructed String with debug information
		/// </returns>
		public override string ToString()
		{
			string result = "";
			result += ("CON:      " + con + System.Environment.NewLine + "pduType:  " + pduType + System.Environment.NewLine + "tve_tok:  " + tvetok + System.Environment.NewLine + "RES1:     " + res1 + System.Environment.NewLine + "RID:      " + rid + System.Environment.NewLine + "TID:      " + tid + System.Environment.NewLine + System.Environment.NewLine + "ENCODED:" + System.Environment.NewLine + BitArrayOutputStream.GetBitString(ToByteArray()));
			
			return result;
		}
		
		/// <summary> Test method</summary>
		/// <param name="args">ignored
		/// </param>
		[STAThread]
		public static void  Main(string[] args)
		{
			CWTPAck a = new CWTPAck((short) 7576);
			a.ToByteArray();
			Console.Out.WriteLine(a.ToString());
		}
	}
}