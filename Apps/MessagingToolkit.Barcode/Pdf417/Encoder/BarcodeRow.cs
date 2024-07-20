
namespace MessagingToolkit.Barcode.Pdf417.Encoder
{

    /// <summary>
    /// 
    /// </summary>
    internal sealed class BarcodeRow
    {

        private readonly byte[] row;
        //A tacker for position in the bar
        private int currentLocation;

        /// <summary>
        /// Creates a Barcode row of the width
        /// </summary>
        /// <param name="width">The width.</param>
        internal BarcodeRow(int width)
        {
            this.row = new byte[width];
            currentLocation = 0;
        }

        /// <summary>
        /// Sets a specific location in the bar
        /// </summary>
        ///
        /// <param name="x">The location in the bar</param>
        /// <param name="value">Black if true, white if false;</param>
        internal void Set(int x, byte value)
        {
            row[x] = value;
        }

        /// <summary>
        /// Sets a specific location in the bar
        /// </summary>
        ///
        /// <param name="x">The location in the bar</param>
        /// <param name="black">Black if true, white if false;</param>
        internal void Set(int x, bool black)
        {
            row[x] = (byte)((black) ? 1 : 0);
        }


        /// <param name="black">A boolean which is true if the bar black false if it is white</param>
        /// <param name="width">How many spots wide the bar is.</param>
        internal void AddBar(bool black, int width)
        {
            for (int ii = 0; ii < width; ii++)
            {
                Set(currentLocation++, black);
            }
        }

        internal byte[] GetRow()
        {
            return row;
        }

        /// <summary>
        /// This function scales the row
        /// </summary>
        ///
        /// <param name="scale">How much you want the image to be scaled, must be greater than or equal to 1.</param>
        /// <returns>the scaled row</returns>
        internal byte[] GetScaledRow(int scale)
        {
            byte[] output = new byte[row.Length * scale];
            for (int i = 0; i < output.Length; i++) 
            {
                output[i] = row[i / scale];
            }
            return output;
        }
    }
}
