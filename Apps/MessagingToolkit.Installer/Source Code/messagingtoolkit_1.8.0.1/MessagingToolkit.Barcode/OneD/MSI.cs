using System;
using System.Collections.Generic;
using System.Text;

namespace MessagingToolkit.Barcode.OneD
{
    class MSI : BarcodeCommon, IBarcode
    {
        private string[] MSI_Code = { "100100100100", "100100100110", "100100110100", "100100110110", "100110100100", "100110100110", "100110110100", "100110110110", "110100100100", "110100100110" };
        private BarcodeFormat Encoded_Type = BarcodeFormat.Unknown;

        public MSI(string input, BarcodeFormat EncodedType)
        {
            Encoded_Type = EncodedType;
            rawData = input;
        }//MSI

        /// <summary>
        /// Encode the raw data using the MSI algorithm.
        /// </summary>
        private string Encode_MSI()
        {
            //check for non-numeric chars
            if (!MessagingToolkit.Barcode.BarcodeEncoder.CheckNumericOnly(rawData))
                Error("Numeric data Only");

            string PreEncoded = rawData;

            //get checksum
            if (Encoded_Type == BarcodeFormat.MSIMod10 || Encoded_Type == BarcodeFormat.MSI2Mod10)
            {
                string odds = "";
                string evens = "";
                for (int i = PreEncoded.Length - 1; i >= 0; i -= 2)
                {
                    odds = PreEncoded[i].ToString() + odds;
                    if (i - 1 >= 0)
                        evens = PreEncoded[i - 1].ToString() + evens;
                }//for

                //multiply odds by 2
                odds = Convert.ToString((Int32.Parse(odds) * 2));

                int evensum = 0;
                int oddsum = 0;
                foreach (char c in evens)
                    evensum += Int32.Parse(c.ToString());
                foreach (char c in odds)
                    oddsum += Int32.Parse(c.ToString());
                int checksum = 10 - ((oddsum + evensum) % 10);
                PreEncoded += checksum.ToString();
            }//if

            if (Encoded_Type == BarcodeFormat.MSIMod11 || Encoded_Type == BarcodeFormat.MSIMod11Mod10)
            {
                int sum = 0;
                int weight = 2;
                for (int i = PreEncoded.Length - 1; i >= 0; i--)
                {
                    if (weight > 7) weight = 2;
                    sum += Int32.Parse(PreEncoded[i].ToString()) * weight++;
                }//foreach
                int checksum = 11 - (sum % 11);

                PreEncoded += checksum.ToString();
            }//else

            if (Encoded_Type == BarcodeFormat.MSI2Mod10 || Encoded_Type == BarcodeFormat.MSIMod11Mod10)
            {
                //get second check digit if 2 mod 10 was selected or Mod11/Mod10
                string odds = "";
                string evens = "";
                for (int i = PreEncoded.Length - 1; i >= 0; i -= 2)
                {
                    odds = PreEncoded[i].ToString() + odds;
                    if (i - 1 >= 0)
                        evens = PreEncoded[i - 1].ToString() + evens;
                }//for

                //multiply odds by 2
                odds = Convert.ToString((Int32.Parse(odds) * 2));

                int evensum = 0;
                int oddsum = 0;
                foreach (char c in evens)
                    evensum += Int32.Parse(c.ToString());
                foreach (char c in odds)
                    oddsum += Int32.Parse(c.ToString());
                int checksum = 10 - ((oddsum + evensum) % 10);
                PreEncoded += checksum.ToString();
            }//if

            string result = "110";
            foreach (char c in PreEncoded)
            {
                result += MSI_Code[Int32.Parse(c.ToString())];
            }//foreach

            //add stop character
            result += "1001";

            return result;
        }//Encode_MSI

        #region IBarcode Members

        public string EncodedValue
        {
            get { return Encode_MSI(); }
        }

        #endregion
    }//class
}//namepsace
