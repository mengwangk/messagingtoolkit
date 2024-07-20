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
    /// USSD response queue
    /// </summary>
    internal sealed class UssdResponseQueue: Queue<UssdResponse>
    {
        private object syncLock;

        /// <summary>
        /// Initializes a new instance of the <see cref="UssdResponseQueue"/> class.
        /// </summary>
        public UssdResponseQueue(): base()
        {
            syncLock = new object();
        }

        /// <summary>
        /// Adds the specified ussd response.
        /// </summary>
        /// <param name="ussdResponse">The ussd response.</param>
        public void Add(UssdResponse ussdResponse)
        {
            lock (syncLock)
            {
                Enqueue(ussdResponse);
            }
        }

        /// <summary>
        /// Removes this instance.
        /// </summary>
        /// <returns></returns>
        public UssdResponse Remove()
        {
            lock (syncLock)
            {
                return Dequeue();
            }
        }
    }
}
