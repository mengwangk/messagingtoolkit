using System;
using System.Text;

namespace MessagingToolkit.Barcode.Client.Results
{
    /// <summary>
    /// Modified: May 10 2012
    /// </summary>
    public sealed class AddressBookParsedResult : ParsedResult
    {

        private readonly String[] names;
        private readonly String pronunciation;
        private readonly String[] phoneNumbers;
        private readonly String[] phoneTypes;
        private readonly String[] emails;
        private readonly String[] emailTypes;
        private readonly String instantMessenger;
        private readonly String note;
        private readonly String[] addresses;
        private readonly String[] addressTypes;
        private readonly String org;
        private readonly String birthday;
        private readonly String title;
        private readonly String url;

        public AddressBookParsedResult(String[] names, String pronunciation, String[] phoneNumbers, String[] phoneTypes, 
                String[] emails, String[] emailTypes, String instantMessenger, String note,
                String[] addresses, String[] addressTypes, String org, String birthday, String title, String url)
            : base(ParsedResultType.AddessBook)
        {
            this.names = names;
            this.pronunciation = pronunciation;
            this.phoneNumbers = phoneNumbers;
            this.phoneTypes = phoneTypes;
            this.emails = emails;
            this.emailTypes = emailTypes;
            this.instantMessenger = instantMessenger;
            this.note = note;
            this.addresses = addresses;
            this.addressTypes = addressTypes;
            this.org = org;
            this.birthday = birthday;
            this.title = title;
            this.url = url;
        }

        public String[] GetNames()
        {
            return names;
        }

        /// <summary>
        /// In Japanese, the name is written in kanji, which can have multiple readings. Therefore a hint
        /// is often provided, called furigana, which spells the name phonetically.
        /// </summary>
        /// <returns>
        /// The pronunciation of the getNames() field, often in hiragana or katakana.
        /// </returns>
        public String GetPronunciation()
        {
            return pronunciation;
        }

        public String[] GetPhoneNumbers()
        {
            return phoneNumbers;
        }


        /// <returns>optional descriptions of the type of each phone number. It could be like "HOME", but,
        /// there is no guaranteed or standard format.</returns>
        public String[] GetPhoneTypes()
        {
            return phoneTypes;
        }

        public String[] GetEmails()
        {
            return emails;
        }


        /// <returns>optional descriptions of the type of each e-mail. It could be like "WORK", but,
        /// there is no guaranteed or standard format.</returns>
        public String[] GetEmailTypes()
        {
            return emailTypes;
        }

        public String GetInstantMessenger()
        {
            return instantMessenger;
        }

        public String GetNote()
        {
            return note;
        }

        public String[] GetAddresses()
        {
            return addresses;
        }


        /// <returns>optional descriptions of the type of each e-mail. It could be like "WORK", but,
        /// there is no guaranteed or standard format.</returns>
        public String[] GetAddressTypes()
        {
            return addressTypes;
        }

        public String GetTitle()
        {
            return title;
        }

        public String GetOrg()
        {
            return org;
        }

        public String GetURL()
        {
            return url;
        }


        /// <returns>birthday formatted as yyyyMMdd (e.g. 19780917)</returns>
        public String GetBirthday()
        {
            return birthday;
        }

        public override String DisplayResult
        {
            get
            {
                StringBuilder result = new StringBuilder(100);
                ParsedResult.MaybeAppend(names, result);
                ParsedResult.MaybeAppend(pronunciation, result);
                ParsedResult.MaybeAppend(title, result);
                ParsedResult.MaybeAppend(org, result);
                ParsedResult.MaybeAppend(addresses, result);
                ParsedResult.MaybeAppend(phoneNumbers, result);
                ParsedResult.MaybeAppend(emails, result);
                ParsedResult.MaybeAppend(instantMessenger, result);
                ParsedResult.MaybeAppend(url, result);
                ParsedResult.MaybeAppend(birthday, result);
                ParsedResult.MaybeAppend(note, result);
                return result.ToString();
            }
        }

    }
}