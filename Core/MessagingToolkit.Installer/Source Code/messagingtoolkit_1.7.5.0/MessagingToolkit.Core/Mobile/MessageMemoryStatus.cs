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
    /// Message memory status
    /// </summary>
    public class MessageMemoryStatus
    {       
        private MemoryStatus readStorage;
        private MemoryStatus receiveStorage;
        private MemoryStatus writeStorage;


        /// <summary>
        /// Constructor
        /// </summary>
        public MessageMemoryStatus()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="readStorage"></param>
        /// <param name="writeStorage"></param>
        /// <param name="receiveStorage"></param>
        public MessageMemoryStatus(MemoryStatus readStorage, MemoryStatus writeStorage, MemoryStatus receiveStorage)
        {
            this.readStorage = readStorage;
            this.writeStorage = writeStorage;
            this.receiveStorage = receiveStorage;
        }

        /// <summary>
        /// </summary>
        /// <value></value>
        public MemoryStatus ReadStorage
        {
            get
            {
                return this.readStorage;
            }
            set
            {
                this.readStorage = value;
            }
        }

        /// <summary>
        /// </summary>
        /// <value></value>
        public MemoryStatus ReceiveStorage
        {
            get
            {
                return this.receiveStorage;
            }
            set
            {
                this.receiveStorage = value;
            }
        }

        /// <summary>
        /// </summary>
        /// <value></value>
        public MemoryStatus WriteStorage
        {
            get
            {
                return this.writeStorage;
            }
            set
            {
                this.writeStorage = value;
            }
        }
    }
}
