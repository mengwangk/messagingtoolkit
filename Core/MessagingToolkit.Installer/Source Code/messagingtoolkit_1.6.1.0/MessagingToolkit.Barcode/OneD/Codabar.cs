using System;
using System.Collections.Generic;
using System.Text;

namespace MessagingToolkit.Barcode.OneD
{
    class Codabar: BarcodeCommon, IBarcode
    {
        private Dictionary<char,string> CodabarCode = new Dictionary<char,string>(); //is initialized by init_Codabar()
        
        public Codabar(string input)
        {
            rawData = input;
        }//Codabar

        /// <summary>
        /// Encode the raw data using the Codabar algorithm.
        /// </summary>
        private string EncodeCodabar()
        {
            if (rawData.Length < 2) Error("Data Format invalid. (Invalid length)");

            //check first char to make sure its a start/stop char
            switch (rawData[0].ToString().ToUpper().Trim())
            {
                case "A": break;
                case "B": break;
                case "C": break;
                case "D": break;
                default: Error("Data Format invalid. (Invalid Start character)");
                    break;
            }//switch

            //check the ending char to make sure its a start/stop char
            switch (rawData[rawData.Trim().Length - 1].ToString().ToUpper().Trim())
            {
                case "A": break;
                case "B": break;
                case "C": break;
                case "D": break;
                default: Error("Data Format invalid. (Invalid STOP character)");
                    break;
            }//switch

            string temp = rawData.Trim().Substring(1, RawData.Trim().Length - 2);
            if (!IsNumeric(temp))
                Error("Data contains non-numeric characters.");

            string result = "";

            //populate the dictionary to begin the process
            this.init_Codabar();

            foreach (char c in rawData)
            {
                result += CodabarCode[c].ToString();
                result += "0"; //inter-character space
            }//foreach

            //remove the extra 0 at the end of the result
            result = result.Remove(result.Length - 1);

            //clears the dictionary so it no longer takes up memory
            this.CodabarCode.Clear();

            //change the Raw_Data to strip out the start stop chars for label purposes
            rawData = rawData.Trim().Substring(1, RawData.Trim().Length - 2);

            return result;
        }//Encode_Codabar
        private void init_Codabar()
        {
            CodabarCode.Clear();
            CodabarCode.Add('0', "101010011");//"101001101101");
            CodabarCode.Add('1', "101011001");//"110100101011");
            CodabarCode.Add('2', "101001011");//"101100101011");
            CodabarCode.Add('3', "110010101");//"110110010101");
            CodabarCode.Add('4', "101101001");//"101001101011");
            CodabarCode.Add('5', "110101001");//"110100110101");
            CodabarCode.Add('6', "100101011");//"101100110101");
            CodabarCode.Add('7', "100101101");//"101001011011");
            CodabarCode.Add('8', "100110101");//"110100101101");
            CodabarCode.Add('9', "110100101");//"101100101101");
            CodabarCode.Add('-', "101001101");//"110101001011");
            CodabarCode.Add('$', "101100101");//"101101001011");
            CodabarCode.Add(':', "1101011011");//"110110100101");
            CodabarCode.Add('/', "1101101011");//"101011001011");
            CodabarCode.Add('.', "1101101101");//"110101100101");
            CodabarCode.Add('+', "101100110011");//"101101100101");
            CodabarCode.Add('A', "1011001001");//"110110100101");
            CodabarCode.Add('B', "1010010011");//"101011001011");
            CodabarCode.Add('C', "1001001011");//"110101100101");
            CodabarCode.Add('D', "1010011001");//"101101100101");
            CodabarCode.Add('a', "1011001001");//"110110100101");
            CodabarCode.Add('b', "1010010011");//"101011001011");
            CodabarCode.Add('c', "1001001011");//"110101100101");
            CodabarCode.Add('d', "1010011001");//"101101100101");
        }//init_Codeabar

        #region IBarcode Members

        public string EncodedValue
        {
            get { return EncodeCodabar(); }
        }

        #endregion

    }//class
}//namespace
