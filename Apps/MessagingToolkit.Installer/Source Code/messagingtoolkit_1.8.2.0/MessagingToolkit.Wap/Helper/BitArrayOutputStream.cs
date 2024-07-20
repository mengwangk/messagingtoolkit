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
using System.IO;
using System.Collections;
using System.Net;

namespace MessagingToolkit.Wap.Helper
{

    /// <summary>
    /// Bit array output stream utility class
    /// </summary>
	public class BitArrayOutputStream
	{
		public const bool debug = false;
	
        /// <summary>
        /// to avoid Math.pow(2,b) take pot[b]
        /// </summary>
		private static readonly long[] pot = new long[]{1L, 2L, 4L, 8L, 16L, 32L, 64L, 128L, 256L, 512L, 
            1024L, 2048L, 4096L, 8192L, 16384L, 32768L, 65536L, 131072L, 262144L, 524288L, 1048576L, 2097152L, 
            4194304L, 8388608L, 16777216L, 33554432L, 67108864L, 134217728L, 268435456L, 536870912L, 1073741824L, 
            2147483648L, 4294967296L, 8589934592L, 17179869184L, 34359738368L, 68719476736L, 137438953472L, 
            274877906944L, 549755813888L, 1099511627776L, 2199023255552L, 4398046511104L, 8796093022208L, 
            17592186044416L, 35184372088832L, 70368744177664L, 140737488355328L, 281474976710656L, 
            562949953421312L, 1125899906842624L, 2251799813685248L, 4503599627370496L, 9007199254740992L, 
            18014398509481984L, 36028797018963968L, 72057594037927936L, 144115188075855872L, 288230376151711744L, 
            576460752303423488L, 1152921504606846976L, 2305843009213693952L, 4611686018427387904L, 9223372036854775807L}; 
        // b = 64
		internal MemoryStream outstream = new MemoryStream();
		internal int aktBit = 0;
		internal int sByte = 0;
		
		
        //////////////////// METHODS CONCERNING THE STREAM /////////////////////////

        /// <summary>
        /// fills up the aktual byte with bits "0" for fill = false
        /// and "1" for fill = true
        /// </summary>
        /// <param name="fill">if set to <c>true</c> [fill].</param>
		public virtual void  Flush(bool fill)
		{
			lock (this)
			{
				while (aktBit != 0)
				{
					Write(fill);
				}
			}
		}
		
		public virtual byte[] ToByteArray()
		{
			return outstream.ToArray();
		}			

		//////////////////// WRITE SINGLE BITS /////////////////////////////////////
		
		/// <summary> writes a bit down to the stream.
		/// is used by all the other methods in this class.
		/// </summary>
		public virtual void  Write(bool b)
		{
			lock (this)
			{
				if (b)
				{
					sByte |= (1 << (7 - aktBit));                   
				}
				aktBit++;
				
				// if byte is full, write down into outstream
				if (aktBit == 8)
				{ 
                    outstream.WriteByte((byte)sByte);
                    aktBit = sByte = 0;                                  
                    
				}
			}
		}
		
		////////////////////////////////////////////////////////////////////////////

		//////////////////// WRITE METHODS /////////////////////////////////////////

        /// <summary>
        /// writes <code>count</code> bits from byte <code>_b</code> beginning on the right hand.
        /// <code>Count</code> has NOT to be in byte-size (e.g. 8, 16, 24, 32,...)
        /// </summary>
        /// <param name="byteValue">The byteValue</param>
        /// <param name="count">The count.</param>
		public virtual void  Write(byte byteValue, int count)
		{            
			lock (this)
			{                
                sbyte sb = unchecked((sbyte)byteValue);
                int b = sb;
				
				// bit #0 (encoding positive/negative numbers)
				if (b < 0)               
				{
					if (count == 8)
					{
						Write(true);
						count = 7;
					}
					
					b += 128;
					
					// if _b >= 0
				}
				else
				{
					if (count == 8)
					{
						Write(false);
						count = 7;
					}
				}
				
				for (int i = 7; i >= count; i--)
				{
					if (b >= pot[i])
					{
						b = (int) (b - pot[i]);
					}
				}
				
				// bits #1 - #7
				for (int i = count - 1; i >= 0; i--)
				{
					if (b >= pot[i])
					{
						b = (int) (b - pot[i]);
						Write(true);
					}
					else
					{
						Write(false);
					}
				}
			}           
		}


