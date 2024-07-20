using MessagingToolkit.Barcode.Common;
using MessagingToolkit.Barcode.Helper;

using System;
using System.Text;
using System.Collections.Generic;

namespace MessagingToolkit.Barcode.OneD.Rss
{

    /// <summary>
    /// Decodes RSS-14, including truncated and stacked variants. See ISO/IEC 24724:2006.
    /// </summary>
    internal sealed class Rss14Decoder : AbstractRssDecoder
    {

        private static readonly int[] OutsideEvenTotalSubset = { 1, 10, 34, 70, 126 };
        private static readonly int[] InsideOddTotalSubset = { 4, 20, 48, 81 };
        private static readonly int[] OutsideGSum = { 0, 161, 961, 2015, 2715 };
        private static readonly int[] InsideGSum = { 0, 336, 1036, 1516 };
        private static readonly int[] OutsideOddWidest = { 8, 6, 4, 3, 1 };
        private static readonly int[] InsideOddWidest = { 2, 4, 6, 8 };

        private static readonly int[][] FinderPatterns = { new int[] { 3, 8, 2, 1 }, new int[] { 3, 5, 5, 1 },
				new int[] { 3, 3, 7, 1 }, new int[] { 3, 1, 9, 1 },
				new int[] { 2, 7, 4, 1 }, new int[] { 2, 5, 6, 1 },
				new int[] { 2, 3, 8, 1 }, new int[] { 1, 5, 7, 1 },
				new int[] { 1, 3, 9, 1 } };

        private readonly List<Pair> possibleLeftPairs;
        private readonly List<Pair> possibleRightPairs;

        public Rss14Decoder()
        {
            possibleLeftPairs = new List<Pair>();
            possibleRightPairs = new List<Pair>();
        }

        public override Result DecodeRow(int rowNumber, BitArray row, IDictionary<DecodeOptions, object> decodingOptions)
        {
            Pair leftPair = DecodePair(row, false, rowNumber, decodingOptions);
            AddOrTally(possibleLeftPairs, leftPair);
            row.Reverse();
            Pair rightPair = DecodePair(row, true, rowNumber, decodingOptions);
            AddOrTally(possibleRightPairs, rightPair);
            row.Reverse();
            int lefSize = possibleLeftPairs.Count;
            for (int i = 0; i < lefSize; i++)
            {
                Pair left = possibleLeftPairs[i];
                if (left.Count > 1)
                {
                    int rightSize = possibleRightPairs.Count;
                    for (int j = 0; j < rightSize; j++)
                    {
                        Pair right = possibleRightPairs[j];
                        if (right.Count > 1)
                        {
                            if (CheckChecksum(left, right))
                            {
                                return ConstructResult(left, right);
                            }
                        }
                    }
                }
            }
            throw NotFoundException.Instance;
        }

        private static void AddOrTally(List<Pair> possiblePairs, Pair pair)
        {
            if (pair == null)
            {
                return;
            }
            System.Collections.IEnumerator e = possiblePairs.GetEnumerator();
            bool found = false;
            while (e.MoveNext())
            {
                Pair other = (Pair)e.Current;
                if (other.Value == pair.Value)
                {
                    other.IncrementCount();
                    found = true;
                    break;
                }
            }
            if (!found)
            {
                possiblePairs.Add(pair);
            }
        }

        public override void Reset()
        {
            possibleLeftPairs.RemoveRange(0, possibleLeftPairs.Count - 0);
            possibleRightPairs.RemoveRange(0, possibleRightPairs.Count - 0);
        }

        private static Result ConstructResult(Pair leftPair, Pair rightPair)
        {
            long symbolValue = 4537077L * leftPair.Value + rightPair.Value;
            String text = symbolValue.ToString();

            StringBuilder buffer = new StringBuilder(14);
            for (int i = 13 - text.Length; i > 0; i--)
            {
                buffer.Append('0');
            }
            buffer.Append(text);

            int checkDigit = 0;
            for (int i = 0; i < 13; i++)
            {
                int digit = buffer[i] - '0';
                checkDigit += ((i & 0x01) == 0) ? 3 * digit : digit;
            }
            checkDigit = 10 - (checkDigit % 10);
            if (checkDigit == 10)
            {
                checkDigit = 0;
            }
            buffer.Append(checkDigit);

            ResultPoint[] leftPoints = leftPair.FinderPattern.ResultPoints;
            ResultPoint[] rightPoints = rightPair.FinderPattern.ResultPoints;
            return new Result(buffer.ToString().ToString(), null,
                    new ResultPoint[] { leftPoints[0], leftPoints[1], rightPoints[0], rightPoints[1], }, MessagingToolkit.Barcode.BarcodeFormat.RSS14);
        }

