using MessagingToolkit.Barcode.Common;

namespace MessagingToolkit.Barcode.QRCode.Decoder
{
    /// <summary>
    /// BitMatrix parser
    /// 
    /// Modified: April 21 2012
    /// </summary>
    internal sealed class BitMatrixParser
    {

        private readonly BitMatrix bitMatrix;
        private Version parsedVersion;
        private FormatInformation parsedFormatInfo;


        /// <summary>
        /// Initializes a new instance of the <see cref="BitMatrixParser"/> class.
        /// </summary>
        /// <param name="bitMatrix">to parse</param>
        /// <exception cref="FormatException">if dimension is not &gt;= 21 and 1 mod 4</exception>
        internal BitMatrixParser(BitMatrix bitMatrix)
        {
            int dimension = bitMatrix.Height;
            if (dimension < 21 || (dimension & 0x03) != 1)
            {
                throw FormatException.Instance;
            }
            this.bitMatrix = bitMatrix;
        }

        /// <summary>
        ///   <p>Reads format information from one of its two locations within the QR Code.</p>
        /// </summary>
        /// <returns>
        /// encapsulating the QR Code's format info
        /// </returns>
        /// <exception cref="FormatException">if both format information locations cannot be parsed asthe valid encoding of format information</exception>
        internal FormatInformation ReadFormatInformation()
        {

            if (parsedFormatInfo != null)
            {
                return parsedFormatInfo;
            }

            // Read top-left format info bits
            int formatInfoBits1 = 0;
            for (int i = 0; i < 6; i++)
            {
                formatInfoBits1 = CopyBit(i, 8, formatInfoBits1);
            }
            // .. and skip a bit in the timing pattern ...
            formatInfoBits1 = CopyBit(7, 8, formatInfoBits1);
            formatInfoBits1 = CopyBit(8, 8, formatInfoBits1);
            formatInfoBits1 = CopyBit(8, 7, formatInfoBits1);
            // .. and skip a bit in the timing pattern ...
            for (int j = 5; j >= 0; j--)
            {
                formatInfoBits1 = CopyBit(8, j, formatInfoBits1);
            }

            // Read the top-right/bottom-left pattern too
            int dimension = bitMatrix.Height;
            int formatInfoBits2 = 0;
            int jMin = dimension - 7;
            for (int j_0 = dimension - 1; j_0 >= jMin; j_0--)
            {
                formatInfoBits2 = CopyBit(8, j_0, formatInfoBits2);
            }
            for (int i_1 = dimension - 8; i_1 < dimension; i_1++)
            {
                formatInfoBits2 = CopyBit(i_1, 8, formatInfoBits2);
            }

            parsedFormatInfo = MessagingToolkit.Barcode.QRCode.Decoder.FormatInformation.DecodeFormatInformation(formatInfoBits1, formatInfoBits2);
            if (parsedFormatInfo != null)
            {
                return parsedFormatInfo;
            }
            throw FormatException.Instance;
        }

        /// <summary>
        /// <p>Reads version information from one of its two locations within the QR Code.</p>
        /// </summary>
        ///
        /// <returns>/// <see cref="T:Com.Google.Zxing.Qrcode.Decoder.Version"/>
        ///  encapsulating the QR Code's version</returns>
        /// <exception cref="FormatException">if both version information locations cannot be parsed asthe valid encoding of version information</exception>
        internal Version ReadVersion()
        {

            if (parsedVersion != null)
            {
                return parsedVersion;
            }

            int dimension = bitMatrix.Height;

            int provisionalVersion = (dimension - 17) >> 2;
            if (provisionalVersion <= 6)
            {
                return MessagingToolkit.Barcode.QRCode.Decoder.Version.GetVersionForNumber(provisionalVersion);
            }

            // Read top-right version info: 3 wide by 6 tall
            int versionBits = 0;
            int ijMin = dimension - 11;
            for (int j = 5; j >= 0; j--)
            {
                for (int i = dimension - 9; i >= ijMin; i--)
                {
                    versionBits = CopyBit(i, j, versionBits);
                }
            }

            Version theParsedVersion = MessagingToolkit.Barcode.QRCode.Decoder.Version.DecodeVersionInformation(versionBits);
            if (theParsedVersion != null && theParsedVersion.DimensionForVersion == dimension)
            {
                parsedVersion = theParsedVersion;
                return theParsedVersion;
            }

            // Hmm, failed. Try bottom left: 6 wide by 3 tall
            versionBits = 0;
            for (int i_0 = 5; i_0 >= 0; i_0--)
            {
                for (int j_1 = dimension - 9; j_1 >= ijMin; j_1--)
                {
                    versionBits = CopyBit(i_0, j_1, versionBits);
                }
            }

            theParsedVersion = MessagingToolkit.Barcode.QRCode.Decoder.Version.DecodeVersionInformation(versionBits);
            if (theParsedVersion != null && theParsedVersion.DimensionForVersion == dimension)
            {
                parsedVersion = theParsedVersion;
                return theParsedVersion;
            }
            throw FormatException.Instance;
        }

        private int CopyBit(int i, int j, int versionBits)
        {
            return (bitMatrix.Get(i, j)) ? (versionBits << 1) | 0x1 : versionBits << 1;
        }

        /// <summary>
        /// <p>Reads the bits in the <see cref="null"/> representing the finder pattern in the
        /// correct order in order to reconstitute the codewords bytes contained within the
        /// QR Code.</p>
        /// </summary>
        ///
        /// <returns>bytes encoded within the QR Code</returns>
        /// <exception cref="FormatException">if the exact number of bytes expected is not read</exception>
        internal byte[] ReadCodewords()
        {

            FormatInformation formatInfo = ReadFormatInformation();
            Version version = ReadVersion();

            // Get the data mask for the format used in this QR Code. This will exclude
            // some bits from reading as we wind through the bit matrix.
            DataMask dataMask = MessagingToolkit.Barcode.QRCode.Decoder.DataMask.ForReference((int)formatInfo.GetDataMask());
            int dimension = bitMatrix.Height;
            dataMask.UnmaskBitMatrix(bitMatrix, dimension);

            BitMatrix functionPattern = version.BuildFunctionPattern();

            bool readingUp = true;
            byte[] result = new byte[version.TotalCodewords];
            int resultOffset = 0;
            int currentByte = 0;
            int bitsRead = 0;
            // Read columns in pairs, from right to left
            for (int j = dimension - 1; j > 0; j -= 2)
            {
                if (j == 6)
                {
                    // Skip whole column with vertical alignment pattern;
                    // saves time and makes the other code proceed more cleanly
                    j--;
                }
                // Read alternatingly from bottom to top then top to bottom
                for (int count = 0; count < dimension; count++)
                {
                    int i = (readingUp) ? (dimension - 1 - count) : count;
                    for (int col = 0; col < 2; col++)
                    {
                        // Ignore bits covered by the function pattern
                        if (!functionPattern.Get(j - col, i))
                        {
                            // Read a bit
                            bitsRead++;
                            currentByte <<= 1;
                            if (bitMatrix.Get(j - col, i))
                            {
                                currentByte |= 1;
                            }
                            // If we've made a whole byte, save it off
                            if (bitsRead == 8)
                            {
                                result[resultOffset++] = (byte)currentByte;
                                bitsRead = 0;
                                currentByte = 0;
                            }
                        }
                    }
                }
                readingUp ^= true; // readingUp = !readingUp; // switch directions
            }
            if (resultOffset != version.TotalCodewords)
            {
                throw FormatException.Instance;
            }
            return result;
        }
    }
}
