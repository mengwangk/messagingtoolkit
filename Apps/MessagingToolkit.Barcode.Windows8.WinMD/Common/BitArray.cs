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

namespace MessagingToolkit.Barcode.Common
{

    /// <summary>
    /// <p>A simple, fast array of bits, represented compactly by an array of ints internally.</p>
    /// 
    /// Modified: April 21 2012
    /// </summary>
    public sealed class BitArray
    {

        private int[] bits;
        private int size;

        public BitArray()
        {
            this.size = 0;
            this.bits = new int[1];
        }

        public BitArray(int size)
        {
            this.size = size;
            this.bits = MakeArray(size);
        }

        public int GetSize()
        {
            return size;
        }

        public int Size
        {
            get
            {
                return size;
            }
        }

        public int GetSizeInBytes()
        {
            return (size + 7) >> 3;
        }

        private void EnsureCapacity(int size)
        {
            if (size > bits.Length << 5)
            {
                int[] newBits = MakeArray(size);
                System.Array.Copy((Array)(bits), 0, (Array)(newBits), 0, bits.Length);
                this.bits = newBits;
            }
        }


        /// <param name="i">bit to get</param>
        /// <returns>true iff bit i is set</returns>
        public bool Get(int i)
        {
            return (bits[i >> 5] & (1 << (i & 0x1F))) != 0;
        }

        /// <param name="i">bit to get</param>
        /// <returns>true iff bit i is set</returns>
        public bool GetValue(int i)
        {
            return Get(i);
        }

        /// <summary>
        /// Sets bit i.
        /// </summary>
        ///
        /// <param name="i">bit to set</param>
        public void Set(int i)
        {
            bits[i >> 5] |= 1 << (i & 0x1F);
        }

        /// <summary>
        /// Flips bit i.
        /// </summary>
        ///
        /// <param name="i">bit to set</param>
        public void Flip(int i)
        {
            bits[i >> 5] ^= 1 << (i & 0x1F);
        }


        /// <param name="from">first bit to check</param>
        /// <returns>index of first bit that is set, starting from the given index, or size if none are set
        /// at or beyond this given index</returns>
        /// <seealso cref="M:Com.Google.Zxing.Common.BitArray.GetNextUnset(System.Int32)"/>
        public int GetNextSet(int from)
        {
            if (from >= size)
            {
                return size;
            }
            int bitsOffset = from >> 5;
            int currentBits = bits[bitsOffset];
            // mask off lesser bits first
            currentBits &= ~((1 << (from & 0x1F)) - 1);
            while (currentBits == 0)
            {
                if (++bitsOffset == bits.Length)
                {
                    return size;
                }
                currentBits = bits[bitsOffset];
            }
            int result = (bitsOffset << 5) + NumberOfTrailingZeros(currentBits);
            return (result > size) ? size : result;
        }


        /// <summary>
        /// Gets the next unset.
        /// </summary>
        /// <param name="from">From.</param>
        /// <returns></returns>
        public int GetNextUnset(int from)
        {
            if (from >= size)
            {
                return size;
            }
            int bitsOffset = from >> 5;
            int currentBits = ~bits[bitsOffset];
            // mask off lesser bits first
            currentBits &= ~((1 << (from & 0x1F)) - 1);
            while (currentBits == 0)
            {
                if (++bitsOffset == bits.Length)
                {
                    return size;
                }
                currentBits = ~bits[bitsOffset];
            }
            int result = (bitsOffset << 5) + NumberOfTrailingZeros(currentBits);
            return (result > size) ? size : result;
        }


        private static int NumberOfTrailingZeros(int num)
        {
            var index = (-num & num) % 37;
            if (index < 0)
                index *= -1;
            return Lookup[index];
        }

        private static readonly int[] Lookup =
         {
            32, 0, 1, 26, 2, 23, 27, 0, 3, 16, 24, 30, 28, 11, 0, 13, 4, 7, 17,
            0, 25, 22, 31, 15, 29, 10, 12, 6, 0, 21, 14, 9, 5, 20, 8, 19, 18
         };


        /// <summary>
        /// Sets a block of 32 bits, starting at bit i.
        /// </summary>
        ///
        /// <param name="i">first bit to set</param>
        /// <param name="newBits"></param>
        public void SetBulk(int i, int newBits)
        {
            bits[i >> 5] = newBits;
        }

        /// <summary>
        /// Sets a range of bits.
        /// </summary>
        ///
        /// <param name="start">start of range, inclusive.</param>
        /// <param name="end">end of range, exclusive</param>
        public void SetRange(int start, int end)
        {
            if (end < start)
            {
                throw new ArgumentException();
            }
            if (end == start)
            {
                return;
            }
            end--; // will be easier to treat this as the last actually set bit -- inclusive
            int firstInt = start >> 5;
            int lastInt = end >> 5;
            for (int i = firstInt; i <= lastInt; i++)
            {
                int firstBit = (i > firstInt) ? 0 : start & 0x1F;
                int lastBit = (i < lastInt) ? 31 : end & 0x1F;
                int mask;
                if (firstBit == 0 && lastBit == 31)
                {
                    mask = -1;
                }
                else
                {
                    mask = 0;
                    for (int j = firstBit; j <= lastBit; j++)
                    {
                        mask |= 1 << j;
                    }
                }
                bits[i] |= mask;
            }
        }

