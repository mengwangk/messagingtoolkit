using MessagingToolkit.Barcode.Common;
using MessagingToolkit.Barcode.Pdf417.Encoder;
using MessagingToolkit.Barcode.Helper;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;

namespace MessagingToolkit.Barcode.Pdf417
{

    public sealed class Pdf417Encoder : IEncoder
    {
        public BitMatrix Encode(String contents, BarcodeFormat format, int width, int height, Dictionary<EncodeOptions, object> encodingOptions)
        {
            if (format != BarcodeFormat.PDF417)
            {
                throw new ArgumentException("Can only encode PDF_417, but got " + format);
            }

            Pdf417.Encoder.Pdf417 encoder = new Pdf417.Encoder.Pdf417();

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
            }

            return BitMatrixFromEncoder(encoder, contents, width, height);
        }

        public BitMatrix Encode(String contents, BarcodeFormat format, int width, int height)
        {
            return Encode(contents, format, width, height, null);
        }

        public BitMatrix Encode(String contents, BarcodeFormat format, bool compact, int width, int height, int minCols, int maxCols, int minRows, int maxRows, Compaction compaction)
        {
            Dictionary<EncodeOptions, object> encodingOptions = new Dictionary<EncodeOptions, object>();
            encodingOptions.Add(EncodeOptions.Pdf417Compact, compact);
            encodingOptions.Add(EncodeOptions.Pdf417Compaction, compaction);
            encodingOptions.Add(EncodeOptions.Pdf417Dimensions, new Dimensions(minCols, maxCols, minRows, maxRows));
            return Encode(contents, format, width, height, encodingOptions);
        }

        /// <summary>
        /// Takes encoder, accounts for width/height, and retrieves bit matrix
        /// </summary>
        ///
        private static BitMatrix BitMatrixFromEncoder(Pdf417.Encoder.Pdf417 encoder, String contents, int width, int height)
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

            if (scale > 1)
            {
                byte[][] scaledMatrix = encoder.GetBarcodeMatrix().GetScaledMatrix(scale * lineThickness, scale * aspectRatio * lineThickness);
                if (rotated)
                {
                    scaledMatrix = RotateArray(scaledMatrix);
                }
                return BitMatrixFrombitArray(scaledMatrix);
            }
            return BitMatrixFrombitArray(originalScale);
        }

        /// <summary>
        /// This takes an array holding the values of the PDF 417
        /// </summary>
        ///
        /// <param name="input">a byte array of information with 0 is black, and 1 is white</param>
        /// <returns>BitMatrix of the input</returns>
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
