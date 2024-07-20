using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MessagingToolkit.Barcode.Pdf417.Decoder
{
    internal sealed class DetectionResultRowIndicatorColumn : DetectionResultColumn
    {

        private readonly bool isLeft;

        internal DetectionResultRowIndicatorColumn(BoundingBox boundingBox, bool isLeft)
            : base(boundingBox)
        {
            this.isLeft = isLeft;
        }

        internal void SetRowNumbers()
        {
            foreach (Codeword codeword in Codewords)
            {
                if (codeword != null)
                {
                    codeword.SetRowNumberAsRowIndicatorColumn();
                }
            }
        }

        // TODO implement properly
        // TODO maybe we should add missing codewords to store the correct row number to make
        // finding row numbers for other columns easier
        // use row height count to make detection of invalid row numbers more reliable
        internal int AdjustCompleteIndicatorColumnRowNumbers(BarcodeMetadata barcodeMetadata)
        {
            Codeword[] codewords = Codewords;
            SetRowNumbers();
            RemoveIncorrectCodewords(codewords, barcodeMetadata);
            BoundingBox boundingBox = BoundingBox;
            ResultPoint top = isLeft ? boundingBox.TopLeft : boundingBox.TopRight;
            ResultPoint bottom = isLeft ? boundingBox.BottomLeft : boundingBox.BottomRight;
            int firstRow = ImageRowToCodewordIndex((int)top.Y);
            int lastRow = ImageRowToCodewordIndex((int)bottom.Y);
            // We need to be careful using the average row height. Barcode could be skewed so that we have smaller and 
            // taller rows
            float averageRowHeight = (lastRow - firstRow) / (float)barcodeMetadata.RowCount;
            int barcodeRow = -1;
            int maxRowHeight = 1;
            int currentRowHeight = 0;
            for (int codewordsRow = firstRow; codewordsRow < lastRow; codewordsRow++)
            {
                if (codewords[codewordsRow] == null)
                {
                    continue;
                }
                Codeword codeword = codewords[codewordsRow];

                //      float expectedRowNumber = (codewordsRow - firstRow) / averageRowHeight;
                //      if (Math.abs(codeword.getRowNumber() - expectedRowNumber) > 2) {
                //        SimpleLog.log(LEVEL.WARNING,
                //            "Removing codeword, rowNumberSkew too high, codeword[" + codewordsRow + "]: Expected Row: " +
                //                expectedRowNumber + ", RealRow: " + codeword.getRowNumber() + ", value: " + codeword.getValue());
                //        codewords[codewordsRow] = null;
                //      }

                int rowDifference = codeword.RowNumber - barcodeRow;

                // TODO improve handling with case where first row indicator doesn't start with 0

                if (rowDifference == 0)
                {
                    currentRowHeight++;
                }
                else if (rowDifference == 1)
                {
                    maxRowHeight = Math.Max(maxRowHeight, currentRowHeight);
                    currentRowHeight = 1;
                    barcodeRow = codeword.RowNumber;
                }
                else if (rowDifference < 0)
                {
                    codewords[codewordsRow] = null;
                }
                else if (codeword.RowNumber >= barcodeMetadata.RowCount)
                {
                    codewords[codewordsRow] = null;
                }
                else if (rowDifference > codewordsRow)
                {
                    codewords[codewordsRow] = null;
                }
                else
                {
                    int checkedRows;
                    if (maxRowHeight > 2)
                    {
                        checkedRows = (maxRowHeight - 2) * rowDifference;
                    }
                    else
                    {
                        checkedRows = rowDifference;
                    }
                    bool closePreviousCodewordFound = checkedRows >= codewordsRow;
                    for (int i = 1; i <= checkedRows && !closePreviousCodewordFound; i++)
                    {
                        // there must be (height * rowDifference) number of codewords missing. For now we assume height = 1.
                        // This should hopefully get rid of most problems already.
                        closePreviousCodewordFound = codewords[codewordsRow - i] != null;
                    }
                    if (closePreviousCodewordFound)
                    {
                        codewords[codewordsRow] = null;
                    }
                    else
                    {
                        barcodeRow = codeword.RowNumber;
                        currentRowHeight = 1;
                    }
                }
            }
            return (int)(averageRowHeight + 0.5);
        }

        internal int[] RowHeights
        {
            get
            {
                BarcodeMetadata barcodeMetadata = BarcodeMetadata;
                if (barcodeMetadata == null)
                {
                    return null;
                }
                AdjustIncompleteIndicatorColumnRowNumbers(barcodeMetadata);
                int[] result = new int[barcodeMetadata.RowCount];
                foreach (Codeword codeword in Codewords)
                {
                    if (codeword != null)
                    {
                        result[codeword.RowNumber]++;
                    }
                }
                return result;
            }
        }

        // TODO maybe we should add missing codewords to store the correct row number to make
        // finding row numbers for other columns easier
        // use row height count to make detection of invalid row numbers more reliable
        internal int AdjustIncompleteIndicatorColumnRowNumbers(BarcodeMetadata barcodeMetadata)
        {
            BoundingBox boundingBox = BoundingBox;
            ResultPoint top = isLeft ? boundingBox.TopLeft : boundingBox.TopRight;
            ResultPoint bottom = isLeft ? boundingBox.BottomLeft : boundingBox.BottomRight;
            int firstRow = ImageRowToCodewordIndex((int)top.Y);
            int lastRow = ImageRowToCodewordIndex((int)bottom.Y);
            float averageRowHeight = (lastRow - firstRow) / (float)barcodeMetadata.RowCount;
            Codeword[] codewords = Codewords;
            int barcodeRow = -1;
            int maxRowHeight = 1;
            int currentRowHeight = 0;
            for (int codewordsRow = firstRow; codewordsRow < lastRow; codewordsRow++)
            {
                if (codewords[codewordsRow] == null)
                {
                    continue;
                }
                Codeword codeword = codewords[codewordsRow];

                codeword.SetRowNumberAsRowIndicatorColumn();

                int rowDifference = codeword.RowNumber - barcodeRow;

                // TODO improve handling with case where first row indicator doesn't start with 0

                if (rowDifference == 0)
                {
                    currentRowHeight++;
                }
                else if (rowDifference == 1)
                {
                    maxRowHeight = Math.Max(maxRowHeight, currentRowHeight);
                    currentRowHeight = 1;
                    barcodeRow = codeword.RowNumber;
                }
                else if (codeword.RowNumber >= barcodeMetadata.RowCount)
                {
                    codewords[codewordsRow] = null;
                }
                else
                {
                    barcodeRow = codeword.RowNumber;
                    currentRowHeight = 1;
                }
            }
            return (int)(averageRowHeight + 0.5);
        }

        internal BarcodeMetadata BarcodeMetadata
        {
            get
            {
                Codeword[] codewords = Codewords;
                BarcodeValue barcodeColumnCount = new BarcodeValue();
                BarcodeValue barcodeRowCountUpperPart = new BarcodeValue();
                BarcodeValue barcodeRowCountLowerPart = new BarcodeValue();
                BarcodeValue barcodeECLevel = new BarcodeValue();
                foreach (Codeword codeword in codewords)
                {
                    if (codeword == null)
                    {
                        continue;
                    }
                    codeword.SetRowNumberAsRowIndicatorColumn();
                    int rowIndicatorValue = codeword.Value % 30;
                    int codewordRowNumber = codeword.RowNumber;
                    if (!isLeft)
                    {
                        codewordRowNumber += 2;
                    }
                    switch (codewordRowNumber % 3)
                    {
                        case 0:
                            barcodeRowCountUpperPart.SetValue (rowIndicatorValue * 3 + 1);
                            break;
                        case 1:
                            barcodeECLevel.SetValue ( rowIndicatorValue / 3);
                            barcodeRowCountLowerPart.SetValue ( rowIndicatorValue % 3);
                            break;
                        case 2:
                            barcodeColumnCount.SetValue( rowIndicatorValue + 1);
                            break;
                    }
                }
                // Maybe we should check if we have ambiguous values?
                if ((barcodeColumnCount.GetValue().Length == 0) || 
                    (barcodeRowCountUpperPart.GetValue().Length == 0) || (barcodeRowCountLowerPart.GetValue().Length == 0) || 
                    (barcodeECLevel.GetValue().Length == 0) || barcodeColumnCount.GetValue()[0] < 1 || 
                    barcodeRowCountUpperPart.GetValue()[0] + barcodeRowCountLowerPart.GetValue()[0] < Pdf417Common.MIN_ROWS_IN_BARCODE || 
                    barcodeRowCountUpperPart.GetValue()[0] + barcodeRowCountLowerPart.GetValue()[0] > Pdf417Common.MAX_ROWS_IN_BARCODE)
                {
                    return null;
                }
                BarcodeMetadata barcodeMetadata = new BarcodeMetadata(barcodeColumnCount.GetValue()[0], barcodeRowCountUpperPart.GetValue()[0], 
                    barcodeRowCountLowerPart.GetValue()[0], barcodeECLevel.GetValue()[0]);
                RemoveIncorrectCodewords(codewords, barcodeMetadata);
                return barcodeMetadata;
            }
        }

        private void RemoveIncorrectCodewords(Codeword[] codewords, BarcodeMetadata barcodeMetadata)
        {
            // Remove codewords which do not match the metadata
            // TODO Maybe we should keep the incorrect codewords for the start and end positions?
            for (int codewordRow = 0; codewordRow < codewords.Length; codewordRow++)
            {
                Codeword codeword = codewords[codewordRow];
                if (codewords[codewordRow] == null)
                {
                    continue;
                }
                int rowIndicatorValue = codeword.Value % 30;
                int codewordRowNumber = codeword.RowNumber;
                if (codewordRowNumber > barcodeMetadata.RowCount)
                {
                    codewords[codewordRow] = null;
                    continue;
                }
                if (!isLeft)
                {
                    codewordRowNumber += 2;
                }
                switch (codewordRowNumber % 3)
                {
                    case 0:
                        if (rowIndicatorValue * 3 + 1 != barcodeMetadata.RowCountUpperPart)
                        {
                            codewords[codewordRow] = null;
                        }
                        break;
                    case 1:
                        if (rowIndicatorValue / 3 != barcodeMetadata.ErrorCorrectionLevel || rowIndicatorValue % 3 != barcodeMetadata.RowCountLowerPart)
                        {
                            codewords[codewordRow] = null;
                        }
                        break;
                    case 2:
                        if (rowIndicatorValue + 1 != barcodeMetadata.ColumnCount)
                        {
                            codewords[codewordRow] = null;
                        }
                        break;
                }
            }
        }

        internal bool Left
        {
            get
            {
                return isLeft;
            }
        }

        public override string ToString()
        {
            return "IsLeft: " + isLeft + '\n' + base.ToString();
        }

    }
}
