using System;

namespace MessagingToolkit.Barcode.Client.Results
{
	
	/// <summary> 
    /// A simple result type encapsulating a string that has no further
	/// interpretation. 
    /// 
    /// Modified: May 18 2012
	/// </summary>
    internal sealed class TextParsedResult : ParsedResult
	{
        private readonly String text;
        private readonly String language;

        public TextParsedResult(String text, String language)
            : base(ParsedResultType.Text)
        {
            this.text = text;
            this.language = language;
        }

        public String GetText()
        {
            return text;
        }

        public String GetLanguage()
        {
            return language;
        }

        public override String DisplayResult
        {
            get
            {
                return text;
            }
        }
	}
}