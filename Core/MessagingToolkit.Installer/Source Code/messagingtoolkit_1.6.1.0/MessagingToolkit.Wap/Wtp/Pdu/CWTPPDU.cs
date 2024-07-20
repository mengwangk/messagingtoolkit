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
using System.Collections;

using MessagingToolkit.Wap.Wtp;
using MessagingToolkit.Wap.Log;
using MessagingToolkit.Wap.Helper;
using MessagingToolkit.Wap.Wsp;


namespace MessagingToolkit.Wap.Wtp.Pdu
{
	
	public abstract class CWTPPDU
	{
        // This is a class, that helps us with decoding bits
        private static BitArrayInputStream bin = new BitArrayInputStream();
        public const byte PduTypeInvoke = (0x01);
        public const byte PduTypeResult = (0x02);
        public const byte PduTypeAck = (0x03);
        public const byte PduTypeAbort = (0x04);
        public const byte PduTypeSegmInvoke = (0x05);
        public const byte PduTypeSegmResult = (0x06);
        public const byte PduTypeNegAck = (0x07);

        /// <summary>
        /// PDU types
        /// </summary>
        public static readonly string[] Types = new string[] {"", "PDU_TYPE_INVOKE", "PDU_TYPE_RESULT", "PDU_TYPE_ACK", "PDU_TYPE_ABORT", "PDU_Type_SEGM_INVOKE", "PDU_TYPE_SEGM_RESULT", "PDU_TYPE_NEG_ACK" };
		

		virtual public byte PDUType
		{
			get
			{
				return pduType;
			}
			
			set
			{
				this.pduType = value;
			}
			
		}
		virtual public byte[] Payload
		{
			get
			{
				return payload;
			}
			
			set
			{
				this.payload = value;
			}
			
		}
		virtual public ArrayList Segments
		{
			get
			{
				return segments;
			}
			
			set
			{
				this.segments = value;
			}
			
		}
	
        
		/////////////////////////////////////////////////////////////////////////////
		///////////////////////// COMMON HEADER FIELDS //////////////////////////////
		
		/// <summary> 
        /// 1 TPIs (Transport Information Item) in variable part
		/// 0 NO TPIs (Transport Information Item) in variable part <---
		/// 
		/// btw: TPIs:
		/// 0x00 Error
		/// 0x01 Info
		/// 0x02 Option
		/// 0x03 Packet Seqence Number (PSN) (N/A if segment. and re-ass. is implemented)
		/// 0x04 SDU Boundary (N/A if extended segment. and re-ass. is implemented)
		/// 0x05 Frame boundary (N/A if extended segment. and re-ass. is implemented)
		/// </summary>
		protected internal bool con; // 1 bit
		
		/// <summary> Group Trailer and Transmission Trailer Flag
		/// GTR/TTR
		/// 0   0  Not last packet
		/// 0   1  Last packet
		/// 1   0  Last packet of packet group
		/// 1   1  segmentation and reassambly NOT supported
		/// </summary>
		protected internal bool gtr; // 1 bit
		protected internal bool ttr; // 1 bit
		
		/// <todo>  Packet Sequence number </todo>
		/// <summary> The position of the packet in a segmented message
		/// NOT available if GTR/TTR = 11 <---
		/// </summary>
		
		// packSeqNo
		
		/// <summary> PDUType, 4 bit
		/// 0x01 Invoke
		/// 0x02 Result
		/// 0x03 Ack
		/// 0x04 Abort
		/// </summary>
		protected internal byte pduType;
		
		/// <summary> Reserved
		/// 0 <---
		/// </summary>
		protected internal bool res1;
		protected internal bool res2;
		
		/// <summary> re-transmission indicator (RID)
		/// 0 <---
		/// </summary>
		protected internal bool rid;
		
		/// <summary> 
        /// Transaction Identifier (TID)
		/// uint16
		/// </summary>
		protected internal int tid;
		protected internal byte[] payload;
		
