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
    /// This Class represents an Invoke PDU.
    /// According to the WTP specification in section 8 this PDU
    /// can be encoded into a byte array.
    /// The first 4 bytes of the PDU are used for the WTP Layer.
    /// After this the payload follows - e.g. the bytes of a upper Layer.
    /// To encode the PDU call ToByteArray().
    /// There are to ways of creation: <b>Either</b> you construct a Object
    /// manually by calling the constructor <b>or</b> you use CWTPFactory
    /// to decode a byte Array.
    /// </summary>
	public class CWTPInvoke:CWTPPDU
	{
		virtual public byte Version
		{
			get
			{
				return version;
			}
			
			set
			{
				this.version = value;
			}
			
		}
		/// <summary> 
        /// Transaction Class (TCL)
		/// 2 bit
		/// </summary>
		private byte tcl;
		
		/// <summary> 
        /// TIDnew Flag
		/// 1 bit
		/// </summary>
		private bool tidNew;
		
		/// <summary> 
        /// Version
		/// 2 bit
		/// </summary>
		private byte version;
		
		/// <summary> U/P flag
		/// do you want user acknowledgement?
		/// 1 bit
		/// </summary>
		private bool upFlag;

        /// <summary>
        /// Initializes a new instance of the <see cref="CWTPInvoke"/> class.
        /// </summary>
        /// <param name="payload">The Bytes belonging to the layer above</param>
        /// <param name="tid">The tid.</param>
        /// <param name="transactionclass">The transactionclass.</param>
		public CWTPInvoke(byte[] payload, int tid, byte transactionclass):base(payload, tid, PduTypeInvoke)
		{
			this.ttr = true;
			
			// do we have to segment the payload?
			if (payload.Length > mtu)
			{
				this.ttr = false;
				this.gtr = true;
				this.payload = new byte[mtu];
				
				for (int i = 0; i < mtu; i++)
				{
					this.payload[i] = payload[i];
				}
				
				// count the segments we need
				int countSegments = (int) (Math.Ceiling(((double) (payload.Length)) / mtu));
					
				
				// for each payload segment
				for (int i = 1; i < countSegments; i++)
				{
					//length of this payload segment
					int tmpLength = (payload.Length - (i * mtu));
					
					if (tmpLength > mtu)
					{
						tmpLength = mtu;
					}
					
					// read payload
					byte[] tmpPayload = new byte[tmpLength];
					
					for (int m = 0; m < tmpLength; m++)
					{
						tmpPayload[m] = payload[(i * mtu) + m];
					}
					
					//System.out.println("Segment no " + i + ": from byte " + (i*MTU)
					//                   + " till byte " + (i*MTU+tmpLength-1));
					CWTPSegmInvoke tmpSegment = new CWTPSegmInvoke((short) i, tmpPayload, this.tid);
					tmpSegment.SetGTR(true);
					tmpSegment.SetTTR(false);
					
					// is this the last segment?
					if (i == (countSegments - 1))
					{
						tmpSegment.SetTTR(true);
						tmpSegment.SetGTR(false);
					}
					
					// add Segment to vector
					segments.Add(tmpSegment);
				}
			}
			
			version = (byte) (0x00);
			tcl = transactionclass;
			upFlag = true;
			
			//if (TID == 0){
			//  TIDnew = true;
			
			/// <todo>  TIDnew auf true setzen, wenn neue TID wieder 0 * </todo>
			
			//}
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
			result.Write(version, 2);
			result.Write(tidNew);
			result.Write(upFlag);
			result.Write(res1);
			result.Write(res2);
			result.Write(tcl, 2);
			result.Write(payload);                   
			return result.ToByteArray();
		}
		
		
		//////////////////////////////// SET/GET ////////////////////////////////////
		public virtual byte GetTCL()
		{
			return tcl;
		}
		
		public virtual void  SetTCL(byte tcl)
		{
			this.tcl = tcl;
		}
		
		public virtual bool GetTIDNew()
		{
			return tidNew;
		}
		
		public virtual void  SetTIDNew(bool tidNew)
		{
			this.tidNew = tidNew;
		}
		
		public virtual bool GetUpFlag()
		{
			return upFlag;
		}
		
		public virtual void  SetUpFlag(bool up)
		{
			this.upFlag = up;
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
			result += ("CON:      " + con + Environment.NewLine + "pduType:  " + pduType + System.Environment.NewLine + "GTR:      " + gtr + System.Environment.NewLine + "TTR:      " + ttr + System.Environment.NewLine + "RID:      " + rid + System.Environment.NewLine + "TID:      " + tid + System.Environment.NewLine + "Version:  " + version + System.Environment.NewLine + "TIDnew:   " + tidNew + System.Environment.NewLine + "U/P:      " + upFlag + System.Environment.NewLine + "RES1:     " + res1 + System.Environment.NewLine + "RES2:     " + res2 + System.Environment.NewLine + "TCL:      " + tcl + System.Environment.NewLine + System.Environment.NewLine + "ENCODED:" + System.Environment.NewLine + BitArrayOutputStream.GetBitString(ToByteArray()));
			
			return result;
		}
	}
}