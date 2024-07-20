using MessagingToolkit.Barcode.Common;

using System.Text;

namespace MessagingToolkit.Barcode.OneD.Rss.Expanded.Decoders
{
    abstract internal class AI01weightDecoder : AI01decoder
    {

        internal AI01weightDecoder(BitArray information)
            : base(information)
        {
        }

        protected internal void EncodeCompressedWeight(StringBuilder buf, int currentPos,
                int weightSize)
        {
            int originalWeightNumeric = this.generalDecoder
                    .ExtractNumericValueFromBitArray(currentPos, weightSize);
            AddWeightCode(buf, originalWeightNumeric);

            int weightNumeric = CheckWeight(originalWeightNumeric);

            int currentDivisor = 100000;
            for (int i = 0; i < 5; ++i)
            {
                if (weightNumeric / currentDivisor == 0)
                {
                    buf.Append('0');
                }
                currentDivisor /= 10;
            }
            buf.Append(weightNumeric);
        }

        protected abstract internal void AddWeightCode(StringBuilder buf, int weight);

        protected abstract internal int CheckWeight(int weight);
    }
}
