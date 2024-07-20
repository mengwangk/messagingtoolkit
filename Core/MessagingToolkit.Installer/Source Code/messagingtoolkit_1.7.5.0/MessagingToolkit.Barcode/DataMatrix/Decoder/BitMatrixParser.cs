using System;
using BarcodeDecoderException = MessagingToolkit.Barcode.BarcodeDecoderException;
using BitMatrix = MessagingToolkit.Barcode.Common.BitMatrix;

namespace MessagingToolkit.Barcode.DataMatrix.Decoders
{
    internal sealed class BitMatrixParser
    {

        private readonly BitMatrix mappingBitMatrix;
        private readonly BitMatrix readMappingMatrix;
        private readonly Version version;


        /// <param name="bitMatrix">to parse</param>
        /// <exception cref="FormatException">if dimension is < 8 or > 144 or not 0 mod 2</exception>
        internal BitMatrixParser(BitMatrix bitMatrix)
        {
            int dimension = bitMatrix.Height;
            if (dimension < 8 || dimension > 144 || (dimension & 0x01) != 0)
            {
                throw FormatException.Instance;
            }

            version = ReadVersion(bitMatrix);
            this.mappingBitMatrix = ExtractDataRegion(bitMatrix);
            this.readMappingMatrix = new BitMatrix(this.mappingBitMatrix.Width, this.mappingBitMatrix.Height);
        }

        internal Version GetVersion()
        {
            return version;
        }

        /// <summary>
        ///   <p>Creates the version object based on the dimension of the original bit matrix from
        /// the datamatrix code.</p>
        ///   <p>See ISO 16022:2006 Table 7 - ECC 200 symbol attributes</p>
        /// </summary>
        /// <param name="bitMatrix">including alignment patterns</param>
        /// <returns></returns>
        private static Version ReadVersion(BitMatrix bitMatrix)
        {
            int numRows = bitMatrix.Height;
            int numColumns = bitMatrix.Width;
            return Version.GetVersionForDimensions(numRows, numColumns);
        }

        /// <summary>
        /// <p>Reads the bits in the <see cref="null"/> representing the mapping matrix (No alignment patterns)
        /// in the correct order in order to reconstitute the codewords bytes contained within the
        /// Data Matrix Code.</p>
        /// </summary>
        ///
        /// <returns>bytes encoded within the Data Matrix Code</returns>
        /// <exception cref="FormatException">if the exact number of bytes expected is not read</exception>
        internal byte[] ReadCodewords()
        {

            byte[] result = new byte[version.GetTotalCodewords()];
            int resultOffset = 0;

            int row = 4;
            int column = 0;

            int numRows = mappingBitMatrix.Height;
            int numColumns = mappingBitMatrix.Width;

            bool corner1Read = false;
            bool corner2Read = false;
            bool corner3Read = false;
            bool corner4Read = false;

            // Read all of the codewords
            do
            {
                // Check the four corner cases
                if ((row == numRows) && (column == 0) && !corner1Read)
                {
                    result[resultOffset++] = (byte)ReadCorner1(numRows, numColumns);
                    row -= 2;
                    column += 2;
                    corner1Read = true;
                }
                else if ((row == numRows - 2) && (column == 0) && ((numColumns & 0x03) != 0) && !corner2Read)
                {
                    result[resultOffset++] = (byte)ReadCorner2(numRows, numColumns);
                    row -= 2;
                    column += 2;
                    corner2Read = true;
                }
                else if ((row == numRows + 4) && (column == 2) && ((numColumns & 0x07) == 0) && !corner3Read)
                {
                    result[resultOffset++] = (byte)ReadCorner3(numRows, numColumns);
                    row -= 2;
                    column += 2;
                    corner3Read = true;
                }
                else if ((row == numRows - 2) && (column == 0) && ((numColumns & 0x07) == 4) && !corner4Read)
                {
                    result[resultOffset++] = (byte)ReadCorner4(numRows, numColumns);
                    row -= 2;
                    column += 2;
                    corner4Read = true;
                }
                else
                {
                    // Sweep upward diagonally to the right
                    do
                    {
                        if ((row < numRows) && (column >= 0) && !readMappingMatrix.Get(column, row))
                        {
                            result[resultOffset++] = (byte)ReadUtah(row, column, numRows, numColumns);
                        }
                        row -= 2;
                        column += 2;
                    } while ((row >= 0) && (column < numColumns));
                    row += 1;
                    column += 3;

                    // Sweep downward diagonally to the left
                    do
                    {
                        if ((row >= 0) && (column < numColumns) && !readMappingMatrix.Get(column, row))
                        {
                            result[resultOffset++] = (byte)ReadUtah(row, column, numRows, numColumns);
                        }
                        row += 2;
                        column -= 2;
                    } while ((row < numRows) && (column >= 0));
                    row += 3;
                    column += 1;
                }
            } while ((row < numRows) || (column < numColumns));

            if (resultOffset != version.GetTotalCodewords())
            {
                throw FormatException.Instance;
            }
            return result;
        }

