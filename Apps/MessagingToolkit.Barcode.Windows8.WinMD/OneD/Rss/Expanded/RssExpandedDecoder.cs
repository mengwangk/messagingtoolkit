using System;
using System.Collections.Generic;

using MessagingToolkit.Barcode.Common;
using MessagingToolkit.Barcode.OneD.Rss.Expanded.Decoders;

namespace MessagingToolkit.Barcode.OneD.Rss.Expanded
{

    internal sealed class RssExpandedDecoder : AbstractRssDecoder
    {
        private static readonly int[] SymbolWidest = { 7, 5, 4, 3, 1 };
        private static readonly int[] EventTotalSubset = { 4, 20, 52, 104, 204 };
        private static readonly int[] GSum = { 0, 348, 1388, 2948, 3988 };

        public RssExpandedDecoder()
        {
            this.pairs = new List<ExpandedPair>(MaxPairs);
            this.rows = new List<ExpandedRow>();
            this.startEnd = new int[2];
            //this.currentSequence = new int[LongestSequenceSize];
        }

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

        //private static readonly int LongestSequenceSize = FinderPatternSequences[FinderPatternSequences.Length - 1].Length;


        private const int MaxPairs = 11;
        private readonly List<ExpandedPair> pairs;
        private readonly List<ExpandedRow> rows;
        private readonly int[] startEnd;
        //private readonly int[] currentSequence;
        private bool startFromEven = false;

        public override Result DecodeRow(int rowNumber, BitArray row, IDictionary<DecodeOptions, object> decodingOptions)
        {
            // Rows can start with even pattern in case in prev rows there where odd number of patters.
            // So lets try twice
            this.pairs.Clear();
            this.startFromEven = false;
            try
            {
                List<ExpandedPair> pairs1 = DecodeRow2pairs(rowNumber, row);
                return ConstructResult(pairs);
            }
            catch (NotFoundException e)
            {
                // OK
            }

            this.pairs.Clear();
            this.startFromEven = true;
            List<ExpandedPair> pairs2 = DecodeRow2pairs(rowNumber, row);
            return ConstructResult(pairs);
        }

        public override void Reset()
        {
            this.pairs.Clear();
            this.rows.Clear();
        }

        // Not private for testing
        List<ExpandedPair> DecodeRow2pairs(int rowNumber, BitArray row)
        {
            try
            {
                while (true)
                {
                    ExpandedPair nextPair = RetrieveNextPair(row, this.pairs, rowNumber);
                    this.pairs.Add(nextPair);
                    //System.out.println(this.pairs.size()+" pairs found so far on row "+rowNumber+": "+this.pairs);
                    // exit this loop when retrieveNextPair() fails and throws
                }
            }
            catch (NotFoundException nfe)
            {
                if (this.pairs.Count == 0)
                {
                    throw nfe;
                }
            }

            // TODO: verify sequence of finder patterns as in checkPairSequence()
            if (CheckChecksum())
            {
                return this.pairs;
            }

            bool tryStackedDecode = rows.Count != 0;
            bool wasReversed = false; // TODO: deal with reversed rows
            StoreRow(rowNumber, wasReversed);
            if (tryStackedDecode)
            {
                // When the image is 180-rotated, then rows are sorted in wrong dirrection.
                // Try twice with both the directions.
                List<ExpandedPair> ps = CheckRows(false);
                if (ps != null)
                {
                    return ps;
                }
                ps = CheckRows(true);
                if (ps != null)
                {
                    return ps;
                }
            }

            throw NotFoundException.Instance;
        }

        private List<ExpandedPair> CheckRows(bool reverse)
        {
            // Limit number of rows we are checking
            // We use recursive algorithm with pure complexity and don't want it to take forever
            // Stacked barcode can have up to 11 rows, so 25 seems resonable enough
            if (this.rows.Count > 25)
            {
                this.rows.Clear();  // We will never have a chance to get result, so clear it
                return null;
            }

            this.pairs.Clear();
            if (reverse)
            {
                rows.Reverse();
            }

            List<ExpandedPair> ps = null;
            try
            {
                ps = CheckRows(new List<ExpandedRow>(), 0);
            }
            catch (NotFoundException e)
            {
                // OK
            }

            if (reverse)
            {
                rows.Reverse();
            }

            return ps;
        }


