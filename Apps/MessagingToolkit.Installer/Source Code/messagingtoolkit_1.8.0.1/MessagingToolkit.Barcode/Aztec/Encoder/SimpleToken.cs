using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MessagingToolkit.Barcode.Common;
using MessagingToolkit.Barcode.Helper;

namespace MessagingToolkit.Barcode.Aztec.Encoder
{
    internal sealed class SimpleToken : Token
    {

        // For normal words, indicates value and bitCount
        private readonly short value;
        private readonly short bitCount;

        internal SimpleToken(Token previous, int totalBitCount, int value, int bitCount)
            : base(previous, totalBitCount)
        {
            this.value = (short)value;
            this.bitCount = (short)bitCount;
        }

        public override void AppendTo(BitArray bitArray, byte[] text)
        {
            bitArray.AppendBits(value, bitCount);
        }

        public override String ToString()
        {
            int value = this.value & ((1 << bitCount) - 1);
            value |= 1 << bitCount;
            return '<' + BarcodeHelper.ToBinaryString(value | (1 << bitCount)).Substring(1) + '>';
        }

    }
}
