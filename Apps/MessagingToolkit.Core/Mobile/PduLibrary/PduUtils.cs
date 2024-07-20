//===============================================================================
// OSML - Open Source Messaging Library
//
//===============================================================================
// Copyright © TWIT88.COM.  All rights reserved.
//
// This file is part of Open Source Messaging Library.
//
// Open Source Messaging Library is free software: you can redistribute it 
// and/or modify it under the terms of the GNU General Public License version 3.
//
// Open Source Messaging Library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this software.  If not, see <http://www.gnu.org/licenses/>.
//===============================================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Collections;

using MessagingToolkit.Core.Mobile;
using MessagingToolkit.Core.Mobile.Message;


namespace MessagingToolkit.Core.Mobile.PduLibrary
{
    /// <summary>
    /// PDU utility class
    /// </summary>
    internal static class PduUtils
    {
        #region ========================= Constants =========================================

        /// <summary>
        /// Greek alphabet mappings
        /// </summary>
        private static Char[,] GrcAlphabetRemapping = {
			{'\u0386', '\u0041'},   //GREEK CAPITAL LETTER ALPHA WITH TONOS
			{'\u0388', '\u0045'},   //GREEK CAPITAL LETTER EPSILON WITH TONOS
			{'\u0389', '\u0048'},   //GREEK CAPITAL LETTER ETA WITH TONOS
			{'\u038A', '\u0049'},   //GREEK CAPITAL LETTER IOTA WITH TONOS
			{'\u038C', '\u004F'},   //GREEK CAPITAL LETTER OMICRON WITH TONOS
			{'\u038E', '\u0059'},   //GREEK CAPITAL LETTER UPSILON WITH TONOS
			{'\u038F', '\u03A9'},   //GREEK CAPITAL LETTER OMEGA WITH TONOS
			{'\u0390', '\u0049'},   //GREEK SMALL LETTER IOTA WITH DIALYTIKA AND TONOS
			{'\u0391', '\u0041'},   //GREEK CAPITAL LETTER ALPHA
			{'\u0392', '\u0042'},   //GREEK CAPITAL LETTER BETA
			{'\u0393', '\u0393'},   //GREEK CAPITAL LETTER GAMMA
			{'\u0394', '\u0394'},   //GREEK CAPITAL LETTER DELTA
			{'\u0395', '\u0045'},   //GREEK CAPITAL LETTER EPSILON
			{'\u0396', '\u005A'},   //GREEK CAPITAL LETTER ZETA
			{'\u0397', '\u0048'},   //GREEK CAPITAL LETTER ETA
			{'\u0398', '\u0398'},   //GREEK CAPITAL LETTER THETA
			{'\u0399', '\u0049'},   //GREEK CAPITAL LETTER IOTA
			{'\u039A', '\u004B'},   //GREEK CAPITAL LETTER KAPPA
			{'\u039B', '\u039B'},   //GREEK CAPITAL LETTER LAMDA
			{'\u039C', '\u004D'},   //GREEK CAPITAL LETTER MU
			{'\u039D', '\u004E'},   //GREEK CAPITAL LETTER NU
			{'\u039E', '\u039E'},   //GREEK CAPITAL LETTER XI
			{'\u039F', '\u004F'},   //GREEK CAPITAL LETTER OMICRON
			{'\u03A0', '\u03A0'},   //GREEK CAPITAL LETTER PI
			{'\u03A1', '\u0050'},   //GREEK CAPITAL LETTER RHO
			{'\u03A3', '\u03A3'},   //GREEK CAPITAL LETTER SIGMA
			{'\u03A4', '\u0054'},   //GREEK CAPITAL LETTER TAU
			{'\u03A5', '\u0059'},   //GREEK CAPITAL LETTER UPSILON
			{'\u03A6', '\u03A6'},   //GREEK CAPITAL LETTER PHI
			{'\u03A7', '\u0058'},   //GREEK CAPITAL LETTER CHI
			{'\u03A8', '\u03A8'},   //GREEK CAPITAL LETTER PSI
			{'\u03A9', '\u03A9'},   //GREEK CAPITAL LETTER OMEGA
			{'\u03AA', '\u0049'},   //GREEK CAPITAL LETTER IOTA WITH DIALYTIKA
			{'\u03AB', '\u0059'},   //GREEK CAPITAL LETTER UPSILON WITH DIALYTIKA
			{'\u03AC', '\u0041'},   //GREEK SMALL LETTER ALPHA WITH TONOS
			{'\u03AD', '\u0045'},   //GREEK SMALL LETTER EPSILON WITH TONOS
			{'\u03AE', '\u0048'},   //GREEK SMALL LETTER ETA WITH TONOS
			{'\u03AF', '\u0049'},   //GREEK SMALL LETTER IOTA WITH TONOS
			{'\u03B0', '\u0059'},   //GREEK SMALL LETTER UPSILON WITH DIALYTIKA AND TONOS
			{'\u03B1', '\u0041'},   //GREEK SMALL LETTER ALPHA
			{'\u03B2', '\u0042'},   //GREEK SMALL LETTER BETA
			{'\u03B3', '\u0393'},   //GREEK SMALL LETTER GAMMA
			{'\u03B4', '\u0394'},   //GREEK SMALL LETTER DELTA
			{'\u03B5', '\u0045'},   //GREEK SMALL LETTER EPSILON
			{'\u03B6', '\u005A'},   //GREEK SMALL LETTER ZETA
			{'\u03B7', '\u0048'},   //GREEK SMALL LETTER ETA
			{'\u03B8', '\u0398'},   //GREEK SMALL LETTER THETA
			{'\u03B9', '\u0049'},   //GREEK SMALL LETTER IOTA
			{'\u03BA', '\u004B'},   //GREEK SMALL LETTER KAPPA
			{'\u03BB', '\u039B'},   //GREEK SMALL LETTER LAMDA
			{'\u03BC', '\u004D'},   //GREEK SMALL LETTER MU
			{'\u03BD', '\u004E'},   //GREEK SMALL LETTER NU
			{'\u03BE', '\u039E'},   //GREEK SMALL LETTER XI
			{'\u03BF', '\u004F'},   //GREEK SMALL LETTER OMICRON
			{'\u03C0', '\u03A0'},   //GREEK SMALL LETTER PI
			{'\u03C1', '\u0050'},   //GREEK SMALL LETTER RHO
			{'\u03C2', '\u03A3'},   //GREEK SMALL LETTER FINAL SIGMA
			{'\u03C3', '\u03A3'},   //GREEK SMALL LETTER SIGMA
			{'\u03C4', '\u0054'},   //GREEK SMALL LETTER TAU
			{'\u03C5', '\u0059'},   //GREEK SMALL LETTER UPSILON
			{'\u03C6', '\u03A6'},   //GREEK SMALL LETTER PHI
			{'\u03C7', '\u0058'},   //GREEK SMALL LETTER CHI
			{'\u03C8', '\u03A8'},   //GREEK SMALL LETTER PSI
			{'\u03C9', '\u03A9'},   //GREEK SMALL LETTER OMEGA
			{'\u03CA', '\u0049'},   //GREEK SMALL LETTER IOTA WITH DIALYTIKA
			{'\u03CB', '\u0059'},   //GREEK SMALL LETTER UPSILON WITH DIALYTIKA
			{'\u03CC', '\u004F'},   //GREEK SMALL LETTER OMICRON WITH TONOS
			{'\u03CD', '\u0059'},   //GREEK SMALL LETTER UPSILON WITH TONOS
			{'\u03CE', '\u03A9'}	//GREEK SMALL LETTER OMEGA WITH TONOS
		};

