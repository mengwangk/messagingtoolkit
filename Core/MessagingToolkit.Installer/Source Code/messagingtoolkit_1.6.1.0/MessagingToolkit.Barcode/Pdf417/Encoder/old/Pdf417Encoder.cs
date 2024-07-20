using MessagingToolkit.Barcode.Common;
using MessagingToolkit.Barcode.Pdf417.Encoder;
using System;
using System.Collections.Generic;

namespace MessagingToolkit.Barcode.Pdf417
{

    public sealed class Pdf417Encoder : IEncoder
    {

        public BitMatrix Encode(String contents, BarcodeFormat format, int width, int height, 
            Dictionary<EncodeOptions, object> encodingOptions)
        {
            return Encode(contents, format, width, height);
        }

        public BitMatrix Encode(String contents, BarcodeFormat format, int width, int height)
        {
            Pdf417.Encoder.Pdf417 encoder = InitializeEncoder(format, false);
            return BitMatrixFromEncoder(encoder, contents, width, height);
        }

        public BitMatrix Encode(String contents, BarcodeFormat format,
                bool compact, int width, int height, int minCols, int maxCols,
                int minRows, int maxRows, bool byteCompaction)
        {
            Pdf417.Encoder.Pdf417 encoder = InitializeEncoder(format, compact);

            // Set options: dimensions and byte compaction
            encoder.SetDimensions(maxCols, minCols, maxRows, minRows);
            encoder.SetByteCompaction(byteCompaction);

            return BitMatrixFromEncoder(encoder, contents, width, height);
        }

        /// <summary>
        /// Initializes the encoder based on the format (whether it's compact or not)
        /// </summary>
        private static Pdf417.Encoder.Pdf417 InitializeEncoder(BarcodeFormat format, bool compact)
        {
            if (format != MessagingToolkit.Barcode.BarcodeFormat.PDF417)
            {
                throw new ArgumentException(
                        "Can only encode PDF_417, but got " + format);
            }

            Pdf417.Encoder.Pdf417 encoder = new Pdf417.Encoder.Pdf417();
            encoder.SetCompact(compact);
            return encoder;
        }

        /// <summary>
        /// Takes encoder, accounts for width/height, and retrieves bit matrix
        /// </summary>
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
                byte[][] scaledMatrix = encoder.GetBarcodeMatrix().GetScaledMatrix(
                        scale * lineThickness, scale * aspectRatio * lineThickness);
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
            //Creates a small whitespace boarder around the barcode
            int whiteSpace = 30;

            //Creates the bitmatrix with extra space for whtespace
            BitMatrix output = new BitMatrix(input.Length + 2 * whiteSpace,
                    input[0].Length + 2 * whiteSpace);
            output.Clear();
            for (int ii = 0; ii < input.Length; ii++)
            {
                for (int jj = 0; jj < input[0].Length; jj++)
                {
                    // Zero is white in the bytematrix
                    if (input[ii][jj] == 1)
                    {
                        output.Set(ii + whiteSpace, jj + whiteSpace);
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
            //byte[][] temp = (byte[][])ILOG.J2CsMapping.Collections.Arrays.CreateJaggedArray(typeof(byte), bitarray[0].Length, bitarray.Length);
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
