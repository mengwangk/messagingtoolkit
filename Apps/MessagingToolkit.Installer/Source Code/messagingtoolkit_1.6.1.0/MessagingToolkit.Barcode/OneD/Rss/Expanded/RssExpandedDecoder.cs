using MessagingToolkit.Barcode.Common;
using MessagingToolkit.Barcode.OneD.Rss.Expanded.Decoders;
using System;
using System.Collections.Generic;

namespace MessagingToolkit.Barcode.OneD.Rss.Expanded
{

    public sealed class RssExpandedDecoder : AbstractRssDecoder
    {

        public RssExpandedDecoder()
        {
            this.pairs = new List<ExpandedPair>(MaxPairs);
            this.startEnd = new int[2];
            this.currentSequence = new int[LongestSequenceSize];
        }

        private static readonly int[] SymbolWidest = { 7, 5, 4, 3, 1 };
        private static readonly int[] EventTotalSubset = { 4, 20, 52, 104, 204 };
        private static readonly int[] GSum = { 0, 348, 1388, 2948, 3988 };

        private static readonly int[][] FinderPatterns = { new int[] { 1, 8, 4, 1 }, new int[] { 3, 6, 4, 1 },
				new int[] { 3, 4, 6, 1 }, new int[] { 3, 2, 8, 1 },
				new int[] { 2, 6, 5, 1 }, new int[] { 2, 2, 9, 1 } };

        private static readonly int[][] Weights = { new int[] { 1, 3, 9, 27, 81, 32, 96, 77 },
				new int[] { 20, 60, 180, 118, 143, 7, 21, 63 },
				new int[] { 189, 145, 13, 39, 117, 140, 209, 205 },
				new int[] { 193, 157, 49, 147, 19, 57, 171, 91 },
				new int[] { 62, 186, 136, 197, 169, 85, 44, 132 },
				new int[] { 185, 133, 188, 142, 4, 12, 36, 108 },
				new int[] { 113, 128, 173, 97, 80, 29, 87, 50 },
				new int[] { 150, 28, 84, 41, 123, 158, 52, 156 },
				new int[] { 46, 138, 203, 187, 139, 206, 196, 166 },
				new int[] { 76, 17, 51, 153, 37, 111, 122, 155 },
				new int[] { 43, 129, 176, 106, 107, 110, 119, 146 },
				new int[] { 16, 48, 144, 10, 30, 90, 59, 177 },
				new int[] { 109, 116, 137, 200, 178, 112, 125, 164 },
				new int[] { 70, 210, 208, 202, 184, 130, 179, 115 },
				new int[] { 134, 191, 151, 31, 93, 68, 204, 190 },
				new int[] { 148, 22, 66, 198, 172, 94, 71, 2 },
				new int[] { 6, 18, 54, 162, 64, 192, 154, 40 },
				new int[] { 120, 149, 25, 75, 14, 42, 126, 167 },
				new int[] { 79, 26, 78, 23, 69, 207, 199, 175 },
				new int[] { 103, 98, 83, 38, 114, 131, 182, 124 },
				new int[] { 161, 61, 183, 127, 170, 88, 53, 159 },
				new int[] { 55, 165, 73, 8, 24, 72, 5, 15 },
				new int[] { 45, 135, 194, 160, 58, 174, 100, 89 } };

        private const int FinderPatA = 0;
        private const int FinderPatB = 1;
        private const int FinderPatC = 2;
        private const int FinderPatD = 3;
        private const int FinderPatE = 4;
        private const int FinderPatF = 5;