		/// <summary> Maximum size of payload in bearer (UDP)</summary>
		protected internal int mtu = 800; //556 mid, empirisch 1408 oder 912?
		
		/// <summary> CWTPSegmInvokes (if payload > MTU)</summary>
		protected internal ArrayList segments = ArrayList.Synchronized(new ArrayList(10));
		
		
		//////////////////////////////// CONSTRUCTORS ///////////////////////////////
        /// <summary>
        /// Initializes a new instance of the <see cref="CWTPPDU"/> class.
        /// </summary>
        /// <param name="payload">The payload.</param>
        /// <param name="tid">The tid.</param>
        /// <param name="type">The type.</param>
		public CWTPPDU(byte[] payload, int tid, byte type):this(tid, type)
		{
			this.payload = payload;
		}

        /// <summary>
        /// Initializes a new instance of the <see cref="CWTPPDU"/> class.
        /// </summary>
        /// <param name="TID">The TID.</param>
        /// <param name="type">The type.</param>
		public CWTPPDU(int tid, byte type)
		{
			con = false;
			gtr = false; // when SAR is not supported by remote, we need to set this true! Niko
			ttr = true;
			res1 = false;
			res2 = false;
			rid = false;
			this.tid = tid;
			pduType = type;
			
			//logger.debug("PDU constructed: " + types[type]);
		}

        /// <summary>
        /// Decodes bytes in CWTPPDU objects
        /// </summary>
        /// <param name="bytes">The bytes.</param>
        /// <param name="length">the actual number of bytes that are used in
        /// the byte buffer.</param>
        /// <returns></returns>
		public static CWTPPDU Decode(byte[] bytes, int length)
		{
			//the PDU returned
			CWTPPDU end = null;
			
			//decode PDU type
			byte pduType = bin.GetByte(bytes[0], 1, 4);
			int tid = 0;
			byte[] payload = null;
			
			switch (pduType)
			{
				
				case CWTPPDU.PduTypeAbort: 
					tid = bin.GetShort(bytes[1], bytes[2], 0, 16);
					
					CWTPAbort abort = new CWTPAbort(tid);
					abort.SetCON(bin.GetBoolean(bytes[0], 0));
					abort.AbortType = bin.GetByte(bytes[0], 5, 3);
					abort.AbortReason = bin.GetByte(bytes[3], 0, 8);
					end = abort;
					
					break;
				
				
				case CWTPPDU.PduTypeAck: 
					tid = bin.GetShort(bytes[1], bytes[2], 0, 16);
					
					CWTPAck ack = new CWTPAck(tid);
					ack.TveTok = bin.GetBoolean(bytes[0], 5);
					ack.SetRES1(bin.GetBoolean(bytes[0], 6));
					ack.SetRID(bin.GetBoolean(bytes[0], 7));
					end = ack;
					
					break;
				
				
				case CWTPPDU.PduTypeInvoke: 
					tid = bin.GetShort(bytes[1], bytes[2], 0, 16);
					payload = new byte[length - 4];
					
					for (int i = 0; i < (length - 4); i++)
					{
						payload[i] = bytes[i + 4];
					}
					
					CWTPInvoke invoke = new CWTPInvoke(payload, tid, IWTPTransactionConstants.ClassType0);
					invoke.SetCON(bin.GetBoolean(bytes[0], 0));
					invoke.SetGTR(bin.GetBoolean(bytes[0], 5));
					invoke.SetTTR(bin.GetBoolean(bytes[0], 6));
					invoke.SetRID(bin.GetBoolean(bytes[0], 7));
					invoke.Version = bin.GetByte(bytes[3], 0, 2);
					invoke.SetTIDNew(bin.GetBoolean(bytes[3], 2));
					invoke.SetUpFlag(bin.GetBoolean(bytes[3], 3));
					invoke.SetRES1(bin.GetBoolean(bytes[3], 4));
					invoke.SetRES2(bin.GetBoolean(bytes[3], 5));
					invoke.SetTCL(bin.GetByte(bytes[3], 6, 2));
					end = invoke;
					
					break;
				
				
				case CWTPPDU.PduTypeResult: 
					tid = bin.GetShort(bytes[1], bytes[2], 0, 16);
					payload = new byte[length - 3];
					
					for (int i = 0; i < (length - 3); i++)
					{
						payload[i] = bytes[i + 3];
					}
					
					CWTPResult result = new CWTPResult(payload, tid);
					result.SetCON(bin.GetBoolean(bytes[0], 0));
					result.SetGTR(bin.GetBoolean(bytes[0], 5));
					result.SetTTR(bin.GetBoolean(bytes[0], 6));
					result.SetRID(bin.GetBoolean(bytes[0], 7));
					end = result;
					
					break;
				
				
				case CWTPPDU.PduTypeSegmResult: 
					tid = bin.GetShort(bytes[1], bytes[2], 0, 16);
					payload = new byte[length - 4];
					
					Array.Copy(bytes, 4, payload, 0, payload.Length);
					
					CWTPSegmResult res = new CWTPSegmResult((short) (0xff & bytes[3]), payload, tid);
					res.SetCON(bin.GetBoolean(bytes[0], 0));
					res.SetGTR(bin.GetBoolean(bytes[0], 5));
					res.SetTTR(bin.GetBoolean(bytes[0], 6));
					res.SetRID(bin.GetBoolean(bytes[0], 7));
					end = res;
					
					break;
				
                case CWTPPDU.PduTypeNegAck:
                    tid = bin.GetShort(bytes[1], bytes[2], 0, 16);

                    CWTPAck negAck = new CWTPAck(tid);
                    negAck.TveTok = bin.GetBoolean(bytes[0], 5);
                    negAck.SetRES1(bin.GetBoolean(bytes[0], 6));
                    negAck.SetRID(bin.GetBoolean(bytes[0], 7));
                    end = negAck;
                                      
                    break;
				
				default: 
					
					// unnown PDU types
					Logger.LogThis("Warning! Unknown PDU Type " + pduType, LogLevel.Verbose);
					throw new EWTPCorruptPDUException("Unknown PDU-Type! By the way: *is* it WTP?");
				
			}
			
			return end;
		}
		
