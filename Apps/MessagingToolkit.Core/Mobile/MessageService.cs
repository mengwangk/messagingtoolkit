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
    /// Message service
    /// </summary>
    public class MessageService
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="mt">The mt.</param>
        /// <param name="mo">The mo.</param>
        /// <param name="bm">The bm.</param>
        public MessageService(int service, int mt, int mo, int bm)
        {
            this.Service = service;
            this.MobileTerminating = mt;
            this.MobileOriginating = mo;         
            this.BroadcastMessage = bm;
        }

        /// <summary>
        /// </summary>
        /// <value></value>
        public int Service
        {
            get;
            private set;
        }

        /// <summary>
        /// </summary>
        /// <value></value>
        public int MobileTerminating
        {
            get;
            private set;
        }

        /// <summary>
        /// </summary>
        /// <value></value>
        public int MobileOriginating
        {
            get;
            private set;
        }

        /// <summary>
        /// </summary>
        /// <value></value>
        public int BroadcastMessage
        {
            get;
            private set;
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
            str = String.Concat(str, "Service = ", Service, "\r\n");
            str = String.Concat(str, "MobileTerminating = ", MobileTerminating, "\r\n");
            str = String.Concat(str, "MobileOriginating = ", MobileOriginating, "\r\n");
            str = String.Concat(str, "BroadcastMessage = ", BroadcastMessage, "\r\n");
            return str;
        }
    }
}
