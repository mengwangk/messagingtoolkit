using System;
using System.Text;
using Result = MessagingToolkit.Barcode.Result;

namespace MessagingToolkit.Barcode.Client.Results
{

    /// <summary>
    /// Abstract class representing the result of decoding a barcode, as more than
    /// a String -- as some type of structured data. This might be a subclass which represents
    /// a URL, or an e-mail address. {@link ResultParser#parseResult(Result)} will turn a raw
    /// decoded string into the most appropriate type of structured representation.
    /// 
    /// Modified: May 18 2012
    /// </summary>
    public abstract class ParsedResult
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

        public static void MaybeAppend(String[] val, StringBuilder result)
        {
            if (val != null)
            {
                /* foreach */
                foreach (String s in val)
                {
                    if (s != null && s.Length > 0)
                    {
                        if (result.Length > 0)
                        {
                            result.Append('\n');
                        }
                        result.Append(s);
                    }
                }
            }
        }
    }
}