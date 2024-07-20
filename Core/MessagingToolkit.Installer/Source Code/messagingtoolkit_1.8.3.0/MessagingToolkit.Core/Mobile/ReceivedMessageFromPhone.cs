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
    /// Received message from phone
    /// </summary>
    public class ReceivedMessageFromPhone : ReceivedMessage
    {
        // Fields
        private int index;
        private int status;

    
        /// <summary>
        /// Initializes a new instance of the <see cref="ReceivedMessageFromPhone"/> class.
        /// </summary>
        public ReceivedMessageFromPhone()
        {
            this.index = 0;
            this.status = 0;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReceivedMessageFromPhone"/> class.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="status">The status.</param>
        /// <param name="alpha">The alpha.</param>
        /// <param name="length">The length.</param>
        /// <param name="data">The data.</param>
        public ReceivedMessageFromPhone(int index, int status, string alpha, int length, string data)
            : base(alpha, length, data)
        {
            this.Index = index;
            this.Status = status;
        }

        /// <summary>
        /// Gets or sets the index.
        /// </summary>
        /// <value>The index.</value>
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
        /// Gets or sets the status.
        /// </summary>
        /// <value>The status.</value>
        public int Status
        {
            get
            {
                return this.status;
            }
            set
            {
                this.status = value;
            }
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
            str = String.Concat(str, "Alpha = ", Alpha, "\r\n");
            str = String.Concat(str, "Data = ", Data, "\r\n");
            str = String.Concat(str, "Length = ", Length, "\r\n");
            str = String.Concat(str, "Index = ", Index, "\r\n");
            str = String.Concat(str, "Status = ", Status, "\r\n");
            return str;
        }
    }
}


 

 
