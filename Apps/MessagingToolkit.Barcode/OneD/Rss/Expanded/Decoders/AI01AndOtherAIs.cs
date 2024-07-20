using MessagingToolkit.Barcode;
using MessagingToolkit.Barcode.Common;
using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace MessagingToolkit.Barcode.OneD.Rss.Expanded.Decoders
{
    internal sealed class AI01AndOtherAIs : AI01decoder
    {

        private const int HEADER_SIZE = 1 + 1 + 2; //first bit encodes the linkage flag,

        //the second one is the encodation method, and the other two are for the variable length
        internal AI01AndOtherAIs(BitArray information)
            : base(information)
        {
        }

        public override String ParseInformation()
        {
            StringBuilder buff = new StringBuilder();

            buff.Append("(01)");
            int initialGtinPosition = buff.Length;
            int firstGtinDigit = this.generalDecoder
                    .ExtractNumericValueFromBitArray(HEADER_SIZE, 4);
            buff.Append(firstGtinDigit);

            this.EncodeCompressedGtinWithoutAI(buff, HEADER_SIZE + 4,
                    initialGtinPosition);

            return this.generalDecoder.DecodeAllCodes(buff, HEADER_SIZE + 44);
        }
    }
}
