﻿//===============================================================================
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
using System.Collections;
using System.Reflection;

namespace MessagingToolkit.Pdu
{
	/// <summary>
	/// PDU utility class
	/// </summary>
	public class PduUtils
	{
		/// <summary>
		/// GSM alphabet mappings
		/// </summary>
		private static readonly char[][] GreekCharacterAlphabetRemapping = new char[][]
		{   
			new char[]{'\u0386', '\u0041'}, 
			new char[]{'\u0388', '\u0045'}, 
			new char[]{'\u0389', '\u0048'}, 
			new char[]{'\u038A', '\u0049'}, 
			new char[]{'\u038C', '\u004F'}, 
			new char[]{'\u038E', '\u0059'}, 
			new char[]{'\u038F', '\u03A9'}, 
			new char[]{'\u0390', '\u0049'}, 
			new char[]{'\u0391', '\u0041'}, 
			new char[]{'\u0392', '\u0042'}, 
			new char[]{'\u0393', '\u0393'}, 
			new char[]{'\u0394', '\u0394'}, 
			new char[]{'\u0395', '\u0045'}, 
			new char[]{'\u0396', '\u005A'}, 
			new char[]{'\u0397', '\u0048'}, 
			new char[]{'\u0398', '\u0398'}, 
			new char[]{'\u0399', '\u0049'}, 
			new char[]{'\u039A', '\u004B'}, 
			new char[]{'\u039B', '\u039B'}, 
			new char[]{'\u039C', '\u004D'}, 
			new char[]{'\u039D', '\u004E'}, 
			new char[]{'\u039E', '\u039E'}, 
			new char[]{'\u039F', '\u004F'}, 
			new char[]{'\u03A0', '\u03A0'}, 
			new char[]{'\u03A1', '\u0050'}, 
			new char[]{'\u03A3', '\u03A3'}, 
			new char[]{'\u03A4', '\u0054'}, 
			new char[]{'\u03A5', '\u0059'}, 
			new char[]{'\u03A6', '\u03A6'}, 
			new char[]{'\u03A7', '\u0058'}, 
			new char[]{'\u03A8', '\u03A8'}, 
			new char[]{'\u03A9', '\u03A9'}, 
			new char[]{'\u03AA', '\u0049'}, 
			new char[]{'\u03AB', '\u0059'}, 
			new char[]{'\u03AC', '\u0041'}, 
			new char[]{'\u03AD', '\u0045'}, 
			new char[]{'\u03AE', '\u0048'}, 
			new char[]{'\u03AF', '\u0049'}, 
			new char[]{'\u03B0', '\u0059'}, 
			new char[]{'\u03B1', '\u0041'}, 
			new char[]{'\u03B2', '\u0042'}, 
			new char[]{'\u03B3', '\u0393'}, 
			new char[]{'\u03B4', '\u0394'}, 
			new char[]{'\u03B5', '\u0045'}, 
			new char[]{'\u03B6', '\u005A'}, 
			new char[]{'\u03B7', '\u0048'}, 
			new char[]{'\u03B8', '\u0398'}, 
			new char[]{'\u03B9', '\u0049'}, 
			new char[]{'\u03BA', '\u004B'}, 
			new char[]{'\u03BB', '\u039B'}, 
			new char[]{'\u03BC', '\u004D'}, 
			new char[]{'\u03BD', '\u004E'}, 
			new char[]{'\u03BE', '\u039E'}, 
			new char[]{'\u03BF', '\u004F'}, 
			new char[]{'\u03C0', '\u03A0'}, 
			new char[]{'\u03C1', '\u0050'}, 
			new char[]{'\u03C2', '\u03A3'}, 
			new char[]{'\u03C3', '\u03A3'}, 
			new char[]{'\u03C4', '\u0054'}, 
			new char[]{'\u03C5', '\u0059'}, 
			new char[]{'\u03C6', '\u03A6'}, 
			new char[]{'\u03C7', '\u0058'}, 
			new char[]{'\u03C8', '\u03A8'}, 
			new char[]{'\u03C9', '\u03A9'}, 
			new char[]{'\u03CA', '\u0049'}, 
			new char[]{'\u03CB', '\u0059'}, 
			new char[]{'\u03CC', '\u004F'}, 
			new char[]{'\u03CD', '\u0059'}, 
			new char[]{'\u03CE', '\u03A9'}
		};

		private static readonly char[] ExtendedAlphabet = new char[]{
													'\u000c', 
													'\u005e', 
													'\u007b', 
													'\u007d', 
													'\\', 
													'\u005b', 
													'\u007e', 
													'\u005d', 
													'\u007c', 
													'\u20ac'};

		private static readonly string[] ExtendedBytes = new string[]{
											"1b0a", 
											"1b14", 
											"1b28", 
											"1b29", 
											"1b2f", 
											"1b3c", 
											"1b3d", 
											"1b3e", 
											"1b40", 
											"1b65"};

