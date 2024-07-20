using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;

using MessagingToolkit.Barcode;
using MessagingToolkit.Barcode.Common;

namespace MessagingToolkit.Barcode.OneD.Rss.Expanded.Decoders
{

    internal abstract class AbstractExpandedDecoder
    {

        internal readonly BitArray information;
        internal readonly GeneralAppIdDecoder generalDecoder;

        internal AbstractExpandedDecoder(BitArray information)
        {
            this.information = information;
            this.generalDecoder = new GeneralAppIdDecoder(information);
        }

        protected BitArray Information
        {
            get
            {
                return information;
            }
        }

        protected GeneralAppIdDecoder GeneralDecoder
        {
            get
            {
                return generalDecoder;
            }
        }

        public abstract String ParseInformation();

        public static AbstractExpandedDecoder CreateDecoder(BitArray information)
        {
            if (information.Get(1))
            {
                return new AI01AndOtherAIs(information);
            }
            else if (!information.Get(2))
            {
                return new AnyAIDecoder(information);
            }

            int fourBitEncodationMethod = MessagingToolkit.Barcode.OneD.Rss.Expanded.Decoders.GeneralAppIdDecoder
                    .ExtractNumericValueFromBitArray(information, 1, 4);

            switch (fourBitEncodationMethod)
            {
                case 4:
                    return new AI013103decoder(information);
                case 5:
                    return new AI01320xDecoder(information);
            }

            int fiveBitEncodationMethod = MessagingToolkit.Barcode.OneD.Rss.Expanded.Decoders.GeneralAppIdDecoder
                    .ExtractNumericValueFromBitArray(information, 1, 5);
            switch (fiveBitEncodationMethod)
            {
                case 12:
                    return new AI01392xDecoder(information);
                case 13:
                    return new AI01393xDecoder(information);
            }

            int sevenBitEncodationMethod = MessagingToolkit.Barcode.OneD.Rss.Expanded.Decoders.GeneralAppIdDecoder.ExtractNumericValueFromBitArray(information, 1, 7);
            switch (sevenBitEncodationMethod)
            {
                case 56:
                    return new AI013x0x1xDecoder(information, "310", "11");
                case 57:
                    return new AI013x0x1xDecoder(information, "320", "11");
                case 58:
                    return new AI013x0x1xDecoder(information, "310", "13");
                case 59:
                    return new AI013x0x1xDecoder(information, "320", "13");
                case 60:
                    return new AI013x0x1xDecoder(information, "310", "15");
                case 61:
                    return new AI013x0x1xDecoder(information, "320", "15");
                case 62:
                    return new AI013x0x1xDecoder(information, "310", "17");
                case 63:
                    return new AI013x0x1xDecoder(information, "320", "17");
            }

            throw new InvalidOperationException("unknown decoder: " + information);
        }

    }
}
