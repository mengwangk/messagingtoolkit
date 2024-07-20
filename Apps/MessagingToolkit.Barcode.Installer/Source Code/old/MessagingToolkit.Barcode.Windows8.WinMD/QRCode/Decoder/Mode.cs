using System;

namespace MessagingToolkit.Barcode.QRCode.Decoder
{
    /// <summary>
    /// 	<p>See ISO 18004:2006, 6.4.1, Tables 2 and 3. This enum encapsulates the various modes in which
    /// data can be encoded to bits in the QR code standard.</p>
    ///
    /// Modified: April 27 2012
    /// </summary>
    internal sealed class Mode
    {
        public static readonly Mode Terminator = new Mode(new int[] { 0, 0, 0 }, 0x00, "TERMINATOR"); // Not really a mode...
        public static readonly Mode Numeric = new Mode(new int[] { 10, 12, 14 }, 0x01, "Numeric");
        public static readonly Mode Alphanumeric = new Mode(new int[] { 9, 11, 13 }, 0x02, "ALPHANUMERIC");
        public static readonly Mode StructuredAppend = new Mode(new int[] { 0, 0, 0 }, 0x03, "STRUCTURED_APPEND"); // Not supported
        public static readonly Mode Byte = new Mode(new int[] { 8, 16, 16 }, 0x04, "Byte");
        public static readonly Mode Eci = new Mode(null, 0x07, "ECI"); // character counts don't apply
        public static readonly Mode Kanji = new Mode(new int[] { 8, 10, 12 }, 0x08, "KANJI");
        public static readonly Mode Fnc1FirstPosition = new Mode(null, 0x05, "FNC1_FIRST_POSITION");
        public static readonly Mode Fnc1SecondPosition = new Mode(null, 0x09, "FNC1_SECOND_POSITION");

        /** See GBT 18284-2000; "Hanzi" is a transliteration of this mode name. */
        public static readonly Mode Hanzi = new Mode(new int[] { 8, 10, 12 }, 0x0D, "HANZI");

        private int[] characterCountBitsForVersions;
        private int bits;
        private string name;

        public int Bits
        {
            get
            {
                return bits;
            }

        }
        public string Name
        {
            get
            {
                return name;
            }

        }

        private Mode(int[] characterCountBitsForVersions, int bits, string name)
        {
            this.characterCountBitsForVersions = characterCountBitsForVersions;
            this.bits = bits;
            this.name = name;
        }

        /// <summary>
        /// Fors the bits.
        /// </summary>
        /// <param name="bits">four bits encoding a QR Code data mode</param>
        /// <returns>Mode encoded by these bits</returns>
        /// <throws>  IllegalArgumentException if bits do not correspond to a known mode </throws>
        public static Mode ForBits(int bits)
        {
            switch (bits)
            {

                case 0x0:
                    return Terminator;

                case 0x1:
                    return Numeric;

                case 0x2:
                    return Alphanumeric;

                case 0x3:
                    return StructuredAppend;

                case 0x4:
                    return Byte;

                case 0x5:
                    return Fnc1FirstPosition;

                case 0x7:
                    return Eci;

                case 0x8:
                    return Kanji;

                case 0x9:
                    return Fnc1SecondPosition;

                case 0xD:
                    // 0xD is defined in GBT 18284-2000, may not be supported in foreign country
                    return Hanzi;

                default:
                    throw new System.ArgumentException();

            }
        }

        /// <summary>
        /// Gets the character count bits.
        /// </summary>
        /// <param name="version">version in question</param>
        /// <returns>
        /// number of bits used, in this QR Code symbol Version, to encode the
        /// count of characters that will follow encoded in this Mode
        /// </returns>
        public int GetCharacterCountBits(Version version)
        {
            if (characterCountBitsForVersions == null)
            {
                throw new System.ArgumentException("Character count doesn't apply to this mode");
            }
            int number = version.VersionNumber;
            int offset;
            if (number <= 9)
            {
                offset = 0;
            }
            else if (number <= 26)
            {
                offset = 1;
            }
            else
            {
                offset = 2;
            }
            return characterCountBitsForVersions[offset];
        }

        public override string ToString()
        {
            return name;
        }
    }
}