        /// <summary>
        /// Extended alphabets
        /// </summary>
        private static Char[] ExtAlphabet = {
			'\u000c',   // FORM FEED
			'\u005e',   // CIRCUMFLEX ACCENT
			'\u007b',   // LEFT CURLY BRACKET
			'\u007d',   // RIGHT CURLY BRACKET
			'\\',		// REVERSE SOLIDUS
			'\u005b',   // LEFT SQUARE BRACKET
			'\u007e',   // TILDE
			'\u005d',   // RIGHT SQUARE BRACKET
			'\u007c',   // VERTICAL LINES
			'\u20ac',   // EURO SIGN
		};

        /// <summary>
        /// Extended bytes
        /// </summary>
        private static string[] ExtBytes = {
			"1b0a",	 // FORM FEED
			"1b14",	 // CIRCUMFLEX ACCENT
			"1b28",	 // LEFT CURLY BRACKET
			"1b29",	 // RIGHT CURLY BRACKET
			"1b2f",	 // REVERSE SOLIDUS
			"1b3c",	 // LEFT SQUARE BRACKET
			"1b3d",	 // TILDE
			"1b3e",	 // RIGHT SQUARE BRACKET
			"1b40",	 // VERTICAL LINES
			"1b65",	 // EURO SIGN
		};

        /// <summary>
        /// Standard alphabets
        /// </summary>
        private static Char[] StdAlphabet = {
			'\u0040',	 // COMMERCIAL AT
			'\u00A3',	 // POUND SIGN
			'\u0024',	 // DOLLAR SIGN
			'\u00A5',	 // YEN SIGN
			'\u00E8',	 // LATIN SMALL LETTER E WITH GRAVE
			'\u00E9',	 // LATIN SMALL LETTER E WITH ACUTE
			'\u00F9',	 // LATIN SMALL LETTER U WITH GRAVE
			'\u00EC',	 // LATIN SMALL LETTER I WITH GRAVE
			'\u00F2',	 // LATIN SMALL LETTER O WITH GRAVE
			'\u00E7',	 // LATIN SMALL LETTER C WITH CEDILLA
			'\n',	     // LINE FEED
			'\u00D8',	 // LATIN CAPITAL LETTER O WITH STROKE
			'\u00F8',	 // LATIN SMALL LETTER O WITH STROKE
			'\r',		 // CARRIAGE RETURN
			'\u00C5',	 // LATIN CAPITAL LETTER A WITH RING ABOVE
			'\u00E5',	 // LATIN SMALL LETTER A WITH RING ABOVE
			'\u0394',	 // GREEK CAPITAL LETTER DELTA
			'\u005F',	 // LOW LINE
			'\u03A6',	 // GREEK CAPITAL LETTER PHI
			'\u0393',	 // GREEK CAPITAL LETTER GAMMA
			'\u039B',	 // GREEK CAPITAL LETTER LAMDA
			'\u03A9',	 // GREEK CAPITAL LETTER OMEGA
			'\u03A0',	 // GREEK CAPITAL LETTER PI
			'\u03A8',	 // GREEK CAPITAL LETTER PSI
			'\u03A3',	 // GREEK CAPITAL LETTER SIGMA
			'\u0398',	 // GREEK CAPITAL LETTER THETA
			'\u039E',	 // GREEK CAPITAL LETTER XI
			'\u00A0',	 // ESCAPE TO EXTENSION TABLE (or displayed as NBSP, see note above)
			'\u00C6',	 // LATIN CAPITAL LETTER AE
			'\u00E6',	 // LATIN SMALL LETTER AE
			'\u00DF',	 // LATIN SMALL LETTER SHARP S (German)
			'\u00C9',	 // LATIN CAPITAL LETTER E WITH ACUTE
			'\u0020',	 // SPACE
			'\u0021',	 // EXCLAMATION MARK
			'\u0022',	 // QUOTATION MARK
			'\u0023',	 // NUMBER SIGN
			'\u00A4',	 // CURRENCY SIGN
			'\u0025',	 // PERCENT SIGN
			'\u0026',	 // AMPERSAND
			'\'',		 // APOSTROPHE
			'\u0028',	 // LEFT PARENTHESIS
			'\u0029',	 // RIGHT PARENTHESIS
			'\u002A',	 // ASTERISK
			'\u002B',	 // PLUS SIGN
			'\u002C',	 // COMMA
			'\u002D',	 // HYPHEN-MINUS
			'\u002E',	 // FULL STOP
			'\u002F',	 // SOLIDUS
			'\u0030',	 // DIGIT ZERO
			'\u0031',	 // DIGIT ONE
			'\u0032',	 // DIGIT TWO
			'\u0033',	 // DIGIT THREE
			'\u0034',	 // DIGIT FOUR
			'\u0035',	 // DIGIT FIVE
			'\u0036',	 // DIGIT SIX
			'\u0037',	 // DIGIT SEVEN
			'\u0038',	 // DIGIT EIGHT
			'\u0039',	 // DIGIT NINE
			'\u003A',	 // COLON
			'\u003B',	 // SEMICOLON
			'\u003C',	 // LESS-THAN SIGN
			'\u003D',	 // EQUALS SIGN
			'\u003E',	 // GREATER-THAN SIGN
			'\u003F',	 // QUESTION MARK
			'\u00A1',	 // INVERTED EXCLAMATION MARK
			'\u0041',	 // LATIN CAPITAL LETTER A
			'\u0042',	 // LATIN CAPITAL LETTER B
			'\u0043',	 // LATIN CAPITAL LETTER C
			'\u0044',	 // LATIN CAPITAL LETTER D
			'\u0045',	 // LATIN CAPITAL LETTER E
			'\u0046',	 // LATIN CAPITAL LETTER F
			'\u0047',	 // LATIN CAPITAL LETTER G
			'\u0048',	 // LATIN CAPITAL LETTER H
			'\u0049',	 // LATIN CAPITAL LETTER I
			'\u004A',	 // LATIN CAPITAL LETTER J
			'\u004B',	 // LATIN CAPITAL LETTER K
			'\u004C',	 // LATIN CAPITAL LETTER L
			'\u004D',	 // LATIN CAPITAL LETTER M
			'\u004E',	 // LATIN CAPITAL LETTER N
			'\u004F',	 // LATIN CAPITAL LETTER O
			'\u0050',	 // LATIN CAPITAL LETTER P
			'\u0051',	 // LATIN CAPITAL LETTER Q
			'\u0052',	 // LATIN CAPITAL LETTER R
			'\u0053',	 // LATIN CAPITAL LETTER S
			'\u0054',	 // LATIN CAPITAL LETTER T
			'\u0055',	 // LATIN CAPITAL LETTER U
			'\u0056',	 // LATIN CAPITAL LETTER V
			'\u0057',	 // LATIN CAPITAL LETTER W
			'\u0058',	 // LATIN CAPITAL LETTER X
			'\u0059',	 // LATIN CAPITAL LETTER Y
			'\u005A',	 // LATIN CAPITAL LETTER Z
			'\u00C4',	 // LATIN CAPITAL LETTER A WITH DIAERESIS
			'\u00D6',	 // LATIN CAPITAL LETTER O WITH DIAERESIS
			'\u00D1',	 // LATIN CAPITAL LETTER N WITH TILDE
			'\u00DC',	 // LATIN CAPITAL LETTER U WITH DIAERESIS
			'\u00A7',	 // SECTION SIGN
			'\u00BF',	 // INVERTED QUESTION MARK
			'\u0061',	 // LATIN SMALL LETTER A
			'\u0062',	 // LATIN SMALL LETTER B
			'\u0063',	 // LATIN SMALL LETTER C
			'\u0064',	 // LATIN SMALL LETTER D
			'\u0065',	 // LATIN SMALL LETTER E
			'\u0066',	 // LATIN SMALL LETTER F
			'\u0067',	 // LATIN SMALL LETTER G
			'\u0068',	 // LATIN SMALL LETTER H
			'\u0069',	 // LATIN SMALL LETTER I
			'\u006A',	 // LATIN SMALL LETTER J
			'\u006B',	 // LATIN SMALL LETTER K
			'\u006C',	 // LATIN SMALL LETTER L
			'\u006D',	 // LATIN SMALL LETTER M
			'\u006E',	 // LATIN SMALL LETTER N
			'\u006F',	 // LATIN SMALL LETTER O
			'\u0070',	 // LATIN SMALL LETTER P
			'\u0071',	 // LATIN SMALL LETTER Q
			'\u0072',	 // LATIN SMALL LETTER R
			'\u0073',	 // LATIN SMALL LETTER S
			'\u0074',	 // LATIN SMALL LETTER T
			'\u0075',	 // LATIN SMALL LETTER U
			'\u0076',	 // LATIN SMALL LETTER V
			'\u0077',	 // LATIN SMALL LETTER W
			'\u0078',	 // LATIN SMALL LETTER X
			'\u0079',	 // LATIN SMALL LETTER Y
			'\u007A',	 // LATIN SMALL LETTER Z
			'\u00E4',	 // LATIN SMALL LETTER A WITH DIAERESIS
			'\u00F6',	 // LATIN SMALL LETTER O WITH DIAERESIS
			'\u00F1',	 // LATIN SMALL LETTER N WITH TILDE
			'\u00FC',	 // LATIN SMALL LETTER U WITH DIAERESIS
			'\u00E0',	 // LATIN SMALL LETTER A WITH GRAVE
		};