        private static bool CheckChecksum(Pair leftPair, Pair rightPair)
        {
            int leftFPValue = leftPair.FinderPattern.Value;
            int rightFPValue = rightPair.FinderPattern.Value;
            if ((leftFPValue == 0 && rightFPValue == 8)
                    || (leftFPValue == 8 && rightFPValue == 0))
            {
            }
            int checkValue = (leftPair.ChecksumPortion + 16 * rightPair.ChecksumPortion) % 79;
            int targetCheckValue = 9 * leftPair.FinderPattern.Value + rightPair.FinderPattern.Value;
            if (targetCheckValue > 72)
            {
                targetCheckValue--;
            }
            if (targetCheckValue > 8)
            {
                targetCheckValue--;
            }
            return checkValue == targetCheckValue;
        }

        private Pair DecodePair(BitArray row, bool right, int rowNumber, IDictionary<DecodeOptions, object> decodingOptions)
        {
            try
            {
                int[] startEnd = FindFinderPattern(row, 0, right);
                FinderPattern pattern = ParseFoundFinderPattern(row, rowNumber,
                        right, startEnd);

                ResultPointCallback resultPointCallback = (decodingOptions == null) ? null
                        : (ResultPointCallback)BarcodeHelper.GetDecodeOptionType(decodingOptions, DecodeOptions.NeedResultPointCallback);

                if (resultPointCallback != null)
                {
                    float center = (startEnd[0] + startEnd[1]) / 2.0f;
                    if (right)
                    {
                        // row is actually reversed
                        center = row.GetSize() - 1 - center;
                    }
                    resultPointCallback.FoundPossibleResultPoint(new ResultPoint(
                            center, rowNumber));
                }

                DataCharacter outside = DecodeDataCharacter(row, pattern, true);
                DataCharacter inside = DecodeDataCharacter(row, pattern, false);
                return new Pair(1597 * outside.Value + inside.Value, outside.ChecksumPortion + 4 * inside.ChecksumPortion, pattern);
            }
            catch (NotFoundException re)
            {
                return null;
            }
        }

