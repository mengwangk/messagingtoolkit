using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MessagingToolkit.Barcode.Common;
using MessagingToolkit.Barcode.Helper;

namespace MessagingToolkit.Barcode.OneD
{
    /// <summary>
    /// <p>Encapsulates functionality and implementation that is common to one-dimensional barcodes.</p>
    /// 
    /// Modified: April 30 2012
    /// </summary>
    internal abstract class OneDEncoder : IEncoder
    {

        public BitMatrix Encode(String contents, BarcodeFormat format, int width, int height)
        {
            return Encode(contents, format, width, height, null);
        }

        /// <param name="contents">The contents to encode in the barcode</param>
        /// <param name="format">The barcode format to generate</param>
        /// <param name="width">The preferred width in pixels</param>
        /// <param name="height">The preferred height in pixels</param>
        /// <param name="encodingOptions">The encoding options.</param>
        /// <returns>
        /// The generated barcode as a Matrix of unsigned bytes (0 == black, 255 == white)
        /// </returns>
        /// <exception cref="System.ArgumentException">Found empty contents</exception>
        public virtual BitMatrix Encode(String contents, BarcodeFormat format, int width, int height, IDictionary<EncodeOptions, object> encodingOptions)
        {
            if (contents.Length == 0)
            {
                throw new ArgumentException("Found empty contents");
            }

            if (width < 0 || height < 0)
            {
                throw new ArgumentException("Negative size is not allowed. Input: "
                                                       + width + 'x' + height);
            }

            int sidesMargin = DefaultMargin;
            if (encodingOptions != null)
            {
                object obj = BarcodeHelper.GetEncodeOptionType(encodingOptions, EncodeOptions.Margin);
                if (obj != null)
                {
                    sidesMargin = Convert.ToInt32(obj);
                }
            }

            bool[] code = Encode(contents);
            return RenderResult(code, width, height, sidesMargin);

        }

        /// <summary>
        /// <returns>a byte array of horizontal pixels (0 = white, 1 = black)</returns>
        /// </summary>
        private static BitMatrix RenderResult(bool[] code, int width, int height, int sidesMargin)
        {
            int inputWidth = code.Length;
            // Add quiet zone on both sides
            int fullWidth = inputWidth + (UPCEANDecoder.StartEndPattern.Length << 1);
            int outputWidth = Math.Max(width, fullWidth);
            int outputHeight = Math.Max(1, height);

            int multiple = outputWidth / fullWidth;
            int leftPadding = (outputWidth - (inputWidth * multiple)) / 2;

            BitMatrix output = new BitMatrix(outputWidth, outputHeight);
            for (int inputX = 0, outputX = leftPadding; inputX < inputWidth; inputX++, outputX += multiple)
            {
                if (code[inputX])
                {
                    output.SetRegion(outputX, 0, multiple, outputHeight);
                }
            }
            return output;
        }


        /// <summary>
        /// Appends the given pattern to the target array starting at pos.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="pos">The pos.</param>
        /// <param name="pattern">The pattern.</param>
        /// <param name="startColor">starting color - false for white, true for black</param>
        /// <returns>the number of elements added to target.</returns>
        internal static int AppendPattern(bool[] target, int pos, int[] pattern, bool startColor)
        {
            bool color = startColor;
            int numAdded = 0;
            foreach (int len in pattern)
            {
                for (int j = 0; j < len; j++)
                {
                    target[pos++] = color;
                }
                numAdded += len;
                color = !color; // flip color after each segment
            }
            return numAdded;
        }


        public virtual int DefaultMargin
        {
            get
            {
                // CodaBar spec requires a side margin to be more than ten times wider than narrow space.
                // This seems like a decent idea for a default for all formats.
                return 10;
            }
        }

        /// <summary>
        /// Encode the contents to boolean array expression of one-dimensional barcode.
        /// Start code and end code should be included in result, and side margins should not be included.
        /// </summary>
        /// <param name="contents">The contents.</param>
        /// <returns>a boolean of horizontal pixels (false = white, true = black)</returns>
        public abstract bool[] Encode(String contents);

    }
}
