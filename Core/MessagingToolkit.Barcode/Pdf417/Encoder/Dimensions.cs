using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MessagingToolkit.Barcode.Pdf417.Encoder
{
    /// <summary>
    /// Data object to specify the minimum and maximum number of rows and columns for a PDF417 barcode.
    /// </summary>
    public sealed class Dimensions
    {
        private int minCols;
        private int maxCols;
        private int minRows;
        private int maxRows;

        /// <summary>
        /// Initializes a new instance of the <see cref="Dimensions"/> class.
        /// </summary>
        /// <param name="minCols">The min cols.</param>
        /// <param name="maxCols">The max cols.</param>
        /// <param name="minRows">The min rows.</param>
        /// <param name="maxRows">The max rows.</param>
        public Dimensions(int minCols, int maxCols, int minRows, int maxRows)
        {
            this.minCols = minCols;
            this.maxCols = maxCols;
            this.minRows = minRows;
            this.maxRows = maxRows;
        }

        public int MinCols
        {
            get
            {
                return minCols;
            }
        }

        public int MaxCols
        {
            get
            {
                return maxCols;
            }
        }

        public int MinRows
        {
            get
            {
                return minRows;
            }
        }

        public int MaxRows
        {
            get
            {
                return maxRows;
            }
        }
    }

}

