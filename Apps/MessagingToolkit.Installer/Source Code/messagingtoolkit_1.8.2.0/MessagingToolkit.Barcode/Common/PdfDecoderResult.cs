using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MessagingToolkit.Barcode.Common
{
    public class PdfDecoderResult : DecoderResult
    {
        private readonly int errorCorrectionCount;
        private readonly MacroPdf417Block macroPdf417Block;


        public PdfDecoderResult(byte[] rawBytes, String text, List<byte[]> byteSegments, String ecLevel, int errorCorrectionCount, MacroPdf417Block macroPdf417Block)
            : base(rawBytes, text, byteSegments, ecLevel)
        {
            this.errorCorrectionCount = errorCorrectionCount;
            this.macroPdf417Block = macroPdf417Block;
        }

        public MacroPdf417Block MacroPdf417Block
        {
            get
            {
                return this.macroPdf417Block;
            }
        }

        public int ErrorCorrectionCount
        {
            get
            {
                return this.errorCorrectionCount;
            }
        }
    }
}
