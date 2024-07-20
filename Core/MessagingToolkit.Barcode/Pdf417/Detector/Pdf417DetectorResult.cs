using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MessagingToolkit.Barcode.Common;

namespace MessagingToolkit.Barcode.Pdf417.Detector
{
    public sealed class Pdf417DetectorResult
    {

        private readonly BitMatrix bits;
        private readonly IList<ResultPoint[]> points;

        public Pdf417DetectorResult(BitMatrix bits, IList<ResultPoint[]> points)
        {
            this.bits = bits;
            this.points = points;
        }

        public BitMatrix Bits
        {
            get
            {
                return bits;
            }
        }

        public IList<ResultPoint[]> Points
        {
            get
            {
                return points;
            }
        }

    }
}
