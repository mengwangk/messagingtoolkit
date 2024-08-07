﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MessagingToolkit.Barcode.Pdf417.Decoder.Ec;
using MessagingToolkit.Barcode.Common;

namespace MessagingToolkit.Barcode.Pdf417.Decoder
{
    internal sealed class Pdf417ScanningDecoder
    {
        private const int CODEWORD_SKEW_SIZE = 2;

        private const int MAX_ERRORS = 3;
        private const int MAX_EC_CODEWORDS = 512;
        private static readonly ErrorCorrection errorCorrection = new ErrorCorrection();

        private Pdf417ScanningDecoder()
        {
        }

        // TODO don't pass in minCodewordWidth and maxCodewordWidth, pass in barcode columns for start and stop pattern
        // columns. That way width can be deducted from the pattern column.
        // This approach also allows to detect more details about the barcode, e.g. if a bar type (white or black) is wider 
        // than it should be. This can happen if the scanner used a bad blackpoint.
        public static DecoderResult Decode(BitMatrix image, ResultPoint imageTopLeft, ResultPoint imageBottomLeft,
              ResultPoint imageTopRight, ResultPoint imageBottomRight, int minCodewordWidth, int maxCodewordWidth)
        {
            BoundingBox boundingBox = new BoundingBox(image, imageTopLeft, imageBottomLeft, imageTopRight, imageBottomRight);
            DetectionResultRowIndicatorColumn leftRowIndicatorColumn = null;
            DetectionResultRowIndicatorColumn rightRowIndicatorColumn = null;
            DetectionResult detectionResult = null;
            for (int i = 0; i < 2; i++)
            {
                if (imageTopLeft != null)
                {
                    leftRowIndicatorColumn = GetRowIndicatorColumn(image, boundingBox, imageTopLeft, true, minCodewordWidth, maxCodewordWidth);
                }
                if (imageTopRight != null)
                {
                    rightRowIndicatorColumn = GetRowIndicatorColumn(image, boundingBox, imageTopRight, false, minCodewordWidth, maxCodewordWidth);
                }
                detectionResult = Merge(leftRowIndicatorColumn, rightRowIndicatorColumn);
                if (detectionResult == null)
                {
                    throw NotFoundException.Instance;
                }
                if (i == 0 && detectionResult.BoundingBox != null && 
                    (detectionResult.BoundingBox.MinY < boundingBox.MinY || detectionResult.BoundingBox.MaxY > boundingBox.MaxY))
                {
                    boundingBox = detectionResult.BoundingBox;
                }
                else
                {
                    detectionResult.BoundingBox = boundingBox;
                    break;
                }
            }
            int maxBarcodeColumn = detectionResult.BarcodeColumnCount + 1;
            detectionResult.SetDetectionResultColumn(0, leftRowIndicatorColumn);
            detectionResult.SetDetectionResultColumn(maxBarcodeColumn, rightRowIndicatorColumn);

            bool leftToRight = leftRowIndicatorColumn != null;
            for (int barcodeColumnCount = 1; barcodeColumnCount <= maxBarcodeColumn; barcodeColumnCount++)
            {
                int barcodeColumn = leftToRight ? barcodeColumnCount : maxBarcodeColumn - barcodeColumnCount;
                if (detectionResult.GetDetectionResultColumn(barcodeColumn) != null)
                {
                    // This will be the case for the opposite row indicator column, which doesn't need to be decoded again.
                    continue;
                }
                DetectionResultColumn detectionResultColumn;
                if (barcodeColumn == 0 || barcodeColumn == maxBarcodeColumn)
                {
                    detectionResultColumn = new DetectionResultRowIndicatorColumn(boundingBox, barcodeColumn == 0);
                }
                else
                {
                    detectionResultColumn = new DetectionResultColumn(boundingBox);
                }
                detectionResult.SetDetectionResultColumn(barcodeColumn, detectionResultColumn);
                int startColumn = -1;
                int previousStartColumn = startColumn;
                // TODO start at a row for which we know the start position, then detect upwards and downwards from there.
                for (int imageRow = boundingBox.MinY; imageRow <= boundingBox.MaxY; imageRow++)
                {
                    startColumn = GetStartColumn(detectionResult, barcodeColumn, imageRow, leftToRight);
                    if (startColumn < 0 || startColumn > boundingBox.MaxX)
                    {
                        if (previousStartColumn == -1)
                        {
                            continue;
                        }
                        startColumn = previousStartColumn;
                    }
                    Codeword codeword = DetectCodeword(image, boundingBox.MinX, boundingBox.MaxX, leftToRight, startColumn, imageRow, minCodewordWidth, maxCodewordWidth);
                    if (codeword != null)
                    {
                        detectionResultColumn.SetCodeword(imageRow, codeword);
                        previousStartColumn = startColumn;
                        minCodewordWidth = Math.Min(minCodewordWidth, codeword.Width);
                        maxCodewordWidth = Math.Max(maxCodewordWidth, codeword.Width);
                    }
                }
            }
            return CreateDecoderResult(detectionResult);
        }

