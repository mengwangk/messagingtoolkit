using System;
using System.Text;
using ErrorCorrectionLevel = MessagingToolkit.Barcode.QRCode.Decoder.ErrorCorrectionLevel;
using Mode = MessagingToolkit.Barcode.QRCode.Decoder.Mode;

namespace MessagingToolkit.Barcode.QRCode.Encoder
{
    public sealed class QRCode
    {

        public const int NUM_MASK_PATTERNS = 8;

        private Mode mode;
        private ErrorCorrectionLevel ecLevel;
        private MessagingToolkit.Barcode.QRCode.Decoder.Version version;
        private int maskPattern;
        private ByteMatrix matrix;

        public QRCode()
        {
            maskPattern = -1;
        }

        public Mode Mode
        {
            // Mode of the QR Code.

            get
            {
                return mode;
            }

            set
            {
                mode = value;
            }
        }

        public ErrorCorrectionLevel ECLevel
        {
            // Error correction level of the QR Code.

            get
            {
                return ecLevel;
            }

            set
            {
                ecLevel = value;
            }

        }

        public MessagingToolkit.Barcode.QRCode.Decoder.Version Version
        {
            // Version of the QR Code.  The bigger size, the bigger version.

            get
            {
                return version;
            }

            set
            {
                version = value;
            }

        }
        
        public int MaskPattern
        {
            // Mask pattern of the QR Code.

            get
            {
                return maskPattern;
            }

            set
            {
                maskPattern = value;
            }

        }

      
        public ByteMatrix Matrix
        {
            // ByteMatrix data of the QR Code.

            get
            {
                return matrix;
            }

            // This takes ownership of the 2D array.

            set
            {
                matrix = value;
            }

        }

       

        // Return debug String.
        public override String ToString()
        {
            StringBuilder result = new StringBuilder(200);
            result.Append("<<\n");
            result.Append(" mode: ");
            result.Append(mode);
            result.Append("\n ecLevel: ");
            result.Append(ecLevel);
            result.Append("\n version: ");
            result.Append(version);
            result.Append("\n maskPattern: ");
            result.Append(maskPattern);
            result.Append("\n numDataBytes: ");
            if (matrix == null)
            {
                result.Append("\n matrix: null\n");
            }
            else
            {
                result.Append("\n matrix:\n");
                result.Append(matrix.ToString());
            }
            result.Append(">>\n");
            return result.ToString();
        }

        // Check if "mask_pattern" is valid.
        public static bool IsValidMaskPattern(int maskPattern)
        {
            return maskPattern >= 0 && maskPattern < NUM_MASK_PATTERNS;
        }

    }
}
