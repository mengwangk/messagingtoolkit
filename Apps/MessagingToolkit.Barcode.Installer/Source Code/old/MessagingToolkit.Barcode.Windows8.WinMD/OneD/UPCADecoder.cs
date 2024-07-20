using System;
using System.Text;
using System.Collections.Generic;
using BarcodeFormat = MessagingToolkit.Barcode.BarcodeFormat;
using BarcodeDecoderException = MessagingToolkit.Barcode.BarcodeDecoderException;
using Result = MessagingToolkit.Barcode.Result;
using BinaryBitmap = MessagingToolkit.Barcode.BinaryBitmap;
using BitArray = MessagingToolkit.Barcode.Common.BitArray;

namespace MessagingToolkit.Barcode.OneD
{

    /// <summary>
    /// Implements decoding of the UPC-A format.
    /// 
    /// Modified: April 19 2012
    /// </summary>
    internal sealed class UPCADecoder : UPCEANDecoder
    {
        private readonly UPCEANDecoder ean13Decoder;

        public UPCADecoder()
        {
            this.ean13Decoder = new EAN13Decoder();
        }

        public override Result DecodeRow(int rowNumber, BitArray row, int[] startGuardRange, IDictionary<DecodeOptions, object> decodingOptions)
        {
            return MaybeReturnResult(ean13Decoder.DecodeRow(rowNumber, row, startGuardRange, decodingOptions));
        }

        public override Result DecodeRow(int rowNumber, BitArray row, IDictionary<DecodeOptions, object> decodingOptions)
        {
            return MaybeReturnResult(ean13Decoder.DecodeRow(rowNumber, row, decodingOptions));
        }

        public override Result Decode(BinaryBitmap image)
        {
            return MaybeReturnResult(ean13Decoder.Decode(image));
        }

        public override Result Decode(BinaryBitmap image, IDictionary<DecodeOptions, object> decodingOptions)
        {
            return MaybeReturnResult(ean13Decoder.Decode(image, decodingOptions));
        }

        override internal BarcodeFormat BarcodeFormat
        {
            get
            {
                return BarcodeFormat.UPCA;
            }
        }


        protected internal override int DecodeMiddle(BitArray row, int[] startRange, StringBuilder resultString)
        {
            return ean13Decoder.DecodeMiddle(row, startRange, resultString);
        }

        private static Result MaybeReturnResult(Result result)
        {
            String text = result.Text;
            if (text[0] == '0')
            {
                return new Result(text.Substring(1), null, result.ResultPoints, BarcodeFormat.UPCA);
            }
            else
            {
                throw FormatException.Instance;
            }
        }
    }
}