using System;
using System.Collections.Generic;
using ErrorCorrectionLevel = MessagingToolkit.Barcode.QRCode.Decoder.ErrorCorrectionLevel;

namespace MessagingToolkit.Barcode.Common
{

    /// <summary> 
    /// Encapsulates the result of decoding a matrix of bits. This typically
    /// applies to 2D barcode formats. For now it contains the raw bytes obtained,
    /// as well as a String interpretation of those bytes, if applicable.
    /// 
    /// Modified: April 21 2012
    /// </summary>
    internal class DecoderResult
    {

        private readonly byte[] rawBytes;
        private readonly String text;
        private readonly List<byte[]> byteSegments;
        private readonly String ecLevel;
     

        public DecoderResult(byte[] rawBytes, String text, List<byte[]> byteSegments, String ecLevel)
        {
            this.rawBytes = rawBytes;
            this.text = text;
            this.byteSegments = byteSegments;
            this.ecLevel = ecLevel;
        }

        public byte[] RawBytes
        {
            get
            {
                return rawBytes;
            }
        }

        public String Text
        {
            get
            {
                return text;
            }
        }

        public List<byte[]> ByteSegments
        {
            get
            {
                return byteSegments;
            }
        }

        public String ECLevel
        {
            get
            {
                return ecLevel;
            }
        }
    }
}
