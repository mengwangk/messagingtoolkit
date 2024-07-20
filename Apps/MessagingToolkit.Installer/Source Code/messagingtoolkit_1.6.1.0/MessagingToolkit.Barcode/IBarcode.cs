using System;
using System.Collections.Generic;
using System.Text;

namespace MessagingToolkit.Barcode
{
    interface IBarcode
    {
        string EncodedValue
        {
            get;
        }

        string RawData
        {
            get;
        }

        List<string> Errors
        {
            get;
        }

    }
}