        // Try to construct a valid rows sequence
        // Recursion is used to implement backtracking
        private List<ExpandedPair> CheckRows(List<ExpandedRow> collectedRows, int currentRow)
        {
            for (int i = currentRow; i < rows.Count; i++)
            {
                ExpandedRow row = rows[i];
                this.pairs.Clear();
                int size = collectedRows.Count;
                for (int j = 0; j < size; j++)
                {
                    pairs.AddRange(collectedRows[j].Pairs);
                }
                pairs.AddRange(row.Pairs);

                if (!IsValidSequence(this.pairs))
                {
                    continue;
                }

                if (CheckChecksum())
                {
                    return this.pairs;
                }

                List<ExpandedRow> rs = new List<ExpandedRow>();
                rs.AddRange(collectedRows);
                rs.Add(row);
                try
                {
                    // Recursion: try to add more rows
                    return CheckRows(rs, i + 1);
                }
                catch (NotFoundException e)
                {
                    // We failed, try the next candidate
                }
            }

            throw NotFoundException.Instance;
        }

        // Whether the pairs form a valid find pattern seqience,
        // either complete or a prefix
        private static bool IsValidSequence(List<ExpandedPair> pairs)
        {
            foreach (int[] sequence in FinderPatternSequences)
            {
                if (pairs.Count > sequence.Length)
                {
                    continue;
                }

                bool stop = true;
                for (int j = 0; j < pairs.Count; j++)
                {
                    if (pairs[j].FinderPattern.Value != sequence[j])
                    {
                        stop = false;
                        break;
                    }
                }

                if (stop)
                {
                    return true;
                }
            }
            return false;
        }


        private void StoreRow(int rowNumber, bool wasReversed)
        {
            // Discard if duplicate above or below; otherwise insert in order by row number.
            int insertPos = 0;
            bool prevIsSame = false;
            bool nextIsSame = false;
            while (insertPos < rows.Count)
            {
                ExpandedRow erow = rows[insertPos];
                if (erow.RowNumber > rowNumber)
                {
                    nextIsSame = erow.IsEquivalent(this.pairs);
                    break;
                }
                prevIsSame = erow.IsEquivalent(this.pairs);
                insertPos++;
            }
            if (nextIsSame || prevIsSame)
            {
                return;
            }

            // When the row was partially decoded (e.g. 2 pairs found instead of 3),
            // it will prevent us from detecting the barcode.
            // Try to merge partial rows

            // Check whether the row is part of an allready detected row
            if (IsPartialRow(this.pairs, this.rows))
            {
                return;
            }

            rows.Insert(insertPos, new ExpandedRow(pairs, rowNumber, wasReversed));

            RemovePartialRows(this.pairs, this.rows);
        }

        // Remove all the rows that contains only specified pairs 
        private static void RemovePartialRows(List<ExpandedPair> pairs, List<ExpandedRow> rows)
        {
            for (var index = 0; index < rows.Count; index++)
            {
                var r = rows[index];
                if (r.Pairs.Count == pairs.Count)
                {
                    continue;
                }
                bool allFound = true;
                foreach (ExpandedPair p in r.Pairs)
                {
                    bool found = false;
                    foreach (ExpandedPair pp in pairs)
                    {
                        if (p.Equals(pp))
                        {
                            found = true;
                            break;
                        }
                    }
                    if (!found)
                    {
                        allFound = false;
                        break;
                    }
                }
                if (allFound)
                {
                    // 'pairs' contains all the pairs from the row 'r'
                    rows.RemoveAt(index);
                }
            }
        }


        // Returns true when one of the rows already contains all the pairs
        private static bool IsPartialRow(IEnumerable<ExpandedPair> pairs, IEnumerable<ExpandedRow> rows)
        {
            foreach (ExpandedRow r in rows)
            {
                var allFound = true;
                foreach (ExpandedPair p in pairs)
                {
                    bool found = false;
                    foreach (ExpandedPair pp in r.Pairs)
                    {
                        if (p.Equals(pp))
                        {
                            found = true;
                            break;
                        }
                    }
                    if (!found)
                    {
                        allFound = false;
                        break;
                    }
                }
                if (allFound)
                {
                    // the row 'r' contain all the pairs from 'pairs'
                    return true;
                }
            }
            return false;
        }

