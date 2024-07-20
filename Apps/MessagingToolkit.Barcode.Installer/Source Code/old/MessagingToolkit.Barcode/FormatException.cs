using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MessagingToolkit.Barcode
{
    /// <summary>
    /// Thrown when a barcode was successfully detected, but some aspect of
    /// the content did not conform to the barcode's format rules. This could have
    /// been due to a mis-detection.
    /// </summary>
    ///
#if !SILVERLIGHT && !NETFX_CORE
	[Serializable]
#endif
    public sealed class FormatException : BarcodeDecoderException
    {

        public static readonly FormatException Instance = new FormatException();

        private FormatException()
        {
            // do nothing
        }
    }
}
