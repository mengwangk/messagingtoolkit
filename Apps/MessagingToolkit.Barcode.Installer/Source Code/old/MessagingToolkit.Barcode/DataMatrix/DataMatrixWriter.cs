using MessagingToolkit.Barcode.Common;
using MessagingToolkit.Barcode.DataMatrix.Encoder;
using MessagingToolkit.Barcode.Helper;
using MessagingToolkit.Barcode.QRCode.Encoder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MessagingToolkit.Barcode.DataMatrix
{
    public sealed class DataMatrixWriter : IEncoder
    {
        public BitMatrix Encode(string contents, BarcodeFormat format, int width, int height)
        {
            return Encode(contents, format, width, height, null);
        }

        public BitMatrix Encode(string contents, BarcodeFormat format, int width, int height, Dictionary<EncodeOptions, object> encodingOptions)
        {

            if (contents.Length == 0)
            {
                throw new ArgumentException("Found empty contents");
            }

            if (format != BarcodeFormat.DataMatrix)
            {
                throw new ArgumentException("Can only encode DATA_MATRIX, but got " + format);
            }

            if (width < 0 || height < 0)
            {
                throw new System.ArgumentException("Requested dimensions are too small: " + width + 'x' + height);
            }

            // Try to get force shape & min / max size
            SymbolShapeHint shape = SymbolShapeHint.ForceNone;
            Dimension minSize = null;
            Dimension maxSize = null;
            if (encodingOptions != null)
            {
                object obj = BarcodeHelper.GetEncodeOptionType(encodingOptions, EncodeOptions.DataMatrixShape);
                if (obj != null)
                {
                    shape = (SymbolShapeHint)obj;
                }

                obj = BarcodeHelper.GetEncodeOptionType(encodingOptions, EncodeOptions.MinimumSize);
                if (obj != null)
                {
                    minSize = (Dimension)obj;
                }

                obj = BarcodeHelper.GetEncodeOptionType(encodingOptions, EncodeOptions.MaximumSize);
                if (obj != null)
                {
                    maxSize = (Dimension)obj;
                }
            }


            //1. step: Data encodation
            string encoded = HighLevelEncoder.EncodeHighLevel(contents, shape, minSize, maxSize, encodingOptions);

            SymbolInfo symbolInfo = SymbolInfo.Lookup(encoded.Length, shape, minSize, maxSize, true);

            //2. step: ECC generation
            string codewords = ErrorCorrection.EncodeECC200(encoded, symbolInfo);

            //3. step: Module placement in Matrix
            DefaultPlacement placement = new DefaultPlacement(codewords, symbolInfo.SymbolDataWidth, symbolInfo.SymbolDataHeight);
            placement.Place();

            //4. step: low-level encoding
            return EncodeLowLevel(placement, symbolInfo);
        }

        /// <summary>
        /// Encode the given symbol info to a bit matrix.
        /// </summary>
        /// <param name="placement">  The DataMatrix placement. </param>
        /// <param name="symbolInfo"> The symbol info to encode. </param>
        /// <returns> The bit matrix generated. </returns>
        private static BitMatrix EncodeLowLevel(DefaultPlacement placement, SymbolInfo symbolInfo)
        {
            int symbolWidth = symbolInfo.SymbolDataWidth;
            int symbolHeight = symbolInfo.SymbolDataHeight;

            ByteMatrix matrix = new ByteMatrix(symbolInfo.SymbolWidth, symbolInfo.SymbolHeight);

            int matrixY = 0;

            for (int y = 0; y < symbolHeight; y++)
            {
                // Fill the top edge with alternate 0 / 1
                int matrixX;
                if ((y % symbolInfo.matrixHeight) == 0)
                {
                    matrixX = 0;
                    for (int x = 0; x < symbolInfo.SymbolWidth; x++)
                    {
                        matrix.Set(matrixX, matrixY, (x % 2) == 0);
                        matrixX++;
                    }
                    matrixY++;
                }
                matrixX = 0;
                for (int x = 0; x < symbolWidth; x++)
                {
                    // Fill the right edge with full 1
                    if ((x % symbolInfo.matrixWidth) == 0)
                    {
                        matrix.Set(matrixX, matrixY, true);
                        matrixX++;
                    }
                    matrix.Set(matrixX, matrixY, placement.GetBit(x, y));
                    matrixX++;
                    // Fill the right edge with alternate 0 / 1
                    if ((x % symbolInfo.matrixWidth) == symbolInfo.matrixWidth - 1)
                    {
                        matrix.Set(matrixX, matrixY, (y % 2) == 0);
                        matrixX++;
                    }
                }
                matrixY++;
                // Fill the bottom edge with full 1
                if ((y % symbolInfo.matrixHeight) == symbolInfo.matrixHeight - 1)
                {
                    matrixX = 0;
                    for (int x = 0; x < symbolInfo.SymbolWidth; x++)
                    {
                        matrix.Set(matrixX, matrixY, true);
                        matrixX++;
                    }
                    matrixY++;
                }
            }

            return ConvertByteMatrixToBitMatrix(matrix);
        }

        /// <summary>
        /// Convert the ByteMatrix to BitMatrix.
        /// </summary>
        /// <param name="matrix"> The input matrix. </param>
        /// <returns> The output matrix. </returns>
        private static BitMatrix ConvertByteMatrixToBitMatrix(ByteMatrix matrix)
        {
            int matrixWidgth = matrix.Width;
            int matrixHeight = matrix.Height;

            BitMatrix output = new BitMatrix(matrixWidgth, matrixHeight);
            output.Clear();
            for (int i = 0; i < matrixWidgth; i++)
            {
                for (int j = 0; j < matrixHeight; j++)
                {
                    // Zero is white in the bytematrix
                    if (matrix.Get(i, j) == 1)
                    {
                        output.Set(i, j);
                    }
                }
            }

            return output;
        }
    }
}
