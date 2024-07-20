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
    public sealed class StreamOutput : IOutputProvider<Stream>
    {
       
        public Stream Generate(BitMatrix bitMatrix, BarcodeFormat format, string content)
        {
            throw new NotImplementedException();
        }

        public Stream Generate(BitMatrix bitMatrix, BarcodeFormat format, string content, Dictionary<EncodeOptions, object> encodingOptions)
        {
            throw new NotImplementedException();
        }
    }
}
