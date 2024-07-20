//===============================================================================
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
using System.Collections;

namespace MessagingToolkit.Wap.Helper
{

    /// <summary>
    /// Bit array input stream utility
    /// </summary>
	public class BitArrayInputStream
	{
		//private const bool debug = false;
		
	    /// <summary>
        /// to avoid Math.pow(2,b) take pot[b]
        /// </summary>
		private static readonly long[] pot = new long[]{1L, 2L, 4L, 8L, 16L, 32L, 64L, 128L, 256L, 512L, 1024L, 2048L, 4096L, 8192L, 16384L, 32768L, 65536L, 131072L, 262144L, 524288L, 1048576L, 2097152L, 4194304L, 8388608L, 16777216L, 33554432L, 67108864L, 134217728L, 268435456L, 536870912L, 1073741824L, 2147483648L, 4294967296L, 8589934592L, 17179869184L, 34359738368L, 68719476736L, 137438953472L, 274877906944L, 549755813888L, 1099511627776L, 2199023255552L, 4398046511104L, 8796093022208L, 17592186044416L, 35184372088832L, 70368744177664L, 140737488355328L, 281474976710656L, 562949953421312L, 1125899906842624L, 2251799813685248L, 4503599627370496L, 9007199254740992L, 18014398509481984L, 36028797018963968L, 72057594037927936L, 144115188075855872L, 288230376151711744L, 576460752303423488L, 1152921504606846976L, 2305843009213693952L, 4611686018427387904L, 9223372036854775807L}; // b = 64

        /// <summary>
        /// Initializes a new instance of the <see cref="BitArrayInputStream"/> class.
        /// </summary>
		public BitArrayInputStream()
		{
		}
		
		/// <summary> returns if the given bit ist set in byte <code>b</code>
		/// 0 is the very left and 7 the very right bit in the byte
		/// </summary>
		public virtual bool GetBoolean(byte b, int bit)
		{
			BitArray bitset = GetBitSet(b);
			
			return bitset.Get(bit);
		}
		
		/// <summary> returns a byte value of <code>length</code> bits
		/// in byte <code>b</code>
		/// beginning with bit <code>offset</code>
		/// where 0 is the very left and 7 the very right bit in the byte
		/// </summary>
		public virtual byte GetByte(byte b, int offset, int length)
		{
			BitArray bitset = GetBitSet(b);
			byte result = 0;
			
			for (int i = 0; i < length; i++)
			{
				int bit = offset + (length - i - 1);
				
				if (bitset.Get(bit))
				{
					if (i == 8)
					{
						result = (byte) (result - 128);
					}
					else
					{
						result = (byte) (result + pot[i]);
					}
				}
			}
			
			return result;
		}
		
