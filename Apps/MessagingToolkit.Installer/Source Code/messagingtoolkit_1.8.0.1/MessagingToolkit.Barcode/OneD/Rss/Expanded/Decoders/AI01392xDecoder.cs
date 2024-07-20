using MessagingToolkit.Barcode.Common;
using System;
using System.Text;

namespace MessagingToolkit.Barcode.OneD.Rss.Expanded.Decoders
{

    internal sealed class AI01392xDecoder : AI01decoder
    {

        private const int headerSize = 5 + 1 + 2;
        private const int lastDigitSize = 2;

        internal AI01392xDecoder(BitArray information)
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
            buf.Append("(392");
            buf.Append(lastAIdigit);
            buf.Append(')');

            DecodedInformation decodedInformation = this.generalDecoder
                    .DecodeGeneralPurposeField(headerSize + MessagingToolkit.Barcode.OneD.Rss.Expanded.Decoders.AI01decoder.gtinSize
                            + lastDigitSize, null);
            buf.Append(decodedInformation.NewString);

            return buf.ToString();
        }

    }
}
