using MessagingToolkit.Barcode.Common;
using MessagingToolkit.Barcode.QRCode.Decoder;
using MessagingToolkit.Barcode.QRCode.Encoder;
using MessagingToolkit.Barcode.Helper;

using System;
using System.Collections.Generic;

namespace MessagingToolkit.Barcode.QRCode
{
    /// <summary>
    /// This object renders a QR Code as a BitMatrix 2D array of greyscale values.
    /// 
    /// Modified: April 29 2012
    /// </summary>
    public sealed class QRCodeEncoder : IEncoder
    {
        // Default quiet zone
        private const int QUIET_ZONE_SIZE = 4;

        public BitMatrix Encode(String contents, BarcodeFormat format, int width, int height)
        {
            return Encode(contents, format, width, height, null);
        }

        public BitMatrix Encode(String contents, BarcodeFormat format, int width, int height, IDictionary<EncodeOptions, object> encodingOptions)
        {
            if (string.IsNullOrEmpty(contents))
            {
                throw new ArgumentException("Found empty contents");
            }

            if (format != BarcodeFormat.QRCode)
            {
                throw new ArgumentException("Can only Encode QR_CODE, but got " + format);
            }

            if (width < 0 || height < 0)
            {
                throw new ArgumentException("Requested dimensions are too small: " + width + 'x' +
                    height);
            }

            ErrorCorrectionLevel errorCorrectionLevel = ErrorCorrectionLevel.L;
            int quietZone = QUIET_ZONE_SIZE;
            if (encodingOptions != null)
            {
                ErrorCorrectionLevel requestedECLevel = (ErrorCorrectionLevel)BarcodeHelper.GetEncodeOptionType(encodingOptions, EncodeOptions.ErrorCorrection);
                if (requestedECLevel != null)
                {
                    errorCorrectionLevel = requestedECLevel;
                }

                object obj = BarcodeHelper.GetEncodeOptionType(encodingOptions, EncodeOptions.Margin);
                if (obj != null)
                {
                    quietZone = Convert.ToInt32(obj);
                }
               
            }

            MessagingToolkit.Barcode.QRCode.Encoder.QRCode code = MessagingToolkit.Barcode.QRCode.Encoder.Encoder.Encode(contents, errorCorrectionLevel, encodingOptions);

            return RenderResult(code, width, height, quietZone);
        }

        // Note that the input matrix uses 0 == white, 1 == black, while the output matrix uses
        // 0 == black, 255 == white (i.e. an 8 bit greyscale bitmap).
        private static BitMatrix RenderResult(MessagingToolkit.Barcode.QRCode.Encoder.QRCode code, int width, int height, int quietZone)
        {
            ByteMatrix input = code.Matrix;
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
