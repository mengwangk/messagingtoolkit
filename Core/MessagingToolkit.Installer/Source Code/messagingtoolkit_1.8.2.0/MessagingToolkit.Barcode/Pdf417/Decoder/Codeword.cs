using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MessagingToolkit.Barcode.Pdf417.Decoder
{
    internal sealed class Codeword
    {

        private const int BARCODE_ROW_UNKNOWN = -1;

        private readonly int startX;
        private readonly int endX;
        private readonly int bucket;
        private readonly int value;
        private int rowNumber = BARCODE_ROW_UNKNOWN;

        internal Codeword(int startX, int endX, int bucket, int value)
        {
            this.startX = startX;
            this.endX = endX;
            this.bucket = bucket;
            this.value = value;
        }

        internal bool HasValidRowNumber()
        {
            return IsValidRowNumber(rowNumber);
        }

        internal bool IsValidRowNumber(int rowNumber)
        {
            return rowNumber != BARCODE_ROW_UNKNOWN && bucket == (rowNumber % 3) * 3;
        }

        internal void SetRowNumberAsRowIndicatorColumn()
        {
            rowNumber = (value / 30) * 3 + bucket / 3;
        }

        internal int Width
        {
            get
            {
                return endX - startX;
            }
        }

        internal int StartX
        {
            get
            {
                return startX;
            }
        }

        internal int EndX
        {
            get
            {
                return endX;
            }
        }

        internal int Bucket
        {
            get
            {
                return bucket;
            }
        }

        internal int Value
        {
            get
            {
                return value;
            }
        }

        internal int RowNumber
        {
            get
            {
                return rowNumber;
            }
            set
            {
                this.rowNumber = value;
            }
        }

        public override string ToString()
        {
            return rowNumber + "|" + value;
        }

    }
}
