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

using MessagingToolkit.Core.Base;

namespace MessagingToolkit.Core.Mobile
{
    /// <summary>
    /// Gateway network registration status
    /// </summary>
    public class NetworkRegistration: BaseClass<NetworkRegistration>
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public NetworkRegistration()
        {
            this.UnsolicitedResultCode = NetworkCapability.EnabledNetworkRegistration;
            this.Status = NetworkRegistrationStatus.Unknown;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="resultCode">Unsolicited result code</param>
        /// <param name="status">Network registration status</param>
        public NetworkRegistration(NetworkCapability resultCode, NetworkRegistrationStatus status)
        {
            this.UnsolicitedResultCode = resultCode;
            this.Status = status;
        }

        /// <summary>
        /// Unsolicited result code display
        /// </summary>
        /// <value>Result code</value>
        public NetworkCapability UnsolicitedResultCode
        {
            get;
            internal set;
        }

        /// <summary>
        /// Network registration status
        /// </summary>
        /// <value>Network registration status</value>
        public NetworkRegistrationStatus Status
        {
            get;
            internal set;
        }

    }
}
