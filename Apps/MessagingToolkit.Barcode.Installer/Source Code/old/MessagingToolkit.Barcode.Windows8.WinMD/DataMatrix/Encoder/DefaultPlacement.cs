using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MessagingToolkit.Barcode.DataMatrix.Encoder
{
    /// <summary>
    /// Symbol Character Placement Program. Adapted from Annex M.1 in ISO/IEC 16022:2000(E). 
    /// </summary>
    internal class DefaultPlacement
    {
        private string codewords;
        protected internal int numrows;
        protected internal int numcols;
        protected internal sbyte[] bits;

        /// <summary>
        /// Main constructor </summary>
        /// <param name="codewords"> the codewords to place </param>
        /// <param name="numcols"> the number of columns </param>
        /// <param name="numrows"> the number of rows </param>
        public DefaultPlacement(string codewords, int numcols, int numrows)
        {
            this.codewords = codewords;
            this.numcols = numcols;
            this.numrows = numrows;
            this.bits = new sbyte[numcols * numrows];

            for (int i = 0; i < this.bits.Length; i++)
                this.bits[i] = -1;

        }

        internal int Numrows
        {
            get
            {
                return numrows;
            }
        }

        internal int Numcols
        {
            get
            {
                return numcols;
            }
        }

        internal sbyte[] Bits
        {
            get
            {
                return bits;
            }
        }

        public virtual bool GetBit(int col, int row)
        {
            return bits[row * numcols + col] == 1;
        }

        public virtual void SetBit(int col, int row, bool bit)
        {
            bits[row * numcols + col] = (bit ? (sbyte)1 : (sbyte)0);
        }

        public virtual bool HasBit(int col, int row)
        {
            return bits[row * numcols + col] >= 0;
        }

        public virtual void Place()
        {
            int pos = 0;
            int row = 4;
            int col = 0;

            do
            {
                /* repeatedly first check for one of the special corner cases, then... */
                if ((row == numrows) && (col == 0))
                {
                    Corner1(pos++);
                }
                if ((row == numrows - 2) && (col == 0) && ((numcols % 4) != 0))
                {
                    Corner2(pos++);
                }
                if ((row == numrows - 2) && (col == 0) && ((numcols % 8 == 4)))
                {
                    Corner3(pos++);
                }
                if ((row == numrows + 4) && (col == 2) && (!((numcols % 8) != 0)))
                {
                    Corner4(pos++);
                }
                /* sweep upward diagonally, inserting successive characters... */
                do
                {
                    if ((row < numrows) && (col >= 0) && !HasBit(col, row))
                    {
                        Utah(row, col, pos++);
                    }
                    row -= 2;
                    col += 2;
                } while ((row >= 0 && (col < numcols)));
                row++;
                col += 3;

                /* and then sweep downward diagonally, inserting successive characters, ... */
                do
                {
                    if ((row >= 0) && (col < numcols) && !HasBit(col, row))
                    {
                        Utah(row, col, pos++);
                    }
                    row += 2;
                    col -= 2;
                } while ((row < numrows) && (col >= 0));
                row += 3;
                col += 1;

                /* ...until the entire array is scanned */
            } while ((row < numrows) || (col < numcols));

            /* Lastly, if the lower righthand corner is untouched, fill in fixed pattern */
            if (!HasBit(numcols - 1, numrows - 1))
            {
                SetBit(numcols - 1, numrows - 1, true);
                SetBit(numcols - 2, numrows - 2, true);
            }
        }

        private void Module(int row, int col, int pos, int bit)
        {
            if (row < 0)
            {
                row += numrows;
                col += 4 - ((numrows + 4) % 8);
            }
            if (col < 0)
            {
                col += numcols;
                row += 4 - ((numcols + 4) % 8);
            }
            // Note the conversion:
            int v = codewords[pos];
            v &= 1 << (8 - bit);
            SetBit(col, row, v != 0);

        }

        /// <summary>
        /// Places the 8 bits of a utah-shaped symbol character in ECC200. </summary>
        /// <param name="row"> the row </param>
        /// <param name="col"> the column </param>
        /// <param name="pos"> character position </param>
        private void Utah(int row, int col, int pos)
        {
            Module(row - 2, col - 2, pos, 1);
            Module(row - 2, col - 1, pos, 2);
            Module(row - 1, col - 2, pos, 3);
            Module(row - 1, col - 1, pos, 4);
            Module(row - 1, col, pos, 5);
            Module(row, col - 2, pos, 6);
            Module(row, col - 1, pos, 7);
            Module(row, col, pos, 8);
        }

        private void Corner1(int pos)
        {
            Module(numrows - 1, 0, pos, 1);
            Module(numrows - 1, 1, pos, 2);
            Module(numrows - 1, 2, pos, 3);
            Module(0, numcols - 2, pos, 4);
            Module(0, numcols - 1, pos, 5);
            Module(1, numcols - 1, pos, 6);
            Module(2, numcols - 1, pos, 7);
            Module(3, numcols - 1, pos, 8);
        }

        private void Corner2(int pos)
        {
            Module(numrows - 3, 0, pos, 1);
            Module(numrows - 2, 0, pos, 2);
            Module(numrows - 1, 0, pos, 3);
            Module(0, numcols - 4, pos, 4);
            Module(0, numcols - 3, pos, 5);
            Module(0, numcols - 2, pos, 6);
            Module(0, numcols - 1, pos, 7);
            Module(1, numcols - 1, pos, 8);
        }

        private void Corner3(int pos)
        {
            Module(numrows - 3, 0, pos, 1);
            Module(numrows - 2, 0, pos, 2);
            Module(numrows - 1, 0, pos, 3);
            Module(0, numcols - 2, pos, 4);
            Module(0, numcols - 1, pos, 5);
            Module(1, numcols - 1, pos, 6);
            Module(2, numcols - 1, pos, 7);
            Module(3, numcols - 1, pos, 8);
        }

        private void Corner4(int pos)
        {
            Module(numrows - 1, 0, pos, 1);
            Module(numrows - 1, numcols - 1, pos, 2);
            Module(0, numcols - 3, pos, 3);
            Module(0, numcols - 2, pos, 4);
            Module(0, numcols - 1, pos, 5);
            Module(1, numcols - 3, pos, 6);
            Module(1, numcols - 2, pos, 7);
            Module(1, numcols - 1, pos, 8);
        }

    }
}
