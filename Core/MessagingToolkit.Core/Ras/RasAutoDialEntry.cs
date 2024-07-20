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
    using System.Diagnostics;

    /// <summary>
    /// Represents an entry associated with a network address in the AutoDial mapping database. This class cannot be inherited.
    /// </summary>
    [Serializable]
    [DebuggerDisplay("DialingLocation = {DialingLocation}, EntryName = {EntryName}")]
    public sealed class RasAutoDialEntry
    {
        #region Fields

        private int _dialingLocation;
        private string _entryName;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MessagingToolkit.Core.Ras.RasAutoDialEntry"/> class.
        /// </summary>
        public RasAutoDialEntry()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessagingToolkit.Core.Ras.RasAutoDialEntry"/> class.
        /// </summary>
        /// <param name="dialingLocation">The TAPI dialing location.</param>
        /// <param name="entryName">The name of the existing phone book entry to dial.</param>
        public RasAutoDialEntry(int dialingLocation, string entryName)
        {
            this._dialingLocation = dialingLocation;
            this._entryName = entryName;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the TAPI dialing location.
        /// </summary>
        /// <remarks>For more information about TAPI dialing locations, see the TAPI Programmer's Reference in the Platform SDK.</remarks>
        public int DialingLocation
        {
            get { return this._dialingLocation; }
            set { this._dialingLocation = value; }
        }

        /// <summary>
        /// Gets or sets the name of an existing phone book entry to dial.
        /// </summary>
        public string EntryName
        {
            get { return this._entryName; }
            set { this._entryName = value; }
        }

        #endregion
    }
}