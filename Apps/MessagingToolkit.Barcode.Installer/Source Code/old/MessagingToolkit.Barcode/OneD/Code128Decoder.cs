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
using BarcodeDecoderException = MessagingToolkit.Barcode.BarcodeDecoderException;
using Result = MessagingToolkit.Barcode.Result;
using ResultPoint = MessagingToolkit.Barcode.ResultPoint;
using BitArray = MessagingToolkit.Barcode.Common.BitArray;

namespace MessagingToolkit.Barcode.OneD
{

    /// <summary>
    /// Decodes Code 128 barcodes. 
    ///
    /// Modified: April 17 2012
    /// </summary>
    public sealed class Code128Decoder : OneDDecoder
    {
        static internal readonly int[][] CODE_PATTERNS = { new int[] { 2, 1, 2, 2, 2, 2 }, new int[] { 2, 2, 2, 1, 2, 2 }, new int[] { 2, 2, 2, 2, 2, 1 }, new int[] { 1, 2, 1, 2, 2, 3 }, new int[] { 1, 2, 1, 3, 2, 2 }, new int[] { 1, 3, 1, 2, 2, 2 },
				new int[] { 1, 2, 2, 2, 1, 3 }, new int[] { 1, 2, 2, 3, 1, 2 }, new int[] { 1, 3, 2, 2, 1, 2 }, new int[] { 2, 2, 1, 2, 1, 3 }, new int[] { 2, 2, 1, 3, 1, 2 },
				new int[] { 2, 3, 1, 2, 1, 2 }, new int[] { 1, 1, 2, 2, 3, 2 }, new int[] { 1, 2, 2, 1, 3, 2 }, new int[] { 1, 2, 2, 2, 3, 1 }, new int[] { 1, 1, 3, 2, 2, 2 },
				new int[] { 1, 2, 3, 1, 2, 2 }, new int[] { 1, 2, 3, 2, 2, 1 }, new int[] { 2, 2, 3, 2, 1, 1 }, new int[] { 2, 2, 1, 1, 3, 2 }, new int[] { 2, 2, 1, 2, 3, 1 },
				new int[] { 2, 1, 3, 2, 1, 2 }, new int[] { 2, 2, 3, 1, 1, 2 }, new int[] { 3, 1, 2, 1, 3, 1 }, new int[] { 3, 1, 1, 2, 2, 2 }, new int[] { 3, 2, 1, 1, 2, 2 },
				new int[] { 3, 2, 1, 2, 2, 1 }, new int[] { 3, 1, 2, 2, 1, 2 }, new int[] { 3, 2, 2, 1, 1, 2 }, new int[] { 3, 2, 2, 2, 1, 1 }, new int[] { 2, 1, 2, 1, 2, 3 },
				new int[] { 2, 1, 2, 3, 2, 1 }, new int[] { 2, 3, 2, 1, 2, 1 }, new int[] { 1, 1, 1, 3, 2, 3 }, new int[] { 1, 3, 1, 1, 2, 3 }, new int[] { 1, 3, 1, 3, 2, 1 },
				new int[] { 1, 1, 2, 3, 1, 3 }, new int[] { 1, 3, 2, 1, 1, 3 }, new int[] { 1, 3, 2, 3, 1, 1 }, new int[] { 2, 1, 1, 3, 1, 3 }, new int[] { 2, 3, 1, 1, 1, 3 },
				new int[] { 2, 3, 1, 3, 1, 1 }, new int[] { 1, 1, 2, 1, 3, 3 }, new int[] { 1, 1, 2, 3, 3, 1 }, new int[] { 1, 3, 2, 1, 3, 1 }, new int[] { 1, 1, 3, 1, 2, 3 },
				new int[] { 1, 1, 3, 3, 2, 1 }, new int[] { 1, 3, 3, 1, 2, 1 }, new int[] { 3, 1, 3, 1, 2, 1 }, new int[] { 2, 1, 1, 3, 3, 1 }, new int[] { 2, 3, 1, 1, 3, 1 },
				new int[] { 2, 1, 3, 1, 1, 3 }, new int[] { 2, 1, 3, 3, 1, 1 }, new int[] { 2, 1, 3, 1, 3, 1 }, new int[] { 3, 1, 1, 1, 2, 3 }, new int[] { 3, 1, 1, 3, 2, 1 },
				new int[] { 3, 3, 1, 1, 2, 1 }, new int[] { 3, 1, 2, 1, 1, 3 }, new int[] { 3, 1, 2, 3, 1, 1 }, new int[] { 3, 3, 2, 1, 1, 1 }, new int[] { 3, 1, 4, 1, 1, 1 },
				new int[] { 2, 2, 1, 4, 1, 1 }, new int[] { 4, 3, 1, 1, 1, 1 }, new int[] { 1, 1, 1, 2, 2, 4 }, new int[] { 1, 1, 1, 4, 2, 2 }, new int[] { 1, 2, 1, 1, 2, 4 },
				new int[] { 1, 2, 1, 4, 2, 1 }, new int[] { 1, 4, 1, 1, 2, 2 }, new int[] { 1, 4, 1, 2, 2, 1 }, new int[] { 1, 1, 2, 2, 1, 4 }, new int[] { 1, 1, 2, 4, 1, 2 },
				new int[] { 1, 2, 2, 1, 1, 4 }, new int[] { 1, 2, 2, 4, 1, 1 }, new int[] { 1, 4, 2, 1, 1, 2 }, new int[] { 1, 4, 2, 2, 1, 1 }, new int[] { 2, 4, 1, 2, 1, 1 },
				new int[] { 2, 2, 1, 1, 1, 4 }, new int[] { 4, 1, 3, 1, 1, 1 }, new int[] { 2, 4, 1, 1, 1, 2 }, new int[] { 1, 3, 4, 1, 1, 1 }, new int[] { 1, 1, 1, 2, 4, 2 },
				new int[] { 1, 2, 1, 1, 4, 2 }, new int[] { 1, 2, 1, 2, 4, 1 }, new int[] { 1, 1, 4, 2, 1, 2 }, new int[] { 1, 2, 4, 1, 1, 2 }, new int[] { 1, 2, 4, 2, 1, 1 },
				new int[] { 4, 1, 1, 2, 1, 2 }, new int[] { 4, 2, 1, 1, 1, 2 }, new int[] { 4, 2, 1, 2, 1, 1 }, new int[] { 2, 1, 2, 1, 4, 1 }, new int[] { 2, 1, 4, 1, 2, 1 },
				new int[] { 4, 1, 2, 1, 2, 1 }, new int[] { 1, 1, 1, 1, 4, 3 }, new int[] { 1, 1, 1, 3, 4, 1 }, new int[] { 1, 3, 1, 1, 4, 1 }, new int[] { 1, 1, 4, 1, 1, 3 },
				new int[] { 1, 1, 4, 3, 1, 1 }, new int[] { 4, 1, 1, 1, 1, 3 }, new int[] { 4, 1, 1, 3, 1, 1 }, new int[] { 1, 1, 3, 1, 4, 1 }, new int[] { 1, 1, 4, 1, 3, 1 },
				new int[] { 3, 1, 1, 1, 4, 1 }, new int[] { 4, 1, 1, 1, 3, 1 }, new int[] { 2, 1, 1, 4, 1, 2 }, new int[] { 2, 1, 1, 2, 1, 4 }, new int[] { 2, 1, 1, 2, 3, 2 },
				new int[] { 2, 3, 3, 1, 1, 1, 2 } };

