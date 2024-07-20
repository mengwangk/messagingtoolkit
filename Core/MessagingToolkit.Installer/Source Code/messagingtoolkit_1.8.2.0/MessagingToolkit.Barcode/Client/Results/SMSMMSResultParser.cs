using System;
using System.Collections.Generic;
using Result = MessagingToolkit.Barcode.Result;

namespace MessagingToolkit.Barcode.Client.Results
{

    /// <summary>
    ///   <p>Parses an "sms:" URI result, which specifies a number to SMS.
    /// See <a href="http://tools.ietf.org/html/rfc5724"> RFC 5724</a> on this.</p>
    /// 	<p>This class supports "via" syntax for numbers, which is not part of the spec.
    /// For example "+12125551212;via=+12124440101" may appear as a number.
    /// It also supports a "subject" query parameter, which is not mentioned in the spec.
    /// These are included since they were mentioned in earlier IETF drafts and might be
    /// used.</p>
    /// 	<p>This actually also parses URIs starting with "mms:" and treats them all the same way,
    /// and effectively converts them to an "sms:" URI for purposes of forwarding to the platform.</p>
    /// Modified: May 18 2012
    /// </summary>
    public sealed class SMSMMSResultParser : ResultParser
    {
        public override ParsedResult Parse(Result result)
        {
            String rawText = GetMassagedText(result);
            if (!(rawText.StartsWith("sms:") || rawText.StartsWith("SMS:") || rawText.StartsWith("mms:") || rawText.StartsWith("MMS:")))
            {
                return null;
            }

            // Check up front if this is a URI syntax string with query arguments
            IDictionary<String, String> nameValuePairs = ParseNameValuePairs(rawText);
            String subject = null;
            String body = null;
            bool querySyntax = false;
            if (nameValuePairs != null && !(nameValuePairs.Count == 0))
            {
                subject = nameValuePairs["subject"];
                body = nameValuePairs["body"];
                querySyntax = true;
            }

            // Drop sms, query portion
            int queryStart = rawText.IndexOf('?', 4);
            String smsURIWithoutQuery;
            // If it's not query syntax, the question mark is part of the subject or message
            if (queryStart < 0 || !querySyntax)
            {
                smsURIWithoutQuery = rawText.Substring(4);
            }
            else
            {
                smsURIWithoutQuery = rawText.Substring(4, (queryStart) - (4));
            }

            int lastComma = -1;
            int comma;
            List<String> numbers = new List<String>(1);
            List<String> vias = new List<String>(1);
            while ((comma = smsURIWithoutQuery.IndexOf(',', lastComma + 1)) > lastComma)
            {
                String numberPart = smsURIWithoutQuery.Substring(lastComma + 1, (comma) - (lastComma + 1));
                AddNumberVia(numbers, vias, numberPart);
                lastComma = comma;
            }
            AddNumberVia(numbers, vias, smsURIWithoutQuery.Substring(lastComma + 1));

            return new SMSParsedResult(ToStringArray(numbers), ToStringArray(vias), subject, body);
        }

        private static void AddNumberVia(ICollection<String> numbers, ICollection<String> vias, String numberPart)
        {
            int numberEnd = numberPart.IndexOf(';');
            if (numberEnd < 0)
            {
                numbers.Add(numberPart);
                vias.Add(null);
            }
            else
            {
                numbers.Add(numberPart.Substring(0, numberEnd));
                String maybeVia = numberPart.Substring(numberEnd + 1);
                String via;
                if (maybeVia.StartsWith("via="))
                {
                    via = maybeVia.Substring(4);
                }
                else
                {
                    via = null;
                }
                vias.Add(via);
            }
        }
	
    }
}