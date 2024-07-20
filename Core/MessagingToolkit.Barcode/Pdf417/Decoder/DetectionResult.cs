using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MessagingToolkit.Barcode.Pdf417.Decoder
{
    internal sealed class DetectionResult
    {

        private const int ADJUST_ROW_NUMBER_SKIP = 2;

        private readonly BarcodeMetadata barcodeMetadata;
        private readonly DetectionResultColumn[] detectionResultColumns;
        private BoundingBox boundingBox;
        private readonly int barcodeColumnCount;

        internal DetectionResult(BarcodeMetadata barcodeMetadata, BoundingBox boundingBox)
        {
            this.barcodeMetadata = barcodeMetadata;
            this.barcodeColumnCount = barcodeMetadata.ColumnCount;
            this.boundingBox = boundingBox;
            detectionResultColumns = new DetectionResultColumn[barcodeColumnCount + 2];
        }

        internal DetectionResultColumn[] DetectionResultColumns
        {
            get
            {
                AdjustIndicatorColumnRowNumbers(detectionResultColumns[0]);
                AdjustIndicatorColumnRowNumbers(detectionResultColumns[barcodeColumnCount + 1]);
                int unadjustedCodewordCount = Pdf417Common.MAX_CODEWORDS_IN_BARCODE;
                int previousUnadjustedCount;
                do
                {
                    previousUnadjustedCount = unadjustedCodewordCount;
                    unadjustedCodewordCount = AdjustRowNumbers();
                } while (unadjustedCodewordCount > 0 && unadjustedCodewordCount < previousUnadjustedCount);
                return detectionResultColumns;
            }
        }

        private void AdjustIndicatorColumnRowNumbers(DetectionResultColumn detectionResultColumn)
        {
            if (detectionResultColumn != null)
            {
                ((DetectionResultRowIndicatorColumn)detectionResultColumn).AdjustCompleteIndicatorColumnRowNumbers(barcodeMetadata);
            }
        }

        // TODO ensure that no detected codewords with unknown row number are left
        // we should be able to estimate the row height and use it as a hint for the row number
        // we should also fill the rows top to bottom and bottom to top
        /// <returns> number of codewords which don't have a valid row number. Note that the count is not accurate as codewords 
        /// will be counted several times. It just serves as an indicator to see when we can stop adjusting row numbers  </returns>
        private int AdjustRowNumbers()
        {
            int unadjustedCount = AdjustRowNumbersByRow();
            if (unadjustedCount == 0)
            {
                return 0;
            }
            for (int barcodeColumn = 1; barcodeColumn < barcodeColumnCount + 1; barcodeColumn++)
            {
                Codeword[] codewords = detectionResultColumns[barcodeColumn].Codewords;
                for (int codewordsRow = 0; codewordsRow < codewords.Length; codewordsRow++)
                {
                    if (codewords[codewordsRow] == null)
                    {
                        continue;
                    }
                    if (!codewords[codewordsRow].HasValidRowNumber())
                    {
                        AdjustRowNumbers(barcodeColumn, codewordsRow, codewords);
                    }
                }
            }
            return unadjustedCount;
        }

        private int AdjustRowNumbersByRow()
        {
            AdjustRowNumbersFromBothRI();
            // TODO we should only do full row adjustments if row numbers of left and right row indicator column match.
            // Maybe it's even better to calculated the height (in codeword rows) and divide it by the number of barcode
            // rows. This, together with the LRI and RRI row numbers should allow us to get a good estimate where a row
            // number starts and ends.
            int unadjustedCount = AdjustRowNumbersFromLRI();
            return unadjustedCount + AdjustRowNumbersFromRRI();
        }

        private int AdjustRowNumbersFromBothRI()
        {
            if (detectionResultColumns[0] == null || detectionResultColumns[barcodeColumnCount + 1] == null)
            {
                return 0;
            }
            Codeword[] LRIcodewords = detectionResultColumns[0].Codewords;
            Codeword[] RRIcodewords = detectionResultColumns[barcodeColumnCount + 1].Codewords;
            for (int codewordsRow = 0; codewordsRow < LRIcodewords.Length; codewordsRow++)
            {
                if (LRIcodewords[codewordsRow] != null && RRIcodewords[codewordsRow] != null && LRIcodewords[codewordsRow].RowNumber == RRIcodewords[codewordsRow].RowNumber)
                {
                    for (int barcodeColumn = 1; barcodeColumn <= barcodeColumnCount; barcodeColumn++)
                    {
                        Codeword codeword = detectionResultColumns[barcodeColumn].Codewords[codewordsRow];
                        if (codeword == null)
                        {
                            continue;
                        }
                        codeword.RowNumber = LRIcodewords[codewordsRow].RowNumber;
                        if (!codeword.HasValidRowNumber())
                        {
                            detectionResultColumns[barcodeColumn].Codewords[codewordsRow] = null;
                        }
                    }
                }
            }
            return 0;
        }

        private int AdjustRowNumbersFromRRI()
        {
            if (detectionResultColumns[barcodeColumnCount + 1] == null)
            {
                return 0;
            }
            int unadjustedCount = 0;
            Codeword[] codewords = detectionResultColumns[barcodeColumnCount + 1].Codewords;
            for (int codewordsRow = 0; codewordsRow < codewords.Length; codewordsRow++)
            {
                if (codewords[codewordsRow] == null)
                {
                    continue;
                }
                int rowIndicatorRowNumber = codewords[codewordsRow].RowNumber;
                int invalidRowCounts = 0;
                for (int barcodeColumn = barcodeColumnCount + 1; barcodeColumn > 0 && invalidRowCounts < ADJUST_ROW_NUMBER_SKIP; barcodeColumn--)
                {
                    Codeword codeword = detectionResultColumns[barcodeColumn].Codewords[codewordsRow];
                    if (codeword != null)
                    {
                        invalidRowCounts = AdjustRowNumberIfValid(rowIndicatorRowNumber, invalidRowCounts, codeword);
                        if (!codeword.HasValidRowNumber())
                        {
                            unadjustedCount++;
                        }
                    }
                }
            }
            return unadjustedCount;
        }

        private int AdjustRowNumbersFromLRI()
        {
            if (detectionResultColumns[0] == null)
            {
                return 0;
            }
            int unadjustedCount = 0;
            Codeword[] codewords = detectionResultColumns[0].Codewords;
            for (int codewordsRow = 0; codewordsRow < codewords.Length; codewordsRow++)
            {
                if (codewords[codewordsRow] == null)
                {
                    continue;
                }
                int rowIndicatorRowNumber = codewords[codewordsRow].RowNumber;
                int invalidRowCounts = 0;
                for (int barcodeColumn = 1; barcodeColumn < barcodeColumnCount + 1 && invalidRowCounts < ADJUST_ROW_NUMBER_SKIP; barcodeColumn++)
                {
                    Codeword codeword = detectionResultColumns[barcodeColumn].Codewords[codewordsRow];
                    if (codeword != null)
                    {
                        invalidRowCounts = AdjustRowNumberIfValid(rowIndicatorRowNumber, invalidRowCounts, codeword);
                        if (!codeword.HasValidRowNumber())
                        {
                            unadjustedCount++;
                        }
                    }
                }
            }
            return unadjustedCount;
        }

        private static int AdjustRowNumberIfValid(int rowIndicatorRowNumber, int invalidRowCounts, Codeword codeword)
        {
            if (codeword == null)
            {
                return invalidRowCounts;
            }
            if (!codeword.HasValidRowNumber())
            {
                if (codeword.IsValidRowNumber(rowIndicatorRowNumber))
                {
                    codeword.RowNumber = rowIndicatorRowNumber;
                    invalidRowCounts = 0;
                }
                else
                {
                    ++invalidRowCounts;
                }
            }
            return invalidRowCounts;
        }

        private void AdjustRowNumbers(int barcodeColumn, int codewordsRow, Codeword[] codewords)
        {
            Codeword codeword = codewords[codewordsRow];
            Codeword[] previousColumnCodewords = detectionResultColumns[barcodeColumn - 1].Codewords;
            Codeword[] nextColumnCodewords = previousColumnCodewords;
            if (detectionResultColumns[barcodeColumn + 1] != null)
            {
                nextColumnCodewords = detectionResultColumns[barcodeColumn + 1].Codewords;
            }

            Codeword[] otherCodewords = new Codeword[14];

            otherCodewords[2] = previousColumnCodewords[codewordsRow];
            otherCodewords[3] = nextColumnCodewords[codewordsRow];

            if (codewordsRow > 0)
            {
                otherCodewords[0] = codewords[codewordsRow - 1];
                otherCodewords[4] = previousColumnCodewords[codewordsRow - 1];
                otherCodewords[5] = nextColumnCodewords[codewordsRow - 1];
            }
            if (codewordsRow > 1)
            {
                otherCodewords[8] = codewords[codewordsRow - 2];
                otherCodewords[10] = previousColumnCodewords[codewordsRow - 2];
                otherCodewords[11] = nextColumnCodewords[codewordsRow - 2];
            }
            if (codewordsRow < codewords.Length - 1)
            {
                otherCodewords[1] = codewords[codewordsRow + 1];
                otherCodewords[6] = previousColumnCodewords[codewordsRow + 1];
                otherCodewords[7] = nextColumnCodewords[codewordsRow + 1];
            }
            if (codewordsRow < codewords.Length - 2)
            {
                otherCodewords[9] = codewords[codewordsRow + 2];
                otherCodewords[12] = previousColumnCodewords[codewordsRow + 2];
                otherCodewords[13] = nextColumnCodewords[codewordsRow + 2];
            }
            foreach (Codeword otherCodeword in otherCodewords)
            {
                if (AdjustRowNumber(codeword, otherCodeword))
                {
                    return;
                }
            }
        }

        /// <returns> true, if row number was adjusted, false otherwise </returns>
        private static bool AdjustRowNumber(Codeword codeword, Codeword otherCodeword)
        {
            if (otherCodeword == null)
            {
                return false;
            }
            if (otherCodeword.HasValidRowNumber() && otherCodeword.Bucket == codeword.Bucket)
            {
                codeword.RowNumber = otherCodeword.RowNumber;
                return true;
            }
            return false;
        }

        internal int BarcodeColumnCount
        {
            get
            {
                return barcodeColumnCount;
            }
        }

        internal int BarcodeRowCount
        {
            get
            {
                return barcodeMetadata.RowCount;
            }
        }

        internal int BarcodeECLevel
        {
            get
            {
                return barcodeMetadata.ErrorCorrectionLevel;
            }
        }

        public BoundingBox BoundingBox
        {
            set
            {
                this.boundingBox = value;
            }
            get
            {
                return boundingBox;
            }
        }


        internal void SetDetectionResultColumn(int barcodeColumn, DetectionResultColumn detectionResultColumn)
        {
            detectionResultColumns[barcodeColumn] = detectionResultColumn;
        }

        internal DetectionResultColumn GetDetectionResultColumn(int barcodeColumn)
        {
            return detectionResultColumns[barcodeColumn];
        }

        public override string ToString()
        {
            DetectionResultColumn rowIndicatorColumn = detectionResultColumns[0];
            if (rowIndicatorColumn == null)
            {
                rowIndicatorColumn = detectionResultColumns[barcodeColumnCount + 1];
            }
            StringBuilder formatter = new StringBuilder();
            for (int codewordsRow = 0; codewordsRow < rowIndicatorColumn.Codewords.Length; codewordsRow++)
            {
                formatter.Append(String.Format("CW %3d:", codewordsRow));
                for (int barcodeColumn = 0; barcodeColumn < barcodeColumnCount + 2; barcodeColumn++)
                {
                    if (detectionResultColumns[barcodeColumn] == null)
                    {
                        formatter.Append("    |   ");
                        continue;
                    }
                    Codeword codeword = detectionResultColumns[barcodeColumn].Codewords[codewordsRow];
                    if (codeword == null)
                    {
                        formatter.Append("    |   ");
                        continue;
                    }
                    formatter.Append(String.Format(" %3d|%3d", codeword.RowNumber, codeword.Value));
                }
                formatter.Append("\n");
            }
            string result = formatter.ToString();
            return result;
        }

    }
}