        /// <summary>
        /// Max characters per message
        /// </summary>
        private const int MaxSize = 140;

        #endregion =====================================================================================

        #region ===================== Public Static Method  ============================================

        /// <summary>
        /// Get a byte from PDU string
        /// </summary>
        /// <param name="pduCode">PDU string</param>
        /// <returns>A byte value</returns>
        public static byte GetByte(ref string pduCode)
        {
            if (string.IsNullOrEmpty(pduCode)) return 0;
            byte r = Convert.ToByte(pduCode.Substring(0, 2), 16);
            pduCode = pduCode.Substring(2);
            return r;
        }

        /// <summary>
        /// Get a string of certain length from the PDU code string
        /// </summary>
        /// <param name="pduCode">PDU code</param>
        /// <param name="length">Expected length</param>
        /// <returns>The extracted string</returns>
        public static string GetString(ref string pduCode, int length)
        {
            string r = pduCode.Substring(0, (length < pduCode.Length ? length : pduCode.Length));
            pduCode = (length < pduCode.Length ? pduCode.Substring(length) : string.Empty);
            return r;
        }


        /// <summary>
        /// Get date from SCTS format
        /// </summary>
        /// <param name="scts">Service center timestamp string</param>
        /// <param name="tz">Time zone format</param>
        /// <returns>Service center timestamp</returns>
        public static DateTime GetDate(ref string scts, ref string tz)
        {
            int year = 0;
            int month = 0;
            int day = 0;
            int hour = 0;
            int minute = 0;
            int second = 0;
            int timezone = 0;

            year = Convert.ToInt32(Swap(GetString(ref scts, 2))) + 2000;
            month = Convert.ToInt32(Swap(GetString(ref scts, 2)));
            day = Convert.ToInt32(Swap(GetString(ref scts, 2)));
            hour = Convert.ToInt32(Swap(GetString(ref scts, 2)));
            minute = Convert.ToInt32(Swap(GetString(ref scts, 2)));
            second = Convert.ToInt32(Swap(GetString(ref scts, 2)));
            timezone = Convert.ToInt32(Swap(GetString(ref scts, 2)));

            if (timezone >= 0)
            {
                tz = "GMT+" + timezone / 4;
            }
            else
            {
                tz = "GMT-" + timezone / 4;
            }

            DateTime result = new DateTime(year, month, day, hour, minute, second);
            return result;
        }


        /// <summary>
        /// Get service center address address
        /// </summary>
        /// <param name="address">PDU code containing the address</param>
        /// <returns>Service center address</returns>
        public static string GetAddress(string address)
        {
            char[] tmpChar = address.ToCharArray();
            int i = 0;
            string result = string.Empty;
            for (i = 0; i <= tmpChar.GetUpperBound(0); i += 2)
            {
                result += Swap(tmpChar[i].ToString() + tmpChar[i + 1].ToString());
            }
            if (result.Contains("F")) result = result.Substring(0, result.Length - 1);
            return result;
        }


        /// <summary>
        /// Fixed decode "@" charactor
        /// </summary>
        /// <param name="hexString">Hexadecimal string</param>
        /// <returns>The inverted hexadecimal string</returns>
        /***
        public static string InvertHexString(string hexString)
        {
            // For example:
            // 123456
            // ===>
            // 563412
            StringBuilder result = new StringBuilder();
            int i = 0;
            for (i = hexString.Length - 2; i >= 0; i += -2)
            {
                result.Append(hexString.Substring(i, 2));
            }
            return result.ToString();
        }
        ***/

        /// <summary>
        /// Inverse bytes
        /// </summary>
        /// <param name="source">Source</param>
        /// <returns>Inverted byte array</returns>
        public static byte[] GetInvertBytes(string source)
        {
            byte[] bytes = GetBytes(source);
            Array.Reverse(bytes);
            return bytes;
        }

        /// <summary>
        /// Get bytes
        /// </summary>
        /// <param name="source">Source</param>
        /// <returns>Hexadecimal byte list</returns>
        public static byte[] GetBytes(string source)
        {
            return GetBytes(source, 16);
        }

        /// <summary>
        /// Get bytes
        /// </summary>
        /// <param name="source">Source</param>
        /// <param name="fromBase">Number base</param>
        /// <returns></returns>
        public static byte[] GetBytes(string source, int fromBase)
        {
            List<byte> bytes = new List<byte>();
            for (int i = 0; i < source.Length / 2; i++)
                bytes.Add(Convert.ToByte(source.Substring(i * 2, 2), fromBase));

            return bytes.ToArray();
        }