		/// <summary>
		/// This is an adjustment required to compensate for
		/// multi-byte characters split across the end of a pdu part
		/// if the previous part is noted to be ending in a '1b'
		/// call this method on the first char of the next part
		/// to adjust it for the missing '1b'
		/// </summary>
		/// <param name="c">Character</param>
		/// <returns>Mult character</returns>
		public static string GetMultiCharFor(char c)
		{
			switch (c)
			{
                // GSM 0x0A (line feed) ==> form feed
				case '\n':
					return "'\u000c'";
				// GSM 0x14 (greek capital lamda) ==> circumflex				
				case '\u039B':
					return "^";
				// GSM 0x28 (left parenthesis) ==> left curly brace				
				case '(':
					return "{";
				// GSM 0x29 (right parenthesis) ==> right curly brace
				case ')':
					return "}";
				// GSM 0x2f (solidus or slash) ==> reverse solidus or backslash
				case '/':
					return "\\";
				// GSM 0x3c (less than sign) ==> left square bracket
				case '<':
					return "[";
				// GSM 0x3d (equals sign) ==> tilde
				case '=':
					return "~";
				// GSM 0x3e (greater than sign) ==> right square bracket
				case '>':
					return "]";
				// GSM 0x40 (inverted exclamation point) ==> pipe
				case '\u00A1':
					return "|";
				// GSM 0x65 (latin small e) ==> euro
				case 'e':
					return "\u20ac";
			}
			return string.Empty;
		}

		/// <summary>
		/// Standard alphabets
		/// </summary>
		private static readonly char[] StandardAlphabets = new char[]{ 
												'\u0040', // COMMERCIAL AT
												'\u00A3', // POUND SIGN
												'\u0024', // DOLLAR SIGN
												'\u00A5', // YEN SIGN
												'\u00E8', // LATIN SMALL LETTER E WITH GRAVE
												'\u00E9', // LATIN SMALL LETTER E WITH ACUTE
												'\u00F9', // LATIN SMALL LETTER U WITH GRAVE
												'\u00EC', // LATIN SMALL LETTER I WITH GRAVE
												'\u00F2', // LATIN SMALL LETTER O WITH GRAVE
												'\u00E7', // LATIN SMALL LETTER C WITH CEDILLA
												'\n', // LINE FEED
												'\u00D8', // LATIN CAPITAL LETTER O WITH STROKE
												'\u00F8', // LATIN SMALL LETTER O WITH STROKE
												'\r', // CARRIAGE RETURN
												'\u00C5', // LATIN CAPITAL LETTER A WITH RING ABOVE
												'\u00E5', // LATIN SMALL LETTER A WITH RING ABOVE
												'\u0394', // GREEK CAPITAL LETTER DELTA
												'\u005F', // LOW LINE
												'\u03A6', // GREEK CAPITAL LETTER PHI
												'\u0393', // GREEK CAPITAL LETTER GAMMA
												'\u039B', // GREEK CAPITAL LETTER LAMDA
												'\u03A9', // GREEK CAPITAL LETTER OMEGA
												'\u03A0', // GREEK CAPITAL LETTER PI
												'\u03A8', // GREEK CAPITAL LETTER PSI
												'\u03A3', // GREEK CAPITAL LETTER SIGMA
												'\u0398', // GREEK CAPITAL LETTER THETA
												'\u039E', // GREEK CAPITAL LETTER XI
												'\u00A0', // ESCAPE TO EXTENSION TABLE (or displayed as NBSP, see
												// note
												// above)
												'\u00C6', // LATIN CAPITAL LETTER AE
												'\u00E6', // LATIN SMALL LETTER AE
												'\u00DF', // LATIN SMALL LETTER SHARP S (German)
												'\u00C9', // LATIN CAPITAL LETTER E WITH ACUTE
												'\u0020', // SPACE
												'\u0021', // EXCLAMATION MARK
												'\u0022', // QUOTATION MARK
												'\u0023', // NUMBER SIGN
												'\u00A4', // CURRENCY SIGN
												'\u0025', // PERCENT SIGN
												'\u0026', // AMPERSAND
												'\'', // APOSTROPHE
												'\u0028', // LEFT PARENTHESIS
												'\u0029', // RIGHT PARENTHESIS
												'\u002A', // ASTERISK
												'\u002B', // PLUS SIGN
												'\u002C', // COMMA
												'\u002D', // HYPHEN-MINUS
												'\u002E', // FULL STOP
												'\u002F', // SOLIDUS
												'\u0030', // DIGIT ZERO
												'\u0031', // DIGIT ONE
												'\u0032', // DIGIT TWO
												'\u0033', // DIGIT THREE
												'\u0034', // DIGIT FOUR
												'\u0035', // DIGIT FIVE
												'\u0036', // DIGIT SIX
												'\u0037', // DIGIT SEVEN
												'\u0038', // DIGIT EIGHT
												'\u0039', // DIGIT NINE
												'\u003A', // COLON
												'\u003B', // SEMICOLON
												'\u003C', // LESS-THAN SIGN
												'\u003D', // EQUALS SIGN
												'\u003E', // GREATER-THAN SIGN
												'\u003F', // QUESTION MARK
												'\u00A1', // INVERTED EXCLAMATION MARK
												'\u0041', // LATIN CAPITAL LETTER A
												'\u0042', // LATIN CAPITAL LETTER B
												'\u0043', // LATIN CAPITAL LETTER C
												'\u0044', // LATIN CAPITAL LETTER D
												'\u0045', // LATIN CAPITAL LETTER E
												'\u0046', // LATIN CAPITAL LETTER F
												'\u0047', // LATIN CAPITAL LETTER G
												'\u0048', // LATIN CAPITAL LETTER H
												'\u0049', // LATIN CAPITAL LETTER I
												'\u004A', // LATIN CAPITAL LETTER J
												'\u004B', // LATIN CAPITAL LETTER K
												'\u004C', // LATIN CAPITAL LETTER L
												'\u004D', // LATIN CAPITAL LETTER M
												'\u004E', // LATIN CAPITAL LETTER N
												'\u004F', // LATIN CAPITAL LETTER O
												'\u0050', // LATIN CAPITAL LETTER P
												'\u0051', // LATIN CAPITAL LETTER Q
												'\u0052', // LATIN CAPITAL LETTER R
												'\u0053', // LATIN CAPITAL LETTER S
												'\u0054', // LATIN CAPITAL LETTER T
												'\u0055', // LATIN CAPITAL LETTER U
												'\u0056', // LATIN CAPITAL LETTER V
												'\u0057', // LATIN CAPITAL LETTER W
												'\u0058', // LATIN CAPITAL LETTER X
												'\u0059', // LATIN CAPITAL LETTER Y
												'\u005A', // LATIN CAPITAL LETTER Z
												'\u00C4', // LATIN CAPITAL LETTER A WITH DIAERESIS
												'\u00D6', // LATIN CAPITAL LETTER O WITH DIAERESIS
												'\u00D1', // LATIN CAPITAL LETTER N WITH TILDE
												'\u00DC', // LATIN CAPITAL LETTER U WITH DIAERESIS
												'\u00A7', // SECTION SIGN
												'\u00BF', // INVERTED QUESTION MARK
												'\u0061', // LATIN SMALL LETTER A
												'\u0062', // LATIN SMALL LETTER B
												'\u0063', // LATIN SMALL LETTER C
												'\u0064', // LATIN SMALL LETTER D
												'\u0065', // LATIN SMALL LETTER E
												'\u0066', // LATIN SMALL LETTER F
												'\u0067', // LATIN SMALL LETTER G
												'\u0068', // LATIN SMALL LETTER H
												'\u0069', // LATIN SMALL LETTER I
												'\u006A', // LATIN SMALL LETTER J
												'\u006B', // LATIN SMALL LETTER K
												'\u006C', // LATIN SMALL LETTER L
												'\u006D', // LATIN SMALL LETTER M
												'\u006E', // LATIN SMALL LETTER N
												'\u006F', // LATIN SMALL LETTER O
												'\u0070', // LATIN SMALL LETTER P
												'\u0071', // LATIN SMALL LETTER Q
												'\u0072', // LATIN SMALL LETTER R
												'\u0073', // LATIN SMALL LETTER S
												'\u0074', // LATIN SMALL LETTER T
												'\u0075', // LATIN SMALL LETTER U
												'\u0076', // LATIN SMALL LETTER V
												'\u0077', // LATIN SMALL LETTER W
												'\u0078', // LATIN SMALL LETTER X
												'\u0079', // LATIN SMALL LETTER Y
												'\u007A', // LATIN SMALL LETTER Z
												'\u00E4', // LATIN SMALL LETTER A WITH DIAERESIS
												'\u00F6', // LATIN SMALL LETTER O WITH DIAERESIS
												'\u00F1', // LATIN SMALL LETTER N WITH TILDE
												'\u00FC', // LATIN SMALL LETTER U WITH DIAERESIS
												'\u00E0', // LATIN SMALL LETTER A WITH GRAVE
												};



