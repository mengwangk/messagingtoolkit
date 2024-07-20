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
    /// Defines the Internet Key Exchange (IKEv2) options.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>Known Limitations:</b>
    /// <list type="bullet">
    /// <item>This type is only available on Windows 7 and later operating systems.</item>
    /// </list>
    /// </para>
    /// </remarks>
    [Serializable]
    public sealed class RasIkeV2Options
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MessagingToolkit.Core.Ras.RasIkeV2Options"/> class.
        /// </summary>
        internal RasIkeV2Options()
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets a value indicating whether the client supports the IKEv2 Mobility and Multihoming (MOBIKE) protocol.
        /// </summary>
        public bool MobileIke
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets a value indicating whether the client is behind Network Address Translation (NAT).
        /// </summary>
        public bool ClientBehindNat
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets a value indicating whether the server is behind Network Address Translation (NAT).
        /// </summary>
        public bool ServerBehindNat
        {
            get;
            internal set;
        }

        #endregion
    }

#endif
}