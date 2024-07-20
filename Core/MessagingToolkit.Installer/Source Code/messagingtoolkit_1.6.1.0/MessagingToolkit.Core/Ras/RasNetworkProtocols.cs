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
    /// Represents network protocols for a remote access service (RAS) entry. This class cannot be inherited.
    /// </summary>
    [Serializable]
    public sealed class RasNetworkProtocols : ICloneable
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="RasNetworkProtocols"/> class.
        /// </summary>
        public RasNetworkProtocols()
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether the NetBEUI protocol will be negotiated.
        /// </summary>
        [Obsolete("This member is no longer supported.")]
        public bool NetBeui
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the IPX protocol will be negotiated.
        /// </summary>
        public bool Ipx
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the IP protocol will be negotiated.
        /// </summary>
        public bool IP
        {
            get;
            set;
        }

#if (WIN2K8 || WIN7)

        /// <summary>
        /// Gets or sets a value indicating whether the IPv6 protocol will be negotiated.
        /// </summary>
        /// <remarks><b>Windows Vista and later:</b> This property is available.</remarks>
        public bool IPv6
        {
            get;
            set;
        }

#endif

        #endregion

        #region Methods

        /// <summary>
        /// Creates a copy of this object.
        /// </summary>
        /// <returns>A new <see cref="DotRas.RasNetworkProtocols"/> object.</returns>
        public object Clone()
        {
            RasNetworkProtocols retval = new RasNetworkProtocols();

#pragma warning disable 0618
            retval.NetBeui = this.NetBeui;
#pragma warning restore 0618
            retval.Ipx = this.Ipx;
            retval.IP = this.IP;

#if (WIN2K8 || WIN7)

            retval.IPv6 = this.IPv6;

#endif

            return retval;
        }

        #endregion
    }
}