		// ==================================================
		// FIRST OCTET CONSTANTS
		// ==================================================
		// to add, use the & with MASK to clear bits on original value
		//         and | this cleared value with constant specified 
		// TP-MTI   xxxxxx00 = SMS-DELIVER
		//          xxxxxx10 = SMS-STATUS-REPORT
		//          xxxxxx01 = SMS-SUBMIT
		public const int TpMtiMask = 0xFC;
		public const int TpMtiSmsDeliver = 0x00;
		public const int TpMtiSmsSubmit = 0x01;
		public const int TpMtiSmsStatusReport = 0x02;

		// TP-RD   xxxxx0xx = accept duplicate messages
		//         xxxxx1xx = reject duplicate messages
		//         for SMS-SUBMIT only
		public const int TpRdMask = 0xFB;
		public const int TpRdAcceptDuplicates = 0x00;
		public const int TpRdRejectDuplicates = 0x04;

		// TP-MMS   xxxxx0xx = more messages for the MS
		//          xxxxx1xx = no more messages for the MS
		//          for SMS-DELIVER and SMS-STATUS-REPORT only
		public const int TpMmsMask = 0xFB;

		public const int TpMmsNoMessages = 0x00;

		public const int TpMmsMoreMessages = 0x04;

		// TP-VPF   xxx00xxx = no validity period
		//          xxx10xxx = validity period integer-representation
		//          xxx11xxx = validity period timestamp-representation    
		public const int TpVpfMask = 0xE7;
		public const int TpVpfNone = 0x00;
		public const int TpVpfInteger = 0x10;
		public const int TpVpfTimestamp = 0x18;

		// TP-SRI   xx0xxxxx = no status report to SME (for SMS-DELIVER only)
		//          xx1xxxxx = status report to SME
		public const int TpSriMask = 0xDF;
		public const int TpSriNoReport = 0x00;
		public const int TpSriReport = 0x20;

		// TP-SRR   xx0xxxxx = no status report (for SMS-SUBMIT only)
		//          xx1xxxxx = status report
		public const int TpSrrMask = 0xDF;
		public const int TpSrrNoReport = 0x00;
		public const int TpSrrReport = 0x20;

