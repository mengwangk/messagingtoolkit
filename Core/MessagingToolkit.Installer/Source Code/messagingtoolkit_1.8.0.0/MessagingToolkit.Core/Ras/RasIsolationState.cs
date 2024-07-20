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

#if (WIN2K8 || WIN7)

    /// <summary>
    /// Describes the the isolation state of a remote access service (RAS) connection.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>Known Limitations:</b>
    /// <list type="bullet">
    /// <item>This type is only available on Windows Vista and later operating systems.</item>
    /// </list>
    /// </para>
    /// </remarks>
    public enum RasIsolationState
    {
        /// <summary>
        /// The connection isolation state is unknown.
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// The connection isolation state is not restricted.
        /// </summary>
        NotRestricted,

        /// <summary>
        /// The connection isolation state is in probation.
        /// </summary>
        InProbation,

        /// <summary>
        /// The connection isolation state is restricted access.
        /// </summary>
        RestrictedAccess
    }

#endif
}