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
    using MessagingToolkit.Core.Ras.Internal;

    /// <summary>
    /// Defines the remote access service (RAS) IPCP options.
    /// </summary>
    [Serializable]
    public class RasIPOptions : ICloneable
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MessagingToolkit.Core.Ras.RasIPOptions"/> class.
        /// </summary>
        internal RasIPOptions()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessagingToolkit.Core.Ras.RasIPOptions"/> class.
        /// </summary>
        /// <param name="value">The flags value to set.</param>
        internal RasIPOptions(NativeMethods.RASIPO value)
        {
            this.VJ = Utilities.HasFlag(value, NativeMethods.RASIPO.VJ);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets a value indicating whether Van Jacobson compression is used.
        /// </summary>
        public bool VJ
        {
            get;
            private set;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Creates a copy of this object.
        /// </summary>
        /// <returns>A new <see cref="MessagingToolkit.Core.Ras.RasLcpOptions"/> object.</returns>
        public object Clone()
        {
            RasIPOptions retval = new RasIPOptions();

            retval.VJ = this.VJ;

            return retval;
        }

        #endregion
    }
}