        private static readonly int[][] FinderPatternSequences = {
				new int[] { FinderPatA, FinderPatA },
				new int[] { FinderPatA, FinderPatB, FinderPatB },
				new int[] { FinderPatA, FinderPatC, FinderPatB, FinderPatD },
				new int[] { FinderPatA, FinderPatE, FinderPatB, FinderPatD,
						FinderPatC },
				new int[] { FinderPatA, FinderPatE, FinderPatB, FinderPatD,
						FinderPatD, FinderPatF },
				new int[] { FinderPatA, FinderPatE, FinderPatB, FinderPatD,
						FinderPatE, FinderPatF, FinderPatF },
				new int[] { FinderPatA, FinderPatA, FinderPatB, FinderPatB,
						FinderPatC, FinderPatC, FinderPatD, FinderPatD },
				new int[] { FinderPatA, FinderPatA, FinderPatB, FinderPatB,
						FinderPatC, FinderPatC, FinderPatD, FinderPatE,
						FinderPatE },
				new int[] { FinderPatA, FinderPatA, FinderPatB, FinderPatB,
						FinderPatC, FinderPatC, FinderPatD, FinderPatE,
						FinderPatF, FinderPatF },
				new int[] { FinderPatA, FinderPatA, FinderPatB, FinderPatB,
						FinderPatC, FinderPatD, FinderPatD, FinderPatE,
						FinderPatE, FinderPatF, FinderPatF } };

        private static readonly int LongestSequenceSize = FinderPatternSequences[FinderPatternSequences.Length - 1].Length;

        private const int MaxPairs = 11;
        private readonly List<ExpandedPair> pairs;
        private readonly int[] startEnd;
        private readonly int[] currentSequence;

        public override Result DecodeRow(int rowNumber, BitArray row, Dictionary<DecodeOptions, object> decodingOptions)
        {
            this.Reset();
            DecodeRow2pairs(rowNumber, row);
            return ConstructResult(this.pairs);
        }

        public override void Reset()
        {
            this.pairs.RemoveRange(0, this.pairs.Count - 0);
        }

        // Not private for testing
        internal List<ExpandedPair> DecodeRow2pairs(int rowNumber, BitArray row)
        {
            while (true)
            {
                ExpandedPair nextPair = RetrieveNextPair(row, this.pairs, rowNumber);
                this.pairs.Add(nextPair);

                if (nextPair.MayBeLast)
                {
                    if (CheckChecksum())
                    {
                        return this.pairs;
                    }
                    if (nextPair.MustBeLast)
                    {
                        throw NotFoundException.Instance;
                    }
                }
            }
        }

        private static Result ConstructResult(List<ExpandedPair> pairs_0)
        {
            BitArray binary = MessagingToolkit.Barcode.OneD.Rss.Expanded.BitArrayBuilder.BuildBitArray(pairs_0);

            AbstractExpandedDecoder decoder = MessagingToolkit.Barcode.OneD.Rss.Expanded.Decoders.AbstractExpandedDecoder.CreateDecoder(binary);
            String resultingString = decoder.ParseInformation();

            ResultPoint[] firstPoints = ((ExpandedPair)pairs_0[0]).FinderPattern.ResultPoints;
            ResultPoint[] lastPoints = ((ExpandedPair)pairs_0[pairs_0.Count - 1]).FinderPattern.ResultPoints;

            return new Result(resultingString, null, new ResultPoint[] {
					firstPoints[0], firstPoints[1], lastPoints[0], lastPoints[1] },
                    MessagingToolkit.Barcode.BarcodeFormat.RSSExpanded);
        }

        private bool CheckChecksum()
        {
            ExpandedPair firstPair = (ExpandedPair)this.pairs[0];
            DataCharacter checkCharacter = firstPair.LeftChar;
            DataCharacter firstCharacter = firstPair.RightChar;

            int checksum = firstCharacter.ChecksumPortion;
            int S = 2;

            for (int i = 1; i < this.pairs.Count; ++i)
            {
                ExpandedPair currentPair = (ExpandedPair)this.pairs[i];
                checksum += currentPair.LeftChar.ChecksumPortion;
                S++;
                if (currentPair.RightChar != null)
                {
                    checksum += currentPair.RightChar.ChecksumPortion;
                    S++;
                }
            }

            checksum %= 211;

            int checkCharacterValue = 211 * (S - 4) + checksum;

            return checkCharacterValue == checkCharacter.Value;
        }