        private static DetectionResult Merge(DetectionResultRowIndicatorColumn leftRowIndicatorColumn, DetectionResultRowIndicatorColumn rightRowIndicatorColumn)
        {
            if (leftRowIndicatorColumn == null && rightRowIndicatorColumn == null)
            {
                return null;
            }
            BarcodeMetadata barcodeMetadata = GetBarcodeMetadata(leftRowIndicatorColumn, rightRowIndicatorColumn);
            if (barcodeMetadata == null)
            {
                return null;
            }
            BoundingBox boundingBox = BoundingBox.Merge(AdjustBoundingBox(leftRowIndicatorColumn), AdjustBoundingBox(rightRowIndicatorColumn));
            return new DetectionResult(barcodeMetadata, boundingBox);
        }

        private static BoundingBox AdjustBoundingBox(DetectionResultRowIndicatorColumn rowIndicatorColumn)
        {
            if (rowIndicatorColumn == null)
            {
                return null;
            }
            int[] rowHeights = rowIndicatorColumn.RowHeights;
            int maxRowHeight = GetMax(rowHeights);
            int missingStartRows = 0;
            foreach (int rowHeight in rowHeights)
            {
                missingStartRows += maxRowHeight - rowHeight;
                if (rowHeight > 0)
                {
                    break;
                }
            }
            Codeword[] codewords = rowIndicatorColumn.Codewords;
            for (int row = 0; missingStartRows > 0 && codewords[row] == null; row++)
            {
                missingStartRows--;
            }
            int missingEndRows = 0;
            for (int row = rowHeights.Length - 1; row >= 0; row--)
            {
                missingEndRows += maxRowHeight - rowHeights[row];
                if (rowHeights[row] > 0)
                {
                    break;
                }
            }
            for (int row = codewords.Length - 1; missingEndRows > 0 && codewords[row] == null; row--)
            {
                missingEndRows--;
            }
            return rowIndicatorColumn.BoundingBox.AddMissingRows(missingStartRows, missingEndRows, rowIndicatorColumn.Left);
        }

        private static int GetMax(int[] values)
        {
            int maxValue = -1;
            foreach (int value in values)
            {
                maxValue = Math.Max(maxValue, value);
            }
            return maxValue;
        }

        private static BarcodeMetadata GetBarcodeMetadata(DetectionResultRowIndicatorColumn leftRowIndicatorColumn,
                        DetectionResultRowIndicatorColumn rightRowIndicatorColumn)
        {
            if (leftRowIndicatorColumn == null || leftRowIndicatorColumn.BarcodeMetadata == null)
            {
                return rightRowIndicatorColumn == null ? null : rightRowIndicatorColumn.BarcodeMetadata;
            }
            if (rightRowIndicatorColumn == null || rightRowIndicatorColumn.BarcodeMetadata == null)
            {
                return leftRowIndicatorColumn == null ? null : leftRowIndicatorColumn.BarcodeMetadata;
            }
            BarcodeMetadata leftBarcodeMetadata = leftRowIndicatorColumn.BarcodeMetadata;
            BarcodeMetadata rightBarcodeMetadata = rightRowIndicatorColumn.BarcodeMetadata;

            if (leftBarcodeMetadata.ColumnCount != rightBarcodeMetadata.ColumnCount && leftBarcodeMetadata.ErrorCorrectionLevel != rightBarcodeMetadata.ErrorCorrectionLevel && leftBarcodeMetadata.RowCount != rightBarcodeMetadata.RowCount)
            {
                return null;
            }
            return leftBarcodeMetadata;
        }

