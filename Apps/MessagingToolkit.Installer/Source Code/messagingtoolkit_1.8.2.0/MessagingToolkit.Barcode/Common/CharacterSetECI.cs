using System;
using System.Collections.Generic;

namespace MessagingToolkit.Barcode.Common
{

    /// <summary>
    /// Encapsulates a Character Set ECI, according to "Extended Channel Interpretations" 5.3.1.1
    /// of ISO 18004.
    /// 
    /// Modified: April 21 2012
    /// </summary>
    public sealed class CharacterSetECI : ECI
    {
        private static Dictionary<int, CharacterSetECI> ValuetoEci;
        private static Dictionary<string, CharacterSetECI> NameToEci;

        private string encodingName;

        public string EncodingName
        {
            get
            {
                return encodingName;
            }
        }


        private static void Initialize()
        {
            ValuetoEci = new Dictionary<int, CharacterSetECI>(29);
            NameToEci = new Dictionary<string, CharacterSetECI>(29);
            // TODO figure out if these values are even right!
            AddCharacterSet(0, "Cp437");
            AddCharacterSet(1, new string[] { "ISO-8859-1", "ISO8859_1" });
            AddCharacterSet(2, "Cp437");
            AddCharacterSet(3, new string[] { "ISO-8859-1", "ISO8859_1" });
            AddCharacterSet(4, new string[] { "ISO-8859-2", "ISO8859_2" });
            AddCharacterSet(5, new string[] { "ISO-8859-3", "ISO8859_3" });
            AddCharacterSet(6, new string[] { "ISO-8859-4", "ISO8859_4" });
            AddCharacterSet(7, new string[] { "ISO-8859-5", "ISO8859_5" });
            AddCharacterSet(8, new string[] { "ISO-8859-6", "ISO8859_6" });
            AddCharacterSet(9, new string[] { "ISO-8859-7", "ISO8859_7" });
            AddCharacterSet(10, new string[] { "ISO-8859-8", "ISO8859_8" });
            AddCharacterSet(11, new string[] { "ISO-8859-9", "ISO8859_9" });
            AddCharacterSet(12, new string[] { "ISO-8859-4", "ISO-8859-10", "ISO8859_10" });
            AddCharacterSet(13, new string[] { "ISO-8859-11", "ISO8859_11" });
            AddCharacterSet(15, new string[] { "ISO-8859-13", "ISO8859_13" });
            AddCharacterSet(16, new string[] { "ISO-8859-1", "ISO-8859-14", "ISO8859_14" });
            AddCharacterSet(17, new string[] { "ISO-8859-15", "ISO8859_15" });
            AddCharacterSet(18, new string[] { "ISO-8859-3", "ISO-8859-16", "ISO8859_16" });
            AddCharacterSet(20, new string[] { "SHIFT-JIS", "SJIS", "Shift_JIS" });
            AddCharacterSet(21, new string[] { "windows-1250", "CP1250" });
            AddCharacterSet(22, new string[] { "windows-1251", "CP1251" });
            AddCharacterSet(23, new string[] { "windows-1252", "CP1252" });
            AddCharacterSet(24, new string[] { "windows-1256", "CP1256", });
            AddCharacterSet(25, new string[] { "UTF-16BE", "UnicodeBig" });
            AddCharacterSet(26, new string[] { "UTF-8", "UTF8" });
            AddCharacterSet(27, "US-ASCII");
            AddCharacterSet(170, "US-ASCII");
            AddCharacterSet(28, "BIG5");
            AddCharacterSet(29, new[] { "GB18030", "GB2312", "EUC_CN", "GBK" });
            AddCharacterSet(30, new[] { "EUC-KR", "EUC_KR"});

        }


        /// <summary>
        /// Initializes a new instance of the <see cref="CharacterSetECI"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="encodingName">Name of the encoding.</param>
        private CharacterSetECI(int value, string encodingName)
            : base(value)
        {
            this.encodingName = encodingName;
        }

        private static void AddCharacterSet(int value, string encodingName)
        {
            CharacterSetECI eci = new CharacterSetECI(value, encodingName);
            ValuetoEci[value] = eci; // can't use valueOf
            NameToEci[encodingName] = eci;
        }

        private static void AddCharacterSet(int value, string[] encodingNames)
        {
            CharacterSetECI eci = new CharacterSetECI(value, encodingNames[0]);
            ValuetoEci[value] = eci; // can't use valueOf
            for (int i = 0; i < encodingNames.Length; i++)
            {
                NameToEci[encodingNames[i]] = eci;
            }
        }

        /// <summary>
        /// Gets the character set ECI by value.
        /// </summary>
        /// <param name="val">The value_ renamed.</param>
        /// <returns>
        /// {@link CharacterSetECI} representing ECI of given value, or null if it is legal but
        /// unsupported
        /// </returns>
        /// <throws>  IllegalArgumentException if ECI value is invalid </throws>
        public static CharacterSetECI GetCharacterSetECIByValue(int value)
        {
            if (ValuetoEci == null)
            {
                Initialize();
            }
            if (value < 0 || value >= 900)
            {
                throw FormatException.Instance;
            }
            if (ValuetoEci.ContainsKey(value))
                return ValuetoEci[value];
            return null;
        }

        /// <summary>
        /// Gets the name of the character set ECI by.
        /// </summary>
        /// <param name="name">character set ECI encoding name</param>
        /// <returns>
        /// {@link CharacterSetECI} representing ECI for character encoding, or null if it is legal
        /// but unsupported
        /// </returns>
        public static CharacterSetECI GetCharacterSetECIByName(string name)
        {
            if (NameToEci == null)
            {
                Initialize();
            }
            if (NameToEci.ContainsKey(name))
            {
                return NameToEci[name];
            }
            return null;
        }
    }
}