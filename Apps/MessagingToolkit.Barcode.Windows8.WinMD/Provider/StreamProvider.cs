using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using MessagingToolkit.Barcode.Common;

namespace MessagingToolkit.Barcode.Provider
{
    /// <summary>
    /// Convert a BitMatrix to a stream
    /// </summary>
    public sealed class StreamOutput : IOutputProvider
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
}