        private static DetectionResultRowIndicatorColumn GetRowIndicatorColumn(BitMatrix image, BoundingBox boundingBox, ResultPoint startPoint, bool leftToRight, int minCodewordWidth, int maxCodewordWidth)
        {
            DetectionResultRowIndicatorColumn rowIndicatorColumn = new DetectionResultRowIndicatorColumn(boundingBox, leftToRight);
            for (int i = 0; i < 2; i++)
            {
                int increment = i == 0 ? 1 : -1;
                int startColumn = (int)startPoint.X;
                for (int imageRow = (int)startPoint.Y; imageRow <= boundingBox.MaxY && imageRow >= boundingBox.MinY; imageRow += increment)
                {
                    Codeword codeword = DetectCodeword(image, 0, image.Width, leftToRight, startColumn, imageRow, minCodewordWidth, maxCodewordWidth);
                    if (codeword != null)
                    {
                        rowIndicatorColumn.SetCodeword(imageRow, codeword);
                        if (leftToRight)
                        {
                            startColumn = codeword.StartX;
                        }
                        else
                        {
                            startColumn = codeword.EndX;
                        }
                    }
                }
            }
            return rowIndicatorColumn;
        }

        private static void AdjustCodewordCount(DetectionResult detectionResult, BarcodeValue[][] barcodeMatrix)
        {
            int[] numberOfCodewords = barcodeMatrix[0][1].GetValue();
            int calculatedNumberOfCodewords = detectionResult.BarcodeColumnCount * detectionResult.BarcodeRowCount - GetNumberOfECCodeWords(detectionResult.BarcodeECLevel);
            if (numberOfCodewords.Length == 0)
            {
                if (calculatedNumberOfCodewords < 1 || calculatedNumberOfCodewords > Pdf417Common.MAX_CODEWORDS_IN_BARCODE)
                {
                    throw NotFoundException.Instance;
                }
                barcodeMatrix[0][1].SetValue(calculatedNumberOfCodewords);
            }
            else if (numberOfCodewords[0] != calculatedNumberOfCodewords)
            {
                // The calculated one is more reliable as it is derived from the row indicator columns
                barcodeMatrix[0][1].SetValue(calculatedNumberOfCodewords);
            }
        }

        private static DecoderResult CreateDecoderResult(DetectionResult detectionResult)
        {
            BarcodeValue[][] barcodeMatrix = CreateBarcodeMatrix(detectionResult);
            AdjustCodewordCount(detectionResult, barcodeMatrix);
            IList<int?> erasures = new List<int?>();
            int[] codewords = new int[detectionResult.BarcodeRowCount * detectionResult.BarcodeColumnCount];
            IList<int[]> ambiguousIndexValuesList = new List<int[]>();
            IList<int?> ambiguousIndexesList = new List<int?>();
            for (int row = 0; row < detectionResult.BarcodeRowCount; row++)
            {
                for (int column = 0; column < detectionResult.BarcodeColumnCount; column++)
                {
                    int[] values = barcodeMatrix[row][column + 1].GetValue();
                    int codewordIndex = row * detectionResult.BarcodeColumnCount + column;
                    if (values.Length == 0)
                    {
                        erasures.Add(codewordIndex);
                    }
                    else if (values.Length == 1)
                    {
                        codewords[codewordIndex] = values[0];
                    }
                    else
                    {
                        ambiguousIndexesList.Add(codewordIndex);
                        ambiguousIndexValuesList.Add(values);
                    }
                }
            }
            int[][] ambiguousIndexValues = new int[ambiguousIndexValuesList.Count][];
            for (int i = 0; i < ambiguousIndexValues.Length; i++)
            {
                ambiguousIndexValues[i] = ambiguousIndexValuesList[i];
            }
            return CreateDecoderResultFromAmbiguousValues(detectionResult.BarcodeECLevel, codewords, Pdf417Common.ToIntArray(erasures),
                        Pdf417Common.ToIntArray(ambiguousIndexesList), ambiguousIndexValues);
        }