        /// <summary>
        /// Convert byte to binary
        /// </summary>
        /// <param name="val">Byte value to be converted</param>
        /// <returns>Converted binary string</returns>
        /***
        public static string ByteToBinary(byte val)
        {
            string result = string.Empty;
            byte temp = val;
            while (true)
            {
                result = (temp % 2) + result;
                if (temp == 1 || temp == 0)
                    break;
                temp = Convert.ToByte(temp / 2);
            }
            result = result.PadLeft(8, Convert.ToChar("0"));
            return result;
        }
        ***/

        /// <summary>
        /// Convert binary to integer
        /// </summary>
        /// <param name="binary">Binary string</param>
        /// <returns>Integer value</returns>
        /****
        public static int BinaryToInt(string binary)
        {
            int result = 0;
            int i = 0;
            for (i = 0; i <= binary.Length - 1; i++)
            {
                result = result + Convert.ToInt32(Convert.ToInt32(binary.Substring(binary.Length - i - 1, 1)) * Math.Pow(2, i));
            }
            return result;
        }
        ***/


        /// <summary>
        /// Determine the SMS type from the PDU code string
        /// </summary>
        /// <param name="pduCode">PDU code</param>
        /// <returns>Message type indicator. See <see cref="MessageTypeIndicator"/></returns>
        public static MessageTypeIndicator GetSmsType(string pduCode)
        {
            // Get first october
            byte firstOctet = 0;
            int l = GetByte(ref pduCode);
            GetByte(ref pduCode);
            GetString(ref pduCode, (l - 1) * 2);
            firstOctet = GetByte(ref pduCode);

            // Get base code. Use last 2 bit and whether there's a header as remark
            int t1 = firstOctet & 3;
            // 00000011
            int t2 = firstOctet & 64;
            // 01000000            
            if (t1 == 3 && t2 == 64) return MessageTypeIndicator.MtiEmsSubmit;
            int type = t1 + t2;
            return (MessageTypeIndicator)Enum.Parse(typeof(MessageTypeIndicator), type.ToString());
        }

        /// <summary>
        /// Determine the message data coding scheme
        /// </summary>
        /// <param name="content">Message content</param>
        /// <returns>Message data coding scheme. See <see cref="MessageDataCodingScheme"/></returns>
        public static MessageDataCodingScheme GetDataCodingScheme(string content)
        {
            int i = 0;
            for (i = 1; i <= content.Length; i++)
            {
                int code = Convert.ToInt32(Convert.ToChar(content.Substring(i - 1, 1)));
                if (code < 0 || code > 255)
                {
                    return MessageDataCodingScheme.Ucs2;
                }
            }
            return MessageDataCodingScheme.DefaultAlphabet;
        }
        
        /// <summary>
        /// Convert a byte to hexadecimal string
        /// </summary>
        /// <param name="aByte">Byte to be converted</param>
        /// <returns>Hexadecimal representation of the byte value</returns>       
        public static string ByteToHex(byte aByte)
        {
            string result = aByte.ToString("X2");
            return result;
        }
        

        /// <summary>
        /// Encode the content into 7 bit PDU code string
        /// </summary>
        /// <param name="content">Content</param>
        /// <returns>Encoded content</returns>        
        public static string Encode7Bit(string content)
        {
            // Prepare
            char[] charArray = content.ToCharArray();
            string t = string.Empty;
            foreach (char c in charArray)
            {
                t = CharTo7Bits(c) + t;
            }
            // Add "0"
            int i = 0;
            if ((t.Length % 8) != 0)
            {
                int j = t.Length % 8;
                for (i = 1; i <= 8 - j; i++)
                {
                    t = "0" + t;
                }
            }
            // Split into 8bits
            string result = string.Empty;
            for (i = t.Length - 8; i >= 0; i += -8)
            {
                result = result + BitsToHex(t.Substring(i, 8));
            }
            return result;
        }
       

        /// <summary>
        /// Convert bits to hexadecimal
        /// </summary>
        /// <param name="bits">Bits to be converted</param>
        /// <returns>Hexadecimal value</returns>       
        public static string BitsToHex(string bits)
        {
            // Convert 8Bits to Hex String
            int i = 0;
            int v = 0;
            for (i = 0; i <= bits.Length - 1; i++)
            {
                v = Convert.ToInt32(v + Convert.ToInt32(bits.Substring(i, 1)) * Math.Pow(2, (7 - i)));
            }
            return v.ToString("X2");
        }
        

        /// <summary>
        /// Convert a character to 7 bits code
        /// </summary>
        /// <param name="c">Character to be converted</param>
        /// <returns>7 bit code of the character</returns>     
        public static string CharTo7Bits(char c)
        {
            if (c == '@') return "0000000";
            string result = "";
            int i = 0;
            for (i = 0; i <= 6; i++)
            {
                int ascValue = (int)c;
                if ((ascValue & Convert.ToInt32(Math.Pow(2, i))) > 0)
                {
                    result = "1" + result;
                }
                else
                {
                    result = "0" + result;
                }
            }
            return result;
        }
        

        /// <summary>
        /// Encode the unicode string
        /// </summary>
        /// <param name="content">Content to be encoded</param>
        /// <returns>Encoded unicode content</returns>
        /// 
        public static string EncodeUcs2(string content)
        {
            int i = 0;
            int v = 0;
            string result = null;
            string t = null;
            result = "";
            for (i = 0; i < content.Length; i++)
            {
                if ((content.Length - i) >= 4)
                    v = (int)(content.Substring(i, 4).ToCharArray()[0]);
                else
                    v = (int)(content.Substring(i).ToCharArray()[0]);

                t = v.ToString("X4");
                result += t;
            }
            return result;
        }
    
        /// <summary>
        /// 8 bit content encoding
        /// </summary>
        /// <param name="content">Content to be encoded</param>
        /// <returns>Encoded content</returns>
        public static string Encode8Bit(string content)
        {
            byte[] byteContent = Encoding.UTF8.GetBytes(content);
            return string.Empty;
        }
        

        /// <summary>
        /// Swap the first 2 characters
        /// </summary>
        /// <param name="twoBitStr">2 bit string</param>
        /// <returns>Swapped string</returns>
        public static string Swap(string twoBitStr)
        {
            // Swap two bit like "EF" TO "FE"
            char[] c = twoBitStr.ToCharArray();
            char t = c[0];
            c[0] = c[1];
            c[1] = t;
            return (c[0].ToString() + c[1].ToString());
        }

        /// <summary>
        /// Get the enum value in hexadecimal format
        /// </summary>
        /// <param name="type">Enum type</param>
        /// <param name="value">Enum value</param>
        /// <returns>A hexadecimal value</returns>
        public static string GetEnumHexValue(Type type, string value)
        {
            int result = (int)Enum.Parse(type, value);
            return result.ToString("X2");
        }


        /// <summary>
        /// Encode the service center address number
        /// </summary>
        /// <param name="value">Value to be encoded into PDU string</param>
        /// <returns>Encoded PDU string</returns>      
        public static string EncodeServiceCenterAddress(string value)
        {
            if (string.IsNullOrEmpty(value)) return Sms.DefaultSmscAddress;

            string serviceCenterNumber = string.Empty;

            // Convert an ServiceCenterNumber to PDU Code
            if (value.Contains("+"))
            {
                serviceCenterNumber = "91";
            }
            else
            {
                serviceCenterNumber = "81";
            }
            value = value.Substring(1);
            int i = 0;
            if ((value.Length % 2) == 1)
            {
                value += "F";
            }
            for (i = 1; i <= value.Length; i += 2)
            {
                serviceCenterNumber += Swap(value.Substring(i - 1, 2));
            }
            serviceCenterNumber = ByteToHex(Convert.ToByte((serviceCenterNumber.Length - 2) / 2 + 1)) + serviceCenterNumber;

            return serviceCenterNumber;
        }        