        /// <summary>
        ///   <p>Reads a bit of the mapping matrix accounting for boundary wrapping.</p>
        /// </summary>
        /// <param name="row">Row to read in the mapping matrix</param>
        /// <param name="column">Column to read in the mapping matrix</param>
        /// <param name="numRows">Number of rows in the mapping matrix</param>
        /// <param name="numColumns">Number of columns in the mapping matrix</param>
        /// <returns>
        /// value of the given bit in the mapping matrix
        /// </returns>
        internal bool ReadModule(int row, int column, int numRows, int numColumns)
        {
            // Adjust the row and column indices based on boundary wrapping
            if (row < 0)
            {
                row += numRows;
                column += 4 - ((numRows + 4) & 0x07);
            }
            if (column < 0)
            {
                column += numColumns;
                row += 4 - ((numColumns + 4) & 0x07);
            }
            readMappingMatrix.Set(column, row);
            return mappingBitMatrix.Get(column, row);
        }

        /// <summary>
        ///   <p>Reads the 8 bits of the standard Utah-shaped pattern.</p>
        ///   <p>See ISO 16022:2006, 5.8.1 Figure 6</p>
        /// </summary>
        /// <param name="row">Current row in the mapping matrix, anchored at the 8th bit (LSB) of the pattern</param>
        /// <param name="column">Current column in the mapping matrix, anchored at the 8th bit (LSB) of the pattern</param>
        /// <param name="numRows">Number of rows in the mapping matrix</param>
        /// <param name="numColumns">Number of columns in the mapping matrix</param>
        /// <returns>
        /// byte from the utah shape
        /// </returns>
        internal int ReadUtah(int row, int column, int numRows, int numColumns)
        {
            int currentByte = 0;
            if (ReadModule(row - 2, column - 2, numRows, numColumns))
            {
                currentByte |= 1;
            }
            currentByte <<= 1;
            if (ReadModule(row - 2, column - 1, numRows, numColumns))
            {
                currentByte |= 1;
            }
            currentByte <<= 1;
            if (ReadModule(row - 1, column - 2, numRows, numColumns))
            {
                currentByte |= 1;
            }
            currentByte <<= 1;
            if (ReadModule(row - 1, column - 1, numRows, numColumns))
            {
                currentByte |= 1;
            }
            currentByte <<= 1;
            if (ReadModule(row - 1, column, numRows, numColumns))
            {
                currentByte |= 1;
            }
            currentByte <<= 1;
            if (ReadModule(row, column - 2, numRows, numColumns))
            {
                currentByte |= 1;
            }
            currentByte <<= 1;
            if (ReadModule(row, column - 1, numRows, numColumns))
            {
                currentByte |= 1;
            }
            currentByte <<= 1;
            if (ReadModule(row, column, numRows, numColumns))
            {
                currentByte |= 1;
            }
            return currentByte;
        }