        /// <summary>
        /// This method deals with the fact, that the decoding process doesn't always yield a single most likely value. The
        /// current error correction implementation doesn't deal with erasures very well, so it's better to provide a value
        /// for these ambiguous codewords instead of treating it as an erasure. The problem is that we don't know which of
        /// the ambiguous values to choose. We try decode using the first value, and if that fails, we use another of the
        /// ambiguous values and try to decode again. This usually only happens on very hard to read and decode barcodes,
        /// so decoding the normal barcodes is not affected by this. </summary>
        /// <param name="ecLevel"> </param>
        /// <param name="codewords"> </param>
        /// <param name="erasureArray"> contains the indexes of erasures </param>
        /// <param name="ambiguousIndexes"> array with the indexes that have more than one most likely value </param>
        /// <param name="ambiguousIndexValues"> two dimensional array that contains the ambiguous values. The first dimension must
        /// be the same length as the ambiguousIndexes array
        /// </param>
        private static DecoderResult CreateDecoderResultFromAmbiguousValues(int ecLevel, int[] codewords, int[] erasureArray, int[] ambiguousIndexes, int[][] ambiguousIndexValues)
        {
            int[] ambiguousIndexCount = new int[ambiguousIndexes.Length];

            int tries = 100;
            while (tries-- > 0)
            {
                for (int i = 0; i < ambiguousIndexCount.Length; i++)
                {
                    codewords[ambiguousIndexes[i]] = ambiguousIndexValues[i][ambiguousIndexCount[i]];
                }
                try
                {
                    return DecodeCodewords(codewords, ecLevel, erasureArray);
                }
                catch (ChecksumException ignored)
                {
                    //
                }
                if (ambiguousIndexCount.Length == 0)
                {
                    throw ChecksumException.Instance;
                }
                for (int i = 0; i < ambiguousIndexCount.Length; i++)
                {
                    if (ambiguousIndexCount[i] < ambiguousIndexValues[i].Length - 1)
                    {
                        ambiguousIndexCount[i]++;
                        break;
                    }
                    else
                    {
                        ambiguousIndexCount[i] = 0;
                        if (i == ambiguousIndexCount.Length - 1)
                        {
                            throw ChecksumException.Instance;
                        }
                    }
                }
            }
            throw ChecksumException.Instance;
        }

        internal static BarcodeValue[][] ReturnRectangularBarcodeValueArray(int size1, int size2)
        {
            BarcodeValue[][] arr = new BarcodeValue[size1][];
            for (int arr1 = 0; arr1 < size1; arr1++)
            {
                arr[arr1] = new BarcodeValue[size2];
            }
            return arr;
        }

        private static BarcodeValue[][] CreateBarcodeMatrix(DetectionResult detectionResult)
        {
            BarcodeValue[][] barcodeMatrix = ReturnRectangularBarcodeValueArray(detectionResult.BarcodeRowCount, detectionResult.BarcodeColumnCount + 2);
            for (int row = 0; row < barcodeMatrix.Length; row++)
            {
                for (int col = 0; col < barcodeMatrix[row].Length; col++)
                {
                    barcodeMatrix[row][col] = new BarcodeValue();
                }
            }

            int column = -1;
            foreach (DetectionResultColumn detectionResultColumn in detectionResult.DetectionResultColumns)
            {
                column++;
                if (detectionResultColumn == null)
                {
                    continue;
                }
                foreach (Codeword codeword in detectionResultColumn.Codewords)
                {
                    if (codeword == null || codeword.RowNumber == -1)
                    {
                        continue;
                    }
                    barcodeMatrix[codeword.RowNumber][column].SetValue(codeword.Value);
                }
            }
            return barcodeMatrix;
        }

