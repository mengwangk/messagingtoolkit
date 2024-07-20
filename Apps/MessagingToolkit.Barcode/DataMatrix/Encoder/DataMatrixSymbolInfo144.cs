using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MessagingToolkit.Barcode.DataMatrix.Encoder
{
    internal sealed class DataMatrixSymbolInfo144 : SymbolInfo
    {

        internal DataMatrixSymbolInfo144()
            : base(false, 1558, 620, 22, 22, 36, -1, 62)
        {
        }

        public override int InterleavedBlockCount
        {
            get
            {
                return 10;
            }
        }

        public override int GetDataLengthForInterleavedBlock(int index)
        {
            return (index <= 8) ? 156 : 155;
        }

    }
}
