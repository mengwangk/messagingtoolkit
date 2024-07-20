
namespace MessagingToolkit.Barcode.Pdf417.Encoder
{
    /// <summary>
    /// Holds all of the information for a barcode in a format where it can be easily accessable
    /// </summary>
    public sealed class BarcodeMatrix
    {
        private readonly BarcodeRow[] matrix;
        private int currentRow;
        private readonly int height;
        private readonly int width;


        public int Width
        {
            get
            {
                return this.width;
            }
        }

        public int Height
        {
            get
            {
                return this.height;
            }
        }

        /// <param name="height">the height of the matrix (Rows)</param>
        /// <param name="width">the width of the matrix (Cols)</param>
        internal BarcodeMatrix(int height, int width)
        {
            matrix = new BarcodeRow[height + 2];
            //Initializes the array to the correct width
            for (int i = 0, matrixLength = matrix.Length; i < matrixLength; i++)
            {
                matrix[i] = new BarcodeRow((width + 4) * 17 + 1);
            }
            this.width = width * 17;
            this.height = height + 2;
            this.currentRow = 0;
        }

        internal void Set(int x, int y, byte val)
        {
            matrix[y].Set(x, val);
        }

        internal void SetMatrix(int x, int y, bool black)
        {
            Set(x, y, (byte)((black) ? 1 : 0));
        }

        internal void StartRow()
        {
            ++currentRow;
        }

        internal BarcodeRow GetCurrentRow()
        {
            return matrix[currentRow];
        }

        public byte[][] GetMatrix()
        {
            return GetScaledMatrix(1, 1);
        }

        public byte[][] GetScaledMatrix(int scale)
        {
            return GetScaledMatrix(scale, scale);
        }

        public byte[][] GetScaledMatrix(int xScale, int yScale)
        {
            byte[][] matrixOut = new byte[height * yScale][];
            for (int i = 0; i < height * yScale; i++)
            {
                matrixOut[i] = new byte[width * xScale];
            }
            int yMax = height * yScale;
            for (int i = 0; i < yMax; i++)
            {
                matrixOut[yMax - i - 1] = matrix[i / yScale].GetScaledRow(xScale);
            }
            return matrixOut;
        }
    }
}
