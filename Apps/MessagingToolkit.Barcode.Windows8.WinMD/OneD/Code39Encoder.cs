using MessagingToolkit.Barcode;
using MessagingToolkit.Barcode.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;


namespace MessagingToolkit.Barcode.OneD
{
    /// <summary>
    /// This object renders a CODE39 code.
    /// 
    /// Modified: April 30 2012
    /// </summary>
    internal sealed class Code39Encoder : OneDEncoder
    {
        public override BitMatrix Encode(String contents, BarcodeFormat format, int width, int height, IDictionary<EncodeOptions, object> encodingOptions)
        {
			if (format != BarcodeFormat.Code39) {
				throw new ArgumentException("Can only encode CODE_39, but got " + format);
			}
			return base.Encode(contents,format,width,height,encodingOptions);
		}
	
		public override bool[] Encode(String contents) {
			int length = contents.Length;
			if (length > 80) {
				throw new ArgumentException("Requested contents should be less than 80 digits long, but got " + length);
			}
	
			int[] widths = new int[9];
			int codeWidth = 24 + 1 + length;
			for (int i = 0; i < length; i++) {
				int indexInString = Code39Decoder.AlphabetString.IndexOf(contents[i]);
                ToIntArray(Code39Decoder.CharacterEncodings[indexInString], widths);
				/* foreach */
				foreach (int width  in  widths) {
					codeWidth += width;
				}
			}
			bool[] result = new bool[codeWidth];
            ToIntArray(Code39Decoder.CharacterEncodings[39], widths);
            int pos = MessagingToolkit.Barcode.OneD.OneDEncoder.AppendPattern(result, 0, widths, true);
			int[] narrowWhite = { 1 };
			pos += MessagingToolkit.Barcode.OneD.OneDEncoder.AppendPattern(result, pos, narrowWhite, false);
			//append next character to bytematrix
			for (int i_0 = length - 1; i_0 >= 0; i_0--) {
				int indexInString_1 = Code39Decoder.AlphabetString.IndexOf(contents[i_0]);
				ToIntArray(Code39Decoder.CharacterEncodings[indexInString_1], widths);
				pos += MessagingToolkit.Barcode.OneD.OneDEncoder.AppendPattern(result, pos, widths, true);
				pos += MessagingToolkit.Barcode.OneD.OneDEncoder.AppendPattern(result, pos, narrowWhite, false);
			}
            ToIntArray(Code39Decoder.CharacterEncodings[39], widths);
            pos += MessagingToolkit.Barcode.OneD.OneDEncoder.AppendPattern(result, pos, widths, true);
			return result;
		}
	
		private static void ToIntArray(int a, int[] toReturn) {
			for (int i = 0; i < 9; i++) {
				int temp = a & (1 << i);
				toReturn[i] = (temp == 0) ? 1 : 2;
			}
		}

    }
}
