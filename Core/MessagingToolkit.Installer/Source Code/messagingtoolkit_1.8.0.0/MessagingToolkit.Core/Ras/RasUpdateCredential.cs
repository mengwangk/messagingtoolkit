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
    /// Defines where user credentials can be saved for a phone book.
    /// </summary>
    public enum RasUpdateCredential
    {
        /// <summary>
        /// No credentials should be updated.
        /// </summary>
        None = 0,

        /// <summary>
        /// Update the credentials in the current user profile.
        /// </summary>
        User,

#if (WINXP || WIN2K8 || WIN7)

        /// <summary>
        /// Update the credentials in the all users profile, if available.
        /// <para>
        /// <b>Windows XP or later:</b> This value is supported.
        /// </para>
        /// </summary>
        AllUsers

#endif
    }
}