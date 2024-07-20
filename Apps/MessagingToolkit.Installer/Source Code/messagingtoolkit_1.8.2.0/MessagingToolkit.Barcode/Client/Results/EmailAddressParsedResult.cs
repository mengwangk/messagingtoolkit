using System;
using System.Text;

namespace MessagingToolkit.Barcode.Client.Results
{
    /// <summary>
    /// Modified: May 10 2012
    /// </summary>
    public sealed class EmailAddressParsedResult : ParsedResult
    {

        private readonly String emailAddress;
        private readonly String subject;
        private readonly String body;
        private readonly String mailtoURI;

        internal EmailAddressParsedResult(String emailAddress, String subject_1, String body_2, String mailtoURI_3)
            : base(ParsedResultType.EmailAddress)
        {
            this.emailAddress = emailAddress;
            this.subject = subject_1;
            this.body = body_2;
            this.mailtoURI = mailtoURI_3;
        }

        public String GetEmailAddress()
        {
            return emailAddress;
        }

        public String GetSubject()
        {
            return subject;
        }

        public String GetBody()
        {
            return body;
        }

        public String GetMailtoURI()
        {
            return mailtoURI;
        }

        public override String DisplayResult
        {
            get
            {
                StringBuilder result = new StringBuilder(30);
                ParsedResult.MaybeAppend(emailAddress, result);
                ParsedResult.MaybeAppend(subject, result);
                ParsedResult.MaybeAppend(body, result);
                return result.ToString();
            }
        }

    }
}