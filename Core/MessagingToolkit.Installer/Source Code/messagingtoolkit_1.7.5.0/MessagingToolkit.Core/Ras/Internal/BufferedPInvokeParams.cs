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
    /// Provides the base implementation for buffered p/invoke information classes.
    /// </summary>
    internal class BufferedPInvokeParams
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BufferedPInvokeParams"/> class.
        /// </summary>
        public BufferedPInvokeParams()
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the memory address to the data.
        /// </summary>
        public IntPtr Address
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the buffer size.
        /// </summary>
        public IntPtr BufferSize
        {
            get;
            set;
        }

        #endregion
    }
}