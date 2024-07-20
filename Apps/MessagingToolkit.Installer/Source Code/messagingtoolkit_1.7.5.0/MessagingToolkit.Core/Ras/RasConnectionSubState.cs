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

namespace MessagingToolkit.Core.Ras
{
    using System;
    using MessagingToolkit.Core.Ras.Internal;

#if (WIN7)

    /// <summary>
    /// Defines the states for Internet Key Exchange version 2 (IKEv2) virtual private network (VPN) tunnel connections.
    /// </summary>
    /// <remarks>These states are not available to other tunneling protocols.</remarks>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1027:MarkEnumsWithFlags", Justification = "Values are defined in the Windows SDK.")]
    public enum RasConnectionSubState
    {
        /// <summary>
        /// The connection state does not have a substate.
        /// </summary>
        None = 0,

        /// <summary>
        /// The underlying internet interface of the connection is down and the connection is waiting for an internet interface to come online.
        /// </summary>
        Dormant,

        /// <summary>
        /// The internet interface has come online and the connection is switching over to this new interface.
        /// </summary>
        Reconnecting,

        /// <summary>
        /// The connection has switched over successfully to the new internet interface.
        /// </summary>
        Reconnected = NativeMethods.RASCSS_DONE
    }

#endif
}