        private static bool IsValidBarcodeColumn(DetectionResult detectionResult, int barcodeColumn)
        {
            return barcodeColumn >= 0 && barcodeColumn <= detectionResult.BarcodeColumnCount + 1;
        }

        private static int GetStartColumn(DetectionResult detectionResult, int barcodeColumn, int imageRow, bool leftToRight)
        {
            int offset = leftToRight ? 1 : -1;
            Codeword codeword = null;
            if (IsValidBarcodeColumn(detectionResult, barcodeColumn - offset))
            {
                codeword = detectionResult.GetDetectionResultColumn(barcodeColumn - offset).GetCodeword(imageRow);
            }
            if (codeword != null)
            {
                return leftToRight ? codeword.EndX : codeword.StartX;
            }
            codeword = detectionResult.GetDetectionResultColumn(barcodeColumn).GetCodewordNearby(imageRow);
            if (codeword != null)
            {
                return leftToRight ? codeword.StartX : codeword.EndX;
            }
            if (IsValidBarcodeColumn(detectionResult, barcodeColumn - offset))
            {
                codeword = detectionResult.GetDetectionResultColumn(barcodeColumn - offset).GetCodewordNearby(imageRow);
            }
            if (codeword != null)
            {
                return leftToRight ? codeword.EndX : codeword.StartX;
            }
            int skippedColumns = 0;

            while (IsValidBarcodeColumn(detectionResult, barcodeColumn - offset))
            {
                barcodeColumn -= offset;
                foreach (Codeword previousRowCodeword in detectionResult.GetDetectionResultColumn(barcodeColumn).Codewords)
                {
                    if (previousRowCodeword != null)
                    {
                        return (leftToRight ? previousRowCodeword.EndX : previousRowCodeword.StartX) + offset * skippedColumns * (previousRowCodeword.EndX - previousRowCodeword.StartX);
                    }
                }
                skippedColumns++;
            }
            return leftToRight ? detectionResult.BoundingBox.MinX : detectionResult.BoundingBox.MaxX;
        }

        private static Codeword DetectCodeword(BitMatrix image, int minColumn, int maxColumn, bool leftToRight, int startColumn, int imageRow, int minCodewordWidth, int maxCodewordWidth)
        {
            startColumn = AdjustCodewordStartColumn(image, minColumn, maxColumn, leftToRight, startColumn, imageRow);
            // we usually know fairly exact now how long a codeword is. We should provide minimum and maximum expected length
            // and try to adjust the read pixels, e.g. remove single pixel errors or try to cut off exceeding pixels.
            // min and maxCodewordWidth should not be used as they are calculated for the whole barcode an can be inaccurate
            // for the current position
            int[] moduleBitCount = GetModuleBitCount(image, minColumn, maxColumn, leftToRight, startColumn, imageRow);
            if (moduleBitCount == null)
            {
                return null;
            }
            int endColumn;
            int codewordBitCount = Pdf417Common.GetBitCountSum(moduleBitCount);
            if (leftToRight)
            {
                endColumn = startColumn + codewordBitCount;
            }
            else
            {
                for (int i = 0; i < (moduleBitCount.Length >> 1); i++)
                {
                    int tmpCount = moduleBitCount[i];
                    moduleBitCount[i] = moduleBitCount[moduleBitCount.Length - 1 - i];
                    moduleBitCount[moduleBitCount.Length - 1 - i] = tmpCount;
                }
                endColumn = startColumn;
                startColumn = endColumn - codewordBitCount;
            }
            // TODO implement check for width and correction of black and white bars
            // use start (and maybe stop pattern) to determine if blackbars are wider than white bars. If so, adjust.
            // should probably done only for codewords with a lot more than 17 bits. 
            // The following fixes 10-1.png, which has wide black bars and small white bars
            //    for (int i = 0; i < moduleBitCount.length; i++) {
            //      if (i % 2 == 0) {
            //        moduleBitCount[i]--;
            //      } else {
            //        moduleBitCount[i]++;
            //      }
            //    }

            // We could also use the width of surrounding codewords for more accurate results, but this seems
            // sufficient for now
            if (!CheckCodewordSkew(codewordBitCount, minCodewordWidth, maxCodewordWidth))
            {
                // We could try to use the startX and endX position of the codeword in the same column in the previous row,
                // create the bit count from it and normalize it to 8. This would help with single pixel errors.
                return null;
            }

            int decodedValue = Pdf417CodewordDecoder.GetDecodedValue(moduleBitCount);
            int codeword = Pdf417Common.GetCodeword(decodedValue);
            if (codeword == -1)
            {
                return null;
            }
            return new Codeword(startColumn, endColumn, GetCodewordBucketNumber(decodedValue), codeword);
        }

