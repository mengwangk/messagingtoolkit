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
    /// Provides data for remote access service (RAS) connection events.
    /// </summary>
    public class RasConnectionEventArgs : EventArgs
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DotRas.RasConnectionEventArgs"/> class.
        /// </summary>
        /// <param name="connection">The <see cref="DotRas.RasConnection"/> that caused the event.</param>
        public RasConnectionEventArgs(RasConnection connection)
        {
            this.Connection = connection;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the connection that caused the event.
        /// </summary>
        public RasConnection Connection
        {
            get;
            private set;
        }

        #endregion
    }
}