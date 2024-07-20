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
using System.Runtime.Serialization;
using System.Text;

namespace MessagingToolkit.Core.Mobile.Http
{
    /// <summary>
    /// Cell location on a GSM phone.
    /// </summary>
    [DataContract]
    public sealed class GsmCellLocation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GsmCellLocation"/> class.
        /// </summary>
        public GsmCellLocation()
        {
            this.Cid = -1;
            this.Lac = -1;
            this.Psc = -1;
        }

        /// <summary>
        /// Gets or sets the cid.
        /// </summary>
        /// <value>
        /// GSM cell id, -1 if unknown, 0xffff max legal value.
        /// </value>
        [DataMember(Name = "cid")]
        public int Cid { get; set; }

        /// <summary>
        /// GSM location area code, -1 if unknown, 0xffff max legal value
        /// </summary>
        /// <value>
        /// The lac.
        /// </value>
        [DataMember(Name = "lac")]
        public int Lac { get; set; }

        /// <summary>
        /// Gets or sets the PSC.
        /// </summary>
        /// <value>
        /// Primary scrambling code for UMTS, -1 if unknown or GSM.
        /// </value>
        [DataMember(Name = "psc")]
        public int Psc { get; set; }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        override public string ToString()
        {
            string str = String.Empty;
            str = String.Concat(str, "Cid = ", Cid, "\r\n");
            str = String.Concat(str, "Lac = ", Lac, "\r\n");
            str = String.Concat(str, "Psc = ", Psc, "\r\n");
            return str;
        }
    }
}
