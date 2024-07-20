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

namespace MessagingToolkit.Core.Mobile
{

    /// <summary>
    /// Enum representing the status of a GSM Unstructured Supplemental Service Data
    /// (USSD) data coding schemd (DCS).
    /// </summary>
    public class UssdDcs
    {

        // Bits 7..4 equal to zero: GSM 7 bit default alphabet Bits 3..0 indicate
        // the language       

        /// <summary>
        /// 
        /// </summary>
        public static readonly UssdDcs German7Bit = new UssdDcs(0x00);
        /// <summary>
        /// 
        /// </summary>
        public static readonly UssdDcs English7Bit = new UssdDcs(0x01);
        /// <summary>
        /// 
        /// </summary>
        public static readonly UssdDcs Italian7Bit = new UssdDcs(0x02);
        /// <summary>
        /// 
        /// </summary>
        public static readonly UssdDcs French7Bit = new UssdDcs(0x03);
        /// <summary>
        /// 
        /// </summary>
        public static readonly UssdDcs Spanish7Bit = new UssdDcs(0x04);
        /// <summary>
        /// 
        /// </summary>
        public static readonly UssdDcs Dutch7Bit = new UssdDcs(0x05);
        /// <summary>
        /// 
        /// </summary>
        public static readonly UssdDcs Swedish7Bit = new UssdDcs(0x06);
        /// <summary>
        /// 
        /// </summary>
        public static readonly UssdDcs Danish7Bit = new UssdDcs(0x07);
        /// <summary>
        /// 
        /// </summary>
        public static readonly UssdDcs Portuguese7Bit = new UssdDcs(0x08);
        /// <summary>
        /// 
        /// </summary>
        public static readonly UssdDcs Finnish7Bit = new UssdDcs(0x09);
        /// <summary>
        /// 
        /// </summary>
        public static readonly UssdDcs Norwegian7Bit = new UssdDcs(0x0a);
        /// <summary>
        /// 
        /// </summary>
        public static readonly UssdDcs Greek7Bit = new UssdDcs(0x0b);
        /// <summary>
        /// 
        /// </summary>
        public static readonly UssdDcs Turkish7Bit = new UssdDcs(0x0c);
        /// <summary>
        /// 
        /// </summary>
        public static readonly UssdDcs Hungarian7Bit = new UssdDcs(0x0d);
        /// <summary>
        /// 
        /// </summary>
        public static readonly UssdDcs Polish7Bit = new UssdDcs(0x0e);
        /// <summary>
        /// 
        /// </summary>
        public static readonly UssdDcs Unspecified7Bit = new UssdDcs(0x0f);


        // Bits 7..4 = 0001 --


        
    
        /// <summary>
        /// Bits 3..0 0000: GSM 7 bit default alphabet; message preceded by language
        /// indication. The first 3 characters of the message are a two-character
        /// representation of the language encoded according to ISO 639 [12],
        /// followed by a CR character. The CR character is then followed by 90
        /// characters of text.     
        /// </summary>
        public static readonly UssdDcs LanguageInPrefix7Bit = new UssdDcs(0x10);

        /// <summary>
        /// UCS2 -  message preceded by language indication The message starts with a
        /// two GSM 7-bit default alphabet character representation of the language
        /// encoded according to ISO 639 [12]. This is padded to the octet boundary
        /// with two bits set to 0 and then followed by 40 characters of UCS2-encoded
        /// message. An MS not supporting UCS2 coding will present the two character
        /// language identifier followed by improperly interpreted user data.
        /// </summary>
        public static readonly UssdDcs LanguageInPrefixUcs2 = new UssdDcs(0x11);

        // Bits 7..4 = 0010 --

        /// <summary>
        /// 
        /// </summary>
        public static readonly UssdDcs Czech = new UssdDcs(0x20);
        /// <summary>
        /// 
        /// </summary>
        public static readonly UssdDcs Hebrew = new UssdDcs(0x21);
        /// <summary>
        /// 
        /// </summary>
        public static readonly UssdDcs Arabic = new UssdDcs(0x22);
        /// <summary>
        /// 
        /// </summary>
        public static readonly UssdDcs Russian = new UssdDcs(0x23);
        /// <summary>
        /// 
        /// </summary>
        public static readonly UssdDcs Icelandic = new UssdDcs(0x24);


        /// <summary>
        /// Gets the values.
        /// </summary>
        /// <value>The values.</value>
        public static IEnumerable<UssdDcs> Values
        {
            get
            {
                yield return German7Bit;
                yield return English7Bit;
                yield return Italian7Bit;
                yield return French7Bit;
                yield return Spanish7Bit;
                yield return Dutch7Bit;
                yield return Swedish7Bit;
                yield return Danish7Bit;
                yield return Portuguese7Bit;

                yield return Finnish7Bit;
                yield return Norwegian7Bit;
                yield return Greek7Bit;
                yield return Turkish7Bit;
                yield return Hungarian7Bit;
                yield return Polish7Bit;
                yield return Unspecified7Bit;
                yield return LanguageInPrefix7Bit;
                yield return LanguageInPrefixUcs2;

                yield return Czech;
                yield return Hebrew;
                yield return Arabic;
                yield return Russian;
                yield return Icelandic;
            }
        }

        /// Further encodings not implemented ///
        private int numeric;

        /// <summary>
        /// Initializes a new instance of the <see cref="UssdDcs"/> class.
        /// </summary>
        /// <param name="aNumeric">A numeric.</param>
        UssdDcs(int aNumeric)
        {
            numeric = aNumeric;
        }

        /// <summary>
        /// Gets the numeric.
        /// </summary>
        /// <value>The numeric.</value>
        public int Numeric
        {
            get
            {
                return numeric;
            }
        }


        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return base.ToString() + " (" + numeric + ")";
        }

        /// <summary>
        /// Gets the by numeric.
        /// </summary>
        /// <param name="aNumeric">A numeric.</param>
        /// <returns></returns>
        public static UssdDcs GetByNumeric(int aNumeric)
        {
            foreach (UssdDcs dcs in UssdDcs.Values)
            {
                if (aNumeric == dcs.Numeric) return dcs;
            }
            return null;
        }
    }

}

