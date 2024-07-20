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
using System.Text;
using System.Collections.Generic;

using BarcodeFormat = MessagingToolkit.Barcode.BarcodeFormat;
using DecodeHintType = MessagingToolkit.Barcode.DecodeOptions;
using BarcodeDecoderException = MessagingToolkit.Barcode.BarcodeDecoderException;
using Result = MessagingToolkit.Barcode.Result;
using ResultPoint = MessagingToolkit.Barcode.ResultPoint;
using BitArray = MessagingToolkit.Barcode.Common.BitArray;
using MessagingToolkit.Barcode.Helper;

namespace MessagingToolkit.Barcode.OneD
{

    /// <summary>
    /// 	<p>Implements decoding of the ITF format.</p>
    /// 	<p>"ITF" stands for Interleaved Two of Five. This Reader will scan ITF barcode with 6, 10 or 14
    /// digits. The checksum is optional and is not applied by this Reader. The consumer of the decoded
    /// value will have to apply a checksum if required.</p>
    /// 	<p><a href="http://en.wikipedia.org/wiki/Interleaved_2_of_5">http://en.wikipedia.org/wiki/Interleaved_2_of_5</a>
    /// is a great reference for Interleaved 2 of 5 information.</p>
    /// 
    /// Modified: April 30 2012
    /// </summary>
    public sealed class ITFDecoder : OneDDecoder
    {
        public ITFDecoder()
        {
            this.narrowLineWidth = -1;
        }

        private const int MAX_AVG_VARIANCE = (int)(OneDDecoder.PatternMatchResultScaleFactor * 0.42f);
        private const int MAX_INDIVIDUAL_VARIANCE = (int)(OneDDecoder.PatternMatchResultScaleFactor * 0.8f);

        private const int W = 3; // Pixel width of a wide line
        private const int N = 1; // Pixed width of a narrow line

        private static readonly int[] DEFAULT_ALLOWED_LENGTHS = { 44, 24, 20, 18, 16, 14, 12, 10, 8, 6 };

        // Stores the actual narrow line width of the image being decoded.
        private int narrowLineWidth;

        /// <summary>
        /// Start/end guard pattern.
        /// Note: The end pattern is reversed because the row is reversed before
        /// searching for the END_PATTERN
        /// </summary>
        private static readonly int[] START_PATTERN = { N, N, N, N };
        private static readonly int[] END_PATTERN_REVERSED = { N, N, W };

        /// <summary>
        /// Patterns of Wide / Narrow lines to indicate each digit
        /// </summary>
        ///
        static internal readonly int[][] PATTERNS = { new int[] { N, N, W, W, N }, new int[] { W, N, N, N, W }, new int[] { N, W, N, N, W }, new int[] { W, W, N, N, N }, new int[] { N, N, W, N, W }, new int[] { W, N, W, N, N },
				new int[] { N, W, W, N, N }, new int[] { N, N, N, W, W }, new int[] { W, N, N, W, N }, new int[] { N, W, N, W, N } };

        public override Result DecodeRow(int rowNumber, BitArray row, Dictionary<DecodeOptions, object> decodingOptions)
        {

            // Find out where the Middle section (payload) starts & ends
            int[] startRange = DecodeStart(row);
            int[] endRange = DecodeEnd(row);

            StringBuilder result = new StringBuilder(20);
            DecodeMiddle(row, startRange[1], endRange[0], result);
            String resultString = result.ToString();

            int[] allowedLengths = null;
            if (decodingOptions != null)
            {
                allowedLengths = (int[])BarcodeHelper.GetDecodeOptionType(decodingOptions, DecodeOptions.AllowedLengths);

            }
            if (allowedLengths == null)
            {
                allowedLengths = DEFAULT_ALLOWED_LENGTHS;
            }

            // To avoid false positives with 2D barcodes (and other patterns), make
            // an assumption that the decoded string must be 6, 10 or 14 digits.
            int length = resultString.Length;
            bool lengthOK = false;
            /* foreach */
            foreach (int allowedLength in allowedLengths)
            {
                if (length == allowedLength)
                {
                    lengthOK = true;
                    break;
                }
            }
            if (!lengthOK)
            {
                throw FormatException.Instance;
            }


            var resultPointCallback = decodingOptions == null || !decodingOptions.ContainsKey(DecodeOptions.NeedResultPointCallback)
                                          ? null
                                          : (ResultPointCallback)decodingOptions[DecodeOptions.NeedResultPointCallback];
            if (resultPointCallback != null)
            {
                resultPointCallback.FoundPossibleResultPoint(new ResultPoint(startRange[1], rowNumber));
                resultPointCallback.FoundPossibleResultPoint(new ResultPoint(endRange[0], rowNumber));
            }

            return new Result(resultString, null, // no natural byte representation for these barcodes
                    new ResultPoint[] { new ResultPoint(startRange[1], rowNumber), new ResultPoint(endRange[0], rowNumber) }, BarcodeFormat.ITF14);
        }


