using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MessagingToolkit.Barcode.DataMatrix.Encoder
{

    internal interface Encoder
    {

        int EncodingMode { get; }

        void Encode(EncoderContext context);

    }
}
