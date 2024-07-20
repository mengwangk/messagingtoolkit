using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MessagingToolkit.Barcode.QRCode.Decoder
{

    /// <summary>
    /// Meta-data container for QR Code decoding. Instances of this class may be used to convey information back to the
    /// decoding caller. Callers are expected to process this.
    /// </summary>
    public sealed class QRCodeDecoderMetaData
    {
        private readonly bool mirrored;

        internal QRCodeDecoderMetaData(bool mirrored)
        {
            this.mirrored = mirrored;
        }

        /// <returns> true if the QR Code was mirrored.  </returns>
        public bool Mirrored
        {
            get
            {
                return mirrored;
            }
        }

        /// <summary>
        /// Apply the result points' order correction due to mirroring.
        /// </summary>
        /// <param name="points"> Array of points to apply mirror correction to. </param>
        public void ApplyMirroredCorrection(ResultPoint[] points)
        {
            if (!mirrored || points == null || points.Length < 3)
            {
                return;
            }
            ResultPoint bottomLeft = points[0];
            points[0] = points[2];
            points[2] = bottomLeft;
            // No need to 'fix' top-left and alignment pattern.
        }

    }
}