		/// <summary> returns a short value of <code>length</code> bits
		/// in byte <code>b</code>
		/// beginning with bit <code>offset</code>
		/// where 0 is the very left and 7 the very right bit in the byte
		/// </summary>
		public virtual short GetShort(byte b1, byte b2, int offset, int length)
		{
            /*
			if (debug)
			{
				Console.Out.WriteLine("b1: " + BitArrayInputStream.GetBitString(b1));
				Console.Out.WriteLine("b2: " + BitArrayInputStream.GetBitString(b2));
			}
            */
			
			BitArray bitset1 = GetBitSet(b1);
			BitArray bitset2 = GetBitSet(b2);
			short result = 0;
			int bit = length - 1;
			
			if ((offset < 0) || (length < 0))
			{
				throw new ArgumentException("offset < 0 or length < 0 not allowed");
			}
			
			if ((offset + length) > 16)
			{
				throw new ArgumentException("Length+length > 16 not allowed");
			}
			
			// wenn erstes Bit gesetzt und es werden 16 bits betrachtet
			if ((length == 16) && (offset == 0) && bitset1.Get(0))
			{
				result = (short) (result - 32768);
				length--;
				offset++;
				bit--;
			}
			
			// erstes Byte
			int offset1 = offset;
			int length1 = length;
			
			if ((length + offset) >= 7)
			{
				length1 = 8 - offset;
			}
			
            /*
			if (debug)
			{
				Console.Out.WriteLine("offset1: " + offset1);
				Console.Out.WriteLine("length1: " + length1);
			}
            */
			
			if ((length1 > 0) && (offset1 < 8) && (offset1 >= 0))
			{
				for (int i = 0; i < length1; i++)
				{
                    /*
					if (debug)
					{
						Console.Out.WriteLine("offset1 +i: " + (offset1 + i) + " |bit: " + bit);
					}
                    */
					
					if (bitset1.Get(offset1 + i))
					{
						result = (short) (result + pot[bit]);
					}
					
					bit--;
				}
			}
			
			// zweites Byte
			int length2 = (length + offset) - 8;
			int offset2 = offset - 8;
			
			if (offset2 < 0)
			{
				offset2 = 0;
			}
			
            /*
			if (debug)
			{
				Console.Out.WriteLine("offset2: " + offset2);
				Console.Out.WriteLine("length2: " + length2);
			}
            */
			
			if ((length2 > 0) && (offset2 < 8) && (offset2 >= 0))
			{
				for (int i = 0; i < length2; i++)
				{
                    /*
					if (debug)
					{
						Console.Out.WriteLine("offset2 +i: " + (offset2 + i) + " |bit: " + bit);
					}
                    */
					
					if (bitset2.Get(offset2 + i))
					{
						result = (short) (result + pot[bit]);
					}
					
					bit--;
				}
			}
			
			return result;
		}
		
		public virtual int GetInt(byte b1, byte b2, byte b3, byte b4, int offset, int length)
		{
            /*
			if (debug)
			{
				Console.Out.WriteLine("b1: " + BitArrayInputStream.GetBitString(b1));
				Console.Out.WriteLine("b2: " + BitArrayInputStream.GetBitString(b2));
				Console.Out.WriteLine("b3: " + BitArrayInputStream.GetBitString(b3));
				Console.Out.WriteLine("b4: " + BitArrayInputStream.GetBitString(b4));
			}
            */
			
			byte[] bytes = new byte[]{b1, b2, b3, b4};
			BitArray bitset = GetBitSet(bytes[0]);
			int result = 0;
			int bit = length - 1;
			
			if ((offset < 0) || (length < 0))
			{
				throw new ArgumentException("offset < 0 or length < 0 not allowed");
			}
			
			if ((offset + length) > 32)
			{
				throw new ArgumentException("Length+length > 32 not allowed");
			}
			
			// wenn erstes Bit gesetzt und es werden 32 bits betrachtet
			if ((length == 32) && (offset == 0) && bitset.Get(0))
			{
				result = - 2147483648;
				length--;
				offset++;
				bit--;
			}
			
			int offsetlocal = offset;
			int lengthlocal = length;
			
			// erstes Byte
			if ((length + offset) >= 7)
			{
				lengthlocal = 8 - offset;
			}
			
            /*
			if (debug)
			{
				Console.Out.WriteLine("offsetlocal: " + offsetlocal);
				Console.Out.WriteLine("lengthlocal: " + lengthlocal);
			}
            */
			
			if ((lengthlocal > 0) && (offsetlocal < 8) && (offsetlocal >= 0))
			{
				for (int i = 0; i < lengthlocal; i++)
				{
                    /*
					if (debug)
					{
						Console.Out.WriteLine("offsetlocal +i: " + (offset + i) + " |bit: " + bit);
					}
                    */
					
					if (bitset.Get(offsetlocal + i))
					{
						result = (int) (result + pot[bit]);
					}
					
					bit--;
				}
			}
			
			// zweites bis viertes Byte
			for (int m = 1; m < 4; m++)
			{
				bitset = GetBitSet(bytes[m]);
				lengthlocal = (length + offset) - (8 * m);
				offsetlocal = offset - (8 * m);
				
				if (offsetlocal < 0)
				{
					offsetlocal = 0;
				}
				
				if (lengthlocal > 8)
				{
					lengthlocal = 8;
				}
				
                /*
				if (debug)
				{
					Console.Out.WriteLine("offsetlocal: " + offsetlocal);
					Console.Out.WriteLine("lengthlocal: " + lengthlocal);
				}
                */
				
				if ((lengthlocal > 0) && (offsetlocal < 8) && (offsetlocal >= 0))
				{
					for (int i = 0; i < lengthlocal; i++)
					{
                        /*
						if (debug)
						{
							Console.Out.WriteLine("offsetlocal +i: " + (offsetlocal + i) + " |bit: " + bit);
						}
                        */
						
						if (bitset.Get(offsetlocal + i))
						{
							result = (int) (result + pot[bit]);
						}
						
						bit--;
					}
				}
			}
			
			return result;
		}
		
