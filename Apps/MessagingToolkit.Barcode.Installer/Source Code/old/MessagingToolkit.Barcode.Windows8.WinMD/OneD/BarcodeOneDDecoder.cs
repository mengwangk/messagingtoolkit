using System;
using System.Collections.Generic;

using BarcodeFormat = MessagingToolkit.Barcode.BarcodeFormat;
using DecodeHintType = MessagingToolkit.Barcode.DecodeOptions;
using BarcodeDecoderException = MessagingToolkit.Barcode.BarcodeDecoderException;
using Result = MessagingToolkit.Barcode.Result;
using BitArray = MessagingToolkit.Barcode.Common.BitArray;

using MessagingToolkit.Barcode.OneD.Rss;
using MessagingToolkit.Barcode.OneD.Rss.Expanded;
using MessagingToolkit.Barcode.Helper;

namespace MessagingToolkit.Barcode.OneD
{

    /// <summary>
    /// Modified April 30 2012
    /// </summary>
    internal sealed class BarcodeOneDDecoder : OneDDecoder
    {
        private readonly List<OneDDecoder> decoders;

        public BarcodeOneDDecoder(IDictionary<DecodeOptions, object> decodingOptions)
        {
            List<BarcodeFormat> possibleFormats = decodingOptions == null ? null : (List<BarcodeFormat>)BarcodeHelper.GetDecodeOptionType(decodingOptions, DecodeOptions.PossibleFormats);
            bool useCode39CheckDigit = false;
            object value = BarcodeHelper.GetDecodeOptionType(decodingOptions, DecodeOptions.AssumeCode39CheckDigit);
            if (value != null) useCode39CheckDigit = (bool)value;

            bool useMsiCheckDigit = true;
            value = BarcodeHelper.GetDecodeOptionType(decodingOptions, DecodeOptions.AssumeMsiCheckDigit);
            if (value != null) useMsiCheckDigit = (bool)value;

            this.decoders = new List<OneDDecoder>();
            if (possibleFormats != null)
            {
                if (possibleFormats.Contains(BarcodeFormat.EAN13) ||
                    possibleFormats.Contains(BarcodeFormat.UPCA) ||
                    possibleFormats.Contains(BarcodeFormat.EAN8) ||
                    possibleFormats.Contains(BarcodeFormat.UPCE))
                {
                    decoders.Add(new BarcodeUPCEANDecoder(decodingOptions));
                }

                if (possibleFormats.Contains(BarcodeFormat.MSIMod10))
                {
                    decoders.Add(new MsiDecoder(useMsiCheckDigit));
                }

                if (possibleFormats.Contains(BarcodeFormat.Code39))
                {
                    decoders.Add(new Code39Decoder(useCode39CheckDigit));
                }
                if (possibleFormats.Contains(BarcodeFormat.Code93))
                {
                    decoders.Add(new Code93Decoder());
                }
                if (possibleFormats.Contains(BarcodeFormat.Code128))
                {
                    decoders.Add(new Code128Decoder());
                }
                if (possibleFormats.Contains(BarcodeFormat.ITF14))
                {
                    decoders.Add(new ITFDecoder());
                }
                if (possibleFormats.Contains(BarcodeFormat.Codabar))
                {
                    decoders.Add(new CodaBarDecoder());
                }
                if (possibleFormats.Contains(BarcodeFormat.RSS14))
                {
                    decoders.Add(new Rss14Decoder());
                }
                if (possibleFormats.Contains(BarcodeFormat.RSSExpanded))
                {
                    decoders.Add(new RssExpandedDecoder());
                }
            }
            if (decoders.Count == 0)
            {
                decoders.Add(new BarcodeUPCEANDecoder(decodingOptions));
                decoders.Add(new Code39Decoder(useCode39CheckDigit));
                decoders.Add(new MsiDecoder(useMsiCheckDigit));
                decoders.Add(new CodaBarDecoder());
                decoders.Add(new Code93Decoder());
                decoders.Add(new Code128Decoder());
                decoders.Add(new ITFDecoder());
                decoders.Add(new Rss14Decoder());
                decoders.Add(new RssExpandedDecoder());
            }
        }

        override public Result DecodeRow(int rowNumber, BitArray row, IDictionary<DecodeOptions, object> decodingOptions)
        {
            foreach (OneDDecoder decoder in decoders)
            {
                try
                {
                    return decoder.DecodeRow(rowNumber, row, decodingOptions);
                }
                catch (BarcodeDecoderException re)
                {
                    // continue
                }
            }

            throw NotFoundException.Instance;
        }

        public override void Reset()
        {
            foreach (IDecoder decoder in decoders)
            {
                decoder.Reset();
            }
        }
    }
}