        /// <summary>
        /// Encode the destination address
        /// </summary>
        /// <param name="value">Destination address</param>
        /// <returns>Encoded address</returns>      
        public static string EncodeDestinationAddress(string value)
        {
            string destinationAddress = string.Empty;
            if (value.Contains("+"))
            {
                destinationAddress += "91";
            }
            else
            {
                destinationAddress += "81";
            }
            value = value.Replace("+", "");
            destinationAddress = value.Length.ToString("X2") + destinationAddress;
            int i = 0;
            if ((value.Length % 2) == 1)
            {
                value += "F";
            }
            for (i = 1; i <= value.Length; i += 2)
            {
                destinationAddress += Swap(value.Substring(i - 1, 2));
            }
            return destinationAddress;
        }
      

        /// <summary>
        /// Encode the content
        /// </summary>
        /// <param name="sms">Value to be encoded</param>
        /// <returns>Encoded content</returns>       
        public static string EncodeContent(Sms sms)
        {
            string content = string.Empty;
            switch (sms.DataCodingScheme)
            {
                case MessageDataCodingScheme.DefaultAlphabet:
                    content = Encode7Bit(sms.Content);
                    break;
                case MessageDataCodingScheme.SevenBits:
                    content = Encode7Bit(sms.Content);
                    break;
                case MessageDataCodingScheme.EightBits:
                case MessageDataCodingScheme.Class1Ud8Bits:
                    content = Encode8Bit(sms.Content);
                    break;
                case MessageDataCodingScheme.Ucs2:
                    content = EncodeUcs2(sms.Content);
                    break;
                default:
                    content = sms.Content;
                    break;
            }
            return content;
        }
       

        /// <summary>
        /// Get the content length based on the data coding scheme
        /// </summary>
        /// <param name="sms">SMS</param>
        /// <returns>Content length</returns>
        public static int GetContentLength(Sms sms)
        {
            int contentLength = sms.Content.Length;
            switch (sms.DataCodingScheme)
            {
                case MessageDataCodingScheme.DefaultAlphabet:
                    contentLength = sms.Content.Length;
                    break;
                case MessageDataCodingScheme.SevenBits:
                    contentLength = sms.Content.Length;
                    break;
                case MessageDataCodingScheme.Ucs2:
                    contentLength = sms.Content.Length * 2;
                    break;
                case MessageDataCodingScheme.EightBits:
                case MessageDataCodingScheme.Class1Ud8Bits:
                    contentLength = sms.Content.Length * 2;
                    break;
            }
            return contentLength;
        }
       

        /// <summary>
        /// Decode a unicode PDU string
        /// </summary>
        /// <param name="unicode">Unicode PDU code string</param>
        /// <returns>Unicode string</returns>
        public static string DecodeUcs2(string unicode)
        {
            string code = string.Empty;
            int j = 0;
            // 2 Byte a Unicode char
            string[] c = new string[Convert.ToInt32(unicode.Length / 4)];

            for (j = 0; j <= unicode.Length / 4 - 1; j++)
            {
                char[] d = unicode.ToCharArray(j * 4, 4);
                c[j] = new string(d);//d.ToString();
                c[j] = Convert.ToString((char)(Convert.ToInt32(c[j], 16)));
                code += c[j];
            }
            return code;
        }


        /// <summary>
        /// Decode an ASCII PDU code
        /// </summary>
        /// <param name="source">PDU code</param>
        /// <param name="length">length</param>
        /// <returns>Decoded ASCII string</returns>
        public static string Decode7Bit(string source, int length)
        {
            byte[] bytes = GetInvertBytes(source);

            string binary = string.Empty;

            foreach (byte b in bytes)
                binary += Convert.ToString(b, 2).PadLeft(8, '0');

            binary = binary.PadRight(length * 7, '0');

            string result = string.Empty;

            for (int i = 1; i <= length; i++)
                result += (char)Convert.ToByte(binary.Substring(binary.Length - i * 7, 7), 2);

            return result.Replace('\x0', '\x40');


            /*
            string inv7BitCode = InvertHexString(source);
            string binary = string.Empty;
            string result = string.Empty;
            int i = 0;
            for (i = 0; i <= inv7BitCode.Length - 1; i += 2)
            {
                binary += ByteToBinary(Convert.ToByte(inv7BitCode.Substring(i, 2), 16));
            }
            int temp = 0;
            for (i = 1; i <= length; i++)
            {
                temp = BinaryToInt(binary.Substring(binary.Length - i * 7, 7));
                // There is a problem:
                // "@" charactor is decoded to binary "0000000", but its Ascii Code is 64!!
                // Don't know what to do with it,maybe it is a bug!
                if (temp == 0) temp = 64;
                result = result + (char)(temp);
            }
            return (result);
            */
        }


        /// <summary>
        /// Decode an UTF8 PDU code
        /// </summary>
        /// <param name="source">PDU code</param>
        /// <param name="length">length</param>
        /// <returns>Decoded UTF8 string</returns>
        public static string Decode8bit(string source, int length)
        {
            int len = length * 2;
            if (len > source.Length) len = source.Length;
            byte[] bytes = GetBytes(source.Substring(0, len));
            return Encoding.UTF8.GetString(bytes);
        }

        /// <summary>
        /// Convert bytes to strings
        /// </summary>
        /// <param name="bytes">Bytes to be converted</param>
        /// <returns>Converted string</returns>    
        /***
        public static string BytesToString(byte[] bytes)
        {
            StringBuilder text;
            string extChar;
            int i, j;

            text = new StringBuilder();
            for (i = 0; i < bytes.Length; i++)
            {
                if (bytes[i] == 0x1b)
                {
                    extChar = "1b" + bytes[++i].ToString("X").ToLower();
                    for (j = 0; j < ExtBytes.Length; j++)
                        if (ExtBytes[j].Equals(extChar)) text.Append(ExtAlphabet[j]);
                }
                else text.Append(StdAlphabet[bytes[i]]);
            }
            return text.ToString();
        }    
        ***/

