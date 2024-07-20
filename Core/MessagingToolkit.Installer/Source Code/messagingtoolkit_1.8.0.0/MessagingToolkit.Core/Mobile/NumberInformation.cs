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

using MessagingToolkit.Core.Base;

namespace MessagingToolkit.Core.Mobile
{
    /// <summary>
    /// Address information. E.g. SMSC address information
    /// </summary>
    public class NumberInformation : BaseClass<NumberInformation>
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public NumberInformation()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="number">Number</param>
        /// <param name="numberType">Number type</param>
        public NumberInformation(string number, NumberType numberType)
        {
            this.Number = number;
            this.NumberType = numberType;
        }
        
        /// <summary>
        /// Address value
        /// </summary>
        /// <value>Address value</value>
        public string Number
        {
            get;
            set;
        }

        /// <summary>
        /// Address type
        /// </summary>
        /// <value>Address type</value>
        public NumberType NumberType
        {
            get;
            set;
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        override public string ToString()
        {
            string str = String.Empty;
            str = String.Concat(str, "Number = ", Number, "\r\n");
            str = String.Concat(str, "NumberType = ", NumberType, "\r\n");
            return str;
        }
    }
}
