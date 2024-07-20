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
    /// Storage memory status
    /// </summary>
    public class MemoryStatusWithStorage: MemoryStatus
    {        
        private string storage;
        
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="storage"></param>
        /// <param name="used"></param>
        /// <param name="total"></param>
        public MemoryStatusWithStorage(string storage, int used, int total) : base(used, total)
        {
            this.storage = storage;
        }

        /// <summary>
        /// </summary>
        /// <value></value>
        public string Storage
        {
            get
            {
                return this.storage;
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
            str = String.Concat(str, "Total = ", Total, "\r\n");
            str = String.Concat(str, "Used = ", Used, "\r\n");
            str = String.Concat(str, "Storage = ", Storage, "\r\n");
            return str;
        }
    }
}
