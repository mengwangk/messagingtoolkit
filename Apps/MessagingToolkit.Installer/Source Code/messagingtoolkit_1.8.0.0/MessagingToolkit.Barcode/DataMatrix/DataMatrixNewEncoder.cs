using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MessagingToolkit.Barcode.Common;
using MessagingToolkit.Barcode.Helper;
using MessagingToolkit.Barcode.QRCode.Encoder;
using MessagingToolkit.Barcode.DataMatrix.Encoder;

namespace MessagingToolkit.Barcode.DataMatrix
{
    public sealed class DataMatrixNewEncoder : IEncoder
    {
        // Default quiet zone
        private const int QUIET_ZONE_SIZE = 4;


        /// <summary>
        /// Encode a barcode using the default settings.
        /// </summary>
        /// <param name="contents">The contents to encode in the barcode</param>
        /// <param name="format">The barcode format to generate</param>
        /// <param name="width">The preferred width in pixels</param>
        /// <param name="height">The preferred height in pixels</param>
        /// <returns>
        /// The generated barcode as a Matrix of unsigned bytes (0 == black, 255 == white)
        /// </returns>
        public BitMatrix Encode(string contents, BarcodeFormat format, int width, int height)
        {
            return Encode(contents, format, width, height, null);
        }

        /// <summary>
        /// Encodes the specified contents.
        /// </summary>
        /// <param name="contents">The contents to encode in the barcode</param>
        /// <param name="format">The barcode format to generate</param>
        /// <param name="width">The preferred width in pixels</param>
        /// <param name="height">The preferred height in pixels</param>
        /// <param name="encodingOptions">The encoding options.</param>
        /// <returns>
        /// The generated barcode as a Matrix of unsigned bytes (0 == black, 255 == white)
        /// </returns>
        /// <exception cref="System.ArgumentException">Found empty contents</exception>
        public BitMatrix Encode(string contents, BarcodeFormat format, int width, int height, Dictionary<EncodeOptions, object> encodingOptions)
        {

            if (string.IsNullOrEmpty(contents))
            {
                throw new ArgumentException("Found empty contents");
            }

            if (format != BarcodeFormat.DataMatrix)
            {
                throw new ArgumentException("Can only encode Data Matrix, but got " + format);
            }

            if (width < 0 || height < 0)
            {
                throw new ArgumentException("Requested dimensions are too small: " + width + 'x' + height);
            }

            // Try to get force shape & min / max size
            SymbolShapeHint shape = SymbolShapeHint.ForceNone;
            Dimension minSize = null;
            Dimension maxSize = null;
            int quietZone = QUIET_ZONE_SIZE;
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

                obj = BarcodeHelper.GetEncodeOptionType(encodingOptions, EncodeOptions.Margin);
                if (obj != null)
                {
                    quietZone = Convert.ToInt32(obj);
                }
            }

            // 1. step: Data encoding
            string encoded;
            try
            {
                encoded = HighLevelEncoder.EncodeHighLevel(contents, shape, minSize, maxSize, encodingOptions);
            }
            catch (Exception e)
            {
                throw new ArgumentException("Cannot fetch data: " + e.Message);
            }

            SymbolInfo symbolInfo = SymbolInfo.Lookup(encoded.Length, shape, minSize, maxSize, true);

            // 2. step: ECC generation
            string codewords = ErrorCorrection.EncodeECC200(encoded, symbolInfo);

            // 3. step: Module placement in Matrix
            DefaultPlacement placement = new DefaultPlacement(codewords, symbolInfo.SymbolDataWidth, symbolInfo.SymbolDataHeight);
            placement.Place();

            // 4. step: low-level encoding
            ByteMatrix byteMatrix = EncodeLowLevel(placement, symbolInfo);

            // 5. step: return the result in bit matrix
            return RenderResult(byteMatrix, width, height, quietZone);
        }

        /// <summary>
        /// Encode the given symbol info to a bit matrix.
        /// </summary>
        /// <param name="placement"> The DataMatrix placement. </param>
        /// <param name="symbolInfo"> The symbol info to encode. </param>
        /// <returns> The byte matrix generated. </returns>
        private ByteMatrix EncodeLowLevel(DefaultPlacement placement, SymbolInfo symbolInfo)
        {
            int symbolWidth = symbolInfo.SymbolDataWidth;
            int symbolHeight = symbolInfo.SymbolDataHeight;
            ByteMatrix matrix = new ByteMatrix(symbolInfo.SymbolWidth, symbolInfo.SymbolHeight);
            int matrixX = 0;
            int matrixY = 0;
            for (int y = 0; y < symbolHeight; y++)
            {
                // Fill the top edge with alternate 0 / 1
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

            return matrix;
        }

        /// <summary>
        /// Convert the ByteMatrix to BitMatrix.
        /// </summary>
        /// <param name="matrix"> The input matrix. </param>
        /// <returns> The output matrix. </returns>
        private BitMatrix ConvertByteMatrixToBitMatrix(ByteMatrix matrix)
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

        /// <summary>
        /// Renders the result.
        /// Note that the input matrix uses 0 == white, 1 == black, while the output matrix uses
        /// 0 == black, 255 == white (i.e. an 8 bit greyscale bitmap).
        /// </summary>
        /// <param name="byteMatrix">The byte matrix.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="quietZone">The quiet zone.</param>
        /// <returns></returns>
        /// <exception cref="System.InvalidOperationException"></exception>
        private static BitMatrix RenderResult(ByteMatrix input, int width, int height, int quietZone)
        {
            if (input == null)
            {
                throw new InvalidOperationException();
            }
            int inputWidth = input.Width;
            int inputHeight = input.Height;
            int qrWidth = inputWidth + (quietZone << 1);
            int qrHeight = inputHeight + (quietZone << 1);
            int outputWidth = Math.Max(width, qrWidth);
            int outputHeight = Math.Max(height, qrHeight);

            int multiple = Math.Min(outputWidth / qrWidth, outputHeight / qrHeight);
            // Padding includes both the quiet zone and the extra white pixels to accommodate the requested
            // dimensions. For example, if input is 25x25 the QR will be 33x33 including the quiet zone.
            // If the requested size is 200x160, the multiple will be 4, for a QR of 132x132. These will
            // handle all the padding from 100x100 (the actual QR) up to 200x160.
            int leftPadding = (outputWidth - (inputWidth * multiple)) / 2;
            int topPadding = (outputHeight - (inputHeight * multiple)) / 2;

            BitMatrix output = new BitMatrix(outputWidth, outputHeight);
            output.LeftPadding = leftPadding;
            output.TopPadding = topPadding;
            output.ActualHeight = multiple * inputHeight;
            output.ActualWidth = multiple * inputWidth;

            for (int inputY = 0, outputY = topPadding; inputY < inputHeight; inputY++, outputY += multiple)
            {
                // Write the contents of this row of the barcode
                for (int inputX = 0, outputX = leftPadding; inputX < inputWidth; inputX++, outputX += multiple)
                {
                    if (input.Get(inputX, inputY) == 1)
                    {
                        output.SetRegion(outputX, outputY, multiple, multiple);
                    }
                }
            }
            return output;
        }

    }
}