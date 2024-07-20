using MessagingToolkit.Barcode.Common;

using System.Text;

namespace MessagingToolkit.Barcode.OneD.Rss.Expanded.Decoders
{
    internal sealed class AI01320xDecoder : AI013x0xDecoder
    {

        internal AI01320xDecoder(BitArray information)
            : base(information)
        {
        }

        protected internal override void AddWeightCode(StringBuilder buf, int weight)
        {
            if (weight < 10000)
            {
                buf.Append("(3202)");
            }
            else
            {
                buf.Append("(3203)");
            }
        }

        protected internal override int CheckWeight(int weight)
        {
            if (weight < 10000)
            {
                return weight;
            }
            return weight - 10000;
        }

    }
}