		// TP-UDHI  x0xxxxxx = no UDH
		//          x1xxxxxx = UDH present
		public const int TpUdhiMask = 0xBF;
		public const int TpUdhiNoUdh = 0x00;
		public const int TpUdhiWithUdh = 0x40;

		// TP-RP reply path - 1xxxxxxx = reply path exist
		//                    0xxxxxxx = no reply path exist
		public const int TpRpMask = 0x7F;
		public const int TpRpNoRp = 0x00;
		public const int TpRpWithRp = 0x80;

		/// <summary>
		/// Reply address element
		/// </summary>
		public const int ReplyAddressElement = 0x22;

		// ==================================================
		// ADDRESS-TYPE CONSTANTS 
		// ==================================================
		// some typical ones used for sending, though receiving may get other types
		// usually 1 001 0001 (0x91) international format 
		//         1 000 0001 (0x81) (unknown) short number (e.g. access codes)
		//         1 101 0000 (0xD0) alphanumeric (e.g. access code names like PasaLoad)
		public const int AddressNumberPlanIdMask = 0x0F;
		public const int AddressNumberPlanIdUnknown = 0x00;
		public const int AddressNumberPlanIdTelephone = 0x01;
		public const int AddressTypeMask = 0x70;
		public const int AddressTypeUnknown = 0x00;
		public const int AddressTypeInternational = 0x10;
		public const int AddressTypeAlphanumeric = 0x50;

		public const int AddressTypeInternationFormat = 0x91;
		public const int AddressTypeDomesticFormat = 0x81;

		// ==================================================
		// DCS ENCODING CONSTANTS 
		// ==================================================
		public const int DcsCodingGroupMask = 0x0F;
		public const int DcsCodingGroupData = 0xF0;
		public const int DcsCodingGroupGeneral = 0xC0;
		public const int DcsEncodingMask = 0xF3;
		public const int DcsEncoding7Bit = 0x00;
		public const int DcsEncoding8Bit = 0x04;
		public const int DcsEncodingUcs2 = 0x08;
		public const int DcsMessageClassMask = 0xEC;
		public const int DcsMessageClassFlash = 0x10;
		public const int DcsMessageClassMe = 0x11;
		public const int DcsMessageClassSim = 0x12;
		public const int DcsMessageClassTe = 0x13;


		/// <summary>
		/// Gets the address type
		/// </summary>
		/// <param name="address">The address</param>
		/// <returns></returns>
		public static int GetAddressTypeFor(string address)
		{
			bool international = false;
			// strip any + to simplify checks
			// but presence of + automatically assumes an
			// international style address
			if (address.StartsWith("+"))
			{
				international = true;
				address = address.Substring(1);
			}

			if (!international) // Modified: May 18 2012
			{
				for (int i = 0; i < address.Length; i++)
				{
					if (!Char.IsDigit(address[i]))
					{
						// check if alphanumeric
						return CreateAddressType(AddressTypeAlphanumeric);
					}
				}
			}
			// check if 15 digits or plus is used
			// 15 digits is the max length and anything number 
			// with this length is already international format
			// if the length is less, the presence of the plus
			// will determine international format or not
			//if ((address.Length > 15) || (international))
			if (international)
			{
				return CreateAddressType(AddressTypeInternational | AddressNumberPlanIdTelephone);
			}
			else
			{
				return CreateAddressType(AddressTypeUnknown | AddressNumberPlanIdTelephone);
			}
		}

		public static int ExtractAddressType(int addressType)
		{
			return addressType & AddressTypeMask;
		}

		public static int ExtractNumberPlan(int addressType)
		{
			return addressType & AddressNumberPlanIdMask;
		}

		public static int CreateAddressType(int addressType)
		{
			// last bit is always set
			return 0x80 | addressType;
		}

		public static int ExtractDcsEncoding(int dataCodingScheme)
		{
			return dataCodingScheme & ~PduUtils.DcsEncodingMask;
		}

		public static int ExtractDcsFlash(int dataCodingScheme)
		{
			// this is only useful if DCS != 0
			return dataCodingScheme & ~DcsMessageClassMask;
		}

		public static int ExtractDcsCodingGroup(int dataCodingScheme)
		{
			return dataCodingScheme & ~DcsCodingGroupMask;
		}

        /// <summary>
        /// Determine the message data coding scheme
        /// </summary>
        /// <param name="content">Message content</param>
        /// <returns>Message data coding scheme. See <see cref="MessageDataCodingScheme"/></returns>
        public static int GetDataCodingScheme(string content)
        {
            int i = 0;
            for (i = 1; i <= content.Length; i++)
            {
                int code = Convert.ToInt32(Convert.ToChar(content.Substring(i - 1, 1)));
                if (code < 0 || code > 255)
                {
                    return DcsEncodingUcs2;
                }
            }
            return DcsEncoding7Bit;
        }

		// ==================================================
		// ENCODING/DECODING utility methods 
		// ==================================================
		private static int GetTpField(Pdu pdu, string fieldName)
		{
			try
			{

#if !NETFX_CORE
				PropertyInfo propertyInfo = pdu.GetType().GetProperty(fieldName);
#else
				PropertyInfo propertyInfo = pdu.GetType().GetRuntimeProperty(fieldName);
#endif
				if (propertyInfo != null)
				{
					object value = propertyInfo.GetValue(pdu, null);
					return (int)value;
				}
			}
			catch (Exception)
			{

			}
			return 0;
		}

