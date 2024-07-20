using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;


using MessagingToolkit.Barcode.Common;
using MessagingToolkit.Barcode.Pdf417.Encoder;
using MessagingToolkit.Barcode.Helper;
using MessagingToolkit.Barcode.QRCode.Encoder;


namespace MessagingToolkit.Barcode.Pdf417
{

    /// <summary>
    /// PDF417 encoder.
    /// </summary>
    public sealed class Pdf417Encoder : IEncoder
    {
        // Default quiet zone
        private const int QUIET_ZONE_SIZE = 4;

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
        /// <exception cref="System.ArgumentException">Can only encode PDF417, but got  + format</exception>
        public BitMatrix Encode(String contents, BarcodeFormat format, int width, int height, Dictionary<EncodeOptions, object> encodingOptions)
        {
            if (format != BarcodeFormat.PDF417)
            {
                throw new ArgumentException("Can only encode PDF417, but got " + format);
            }

            Pdf417.Encoder.Pdf417 encoder = new Pdf417.Encoder.Pdf417();
            int quietZone = QUIET_ZONE_SIZE;
            if (encodingOptions != null)
            {
                if (encodingOptions.ContainsKey(EncodeOptions.Pdf417Compact))
                {
                    object obj = BarcodeHelper.GetEncodeOptionType(encodingOptions, EncodeOptions.Pdf417Compact);
                    encoder.SetCompact((bool)obj);
                }
                if (encodingOptions.ContainsKey(EncodeOptions.Pdf417Compaction))
                {
                    object obj = BarcodeHelper.GetEncodeOptionType(encodingOptions, EncodeOptions.Pdf417Compaction);
                    encoder.SetCompaction((Compaction)obj);
                }
                if (encodingOptions.ContainsKey(EncodeOptions.Pdf417Dimensions))
                {
                    Dimensions dimensions = (Dimensions)BarcodeHelper.GetEncodeOptionType(encodingOptions, EncodeOptions.Pdf417Dimensions); ;
                    encoder.SetDimensions(dimensions.MaxCols, dimensions.MinCols, dimensions.MaxRows, dimensions.MinRows);
                }

                if (encodingOptions.ContainsKey(EncodeOptions.Margin))
                {
                    object obj = BarcodeHelper.GetEncodeOptionType(encodingOptions, EncodeOptions.Margin);
                    quietZone = Convert.ToInt32(obj);
                }
            }

            return BitMatrixFromEncoder(encoder, contents, width, height, quietZone);
        }

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
        public BitMatrix Encode(String contents, BarcodeFormat format, int width, int height)
        {
            return Encode(contents, format, width, height, null);
        }

        /// <summary>
        /// Takes encoder, accounts for width/height, and retrieves bit matrix
        /// </summary>
        /// <param name="encoder">The encoder.</param>
        /// <param name="contents">The contents.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="quietZone">The quiet zone.</param>
        /// <returns></returns>
        private static BitMatrix BitMatrixFromEncoder(Pdf417.Encoder.Pdf417 encoder, String contents, int width, int height, int quietZone)
        {
            int errorCorrectionLevel = 2;
            encoder.GenerateBarcodeLogic(contents, errorCorrectionLevel);

            int lineThickness = 2;
            int aspectRatio = 4;
            byte[][] originalScale = encoder.GetBarcodeMatrix().GetScaledMatrix(lineThickness, aspectRatio * lineThickness);
            bool rotated = false;
            if ((height > width) ^ (originalScale[0].Length < originalScale.Length))
            {
                originalScale = RotateArray(originalScale);
                rotated = true;
            }

            int scaleX = width / originalScale[0].Length;
            int scaleY = height / originalScale.Length;

            int scale;
            if (scaleX < scaleY)
            {
                scale = scaleX;
            }
            else
            {
                scale = scaleY;
            }

            ByteMatrix input = null;
            if (scale > 1)
            {
                byte[][] scaledMatrix = encoder.GetBarcodeMatrix().GetScaledMatrix(scale * lineThickness, scale * aspectRatio * lineThickness);
                if (rotated)
                {
                    scaledMatrix = RotateArray(scaledMatrix);
                }
                input = new ByteMatrix(scaledMatrix[0].Length, scaledMatrix.Length, scaledMatrix);
                return RenderResult(input, contents, width, height, quietZone);
            }

            input = new ByteMatrix(originalScale[0].Length, originalScale.Length, originalScale);
            return RenderResult(input, contents, width, height, quietZone);
        }

        /// <summary>
        /// Render the results
        /// </summary>
        private static BitMatrix RenderResult(ByteMatrix input, String contents, int width, int height, int quietZone)
        {
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




        /*
        private static BitMatrix RenderResult(Pdf417.Encoder.Pdf417 encoder, String contents, int width, int height, int quietZone)
        {
            int errorCorrectionLevel = 2;
            encoder.GenerateBarcodeLogic(contents, errorCorrectionLevel);

            byte[][] data = encoder.GetBarcodeMatrix().GetMatrix();
            ByteMatrix input = new ByteMatrix(data[0].Length, data.Length, data);
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
        */

        /// <summary>
        /// This takes an array holding the values of the PDF 417
        /// </summary>
        ///
        /// <param name="input">a byte array of information with 0 is black, and 1 is white</param>
        /// <returns>BitMatrix of the input</returns>
        /*
        private static BitMatrix BitMatrixFrombitArray(byte[][] input)
        {
            // Creates a small whitespace border around the barcode
            int whiteSpace = 30;

            // Creates the bitmatrix with extra space for whitespace
            BitMatrix output = new BitMatrix(input[0].Length + 2 * whiteSpace, input.Length + 2 * whiteSpace);
            output.Clear();
            for (int y = 0, yOutput = output.Height - whiteSpace; y < input.Length; y++, yOutput--)
            {
                for (int x = 0; x < input[0].Length; x++)
                {
                    // Zero is white in the bytematrix
                    if (input[y][x] == 1)
                    {
                        output.Set(x + whiteSpace, yOutput);
                    }
                }
            }
            return output;
        }
        */

        /// <summary>
        /// Takes and rotates the it 90 degrees
        /// </summary>
        ///
        private static byte[][] RotateArray(byte[][] bitarray)
        {
            byte[][] temp = new byte[bitarray[0].Length][];
            for (int i = 0; i < bitarray[0].Length; i++)
            {
                temp[i] = new byte[bitarray.Length];
            }
            for (int ii = 0; ii < bitarray.Length; ii++)
            {
                // This makes the direction consistent on screen when rotating the
                // screen;
                int inverseii = bitarray.Length - ii - 1;
                for (int jj = 0; jj < bitarray[0].Length; jj++)
                {
                    temp[jj][inverseii] = bitarray[ii][jj];
                }
            }
            return temp;
        }

    }
}