        /// <param name="row">row of black/white values to search</param>
        /// <param name="payloadStart">offset of start pattern</param>
        /// <param name="resultString">to append decoded chars to</param>
        /// <exception cref="NotFoundException">if decoding could not complete successfully</exception>
        private static void DecodeMiddle(BitArray row, int payloadStart, int payloadEnd, StringBuilder resultString)
        {

            // Digits are interleaved in pairs - 5 black lines for one digit, and the
            // 5
            // interleaved white lines for the second digit.
            // Therefore, need to scan 10 lines and then
            // split these into two arrays
            int[] counterDigitPair = new int[10];
            int[] counterBlack = new int[5];
            int[] counterWhite = new int[5];

            while (payloadStart < payloadEnd)
            {

                // Get 10 runs of black/white.
                RecordPattern(row, payloadStart, counterDigitPair);
                // Split them into each array
                for (int k = 0; k < 5; k++)
                {
                    int twoK = k << 1;
                    counterBlack[k] = counterDigitPair[twoK];
                    counterWhite[k] = counterDigitPair[twoK + 1];
                }

                int bestMatch = DecodeDigit(counterBlack);
                resultString.Append((char)('0' + bestMatch));
                bestMatch = DecodeDigit(counterWhite);
                resultString.Append((char)('0' + bestMatch));

                /* foreach */
                foreach (int counterDigit in counterDigitPair)
                {
                    payloadStart += counterDigit;
                }
            }
        }

        /// <summary>
        /// Identify where the start of the middle / payload section starts.
        /// </summary>
        ///
        /// <param name="row">row of black/white values to search</param>
        /// <returns>Array, containing index of start of 'start block' and end of
        /// 'start block'</returns>
        /// <exception cref="NotFoundException"></exception>
        internal int[] DecodeStart(BitArray row)
        {
            int endStart = SkipWhiteSpace(row);
            int[] startPattern = FindGuardPattern(row, endStart, START_PATTERN);

            // Determine the width of a narrow line in pixels. We can do this by
            // getting the width of the start pattern and dividing by 4 because its
            // made up of 4 narrow lines.
            this.narrowLineWidth = (startPattern[1] - startPattern[0]) >> 2;

            ValidateQuietZone(row, startPattern[0]);

            return startPattern;
        }

        /// <summary>
        /// The start & end patterns must be pre/post fixed by a quiet zone. This
        /// zone must be at least 10 times the width of a narrow line.  Scan back until
        /// we either get to the start of the barcode or match the necessary number of
        /// quiet zone pixels.
        /// Note: Its assumed the row is reversed when using this method to find
        /// quiet zone after the end pattern.
        /// ref: http://www.barcode-1.net/i25code.html
        /// </summary>
        ///
        /// <param name="row">bit array representing the scanned barcode.</param>
        /// <param name="startPattern">index into row of the start or end pattern.</param>
        /// <exception cref="NotFoundException">if the quiet zone cannot be found, a ReaderException is thrown.</exception>
        private void ValidateQuietZone(BitArray row, int startPattern)
        {

            int quietCount = this.narrowLineWidth * 10; // expect to find this many pixels of quiet zone

            for (int i = startPattern - 1; quietCount > 0 && i >= 0; i--)
            {
                if (row.Get(i))
                {
                    break;
                }
                quietCount--;
            }
            if (quietCount != 0)
            {
                // Unable to find the necessary number of quiet zone pixels.
                throw NotFoundException.Instance;
            }
        }

