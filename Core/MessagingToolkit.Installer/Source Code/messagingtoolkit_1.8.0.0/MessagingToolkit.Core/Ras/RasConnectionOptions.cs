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
    /// Represents connection options for a remote access service (RAS) connection. This class cannot be inherited.
    /// </summary>
    [Serializable]
    public sealed class RasConnectionOptions
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MessagingToolkit.Core.Ras.RasConnectionOptions"/> class.
        /// </summary>
        internal RasConnectionOptions()
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets a value indicating whether the connection is available to all users.
        /// </summary>
        public bool AllUsers
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets a value indicating whether the credentials used for the connection are the default credentials.
        /// </summary>
        public bool GlobalCredentials
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets a value indicating whether the owner of the connection is known.
        /// </summary>
        public bool OwnerKnown
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets a value indicating whether the owner of the connection matches the current user.
        /// </summary>
        public bool OwnerMatch
        {
            get;
            internal set;
        }

        #endregion
    }

#endif
}