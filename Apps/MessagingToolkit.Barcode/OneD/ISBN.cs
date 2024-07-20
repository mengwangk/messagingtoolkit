using System;
using System.Collections.Generic;
using System.Text;

namespace MessagingToolkit.Barcode.OneD
{
    class ISBN : BarcodeCommon, IBarcode
    {
        public ISBN(string input)
        {
            rawData = input;
        }
        /// <summary>
        /// Encode the raw data using the Bookland/ISBN algorithm.
        /// </summary>
        private string Encode_ISBN_Bookland()
        {
            if (!MessagingToolkit.Barcode.BarcodeEncoder.CheckNumericOnly(rawData))
                Error("Numeric data Only");

            string type = "UNKNOWN";
            if (rawData.Length == 10 || rawData.Length == 9)
            {
                if (rawData.Length == 10) rawData = rawData.Remove(9, 1);
                rawData = "978" + rawData;
                type = "ISBN";
            }//if
            else if (rawData.Length == 12 && rawData.StartsWith("978"))
            {
                type = "Bookland-NOCHECKDIGIT";
            }//else if
            else if (rawData.Length == 13 && rawData.StartsWith("978"))
            {
                type = "Bookland-CHECKDIGIT";
                rawData = rawData.Remove(12, 1);
            }//else if

            //check to see if its an unknown type
            if (type == "UNKNOWN") Error("Invalid input.  Must start with 978 and the length must be 9, 10, 12, 13 characters.");

            EAN13 ean13 = new EAN13(rawData);
            return ean13.EncodedValue;
        }//Encode_ISBN_Bookland

        #region IBarcode Members

        public string EncodedValue
        {
            get { return Encode_ISBN_Bookland(); }
        }

        #endregion
    }
}
