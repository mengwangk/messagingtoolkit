using System;
using System.Text;

using MessagingToolkit.Barcode.Common;

namespace MessagingToolkit.Barcode.OneD.Rss.Expanded.Decoders
{
    internal sealed class AI01393xDecoder : AI01decoder
    {

        private const int headerSize = 5 + 1 + 2;
        private const int lastDigitSize = 2;
        private const int firstThreeDigitsSize = 10;

        internal AI01393xDecoder(BitArray information)
            : base(information)
        {
        }

        public override String ParseInformation()
        {
            if (this.information.Size < headerSize + MessagingToolkit.Barcode.OneD.Rss.Expanded.Decoders.AI01decoder.gtinSize)
            {
                throw NotFoundException.Instance;
            }

            StringBuilder buf = new StringBuilder();

            EncodeCompressedGtin(buf, headerSize);

            int lastAIdigit = this.generalDecoder.ExtractNumericValueFromBitArray(
                    headerSize + MessagingToolkit.Barcode.OneD.Rss.Expanded.Decoders.AI01decoder.gtinSize, lastDigitSize);

            buf.Append("(393");
            buf.Append(lastAIdigit);
            buf.Append(')');

            int firstThreeDigits = this.generalDecoder
                    .ExtractNumericValueFromBitArray(headerSize + MessagingToolkit.Barcode.OneD.Rss.Expanded.Decoders.AI01decoder.gtinSize
                            + lastDigitSize, firstThreeDigitsSize);
            if (firstThreeDigits / 100 == 0)
            {
                buf.Append('0');
            }
            if (firstThreeDigits / 10 == 0)
            {
                buf.Append('0');
            }
            buf.Append(firstThreeDigits);

            DecodedInformation generalInformation = this.generalDecoder
                    .DecodeGeneralPurposeField(headerSize + MessagingToolkit.Barcode.OneD.Rss.Expanded.Decoders.AI01decoder.gtinSize
                            + lastDigitSize + firstThreeDigitsSize, null);
            buf.Append(generalInformation.NewString);

            return buf.ToString();
        }
    }
}
