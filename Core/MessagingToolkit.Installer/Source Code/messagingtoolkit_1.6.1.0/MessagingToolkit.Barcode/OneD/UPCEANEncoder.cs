using MessagingToolkit.Barcode.Common;

using System;
using System.Collections.Generic;

namespace MessagingToolkit.Barcode.OneD
{
    /// <summary>
    /// <p>Encapsulates functionality and implementation that is common to UPC and EAN families
    /// of one-dimensional barcodes.</p>
    /// 
    /// Modified: April 30 2012
    /// </summary>
    public abstract class UPCEANEncoder : OneDEncoder
    {

        public int DefaultMargin
        {
            get
            {
                // Use a different default more appropriate for UPC/EAN
                return UPCEANDecoder.StartEndPattern.Length;
            }

        }
    }
}
