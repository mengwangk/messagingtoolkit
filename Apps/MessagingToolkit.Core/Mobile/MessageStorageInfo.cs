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
    /// Message storage information
    /// </summary>
    public class MessageStorageInfo
    {
        /// <summary>
        /// </summary>
        /// <value></value>
        public string[] ReadStorages
        {
            get;
            set;
        }

        /// <summary>
        /// </summary>
        /// <value></value>
        public string[] WriteStorages
        {
            get;
            set;
        }

        /// <summary>
        /// </summary>
        /// <value></value>
        public string[] ReceiveStorages
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
            str = String.Concat(str, "ReadStorages = ", ReadStorages, "\r\n");
            str = String.Concat(str, "WriteStorages = ", WriteStorages, "\r\n");
            str = String.Concat(str, "ReceiveStorages = ", ReceiveStorages, "\r\n");
            return str;
        }

    }
}
