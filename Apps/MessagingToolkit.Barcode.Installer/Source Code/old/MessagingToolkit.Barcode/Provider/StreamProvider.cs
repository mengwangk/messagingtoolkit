using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using Windows.Foundation;
using Windows.Storage.Streams;

using MessagingToolkit.Barcode.Common;

namespace MessagingToolkit.Barcode.Provider
{
    /// <summary>
    /// Convert a BitMatrix to a stream
    /// </summary>
    public sealed class StreamOutput //: IOutputProvider<IRandomAccessStream>
    {
       
        public IAsyncOperation<IRandomAccessStream> Generate(BitMatrix bitMatrix, BarcodeFormat format, string content)
        {
            return null;
        }

        public IAsyncOperation<IRandomAccessStream> Generate(BitMatrix bitMatrix, BarcodeFormat format, string content, Dictionary<EncodeOptions, object> encodingOptions)
        {
            return null;
        }
    }
}