        /// <summary>
        /// <p>Reads the 8 bits of the special corner condition 1.</p>
        /// <p>See ISO 16022:2006, Figure F.3</p>
        /// </summary>
        ///
        /// <param name="numRows">Number of rows in the mapping matrix</param>
        /// <param name="numColumns">Number of columns in the mapping matrix</param>
        /// <returns>byte from the Corner condition 1</returns>
        internal int ReadCorner1(int numRows, int numColumns)
        {
            int currentByte = 0;
            if (ReadModule(numRows - 1, 0, numRows, numColumns))
            {
                currentByte |= 1;
            }
            currentByte <<= 1;
            if (ReadModule(numRows - 1, 1, numRows, numColumns))
            {
                currentByte |= 1;
            }
            currentByte <<= 1;
            if (ReadModule(numRows - 1, 2, numRows, numColumns))
            {
                currentByte |= 1;
            }
            currentByte <<= 1;
            if (ReadModule(0, numColumns - 2, numRows, numColumns))
            {
                currentByte |= 1;
            }
            currentByte <<= 1;
            if (ReadModule(0, numColumns - 1, numRows, numColumns))
            {
                currentByte |= 1;
            }
            currentByte <<= 1;
            if (ReadModule(1, numColumns - 1, numRows, numColumns))
            {
                currentByte |= 1;
            }
            currentByte <<= 1;
            if (ReadModule(2, numColumns - 1, numRows, numColumns))
            {
                currentByte |= 1;
            }
            currentByte <<= 1;
            if (ReadModule(3, numColumns - 1, numRows, numColumns))
            {
                currentByte |= 1;
            }
            return currentByte;
        }

        /// <summary>
        /// <p>Reads the 8 bits of the special corner condition 2.</p>
        /// <p>See ISO 16022:2006, Figure F.4</p>
        /// </summary>
        ///
        /// <param name="numRows">Number of rows in the mapping matrix</param>
        /// <param name="numColumns">Number of columns in the mapping matrix</param>
        /// <returns>byte from the Corner condition 2</returns>
        internal int ReadCorner2(int numRows, int numColumns)
        {
            int currentByte = 0;
            if (ReadModule(numRows - 3, 0, numRows, numColumns))
            {
                currentByte |= 1;
            }
            currentByte <<= 1;
            if (ReadModule(numRows - 2, 0, numRows, numColumns))
            {
                currentByte |= 1;
            }
            currentByte <<= 1;
            if (ReadModule(numRows - 1, 0, numRows, numColumns))
            {
                currentByte |= 1;
            }
            currentByte <<= 1;
            if (ReadModule(0, numColumns - 4, numRows, numColumns))
            {
                currentByte |= 1;
            }
            currentByte <<= 1;
            if (ReadModule(0, numColumns - 3, numRows, numColumns))
            {
                currentByte |= 1;
            }
            currentByte <<= 1;
            if (ReadModule(0, numColumns - 2, numRows, numColumns))
            {
                currentByte |= 1;
            }
            currentByte <<= 1;
            if (ReadModule(0, numColumns - 1, numRows, numColumns))
            {
                currentByte |= 1;
            }
            currentByte <<= 1;
            if (ReadModule(1, numColumns - 1, numRows, numColumns))
            {
                currentByte |= 1;
            }
            return currentByte;
        }

        /// <summary>
        ///   <p>Reads the 8 bits of the special corner condition 3.</p>
        ///   <p>See ISO 16022:2006, Figure F.5</p>
        /// </summary>
        /// <param name="numRows">Number of rows in the mapping matrix</param>
        /// <param name="numColumns">Number of columns in the mapping matrix</param>
        /// <returns>
        /// byte from the Corner condition 3
        /// </returns>
        internal int ReadCorner3(int numRows, int numColumns)
        {
            int currentByte = 0;
            if (ReadModule(numRows - 1, 0, numRows, numColumns))
            {
                currentByte |= 1;
            }
            currentByte <<= 1;
            if (ReadModule(numRows - 1, numColumns - 1, numRows, numColumns))
            {
                currentByte |= 1;
            }
            currentByte <<= 1;
            if (ReadModule(0, numColumns - 3, numRows, numColumns))
            {
                currentByte |= 1;
            }
            currentByte <<= 1;
            if (ReadModule(0, numColumns - 2, numRows, numColumns))
            {
                currentByte |= 1;
            }
            currentByte <<= 1;
            if (ReadModule(0, numColumns - 1, numRows, numColumns))
            {
                currentByte |= 1;
            }
            currentByte <<= 1;
            if (ReadModule(1, numColumns - 3, numRows, numColumns))
            {
                currentByte |= 1;
            }
            currentByte <<= 1;
            if (ReadModule(1, numColumns - 2, numRows, numColumns))
            {
                currentByte |= 1;
            }
            currentByte <<= 1;
            if (ReadModule(1, numColumns - 1, numRows, numColumns))
            {
                currentByte |= 1;
            }
            return currentByte;
        }