        private DataCharacter DecodeDataCharacter(BitArray row,
                FinderPattern pattern, bool outsideChar)
        {

            int[] counters = dataCharacterCounters;
            counters[0] = 0;
            counters[1] = 0;
            counters[2] = 0;
            counters[3] = 0;
            counters[4] = 0;
            counters[5] = 0;
            counters[6] = 0;
            counters[7] = 0;

            if (outsideChar)
            {
                MessagingToolkit.Barcode.OneD.OneDDecoder.RecordPatternInReverse(row, pattern.StartEnd[0], counters);
            }
            else
            {
                MessagingToolkit.Barcode.OneD.OneDDecoder.RecordPattern(row, pattern.StartEnd[1] + 1, counters);
                // reverse it
                for (int i = 0, j = counters.Length - 1; i < j; i++, j--)
                {
                    int temp = counters[i];
                    counters[i] = counters[j];
                    counters[j] = temp;
                }
            }

            int numModules = (outsideChar) ? 16 : 15;
            float elementWidth = (float)MessagingToolkit.Barcode.OneD.Rss.AbstractRssDecoder.Count(counters) / (float)numModules;

            int[] oddCounts = this.oddCounts;
            int[] evenCounts = this.evenCounts;
            float[] oddRoundingErrors = this.oddRoundingErrors;
            float[] evenRoundingErrors = this.evenRoundingErrors;

            for (int i_0 = 0; i_0 < counters.Length; i_0++)
            {
                float val = (float)counters[i_0] / elementWidth;
                int count = (int)(val + 0.5f); // Round
                if (count < 1)
                {
                    count = 1;
                }
                else if (count > 8)
                {
                    count = 8;
                }
                int offset = i_0 >> 1;
                if ((i_0 & 0x01) == 0)
                {
                    oddCounts[offset] = count;
                    oddRoundingErrors[offset] = val - count;
                }
                else
                {
                    evenCounts[offset] = count;
                    evenRoundingErrors[offset] = val - count;
                }
            }

            AdjustOddEvenCounts(outsideChar, numModules);

            int oddSum = 0;
            int oddChecksumPortion = 0;
            for (int i = oddCounts.Length - 1; i >= 0; i--)
            {
                oddChecksumPortion *= 9;
                oddChecksumPortion += oddCounts[i];
                oddSum += oddCounts[i];
            }
            int evenChecksumPortion = 0;
            int evenSum = 0;
            for (int i_2 = evenCounts.Length - 1; i_2 >= 0; i_2--)
            {
                evenChecksumPortion *= 9;
                evenChecksumPortion += evenCounts[i_2];
                evenSum += evenCounts[i_2];
            }
            int checksumPortion = oddChecksumPortion + 3 * evenChecksumPortion;

            if (outsideChar)
            {
                if ((oddSum & 0x01) != 0 || oddSum > 12 || oddSum < 4)
                {
                    throw NotFoundException.Instance;
                }
                int group = (12 - oddSum) / 2;
                int oddWidest = OutsideOddWidest[group];
                int evenWidest = 9 - oddWidest;
                int vOdd = MessagingToolkit.Barcode.OneD.Rss.RssUtils.GetRssValue(oddCounts, oddWidest, false);
                int vEven = MessagingToolkit.Barcode.OneD.Rss.RssUtils.GetRssValue(evenCounts, evenWidest, true);
                int tEven = OutsideEvenTotalSubset[group];
                int gSum = OutsideGSum[group];
                return new DataCharacter(vOdd * tEven + vEven + gSum,
                        checksumPortion);
            }
            else
            {
                if ((evenSum & 0x01) != 0 || evenSum > 10 || evenSum < 4)
                {
                    throw NotFoundException.Instance;
                }
                int group_3 = (10 - evenSum) / 2;
                int oddWidest_4 = InsideOddWidest[group_3];
                int evenWidest_5 = 9 - oddWidest_4;
                int vOdd_6 = MessagingToolkit.Barcode.OneD.Rss.RssUtils.GetRssValue(oddCounts, oddWidest_4, true);
                int vEven_7 = MessagingToolkit.Barcode.OneD.Rss.RssUtils.GetRssValue(evenCounts, evenWidest_5, false);
                int tOdd = InsideOddTotalSubset[group_3];
                int gSum_8 = InsideGSum[group_3];
                return new DataCharacter(vEven_7 * tOdd + vOdd_6 + gSum_8,
                        checksumPortion);
            }

        }

