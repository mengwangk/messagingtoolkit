using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MessagingToolkit.Barcode.DataMatrix.Encoder
{

    /// <summary>
    /// Symbol info table for DataMatrix.
    /// </summary>
    internal class SymbolInfo
    {

        public static readonly SymbolInfo[] ProdSymbols = new SymbolInfo[] 
                        { 
                          new SymbolInfo(false, 3, 5, 8, 8, 1), 
                          new SymbolInfo(false, 5, 7, 10, 10, 1), 
                          new SymbolInfo(true, 5, 7, 16, 6, 1), 
                          new SymbolInfo(false, 8, 10, 12, 12, 1), 
                          new SymbolInfo(true, 10, 11, 14, 6, 2), 
                          new SymbolInfo(false, 12, 12, 14, 14, 1), 
                          new SymbolInfo(true, 16, 14, 24, 10, 1), 
                          new SymbolInfo(false, 18, 14, 16, 16, 1), 
                          new SymbolInfo(false, 22, 18, 18, 18, 1), 
                          new SymbolInfo(true, 22, 18, 16, 10, 2), 
                          new SymbolInfo(false, 30, 20, 20, 20, 1), 
                          new SymbolInfo(true, 32, 24, 16, 14, 2), 
                          new SymbolInfo(false, 36, 24, 22, 22, 1), 
                          new SymbolInfo(false, 44, 28, 24, 24, 1), 
                          new SymbolInfo(true, 49, 28, 22, 14, 2), 
                          new SymbolInfo(false, 62, 36, 14, 14, 4), 
                          new SymbolInfo(false, 86, 42, 16, 16, 4), 
                          new SymbolInfo(false, 114, 48, 18, 18, 4), 
                          new SymbolInfo(false, 144, 56, 20, 20, 4), 
                          new SymbolInfo(false, 174, 68, 22, 22, 4), 
                          new SymbolInfo(false, 204, 84, 24, 24, 4, 102, 42), 
                          new SymbolInfo(false, 280, 112, 14, 14, 16, 140, 56), 
                          new SymbolInfo(false, 368, 144, 16, 16, 16, 92, 36), 
                          new SymbolInfo(false, 456, 192, 18, 18, 16, 114, 48), 
                          new SymbolInfo(false, 576, 224, 20, 20, 16, 144, 56), 
                          new SymbolInfo(false, 696, 272, 22, 22, 16, 174, 68), 
                          new SymbolInfo(false, 816, 336, 24, 24, 16, 136, 56), 
                          new SymbolInfo(false, 1050, 408, 18, 18, 36, 175, 68), 
                          new SymbolInfo(false, 1304, 496, 20, 20, 36, 163, 62), 
                          new DataMatrixSymbolInfo144() };

        private static SymbolInfo[] symbols = ProdSymbols;

        /// <summary>
        /// Overrides the symbol info set used by this class. Used for testing purposes. </summary>
        /// <param name="override"> the symbol info set to use </param>
        public static void OverrideSymbolSet(SymbolInfo[] s)
        {
            symbols = s;
        }

        public bool Rectangular;
        public int dataCapacity;
        public int errorCodewords;
        public int matrixWidth;
        public int matrixHeight;
        public int dataRegions;
        public int rsBlockData;
        public int rsBlockError;

        public SymbolInfo(bool rectangular, int dataCapacity, int errorCodewords, int matrixWidth, int matrixHeight, int dataRegions)
            : this(rectangular, dataCapacity, errorCodewords, matrixWidth, matrixHeight, dataRegions, dataCapacity, errorCodewords)
        {
        }

        public SymbolInfo(bool rectangular, int dataCapacity, int errorCodewords, int matrixWidth, int matrixHeight, int dataRegions, int rsBlockData, int rsBlockError)
        {
            this.Rectangular = rectangular;
            this.dataCapacity = dataCapacity;
            this.errorCodewords = errorCodewords;
            this.matrixWidth = matrixWidth;
            this.matrixHeight = matrixHeight;
            this.dataRegions = dataRegions;
            this.rsBlockData = rsBlockData;
            this.rsBlockError = rsBlockError;
        }

        public static SymbolInfo Lookup(int dataCodewords)
        {
            return Lookup(dataCodewords, SymbolShapeHint.ForceNone, true);
        }

        public static SymbolInfo Lookup(int dataCodewords, SymbolShapeHint shape)
        {
            return Lookup(dataCodewords, shape, true);
        }

        public static SymbolInfo Lookup(int dataCodewords, bool allowRectangular, bool fail)
        {
            SymbolShapeHint shape = allowRectangular ? SymbolShapeHint.ForceNone : SymbolShapeHint.ForceSquare;
            return Lookup(dataCodewords, shape, fail);
        }

        public static SymbolInfo Lookup(int dataCodewords, SymbolShapeHint shape, bool fail)
        {
            return Lookup(dataCodewords, shape, null, null, fail);
        }

        public static SymbolInfo Lookup(int dataCodewords, SymbolShapeHint shape, Dimension minSize, Dimension maxSize, bool fail)
        {
            for (int i = 0, c = symbols.Length; i < c; i++)
            {
                SymbolInfo symbol = symbols[i];
                if (shape == SymbolShapeHint.ForceSquare && symbol.Rectangular)
                {
                    continue;
                }
                if (shape == SymbolShapeHint.ForceRectangle && !symbol.Rectangular)
                {
                    continue;
                }
                if (minSize != null && (symbol.SymbolWidth < minSize.Width || symbol.SymbolHeight < minSize.Height))
                {
                    continue;
                }
                if (maxSize != null && (symbol.SymbolWidth > maxSize.Width || symbol.SymbolHeight > maxSize.Height))
                {
                    continue;
                }
                if (dataCodewords <= symbol.dataCapacity)
                {
                    return symbol;
                }
            }
            if (fail)
            {
                throw new ArgumentException("Can't find a symbol arrangement that matches the message. Data codewords: " + dataCodewords);
            }
            return null;
        }

        public virtual int HorzDataRegions
        {
            get
            {
                switch (dataRegions)
                {
                    case 1:
                        return 1;
                    case 2:
                        return 2;
                    case 4:
                        return 2;
                    case 16:
                        return 4;
                    case 36:
                        return 6;
                    default:
                        throw new ArgumentException("Cannot handle this number of data regions");
                }
            }
        }

        public virtual int VertDataRegions
        {
            get
            {
                switch (dataRegions)
                {
                    case 1:
                        return 1;
                    case 2:
                        return 1;
                    case 4:
                        return 2;
                    case 16:
                        return 4;
                    case 36:
                        return 6;
                    default:
                        throw new ArgumentException("Cannot handle this number of data regions");
                }
            }
        }

        public virtual int SymbolDataWidth
        {
            get
            {
                return HorzDataRegions * matrixWidth;
            }
        }

        public virtual int SymbolDataHeight
        {
            get
            {
                return VertDataRegions * matrixHeight;
            }
        }

        public virtual int SymbolWidth
        {
            get
            {
                return SymbolDataWidth + (HorzDataRegions * 2);
            }
        }

        public virtual int SymbolHeight
        {
            get
            {
                return SymbolDataHeight + (VertDataRegions * 2);
            }
        }

        public virtual int CodewordCount
        {
            get
            {
                return dataCapacity + errorCodewords;
            }
        }

        public virtual int InterleavedBlockCount
        {
            get
            {
                return dataCapacity / rsBlockData;
            }
        }

        public virtual int GetDataLengthForInterleavedBlock(int index)
        {
            return rsBlockData;
        }

        public virtual int GetErrorLengthForInterleavedBlock(int index)
        {
            return rsBlockError;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(Rectangular ? "Rectangular Symbol:" : "Square Symbol:");
            sb.Append(" data region ").Append(matrixWidth).Append("x").Append(matrixHeight);
            sb.Append(", symbol size ").Append(SymbolWidth).Append("x").Append(SymbolHeight);
            sb.Append(", symbol data size ").Append(SymbolDataWidth).Append("x").Append(SymbolDataHeight);
            sb.Append(", codewords ").Append(dataCapacity).Append("+").Append(errorCodewords);
            return sb.ToString();
        }

        /**
        private class DataMatrixSymbolInfo144 : SymbolInfo
        {

            public DataMatrixSymbolInfo144()
                : base(false, 1558, 620, 22, 22, 36)
            {
                this.rsBlockData = -1; //special! see below
                this.rsBlockError = 62;
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
        */ 
        

    }
}
