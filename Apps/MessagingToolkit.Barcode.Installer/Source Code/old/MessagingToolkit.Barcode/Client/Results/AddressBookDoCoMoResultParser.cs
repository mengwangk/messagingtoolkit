using System;
using Result = MessagingToolkit.Barcode.Result;

namespace MessagingToolkit.Barcode.Client.Results
{
	
	/// <summary> 
    /// Implements the "MECARD" address book entry format.
	/// Supported keys: N, SOUND, TEL, EMAIL, NOTE, ADR, BDAY, URL, plus ORG
	/// Unsupported keys: TEL-AV, NICKNAME
	/// 
	/// Except for TEL, multiple values for keys are also not supported;
	/// the first one found takes precedence.
	/// 
	/// Our understanding of the MECARD format is based on this document:
	/// 
	/// http://www.mobicode.org.tw/files/OMIA%20Mobile%20Bar%20Code%20Standard%20v3.2.1.doc 
	/// 
    /// Modified: May 10 2012
	/// </summary>	
	sealed class AddressBookDoCoMoResultParser:AbstractDoCoMoResultParser
	{

        public override ParsedResult Parse(Result result)
        {
            String rawText = ResultParser.GetMassagedText(result);
            if (!rawText.StartsWith("MECARD:"))
            {
                return null;
            }
            String[] rawName = AbstractDoCoMoResultParser.MatchDoCoMoPrefixedField("N:", rawText, true);
            if (rawName == null)
            {
                return null;
            }
            String name = ParseName(rawName[0]);
            String pronunciation = AbstractDoCoMoResultParser.MatchSingleDoCoMoPrefixedField("SOUND:", rawText, true);
            String[] phoneNumbers = AbstractDoCoMoResultParser.MatchDoCoMoPrefixedField("TEL:", rawText, true);
            String[] emails = AbstractDoCoMoResultParser.MatchDoCoMoPrefixedField("EMAIL:", rawText, true);
            String note = AbstractDoCoMoResultParser.MatchSingleDoCoMoPrefixedField("NOTE:", rawText, false);
            String[] addresses = AbstractDoCoMoResultParser.MatchDoCoMoPrefixedField("ADR:", rawText, true);
            String birthday = AbstractDoCoMoResultParser.MatchSingleDoCoMoPrefixedField("BDAY:", rawText, true);
            if (birthday != null && !ResultParser.IsStringOfDigits(birthday, 8))
            {
                // No reason to throw out the whole card because the birthday is formatted wrong.
                birthday = null;
            }
            String url = AbstractDoCoMoResultParser.MatchSingleDoCoMoPrefixedField("URL:", rawText, true);

            // Although ORG may not be strictly legal in MECARD, it does exist in VCARD and we might as well
            // honor it when found in the wild.
            String org = AbstractDoCoMoResultParser.MatchSingleDoCoMoPrefixedField("ORG:", rawText, true);

            return new AddressBookParsedResult(ResultParser.MaybeWrap(name), null, pronunciation, phoneNumbers, null, emails, null, null, note, addresses, null, org, birthday, null, url, null);
        }

        private static String ParseName(String name)
        {
            int comma = name.IndexOf(',');
            if (comma >= 0)
            {
                // Format may be last,first; switch it around
                return name.Substring(comma + 1) + ' ' + name.Substring(0, (comma) - (0));
            }
            return name;
        }
	}
}