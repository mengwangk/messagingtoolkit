using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MessagingToolkit.Barcode.Common;

namespace MessagingToolkit.Barcode.Provider
{

    public sealed class PdfProvider:IOutputProvider
    {
        public IOutput Generate(BitMatrix bitMatrix, BarcodeFormat format, string content)
        {
            throw new NotImplementedException();
        }

        public IOutput Generate(BitMatrix bitMatrix, BarcodeFormat format, string content, IDictionary<EncodeOptions, object> options)
        {
            throw new NotImplementedException();
        }
    }

    public sealed class Pdf
    {
    }
}
