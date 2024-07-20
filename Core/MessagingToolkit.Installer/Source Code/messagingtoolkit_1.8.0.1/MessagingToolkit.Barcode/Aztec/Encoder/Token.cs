using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MessagingToolkit.Barcode.Common;

namespace MessagingToolkit.Barcode.Aztec.Encoder
{
    internal abstract class Token
    {

        internal static readonly Token EMPTY = new SimpleToken(null, 0, 0, 0);

        private readonly Token previous;
        private readonly int totalBitCount; // For debugging purposes, only

        internal Token(Token previous, int totalBitCount)
        {
            this.previous = previous;
            this.totalBitCount = totalBitCount;
        }

        internal Token Previous
        {
            get
            {
                return previous;
            }
        }

        internal int TotalBitCount
        {
            get
            {
                return totalBitCount;
            }
        }

        internal Token Add(int value, int bitCount)
        {
            return new SimpleToken(this, this.totalBitCount + bitCount, value, bitCount);
        }

        internal Token AddBinaryShift(int start, int byteCount)
        {
            int bitCount = (byteCount * 8) + (byteCount <= 31 ? 10 : byteCount <= 62 ? 20 : 21);
            return new BinaryShiftToken(this, this.totalBitCount + bitCount, start, byteCount);
        }

        public abstract void AppendTo(BitArray bitArray, byte[] text);

    }
}