        /// <summary>
        /// Convert string to bytes
        /// </summary>
        /// <param name="text">String to be converted</param>
        /// <param name="bytes">Converted bytes</param>
        /// <returns>Bytes array</returns>
        public static int StringToBytes(string text, byte[] bytes)
        {
            int i, j, k, index;
            char ch;

            k = 0;
            for (i = 0; i < text.Length; i++)
            {
                ch = text[i];
                index = -1;
                for (j = 0; j < ExtAlphabet.Length; j++)
                    if (ExtAlphabet[j] == ch)
                    {
                        index = j;
                        break;
                    }
                if (index != -1)	// An extended char...
                {
                    bytes[k] = (byte)Int16.Parse(ExtBytes[index].Substring(0, 2), NumberStyles.HexNumber);
                    k++;
                    bytes[k] = (byte)Int16.Parse(ExtBytes[index].Substring(2, 2), NumberStyles.HexNumber);
                    k++;
                }
                else	// Maybe a standard char...
                {
                    index = -1;
                    for (j = 0; j < StdAlphabet.Length; j++)
                        if (StdAlphabet[j] == ch)
                        {
                            index = j;
                            bytes[k] = (byte)j;
                            k++;
                            break;
                        }
                    if (index == -1)	// Maybe a Greek Char...
                    {
                        for (j = 0; j < GrcAlphabetRemapping.GetUpperBound(0); j++)
                            if (GrcAlphabetRemapping[j, 0] == ch)
                            {
                                index = j;
                                ch = GrcAlphabetRemapping[j, 1];
                                break;
                            }
                        if (index != -1)
                        {
                            for (j = 0; j < StdAlphabet.Length; j++)
                                if (StdAlphabet[j] == ch)
                                {
                                    index = j;
                                    bytes[k] = (byte)j;
                                    k++;
                                    break;
                                }
                        }
                        else	// Unknown char replacement...
                        {
                            bytes[k] = (byte)' ';
                            k++;
                        }
                    }
                }
            }
            return k;
        }

        /// <summary>
        /// Convert text to PDU
        /// </summary>
        /// <param name="text">Text to be converted</param>
        /// <returns>PDU</returns>
        public static string TextToPdu(string text)
        {
            return MessagingToolkit.Pdu.PduUtils.StringToPdu(text);
        }


        /// <summary>
        /// Sets the specified bit to true.
        /// </summary>
        /// <param name="bits">The BitArray to modify.</param>
        /// <param name="index">The bit index to set to true.</param>
        private static void Set(BitArray bits, int index)
        {
            for (int increment = 0; index >= bits.Length; increment = +128)
            {
                bits.Length += increment;
            }

            bits.Set(index, true);
        }               

        /// <summary>
        /// Convert string to BCD (binary coded decimal) format
        /// </summary>
        /// <param name="s">String to be converted</param>
        /// <returns>BCD string</returns>
        public static string ToBcdFormat(string s)
        {
            string bcd;
            int i;

            if ((s.Length % 2) != 0) s = s + "F";
            bcd = "";
            for (i = 0; i < s.Length; i += 2) bcd = bcd + s[i + 1] + s[i];
            return bcd;
        }

        /// <summary>
        /// Determine if this is a EMS
        /// </summary>
        /// <param name="encodedText">The encoded text</param>
        /// <returns>true of false</returns>
        public static bool IsBig(string encodedText)
        {
            int messageLength;
            messageLength = encodedText.Length / 2;
            return (messageLength > MaxSize ? true : false);
        }


        /// <summary>
        /// Get the validity period
        /// </summary>
        /// <param name="validPeriod">Valid period for the message</param>
        /// <returns>Validity period string</returns>
        public static string GetValidityPeriodBits(MessageValidPeriod validPeriod)
        {
            int validityPeriod = (int)Enum.Parse(typeof(MessageValidPeriod), validPeriod.ToString());

            string bits;
            int value;

            if (validityPeriod == -1) bits = "FF";
            else
            {
                if (validityPeriod <= 12)
                    value = (validityPeriod * 12) - 1;
                else if (validityPeriod <= 24)
                    value = (((validityPeriod - 12) * 2) + 143);
                else if (validityPeriod <= 720)
                    value = (validityPeriod / 24) + 166;
                else
                    value = (validityPeriod / 168) + 192;
                bits = value.ToString("X");
                if (bits.Length != 2)
                    bits = "0" + bits;
                if (bits.Length > 2)
                    bits = "FF";
            }
            return bits;
        }


        /// <summary>
        /// Get number of SMS parts
        /// </summary>
        /// <param name="encodedText">Encoded SMS content</param>
        /// <returns>Number of parts in the SMS</returns>
        public static int GetNoOfParts(string encodedText)
        {
            int noOfParts = 0;
            int partSize;
            int messageLength;

            partSize = MaxSize - 8;
            messageLength = encodedText.Length / 2;
            noOfParts = messageLength / partSize;
            if ((noOfParts * partSize) < (messageLength))
                noOfParts++;
            return noOfParts;
        }


        /// <summary>
        /// Get the current part of the SMS
        /// </summary>
        /// <param name="encodedText">Encoded SMS content</param>
        /// <param name="partNo">SMS part number</param>
        /// <param name="udhLength">User data header length</param>
        /// <returns>Current SMS PDU part</returns>
        public static string GetPart(string encodedText, int partNo, int udhLength)
        {
            int partSize;

            if (partNo != 0)
            {
                partSize = MaxSize - udhLength;
                partSize *= 2;
                if (((partSize * (partNo - 1)) + partSize) > encodedText.Length)
                    return encodedText.Substring(partSize * (partNo - 1));
                else
                    return encodedText.Substring(partSize * (partNo - 1), partSize);
            }
            else return encodedText;
        }


