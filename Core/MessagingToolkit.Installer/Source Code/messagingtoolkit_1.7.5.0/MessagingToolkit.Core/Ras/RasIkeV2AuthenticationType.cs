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

#if (WIN7)

    /// <summary>
    /// Defines the Internet Key Exchange (IKEv2) authentication types.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>Known Limitations:</b>
    /// <list type="bullet">
    /// <item>This type is only available on Windows 7 and later operating systems.</item>
    /// </list>
    /// </para>
    /// </remarks>
    public enum RasIkeV2AuthenticationType
    {
        /// <summary>
        /// No authentication.
        /// </summary>
        None = 0,

        /// <summary>
        /// X.509 Public Key Infrastructure Certificate.
        /// </summary>
        X509Certificate = 1,

        /// <summary>
        /// Extensible Authentication Protocol (EAP).
        /// </summary>
        Eap = 2
    }

#endif
}