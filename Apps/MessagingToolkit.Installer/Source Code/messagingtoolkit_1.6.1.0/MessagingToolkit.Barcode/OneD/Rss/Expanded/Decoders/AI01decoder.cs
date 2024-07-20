using MessagingToolkit.Barcode.Common;
using System.Text;

namespace MessagingToolkit.Barcode.OneD.Rss.Expanded.Decoders
{
    abstract internal class AI01decoder : AbstractExpandedDecoder
    {

        protected internal const int gtinSize = 40;

        internal AI01decoder(BitArray information)
            : base(information)
        {
        }

        protected internal void EncodeCompressedGtin(StringBuilder buf, int currentPos)
        {
            buf.Append("(01)");
            int initialPosition = buf.Length;
            buf.Append('9');

            EncodeCompressedGtinWithoutAI(buf, currentPos, initialPosition);
        }

        protected internal void EncodeCompressedGtinWithoutAI(StringBuilder buf,
                int currentPos, int initialBufferPosition)
        {
            for (int i = 0; i < 4; ++i)
            {
                int currentBlock = this.generalDecoder
                        .ExtractNumericValueFromBitArray(currentPos + 10 * i, 10);
                if (currentBlock / 100 == 0)
                {
                    buf.Append('0');
                }
                if (currentBlock / 10 == 0)
                {
                    buf.Append('0');
                }
                buf.Append(currentBlock);
            }

            AppendCheckDigit(buf, initialBufferPosition);
        }

        private static void AppendCheckDigit(StringBuilder buf, int currentPos)
        {
            int checkDigit = 0;
            for (int i = 0; i < 13; i++)
            {
                int digit = buf[i + currentPos] - '0';
                checkDigit += ((i & 0x01) == 0) ? 3 * digit : digit;
            }

            checkDigit = 10 - (checkDigit % 10);
            if (checkDigit == 10)
            {
                checkDigit = 0;
            }

            buf.Append(checkDigit);
        }

    }
}
