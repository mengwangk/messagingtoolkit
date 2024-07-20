using System;
using System.Text.RegularExpressions;

using Result = MessagingToolkit.Barcode.Result;

namespace MessagingToolkit.Barcode.Client.Results
{

    /// <summary> 
    /// Implements the "MATMSG" email message entry format.
    /// Supported keys: TO, SUB, BODY
    /// 
    /// Modified: May 18 2012
    /// </summary>
    public sealed class EmailDoCoMoResultParser : AbstractDoCoMoResultParser
    {
        private static Regex ATEXT_ALPHANUMERIC = new Regex("^[a-zA-Z0-9@.!#$%&'*+\\-/=?^_`{|}~]+$");

        public override ParsedResult Parse(Result result)
        {
            String rawText = ResultParser.GetMassagedText(result);
            if (!rawText.StartsWith("MATMSG:"))
            {
                return null;
            }
            String[] rawTo = MatchDoCoMoPrefixedField("TO:", rawText, true);
            if (rawTo == null)
            {
                return null;
            }
            String to = rawTo[0];
            if (!IsBasicallyValidEmailAddress(to))
            {
                return null;
            }
            String subject = MatchSingleDoCoMoPrefixedField("SUB:", rawText, false);
            String body = MatchSingleDoCoMoPrefixedField("BODY:", rawText, false);
            return new EmailAddressParsedResult(to, subject, body, "mailto:" + to);
        }

        /// <summary>
        /// This implements only the most basic checking for an email address's validity -- that it contains
        /// an '@' and contains no characters disallowed by RFC 2822. This is an overly lenient definition of
        /// validity. We want to generally be lenient here since this class is only intended to encapsulate what's
        /// in a barcode, not "judge" it.
        /// </summary>
        ///
        static internal bool IsBasicallyValidEmailAddress(String email)
        {
            return email != null && ATEXT_ALPHANUMERIC.Match(email).Success && email.IndexOf('@') >= 0;
        }
    }
}