        /// <summary>
        /// Get the PDU for a multipart SMS
        /// </summary>
        /// <param name="sms">SMS object</param>
        /// <param name="encodedText">Encoded content</param>
        /// <param name="noOfParts">Number of SMS parts</param>
        /// <param name="partNo">Current part number</param>
        /// <param name="refNo">Message reference number</param>
        /// <returns>The PDU</returns>
        public static string GetPdu(Sms sms, string encodedText, int noOfParts, int partNo, int refNo)
        {
            int protocolIdentifier = 0;

            string pdu, udh, ud = string.Empty, dataLen = string.Empty;
            string str1, str2;

            pdu = string.Empty;
            udh = string.Empty;

            if (!string.IsNullOrEmpty(sms.ServiceCenterNumber) && sms.ServiceCenterNumber != Sms.DefaultSmscAddress && !IsAlphaNumericAddress(sms.ServiceCenterNumber))
            {
                if (sms.ServiceCenterNumber.StartsWith("+"))
                {
                    str1 = "91" + ToBcdFormat(sms.ServiceCenterNumber.Substring(1));
                }
                else
                {
                    str1 = "81" + ToBcdFormat(sms.ServiceCenterNumber);
                }
                str2 = (str1.Length / 2).ToString("X");
                if (str2.Length != 2)
                    str2 = "0" + str2;
                pdu = pdu + str2 + str1;
            }
            else
            {
                pdu = pdu + Sms.DefaultSmscAddress;
            }

            if (((sms.SourcePort != -1) && (sms.DestinationPort != -1)) || IsBig(encodedText))
            {
                if (sms.StatusReportRequest == MessageStatusReportRequest.SmsReportRequest)
                    pdu = pdu + "71";
                else
                    pdu = pdu + "51";
            }
            else
            {
                if (sms.StatusReportRequest == MessageStatusReportRequest.SmsReportRequest)
                    pdu = pdu + "31";
                else
                    pdu = pdu + "11";
            }

            pdu = pdu + "00";
            str1 = sms.DestinationAddress;
            if (str1[0] == '+')
            {
                str1 = ToBcdFormat(str1.Substring(1));
                str2 = (sms.DestinationAddress.Length - 1).ToString("X");
                str1 = "91" + str1;
            }
            else
            {
                str1 = ToBcdFormat(str1);
                str2 = (sms.DestinationAddress.Length).ToString("X");
                str1 = "81" + str1;
            }

            if (str2.Length != 2) str2 = "0" + str2;

            pdu = pdu + str2 + str1;


            string s;
            s = protocolIdentifier.ToString("X");
            while (s.Length < 2) s = "0" + s;
            pdu = pdu + s;
            
            switch (sms.DataCodingScheme)
            {
                case MessageDataCodingScheme.DefaultAlphabet:
                case MessageDataCodingScheme.Undefined:
                case MessageDataCodingScheme.SevenBits:
                    if (sms.Flash)
                        pdu = pdu + "10";
                    else
                        pdu = pdu + "00";
                    break;
                case MessageDataCodingScheme.EightBits:
                case MessageDataCodingScheme.Class1Ud8Bits:
                    if (sms.Flash)
                        pdu = pdu + "14";
                    else
                        pdu = pdu + "04";
                    break;
                case MessageDataCodingScheme.Ucs2:
                    if (sms.Flash)
                        pdu = pdu + "18";
                    else
                        pdu = pdu + "08";
                    break;
                case MessageDataCodingScheme.Custom:
                    int dcs = (int)Enum.Parse(typeof(MessageDataCodingScheme), sms.DataCodingScheme.ToString());
                    s = dcs.ToString("X");
                    while (s.Length < 2)
                        s = "0" + s;
                    pdu = pdu + s;
                    break;
            }
            pdu = pdu + GetValidityPeriodBits(sms.ValidityPeriod);

            if ((sms.SourcePort != -1) && (sms.DestinationPort != -1))
            {
                udh += "060504";
                s = sms.DestinationPort.ToString("X");
                while (s.Length < 4)
                    s = "0" + s;
                udh += s;
                s = sms.SourcePort.ToString("X");
                while (s.Length < 4)
                    s = "0" + s;
                udh += s;
            }

            if (IsBig(encodedText))
            {
                if ((sms.SourcePort != -1) && (sms.DestinationPort != -1))
                    udh = "0C" + udh.Substring(2) + "0804";
                else
                    udh += "060804";
                s = refNo.ToString("X");
                while (s.Length < 4)
                    s = "0" + s;
                udh += s;
                s = noOfParts.ToString("X");
                while (s.Length < 2)
                    s = "0" + s;
                udh += s;
                s = partNo.ToString("X");
                while (s.Length < 2) s = "0" + s;
                udh += s;
            }

            switch (sms.DataCodingScheme)
            {
                case MessageDataCodingScheme.DefaultAlphabet:
                case MessageDataCodingScheme.Undefined:
                case MessageDataCodingScheme.SevenBits:
                    ud = GetPart(encodedText, partNo, udh.Length);
                    int udLength = sms.Content.Length % 8 == 7 ? ud.Length - 1 : ud.Length;
                    dataLen = (((udLength + udh.Length) * 8 / 7) / 2).ToString("X");
                    //dataLen = GetUdLength(sms.Content, 160, partNo).ToString("X");
                    break;
                case MessageDataCodingScheme.EightBits:
                case MessageDataCodingScheme.Class1Ud8Bits:
                    ud = GetPart(encodedText, partNo, udh.Length);
                    dataLen = ((ud.Length + udh.Length) / 2).ToString("X");
                    break;
                case MessageDataCodingScheme.Ucs2:
                    ud = GetPart(encodedText, partNo, udh.Length);
                    dataLen = ((ud.Length + udh.Length) / 2).ToString("X");
                    break;
                case MessageDataCodingScheme.Custom:
                    int dcs = (int)Enum.Parse(typeof(MessageDataCodingScheme), sms.DataCodingScheme.ToString());
                    if ((dcs & 0x04) == 0)
                    {
                        ud = GetPart(encodedText, partNo, udh.Length);
                        dataLen = (((ud.Length + udh.Length) * 8 / 7) / 2).ToString("X");
                    }
                    else
                    {
                        ud = GetPart(encodedText, partNo, udh.Length);
                        dataLen = ((ud.Length + udh.Length) / 2).ToString("X");
                    }
                    break;
            }
            if (dataLen.Length != 2)
                dataLen = "0" + dataLen;
            if (udh.Length != 0)
                pdu = pdu + dataLen + udh + ud;
            else
                pdu = pdu + dataLen + ud;

            return pdu.ToUpper();
        }


        /// <summary>
        /// Gets the length of the user data
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="maxMessageLength">Length of the max message.</param>
        /// <param name="partNo">The part no.</param>
        /// <returns></returns>
        private static int GetUdLength(string text, int maxMessageLength, int partNo)
        {
            // computes offset to which part of the string is to be encoded into the PDU
            // also sets the MpMaxNo field of the concatInfo if message is multi-part
            int offset;
            int maxParts = 1;

            // must use the unencoded septets not the actual string since
            // it is possible that some special characters in string are multi-septet
            byte[] unencodedSeptets = MessagingToolkit.Pdu.PduUtils.StringToUnencodedSeptets(text);

            maxParts = (unencodedSeptets.Length / maxMessageLength) + 1;
          
            if ((maxParts > 1) && (partNo > 0))
            {
                //      - if partNo > maxParts
                //          - error
                if (partNo > maxParts)
                {
                    throw new SystemException("Invalid partNo: " + partNo + ", maxParts=" + maxParts);
                }
                offset = ((partNo - 1) * maxMessageLength);
            }
            else
            {
                // just get from the start
                offset = 0;
            }

            // copy the portion of the full unencoded septet array for this part
            byte[] septetsForPart = new byte[Math.Min(maxMessageLength, unencodedSeptets.Length - offset)];
            Array.Copy(unencodedSeptets, offset, septetsForPart, 0, septetsForPart.Length);

            return septetsForPart.Length;
        }

        public static bool IsAlphaNumericAddress(string address)
        {
            if (string.IsNullOrEmpty(address))
            {              
                return true;
            }

            if (address.StartsWith("+"))
            {               
                return false;
            }

            // now we need to loop through the string, examining each character
            for (int i = 0; i < address.Length; i++)
            {
                // if this character isn't a letter and it isn't a number then return false
                // because it means this isn't a valid alpha numeric string
                if (!(char.IsNumber(address[i])))
                    return true;
                    
            }
            return false;
        }

        /// <summary>
        /// Determine if it is status report
        /// </summary>
        /// <param name="pdu">PDU</param>
        /// <returns>true or false</returns>
        /**
        public static bool IsStatusReportMessage(string pdu)
        {
            int index, i;

            i = Int16.Parse(pdu.Substring(0, 2), NumberStyles.HexNumber);
            index = (i + 1) * 2;
            i = Int16.Parse(pdu.Substring(index, 2), NumberStyles.HexNumber);
            if ((i & 0x02) == 2)
                return true;
            else
                return false;
        }
        **/

        /// <summary>
        /// Determine if it is a inbound message
        /// </summary>
        /// <param name="pdu">PDU</param>
        /// <returns>true or false</returns>
        /// 
        /***
        public static bool IsInboundMessage(string pdu)
        {
            int index, i;

            i = Int16.Parse(pdu.Substring(0, 2), NumberStyles.HexNumber);
            index = (i + 1) * 2;
            i = Int16.Parse(pdu.Substring(index, 2), NumberStyles.HexNumber);
            if ((i & 0x03) == 0)
                return true;
            else
                return false;
        }
        **/

