using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MessagingToolkit.Barcode
{
    public class BaseLuminanceSource : LuminanceSource
    {
        protected byte[] luminances;

        protected BaseLuminanceSource(int width, int height)
            : base(width, height)
        {
            luminances = new byte[width * height];
        }

        protected BaseLuminanceSource(byte[] luminanceArray, int width, int height)
            : base(width, height)
        {
            luminances = new byte[width * height];
            Buffer.BlockCopy(luminanceArray, 0, luminances, 0, width * height);
        }

        override public byte[] GetRow(int y, byte[] row)
        {
            int width = Width;
            if (row == null || row.Length < width)
            {
                row = new byte[width];
            }
            for (int i = 0; i < width; i++)
                row[i] = (byte)(luminances[y * width + i] - 128);
            return row;
        }

        public override byte[] Matrix
        {
            get { return luminances; }
        }

        public override LuminanceSource RotateCounterClockwise()
        {
            var rotatedLuminances = new byte[Width * Height];
            var newWidth = Height;
            var newHeight = Width;
            for (var yold = 0; yold < Height; yold++)
            {
                for (var xold = 0; xold < Width; xold++)
                {
                    var ynew = xold;
                    var xnew = newWidth - yold - 1;
                    rotatedLuminances[ynew * newWidth + xnew] = luminances[yold * Width + xold];
                }
            }
            luminances = rotatedLuminances;
            Height = newHeight;
            Width = newWidth;
            return this;
        }

        public override bool RotateSupported
        {
            get
            {
                return true;
            }
        }
    }
}
