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
    /// Received message
    /// </summary>
    public class ReceivedMessage : IIndicationObject
    {
        // Fields
        private string alpha;
        private string data;
        private int length;
             
        /// <summary>
        /// Initializes a new instance of the <see cref="ReceivedMessage"/> class.
        /// </summary>
        public ReceivedMessage()
        {
            this.alpha = string.Empty;
            this.length = 0;
            this.data = string.Empty;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReceivedMessage"/> class.
        /// </summary>
        /// <param name="alpha">The alpha.</param>
        /// <param name="length">The length.</param>
        /// <param name="data">The data.</param>
        public ReceivedMessage(string alpha, int length, string data)
        {
            this.Alpha = alpha;
            this.Length = length;
            this.Data = data;
        }


        /// <summary>
        /// Gets or sets the alpha.
        /// </summary>
        /// <value>The alpha.</value>
        public string Alpha
        {
            get
            {
                return this.alpha;
            }
            set
            {
                this.alpha = value;
            }
        }

        /// <summary>
        /// Gets or sets the data.
        /// </summary>
        /// <value>The data.</value>
        public string Data
        {
            get
            {
                return this.data;
            }
            set
            {
                this.data = value;
            }
        }


        /// <summary>
        /// Gets or sets the length.
        /// </summary>
        /// <value>The length.</value>
        public int Length
        {
            get
            {
                return this.length;
            }
            set
            {
                this.length = value;
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
            return str;
        }
    }
}
