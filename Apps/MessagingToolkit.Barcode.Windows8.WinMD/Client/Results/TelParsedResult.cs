
using System;
using System.Text;

namespace MessagingToolkit.Barcode.Client.Results
{
    /// <summary>
    /// Modified: May 18 2012
    /// </summary>
    internal sealed class TelParsedResult : ParsedResult
	{
        private readonly String number;
        private readonly String telURI;
        private readonly String title;

        public TelParsedResult(String number, String telURI, String title)
            : base(ParsedResultType.Tel)
        {
            this.number = number;
            this.telURI = telURI;
            this.title = title;
        }

        public String GetNumber()
        {
            return number;
        }

        public String GetTelURI()
        {
            return telURI;
        }

        public String GetTitle()
        {
            return title;
        }

        public override String DisplayResult
        {
            get
            {
                StringBuilder result = new StringBuilder(20);
                MaybeAppend(number, result);
                MaybeAppend(title, result);
                return result.ToString();
            }
        }
	}
}