        private static int[] GetModuleBitCount(BitMatrix image, int minColumn, int maxColumn, bool leftToRight, int startColumn, int imageRow)
        {
            int imageColumn = startColumn;
            int[] moduleBitCount = new int[8];
            int moduleNumber = 0;
            int increment = leftToRight ? 1 : -1;
            bool previousPixelValue = leftToRight;
            while (((leftToRight && imageColumn < maxColumn) || (!leftToRight && imageColumn >= minColumn)) && moduleNumber < moduleBitCount.Length)
            {
                if (image.Get(imageColumn, imageRow) == previousPixelValue)
                {
                    moduleBitCount[moduleNumber]++;
                    imageColumn += increment;
                }
                else
                {
                    moduleNumber++;
                    previousPixelValue = !previousPixelValue;
                }
            }
            if (moduleNumber == moduleBitCount.Length || (((leftToRight && imageColumn == maxColumn) || (!leftToRight && imageColumn == minColumn)) && moduleNumber == moduleBitCount.Length - 1))
            {
                return moduleBitCount;
            }
            return null;
        }

        private static int GetNumberOfECCodeWords(int barcodeECLevel)
        {
            return 2 << barcodeECLevel;
        }

        private static int AdjustCodewordStartColumn(BitMatrix image, int minColumn, int maxColumn, bool leftToRight, int codewordStartColumn, int imageRow)
        {
            int correctedStartColumn = codewordStartColumn;
            int increment = leftToRight ? -1 : 1;
            // there should be no black pixels before the start column. If there are, then we need to start earlier.
            for (int i = 0; i < 2; i++)
            {
                while (((leftToRight && correctedStartColumn >= minColumn) || (!leftToRight && correctedStartColumn < maxColumn)) && leftToRight == image.Get(correctedStartColumn, imageRow))
                {
                    if (Math.Abs(codewordStartColumn - correctedStartColumn) > CODEWORD_SKEW_SIZE)
                    {
                        return codewordStartColumn;
                    }
                    correctedStartColumn += increment;
                }
                increment = -increment;
                leftToRight = !leftToRight;
            }
            return correctedStartColumn;
        }

        private static bool CheckCodewordSkew(int codewordSize, int minCodewordWidth, int maxCodewordWidth)
        {
            return minCodewordWidth - CODEWORD_SKEW_SIZE <= codewordSize && codewordSize <= maxCodewordWidth + CODEWORD_SKEW_SIZE;
        }

        private static DecoderResult DecodeCodewords(int[] codewords, int ecLevel, int[] erasures)
        {
            if (codewords.Length == 0)
            {
                throw FormatException.Instance;
            }

            int numECCodewords = 1 << (ecLevel + 1);
            int correctedErrorsCount = CorrectErrors(codewords, erasures, numECCodewords);
            VerifyCodewordCount(codewords, numECCodewords);

            // Decode the codewords
            DecoderResult decoderResult = DecodedBitStreamParser.Decode(codewords, Convert.ToString(ecLevel));
            decoderResult.ErrorsCorrected = correctedErrorsCount;
            decoderResult.Erasures = erasures.Length;
            return decoderResult;
        }

