using System;
using System.Collections.Generic;
using System.Text;

namespace MessagingToolkit.Barcode.OneD
{
    class Blank: BarcodeCommon, IBarcode
    {
        
        #region IBarcode Members

        public string EncodedValue
        {
            get { throw new NotImplementedException(); }
        }

        #endregion
    }
}
