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
    /// Provides information for the RasGetAutodialParam API call.
    /// </summary>
    internal class RasGetAutodialParamParams
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="RasGetAutodialParamParams"/> class.
        /// </summary>
        public RasGetAutodialParamParams()
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        public NativeMethods.RASADP Key
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the memory address to receive the information.
        /// </summary>
        public IntPtr Address
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the size of the buffer.
        /// </summary>
        public int BufferSize
        {
            get;
            set;
        }

        #endregion
    }
}