		private static bool HasTpField(Pdu pdu, string fieldName)
		{
			try
			{
#if !NETFX_CORE
				MethodInfo method = pdu.GetType().GetMethod("Has" + fieldName);
#else
				MethodInfo method = pdu.GetType().GetRuntimeMethod("Has" + fieldName, new Type[0]);
#endif

				if (method != null)
				{
					object result = method.Invoke(pdu, null);
					return (bool)result;
				}
				return false;

			}
			catch (Exception)
			{
			}
			return false;
		}

		public static string DecodeFirstOctet(Pdu pdu)
		{
			// Use reflection to check for method signatures hasTpXXX() and getTpXXX()
			// that are specific to a particular type in actual object 
			StringBuilder sb = new StringBuilder();
			sb.Append("First Octet: " + PduUtils.ByteToPdu(pdu.FirstOctet));
			sb.Append(" [");
			// TP-MTI
			switch (pdu.TpMti)
			{

				case PduUtils.TpMtiSmsDeliver:
					sb.Append("TP-MTI: (SMS-DELIVER)");
					break;

				case PduUtils.TpMtiSmsStatusReport:
					sb.Append("TP-MTI: (SMS-STATUS REPORT)");
					break;

				case PduUtils.TpMtiSmsSubmit:
					sb.Append("TP-MTI: (SMS-SUBMIT)");
					break;

				default:
					throw new Exception("Invalid message type");

			}
			if (HasTpField(pdu, "TpMms"))
			{
				if (HasTpField(pdu, "TpMms"))
				{
					sb.Append(", TP-MMS: (Has more messages)");
				}
				else
				{
					sb.Append(", TP-MMS: (has no messages)");
				}
			}

			// TP-RD
			if (HasTpField(pdu, "TpRd"))
			{
				if (HasTpField(pdu, "TpRd"))
				{
					sb.Append(", TP-RD: (Reject duplicates)");
				}
				else
				{
					sb.Append(", TP-RD: (allow duplicates)");
				}
			}

			//TP-VPF
			if ((HasTpField(pdu, "TpVpf")) && ((Object)HasTpField(pdu, "TpVpf") != null))
			{
				switch (GetTpField(pdu, "TpVpf"))
				{

					case PduUtils.TpVpfInteger:
						sb.Append(", TP-VPF: (validity format, integer");
						break;

					case PduUtils.TpVpfTimestamp:
						sb.Append(", TP-VPF: (validity format, timestamp");
						break;

					case PduUtils.TpVpfNone:
						sb.Append(", TP-VPF: (validity format, none)");
						break;
				}
			}
			// TP-SRI
			if (HasTpField(pdu, "TpSri"))
			{
				if (HasTpField(pdu, "TpSri"))
				{
					sb.Append(", TP-SRI: (Requests Status Report)");
				}
				else
				{
					sb.Append(", TP-SRI: (No Status Report)");
				}
			}
			// TP-SRR
			if (HasTpField(pdu, "TpSrr"))
			{
				if (HasTpField(pdu, "TpSrr"))
				{
					sb.Append(", TP-SRR: (Requests Status Report)");
				}
				else
				{
					sb.Append(", TP-SRR: (No Status Report)");
				}
			}

			// TP-RP
			if (HasTpField(pdu, "TpRp"))
			{
				if (HasTpField(pdu, "TpRp"))
				{
					sb.Append(", TP-RP: (Reply Path)");
				}
				else
				{
					sb.Append(", TP-RP: (No Reply Path)");
				}
			}

			//TP-UDHI
			if (pdu.HasTpUdhi())
			{
				sb.Append(", TP-UDHI: (has UDH)");
			}
			else
			{
				sb.Append(", TP-UDHI: (no UDH)");
			}
			sb.Append("]");
			sb.Append("\r\n");
			return sb.ToString();
		}

		public static string DecodeDataCodingScheme(Pdu pdu)
		{
			StringBuilder sb = new StringBuilder();
			switch (PduUtils.ExtractDcsEncoding(pdu.DataCodingScheme))
			{

				case PduUtils.DcsEncoding7Bit:
					sb.Append("7-bit GSM Alphabet");
					break;

				case PduUtils.DcsEncoding8Bit:
					sb.Append("8-bit encoding");
					break;

				case PduUtils.DcsEncodingUcs2:
					sb.Append("UCS2 encoding");
					break;
			}
			// are flash messages are only applicable to general coding group?
			if ((pdu.DataCodingScheme & ~PduUtils.DcsCodingGroupGeneral) == 0)
			{
				if (PduUtils.ExtractDcsFlash(pdu.DataCodingScheme) == PduUtils.DcsMessageClassFlash)
				{
					sb.Append(", (Flash Message)");
				}
			}
			return sb.ToString();
		}

		public static byte[] Encode8bitUserData(string text)
		{
			try
			{
				return Encoding.GetEncoding("iso-8859-1").GetBytes(text);
			}
			catch (Exception e)
			{
				throw e;
			}
		}

		public static byte[] EncodeUcs2UserData(string text)
		{
			try
			{
				return Encoding.GetEncoding("UTF-16BE").GetBytes(text);
			}
			catch (Exception e)
			{
				throw e;
			}
		}