        // Only used for unit testing
        internal List<ExpandedRow> Rows
        {
            get { return this.rows; }
        }



        static Result ConstructResult(List<ExpandedPair> pairs)
        {
            BitArray binary = MessagingToolkit.Barcode.OneD.Rss.Expanded.BitArrayBuilder.BuildBitArray(pairs);

            AbstractExpandedDecoder decoder = MessagingToolkit.Barcode.OneD.Rss.Expanded.Decoders.AbstractExpandedDecoder.CreateDecoder(binary);
            String resultingString = decoder.ParseInformation();

            ResultPoint[] firstPoints = ((ExpandedPair)pairs[0]).FinderPattern.ResultPoints;
            ResultPoint[] lastPoints = ((ExpandedPair)pairs[pairs.Count - 1]).FinderPattern.ResultPoints;

            return new Result(resultingString, null, new ResultPoint[] {
					firstPoints[0], firstPoints[1], lastPoints[0], lastPoints[1] },
                    MessagingToolkit.Barcode.BarcodeFormat.RSSExpanded);
        }

        private bool CheckChecksum()
        {
            ExpandedPair firstPair = (ExpandedPair)this.pairs[0];
            DataCharacter checkCharacter = firstPair.LeftChar;
            DataCharacter firstCharacter = firstPair.RightChar;

            if (firstCharacter == null)
            {
                return false;
            }

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
            if (startFromEven)
            {
                isOddPattern = !isOddPattern;
            }
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

            // When stacked symbol is split over multiple rows, there's no way to guess if this pair can be last or not.
            // boolean mayBeLast = checkPairSequence(previousPairs, pattern);

            DataCharacter leftChar = this.DecodeDataCharacter(row, pattern, isOddPattern, true);

            if (previousPairs.Count != 0 && previousPairs[previousPairs.Count - 1].MustBeLast)
            {
                throw NotFoundException.Instance;
            }


            DataCharacter rightChar;
            try
            {
                rightChar = this.DecodeDataCharacter(row, pattern, isOddPattern, false);
            }
            catch (NotFoundException)
            {
                rightChar = null;
            }
            bool mayBeLast = true;
            return new ExpandedPair(leftChar, rightChar, pattern, mayBeLast);
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
            if (startFromEven)
            {
                searchingEvenPair = !searchingEvenPair;
            }
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
            catch (NotFoundException)
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
                MessagingToolkit.Barcode.OneD.OneDDecoder.RecordPattern(row, pattern.StartEnd[1], counters);
                // reverse it
                for (int i = 0, j = counters.Length - 1; i < j; i++, j--)
                {
                    int temp = counters[i];
                    counters[i] = counters[j];
                    counters[j] = temp;
                }
            }//counters[] has the pixels of the module

            const int numModules = 17; //left and right data characters have all the same length
            float elementWidth = (float)MessagingToolkit.Barcode.OneD.Rss.AbstractRssDecoder.Count(counters) / (float)numModules;

            // Sanity check: element width for pattern and the character should match
            float expectedElementWidth = (pattern.StartEnd[1] - pattern.StartEnd[0]) / 15.0f;
            if (Math.Abs(elementWidth - expectedElementWidth) / expectedElementWidth > 0.3f)
            {
                throw NotFoundException.Instance;
            }

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
                    if (val1 < 0.3f)
                    {
                        throw NotFoundException.Instance;
                    }
                    count = 1;
                }
                else if (count > 8)
                {
                    if (val1 > 8.7f)
                    {
                        throw NotFoundException.Instance;
                    }
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
            for (int i = oddCounts.Length - 1; i >= 0; i--)
            {
                if (IsNotA1left(pattern, isOddPattern, leftChar))
                {
                    int weight = Weights[weightRowNumber][2 * i];
                    oddChecksumPortion += oddCounts[i] * weight;
                }
                oddSum += oddCounts[i];
            }
            int evenChecksumPortion = 0;
            int evenSum = 0;
            for (int i = evenCounts.Length - 1; i >= 0; i--)
            {
                if (IsNotA1left(pattern, isOddPattern, leftChar))
                {
                    int weight_3 = Weights[weightRowNumber][2 * i + 1];
                    evenChecksumPortion += evenCounts[i] * weight_3;
                }
                evenSum += evenCounts[i];
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