		/////////////////////////////////////////////////////////////////////////////
		//////////////////////////////// ABSTRACT METHOD ////////////////////////////

        /// <summary>
        /// Encodes the PDU according to WAP-224-WTP-20010710-a
        /// </summary>
        /// <returns>encoded bytes</returns>
		public abstract byte[] ToByteArray();
		
		/////////////////////////////////////////////////////////////////////////////
		//////////////////////////////// SET/GET ////////////////////////////////////
		public virtual bool GetCON()
		{
			return con;
		}
		
		public virtual void  SetCON(bool CON)
		{
			this.con = CON;
		}
		
		public virtual bool GetGTR()
		{
			return gtr;
		}
		
		public virtual void  SetGTR(bool GTR)
		{
			this.gtr = GTR;
		}
		
		public virtual bool GetTTR()
		{
			return ttr;
		}
		
		public virtual void  SetTTR(bool TTR)
		{
			this.ttr = TTR;
		}
		
		public virtual bool GetRES1()
		{
			return res1;
		}
		
		public virtual void  SetRES1(bool RES)
		{
			this.res1 = RES;
		}
		
		public virtual bool GetRES2()
		{
			return res2;
		}
		
		public virtual void  SetRES2(bool RES)
		{
			this.res2 = RES;
		}
		
		public virtual bool GetRID()
		{
			return rid;
		}
		
		public virtual void  SetRID(bool RID)
		{
			this.rid = RID;
		}
		
		public virtual int GetTID()
		{
			return tid;
		}
		
	}
}