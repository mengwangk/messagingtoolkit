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
    /// PDP data connection
    /// </summary>
    public class PdpConnection
    {

        /// <summary>
        /// TRUE indicator
        /// </summary>
        public const string EnabledFlag = "1";

        /// <summary>
        /// False indicator
        /// </summary>
        public const string DisabledFlag = "0";

        /// <summary>
        /// Default PDP address
        /// </summary>
        public const string DefaultPdpAddress = "0.0.0.0";

        /// <summary>
        /// Default compression control value.
        /// </summary>
        public const string DefaultCompressionControlValue = DisabledFlag;

        /// <summary>
        /// Initializes a new instance of the <see cref="PdpConnection"/> class.
        /// </summary>
        public PdpConnection()
        {
            PdpAddress = DefaultPdpAddress;
            DataCompressionControl = DefaultCompressionControlValue;
            HeaderCompressionControl = DefaultCompressionControlValue;
        }


        /// <summary>
        /// Gets or sets the context id.
        /// </summary>
        /// <value>The context id.</value>
        public int ContextId
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the type of the PDP.
        /// </summary>
        /// <value>The type of the PDP.</value>
        public string PdpType
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the APN.
        /// </summary>
        /// <value>The APN.</value>
        public string APN
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the PDP address.
        /// </summary>
        /// <value>The PDP address.</value>
        public string PdpAddress
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the data compression control.
        /// </summary>
        /// <value>The data compression control.</value>
        public string DataCompressionControl
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the header compression control.
        /// </summary>
        /// <value>The header compression control.</value>
        public string HeaderCompressionControl
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
            str = String.Concat(str, "ContextId = ", ContextId, "\r\n");
            str = String.Concat(str, "PdpType = ", PdpType, "\r\n");
            str = String.Concat(str, "APN = ", APN, "\r\n");
            str = String.Concat(str, "PdpAddress = ", PdpAddress, "\r\n");
            str = String.Concat(str, "DataCompressionControl = ", DataCompressionControl, "\r\n");
            str = String.Concat(str, "HeaderCompressionControl = ", HeaderCompressionControl, "\r\n");
            return str;
        }
    }
}
