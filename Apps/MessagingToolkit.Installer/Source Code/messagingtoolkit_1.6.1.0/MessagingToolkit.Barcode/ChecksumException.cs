using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MessagingToolkit.Barcode
{
    /// <summary>
    /// Thrown when a barcode was successfully detected and decoded, but
    /// was not returned because its checksum feature failed.
    /// </summary>
#if !SILVERLIGHT
    [Serializable]
#endif
    public sealed class ChecksumException : BarcodeDecoderException
    {

        public static readonly ChecksumException Instance = new ChecksumException();

        private ChecksumException()
        {
            // do nothing
        }
    }
}
