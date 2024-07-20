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
    using System.Collections.ObjectModel;
    using System.Diagnostics;

    /// <summary>
    /// Represents a network address in the AutoDial mapping database. This class cannot be inherited.
    /// </summary>
    [Serializable]
    [DebuggerDisplay("Address = {Address}")]
    public sealed class RasAutoDialAddress
    {
        #region Fields

        private string _address;
        private Collection<RasAutoDialEntry> _entries;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MessagingToolkit.Core.Ras.RasAutoDialAddress"/> class.
        /// </summary>
        /// <param name="address">The network address.</param>
        public RasAutoDialAddress(string address)
        {
            this._address = address;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the network address.
        /// </summary>
        public string Address
        {
            get { return this._address; }
        }

        /// <summary>
        /// Gets the collection of entries associated with the address.
        /// </summary>
        public Collection<RasAutoDialEntry> Entries
        {
            get
            {
                if (this._entries == null)
                {
                    this._entries = new Collection<RasAutoDialEntry>();
                }

                return this._entries;
            }
        }

        #endregion
    }
}