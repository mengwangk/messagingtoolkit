using MessagingToolkit.Barcode.Common;
using System;
using System.Collections.Generic;

namespace MessagingToolkit.Barcode.OneD
{
    /// <summary>
    /// This object renders a UPC-A code.
    /// 
    /// Modified: April 30 2012
    /// </summary>
    public class UPCAEncoder : IEncoder
    {
        private readonly EAN13Encoder subEncoder;

        public UPCAEncoder()
        {
            this.subEncoder = new EAN13Encoder();
        }

      public virtual BitMatrix Encode(String contents, BarcodeFormat format, int width, int height) {
			return Encode(contents, format, width, height, null);
		}
	
		public virtual BitMatrix Encode(String contents, BarcodeFormat format, int width, int height, Dictionary<EncodeOptions, object> encodingOptions) {
			if (format != BarcodeFormat.UPCA) {
				throw new ArgumentException("Can only encode UPC-A, but got " + format);
			}
            return subEncoder.Encode(Preencode(contents), BarcodeFormat.EAN13, width, height, encodingOptions);
		}
	
		/// <summary>
		/// Transform a UPC-A code into the equivalent EAN-13 code, and add a check digit if it is not
		/// already present.
		/// </summary>
		///
		private static String Preencode(String contents) {
			int length = contents.Length;
			if (length == 11) {
				// No check digit present, calculate it and add it
				int sum = 0;
				for (int i = 0; i < 11; ++i) {
					sum += (contents[i] - '0') * ((i % 2 == 0) ? 3 : 1);
				}
				contents += (1000 - sum) % 10;
			} else if (length != 12) {
				throw new ArgumentException("Requested contents should be 11 or 12 digits long, but got " + contents.Length);
			}
			return '0' + contents;
		}
    }
}