        /// <summary>
        /// Skip all whitespace until we get to the first black line.
        /// </summary>
        ///
        /// <param name="row">row of black/white values to search</param>
        /// <returns>index of the first black line.</returns>
        /// <exception cref="NotFoundException">Throws exception if no black lines are found in the row</exception>
        private static int SkipWhiteSpace(BitArray row)
        {
            int width = row.GetSize();
            int endStart = row.GetNextSet(0);
            if (endStart == width)
            {
                throw NotFoundException.Instance;
            }

            return endStart;
        }

        /// <summary>
        /// Identify where the end of the middle / payload section ends.
        /// </summary>
        ///
        /// <param name="row">row of black/white values to search</param>
        /// <returns>Array, containing index of start of 'end block' and end of 'end
        /// block'</returns>
        /// <exception cref="NotFoundException"></exception>
        internal int[] DecodeEnd(BitArray row)
        {

            // For convenience, reverse the row and then
            // search from 'the start' for the end block
            row.Reverse();
            try
            {
                int endStart = SkipWhiteSpace(row);
                int[] endPattern = FindGuardPattern(row, endStart, END_PATTERN_REVERSED);

                // The start & end patterns must be pre/post fixed by a quiet zone. This
                // zone must be at least 10 times the width of a narrow line.
                // ref: http://www.barcode-1.net/i25code.html
                ValidateQuietZone(row, endPattern[0]);

                // Now recalculate the indices of where the 'endblock' starts & stops to
                // accommodate
                // the reversed nature of the search
                int temp = endPattern[0];
                endPattern[0] = row.GetSize() - endPattern[1];
                endPattern[1] = row.GetSize() - temp;

                return endPattern;
            }
            finally
            {
                // Put the row back the right way.
                row.Reverse();
            }
        }


        /// <param name="row">row of black/white values to search</param>
        /// <param name="rowOffset">position to start search</param>
        /// <param name="pattern"></param>
        /// <returns>start/end horizontal offset of guard pattern, as an array of two
        /// ints</returns>
        /// <exception cref="NotFoundException">if pattern is not found</exception>
        private static int[] FindGuardPattern(BitArray row, int rowOffset, int[] pattern)
        {

            // TODO: This is very similar to implementation in UPCEANReader. Consider if they can be
            // merged to a single method.
            int patternLength = pattern.Length;
            int[] counters = new int[patternLength];
            int width = row.GetSize();
            bool isWhite = false;

            int counterPosition = 0;
            int patternStart = rowOffset;
            for (int x = rowOffset; x < width; x++)
            {
                if (row.Get(x) ^ isWhite)
                {
                    counters[counterPosition]++;
                }
                else
                {
                    if (counterPosition == patternLength - 1)
                    {
                        if (PatternMatchVariance(counters, pattern, MAX_INDIVIDUAL_VARIANCE) < MAX_AVG_VARIANCE)
                        {
                            return new int[] { patternStart, x };
                        }
                        patternStart += counters[0] + counters[1];
                        System.Array.Copy((Array)(counters), 2, (Array)(counters), 0, patternLength - 2);
                        counters[patternLength - 2] = 0;
                        counters[patternLength - 1] = 0;
                        counterPosition--;
                    }
                    else
                    {
                        counterPosition++;
                    }
                    counters[counterPosition] = 1;
                    isWhite = !isWhite;
                }
            }
            throw NotFoundException.Instance;
        }

        /// <summary>
        /// Attempts to decode a sequence of ITF black/white lines into single
        /// digit.
        /// </summary>
        ///
        /// <param name="counters">the counts of runs of observed black/white/black/... values</param>
        /// <returns>The decoded digit</returns>
        /// <exception cref="NotFoundException">if digit cannot be decoded</exception>
        private static int DecodeDigit(int[] counters)
        {

            int bestVariance = MAX_AVG_VARIANCE; // worst variance we'll accept
            int bestMatch = -1;
            int max = PATTERNS.Length;
            for (int i = 0; i < max; i++)
            {
                int[] pattern = PATTERNS[i];
                int variance = PatternMatchVariance(counters, pattern, MAX_INDIVIDUAL_VARIANCE);
                if (variance < bestVariance)
                {
                    bestVariance = variance;
                    bestMatch = i;
                }
            }
            if (bestMatch >= 0)
            {
                return bestMatch;
            }
            else
            {
                throw NotFoundException.Instance;
            }
        }

    }
}