using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MessagingToolkit.Barcode;
using MessagingToolkit.Barcode.Pdf417.Decoder;

namespace MessagingToolkit.Barcode.Pdf417.Detector
{
    /// <summary>
    /// PDF417 barcode
    /// </summary>
    public sealed class Pdf417Barcode
    {
        private int[][] matrix;
        private int rows;
        private int columns;
        private int ecLevel;
        private int erasures;


        /// <summary>
        /// Prevents a default instance of the <see cref="Pdf417Barcode"/> class from being created.
        /// </summary>
        /// <param name="r">The r.</param>
        /// <param name="c">The c.</param>
        /// <param name="ec">The ec.</param>
        public Pdf417Barcode(int r, int c, int ec)
        {
            this.rows = r;
            this.columns = c;
            this.ecLevel = ec;
            this.matrix = new int[columns][];
            for (int x = 0; x < columns; x++)
            {
                this.matrix[x] = new int[rows];
            }
            for (int x = 0; x < this.columns; x++)
            {
                for (int y = 0; y < this.rows; y++)
                {
                    this.matrix[x][y] = -1;
                }
            }
            erasures = rows * columns;
        }

        public void Set(int x, int y, int codeword)
        {
            if (x >= 0 && x < columns && y >= 0 && y < rows && codeword >= 0)
            {
                if (matrix[x][y] == -1)
                {
                    erasures--;
                }
                matrix[x][y] = codeword;
            }
        }

        public void SetGuess(int x, int y, int codeword)
        {
            if (x >= 0 && x < columns && y >= 0 && y < rows && codeword >= 0 && matrix[x][y] == -1)
            {
                matrix[x][y] = codeword;
                erasures--;
            }
        }

        public int Get(int x, int y)
        {
            return matrix[x][y];
        }


        public int GetRows() { return this.rows; }
        public int GetColumns() { return this.columns; }
        public int GetECLevel() { return this.ecLevel; }
        public int GetErasures() { return this.erasures; }

        public int[] GetCodewords()
        {
            int[] codewords = new int[rows * columns];
            for (int y = 0; y < rows; y++)
            {
                for (int x = 0; x < columns; x++)
                {
                    int i = y * columns + x;
                    if (matrix[x][y] != -1)
                    {
                        codewords[i] = matrix[x][y];
                    }
                    else
                    {
                        codewords[i] = 0;
                    }
                }
            }
            return codewords;
        }

        public bool CorrectErrors(int[] codewords)
        {
            int ecCodewords = (2 << (ecLevel + 1));
            if (codewords.Length < 4)
            {
                throw ChecksumException.Instance;
            }

            int numberOfCodewords = codewords[0];
            if (numberOfCodewords > codewords.Count() || numberOfCodewords < 0)
            {
                throw ChecksumException.Instance;
            }
            if (numberOfCodewords == 0)
            {
                // Reset to the length of the array - 8 (Allow for at least level 3 Error Correction (8 Error Codewords)
                if (ecCodewords < codewords.Count())
                {
                    codewords[0] = codewords.Count() - ecCodewords;
                }
                else
                {
                    throw ChecksumException.Instance;
                }
            }

            Pdf417RsDecoder decoder = new Pdf417RsDecoder();
            int num = decoder.CorrectErrors(codewords, null, 0, rows * columns, rows * columns - codewords[0]);

            if (num < 0)
            {
                return false;
            }
            else if (num > 0)
            {
                return true;
            }
            else
            {
                return true;
            }
        }
    }
}
