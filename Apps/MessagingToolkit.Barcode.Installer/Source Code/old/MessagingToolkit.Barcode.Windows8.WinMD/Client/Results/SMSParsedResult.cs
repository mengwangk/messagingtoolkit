using System;
using System.Text;

namespace MessagingToolkit.Barcode.Client.Results
{
    /// <summary>
    /// Modified: May 18 2012
    /// </summary>
    internal sealed class SMSParsedResult : ParsedResult
    {
        private readonly String[] numbers;
        private readonly String[] vias;
        private readonly String subject;
        private readonly String body;

        public SMSParsedResult(String number, String via, String subject, String body)
            : base(ParsedResultType.Sms)
        {
            this.numbers = new String[] { number };
            this.vias = new String[] { via };
            this.subject = subject;
            this.body = body;
        }

        public SMSParsedResult(String[] numbers, String[] vias, String subject, String body)
            : base(ParsedResultType.Sms)
        {
            this.numbers = numbers;
            this.vias = vias;
            this.subject = subject;
            this.body = body;
        }

        public String GetSMSURI()
        {
            StringBuilder result = new StringBuilder();
            result.Append("sms:");
            bool first = true;
            for (int i = 0; i < numbers.Length; i++)
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                    result.Append(',');
                }
                result.Append(numbers[i]);
                if (vias != null && vias[i] != null)
                {
                    result.Append(";via=");
                    result.Append(vias[i]);
                }
            }
            bool hasBody = body != null;
            bool hasSubject = subject != null;
            if (hasBody || hasSubject)
            {
                result.Append('?');
                if (hasBody)
                {
                    result.Append("body=");
                    result.Append(body);
                }
                if (hasSubject)
                {
                    if (hasBody)
                    {
                        result.Append('&');
                    }
                    result.Append("subject=");
                    result.Append(subject);
                }
            }
            return result.ToString();
        }

        public String[] GetNumbers()
        {
            return numbers;
        }

        public String[] GetVias()
        {
            return vias;
        }

        public String GetSubject()
        {
            return subject;
        }

        public String GetBody()
        {
            return body;
        }

        public override String DisplayResult
        {
            get
            {
                StringBuilder result = new StringBuilder(100);
                MaybeAppend(numbers, result);
                MaybeAppend(subject, result);
                MaybeAppend(body, result);
                return result.ToString();
            }
        }
	
    }
}