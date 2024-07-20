using System;
using System.Collections.Generic;
using System.Text;

namespace MessagingToolkit.Barcode
{
    abstract class BarcodeCommon
    {
        protected string rawData = "";
        protected List<string> _errors = new List<string>();

        public string RawData
        {
            get { return this.rawData; }
        }

        public List<string> Errors
        {
            get { return this._errors; }
        }

        public void Error(string ErrorMessage)
        {
            this._errors.Add(ErrorMessage);
            throw new Exception(ErrorMessage);
        }

        public bool IsNumeric(string input)
        {
            try
            {
                Int32 i32temp = new Int32();
                if (!Int32.TryParse(input, out i32temp))
                {
                    //parse didnt work so check each char because it may just be too long.
                    foreach (char c in input)
                    {
                        if (!char.IsDigit(c))
                            return false;
                    }//foreach
                }//if
                return true;
            }//try
            catch
            {
                return false;
            }//catch
        }//IsNumeric
    }//BarcodeVariables abstract class
}//namespace
