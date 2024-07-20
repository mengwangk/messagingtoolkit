using System;
using System.Collections.Generic;
using System.Text;

namespace MessagingToolkit.Barcode.OneD
{
    class JAN13 : BarcodeCommon, IBarcode
    {
        public JAN13(string input)
        {
            rawData = input;
        }
        /// <summary>
        /// Encode the raw data using the JAN-13 algorithm.
        /// </summary>
        private string Encode_JAN13()
        {
            if (!rawData.StartsWith("49")) Error("Invalid Country Code for JAN13 (49 required)");
            if (!MessagingToolkit.Barcode.BarcodeEncoder.CheckNumericOnly(rawData))
                Error("Numeric data Only");

            EAN13 ean13 = new EAN13(rawData);
            return ean13.EncodedValue;
        }//Encode_JAN13

        #region IBarcode Members

        public string EncodedValue
        {
            get { return Encode_JAN13(); }
        }

        #endregion
    }
}