        /// <summary>
        ///   <p>Reads the 8 bits of the special corner condition 4.</p>
        ///   <p>See ISO 16022:2006, Figure F.6</p>
        /// </summary>
        /// <param name="numRows">Number of rows in the mapping matrix</param>
        /// <param name="numColumns">Number of columns in the mapping matrix</param>
        /// <returns>
        /// byte from the Corner condition 4
        /// </returns>
        internal int ReadCorner4(int numRows, int numColumns)
        {
            int currentByte = 0;
            if (ReadModule(numRows - 3, 0, numRows, numColumns))
            {
                currentByte |= 1;
            }
            currentByte <<= 1;
            if (ReadModule(numRows - 2, 0, numRows, numColumns))
            {
                currentByte |= 1;
            }
            currentByte <<= 1;
            if (ReadModule(numRows - 1, 0, numRows, numColumns))
            {
                currentByte |= 1;
            }
            currentByte <<= 1;
            if (ReadModule(0, numColumns - 2, numRows, numColumns))
            {
                currentByte |= 1;
            }
            currentByte <<= 1;
            if (ReadModule(0, numColumns - 1, numRows, numColumns))
            {
                currentByte |= 1;
            }
            currentByte <<= 1;
            if (ReadModule(1, numColumns - 1, numRows, numColumns))
            {
                currentByte |= 1;
            }
            currentByte <<= 1;
            if (ReadModule(2, numColumns - 1, numRows, numColumns))
            {
                currentByte |= 1;
            }
            currentByte <<= 1;
            if (ReadModule(3, numColumns - 1, numRows, numColumns))
            {
                currentByte |= 1;
            }
            return currentByte;
        }

        /// <summary>
        /// <p>Extracts the data region from a <see cref="null"/> that contains
        /// alignment patterns.</p>
        /// </summary>
        ///
        /// <param name="bitMatrix">with alignment patterns</param>
        /// <returns>BitMatrix that has the alignment patterns removed</returns>
        internal BitMatrix ExtractDataRegion(BitMatrix bitMatrix)
        {
            int symbolSizeRows = version.GetSymbolSizeRows();
            int symbolSizeColumns = version.GetSymbolSizeColumns();

            if (bitMatrix.Height != symbolSizeRows)
            {
                throw new ArgumentException("Dimension of bitMarix must match the version size");
            }

            int dataRegionSizeRows = version.GetDataRegionSizeRows();
            int dataRegionSizeColumns = version.GetDataRegionSizeColumns();

            int numDataRegionsRow = symbolSizeRows / dataRegionSizeRows;
            int numDataRegionsColumn = symbolSizeColumns / dataRegionSizeColumns;

            int sizeDataRegionRow = numDataRegionsRow * dataRegionSizeRows;
            int sizeDataRegionColumn = numDataRegionsColumn * dataRegionSizeColumns;

            BitMatrix bitMatrixWithoutAlignment = new BitMatrix(sizeDataRegionColumn, sizeDataRegionRow);
            for (int dataRegionRow = 0; dataRegionRow < numDataRegionsRow; ++dataRegionRow)
            {
                int dataRegionRowOffset = dataRegionRow * dataRegionSizeRows;
                for (int dataRegionColumn = 0; dataRegionColumn < numDataRegionsColumn; ++dataRegionColumn)
                {
                    int dataRegionColumnOffset = dataRegionColumn * dataRegionSizeColumns;
                    for (int i = 0; i < dataRegionSizeRows; ++i)
                    {
                        int readRowOffset = dataRegionRow * (dataRegionSizeRows + 2) + 1 + i;
                        int writeRowOffset = dataRegionRowOffset + i;
                        for (int j = 0; j < dataRegionSizeColumns; ++j)
                        {
                            int readColumnOffset = dataRegionColumn * (dataRegionSizeColumns + 2) + 1 + j;
                            if (bitMatrix.Get(readColumnOffset, readRowOffset))
                            {
                                int writeColumnOffset = dataRegionColumnOffset + j;
                                bitMatrixWithoutAlignment.Set(writeColumnOffset, writeRowOffset);
                            }
                        }
                    }
                }
            }
            return bitMatrixWithoutAlignment;
        }

    }
}
