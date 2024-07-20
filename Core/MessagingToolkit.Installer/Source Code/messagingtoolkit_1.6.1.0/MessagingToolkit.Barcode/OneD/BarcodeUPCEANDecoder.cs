//===============================================================================
// Copyright © TWIT88.COM.  All rights reserved.
//
// This file is part of Open Source Messaging Library.
//
// Open Source Messaging Library is free software: you can redistribute it 
// and/or modify it under the terms of the GNU General Public License version 3.
//
// Open Source Messaging Library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this software.  If not, see <http://www.gnu.org/licenses/>.
//===============================================================================

using System;
using System.Collections.Generic;

using BarcodeFormat = MessagingToolkit.Barcode.BarcodeFormat;
using DecodeHintType = MessagingToolkit.Barcode.DecodeOptions;
using BarcodeDecoderException = MessagingToolkit.Barcode.BarcodeDecoderException;
using Result = MessagingToolkit.Barcode.Result;
using BitArray = MessagingToolkit.Barcode.Common.BitArray;
using MessagingToolkit.Barcode.Helper;
using MessagingToolkit.Barcode.Common;

namespace MessagingToolkit.Barcode.OneD
{

    /// <summary>
    /// A reader that can read all available UPC/EAN formats. If a caller wants to try to
    /// read all such formats, it is most efficient to use this implementation rather than invoke
    /// individual readers.
    /// 
    /// Modified: April 30 2012
    /// </summary>
    public sealed class BarcodeUPCEANDecoder : OneDDecoder
    {
        private readonly UPCEANDecoder[] decoders;

        public BarcodeUPCEANDecoder(Dictionary<DecodeOptions, object> decodingOptions)
        {
            List<BarcodeFormat> possibleFormats = decodingOptions == null ? null : (List<BarcodeFormat>)BarcodeHelper.GetDecodeOptionType(decodingOptions, DecodeOptions.PossibleFormats);

            var readers = new List<UPCEANDecoder>();
            if (possibleFormats != null)
            {
                if (possibleFormats.Contains(BarcodeFormat.EAN13))
                {
                    readers.Add(new EAN13Decoder());
                }
                else if (possibleFormats.Contains(BarcodeFormat.UPCA))
                {
                    readers.Add(new UPCADecoder());
                }
                if (possibleFormats.Contains(BarcodeFormat.EAN8))
                {
                    readers.Add(new EAN8Decoder());
                }
                if (possibleFormats.Contains(BarcodeFormat.UPCE))
                {
                    readers.Add(new UPCEDecoder());
                }
            }
            if (readers.Count == 0)
            {
                readers.Add(new EAN13Decoder());
                // UPC-A is covered by EAN-13
                readers.Add(new EAN8Decoder());
                readers.Add(new UPCEDecoder());
            }
            this.decoders = readers.ToArray();
        }

        override public Result DecodeRow(int rowNumber,
                                BitArray row,
                                Dictionary<DecodeHintType, object> decodingOptions)
        {
            // Compute this location once and reuse it on multiple implementations
            int[] startGuardPattern = UPCEANDecoder.FindStartGuardPattern(row);
            foreach (UPCEANDecoder decoder in decoders)
            {
                Result result;
                try
                {
                    result = decoder.DecodeRow(rowNumber, row, startGuardPattern, decodingOptions);
                }
                catch (BarcodeDecoderException re)
                {
                    continue;
                }

                // Special case: a 12-digit code encoded in UPC-A is identical to a "0"
                // followed by those 12 digits encoded as EAN-13. Each will recognize such a code,
                // UPC-A as a 12-digit string and EAN-13 as a 13-digit string starting with "0".
                // Individually these are correct and their readers will both read such a code
                // and correctly call it EAN-13, or UPC-A, respectively.
                //
                // In this case, if we've been looking for both types, we'd like to call it
                // a UPC-A code. But for efficiency we only run the EAN-13 decoder to also read
                // UPC-A. So we special case it here, and convert an EAN-13 result to a UPC-A
                // result if appropriate.
                //
                // But, don't return UPC-A if UPC-A was not a requested format!
                bool ean13MayBeUPCA =
                    result.BarcodeFormat == BarcodeFormat.EAN13 &&
                        result.Text[0] == '0';
                List<BarcodeFormat> possibleFormats = (decodingOptions == null) ? null : (List<BarcodeFormat>)BarcodeHelper.GetDecodeOptionType(decodingOptions, DecodeOptions.PossibleFormats);
                bool canReturnUPCA = possibleFormats == null || possibleFormats.Contains(BarcodeFormat.UPCA);

                if (ean13MayBeUPCA && canReturnUPCA)
                {
                    // Transfer the metdata across
                    var resultUPCA = new Result(result.Text.Substring(1),
                                                   result.RawBytes,
                                                   result.ResultPoints,
                                                   BarcodeFormat.UPCA);
                    resultUPCA.PutAllMetadata(result.ResultMetadata);
                    return resultUPCA;
                }
                return result;
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