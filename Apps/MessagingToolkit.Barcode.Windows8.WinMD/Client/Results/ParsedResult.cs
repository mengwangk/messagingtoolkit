using System;
using System.Text;
using Result = MessagingToolkit.Barcode.Result;

namespace MessagingToolkit.Barcode.Client.Results
{

    /// <summary>
    /// Abstract class representing the result of decoding a barcode, as more than
    /// a String -- as some type of structured data. This might be a subclass which represents
    /// a URL, or an e-mail address.  ResultParser.ParseResult will turn a raw
    /// decoded string into the most appropriate type of structured representation.
    /// 
    /// Modified: May 18 2012
    /// </summary>
    internal abstract class ParsedResult
    {
        private readonly ParsedResultType type;

        protected internal ParsedResult(ParsedResultType type)
        {
            this.type = type;
        }

        public virtual ParsedResultType Type
        {
            get
            {
                return this.type;
            }
        }

        public abstract String DisplayResult
        {
            get;
        }

        public sealed override String ToString()
        {
            return DisplayResult;
        }

        public static void MaybeAppend(String val, StringBuilder result)
        {
            if (val != null && val.Length > 0)
            {
                // Don't add a newline before the first value
                if (result.Length > 0)
                {
                    result.Append('\n');
                }
                result.Append(val);
            }
        }

        public static void MaybeAppend(String[] values, StringBuilder result)
        {
            if (values != null)
            {
                foreach (String value in values)
                {
                    MaybeAppend(value, result);
                }
            }
        }
    }
}