        /// <summary>
        /// Decode status report
        /// </summary>
        /// <param name="pdu">PDU</param>
        /// <returns>Decoded status report</returns>
        /**
        public static MessageInformation DecodeStatusReport(string pdu)
        {
            return new MessageInformation();
        }
        **/

        /// <summary>
        /// Convert PDU to text
        /// </summary>
        /// <param name="pdu">PDU</param>
        /// <returns>Decoded text</returns>
        /****
        public static string PduToText(string pdu)
        {
            StringBuilder text;
            byte[] oldBytes, newBytes;
            BitArray bits;
            int i, j, value1, value2;

            oldBytes = new byte[pdu.Length / 2];
            for (i = 0; i < pdu.Length / 2; i++)
            {
                oldBytes[i] = (byte)(Byte.Parse("" + pdu[i * 2], NumberStyles.HexNumber) * 16);
                oldBytes[i] += Byte.Parse("" + pdu[(i * 2) + 1], NumberStyles.HexNumber);
            }

            bits = new BitArray(pdu.Length / 2 * 8);
            value1 = 0;
            for (i = 0; i < pdu.Length / 2; i++)
                for (j = 0; j < 8; j++)
                {
                    value1 = (i * 8) + j;
                    if ((oldBytes[i] & (1 << j)) != 0) bits.Set(value1, true);
                }
            value1++;

            value2 = value1 / 7;
            if (value2 == 0) value2++;
            newBytes = new byte[value2];
            for (i = 0; i < value2; i++)
                for (j = 0; j < 7; j++)
                    if ((value1 + 1) > (i * 7 + j))
                        if ((i * 7 + j) < bits.Length)
                            if (bits.Get(i * 7 + j)) newBytes[i] |= (byte)(1 << j);

            text = new StringBuilder();
            if (newBytes[value2 - 1] == 0)
                for (i = 0; i < newBytes.Length - 1; i++) text.Append(Convert.ToChar(newBytes[i]));
            else
                for (i = 0; i < newBytes.Length; i++) text.Append(Convert.ToChar(newBytes[i]));
            return text.ToString();
        }
        **/

        /// <summary>
        /// Decoded inbound messages
        /// </summary>
        /// <param name="pdu">PDU</param>
        /// <returns>The message information</returns>
        /// 
        /****
        public static MessageInformation DecodeInboundMessage(string pdu)
        {
            MessageInformation messageInformation = new MessageInformation();

            DateTime date;
            string originator;
            string str1, str2;
            int index, i, j, k, protocol, addr, year, month, day, hour, min, sec, skipBytes;
            bool hasUdh;
            int udhLength;
            string udhData;
            byte[] bytes;

            int mpRefNo = 0;
            int mpMaxNo = 0;
            int mpSeqNo = 0;

            skipBytes = 0;

            i = Int16.Parse(pdu.Substring(0, 2), NumberStyles.HexNumber);
            index = (i + 1) * 2;

            hasUdh = ((Int16.Parse(pdu.Substring(index, 2), NumberStyles.HexNumber) & 0x40) != 0) ? true : false;

            index += 2;
            i = Int16.Parse(pdu.Substring(index, 2), NumberStyles.HexNumber);
            j = index + 4;
            originator = string.Empty;
            for (k = 0; k < i; k += 2)
                originator = originator + pdu[j + k + 1] + pdu[j + k];
            originator = "+" + originator;
            if (originator[originator.Length - 1].Equals('F'))
                originator = originator.Substring(0, originator.Length - 1);

            addr = Int16.Parse(pdu.Substring(j - 2, 2), NumberStyles.HexNumber);
            if ((addr & (1 << 6)) != 0 && (addr & (1 << 5)) == 0 && (addr & (1 << 4)) != 0)
            {
                str1 = PduToText(pdu.Substring(j, k));
                bytes = new byte[str1.Length];
                for (i = 0; i < str1.Length; i++) bytes[i] = (byte)str1[i];
                originator = BytesToString(bytes);
            }

            index = j + k + 2;
            str1 = "" + pdu[index] + pdu[index + 1];
            protocol = Int16.Parse(str1, NumberStyles.HexNumber);
            index += 2;
            year = Int16.Parse("" + pdu[index + 1] + pdu[index]); index += 2;
            month = Int16.Parse("" + pdu[index + 1] + pdu[index]); index += 2;
            day = Int16.Parse("" + pdu[index + 1] + pdu[index]); index += 2;
            hour = Int16.Parse("" + pdu[index + 1] + pdu[index]); index += 2;
            min = Int16.Parse("" + pdu[index + 1] + pdu[index]); index += 2;
            sec = Int16.Parse("" + pdu[index + 1] + pdu[index]); index += 4;
            date = new DateTime(year + 2000, month, day, hour, min, sec);

            if (hasUdh)
            {
                udhLength = Int16.Parse(pdu.Substring(index + 2, 2), NumberStyles.HexNumber);
                udhData = pdu.Substring(index + 2 + 2, udhLength * 2);

                if (udhData.Substring(0, 2).Equals("00"))
                {
                    mpRefNo = Int16.Parse(udhData.Substring(4, 2), NumberStyles.HexNumber);
                    mpMaxNo = Int16.Parse(udhData.Substring(6, 2), NumberStyles.HexNumber);
                    mpSeqNo = Int16.Parse(udhData.Substring(8, 2), NumberStyles.HexNumber);
                    skipBytes = 7;
                }
                if (udhData.Substring(0, 2).Equals("08"))
                {
                    mpRefNo = Int16.Parse(udhData.Substring(4, 4), NumberStyles.HexNumber);
                    mpMaxNo = Int16.Parse(udhData.Substring(8, 2), NumberStyles.HexNumber);
                    mpSeqNo = Int16.Parse(udhData.Substring(10, 2), NumberStyles.HexNumber);
                    skipBytes = 8;
                }
            }
            else
            {
                udhLength = 0;
                udhData = "";
            }

            switch (protocol & 0x0C)
            {
                case 0:
                    str1 = PduToText(pdu.Substring(index + 2));
                    bytes = new byte[str1.Length];
                    for (i = 0; i < str1.Length; i++) bytes[i] = (byte)str1[i];
                    str2 = BytesToString(bytes);
                    if (hasUdh) str1 = str2.Substring(udhLength + 2);
                    else str1 = str2;
                    break;
                case 4:
                    index += 2;
                    if (hasUdh) index += udhLength + skipBytes;
                    str1 = "";
                    while (index < pdu.Length)
                    {
                        i = Int16.Parse("" + pdu[index] + pdu[index + 1], NumberStyles.HexNumber);
                        str1 = str1 + (char)i;
                        index += 2;
                    }
                    break;
                case 8:
                    index += 2;
                    if (hasUdh) index += udhLength + skipBytes;
                    str1 = "";
                    while (index < pdu.Length)
                    {
                        i = Int16.Parse("" + pdu[index] + pdu[index + 1], NumberStyles.HexNumber);
                        j = Int16.Parse("" + pdu[index + 2] + pdu[index + 3], NumberStyles.HexNumber);
                        str1 = str1 + (char)((i * 256) + j);
                        index += 4;
                    }
                    break;
            }
            messageInformation.PhoneNumber = originator;
            messageInformation.ReceivedDate = date;
            messageInformation.Content = str1;
            return messageInformation;
        }
        ***/

        #endregion ================================================================

    }
}
