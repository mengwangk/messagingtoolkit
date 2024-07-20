
using System;
using System.Collections.Generic;
using System.Collections;
using Result = MessagingToolkit.Barcode.Result;


namespace MessagingToolkit.Barcode.Client.Results
{
	
	/// <summary> 
    /// Implements KDDI AU's address book format. See
	/// <a href="http://www.au.kddi.com/ezfactory/tec/two_dimensions/index.html">
	/// http://www.au.kddi.com/ezfactory/tec/two_dimensions/index.html</a>.
    /// 
    /// Modified: May 10 2012
	/// </summary>	
	public sealed class AddressBookAUResultParser:ResultParser
	{

        public override ParsedResult Parse(Result result)
        {
            String rawText = GetMassagedText(result);
            // MEMORY is mandatory; seems like a decent indicator, as does end-of-record separator CR/LF
            if (!rawText.Contains("MEMORY") || !rawText.Contains("\r\n"))
            {
                return null;
            }

            // NAME1 and NAME2 have specific uses, namely written name and pronunciation, respectively.
            // Therefore we treat them specially instead of as an array of names.
            String name = MatchSinglePrefixedField("NAME1:", rawText, '\r', true);
            String pronunciation = MatchSinglePrefixedField("NAME2:", rawText, '\r', true);

            String[] phoneNumbers = MatchMultipleValuePrefix("TEL", 3, rawText, true);
            String[] emails = MatchMultipleValuePrefix("MAIL", 3, rawText, true);
            String note = MatchSinglePrefixedField("MEMORY:", rawText, '\r', false);
            String address = MatchSinglePrefixedField("ADD:", rawText, '\r', true);
            String[] addresses = (address == null) ? null : new String[] { address };
            return new AddressBookParsedResult(MaybeWrap(name),null,  pronunciation, phoneNumbers, null, emails, null, null, note, addresses, null, null, null, null, null, null);
        }

        private static String[] MatchMultipleValuePrefix(String prefix, int max, String rawText, bool trim)
        {
            List<String> values = null;
            for (int i = 1; i <= max; i++)
            {
                String val = MatchSinglePrefixedField(prefix + i + ':', rawText, '\r', trim);
                if (val == null)
                {
                    break;
                }
                if (values == null)
                {
                    values = new List<String>(max); // lazy init
                }
                values.Add(val);
            }
            if (values == null)
            {
                return null;
            }
            return ToStringArray(values);
        }
	}
}