        /// <summary>
        /// Resets this instance.
        /// </summary>
		public virtual void  Reset()
		{
			lock (this)
			{
				//outstream.reset();
                outstream.Close();
                outstream = null;
                outstream = new MemoryStream();               
                sByte = aktBit = 0;
			}
		}

        /// <summary>
        /// Writes the specified b.
        /// </summary>
        /// <param name="byteValue">The byte value.</param>
        /// <param name="count">The count.</param>
		public virtual void  Write(int byteValue, int count)
		{
			lock (this)
			{             
				// bit #0 (encoding positive/negative numbers)
				if (byteValue < 0)
				{
					if (count == 16)
					{
						Write(true);
						count = 15;
					}
					
					byteValue += 32768;
					
					// if b >= 0
				}
				else
				{
					if (count == 16)
					{
						Write(false);
						count = 15;
					}
				}
				
				for (int i = 15; i >= count; i--)
				{
					if (byteValue >= pot[i])
					{
						byteValue = (int) (byteValue - pot[i]);
					}
				}
				
				// bits #1 - #7
				for (int i = count - 1; i >= 0; i--)
				{
					if (byteValue >= pot[i])
					{
						byteValue = (int) (byteValue - pot[i]);
						Write(true);
					}
					else
					{
						Write(false);
					}
				}
			}
		}
		
		////////////////////////////////////////////////////////////////////////////
		//////////////////// WRITE UINT* METHODS ///////////////////////////////////

        /// <summary>
        /// writes <code>count</code> bits of an integer into the stream beginning on the right hand.
        /// <code>Count</code> has NOT to be in byte-size (e.g. 8, 16, 24, 32,...)
        /// also use it for a uint8  (use bits=8)
        /// or unit16 (use bits=16)
        /// or uint32 (use bits=32)
        /// according to WAP-230-WSP-10010705-a secition 8.1.2
        /// </summary>
        /// <param name="b">The b.</param>
        /// <param name="count">The count.</param>
		public virtual void  Write(long b, int count)
		{
			lock (this)
			{
				// bit #0 (encoding positive/negative numbers)
				if (b < 0)
				{
					if (count == 32)
					{
						Write(true);
						count = 31;
					}
					
					b += 2147483648L;
					
					// if b >= 0
				}
				else
				{
					if (count == 32)
					{
						Write(false);
						count = 31;
					}
				}
				
				for (int i = 31; i >= count; i--)
				{
					if (b >= pot[i])
					{
						b -= pot[i];
					}
				}
				
				// bits #1 - #7
				for (int i = count - 1; i >= 0; i--)
				{
					if (b >= pot[i])
					{
						b -= pot[i];
						Write(true);
					}
					else
					{
						Write(false);
					}
				}
			}
		}

