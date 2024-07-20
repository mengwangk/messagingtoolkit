using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MessagingToolkit.Barcode.Pdf417.Decoder
{
    internal sealed class BarcodeMetadata
    {
        private readonly int columnCount;
        private readonly int errorCorrectionLevel;
        private readonly int rowCountUpperPart;
        private readonly int rowCountLowerPart;
        private readonly int rowCount;

        internal BarcodeMetadata(int columnCount, int rowCountUpperPart, int rowCountLowerPart, int errorCorrectionLevel)
        {
            this.columnCount = columnCount;
            this.errorCorrectionLevel = errorCorrectionLevel;
            this.rowCountUpperPart = rowCountUpperPart;
            this.rowCountLowerPart = rowCountLowerPart;
            this.rowCount = rowCountUpperPart + rowCountLowerPart;
        }

        internal int ColumnCount
        {
            get
            {
                return columnCount;
            }
        }

        internal int ErrorCorrectionLevel
        {
            get
            {
                return errorCorrectionLevel;
            }
        }

        internal int RowCount
        {
            get
            {
                return rowCount;
            }
        }

        internal int RowCountUpperPart
        {
            get
            {
                return rowCountUpperPart;
            }
        }

        internal int RowCountLowerPart
        {
            get
            {
                return rowCountLowerPart;
            }
        }

    }
}
