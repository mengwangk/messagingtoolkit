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
    using MessagingToolkit.Core.Ras.Design;
    using MessagingToolkit.Core.Ras.Internal;

    /// <summary>
    /// Represents a strongly-typed collection of <see cref="DotRas.RasAutoDialAddress"/> objects. This class cannot be inherited.
    /// </summary>
    public sealed class RasAutoDialAddressCollection : RasCollection<RasAutoDialAddress>
    {
        #region Fields

        private RasAutoDialAddressItemCollection _lookUpTable;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DotRas.RasAutoDialAddressCollection"/> class.
        /// </summary>
        internal RasAutoDialAddressCollection()
        {
            this._lookUpTable = new RasAutoDialAddressItemCollection();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets an entry from the collection.
        /// </summary>
        /// <param name="address">The address to get.</param>
        /// <returns>An <see cref="RasAutoDialAddress"/> object.</returns>
        public RasAutoDialAddress this[string address]
        {
            get { return this._lookUpTable[address]; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Loads the collection of AutoDial addresses.
        /// </summary>
        internal void Load()
        {
            try
            {
                this.BeginLock();
                this.IsInitializing = true;

                this.Clear();

                Collection<string> addresses = RasHelper.Instance.GetAutoDialAddresses();
                if (addresses != null && addresses.Count > 0)
                {
                    for (int index = 0; index < addresses.Count; index++)
                    {
                        RasAutoDialAddress item = RasHelper.Instance.GetAutoDialAddress(addresses[index]);
                        if (item != null)
                        {
                            this.Add(item);
                        }
                    }
                }
            }
            finally
            {
                this.IsInitializing = false;
                this.EndLock();
            }
        }

        /// <summary>
        /// Clears the items in the collection.
        /// </summary>
        protected override void ClearItems()
        {
            while (this.Count > 0)
            {
                this.RemoveAt(0);
            }
        }

        /// <summary>
        /// Inserts the item at the index specified.
        /// </summary>
        /// <param name="index">The zero-based index at which the item will be inserted.</param>
        /// <param name="item">An <see cref="RasAutoDialAddress"/> to insert.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1", Justification = "The argument has been validated.")]
        protected override void InsertItem(int index, RasAutoDialAddress item)
        {
            if (item == null)
            {
                ThrowHelper.ThrowArgumentNullException("item");
            }

            if (this.IsInitializing)
            {
                this._lookUpTable.Insert(index, item);
            }
            else
            {
                if (RasHelper.Instance.SetAutoDialAddress(item.Address, item.Entries))
                {
                    // The item has been added to the database, retrieve the item again to ensure entries match what 
                    // is already in the database. Removing an item from the database does not clear existing entries.
                    item = RasHelper.Instance.GetAutoDialAddress(item.Address);

                    this._lookUpTable.Insert(index, item);
                }
            }

            base.InsertItem(index, item);
        }

        /// <summary>
        /// Removes the item at the index specified.
        /// </summary>
        /// <param name="index">The zero-based index of the item to remove.</param>
        protected override void RemoveItem(int index)
        {
            if (!this.IsInitializing)
            {
                RasAutoDialAddress item = this[index];
                if (item != null)
                {
                    if (RasHelper.Instance.SetAutoDialAddress(item.Address, null))
                    {
                        this._lookUpTable.RemoveAt(index);
                    }
                }
            }

            base.RemoveItem(index);
        }

        #endregion

        #region RasAutoDialAddressItemCollection Class

        /// <summary>
        /// Represents a collection of <see cref="DotRas.RasAutoDialAddress"/> objects keyed by the entry address.
        /// </summary>
        private class RasAutoDialAddressItemCollection : KeyedCollection<string, RasAutoDialAddress>
        {
            #region Constructors

            /// <summary>
            /// Initializes a new instance of the <see cref="RasAutoDialAddressItemCollection"/> class.
            /// </summary>
            public RasAutoDialAddressItemCollection()
            {
            }

            #endregion

            #region Methods

            /// <summary>
            /// Extracts the key for the <see cref="DotRas.RasAutoDialAddress"/> object.
            /// </summary>
            /// <param name="item">Required. An <see cref="DotRas.RasAutoDialAddress"/> from which to extract the key.</param>
            /// <returns>The key for the <paramref name="item"/> specified.</returns>
            /// <exception cref="System.ArgumentNullException"><paramref name="item"/> is a null reference (<b>Nothing</b> in Visual Basic).</exception>
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "The argument has been validated.")]
            protected override string GetKeyForItem(RasAutoDialAddress item)
            {
                if (item == null)
                {
                    ThrowHelper.ThrowArgumentNullException("item");
                }

                return item.Address;
            }

            #endregion
        }

        #endregion
    }
}