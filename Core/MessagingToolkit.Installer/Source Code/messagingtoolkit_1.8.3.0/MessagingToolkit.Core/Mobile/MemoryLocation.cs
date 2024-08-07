﻿//===============================================================================
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
    /// Message memory location
    /// </summary>
    public class MemoryLocation : IIndicationObject
    {        
        private int index;
        private string storage;
        private MessageNotification notificationType;

        /// <summary>
        /// Initializes a new instance of the <see cref="MemoryLocation" /> class.
        /// </summary>
        /// <param name="notificationType">Type of the notification.</param>
        /// <param name="storage">The storage.</param>
        /// <param name="index">The index.</param>
        public MemoryLocation(MessageNotification notificationType, string storage, int index)
        {
            this.notificationType = notificationType;
            this.storage = storage;
            this.index = index;
        }

        /// <summary>
        /// Message index
        /// </summary>
        /// <value>Index</value>
        public int Index
        {
            get
            {
                return this.index;
            }
        }

        /// <summary>
        /// Gets the storage.
        /// </summary>
        /// <value>The storage.</value>
        public string Storage
        {
            get
            {
                return this.storage;
            }
        }

        /// <summary>
        /// Gets the type of the notification.
        /// </summary>
        /// <value>
        /// The type of the notification.
        /// </value>
        public MessageNotification NotificationType
        {
            get
            {
                return this.notificationType;
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
            str = String.Concat(str, "Index = ", Index, "\r\n");
            str = String.Concat(str, "Storage = ", Storage, "\r\n");
            str = String.Concat(str, "NotificationType = ", NotificationType, "\r\n");
            return str;
        }
    }
}

