using System;

namespace MessagingToolkit.Barcode.QRCode.Decoder
{
    /// <summary>
    /// <p>See ISO 18004:2006, 6.5.1. This enum encapsulates the four error correction levels
    /// defined by the QR code standard.</p>
    /// </summary>
    public sealed class ErrorCorrectionLevel
    {
        /// <summary>
        /// L = ~7% correction
        /// </summary>
        public static readonly ErrorCorrectionLevel L = new ErrorCorrectionLevel(0, 0x01, "L");

        /// <summary>
        /// M = ~15% correction
        /// </summary>
        public static readonly ErrorCorrectionLevel M = new ErrorCorrectionLevel(1, 0x00, "M");

        /// <summary>
        /// Q = ~25% correction
        /// </summary>
        public static readonly ErrorCorrectionLevel Q = new ErrorCorrectionLevel(2, 0x03, "Q");

        /// <summary>
        /// H = ~30% correction
        /// </summary>
        public static readonly ErrorCorrectionLevel H = new ErrorCorrectionLevel(3, 0x02, "h");

        private static readonly ErrorCorrectionLevel[] FOR_BITS = { M, L, H, Q };

        private readonly int ordinal;
        private readonly int bits;
        private readonly String name;

        private ErrorCorrectionLevel(int ordinal, int bits, String name)
        {
            this.ordinal = ordinal;
            this.bits = bits;
            this.name = name;
        }

        public int Ordinal
        {
            get
            {
                return ordinal;
            }
        }

        public int Bits
        {
            get
            {
                return bits;
            }
        }

        public String Name
        {
            get
            {
                return name;
            }
        }

        public override String ToString()
        {
            return name;
        }


        /// <param name="bits">int containing the two bits encoding a QR Code's error correction level</param>
        /// <returns>ErrorCorrectionLevel representing the encoded error correction level</returns>
        public static ErrorCorrectionLevel ForBits(int bits)
        {
            if (bits < 0 || bits >= FOR_BITS.Length)
            {
                throw new ArgumentException();
            }
            return FOR_BITS[bits];
        }

    }
}
