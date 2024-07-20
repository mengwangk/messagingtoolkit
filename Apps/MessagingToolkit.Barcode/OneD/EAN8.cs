using System;
using System.Collections.Generic;
using System.Text;

namespace MessagingToolkit.Barcode.OneD
{
    class EAN8 : BarcodeCommon, IBarcode
    {
        private string[] EAN_CodeA = { "0001101", "0011001", "0010011", "0111101", "0100011", "0110001", "0101111", "0111011", "0110111", "0001011" };
        private string[] EAN_CodeC = { "1110010", "1100110", "1101100", "1000010", "1011100", "1001110", "1010000", "1000100", "1001000", "1110100" };

        public EAN8(string input)
        {
            rawData = input;

            CheckDigit();
        }
        /// <summary>
        /// Encode the raw data using the EAN-8 algorithm.
        /// </summary>
        private string Encode_EAN8()
        {
            //check length
            if (rawData.Length != 8 && rawData.Length != 7) Error("Invalid data length. (7 or 8 numbers only)");

            //check numeric only
            if (!MessagingToolkit.Barcode.BarcodeEncoder.CheckNumericOnly(rawData)) Error("Numeric only.");

            //encode the data
            string result = "101";

            //first half (Encoded using left hand / odd parity)
            for (int i = 0; i < rawData.Length / 2; i++)
            {
                result += EAN_CodeA[Int32.Parse(rawData[i].ToString())];
            }//for

            //center guard bars
            result += "01010";

            //second half (Encoded using right hand / even parity)
            for (int i = rawData.Length / 2; i < rawData.Length; i++)
            {
                result += EAN_CodeC[Int32.Parse(rawData[i].ToString())];
            }//for

            result += "101";

            return result;
        }//Encode_EAN8

        private void CheckDigit()
        {
            //calculate the checksum digit if necessary
            if (rawData.Length == 7)
            {
                //calculate the checksum digit
                int even = 0;
                int odd = 0;

                //odd
                for (int i = 0; i <= 6; i += 2)
                {
                    odd += Int32.Parse(rawData.Substring(i, 1)) * 3;
                }//for

                //even
                for (int i = 1; i <= 5; i += 2)
                {
                    even += Int32.Parse(rawData.Substring(i, 1));
                }//for

                int total = even + odd;
                int checksum = total % 10;
                checksum = 10 - checksum;
                if (checksum == 10)
                    checksum = 0;

                //add the checksum to the end of the 
                rawData += checksum.ToString();
            }//if
        }

        #region IBarcode Members

        public string EncodedValue
        {
            get { return Encode_EAN8(); }
        }

        #endregion
    }
}
