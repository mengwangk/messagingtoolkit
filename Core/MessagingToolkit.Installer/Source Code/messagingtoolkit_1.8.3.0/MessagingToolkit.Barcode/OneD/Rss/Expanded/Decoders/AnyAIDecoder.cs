using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

using MessagingToolkit.Barcode;
using MessagingToolkit.Barcode.Common;

namespace MessagingToolkit.Barcode.OneD.Rss.Expanded.Decoders
{


    internal sealed class AnyAIDecoder : AbstractExpandedDecoder
    {

        private const int HeaderSize = 2 + 1 + 2;

        internal AnyAIDecoder(BitArray information)
            : base(information)
        {
        }

        public override String ParseInformation()
        {
            StringBuilder buf = new StringBuilder();
            return this.generalDecoder.DecodeAllCodes(buf, HeaderSize);
        }
    }
}