		public virtual long GetUIntVar(byte[] bytes, int offset)
		{
			int lastone = (offset + GetLengthOfUIntVar(bytes, offset)) - 1;
			
            /*
			if (debug)
			{
				Console.Out.WriteLine("Last one: " + lastone);
			}
            */
			
			int exp = 0; // pot[exp]
			long result = 0;
			
			for (int aktbyte = lastone; aktbyte >= offset; aktbyte--)
			{
				/*
				if (debug)
				{
					Console.Out.WriteLine("byte: " + aktbyte);
				}
                */
				
				BitArray setValue = GetBitSet(bytes[aktbyte]);
				
				for (int i = 7; i > 0; i--)
				{
                    /*
					if (debug)
					{
						Console.Out.WriteLine("Bit: " + i);
					}
                    */
					
					if (setValue.Get(i))
					{
						result += pot[exp];
					}
					
					exp++;
				}
			}
			
			return result;
		}
		
		public virtual short GetUInt8(byte b, int offset, int length)
		{
			return GetShort((byte) 0, b, 8 + offset, length);
		}
		
		public virtual short GetUInt8(byte b)
		{
			return GetUInt8(b, 0, 8);
		}
		
		public virtual int GetUInt16(byte b1, byte b2, int offset, int length)
		{
			return GetInt((byte) 0, (byte) 0, b1, b2, 16, length);
		}
		
		public virtual int GetUInt16(byte b1, byte b2)
		{
			return GetUInt16(b1, b2, 16, 16);
		}
		
		////////////////////////////HELPERS//////////////////////////
		public virtual int GetLengthOfUIntVar(byte[] bytes, int offset)
		{
			for (int i = offset; i < bytes.Length; i++)
			{
				if (!(GetBitSet(bytes[i]).Get(0)))
				{
					return i - offset + 1;
				}
			}
			
			return 0;
		}
		
		/// <summary>  Constructs a binary representation of the byte <code>m</code></summary>
		public static string GetBitString(byte b)
		{
			string result = "";

            sbyte m = unchecked((sbyte)b);

			if (m < 0)
			{
				result += "1";
				m = unchecked((sbyte) (m + 128));
				
				// if m >= 0
			}
			else
			{
				result += "0";
			}
			
			for (int i = 6; i >= 0; i--)
			{
				if (m >= pot[i])
				{
					m = unchecked((sbyte) (m - pot[i]));
					result += "1";
				}
				else
				{
					result += "0";
				}
			}
			
			return result;
		}
		
		/// <summary>  Constructs a binary representation of the byte-array <code>m</code></summary>
		public static string GetBitString(byte[] m)
		{
			string result = "";
			
			for (int i = 0; i < m.Length; i++)
			{
				result += ("" + i + ": " + GetBitString(m[i]) + System.Environment.NewLine);
			}
			
			return result;
		}
		
		/// <summary>  Constructs a BitSet of the byte <code>m</code></summary>
		public static BitArray GetBitSet(byte b)
		{
			BitArray result = new BitArray((8 % 64 == 0?8 / 64:8 / 64 + 1) * 64);

            sbyte m = unchecked((sbyte)b);

			if (m < 0)
			{
				BitArrayHelper.Set(result, 0);
				m = unchecked ((sbyte) (m + 128));
			}
			
			for (int i = 6; i >= 0; i--)
			{
				if (m >= pot[i])
				{
					m = unchecked((sbyte) (m - pot[i]));
					BitArrayHelper.Set(result, 7 - i);
				}
			}
			
			return result;
		}
		
	
	}
}