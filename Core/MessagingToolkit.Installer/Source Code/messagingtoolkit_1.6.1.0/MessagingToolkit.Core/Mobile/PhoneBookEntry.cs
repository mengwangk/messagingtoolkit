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
using System.Xml.Serialization;

using MessagingToolkit.Core.Mobile.Message;

namespace MessagingToolkit.Core.Mobile
{
    /// <summary>
    /// Phone book entry
    /// </summary>
    [Serializable]
    public class PhoneBookEntry
    {
        private int index;
        private string number;
        private string text;
        private int type;

        /// <summary>
        /// Constructor
        /// </summary>
        public PhoneBookEntry()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="entry"></param>
        public PhoneBookEntry(PhoneBookEntry entry)
        {
            this.index = entry.Index;
            this.number = entry.Number;
            this.type = entry.type;
            this.text = entry.Text;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="index"></param>
        /// <param name="number"></param>
        /// <param name="type"></param>
        /// <param name="text"></param>
        public PhoneBookEntry(int index, string number, int type, string text)
        {
            this.index = index;
            this.number = number;
            this.type = type;
            this.text = text;
        }

        /// <summary>
        /// Phone book entry index
        /// </summary>
        /// <value></value>
        [XmlAttribute]
        public int Index
        {
            get
            {
                return this.index;
            }
            set
            {
                this.index = value;
            }
        }

        /// <summary>
        /// Phone book entry number
        /// </summary>
        /// <value></value>
        [XmlAttribute]
        public string Number
        {
            get
            {
                return this.number;
            }
            set
            {
                this.number = value;
            }
        }

        /// <summary>
        /// Phone book entry text
        /// </summary>
        /// <value></value>
        [XmlAttribute]
        public string Text
        {
            get
            {
                return this.text;
            }
            set
            {
                this.text = value;
            }
        }

        /// <summary>
        /// Phone number type. 
        /// <code>
        /// 129 - Domestic
        /// 145 - International
        /// </code>
        /// See <see cref="NumberType"/>
        /// </summary>
        /// <value></value>
        [XmlAttribute]
        public int Type
        {
            get
            {
                return this.type;
            }
            set
            {
                this.type = value;
            }
        }
    }
}