        private int[] FindFinderPattern(BitArray row, int rowOffset,
                bool rightFinderPattern)
        {

            int[] counters = decodeFinderCounters;
            counters[0] = 0;
            counters[1] = 0;
            counters[2] = 0;
            counters[3] = 0;

            int width = row.GetSize();
            bool isWhite = false;
            while (rowOffset < width)
            {
                isWhite = !row.Get(rowOffset);
                if (rightFinderPattern == isWhite)
                {
                    // Will encounter white first when searching for right finder pattern
                    break;
                }
                rowOffset++;
            }

            int counterPosition = 0;
            int patternStart = rowOffset;
            for (int x = rowOffset; x < width; x++)
            {
                bool pixel = row.Get(x);
                if (pixel ^ isWhite)
                {
                    counters[counterPosition]++;
                }
                else
                {
                    if (counterPosition == 3)
                    {
                        if (MessagingToolkit.Barcode.OneD.Rss.AbstractRssDecoder.IsFinderPattern(counters))
                        {
                            return new int[] { patternStart, x };
                        }
                        patternStart += counters[0] + counters[1];
                        counters[0] = counters[2];
                        counters[1] = counters[3];
                        counters[2] = 0;
                        counters[3] = 0;
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

        private FinderPattern ParseFoundFinderPattern(BitArray row, int rowNumber,
                bool right, int[] startEnd)
        {
            // Actually we found elements 2-5
            bool firstIsBlack = row.Get(startEnd[0]);
            int firstElementStart = startEnd[0] - 1;
            // Locate element 1
            while (firstElementStart >= 0 && firstIsBlack
                    ^ row.Get(firstElementStart))
            {
                firstElementStart--;
            }
            firstElementStart++;
            int firstCounter = startEnd[0] - firstElementStart;
            // Make 'counters' hold 1-4
            int[] counters = decodeFinderCounters;
            for (int i = counters.Length - 1; i > 0; i--)
            {
                counters[i] = counters[i - 1];
            }
            counters[0] = firstCounter;
            int val = MessagingToolkit.Barcode.OneD.Rss.AbstractRssDecoder.ParseFinderValue(counters, FinderPatterns);
            int start = firstElementStart;
            int end = startEnd[1];
            if (right)
            {
                // row is actually reversed
                start = row.GetSize() - 1 - start;
                end = row.GetSize() - 1 - end;
            }
            return new FinderPattern(val, new int[] { firstElementStart,
					startEnd[1] }, start, end, rowNumber);
        }

        private void AdjustOddEvenCounts(bool outsideChar, int numModules)
        {

            int oddSum = MessagingToolkit.Barcode.OneD.Rss.AbstractRssDecoder.Count(oddCounts);
            int evenSum = MessagingToolkit.Barcode.OneD.Rss.AbstractRssDecoder.Count(evenCounts);
            int mismatch = oddSum + evenSum - numModules;
            bool oddParityBad = (oddSum & 0x01) == ((outsideChar) ? 1 : 0);
            bool evenParityBad = (evenSum & 0x01) == 1;

            bool incrementOdd = false;
            bool decrementOdd = false;
            bool incrementEven = false;
            bool decrementEven = false;

            if (outsideChar)
            {
                if (oddSum > 12)
                {
                    decrementOdd = true;
                }
                else if (oddSum < 4)
                {
                    incrementOdd = true;
                }
                if (evenSum > 12)
                {
                    decrementEven = true;
                }
                else if (evenSum < 4)
                {
                    incrementEven = true;
                }
            }
            else
            {
                if (oddSum > 11)
                {
                    decrementOdd = true;
                }
                else if (oddSum < 5)
                {
                    incrementOdd = true;
                }
                if (evenSum > 10)
                {
                    decrementEven = true;
                }
                else if (evenSum < 4)
                {
                    incrementEven = true;
                }
            }

        
            if (mismatch == 1)
            {
                if (oddParityBad)
                {
                    if (evenParityBad)
                    {
                        throw NotFoundException.Instance;
                    }
                    decrementOdd = true;
                }
                else
                {
                    if (!evenParityBad)
                    {
                        throw NotFoundException.Instance;
                    }
                    decrementEven = true;
                }
            }
            else if (mismatch == -1)
            {
                if (oddParityBad)
                {
                    if (evenParityBad)
                    {
                        throw NotFoundException.Instance;
                    }
                    incrementOdd = true;
                }
                else
                {
                    if (!evenParityBad)
                    {
                        throw NotFoundException.Instance;
                    }
                    incrementEven = true;
                }
            }
            else if (mismatch == 0)
            {
                if (oddParityBad)
                {
                    if (!evenParityBad)
                    {
                        throw NotFoundException.Instance;
                    }
                    // Both bad
                    if (oddSum < evenSum)
                    {
                        incrementOdd = true;
                        decrementEven = true;
                    }
                    else
                    {
                        decrementOdd = true;
                        incrementEven = true;
                    }
                }
                else
                {
                    if (evenParityBad)
                    {
                        throw NotFoundException.Instance;
                    }
                    // Nothing to do!
                }
            }
            else
            {
                throw NotFoundException.Instance;
            }

            if (incrementOdd)
            {
                if (decrementOdd)
                {
                    throw NotFoundException.Instance;
                }
                MessagingToolkit.Barcode.OneD.Rss.AbstractRssDecoder.Increment(oddCounts, oddRoundingErrors);
            }
            if (decrementOdd)
            {
                MessagingToolkit.Barcode.OneD.Rss.AbstractRssDecoder.Decrement(oddCounts, oddRoundingErrors);
            }
            if (incrementEven)
            {
                if (decrementEven)
                {
                    throw NotFoundException.Instance;
                }
                MessagingToolkit.Barcode.OneD.Rss.AbstractRssDecoder.Increment(evenCounts, oddRoundingErrors);
            }
            if (decrementEven)
            {
                MessagingToolkit.Barcode.OneD.Rss.AbstractRssDecoder.Decrement(evenCounts, evenRoundingErrors);
            }

        }

    }
}
