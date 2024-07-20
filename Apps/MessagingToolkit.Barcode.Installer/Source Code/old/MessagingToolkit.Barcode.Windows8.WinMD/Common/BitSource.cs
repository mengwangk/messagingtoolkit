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

namespace MessagingToolkit.Barcode.Common
{

    /// <summary>
    /// This provides an easy abstraction to read bits at a time from a sequence of bytes, where the
    /// number of bits read is not often a multiple of 8.
    /// This class is thread-safe but not reentrant. Unless the caller modifies the bytes array
    /// it passed in, in which case all bets are off.
    /// 
    /// Modified: April 21 2012
    /// </summary>
    public sealed class BitSource
    {
        private readonly byte[] bytes;
        private int byteOffset;
        private int bitOffset;


        /// <param name="bytes"></param>
        public BitSource([System.Runtime.InteropServices.WindowsRuntime.ReadOnlyArray] byte[] bytes)
        {
            this.bytes = bytes;
        }


        /// <summary>
        /// Gets the byte offset.
        /// </summary>
        /// <returns>
        /// index of next byte in input byte array which would be read by the next call to
        /// </returns>
        public int ByteOffset
        {
            get
            {
                return byteOffset;
            }
        }


        /// <summary>
        /// Return index of next bit in current byte which would be read by the next call to <seealso cref="ReadBits"/>
        /// </summary>
        public int BitOffset
        {
            get
            {
                return bitOffset;
            }
        }

        /// <param name="numBits">number of bits to read</param>
        /// <returns>int representing the bits read. The bits will appear as the least-significant
        /// bits of the int</returns>
        /// <exception cref="System.ArgumentException">if numBits isn't in [1,32]</exception>
        public int ReadBits(int numBits)
        {
            if (numBits < 1 || numBits > 32)
            {
                throw new ArgumentException(numBits.ToString());
            }

            int result = 0;

            // First, read remainder from current byte
            if (bitOffset > 0)
            {
                int bitsLeft = 8 - bitOffset;
                int toRead = (numBits < bitsLeft) ? numBits : bitsLeft;
                int bitsToNotRead = bitsLeft - toRead;
                int mask = (0xFF >> (8 - toRead)) << bitsToNotRead;
                result = (bytes[byteOffset] & mask) >> bitsToNotRead;
                numBits -= toRead;
                bitOffset += toRead;
                if (bitOffset == 8)
                {
                    bitOffset = 0;
                    byteOffset++;
                }
            }

            // Next read whole bytes
            if (numBits > 0)
            {
                while (numBits >= 8)
                {
                    result = (result << 8) | (bytes[byteOffset] & 0xFF);
                    byteOffset++;
                    numBits -= 8;
                }

                // Finally read a partial byte
                if (numBits > 0)
                {
                    int bitsToNotRead_0 = 8 - numBits;
                    int mask_1 = (0xFF >> bitsToNotRead_0) << bitsToNotRead_0;
                    result = (result << numBits) | ((bytes[byteOffset] & mask_1) >> bitsToNotRead_0);
                    bitOffset += numBits;
                }
            }

            return result;
        }


        /// <returns>number of bits that can be read successfully</returns>
        public int Available()
        {
            return 8 * (bytes.Length - byteOffset) - bitOffset;
        }

    }
}