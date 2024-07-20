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

#if (WINXP || WIN2K8 || WIN7)

    /// <summary>
    /// Defines the pre-shared keys.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>Known Limitations:</b>
    /// <list type="bullet">
    /// <item>This type is only available on Windows XP and later operating systems.</item>
    /// </list>
    /// </para>
    /// </remarks>
    public enum RasPreSharedKey
    {
        /// <summary>
        /// The client pre-shared key.
        /// </summary>
        Client,

        /// <summary>
        /// The server pre-shared key.
        /// </summary>
        Server,

        /// <summary>
        /// The demand-dial interface (DDI) pre-shared key.
        /// </summary>
        Ddm
    }

#endif
}