		public static byte[] Encode7bitUserData(byte[] udhOctets, byte[] textSeptets)
		{
			// UDH octets and text have to be encoded together in a single pass
			// UDH octets will need to be converted to unencoded septets in order
			// to properly pad the data
			if (udhOctets == null)
			{
				// convert string to uncompressed septets
				return UnencodedSeptetsToEncodedSeptets(textSeptets);
			}
			else
			{
				// convert UDH octets as if they were encoded septets
				// NOTE: DO NOT DISCARD THE LAST SEPTET IF IT IS ZERO
				byte[] udhSeptets = PduUtils.EncodedSeptetsToUnencodedSeptets(udhOctets, false);
				// combine the two arrays and encode them as a whole
				byte[] combined = new byte[udhSeptets.Length + textSeptets.Length];
				Array.Copy(udhSeptets, 0, combined, 0, udhSeptets.Length);
				Array.Copy(textSeptets, 0, combined, udhSeptets.Length, textSeptets.Length);
				// convert encoded byte[] to a PDU string
				return UnencodedSeptetsToEncodedSeptets(combined);
			}
		}

		public static string Decode7bitEncoding(byte[] encodedPduData)
		{
			//return Decode7bitEncoding(null, encodedPduData);
			return UnencodedSeptetsToString(EncodedSeptetsToUnencodedSeptets(encodedPduData));
		}

		public static string Decode7bitEncoding(byte[] udhData, byte[] encodedPduData)
		{
			int udhLength = ((udhData == null) ? 0 : udhData.Length);

			if (udhLength == 0)
			{
				// just process the whole pdu as one thing
				return UnencodedSeptetsToString(EncodedSeptetsToUnencodedSeptets(encodedPduData));
			}
			else
			{
				// 
				string decodedUdh = UnencodedSeptetsToString(EncodedSeptetsToUnencodedSeptets(udhData, false));
				string decoded = UnencodedSeptetsToString(EncodedSeptetsToUnencodedSeptets(encodedPduData));
				return decoded.Substring(decodedUdh.Length);
			}
		}

		public static string Decode8bitEncoding(byte[] udhData, byte[] pduData)
		{
			// standard 8-bit characters
			try
			{
				int udhLength = ((udhData == null) ? 0 : udhData.Length);

				string tempStr = Encoding.GetEncoding("iso-8859-1").GetString(pduData, 0, pduData.Length);
				return new string(tempStr.ToCharArray(), udhLength, pduData.Length - udhLength);
			}
			catch (Exception e)
			{
				throw e;
			}
		}

		public static string DecodeUcs2Encoding(byte[] udhData, byte[] pduData)
		{
			try
			{
				int udhLength = ((udhData == null) ? 0 : udhData.Length);
				string header = string.Empty;

				if (udhData != null)
				{
					byte[] paddedByteArray = PadByteArray(udhData);
					header = Encoding.GetEncoding("UTF-16BE").GetString(paddedByteArray, 0, paddedByteArray.Length);
				}

				// standard unicode
				if (udhData != null)
				{
					byte[] dataBytes = new byte[pduData.Length - udhData.Length];
					Array.Copy(pduData, udhData.Length, dataBytes, 0, dataBytes.Length);
					byte[] paddedByteArray = PadByteArray(dataBytes);
					string tempStr = Encoding.GetEncoding("UTF-16BE").GetString(paddedByteArray, 0, paddedByteArray.Length);
					char[] charArray = tempStr.ToCharArray();
					return new string(charArray);
				}
				else
				{
					byte[] paddedByteArray = PadByteArray(pduData);
					string tempStr = Encoding.GetEncoding("UTF-16BE").GetString(paddedByteArray, 0, paddedByteArray.Length);
					char[] charArray = tempStr.ToCharArray();
					return new string(charArray, header.Length, charArray.Length - header.Length);
				}

			}
			catch (Exception e)
			{
				throw e;
			}
		}


		public static byte SwapNibbles(int b)
		{
			return (byte)(((b << 4) & 0xF0) | ((URShift(b, 4)) & 0x0F));
		}

		public static string ReadBcdNumbers(int numDigits, byte[] addressData)
		{
			// reads length BCD numbers from the current position
			StringBuilder sb = new StringBuilder();
			for (int i = 0; i < addressData.Length; i++)
			{
				int b = addressData[i];
				int num1 = b & 0x0F;
				sb.Append(num1);
				int num2 = (URShift(b, 4)) & 0x0F;
				if (num2 != 0x0F)
				{
					// check if fillbits
					sb.Append(num2);
				}
			}
			return sb.ToString();
		}

		public static int CreateSwappedBcd(int dec)
		{
			// creates a swapped BCD representation of a 2-digit decimal
			int tens = (dec & 0xFF) / 10;
			int ones = (dec & 0xFF) - (tens * 10);
			return (ones << 4) | tens;
		}


		public static string StringToPdu(string txt)
		{
			byte[] txtBytes = StringToUnencodedSeptets(txt);
			return BytesToPdu(UnencodedSeptetsToEncodedSeptets(txtBytes));
		}

