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

namespace MessagingToolkit.Core.Ras.Internal
{
    using System;

    /// <summary>
    /// Provides information for the RasGetAutodialAddress API call.
    /// </summary>
    internal class RasGetAutodialAddressParams : StructBufferedPInvokeParams
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="RasGetAutodialAddressParams"/> class.
        /// </summary>
        public RasGetAutodialAddressParams()
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the Autodial address for which information is being requested.
        /// </summary>
        public string AutodialAddress
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the reserved value.
        /// </summary>
        /// <remarks>This value must be <see cref="IntPtr.Zero"/>.</remarks>
        public IntPtr Reserved
        {
            get;
            set;
        }

        #endregion
    }
}