        private static int GetNextSecondBar(BitArray row, int initialPos)
        {
            int currentPos;
            if (row.Get(initialPos))
            {
                currentPos = row.GetNextUnset(initialPos);
                currentPos = row.GetNextSet(currentPos);
            }
            else
            {
                currentPos = row.GetNextSet(initialPos);
                currentPos = row.GetNextUnset(currentPos);
            }
            return currentPos;
        }

        // not private for testing
        internal ExpandedPair RetrieveNextPair(BitArray row, List<ExpandedPair> previousPairs, int rowNumber)
        {
            bool isOddPattern = previousPairs.Count % 2 == 0;

            FinderPattern pattern;

            bool keepFinding = true;
            int forcedOffset = -1;
            do
            {
                this.FindNextPair(row, previousPairs, forcedOffset);
                pattern = ParseFoundFinderPattern(row, rowNumber, isOddPattern);
                if (pattern == null)
                {
                    forcedOffset = GetNextSecondBar(row, this.startEnd[0]);
                }
                else
                {
                    keepFinding = false;
                }
            } while (keepFinding);

            bool mayBeLast = CheckPairSequence(previousPairs, pattern);

            DataCharacter leftChar = this.DecodeDataCharacter(row, pattern,
                    isOddPattern, true);
            DataCharacter rightChar;
            try
            {
                rightChar = this.DecodeDataCharacter(row, pattern, isOddPattern,
                        false);
            }
            catch (NotFoundException nfe)
            {
                if (mayBeLast)
                {
                    rightChar = null;
                }
                else
                {
                    throw nfe;
                }
            }

            return new ExpandedPair(leftChar, rightChar, pattern, mayBeLast);
        }

        private bool CheckPairSequence(List<ExpandedPair> previousPairs, FinderPattern pattern)
        {
            int currentSequenceLength = previousPairs.Count + 1;
            if (currentSequenceLength > this.currentSequence.Length)
            {
                throw NotFoundException.Instance;
            }

            for (int pos = 0; pos < previousPairs.Count; ++pos)
            {
                this.currentSequence[pos] = ((ExpandedPair)previousPairs[pos]).FinderPattern.Value;
            }

            this.currentSequence[currentSequenceLength - 1] = pattern.Value;

            for (int i = 0; i < FinderPatternSequences.Length; ++i)
            {
                int[] validSequence = FinderPatternSequences[i];
                if (validSequence.Length >= currentSequenceLength)
                {
                    bool valid = true;
                    for (int pos_0 = 0; pos_0 < currentSequenceLength; ++pos_0)
                    {
                        if (this.currentSequence[pos_0] != validSequence[pos_0])
                        {
                            valid = false;
                            break;
                        }
                    }

                    if (valid)
                    {
                        return currentSequenceLength == validSequence.Length;
                    }
                }
            }

            throw NotFoundException.Instance;
        }