		// from string to uncompressed septets (GSM characters)
		public static byte[] StringToUnencodedSeptets(string s)
		{
			System.IO.MemoryStream baos = new System.IO.MemoryStream();
			int i, j, index;
			char ch;
            s = s.Replace('\u00C7', // LATIN CAPITAL LETTER C WITH CEDILLA
                       '\u00E7' // LATIN SMALL LETTER C WITH CEDILLA
			   );
			for (i = 0; i < s.Length; i++)
			{
				ch = s[i];
				index = -1;
				for (j = 0; j < ExtendedAlphabet.Length; j++)
					if (ExtendedAlphabet[j] == ch)
					{
						index = j;
						break;
					}
				if (index != -1)
				// An extended char...
				{
					baos.WriteByte((byte)Convert.ToInt32(ExtendedBytes[index].Substring(0, (2) - (0)), 16));
					baos.WriteByte((byte)Convert.ToInt32(ExtendedBytes[index].Substring(2, (4) - (2)), 16));
				}
				// Maybe a standard char...
				else
				{
					index = -1;
					for (j = 0; j < StandardAlphabets.Length; j++)
						if (StandardAlphabets[j] == ch)
						{
							index = j;
							baos.WriteByte((byte)j);
							break;
						}
					if (index == -1)
					// Maybe a Greek Char...
					{
						for (j = 0; j < GreekCharacterAlphabetRemapping.Length; j++)
							if (GreekCharacterAlphabetRemapping[j][0] == ch)
							{
								index = j;
								ch = GreekCharacterAlphabetRemapping[j][1];
								break;
							}
						if (index != -1)
						{
							for (j = 0; j < StandardAlphabets.Length; j++)
								if (StandardAlphabets[j] == ch)
								{
									index = j;
									baos.WriteByte((byte)j);
									break;
								}
						}
						// Unknown char replacement...
						else
						{
							baos.WriteByte((byte)' ');
						}
					}
				}
			}
			return baos.ToArray();
		}


		/*
		public static byte[] unencodedSeptetsToEncodedSeptets(byte[] septetBytes)
		{
			byte[] txtBytes;
			byte[] txtSeptets;
			int txtBytesLen;
			BitSet bits;
			int i, j;
			txtBytes = septetBytes;
			txtBytesLen = txtBytes.length;
			bits = new BitSet();
			for (i = 0; i < txtBytesLen; i++)
				for (j = 0; j < 7; j++)
					if ((txtBytes[i] & (1 << j)) != 0) bits.set((i * 7) + j);
			// big diff here
			int encodedSeptetByteArrayLength = txtBytesLen * 7 / 8 + ((txtBytesLen * 7 % 8 != 0) ? 1 : 0);
			txtSeptets = new byte[encodedSeptetByteArrayLength];
			for (i = 0; i < encodedSeptetByteArrayLength; i++)
			{
				for (j = 0; j < 8; j++)
				{
					txtSeptets[i] |= (byte)((bits.get((i * 8) + j) ? 1 : 0) << j);
				}
			}
			return txtSeptets;
		}
		*/

		// from compress unencoded septets
		public static byte[] UnencodedSeptetsToEncodedSeptets(byte[] septetBytes)
		{
			byte[] txtBytes;
			byte[] txtSeptets;
			int txtBytesLen;
			BitArray bits;
			int i, j;
			txtBytes = septetBytes;
			txtBytesLen = txtBytes.Length;
			bits = new BitArray(128);
			for (i = 0; i < txtBytesLen; i++)
				for (j = 0; j < 7; j++)
					if ((txtBytes[i] & (1 << j)) != 0)
						Set(bits, (i * 7) + j);
			// big diff here
			int encodedSeptetByteArrayLength = txtBytesLen * 7 / 8 + ((txtBytesLen * 7 % 8 != 0) ? 1 : 0);
			txtSeptets = new byte[encodedSeptetByteArrayLength];
			for (i = 0; i < encodedSeptetByteArrayLength; i++)
			{
				for (j = 0; j < 8; j++)
				{
					try
					{
						txtSeptets[i] |= (byte)((bits.Get((i * 8) + j) ? 1 : 0) << j);
					}
					catch (Exception) { }
				}
			}
			return txtSeptets;
		}

		/// <summary>
		/// Sets the specified bit to true.
		/// </summary>
		/// <param name="bits">The BitArray to modify.</param>
		/// <param name="index">The bit index to set to true.</param>
		public static void Set(BitArray bits, int index)
		{
			for (int increment = 0; index >= bits.Length; increment = +128)
			{
				bits.Length += increment;
			}

			bits.Set(index, true);
		}

		public static int GetNumMultiCharsInSeptets(byte[] bytes)
		{
			int count = 0;
			int i;
			for (i = 0; i < bytes.Length; i++)
			{
				if (bytes[i] == 0x1b)
				{
					count++;
				}
			}
			return count;
		}


		// from GSM characters to string
		public static string UnencodedSeptetsToString(byte[] bytes)
		{
			StringBuilder text;
			string extChar;
			int i, j;
			text = new StringBuilder();
			for (i = 0; i < bytes.Length; i++)
			{
				if (bytes[i] == 0x1b)
				{
					// NOTE: - ++i can be a problem if the '1b'
					//         is right at the end of a PDU
					//       - this will be an issue for displaying 
					//         partial PDUs e.g. via toString()
					if (i < bytes.Length - 1)
					{
						extChar = "1b" + Convert.ToString(bytes[++i], 16);
						for (j = 0; j < ExtendedBytes.Length; j++)
							if (ExtendedBytes[j].ToUpper().Equals(extChar.ToUpper()))
								text.Append(ExtendedAlphabet[j]);
					}
				}
				else
				{
					text.Append(StandardAlphabets[bytes[i]]);
				}
			}
			return text.ToString();
		}