        /// <summary>
        /// Clears all bits (sets to false).
        /// </summary>
        ///
        public void Clear()
        {
            int max = bits.Length;
            for (int i = 0; i < max; i++)
            {
                bits[i] = 0;
            }
        }

        /// <summary>
        /// Efficient method to check if a range of bits is set, or not set.
        /// </summary>
        ///
        /// <param name="start">start of range, inclusive.</param>
        /// <param name="end">end of range, exclusive</param>
        /// <param name="value">if true, checks that bits in range are set, otherwise checks that they are not set</param>
        /// <returns>true iff all bits are set or not set in range, according to value argument</returns>
        /// <exception cref="System.ArgumentException">if end is less than or equal to start</exception>
        public bool IsRange(int start, int end, bool val)
        {
            if (end < start)
            {
                throw new ArgumentException();
            }
            if (end == start)
            {
                return true; // empty range matches
            }
            end--; // will be easier to treat this as the last actually set bit -- inclusive
            int firstInt = start >> 5;
            int lastInt = end >> 5;
            for (int i = firstInt; i <= lastInt; i++)
            {
                int firstBit = (i > firstInt) ? 0 : start & 0x1F;
                int lastBit = (i < lastInt) ? 31 : end & 0x1F;
                int mask;
                if (firstBit == 0 && lastBit == 31)
                {
                    mask = -1;
                }
                else
                {
                    mask = 0;
                    for (int j = firstBit; j <= lastBit; j++)
                    {
                        mask |= 1 << j;
                    }
                }

                // Return false if we're looking for 1s and the masked bits[i] isn't all 1s (that is,
                // equals the mask, or we're looking for 0s and the masked portion is not all 0s
                if ((bits[i] & mask) != ((val) ? mask : 0))
                {
                    return false;
                }
            }
            return true;
        }

        public void AppendBit(bool bit)
        {
            EnsureCapacity(size + 1);
            if (bit)
            {
                bits[size >> 5] |= 1 << (size & 0x1F);
            }
            size++;
        }

        /// <summary>
        /// Appends the least-significant bits, from value, in order from most-significant to
        /// least-significant. For example, appending 6 bits from 0x000001E will append the bits
        /// 0, 1, 1, 1, 1, 0 in that order.
        /// </summary>
        ///
        public void AppendBits(int val, int numBits)
        {
            if (numBits < 0 || numBits > 32)
            {
                throw new ArgumentException("Num bits must be between 0 and 32");
            }
            EnsureCapacity(size + numBits);
            for (int numBitsLeft = numBits; numBitsLeft > 0; numBitsLeft--)
            {
                AppendBit(((val >> (numBitsLeft - 1)) & 0x01) == 1);
            }
        }

        public void AppendBitArray(BitArray other)
        {
            int otherSize = other.size;
            EnsureCapacity(size + otherSize);
            for (int i = 0; i < otherSize; i++)
            {
                AppendBit(other.Get(i));
            }
        }

        public void Xor(BitArray other)
        {
            if (bits.Length != other.bits.Length)
            {
                throw new ArgumentException("Sizes don't match");
            }
            for (int i = 0; i < bits.Length; i++)
            {
                // The last byte could be incomplete (i.e. not have 8 bits in
                // it) but there is no problem since 0 XOR 0 == 0.
                bits[i] ^= other.bits[i];
            }
        }


        /// <param name="bitOffset">first bit to start writing</param>
        /// <param name="array"><see cref="M:Com.Google.Zxing.Common.BitArray.GetBitArray"/></param>
        /// <param name="offset">position in array to start writing</param>
        /// <param name="numBytes">how many bytes to write</param>
        public void ToBytes(int bitOffset, [System.Runtime.InteropServices.WindowsRuntime.ReadOnlyArray] byte[] array, int offset, int numBytes)
        {
            for (int i = 0; i < numBytes; i++)
            {
                int theByte = 0;
                for (int j = 0; j < 8; j++)
                {
                    if (Get(bitOffset))
                    {
                        theByte |= 1 << (7 - j);
                    }
                    bitOffset++;
                }
                array[offset + i] = (byte)theByte;
            }
        }


        /// <returns>underlying array of ints. The first element holds the first 32 bits, and the least
        /// significant bit is bit 0.</returns>
        public int[] GetBitArray()
        {
            return bits;
        }

        /// <summary>
        /// Reverses all bits in the array.
        /// </summary>
        ///
        public void Reverse()
        {
            int[] newBits = new int[bits.Length];
            int size_0 = this.size;
            for (int i = 0; i < size_0; i++)
            {
                if (Get(size_0 - i - 1))
                {
                    newBits[i >> 5] |= 1 << (i & 0x1F);
                }
            }
            bits = newBits;
        }

        private static int[] MakeArray(int size)
        {
            return new int[(size + 31) >> 5];
        }

        public override String ToString()
        {
            StringBuilder result = new StringBuilder(size);
            for (int i = 0; i < size; i++)
            {
                if ((i & 0x07) == 0)
                {
                    result.Append(' ');
                }
                result.Append((Get(i)) ? 'X' : '.');
            }
            return result.ToString();
        }

    }
}