using MessagingToolkit.Barcode.Common;
using System;
using System.Text;

namespace MessagingToolkit.Barcode.OneD.Rss.Expanded.Decoders
{

    internal sealed class AI013x0x1xDecoder : AI01weightDecoder
    {

        private const int headerSize = 7 + 1;
        private const int weightSize = 20;
        private const int dateSize = 16;

        private readonly String dateCode;
        private readonly String firstAIdigits;

        internal AI013x0x1xDecoder(BitArray information, String firstAIdigits,
                String dateCode)
            : base(information)
        {
            this.dateCode = dateCode;
            this.firstAIdigits = firstAIdigits;
        }

        public override String ParseInformation()
        {
            if (this.information.Size != headerSize + MessagingToolkit.Barcode.OneD.Rss.Expanded.Decoders.AI01decoder.gtinSize + weightSize
                    + dateSize)
            {
                throw NotFoundException.Instance;
            }

            StringBuilder buf = new StringBuilder();

            EncodeCompressedGtin(buf, headerSize);
            EncodeCompressedWeight(buf, headerSize + MessagingToolkit.Barcode.OneD.Rss.Expanded.Decoders.AI01decoder.gtinSize, weightSize);
            EncodeCompressedDate(buf, headerSize + MessagingToolkit.Barcode.OneD.Rss.Expanded.Decoders.AI01decoder.gtinSize + weightSize);

            return buf.ToString();
        }

        private void EncodeCompressedDate(StringBuilder buf, int currentPos)
        {
            int numericDate = this.generalDecoder.ExtractNumericValueFromBitArray(currentPos, dateSize);
            if (numericDate == 38400)
            {
                return;
            }

            buf.Append('(');
            buf.Append(this.dateCode);
            buf.Append(')');

            int day = numericDate % 32;
            numericDate /= 32;
            int month = numericDate % 12 + 1;
            numericDate /= 12;
            int year = numericDate;

            if (year / 10 == 0)
            {
                buf.Append('0');
            }
            buf.Append(year);
            if (month / 10 == 0)
            {
                buf.Append('0');
            }
            buf.Append(month);
            if (day / 10 == 0)
            {
                buf.Append('0');
            }
            buf.Append(day);
        }

        protected internal override void AddWeightCode(StringBuilder buf, int weight)
        {
            int lastAI = weight / 100000;
            buf.Append('(');
            buf.Append(this.firstAIdigits);
            buf.Append(lastAI);
            buf.Append(')');
        }

        protected internal override int CheckWeight(int weight)
        {
            return weight % 100000;
        }
    }
}
