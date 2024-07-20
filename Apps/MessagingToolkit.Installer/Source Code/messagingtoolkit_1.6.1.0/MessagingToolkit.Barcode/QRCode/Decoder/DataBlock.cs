using System;

namespace MessagingToolkit.Barcode.QRCode.Decoder
{

    /// <summary>
    /// <p>Encapsulates a block of data within a QR Code. QR Codes may split their data into
    /// multiple blocks, each of which is a unit of data and error-correction codewords. Each
    /// is represented by an instance of this class.</p>
    /// 
    /// Modified: April 21 2012
    /// </summary>
    internal sealed class DataBlock
    {

        private readonly int numDataCodewords;
        private readonly byte[] codewords;

        private DataBlock(int numDataCodewords, byte[] codewords)
        {
            this.numDataCodewords = numDataCodewords;
            this.codewords = codewords;
        }

        /// <summary>
        ///   <p>When QR Codes use multiple data blocks, they are actually interleaved.
        /// That is, the first byte of data block 1 to n is written, then the second bytes, and so on. This
        /// method will separate the data into original blocks.</p>
        /// </summary>
        /// <param name="rawCodewords">bytes as read directly from the QR Code</param>
        /// <param name="version">version of the QR Code</param>
        /// <param name="ecLevel">error-correction level of the QR Code</param>
        /// <returns>
        /// DataBlocks containing original bytes, "de-interleaved" from representation in the
        /// QR Code
        /// </returns>
        static internal DataBlock[] GetDataBlocks(byte[] rawCodewords, Version version, ErrorCorrectionLevel ecLevel)
        {

            if (rawCodewords.Length != version.TotalCodewords)
            {
                throw new ArgumentException();
            }

            // Figure out the number and size of data blocks used by this version and
            // error correction level
            Version.ECBlocks ecBlocks = version.GetECBlocksForLevel(ecLevel);

            // First count the total number of data blocks
            int totalBlocks = 0;
            Version.ECB[] ecBlockArray = ecBlocks.GetECBlocks();
            /* foreach */
            foreach (Version.ECB ecBlock in ecBlockArray)
            {
                totalBlocks += ecBlock.GetCount();
            }

            // Now establish DataBlocks of the appropriate size and number of data codewords
            DataBlock[] result = new DataBlock[totalBlocks];
            int numResultBlocks = 0;
            /* foreach */
            foreach (Version.ECB ecBlock_0 in ecBlockArray)
            {
                for (int i = 0; i < ecBlock_0.GetCount(); i++)
                {
                    int numDataCodewords_1 = ecBlock_0.GetDataCodewords();
                    int numBlockCodewords = ecBlocks.ECCodewordsPerBlock + numDataCodewords_1;
                    result[numResultBlocks++] = new DataBlock(numDataCodewords_1, new byte[numBlockCodewords]);
                }
            }

            // All blocks have the same amount of data, except that the last n
            // (where n may be 0) have 1 more byte. Figure out where these start.
            int shorterBlocksTotalCodewords = result[0].codewords.Length;
            int longerBlocksStartAt = result.Length - 1;
            while (longerBlocksStartAt >= 0)
            {
                int numCodewords = result[longerBlocksStartAt].codewords.Length;
                if (numCodewords == shorterBlocksTotalCodewords)
                {
                    break;
                }
                longerBlocksStartAt--;
            }
            longerBlocksStartAt++;

            int shorterBlocksNumDataCodewords = shorterBlocksTotalCodewords - ecBlocks.ECCodewordsPerBlock;
            // The last elements of result may be 1 element longer;
            // first fill out as many elements as all of them have
            int rawCodewordsOffset = 0;
            for (int i_2 = 0; i_2 < shorterBlocksNumDataCodewords; i_2++)
            {
                for (int j = 0; j < numResultBlocks; j++)
                {
                    result[j].codewords[i_2] = rawCodewords[rawCodewordsOffset++];
                }
            }
            // Fill out the last data block in the longer ones
            for (int j_3 = longerBlocksStartAt; j_3 < numResultBlocks; j_3++)
            {
                result[j_3].codewords[shorterBlocksNumDataCodewords] = rawCodewords[rawCodewordsOffset++];
            }
            // Now add in error correction blocks
            int max = result[0].codewords.Length;
            for (int i_4 = shorterBlocksNumDataCodewords; i_4 < max; i_4++)
            {
                for (int j_5 = 0; j_5 < numResultBlocks; j_5++)
                {
                    int iOffset = (j_5 < longerBlocksStartAt) ? i_4 : i_4 + 1;
                    result[j_5].codewords[iOffset] = rawCodewords[rawCodewordsOffset++];
                }
            }
            return result;
        }

        internal int NumDataCodewords
        {
            get
            {
                return numDataCodewords;
            }
        }

        internal byte[] Codewords
        {
            get
            {
                return codewords;
            }
        }
	

    }
}
