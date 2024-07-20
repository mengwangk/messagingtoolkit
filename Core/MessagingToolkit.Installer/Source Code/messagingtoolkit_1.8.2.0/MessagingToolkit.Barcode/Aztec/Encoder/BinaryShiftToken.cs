using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MessagingToolkit.Barcode.Common;

namespace MessagingToolkit.Barcode.Aztec.Encoder
{
    internal sealed class BinaryShiftToken : Token
    {

        private readonly short binaryShiftStart;
        private readonly short binaryShiftByteCount;

        internal BinaryShiftToken(Token previous, int totalBitCount, int binaryShiftStart, int binaryShiftByteCount)
            : base(previous, totalBitCount)
        {
            this.binaryShiftStart = (short)binaryShiftStart;
            this.binaryShiftByteCount = (short)binaryShiftByteCount;
        }

        public override void AppendTo(BitArray bitArray, byte[] text)
        {
            for (int i = 0; i < binaryShiftByteCount; i++)
            {
                if (i == 0 || (i == 31 && binaryShiftByteCount <= 62))
                {
                    // We need a header before the first character, and before
                    // character 31 when the total byte code is <= 62
                    bitArray.AppendBits(31, 5);
                    if (binaryShiftByteCount > 62)
                    {
                        bitArray.AppendBits(binaryShiftByteCount - 31, 16);
                    }
                    else if (i == 0)
                    {
                        // 1 <= binaryShiftByteCode <= 62
                        short val = 31;
                        bitArray.AppendBits(Math.Min(binaryShiftByteCount, val), 5);
                    }
                    else
                    {
                        // 32 <= binaryShiftCount <= 62 and i == 31
                        bitArray.AppendBits(binaryShiftByteCount - 31, 5);
                    }
                }
                bitArray.AppendBits(text[binaryShiftStart + i], 8);
            }
            //assert bitArray.getSize() == getTotalBitCount();
        }

        public override string ToString()
        {
            return "<" + binaryShiftStart + "::" + (binaryShiftStart + binaryShiftByteCount - 1) + '>';
        }

    }
}
