using System;
using System.Collections.Generic;
using Result = MessagingToolkit.Barcode.Result;

namespace MessagingToolkit.Barcode.Client.Results
{

    /// <summary>
    /// Represents a result that encodes an e-mail address, either as a plain address
    /// like "joe@example.org" or a mailto: URL like "mailto:joe@example.org".
    /// 
    /// Modified: May 10 2012
    /// </summary>
    internal sealed class EmailAddressResultParser : ResultParser
    {

        public override ParsedResult Parse(Result result)
        {
            String rawText = ResultParser.GetMassagedText(result);
            String emailAddress;
            if (rawText.StartsWith("mailto:") || rawText.StartsWith("MAILTO:"))
            {
                // If it starts with mailto:, assume it is definitely trying to be an email address
                emailAddress = rawText.Substring(7);
                int queryStart = emailAddress.IndexOf('?');
                if (queryStart >= 0)
                {
                    emailAddress = emailAddress.Substring(0, (queryStart) - (0));
                }
                emailAddress = UrlDecode(emailAddress);
                IDictionary<String, String> nameValues = ResultParser.ParseNameValuePairs(rawText);
                String subject = null;
                String body = null;
                if (nameValues != null)
                {
                    if (emailAddress.Length == 0)
                    {
                        emailAddress = nameValues["to"];
                    }
                    subject = nameValues["subject"];
                    body = nameValues["body"];
                }
                return new EmailAddressParsedResult(emailAddress, subject, body, rawText);
            }
            else
            {
                if (!EmailDoCoMoResultParser.IsBasicallyValidEmailAddress(rawText))
                {
                    return null;
                }
                emailAddress = rawText;
                return new EmailAddressParsedResult(emailAddress, null, null, "mailto:" + emailAddress);
            }
        }

    }
}