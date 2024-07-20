using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

using MessagingToolkit.Barcode.Common;

namespace MessagingToolkit.Barcode.OneD.Rss.Expanded.Decoders
{

    internal sealed class AI013103decoder : AI013x0xDecoder
    {

        internal AI013103decoder(BitArray information)
            : base(information)
        {
        }

        protected internal override void AddWeightCode(StringBuilder buf, int weight)
        {
            buf.Append("(3103)");
        }

        protected internal override int CheckWeight(int weight)
        {
            return weight;
        }
    }
}
