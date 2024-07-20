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
    using MessagingToolkit.Core.Ras.Internal;
    using MessagingToolkit.Core.Properties;

    /// <summary>
    /// Represents a subentry of a remote access service (RAS) entry. This class cannot be inherited.
    /// </summary>
    [DebuggerDisplay("PhoneNumber = {PhoneNumber}")]
    public sealed class RasSubEntry : MarshalByRefObject, ICloneable
    {
        #region Fields

        private Collection<string> alternatePhoneNumbers;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MessagingToolkit.Core.Ras.RasSubEntry"/> class.
        /// </summary>
        public RasSubEntry()
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the owner of the subentry.
        /// </summary>
        public RasEntry Owner
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets or sets the remote access device.
        /// </summary>
        /// <remarks>To retrieve a list of available devices, use the <see cref="RasDevice.GetDevices"/> method.</remarks>
        public RasDevice Device
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the phone number.
        /// </summary>
        public string PhoneNumber
        {
            get;
            set;
        }

        /// <summary>
        /// Gets a collection of alternate phone numbers that are dialed in the order listed if the primary number fails.
        /// </summary>
        public Collection<string> AlternatePhoneNumbers
        {
            get
            {
                if (this.alternatePhoneNumbers == null)
                {
                    this.alternatePhoneNumbers = new Collection<string>();
                }

                return this.alternatePhoneNumbers;
            }

            internal set
            {
                this.alternatePhoneNumbers = value;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Creates a copy of this <see cref="RasSubEntry"/>.
        /// </summary>
        /// <returns>A new <see cref="MessagingToolkit.Core.Ras.RasSubEntry"/> object.</returns>
        public object Clone()
        {
            RasSubEntry retval = new RasSubEntry();

            if (this.AlternatePhoneNumbers.Count > 0)
            {
                foreach (string value in this.AlternatePhoneNumbers)
                {
                    retval.AlternatePhoneNumbers.Add(value);
                }
            }

            retval.Device = this.Device;
            retval.PhoneNumber = this.PhoneNumber;

            return retval;
        }

        /// <summary>
        /// Removes the subentry from the phone book.
        /// </summary>
        /// <returns><b>true</b> if the operation was successful, otherwise <b>false</b>.</returns>
        public bool Remove()
        {
            bool retval = false;

            if (this.Owner != null)
            {
                retval = this.Owner.SubEntries.Remove(this);
            }

            return retval;
        }

        /// <summary>
        /// Updates the subentry.
        /// </summary>
        /// <returns><b>true</b> if the operation was successful, otherwise <b>false</b>.</returns>
        /// <exception cref="System.InvalidOperationException">The collection is not associated with a phone book.</exception>
        public bool Update()
        {
            if (this.Owner == null || this.Owner.Owner == null)
            {
                ThrowHelper.ThrowInvalidOperationException(Resources.Exception_EntryNotInPhoneBook);
            }

            bool retval = false;

            int index = this.Owner.SubEntries.IndexOf(this);
            if (index != -1)
            {
                retval = RasHelper.Instance.SetSubEntryProperties(this.Owner.Owner, this.Owner, index, this);
            }

            return retval;
        }

        #endregion
    }
}