        private const int MAX_AVG_VARIANCE = (int)(PatternMatchResultScaleFactor * 0.25f);
        private const int MAX_INDIVIDUAL_VARIANCE = (int)(PatternMatchResultScaleFactor * 0.7f);

        private const int CODE_SHIFT = 98;

        private const int CODE_CODE_C = 99;
        private const int CODE_CODE_B = 100;
        private const int CODE_CODE_A = 101;

        private const int CODE_FNC_1 = 102;
        private const int CODE_FNC_2 = 97;
        private const int CODE_FNC_3 = 96;
        private const int CODE_FNC_4_A = 101;
        private const int CODE_FNC_4_B = 100;

        private const int CODE_START_A = 103;
        private const int CODE_START_B = 104;
        private const int CODE_START_C = 105;
        private const int CODE_STOP = 106;

        private static int[] FindStartPattern(BitArray row)
        {
            int width = row.GetSize();
            int rowOffset = row.GetNextSet(0);

            int counterPosition = 0;
            int[] counters = new int[6];
            int patternStart = rowOffset;
            bool isWhite = false;
            int patternLength = counters.Length;

            for (int i = rowOffset; i < width; i++)
            {
                if (row.Get(i) ^ isWhite)
                {
                    counters[counterPosition]++;
                }
                else
                {
                    if (counterPosition == patternLength - 1)
                    {
                        int bestVariance = MAX_AVG_VARIANCE;
                        int bestMatch = -1;
                        for (int startCode = CODE_START_A; startCode <= CODE_START_C; startCode++)
                        {
                            int variance = MessagingToolkit.Barcode.OneD.OneDDecoder.PatternMatchVariance(counters, CODE_PATTERNS[startCode], MAX_INDIVIDUAL_VARIANCE);
                            if (variance < bestVariance)
                            {
                                bestVariance = variance;
                                bestMatch = startCode;
                            }
                        }
                        // Look for whitespace before start pattern, >= 50% of width of start pattern
                        if (bestMatch >= 0 && row.IsRange(Math.Max(0, patternStart - (i - patternStart) / 2), patternStart, false))
                        {
                            return new int[] { patternStart, i, bestMatch };
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

        private static int DecodeCode(BitArray row, int[] counters, int rowOffset)
        {
            MessagingToolkit.Barcode.OneD.OneDDecoder.RecordPattern(row, rowOffset, counters);
            int bestVariance = MAX_AVG_VARIANCE; // worst variance we'll accept
            int bestMatch = -1;
            for (int d = 0; d < CODE_PATTERNS.Length; d++)
            {
                int[] pattern = CODE_PATTERNS[d];
                int variance = MessagingToolkit.Barcode.OneD.OneDDecoder.PatternMatchVariance(counters, pattern, MAX_INDIVIDUAL_VARIANCE);
                if (variance < bestVariance)
                {
                    bestVariance = variance;
                    bestMatch = d;
                }
            }
            // TODO We're overlooking the fact that the STOP pattern has 7 values, not 6.
            if (bestMatch >= 0)
            {
                return bestMatch;
            }
            else
            {
                throw NotFoundException.Instance;
            }
        }

        public override Result DecodeRow(int rowNumber, BitArray row, Dictionary<DecodeOptions, object> decodingOptions)
        {

            int[] startPatternInfo = FindStartPattern(row);
            int startCode = startPatternInfo[2];
            int codeSet;
            switch (startCode)
            {
                case CODE_START_A:
                    codeSet = CODE_CODE_A;
                    break;
                case CODE_START_B:
                    codeSet = CODE_CODE_B;
                    break;
                case CODE_START_C:
                    codeSet = CODE_CODE_C;
                    break;
                default:
                    throw FormatException.Instance;
            }

            bool done = false;
            bool isNextShifted = false;

            StringBuilder result = new StringBuilder(20);
            IList<Byte> rawCodes = new List<Byte>(20);

            int lastStart = startPatternInfo[0];
            int nextStart = startPatternInfo[1];
            int[] counters = new int[6];

            int lastCode = 0;
            int code = 0;
            int checksumTotal = startCode;
            int multiplier = 0;
            bool lastCharacterWasPrintable = true;

            while (!done)
            {

                bool unshift = isNextShifted;
                isNextShifted = false;

                // Save off last code
                lastCode = code;

                // Decode another code from image
                code = DecodeCode(row, counters, nextStart);

                rawCodes.Add((byte)code);

                // Remember whether the last code was printable or not (excluding CODE_STOP)
                if (code != CODE_STOP)
                {
                    lastCharacterWasPrintable = true;
                }

                // Add to checksum computation (if not CODE_STOP of course)
                if (code != CODE_STOP)
                {
                    multiplier++;
                    checksumTotal += multiplier * code;
                }

                // Advance to where the next code will to start
                lastStart = nextStart;
                /* foreach */
                foreach (int counter in counters)
                {
                    nextStart += counter;
                }

                // Take care of illegal start codes
                switch (code)
                {
                    case CODE_START_A:
                    case CODE_START_B:
                    case CODE_START_C:
                        throw FormatException.Instance;
                }

                switch (codeSet)
                {

                    case CODE_CODE_A:
                        if (code < 64)
                        {
                            result.Append((char)(' ' + code));
                        }
                        else if (code < 96)
                        {
                            result.Append((char)(code - 64));
                        }
                        else
                        {
                            // Don't let CODE_STOP, which always appears, affect whether whether we think the last
                            // code was printable or not.
                            if (code != CODE_STOP)
                            {
                                lastCharacterWasPrintable = false;
                            }
                            switch (code)
                            {
                                case CODE_FNC_1:
                                case CODE_FNC_2:
                                case CODE_FNC_3:
                                case CODE_FNC_4_A:
                                    // do nothing?
                                    break;
                                case CODE_SHIFT:
                                    isNextShifted = true;
                                    codeSet = CODE_CODE_B;
                                    break;
                                case CODE_CODE_B:
                                    codeSet = CODE_CODE_B;
                                    break;
                                case CODE_CODE_C:
                                    codeSet = CODE_CODE_C;
                                    break;
                                case CODE_STOP:
                                    done = true;
                                    break;
                            }
                        }
                        break;
                    case CODE_CODE_B:
                        if (code < 96)
                        {
                            result.Append((char)(' ' + code));
                        }
                        else
                        {
                            if (code != CODE_STOP)
                            {
                                lastCharacterWasPrintable = false;
                            }
                            switch (code)
                            {
                                case CODE_FNC_1:
                                case CODE_FNC_2:
                                case CODE_FNC_3:
                                case CODE_FNC_4_B:
                                    // do nothing?
                                    break;
                                case CODE_SHIFT:
                                    isNextShifted = true;
                                    codeSet = CODE_CODE_A;
                                    break;
                                case CODE_CODE_A:
                                    codeSet = CODE_CODE_A;
                                    break;
                                case CODE_CODE_C:
                                    codeSet = CODE_CODE_C;
                                    break;
                                case CODE_STOP:
                                    done = true;
                                    break;
                            }
                        }
                        break;
                    case CODE_CODE_C:
                        if (code < 100)
                        {
                            if (code < 10)
                            {
                                result.Append('0');
                            }
                            result.Append(code);
                        }
                        else
                        {
                            if (code != CODE_STOP)
                            {
                                lastCharacterWasPrintable = false;
                            }
                            switch (code)
                            {
                                case CODE_FNC_1:
                                    // do nothing?
                                    break;
                                case CODE_CODE_A:
                                    codeSet = CODE_CODE_A;
                                    break;
                                case CODE_CODE_B:
                                    codeSet = CODE_CODE_B;
                                    break;
                                case CODE_STOP:
                                    done = true;
                                    break;
                            }
                        }
                        break;
                }

                // Unshift back to another code set if we were shifted
                if (unshift)
                {
                    codeSet = (codeSet == CODE_CODE_A) ? CODE_CODE_B : CODE_CODE_A;
                }

            }

            // Check for ample whitespace following pattern, but, to do this we first need to remember that
            // we fudged decoding CODE_STOP since it actually has 7 bars, not 6. There is a black bar left
            // to read off. Would be slightly better to properly read. Here we just skip it:
            nextStart = row.GetNextUnset(nextStart);
            if (!row.IsRange(nextStart, Math.Min(row.GetSize(), nextStart + (nextStart - lastStart) / 2), false))
            {
                throw NotFoundException.Instance;
            }

            // Pull out from sum the value of the penultimate check code
            checksumTotal -= multiplier * lastCode;
            // lastCode is the checksum then:
            if (checksumTotal % 103 != lastCode)
            {
                throw ChecksumException.Instance;
            }

            // Need to pull out the check digits from string
            int resultLength = result.Length;
            if (resultLength == 0)
            {
                // false positive
                throw NotFoundException.Instance;
            }

            // Only bother if the result had at least one character, and if the checksum digit happened to
            // be a printable character. If it was just interpreted as a control code, nothing to remove.
            if (resultLength > 0 && lastCharacterWasPrintable)
            {
                if (codeSet == CODE_CODE_C)
                {
                    result.Remove(resultLength - 2, 2);
                }
                else
                {
                    result.Remove(resultLength - 1,  1);
                }
            }

            float left = (float)(startPatternInfo[1] + startPatternInfo[0]) / 2.0f;
            float right = (float)(nextStart + lastStart) / 2.0f;

            var resultPointCallback = decodingOptions == null || !decodingOptions.ContainsKey(DecodeOptions.NeedResultPointCallback)
                                  ? null
                                  : (ResultPointCallback)decodingOptions[DecodeOptions.NeedResultPointCallback];
            if (resultPointCallback != null)
            {
                resultPointCallback.FoundPossibleResultPoint(new ResultPoint(left, rowNumber));
                resultPointCallback.FoundPossibleResultPoint(new ResultPoint(right, rowNumber));
            }
            int rawCodesSize = rawCodes.Count;
            byte[] rawBytes = new byte[rawCodesSize];
            for (int i = 0; i < rawCodesSize; i++)
            {
                rawBytes[i] = (rawCodes[i]);
            }

            return new Result(result.ToString(), rawBytes, new ResultPoint[] { new ResultPoint(left, (float)rowNumber), new ResultPoint(right, (float)rowNumber) }, BarcodeFormat.Code128);

        }
    }
}