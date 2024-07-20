
using System;
using System.Collections.Generic;
using Result = MessagingToolkit.Barcode.Result;

namespace MessagingToolkit.Barcode.Client.Results
{

    /// <summary>
    /// Implements the "BIZCARD" address book entry format, though this has been
    /// largely reverse-engineered from examples observed in the wild -- still
    /// looking for a definitive reference.
    /// 
    /// Modified: May 10 2012
    /// </summary>
    public sealed class BizcardResultParser : AbstractDoCoMoResultParser
    {

        // Yes, we extend AbstractDoCoMoResultParser since the format is very much
        // like the DoCoMo MECARD format, but this is not technically one of 
        // DoCoMo's proposed formats

        public override ParsedResult Parse(Result result)
        {
            String rawText = ResultParser.GetMassagedText(result);
            if (!rawText.StartsWith("BIZCARD:"))
            {
                return null;
            }
            String firstName = AbstractDoCoMoResultParser.MatchSingleDoCoMoPrefixedField("N:", rawText, true);
            String lastName = AbstractDoCoMoResultParser.MatchSingleDoCoMoPrefixedField("X:", rawText, true);
            String fullName = BuildName(firstName, lastName);
            String title = AbstractDoCoMoResultParser.MatchSingleDoCoMoPrefixedField("T:", rawText, true);
            String org = AbstractDoCoMoResultParser.MatchSingleDoCoMoPrefixedField("C:", rawText, true);
            String[] addresses = AbstractDoCoMoResultParser.MatchDoCoMoPrefixedField("A:", rawText, true);
            String phoneNumber1 = AbstractDoCoMoResultParser.MatchSingleDoCoMoPrefixedField("B:", rawText, true);
            String phoneNumber2 = AbstractDoCoMoResultParser.MatchSingleDoCoMoPrefixedField("M:", rawText, true);
            String phoneNumber3 = AbstractDoCoMoResultParser.MatchSingleDoCoMoPrefixedField("F:", rawText, true);
            String email = AbstractDoCoMoResultParser.MatchSingleDoCoMoPrefixedField("E:", rawText, true);

            return new AddressBookParsedResult(MaybeWrap(fullName), null, null, BuildPhoneNumbers(phoneNumber1, phoneNumber2, phoneNumber3), null, ResultParser.MaybeWrap(email), null, null, null, addresses, null, org,
                    null, title,null, null);
        }

        private static String[] BuildPhoneNumbers(String number1, String number2, String number3)
        {
            List<String> numbers = new List<String>(3);
            if (number1 != null)
            {
                numbers.Add(number1);
            }
            if (number2 != null)
            {
                numbers.Add(number2);
            }
            if (number3 != null)
            {
                numbers.Add(number3);
            }
            int size = numbers.Count;
            if (size == 0)
            {
                return null;
            }
            return ToStringArray(numbers);
        }

        private static String BuildName(String firstName, String lastName)
        {
            if (firstName == null)
            {
                return lastName;
            }
            else
            {
                return (lastName == null) ? firstName : firstName + ' ' + lastName;
            }
        }

    }
}