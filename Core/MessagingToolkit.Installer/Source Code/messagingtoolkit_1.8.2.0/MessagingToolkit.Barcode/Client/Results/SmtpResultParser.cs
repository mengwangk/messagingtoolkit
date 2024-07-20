using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MessagingToolkit.Barcode.Client.Results
{
    /// <summary>
    ///   <p>Parses an "smtp:" URI result, whose format is not standardized but appears to be like:
    ///   <c>smtp(:subject(:body))</c>.</p>
    ///   <p>See http://code.google.com/p/zxing/issues/detail?id=536</p>
    ///   
    /// Modified: May 18 2012
    /// </summary>
    public sealed class SMTPResultParser : ResultParser 
    {

        public override ParsedResult Parse(Result result)
        {
            String rawText = GetMassagedText(result);
            if (!(rawText.StartsWith("smtp:") || rawText.StartsWith("SMTP:")))
            {
                return null;
            }
            String emailAddress = rawText.Substring(5);
            String subject = null;
            String body = null;
            int colon = emailAddress.IndexOf(':');
            if (colon >= 0)
            {
                subject = emailAddress.Substring(colon + 1);
                emailAddress = emailAddress.Substring(0, (colon) - (0));
                colon = subject.IndexOf(':');
                if (colon >= 0)
                {
                    body = subject.Substring(colon + 1);
                    subject = subject.Substring(0, (colon) - (0));
                }
            }
            String mailtoURI = "mailto:" + emailAddress;
            return new EmailAddressParsedResult(emailAddress, subject, body, mailtoURI);
        }
    }
}
