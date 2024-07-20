using MessagingToolkit.Barcode.Common;
using MessagingToolkit.Barcode.Common.ReedSolomon;
using MessagingToolkit.Barcode.QRCode.Decoder;
using MessagingToolkit.Barcode.Helper;

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MessagingToolkit.Barcode.QRCode.Encoder
{

    /// <summary>
    /// Modified: April 28 2012
    /// </summary>
    public sealed class Encoder
    {

        // The original table is defined in the table 5 of JISX0510:2004 (p.19).
        private static readonly int[] ALPHANUMERIC_TABLE = { 
                -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, // 0x00-0x0f
				-1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, // 0x10-0x1f
				36, -1, -1, -1, 37, 38, -1, -1, -1, -1, 39, 40, -1, 41, 42, 43, // 0x20-0x2f
				0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 44, -1, -1, -1, -1, -1, // 0x30-0x3f
				-1, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, // 0x40-0x4f
				25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35, -1, -1, -1, -1, -1, // 0x50-0x5f
		};

#if SILVERLIGHT

    #if WINDOWS_PHONE
            internal const String DefaultByteModeEncoding = "ISO-8859-1";
    #else
        internal const String DefaultByteModeEncoding = "UTF-8";
    #endif

#else
         internal const String DefaultByteModeEncoding = "ISO-8859-1";
#endif

        private Encoder()
        {
        }

        // The mask penalty calculation is complicated.  See Table 21 of JISX0510:2004 (p.45) for details.
        // Basically it applies four rules and summate all penalties.
        private static int CalculateMaskPenalty(ByteMatrix matrix)
        {
            return MaskUtil.ApplyMaskPenaltyRule1(matrix)
                    + MaskUtil.ApplyMaskPenaltyRule2(matrix)
                    + MaskUtil.ApplyMaskPenaltyRule3(matrix)
                    + MaskUtil.ApplyMaskPenaltyRule4(matrix);
        }

        /// <summary>
        /// Encode "bytes" with the error correction level "ecLevel". The encoding mode will be chosen
        /// internally by chooseMode(). On success, store the result in "qrCode".
        /// We recommend you to use QRCode.EC_LEVEL_L (the lowest level) for
        /// "getECLevel" since our primary use is to show QR code on desktop screens. We don't need very
        /// strong error correction for this purpose.
        /// Note that there is no way to encode bytes in MODE_KANJI. We might want to add EncodeWithMode()
        /// with which clients can specify the encoding mode. For now, we don't need the functionality.
        /// </summary>
        ///
        public static QRCode Encode(String content, ErrorCorrectionLevel ecLevel)
        {
            return Encode(content, ecLevel, null);
        }

        public static QRCode Encode(String content, ErrorCorrectionLevel ecLevel, Dictionary<EncodeOptions, object> encodingOptions)
        {
            // Determine what character encoding has been specified by the caller, if any
            String encoding = (encodingOptions == null) ? null : (String)BarcodeHelper.GetEncodeOptionType(encodingOptions, EncodeOptions.CharacterSet);
            if (encoding == null)
            {
                encoding = DefaultByteModeEncoding;
            }

            // Pick an encoding mode appropriate for the content. Note that this will not attempt to use
            // multiple modes / segments even if that were more efficient. Twould be nice.
            Mode mode = ChooseMode(content, encoding);

            // This will store the header information, like mode and
            // length, as well as "header" segments like an ECI segment.
            BitArray headerBits = new BitArray();

            // Append ECI segment if applicable
            if (mode == Mode.Byte && !DefaultByteModeEncoding.Equals(encoding))
            {
                CharacterSetECI eci = CharacterSetECI.GetCharacterSetECIByName(encoding);
                if (eci != null)
                {
                    AppendECI(eci, headerBits);
                }
            }

            // (With ECI in place,) Write the mode marker
            AppendModeInfo(mode, headerBits);

            // Collect data within the main segment, separately, to count its size if needed. Don't add it to
            // main payload yet.
            BitArray dataBits = new BitArray();
            AppendBytes(content, mode, dataBits, encoding);

            // Hard part: need to know version to know how many bits length takes. But need to know how many
            
           // bits it takes to know version. First we take a guess at version by assuming version will be
    	   // the minimum, 1:
    	    int provisionalBitsNeeded = headerBits.GetSize()
                    + mode.GetCharacterCountBits(MessagingToolkit.Barcode.QRCode.Decoder.Version.GetVersionForNumber(1))
	                + dataBits.GetSize();
            MessagingToolkit.Barcode.QRCode.Decoder.Version provisionalVersion = ChooseVersion(provisionalBitsNeeded, ecLevel);
	
	        // Use that guess to calculate the right version. I am still not sure this works in 100% of cases.
            int bitsNeeded = headerBits.GetSize()
                + mode.GetCharacterCountBits(provisionalVersion)
                + dataBits.GetSize();
            MessagingToolkit.Barcode.QRCode.Decoder.Version version = ChooseVersion(bitsNeeded, ecLevel);

            BitArray headerAndDataBits = new BitArray();
            headerAndDataBits.AppendBitArray(headerBits);
            // Find "length" of main segment and write it
            int numLetters = mode == Mode.Byte ? dataBits.GetSizeInBytes() : content.Length;
            AppendLengthInfo(numLetters, version, mode, headerAndDataBits);
            // Put data together into the overall payload
            headerAndDataBits.AppendBitArray(dataBits);

            MessagingToolkit.Barcode.QRCode.Decoder.Version.ECBlocks ecBlocks = version.GetECBlocksForLevel(ecLevel);
            int numDataBytes = version.TotalCodewords - ecBlocks.TotalECCodewords;

            // Terminate the bits properly.
            TerminateBits(numDataBytes, headerAndDataBits);

            // Interleave data bits with error correction code.
            BitArray finalBits = InterleaveWithECBytes(headerAndDataBits,
                                                       version.TotalCodewords,
                                                       numDataBytes,
                                                       ecBlocks.NumBlocks);

            QRCode qrCode = new QRCode();

            qrCode.ECLevel = ecLevel;
            qrCode.Mode = mode;
            qrCode.Version = version;

            //  Choose the mask pattern and set to "qrCode".
            int dimension = version.DimensionForVersion;
            ByteMatrix matrix = new ByteMatrix(dimension, dimension);
            int maskPattern = ChooseMaskPattern(finalBits, ecLevel, version, matrix);
            qrCode.MaskPattern = maskPattern;

            // Build the matrix and set it to "qrCode".
            MatrixUtil.BuildMatrix(finalBits, ecLevel, version, maskPattern, matrix);
            qrCode.Matrix = matrix;

            return qrCode;
        }


        /// <returns>the code point of the table used in alphanumeric mode or
        /// -1 if there is no corresponding code in the table.</returns>
        static internal int GetAlphanumericCode(int code)
        {
            if (code < ALPHANUMERIC_TABLE.Length)
            {
                return ALPHANUMERIC_TABLE[code];
            }
            return -1;
        }

        public static Mode ChooseMode(String content)
        {
            return ChooseMode(content, null);
        }

        /// <summary>
        /// Choose the best mode by examining the content. Note that 'encoding' is used as a hint;
        /// if it is Shift_JIS, and the input is only double-byte Kanji, then we return <see cref="null"/>.
        /// </summary>
        ///
        private static Mode ChooseMode(String content, String encoding)
        {
            if ("Shift-JIS".Equals(encoding))
            {
                // Choose Kanji mode if all input are double-byte characters
                return (IsOnlyDoubleByteKanji(content)) ? MessagingToolkit.Barcode.QRCode.Decoder.Mode.Kanji : MessagingToolkit.Barcode.QRCode.Decoder.Mode.Byte;
            }
            bool hasNumeric = false;
            bool hasAlphanumeric = false;
            for (int i = 0; i < content.Length; ++i)
            {
                char c = content[i];
                if (c >= '0' && c <= '9')
                {
                    hasNumeric = true;
                }
                else if (GetAlphanumericCode(c) != -1)
                {
                    hasAlphanumeric = true;
                }
                else
                {
                    return MessagingToolkit.Barcode.QRCode.Decoder.Mode.Byte;
                }
            }
            if (hasAlphanumeric)
            {
                return MessagingToolkit.Barcode.QRCode.Decoder.Mode.Alphanumeric;
            }
            if (hasNumeric)
            {
                return MessagingToolkit.Barcode.QRCode.Decoder.Mode.Numeric;
            }
            return MessagingToolkit.Barcode.QRCode.Decoder.Mode.Byte;
        }

        private static bool IsOnlyDoubleByteKanji(String content)
        {
            byte[] bytes;
            try
            {
                bytes = Encoding.GetEncoding("SHIFT-JIS").GetBytes(content);
            }
            catch (IOException uee)
            {
                return false;
            }
            int length = bytes.Length;
            if (length % 2 != 0)
            {
                return false;
            }
            for (int i = 0; i < length; i += 2)
            {
                int byte1 = bytes[i] & 0xFF;
                if ((byte1 < 0x81 || byte1 > 0x9F) && (byte1 < 0xE0 || byte1 > 0xEB))
                {
                    return false;
                }
            }
            return true;
        }

        private static int ChooseMaskPattern(BitArray bits, ErrorCorrectionLevel ecLevel, MessagingToolkit.Barcode.QRCode.Decoder.Version version, ByteMatrix matrix)
        {

            int minPenalty = Int32.MaxValue; // Lower penalty is better.
            int bestMaskPattern = -1;
            // We try all mask patterns to choose the best one.
            for (int maskPattern = 0; maskPattern < MessagingToolkit.Barcode.QRCode.Encoder.QRCode.NUM_MASK_PATTERNS; maskPattern++)
            {
                MatrixUtil.BuildMatrix(bits, ecLevel, version, maskPattern, matrix);
                int penalty = CalculateMaskPenalty(matrix);
                if (penalty < minPenalty)
                {
                    minPenalty = penalty;
                    bestMaskPattern = maskPattern;
                }
            }
            return bestMaskPattern;
        }

        private static MessagingToolkit.Barcode.QRCode.Decoder.Version ChooseVersion(int numInputBits, ErrorCorrectionLevel ecLevel)
        {

            // In the following comments, we use numbers of Version 7-H.
            for (int versionNum = 1; versionNum <= 40; versionNum++)
            {
                MessagingToolkit.Barcode.QRCode.Decoder.Version version = MessagingToolkit.Barcode.QRCode.Decoder.Version.GetVersionForNumber(versionNum);
                // numBytes = 196
                int numBytes = version.TotalCodewords;
                // getNumECBytes = 130
                MessagingToolkit.Barcode.QRCode.Decoder.Version.ECBlocks ecBlocks = version.GetECBlocksForLevel(ecLevel);
                int numEcBytes = ecBlocks.TotalECCodewords;
                // getNumDataBytes = 196 - 130 = 66
                int numDataBytes = numBytes - numEcBytes;
                int totalInputBytes = (numInputBits + 7) / 8;
                if (numDataBytes >= totalInputBytes)
                {
                    return version;
                }
            }
            throw new BarcodeEncoderException(
                   "Data too big");
        }

       
        /// <summary>
        /// Terminate bits as described in 8.4.8 and 8.4.9 of JISX0510:2004 (p.24).
        /// </summary>
        ///
        static internal void TerminateBits(int numDataBytes, BitArray bits)
        {
            int capacity = numDataBytes << 3;
            if (bits.GetSize() > capacity)
            {
                throw new BarcodeEncoderException("data bits cannot fit in the QR Code"
                       + bits.GetSize() + " > " + capacity);
            }
            for (int i = 0; i < 4 && bits.GetSize() < capacity; ++i)
            {
                bits.AppendBit(false);
            }
            // Append termination bits. See 8.4.8 of JISX0510:2004 (p.24) for details.
            // If the last byte isn't 8-bit aligned, we'll add padding bits.
            int numBitsInLastByte = bits.GetSize() & 0x07;
            if (numBitsInLastByte > 0)
            {
                for (int i_0 = numBitsInLastByte; i_0 < 8; i_0++)
                {
                    bits.AppendBit(false);
                }
            }
            // If we have more space, we'll fill the space with padding patterns defined in 8.4.9 (p.24).
            int numPaddingBytes = numDataBytes - bits.GetSizeInBytes();
            for (int i_1 = 0; i_1 < numPaddingBytes; ++i_1)
            {
                bits.AppendBits(((i_1 & 0x01) == 0) ? 0xEC : 0x11, 8);
            }
            if (bits.GetSize() != capacity)
            {
                throw new BarcodeEncoderException("Bits size does not equal capacity");
            }
        }

        /// <summary>
        /// Get number of data bytes and number of error correction bytes for block id "blockID". Store
        /// the result in "numDataBytesInBlock", and "numECBytesInBlock". See table 12 in 8.5.1 of
        /// JISX0510:2004 (p.30)
        /// </summary>
        ///
        static internal void GetNumDataBytesAndNumECBytesForBlockID(int numTotalBytes, int numDataBytes, int numRSBlocks, int blockID, int[] numDataBytesInBlock, int[] numECBytesInBlock)
        {
            if (blockID >= numRSBlocks)
            {
                throw new BarcodeEncoderException("Block ID too large");
            }
            // numRsBlocksInGroup2 = 196 % 5 = 1
            int numRsBlocksInGroup2 = numTotalBytes % numRSBlocks;
            // numRsBlocksInGroup1 = 5 - 1 = 4
            int numRsBlocksInGroup1 = numRSBlocks - numRsBlocksInGroup2;
            // numTotalBytesInGroup1 = 196 / 5 = 39
            int numTotalBytesInGroup1 = numTotalBytes / numRSBlocks;
            // numTotalBytesInGroup2 = 39 + 1 = 40
            int numTotalBytesInGroup2 = numTotalBytesInGroup1 + 1;
            // numDataBytesInGroup1 = 66 / 5 = 13
            int numDataBytesInGroup1 = numDataBytes / numRSBlocks;
            // numDataBytesInGroup2 = 13 + 1 = 14
            int numDataBytesInGroup2 = numDataBytesInGroup1 + 1;
            // numEcBytesInGroup1 = 39 - 13 = 26
            int numEcBytesInGroup1 = numTotalBytesInGroup1 - numDataBytesInGroup1;
            // numEcBytesInGroup2 = 40 - 14 = 26
            int numEcBytesInGroup2 = numTotalBytesInGroup2 - numDataBytesInGroup2;
            // Sanity checks.
            // 26 = 26
            if (numEcBytesInGroup1 != numEcBytesInGroup2)
            {
                throw new BarcodeEncoderException("EC bytes mismatch");
            }
            // 5 = 4 + 1.
            if (numRSBlocks != numRsBlocksInGroup1 + numRsBlocksInGroup2)
            {
                throw new BarcodeEncoderException("RS blocks mismatch");
            }
            // 196 = (13 + 26) * 4 + (14 + 26) * 1
            if (numTotalBytes != ((numDataBytesInGroup1 + numEcBytesInGroup1) * numRsBlocksInGroup1) + ((numDataBytesInGroup2 + numEcBytesInGroup2) * numRsBlocksInGroup2))
            {
                throw new BarcodeEncoderException("Total bytes mismatch");
            }

            if (blockID < numRsBlocksInGroup1)
            {
                numDataBytesInBlock[0] = numDataBytesInGroup1;
                numECBytesInBlock[0] = numEcBytesInGroup1;
            }
            else
            {
                numDataBytesInBlock[0] = numDataBytesInGroup2;
                numECBytesInBlock[0] = numEcBytesInGroup2;
            }
        }

        /// <summary>
        /// Interleave "bits" with corresponding error correction bytes. On success, store the result in
        /// "result". The interleave rule is complicated. See 8.6 of JISX0510:2004 (p.37) for details.
        /// </summary>
        static internal BitArray InterleaveWithECBytes(BitArray bits, int numTotalBytes, int numDataBytes, int numRSBlocks)
        {

            // "bits" must have "getNumDataBytes" bytes of data.
            if (bits.GetSizeInBytes() != numDataBytes)
            {
                throw new BarcodeEncoderException(
                        "Number of bits and data bytes does not match");
            }

            // Step 1.  Divide data bytes into blocks and generate error correction bytes for them. We'll
            // store the divided data bytes blocks and error correction bytes blocks into "blocks".
            int dataBytesOffset = 0;
            int maxNumDataBytes = 0;
            int maxNumEcBytes = 0;

            // Since, we know the number of reedsolmon blocks, we can initialize the vector with the number.
            List<BlockPair> blocks = new List<BlockPair>(numRSBlocks);

            for (int i = 0; i < numRSBlocks; ++i)
            {
                int[] numDataBytesInBlock = new int[1];
                int[] numEcBytesInBlock = new int[1];
                GetNumDataBytesAndNumECBytesForBlockID(numTotalBytes, numDataBytes, numRSBlocks, i, numDataBytesInBlock, numEcBytesInBlock);

                int size = numDataBytesInBlock[0];
                byte[] dataBytes = new byte[size];
                bits.ToBytes(8 * dataBytesOffset, dataBytes, 0, size);
                byte[] ecBytes = GenerateECBytes(dataBytes, numEcBytesInBlock[0]);
                blocks.Add(new BlockPair(dataBytes, ecBytes));

                maxNumDataBytes = Math.Max(maxNumDataBytes, size);
                maxNumEcBytes = Math.Max(maxNumEcBytes, ecBytes.Length);
                dataBytesOffset += numDataBytesInBlock[0];
            }
            if (numDataBytes != dataBytesOffset)
            {
                throw new BarcodeEncoderException("Data bytes does not match offset");
            }

            BitArray result = new BitArray();
            // First, place data blocks.
            for (int i_0 = 0; i_0 < maxNumDataBytes; ++i_0)
            {
                /* foreach */
                foreach (BlockPair block in blocks)
                {
                    byte[] dataBytes_1 = block.GetDataBytes();
                    if (i_0 < dataBytes_1.Length)
                    {
                        result.AppendBits(dataBytes_1[i_0], 8);
                    }
                }
            }
            // Then, place error correction blocks.
            for (int i_2 = 0; i_2 < maxNumEcBytes; ++i_2)
            {
                /* foreach */
                foreach (BlockPair block_3 in blocks)
                {
                    byte[] ecBytes_4 = block_3.GetErrorCorrectionBytes();
                    if (i_2 < ecBytes_4.Length)
                    {
                        result.AppendBits(ecBytes_4[i_2], 8);
                    }
                }
            }
            if (numTotalBytes != result.GetSizeInBytes())
            {
                // Should be same.
                throw new BarcodeEncoderException("Interleaving error: " + numTotalBytes
                      + " and " + result.GetSizeInBytes() + " differ.");
            }
            return result;
        }

        static internal byte[] GenerateECBytes(byte[] dataBytes, int numEcBytesInBlock)
        {
            int numDataBytes = dataBytes.Length;
            int[] toEncode = new int[numDataBytes + numEcBytesInBlock];
            for (int i = 0; i < numDataBytes; i++)
            {
                toEncode[i] = dataBytes[i] & 0xFF;
            }
            new ReedSolomonEncoder(GenericGF.QRCodeField256).Encode(toEncode, numEcBytesInBlock);

            byte[] ecBytes = new byte[numEcBytesInBlock];
            for (int i_0 = 0; i_0 < numEcBytesInBlock; i_0++)
            {
                ecBytes[i_0] = (byte)toEncode[numDataBytes + i_0];
            }
            return ecBytes;
        }

        /// <summary>
        /// Append mode info. On success, store the result in "bits".
        /// </summary>
        ///
        static internal void AppendModeInfo(Mode mode, BitArray bits)
        {
            bits.AppendBits(mode.Bits, 4);
        }

        /// <summary>
        /// Append length info. On success, store the result in "bits".
        /// </summary>
        ///
        static internal void AppendLengthInfo(int numLetters, MessagingToolkit.Barcode.QRCode.Decoder.Version version, Mode mode, BitArray bits)
        {
            int numBits = mode.GetCharacterCountBits(version);
            if (numLetters >= (1 << numBits))
            {
                throw new BarcodeEncoderException(numLetters + " is bigger than " + ((1 << numBits) - 1));
            }
            bits.AppendBits(numLetters, numBits);
        }

        /// <summary>
        /// Append "bytes" in "mode" mode (encoding) into "bits". On success, store the result in "bits".
        /// </summary>
        static internal void AppendBytes(String content, Mode mode, BitArray bits, String encoding)
        {

            if (mode.Equals(MessagingToolkit.Barcode.QRCode.Decoder.Mode.Numeric))
            {
                AppendNumericBytes(content, bits);
            }
            else if (mode.Equals(MessagingToolkit.Barcode.QRCode.Decoder.Mode.Alphanumeric))
            {
                AppendAlphanumericBytes(content, bits);
            }
            else if (mode.Equals(MessagingToolkit.Barcode.QRCode.Decoder.Mode.Byte))
            {
                Append8BitBytes(content, bits, encoding);
            }
            else if (mode.Equals(MessagingToolkit.Barcode.QRCode.Decoder.Mode.Kanji))
            {
                AppendKanjiBytes(content, bits);
            }
            else
            {
                throw new BarcodeEncoderException("Invalid mode: " + mode);
            }
        }

        static internal void AppendNumericBytes(String content, BitArray bits)
        {
            int length = content.Length;
            int i = 0;
            while (i < length)
            {
                int num1 = content[i] - '0';
                if (i + 2 < length)
                {
                    // Encode three numeric letters in ten bits.
                    int num2 = content[i + 1] - '0';
                    int num3 = content[i + 2] - '0';
                    bits.AppendBits(num1 * 100 + num2 * 10 + num3, 10);
                    i += 3;
                }
                else if (i + 1 < length)
                {
                    // Encode two numeric letters in seven bits.
                    int num2_0 = content[i + 1] - '0';
                    bits.AppendBits(num1 * 10 + num2_0, 7);
                    i += 2;
                }
                else
                {
                    // Encode one numeric letter in four bits.
                    bits.AppendBits(num1, 4);
                    i++;
                }
            }
        }

        static internal void AppendAlphanumericBytes(String content, BitArray bits)
        {
            int length = content.Length;
            int i = 0;
            while (i < length)
            {
                int code1 = GetAlphanumericCode(content[i]);
                if (code1 == -1)
                {
                    throw new BarcodeEncoderException();
                }
                if (i + 1 < length)
                {
                    int code2 = GetAlphanumericCode(content[i + 1]);
                    if (code2 == -1)
                    {
                        throw new BarcodeEncoderException();
                    }
                    // Encode two alphanumeric letters in 11 bits.
                    bits.AppendBits(code1 * 45 + code2, 11);
                    i += 2;
                }
                else
                {
                    // Encode one alphanumeric letter in six bits.
                    bits.AppendBits(code1, 6);
                    i++;
                }
            }
        }

        static internal void Append8BitBytes(String content, BitArray bits, String encoding)
        {
            byte[] bytes;
            try
            {
                bytes = Encoding.GetEncoding(encoding).GetBytes(content);
            }
            catch (IOException uee)
            {
                throw new BarcodeEncoderException(uee.ToString());
            }
            /* foreach */
            foreach (byte b in bytes)
            {
                bits.AppendBits(b, 8);
            }
        }

        static internal void AppendKanjiBytes(String content, BitArray bits)
        {
            byte[] bytes;
            try
            {
                bytes = Encoding.GetEncoding("SHIFT-JIS").GetBytes(content);
            }
            catch (IOException uee)
            {
                throw new BarcodeEncoderException(uee.ToString());
            }
            int length = bytes.Length;
            for (int i = 0; i < length; i += 2)
            {
                int byte1 = bytes[i] & 0xFF;
                int byte2 = bytes[i + 1] & 0xFF;
                int code = (byte1 << 8) | byte2;
                int subtracted = -1;
                if (code >= 0x8140 && code <= 0x9ffc)
                {
                    subtracted = code - 0x8140;
                }
                else if (code >= 0xe040 && code <= 0xebbf)
                {
                    subtracted = code - 0xc140;
                }
                if (subtracted == -1)
                {
                    throw new BarcodeEncoderException("Invalid byte sequence");
                }
                int encoded = ((subtracted >> 8) * 0xc0) + (subtracted & 0xff);
                bits.AppendBits(encoded, 13);
            }
        }

        private static void AppendECI(CharacterSetECI eci, BitArray bits)
        {
            bits.AppendBits(Decoder.Mode.Eci.Bits, 4);
            // This is correct for values up to 127, which is all we need now.
            bits.AppendBits(eci.Value, 8);
        }

    }
}
