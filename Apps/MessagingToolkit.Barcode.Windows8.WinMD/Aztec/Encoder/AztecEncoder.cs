using System;
using System.Collections.Generic;
using System.Text;
using MessagingToolkit.Barcode.Common;
using MessagingToolkit.Barcode.QRCode.Encoder;
using MessagingToolkit.Barcode.Helper;

namespace MessagingToolkit.Barcode.Aztec.Encoder
{
    internal sealed class AztecEncoder : IEncoder
    {
#if (SILVERLIGHT || NETFX_CORE || PORTABLE)
        private static readonly  Encoding DEFAULT_CHARSET = Encoding.GetEncoding("UTF-8");
#else
        private static readonly Encoding DEFAULT_CHARSET = Encoding.GetEncoding("ISO-8859-1");
#endif

        // Default quiet zone
        private const int QUIET_ZONE_SIZE = 0;

        public BitMatrix Encode(string contents, BarcodeFormat format, int width, int height)
        {
            return Encode(contents, format, width, height, null);
        }

        public BitMatrix Encode(string contents, BarcodeFormat format, int width, int height, IDictionary<EncodeOptions, object> encodingOptions)
        {

            if (string.IsNullOrEmpty(contents))
            {
                throw new ArgumentException("Found empty contents");
            }

            if (format != BarcodeFormat.Aztec)
            {
                throw new ArgumentException("Can only encode Aztec, but got " + format);
            }

            if (width < 0 || height < 0)
            {
                throw new ArgumentException("Requested dimensions are too small: " + width + 'x' + height);
            }


            int quietZone = QUIET_ZONE_SIZE;
            Encoding encoder = DEFAULT_CHARSET;
            int eccPercent = Encoder.DEFAULT_EC_PERCENT;
            if (encodingOptions != null)
            {
                object obj = BarcodeHelper.GetEncodeOptionType(encodingOptions, EncodeOptions.Margin);
                if (obj != null)
                {
                    quietZone = Convert.ToInt32(obj);
                }

                String encoding = (encodingOptions == null) ? null : (String)BarcodeHelper.GetEncodeOptionType(encodingOptions, EncodeOptions.CharacterSet);
                if (!string.IsNullOrEmpty(encoding))
                {
                    encoder = Encoding.GetEncoding(encoding);
                }

                obj = BarcodeHelper.GetEncodeOptionType(encodingOptions, EncodeOptions.ErrorCorrection);
                if (obj != null)
                {
                    eccPercent = Convert.ToInt32(obj);
                }
            }

            AztecCode aztec = Encoder.Encode(encoder.GetBytes(contents), eccPercent);

            // For Aztec barcode, quiet zone is not necessary. Pass in value of 0;
            return RenderResult(aztec.Matrix, width, height, quietZone);
           
        }


        /// <summary>
        /// Renders the result.
        /// Note that the input matrix uses 0 == white, 1 == black, while the output matrix uses
        /// 0 == black, 255 == white (i.e. an 8 bit greyscale bitmap).
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="quietZone">The quiet zone.</param>
        /// <returns></returns>
        /// <exception cref="System.InvalidOperationException"></exception>
        private static BitMatrix RenderResult(BitMatrix input, int width, int height, int quietZone)
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
                    if (input.Get(inputX, inputY))
                    {
                        output.SetRegion(outputX, outputY, multiple, multiple);
                    }
                }
            }
            return output;
        }
    }

}