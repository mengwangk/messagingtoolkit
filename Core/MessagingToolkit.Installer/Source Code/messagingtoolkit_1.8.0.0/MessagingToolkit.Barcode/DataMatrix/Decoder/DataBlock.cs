using System;

namespace MessagingToolkit.Barcode.DataMatrix.Decoders
{

    /// <summary>
    /// <p>Encapsulates a block of data within a Data Matrix Code. Data Matrix Codes may split their data into
    /// multiple blocks, each of which is a unit of data and error-correction codewords. Each
    /// is represented by an instance of this class.</p>
    /// </summary>
    internal sealed class DataBlock
    {

        private readonly int numDataCodewords;
        private readonly byte[] codewords;

        private DataBlock(int numDataCodewords_0, byte[] codewords_1)
        {
            this.numDataCodewords = numDataCodewords_0;
            this.codewords = codewords_1;
        }

        /// <summary>
        /// <p>When Data Matrix Codes use multiple data blocks, they actually interleave the bytes of each of them.
        /// That is, the first byte of data block 1 to n is written, then the second bytes, and so on. This
        /// method will separate the data into original blocks.</p>
        /// </summary>
        ///
        /// <param name="rawCodewords">bytes as read directly from the Data Matrix Code</param>
        /// <param name="version">version of the Data Matrix Code</param>
        /// <returns>DataBlocks containing original bytes, "de-interleaved" from representation in the
        /// Data Matrix Code</returns>
        static internal DataBlock[] GetDataBlocks(byte[] rawCodewords, Version version)
        {
            // Figure out the number and size of data blocks used by this version
            Version.ECBlocks ecBlocks = version.GetECBlocks();

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
                    int numBlockCodewords = ecBlocks.GetECCodewords() + numDataCodewords_1;
                    result[numResultBlocks++] = new DataBlock(numDataCodewords_1, new byte[numBlockCodewords]);
                }
            }

            // All blocks have the same amount of data, except that the last n
            // (where n may be 0) have 1 less byte. Figure out where these start.
            // TODO(bbrown): There is only one case where there is a difference for Data Matrix for size 144
            int longerBlocksTotalCodewords = result[0].codewords.Length;
            //int shorterBlocksTotalCodewords = longerBlocksTotalCodewords - 1;

            int longerBlocksNumDataCodewords = longerBlocksTotalCodewords - ecBlocks.GetECCodewords();
            int shorterBlocksNumDataCodewords = longerBlocksNumDataCodewords - 1;
            // The last elements of result may be 1 element shorter for 144 matrix
            // first fill out as many elements as all of them have minus 1
            int rawCodewordsOffset = 0;
            for (int i_2 = 0; i_2 < shorterBlocksNumDataCodewords; i_2++)
            {
                for (int j = 0; j < numResultBlocks; j++)
                {
                    result[j].codewords[i_2] = rawCodewords[rawCodewordsOffset++];
                }
            }

            // Fill out the last data block in the longer ones
            bool specialVersion = version.GetVersionNumber() == 24;
            int numLongerBlocks = (specialVersion) ? 8 : numResultBlocks;
            for (int j_3 = 0; j_3 < numLongerBlocks; j_3++)
            {
                result[j_3].codewords[longerBlocksNumDataCodewords - 1] = rawCodewords[rawCodewordsOffset++];
            }

            // Now add in error correction blocks
            int max = result[0].codewords.Length;
            for (int i_4 = longerBlocksNumDataCodewords; i_4 < max; i_4++)
            {
                for (int j_5 = 0; j_5 < numResultBlocks; j_5++)
                {
                    int iOffset = (specialVersion && j_5 > 7) ? i_4 - 1 : i_4;
                    result[j_5].codewords[iOffset] = rawCodewords[rawCodewordsOffset++];
                }
            }

            if (rawCodewordsOffset != rawCodewords.Length)
            {
                throw new ArgumentException();
            }

            return result;
        }

        internal int GetNumDataCodewords()
        {
            return numDataCodewords;
        }

        internal byte[] GetCodewords()
        {
            return codewords;
        }

    }
}
