using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MessagingToolkit.Barcode.Client.Results
{
    /// <summary>
    ///   <p>Parses an "smsto:" URI result, whose format is not standardized but appears to be like:
    ///   <c>smsto:number(:body)</c>.</p>
    ///   <p>This actually also parses URIs starting with "smsto:", "mmsto:", "SMSTO:", and
    /// "MMSTO:", and treats them all the same way, and effectively converts them to an "sms:" URI
    /// for purposes of forwarding to the platform.</p>
    /// 
    /// Modified: May 18 2012
    /// </summary>
    public sealed class SmsToMmsToResultParser : ResultParser
    {
        public override ParsedResult Parse(Result result)
        {
            String rawText = GetMassagedText(result);
            if (!(rawText.StartsWith("smsto:") || rawText.StartsWith("SMSTO:") || rawText.StartsWith("mmsto:") || rawText.StartsWith("MMSTO:")))
            {
                return null;
            }
            // Thanks to dominik.wild for suggesting this enhancement to support
            // smsto:number:body URIs
            String number = rawText.Substring(6);
            String body = null;
            int bodyStart = number.IndexOf(':');
            if (bodyStart >= 0)
            {
                body = number.Substring(bodyStart + 1);
                number = number.Substring(0, (bodyStart) - (0));
            }
            return new SMSParsedResult(number, null, null, body);
        }

    }
}
