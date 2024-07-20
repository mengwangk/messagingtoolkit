using System;
using System.Collections.Generic;
using BarcodeFormat = MessagingToolkit.Barcode.BarcodeFormat;
using WriterException = MessagingToolkit.Barcode.BarcodeEncoderException;
using BitMatrix = MessagingToolkit.Barcode.Common.BitMatrix;

namespace MessagingToolkit.Barcode.OneD
{

    /// <summary>
    /// This object renders an EAN8 code as a BitMatrix 2D array of greyscale
    /// values.
    /// 
    /// Modified: April 30 2012
    /// </summary>
    public sealed class EAN8Encoder : UPCEANEncoder
    {
       private const int CODE_WIDTH = 3 + // start guard
				(7 * 4) + // left bars
				5 + // middle guard
				(7 * 4) + // right bars
				3; // end guard
	
		public override BitMatrix Encode(String contents, BarcodeFormat format, int width, int height, Dictionary<EncodeOptions, object> encodingOptions) {
			if (format != BarcodeFormat.EAN8) {
				throw new ArgumentException("Can only encode EAN_8, but got " + format);
			}
	
			return base.Encode(contents,format,width,height,encodingOptions);
		}



        /// <summary>
        /// Encode the contents to byte array expression of one-dimensional barcode.
        /// Start code and end code should be included in result, and side margins should not be included.
        /// </summary>
        /// <param name="contents">The contents.</param>
        /// <returns>
        /// a boolean of horizontal pixels (false = white, true = black)
        /// </returns>
		public override bool[] Encode(String contents) {
			if (contents.Length != 8) {
				throw new ArgumentException("Requested contents should be 8 digits long, but got " + contents.Length);
			}
	
			bool[] result = new bool[CODE_WIDTH];
			int pos = 0;
	
			pos += AppendPattern(result, pos, UPCEANDecoder.StartEndPattern, true);
	
			for (int i = 0; i <= 3; i++) {
				int digit = Int32.Parse(contents.Substring(i,(i + 1)-(i)));
                pos += AppendPattern(result, pos, UPCEANDecoder.LPatterns[digit], false);
			}

            pos += AppendPattern(result, pos, UPCEANDecoder.MiddlePattern, false);
	
			for (int i_0 = 4; i_0 <= 7; i_0++) {
				int digit_1 = Int32.Parse(contents.Substring(i_0,(i_0 + 1)-(i_0)));
                pos += AppendPattern(result, pos, UPCEANDecoder.LPatterns[digit_1], true);
			}
            pos += AppendPattern(result, pos, UPCEANDecoder.StartEndPattern, true);
	
			return result;
		}
	
    }
}