        /// <summary>
        /// write a unintvar
        /// according to WAP-230-WSP-10010705-a secition 8.1.2
        /// </summary>
        /// <param name="byteValue">The byte value.</param>
		public virtual void  WriteUIntVar(long byteValue)
		{
			lock (this)
			{              
				if (byteValue < 0)
				{
					throw new FormatException("No negative values supported");
				}
				else if (byteValue == 0)
				{
					for (int i = 0; i < 8; i++)
					{
						Write(false);
					}
				}
				else
				{
					int i = 63;
					
					while (byteValue < pot[i])
					{
						i--;
					}
					
					int length = i;
					
					// i+1 ist jetzt die Anzahl der benötigten bits
					int fill = 7 - ((i + 1) % 7); //nur 7 bits payload
					
					if (fill == 7)
					{
						fill = 0;
					}
					
					if (debug)
					{
						Console.Out.WriteLine("used bits: " + (i + 1) + " | to fill: " + fill);
					}
					
					// set the continue bit
					if ((i + 1) <= 7)
					{
						Write(false);
					}
					else
					{
						Write(true);
					}
					
					// fill up with "0"
					for (int s = 0; s < fill; s++)
					{
						Write(false);
					}
					
					for (; i >= 0; i--)
					{
						//write continue bit
						if ((((i + 1) % 7) == 0) && (length > 7))
						{
							if ((i + 1) <= 7)
							{
								Write(false);
							}
							else
							{
								Write(true);
							}
						}
						
						//write payload
						if (byteValue >= pot[i])
						{
							byteValue -= pot[i];
							Write(true);
						}
						else
						{
							Write(false);
						}
					}
				}
			}
		}

        /// <summary>
        /// writes <code>count</code> bits from byte <code>_b</code> beginning on the right hand.
        /// <code>Count</code> has NOT to be in byte-size (e.g. 8, 16, 24, 32,...)
        /// </summary>
        /// <param name="b">The b.</param>
		public virtual void  Write(byte[] b)
		{
			lock (this)
			{
				for (int i = 0; i < b.Length; i++)
				{
					this.Write(b[i], 8);
				}
			}
		}
		
		//////////////////// HELPER METHODS ////////////////////////////////////////
		
		/// <summary> constructs a string representation of the outputstream.</summary>
		public override string ToString()
		{
			string result = string.Empty;
			byte[] array = outstream.ToArray();
			
			for (int i = 0; i < array.Length; i++)
			{
				result += (GetBitString(array[i]) + "\r\n");
			}
			
			result += GetBitString((byte) (sByte & 0xFF));
			
			return result;
		}

        /// <summary>
        /// Constructs a binary representation of the byte <code>m</code>
        /// </summary>
        /// <param name="byteValue">The byte value.</param>
        /// <returns></returns>
		public static string GetBitString(byte byteValue)
		{

            string result = Convert.ToString(byteValue, 2);
            if (result.Length < 8) 
                return result.PadLeft(8, '0');
            return result;
            
            
            /*
			string result = "";

            sbyte m = unchecked((sbyte)byteValue);           
			if (m < 0)
			{
				result += "1";
				m = (byte) (m + 128);
				
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
					m = (byte) (m - pot[i]);
					result += "1";
				}
				else
				{
					result += "0";
				}
			}
			
			return result;
            */
            
		}

        /// <summary>
        /// Constructs a binary representation of the byte-array <code>m</code>
        /// </summary>
        /// <param name="m">The m.</param>
        /// <returns></returns>
		public static string GetBitString(byte[] m)
		{
			string result = "";
			
			for (int i = 0; i < m.Length; i++)
			{
				result += (GetBitString(m[i]) + Environment.NewLine);
			}
			
			return result;
		}

        /// <summary>
        /// Constructs a BitSet of the byte <code>m</code>
        /// </summary>
        /// <param name="byteValue">The byte value.</param>
        /// <returns></returns>
		public static BitArray GetBitSet(byte b)
		{
			BitArray result = new BitArray((8 % 64 == 0?8 / 64:8 / 64 + 1) * 64);

            sbyte byteValue = unchecked((sbyte)b);        

			if (byteValue < 0)
			{
				BitArrayHelper.Set(result, 0);
				byteValue = unchecked((sbyte) (byteValue + 128));
			}
			
			for (int i = 6; i >= 0; i--)
			{
				if (byteValue >= pot[i])
				{
					byteValue = unchecked((sbyte)(byteValue - pot[i]));
					BitArrayHelper.Set(result, 7 - i);
				}
			}
			
			return result;
		}
		
		
	}
}