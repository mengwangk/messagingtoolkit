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

using MessagingToolkit.Barcode.OneD.Rss;
using MessagingToolkit.Barcode.OneD.Rss.Expanded;
using MessagingToolkit.Barcode.Helper;

namespace MessagingToolkit.Barcode.OneD
{

    /// <summary>
    /// Modified April 30 2012
    /// </summary>
    public sealed class BarcodeOneDDecoder : OneDDecoder
    {
        private readonly List<OneDDecoder> decoders;

        public BarcodeOneDDecoder(Dictionary<DecodeOptions, object> decodingOptions)
        {
            List<BarcodeFormat> possibleFormats = decodingOptions == null ? null : (List<BarcodeFormat>)BarcodeHelper.GetDecodeOptionType(decodingOptions, DecodeOptions.PossibleFormats);
            bool useCode39CheckDigit = decodingOptions != null && BarcodeHelper.GetDecodeOptionType(decodingOptions, DecodeOptions.AssumeCode39CheckDigit) != null;
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
                decoders.Add(new Code39Decoder());
                decoders.Add(new CodaBarDecoder());
                decoders.Add(new Code93Decoder());
                decoders.Add(new Code128Decoder());
                decoders.Add(new ITFDecoder());
                decoders.Add(new Rss14Decoder());
                decoders.Add(new RssExpandedDecoder());
            }
        }

        override public Result DecodeRow(int rowNumber,
                                BitArray row,
                                Dictionary<DecodeOptions, object> decodingOptions)
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