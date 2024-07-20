
namespace MessagingToolkit.Barcode.Pdf417.Encoder
{
    /// <summary>
    /// Holds all of the information for a barcode in a format where it can be easily accessable
    /// </summary>
    internal sealed class BarcodeMatrix
    {
        private readonly BarcodeRow[] matrix;
        private int currentRow;
        private readonly int height;
        private readonly int width;


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

        internal byte[][] GetMatrix()
        {
            return GetScaledMatrix(1, 1);
        }

        internal byte[][] GetScaledMatrix(int Scale)
        {
            return GetScaledMatrix(Scale, Scale);
        }

        internal byte[][] GetScaledMatrix(int xScale, int yScale)
        {
            //byte[][] matrixOut = (byte[][])ILOG.J2CsMapping.Collections.Arrays.CreateJaggedArray(typeof(byte), height * yScale, width * xScale);
            byte[][] matrixOut = new byte[height * yScale][];
            for (int i = 0; i < height * yScale; i++)
            {
                matrixOut[i] = new byte[width * xScale];
            }
            int yMax = height * yScale;
            for (int ii = 0; ii < yMax; ii++)
            {
                matrixOut[yMax - ii - 1] = matrix[ii / yScale].GetScaledRow(xScale);
            }
            return matrixOut;
        }
    }
}
