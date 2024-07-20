
using System;
using System.Text;

using MessagingToolkit.Barcode.Common;

namespace MessagingToolkit.Barcode.OneD.Rss.Expanded.Decoders
{
    abstract internal class AI013x0xDecoder : AI01weightDecoder
    {

        private const int headerSize = 4 + 1;
        private const int weightSize = 15;

        internal AI013x0xDecoder(BitArray information)
            : base(information)
        {
        }

        public override String ParseInformation()
        {
            if (this.information.Size != headerSize + MessagingToolkit.Barcode.OneD.Rss.Expanded.Decoders.AI01decoder.gtinSize + weightSize)
            {
                throw NotFoundException.Instance;
            }

            StringBuilder buf = new StringBuilder();

            EncodeCompressedGtin(buf, headerSize);
            EncodeCompressedWeight(buf, headerSize + MessagingToolkit.Barcode.OneD.Rss.Expanded.Decoders.AI01decoder.gtinSize, weightSize);

            return buf.ToString();
        }
    }
}
