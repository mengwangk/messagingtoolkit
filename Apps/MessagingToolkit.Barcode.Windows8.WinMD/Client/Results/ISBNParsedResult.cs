using System;

namespace MessagingToolkit.Barcode.Client.Results
{

    /// <summary>
    /// Modified: May 18 2010
    /// </summary>
    internal sealed class ISBNParsedResult : ParsedResult
    {

        private readonly String isbn;

        internal ISBNParsedResult(String isbn)
            : base(ParsedResultType.Isbn)
        {
            this.isbn = isbn;
        }

        public String ISBN
        {
            get
            {
                return isbn;
            }
        }

        public override String DisplayResult
        {
            get
            {
                return isbn;
            }
        }
    }
}