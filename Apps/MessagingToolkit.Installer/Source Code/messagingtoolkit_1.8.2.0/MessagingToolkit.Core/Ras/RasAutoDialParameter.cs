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

    /// <summary>
    /// Defines the remote access service (RAS) AutoDial parameters.
    /// </summary>
    public enum RasAutoDialParameter
    {
        /// <summary>
        /// Causes AutoDial to disable a dialog box displayed to the user before dialing a connection.
        /// </summary>
        DisableConnectionQuery = 0,

        /// <summary>
        /// Causes the system to disable all AutoDial connections for the current logon session.
        /// </summary>
        LogOnSessionDisable,

        /// <summary>
        /// Indicates the maximum number of addresses that AutoDial stores in the registry.
        /// </summary>
        SavedAddressesLimit,

        /// <summary>
        /// Indicates a timeout value (in seconds) when an AutoDial connection attempt fails before another connection attempt begins.
        /// </summary>
        FailedConnectionTimeout,

        /// <summary>
        /// Indicates a timeout value (in seconds) when the system displays a dialog box asking the user to confirm that the system should dial.
        /// </summary>
        ConnectionQueryTimeout
    }
}