        private void FindNextPair(BitArray row, List<ExpandedPair> previousPairs, int forcedOffset)
        {
            int[] counters = this.decodeFinderCounters;
            counters[0] = 0;
            counters[1] = 0;
            counters[2] = 0;
            counters[3] = 0;

            int width = row.GetSize();

            int rowOffset;
            if (forcedOffset >= 0)
            {
                rowOffset = forcedOffset;
            }
            else if ((previousPairs.Count == 0))
            {
                rowOffset = 0;
            }
            else
            {
                ExpandedPair lastPair = (ExpandedPair)previousPairs[previousPairs.Count - 1];
                rowOffset = lastPair.FinderPattern.StartEnd[1];
            }
            bool searchingEvenPair = previousPairs.Count % 2 != 0;

            bool isWhite = false;
            while (rowOffset < width)
            {
                isWhite = !row.Get(rowOffset);
                if (!isWhite)
                {
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
                        if (searchingEvenPair)
                        {
                            ReverseCounters(counters);
                        }

                        if (MessagingToolkit.Barcode.OneD.Rss.AbstractRssDecoder.IsFinderPattern(counters))
                        {
                            this.startEnd[0] = patternStart;
                            this.startEnd[1] = x;
                            return;
                        }

                        if (searchingEvenPair)
                        {
                            ReverseCounters(counters);
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

        private static void ReverseCounters(int[] counters)
        {
            int length = counters.Length;
            for (int i = 0; i < length / 2; ++i)
            {
                int tmp = counters[i];
                counters[i] = counters[length - i - 1];
                counters[length - i - 1] = tmp;
            }
        }

        private FinderPattern ParseFoundFinderPattern(BitArray row, int rowNumber,
                bool oddPattern)
        {
            // Actually we found elements 2-5.
            int firstCounter;
            int start;
            int end;

            if (oddPattern)
            {
                // If pattern number is odd, we need to locate element 1 *before* the current block.

                int firstElementStart = this.startEnd[0] - 1;
                // Locate element 1
                while (firstElementStart >= 0 && !row.Get(firstElementStart))
                {
                    firstElementStart--;
                }

                firstElementStart++;
                firstCounter = this.startEnd[0] - firstElementStart;
                start = firstElementStart;
                end = this.startEnd[1];

            }
            else
            {
                // If pattern number is even, the pattern is reversed, so we need to locate element 1 *after* the current block.

                start = this.startEnd[0];
                end = row.GetNextUnset(this.startEnd[1] + 1);
                firstCounter = end - this.startEnd[1];
            }

            // Make 'counters' hold 1-4
            int[] counters = this.decodeFinderCounters;
            for (int i = counters.Length - 1; i > 0; i--)
            {
                counters[i] = counters[i - 1];
            }

            counters[0] = firstCounter;
            int val;
            try
            {
                val = MessagingToolkit.Barcode.OneD.Rss.AbstractRssDecoder.ParseFinderValue(counters, FinderPatterns);
            }
            catch (NotFoundException nfe)
            {
                return null;
            }
            return new FinderPattern(val, new int[] { start, end }, start, end,
                    rowNumber);
        }

        internal DataCharacter DecodeDataCharacter(BitArray row, FinderPattern pattern,
                bool isOddPattern, bool leftChar)
        {
            int[] counters = this.dataCharacterCounters;
            counters[0] = 0;
            counters[1] = 0;
            counters[2] = 0;
            counters[3] = 0;
            counters[4] = 0;
            counters[5] = 0;
            counters[6] = 0;
            counters[7] = 0;

            if (leftChar)
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
            }//counters[] has the pixels of the module

            int numModules = 17; //left and right data characters have all the same length
            float elementWidth = (float)MessagingToolkit.Barcode.OneD.Rss.AbstractRssDecoder.Count(counters) / (float)numModules;

            int[] oddCounts = this.oddCounts;
            int[] evenCounts = this.evenCounts;
            float[] oddRoundingErrors = this.oddRoundingErrors;
            float[] evenRoundingErrors = this.evenRoundingErrors;

            for (int i = 0; i < counters.Length; i++)
            {
                float val1 = 1.0f * counters[i] / elementWidth;
                int count = (int)(val1 + 0.5f); // Round
                if (count < 1)
                {
                    count = 1;
                }
                else if (count > 8)
                {
                    count = 8;
                }
                int offset = i >> 1;
                if ((i & 0x01) == 0)
                {
                    oddCounts[offset] = count;
                    oddRoundingErrors[offset] = val1 - count;
                }
                else
                {
                    evenCounts[offset] = count;
                    evenRoundingErrors[offset] = val1 - count;
                }
            }

            AdjustOddEvenCounts(numModules);

            int weightRowNumber = 4 * pattern.Value + ((isOddPattern) ? 0 : 2)
                    + ((leftChar) ? 0 : 1) - 1;

            int oddSum = 0;
            int oddChecksumPortion = 0;
            for (int i_1 = oddCounts.Length - 1; i_1 >= 0; i_1--)
            {
                if (IsNotA1left(pattern, isOddPattern, leftChar))
                {
                    int weight = Weights[weightRowNumber][2 * i_1];
                    oddChecksumPortion += oddCounts[i_1] * weight;
                }
                oddSum += oddCounts[i_1];
            }
            int evenChecksumPortion = 0;
            int evenSum = 0;
            for (int i_2 = evenCounts.Length - 1; i_2 >= 0; i_2--)
            {
                if (IsNotA1left(pattern, isOddPattern, leftChar))
                {
                    int weight_3 = Weights[weightRowNumber][2 * i_2 + 1];
                    evenChecksumPortion += evenCounts[i_2] * weight_3;
                }
                evenSum += evenCounts[i_2];
            }
            int checksumPortion = oddChecksumPortion + evenChecksumPortion;

            if ((oddSum & 0x01) != 0 || oddSum > 13 || oddSum < 4)
            {
                throw NotFoundException.Instance;
            }

            int group = (13 - oddSum) / 2;
            int oddWidest = SymbolWidest[group];
            int evenWidest = 9 - oddWidest;
            int vOdd = MessagingToolkit.Barcode.OneD.Rss.RssUtils.GetRssValue(oddCounts, oddWidest, true);
            int vEven = MessagingToolkit.Barcode.OneD.Rss.RssUtils.GetRssValue(evenCounts, evenWidest, false);
            int tEven = EventTotalSubset[group];
            int gSum = GSum[group];
            int val = vOdd * tEven + vEven + gSum;

            return new DataCharacter(val, checksumPortion);
        }

        private static bool IsNotA1left(FinderPattern pattern,
                bool isOddPattern, bool leftChar)
        {
            // A1: pattern.getValue is 0 (A), and it's an oddPattern, and it is a left char
            return !(pattern.Value == 0 && isOddPattern && leftChar);
        }

        private void AdjustOddEvenCounts(int numModules)
        {

            int oddSum = MessagingToolkit.Barcode.OneD.Rss.AbstractRssDecoder.Count(this.oddCounts);
            int evenSum = MessagingToolkit.Barcode.OneD.Rss.AbstractRssDecoder.Count(this.evenCounts);
            int mismatch = oddSum + evenSum - numModules;
            bool oddParityBad = (oddSum & 0x01) == 1;
            bool evenParityBad = (evenSum & 0x01) == 0;

            bool incrementOdd = false;
            bool decrementOdd = false;

            if (oddSum > 13)
            {
                decrementOdd = true;
            }
            else if (oddSum < 4)
            {
                incrementOdd = true;
            }
            bool incrementEven = false;
            bool decrementEven = false;
            if (evenSum > 13)
            {
                decrementEven = true;
            }
            else if (evenSum < 4)
            {
                incrementEven = true;
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
                MessagingToolkit.Barcode.OneD.Rss.AbstractRssDecoder.Increment(this.oddCounts, this.oddRoundingErrors);
            }
            if (decrementOdd)
            {
                MessagingToolkit.Barcode.OneD.Rss.AbstractRssDecoder.Decrement(this.oddCounts, this.oddRoundingErrors);
            }
            if (incrementEven)
            {
                if (decrementEven)
                {
                    throw NotFoundException.Instance;
                }
                MessagingToolkit.Barcode.OneD.Rss.AbstractRssDecoder.Increment(this.evenCounts, this.oddRoundingErrors);
            }
            if (decrementEven)
            {
                MessagingToolkit.Barcode.OneD.Rss.AbstractRssDecoder.Decrement(this.evenCounts, this.evenRoundingErrors);
            }
        }
    }
}
