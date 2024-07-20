using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MessagingToolkit.Barcode
{
    /// <summary>
    /// Thrown when a barcode was not found in the image. It might have been
    /// partially detected but could not be confirmed.
    /// </summary>
#if !SILVERLIGHT 
    [Serializable]
#endif
    public sealed class NotFoundException : BarcodeDecoderException
    {
        public static readonly NotFoundException Instance = new NotFoundException();

        private NotFoundException()
        {
            // do nothing
        }

       
    }
}
