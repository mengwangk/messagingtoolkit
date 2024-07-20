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
using System.Text;

using BarcodeFormat = MessagingToolkit.Barcode.BarcodeFormat;
using ReaderException = MessagingToolkit.Barcode.BarcodeDecoderException;
using BitArray = MessagingToolkit.Barcode.Common.BitArray;

namespace MessagingToolkit.Barcode.OneD
{

    /// <summary>
    /// Implements decoding of the EAN-13 format.
    /// 
    /// Modified: April 30 2012
    /// </summary>
	public sealed class EAN13Decoder:UPCEANDecoder
	{
		// For an EAN-13 barcode, the first digit is represented by the parities used
		// to encode the next six digits, according to the table below. For example,
		// if the barcode is 5 123456 789012 then the value of the first digit is
		// signified by using odd for '1', even for '2', even for '3', odd for '4',
		// odd for '5', and even for '6'. See http://en.wikipedia.org/wiki/EAN-13
		//
		//                Parity of next 6 digits
		//    Digit   0     1     2     3     4     5
		//       0    Odd   Odd   Odd   Odd   Odd   Odd
		//       1    Odd   Odd   Even  Odd   Even  Even
		//       2    Odd   Odd   Even  Even  Odd   Even
		//       3    Odd   Odd   Even  Even  Even  Odd
		//       4    Odd   Even  Odd   Odd   Even  Even
		//       5    Odd   Even  Even  Odd   Odd   Even
		//       6    Odd   Even  Even  Even  Odd   Odd
		//       7    Odd   Even  Odd   Even  Odd   Even
		//       8    Odd   Even  Odd   Even  Even  Odd
		//       9    Odd   Even  Even  Odd   Even  Odd
		//
		// Note that the encoding for '0' uses the same parity as a UPC barcode. Hence
		// a UPC barcode can be converted to an EAN-13 barcode by prepending a 0.
		//
		// The encoding is represented by the following array, which is a bit pattern
		// using Odd = 0 and Even = 1. For example, 5 is represented by:
		//
		//              Odd Even Even Odd Odd Even
		// in binary:
		//                0    1    1   0   0    1   == 0x19
		//
		static internal readonly int[] FirstDigitEncodings = { 0x00, 0x0B, 0x0D, 0xE, 0x13, 0x19, 0x1C, 0x15, 0x16, 0x1A };
	
		private readonly int[] decodeMiddleCounters;

        public EAN13Decoder()
        {
			decodeMiddleCounters = new int[4];
		}
	
		protected internal override int DecodeMiddle(BitArray row, int[] startRange, StringBuilder resultString) {
			int[] counters = decodeMiddleCounters;
			counters[0] = 0;
			counters[1] = 0;
			counters[2] = 0;
			counters[3] = 0;
			int end = row.GetSize();
			int rowOffset = startRange[1];
	
			int lgPatternFound = 0;
	
			for (int x = 0; x < 6 && rowOffset < end; x++) {
				int bestMatch = UPCEANDecoder.DecodeDigit(row, counters, rowOffset, UPCEANDecoder.LAndGPatterns);
				resultString.Append((char) ('0' + bestMatch % 10));
				/* foreach */
				foreach (int counter  in  counters) {
					rowOffset += counter;
				}
				if (bestMatch >= 10) {
					lgPatternFound |= 1 << (5 - x);
				}
			}
	
			DetermineFirstDigit(resultString, lgPatternFound);

            int[] middleRange = UPCEANDecoder.FindGuardPattern(row, rowOffset, true, UPCEANDecoder.MiddlePattern);
			rowOffset = middleRange[1];
	
			for (int x_0 = 0; x_0 < 6 && rowOffset < end; x_0++) {
                int bestMatch_1 = UPCEANDecoder.DecodeDigit(row, counters, rowOffset, UPCEANDecoder.LPatterns);
				resultString.Append((char) ('0' + bestMatch_1));
				/* foreach */
				foreach (int counter_2  in  counters) {
					rowOffset += counter_2;
				}
			}
	
			return rowOffset;
		}
	
		internal override BarcodeFormat BarcodeFormat {
            get {
			return BarcodeFormat.EAN13;
            }
		}
	
		/// <summary>
		/// Based on pattern of odd-even ('L' and 'G') patterns used to encoded the explicitly-encoded
		/// digits in a barcode, determines the implicitly encoded first digit and adds it to the
		/// result string.
		/// </summary>
		///
		/// <param name="resultString">string to insert decoded first digit into</param>
		/// <param name="lgPatternFound"></param>
		/// <exception cref="NotFoundException">if first digit cannot be determined</exception>
		private static void DetermineFirstDigit(StringBuilder resultString, int lgPatternFound) {
			for (int d = 0; d < 10; d++) {
				if (lgPatternFound == FirstDigitEncodings[d]) {

#if !SILVERLIGHT
					resultString.Insert(0, (char) ('0' + d));
#else
                    resultString.Insert(0, new char[] { (char)('0' + d) });
#endif
                    return;
				}
			}
            throw NotFoundException.Instance;
		}
	
	}
}