using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MessagingToolkit.Barcode.Common;

namespace MessagingToolkit.Barcode.Provider
{

    public sealed class PdfProvider:IOutputProvider<Pdf>
    {
        public Pdf Generate(BitMatrix bitMatrix, BarcodeFormat format, string content)
        {
            throw new NotImplementedException();
        }

        public Pdf Generate(BitMatrix bitMatrix, BarcodeFormat format, string content, Dictionary<EncodeOptions, object> encodingOptions)
        {
            throw new NotImplementedException();
        }
    }

    public sealed class Pdf
    {
    }
}