        /// <summary>
        /// <p>Given data and error-correction codewords received, possibly corrupted by errors, attempts to
        /// correct the errors in-place.</p>
        /// </summary>
        /// <param name="codewords">   data and error correction codewords </param>
        /// <param name="erasures"> positions of any known erasures </param>
        /// <param name="numECCodewords"> number of error correction codewords that are available in codewords </param>
        /// <exception cref="ChecksumException"> if error correction fails </exception>
        private static int CorrectErrors(int[] codewords, int[] erasures, int numECCodewords)
        {
            if (erasures != null && erasures.Length > numECCodewords / 2 + MAX_ERRORS || numECCodewords < 0 || numECCodewords > MAX_EC_CODEWORDS)
            {
                // Too many errors or EC Codewords is corrupted
                throw ChecksumException.Instance;
            }
            return errorCorrection.Decode(codewords, numECCodewords, erasures);
        }

        /// <summary>
        /// Verify that all is OK with the codeword array.
        /// </summary>
        /// <param name="codewords"> </param>
        /// <returns> an index to the first data codeword. </returns>
        private static void VerifyCodewordCount(int[] codewords, int numECCodewords)
        {
            if (codewords.Length < 4)
            {
                // Codeword array size should be at least 4 allowing for
                // Count CW, At least one Data CW, Error Correction CW, Error Correction CW
                throw FormatException.Instance;
            }
            // The first codeword, the Symbol Length Descriptor, shall always encode the total number of data
            // codewords in the symbol, including the Symbol Length Descriptor itself, data codewords and pad
            // codewords, but excluding the number of error correction codewords.
            int numberOfCodewords = codewords[0];
            if (numberOfCodewords > codewords.Length)
            {
                throw FormatException.Instance;
            }
            if (numberOfCodewords == 0)
            {
                // Reset to the length of the array - 8 (Allow for at least level 3 Error Correction (8 Error Codewords)
                if (numECCodewords < codewords.Length)
                {
                    codewords[0] = codewords.Length - numECCodewords;
                }
                else
                {
                    throw FormatException.Instance;
                }
            }
        }

        private static int[] GetBitCountForCodeword(int codeword)
        {
            int[] result = new int[8];
            int previousValue = 0;
            int i = result.Length - 1;
            while (true)
            {
                if ((codeword & 0x1) != previousValue)
                {
                    previousValue = codeword & 0x1;
                    i--;
                    if (i < 0)
                    {
                        break;
                    }
                }
                result[i]++;
                codeword >>= 1;
            }
            return result;
        }

        private static int GetCodewordBucketNumber(int codeword)
        {
            return GetCodewordBucketNumber(GetBitCountForCodeword(codeword));
        }

        private static int GetCodewordBucketNumber(int[] moduleBitCount)
        {
            return (moduleBitCount[0] - moduleBitCount[2] + moduleBitCount[4] - moduleBitCount[6] + 9) % 9;
        }

        public static string ToString(BarcodeValue[][] barcodeMatrix)
        {
            StringBuilder formatter = new StringBuilder();
            for (int row = 0; row < barcodeMatrix.Length; row++)
            {
                formatter.Append(String.Format("Row %2d: ", row));
                for (int column = 0; column < barcodeMatrix[row].Length; column++)
                {
                    BarcodeValue barcodeValue = barcodeMatrix[row][column];
                    if (barcodeValue.GetValue().Length == 0)
                    {
                        //formatter.format("        ", (Object[]) null);
                        formatter.Append("        ");
                    }
                    else
                    {
                        formatter.Append(String.Format("%4d(%2d)", barcodeValue.GetValue()[0], barcodeValue.GetConfidence(barcodeValue.GetValue()[0])));
                    }
                }
                formatter.Append("\n");
            }
            string result = formatter.ToString();
            return result;
        }


    }
}
