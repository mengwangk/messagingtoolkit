using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MessagingToolkit.Barcode
{

    /// <summary>
    /// A wrapper implementation of LuminanceSource which inverts the luminances it returns -- black becomes
    /// white and vice versa, and each value becomes (255-value).
    /// </summary>
    internal class InvertedLuminanceSource : LuminanceSource
    {

        private LuminanceSource source;

        public InvertedLuminanceSource(LuminanceSource source)
            : base(source.Width, source.Height)
        {

            this.source = source;
        }

        public override byte[] GetRow(int y, byte[] row)
        {
            row = source.GetRow(y, row);
            int width = this.Width;
            for (int i = 0; i < width; i++)
            {
                row[i] = (byte)(255 - (row[i] & 0xFF));
            }
            return row;
        }

        public override byte[] Matrix
        {
            get
            {
                byte[] matrix = source.Matrix;
                int length = this.Width * this.Height;
                byte[] invertedMatrix = new byte[length];
                for (int i = 0; i < length; i++)
                {
                    invertedMatrix[i] = (byte)(255 - (matrix[i] & 0xFF));
                }
                return invertedMatrix;
            }
        }

        public override bool CropSupported
        {
            get
            {
                return source.CropSupported;
            }
        }

        public override LuminanceSource Crop(int left, int top, int width, int height)
        {
            return new InvertedLuminanceSource(source.Crop(left, top, width, height));
        }

        public override bool RotateSupported
        {
            get
            {
                return source.RotateSupported;
            }
        }

        /// <summary>
        /// Return original source since invert undoes itself
        /// </summary>
        /// <returns></returns>
        public override LuminanceSource Invert()
        {
            return source;
        }

        public override LuminanceSource RotateCounterClockwise()
        {
            return new InvertedLuminanceSource(source.RotateCounterClockwise());
        }

        public override LuminanceSource RotateCounterClockwise45()
        {
            return new InvertedLuminanceSource(source.RotateCounterClockwise45());
        }
    }
}