		public static string EncodedSeptetsToString(byte[] encodedSeptets)
		{
			return UnencodedSeptetsToString(EncodedSeptetsToUnencodedSeptets(encodedSeptets));
		}

		public static int GetNumSeptetsForOctets(int numOctets)
		{
			return numOctets * 8 / 7 + ((numOctets * 8 % 7 != 0) ? 1 : 0);

			//return numOctets + (numOctets/7);
		}

		// decompress encoded septets to unencoded form
		public static byte[] EncodedSeptetsToUnencodedSeptets(byte[] octetBytes)
		{
			return EncodedSeptetsToUnencodedSeptets(octetBytes, true);
		}

		public static byte[] EncodedSeptetsToUnencodedSeptets(byte[] octetBytes, bool discardLast)
		{
			byte[] newBytes;
			BitArray bitSet;
			int i, j, value1, value2;
			bitSet = new BitArray((octetBytes.Length * 8 % 64 == 0 ? octetBytes.Length * 8 / 64 : octetBytes.Length * 8 / 64 + 1) * 64);
			value1 = 0;
			for (i = 0; i < octetBytes.Length; i++)
				for (j = 0; j < 8; j++)
				{
					value1 = (i * 8) + j;
					if ((octetBytes[i] & (1 << j)) != 0)
						Set(bitSet, value1);
				}
			value1++;
			// this is a bit count NOT a byte count
			value2 = value1 / 7 + ((value1 % 7 != 0) ? 1 : 0); // big diff here
			if (value2 == 0)
				value2++;
			newBytes = new byte[value2];
			for (i = 0; i < value2; i++)
			{
				for (j = 0; j < 7; j++)
				{
					if ((value1 + 1) > (i * 7 + j))
					{
						int index = i * 7 + j;
						if (index < bitSet.Length)
						{
							if (bitSet.Get(index))
							{
								newBytes[i] |= (byte)(1 << j);
							}
						}
					}
				}
			}
			if (discardLast && octetBytes.Length * 8 % 7 > 0)
			{
				// when decoding a 7bit encoded string 
				// the last septet may become 0, this should be discarded
				// since this is an artifact of the encoding not part of the 
				// original string
				// this is only done for decoding 7bit encoded text NOT for
				// reversing octets to septets (e.g. for the encoding the UDH)
				if (newBytes[newBytes.Length - 1] == 0)
				{
					byte[] retVal = new byte[newBytes.Length - 1];
					Array.Copy(newBytes, 0, retVal, 0, retVal.Length);
					return retVal;
				}
			}
			return newBytes;
		}

		// converts a PDU style string to a byte array
		public static byte[] PduToBytes(string s)
		{
			byte[] bytes = new byte[s.Length / 2];
			for (int i = 0; i < s.Length; i += 2)
			{
				int upper = (i + 2) - (i);
				if ((i + upper) > s.Length) upper = s.Length - i;
				if ((i / 2) < bytes.Length)
					bytes[i / 2] = (byte)(Convert.ToInt32(s.Substring(i, upper), 16));
			}
			return bytes;
		}

		// converts a PDU style string to a bit string
		public static string PduToBits(string pduString)
		{
			return BytesToBits(PduToBytes(pduString));
		}

		// converts a byte array to PDU style string
		public static string BytesToPdu(byte[] bytes)
		{
			StringBuilder sb = new StringBuilder();
			for (int i = 0; i < bytes.Length; i++)
			{
				sb.Append(ByteToPdu(bytes[i] & 0xFF));
			}
			return sb.ToString();
		}

		// converts a byte array to a bit string
		public static string BytesToBits(byte[] b)
		{
			StringBuilder sb = new StringBuilder();
			for (int i = 0; i < b.Length; i++)
			{
				string bits = Convert.ToString(b[i] & 0xFF, 2);
				while (bits.Length < 8)
				{
					bits = "0" + bits;
				}
				if (i > 0)
				{
					sb.Append(" ");
				}
				sb.Append(bits);
			}
			return sb.ToString();
		}

		public static string ByteToBits(byte b)
		{
			string bits = Convert.ToString(b & 0xFF, 2);
			while (bits.Length < 8)
			{
				bits = "0" + bits;
			}
			return bits;
		}

		public static string ByteToPdu(int b)
		{
			StringBuilder sb = new StringBuilder();
			string s = Convert.ToString(b & 0xFF, 16);
			if (s.Length == 1)
			{
				sb.Append("0");
			}
			sb.Append(s);
			return sb.ToString().ToUpper();
		}


		/// <summary>
		/// Performs an unsigned bitwise right shift with the specified number
		/// </summary>
		/// <param name="number">Number to operate on</param>
		/// <param name="bits">Ammount of bits to shift</param>
		/// <returns>The resulting number from the shift operation</returns>
		public static int URShift(int number, int bits)
		{
			if (number >= 0)
				return number >> bits;
			else
				return (number >> bits) + (2 << ~bits);
		}


		private static byte[] PadByteArray(byte[] byteArray)
		{
			if ((byteArray.Length % 2) != 0)
			{
				byte[] tempByteArray = new byte[byteArray.Length + 1];
				for (int i = 0; i < byteArray.Length; i++)
				{
					tempByteArray[i] = byteArray[i];
				}
				return tempByteArray;
			}
			return byteArray;
		}
	}
}