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
    using MessagingToolkit.Core.Properties;

    /// <summary>
    /// Represents a strongly-typed collection of <see cref="MessagingToolkit.Core.Ras.RasEntry"/> objects. This class cannot be inherited.
    /// </summary>
    public sealed class RasEntryCollection : RasOwnedCollection<RasPhoneBook, RasEntry>
    {
        #region Fields

        private RasEntryItemCollection _lookUpTable;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MessagingToolkit.Core.Ras.RasEntryCollection"/> class.
        /// </summary>
        /// <param name="owner">Required. The <see cref="MessagingToolkit.Core.Ras.RasPhoneBook"/> that owns the collection.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="owner"/> is a null reference (<b>Nothing</b> in Visual Basic).</exception>
        internal RasEntryCollection(RasPhoneBook owner)
            : base(owner)
        {
            this._lookUpTable = new RasEntryItemCollection();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets an entry from the collection.
        /// </summary>
        /// <param name="name">The name of the entry to get.</param>
        /// <returns>An <see cref="RasEntry"/> object.</returns>
        public RasEntry this[string name]
        {
            get { return this._lookUpTable[name]; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Determines whether the phone book contains the entry specified.
        /// </summary>
        /// <param name="name">The name of the entry to locate.</param>
        /// <returns><b>true</b> if the item was found in the phone book, otherwise <b>false</b>.</returns>
        /// <exception cref="System.ArgumentException"><paramref name="name"/> is an empty string or null reference (<b>Nothing</b> in Visual Basic).</exception>
        public bool Contains(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                ThrowHelper.ThrowArgumentException("name", Resources.Argument_StringCannotBeNullOrEmpty);
            }

            return this._lookUpTable.Contains(name);
        }

        /// <summary>
        /// Removes an entry from the collection.
        /// </summary>
        /// <param name="name">Required. The name of the entry to remove.</param>
        /// <returns><b>true</b> if the item was removed successfully, otherwise <b>false</b>.</returns>
        /// <exception cref="System.ArgumentNullException"><paramref name="name"/> is a null reference (<b>Nothing</b> in Visual Basic).</exception>
        public bool Remove(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                ThrowHelper.ThrowArgumentException("name", Resources.Argument_StringCannotBeNullOrEmpty);
            }

            bool retval = false;

            if (this.Contains(name))
            {
                RasEntry item = this[name];
                if (item != null)
                {
                    retval = this.Remove(item);
                }
            }

            return retval;
        }

        /// <summary>
        /// Loads the collection of entries for the phone book.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">The phone book of the entry collection has not been opened.</exception>
        /// <exception cref="System.UnauthorizedAccessException">The caller does not have the required permission to perform the action requested.</exception>
        internal void Load()
        {
            if (string.IsNullOrEmpty(this.Owner.Path))
            {
                ThrowHelper.ThrowInvalidOperationException(Resources.Exception_PhoneBookNotOpened);
            }

            try
            {
                this.BeginLock();
                this.IsInitializing = true;

                this.Clear();

                NativeMethods.RASENTRYNAME[] entries = RasHelper.Instance.GetEntryNames(this.Owner);
                if (entries != null && entries.Length > 0)
                {
                    for (int index = 0; index < entries.Length; index++)
                    {
                        NativeMethods.RASENTRYNAME entry = entries[index];

                        RasEntry item = RasHelper.Instance.GetEntryProperties(this.Owner, entry.name);
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
        /// Changes the key for the item specified.
        /// </summary>
        /// <param name="item">Required. An <see cref="MessagingToolkit.Core.Ras.RasEntry"/> whose key to change.</param>
        /// <param name="newKey">The new key.</param>
        /// <exception cref="System.ArgumentException"><paramref name="newKey"/> is an empty string or null reference (<b>Nothing</b> in Visual Basic).</exception>
        /// <exception cref="System.ArgumentNullException"><paramref name="item"/> is a null reference (<b>Nothing</b> in Visual Basic).</exception>
        internal void ChangeKey(RasEntry item, string newKey)
        {
            this._lookUpTable.ChangeKey(item, newKey);
        }

        /// <summary>
        /// Clears the items in the collection.
        /// </summary>
        protected override void ClearItems()
        {
            bool isFileWatcherEnabled = false;

            try
            {
                isFileWatcherEnabled = this.Owner.EnableFileWatcher;
                this.Owner.EnableFileWatcher = false;

                while (this.Count > 0)
                {
                    this.RemoveAt(0);
                }
            }
            finally
            {
                this.Owner.EnableFileWatcher = isFileWatcherEnabled;

                if (!this.IsInitializing)
                {
                    this.Load();
                }
            }
        }

        /// <summary>
        /// Inserts the item at the index specified.
        /// </summary>
        /// <param name="index">The zero-based index at which the item will be inserted.</param>
        /// <param name="item">An <see cref="RasEntry"/> to insert.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1", Justification = "The argument has been validated.")]
        protected override void InsertItem(int index, RasEntry item)
        {
            if (item == null)
            {
                ThrowHelper.ThrowArgumentNullException("item");
            }

            if (this.Owner == null || string.IsNullOrEmpty(this.Owner.Path))
            {
                ThrowHelper.ThrowInvalidOperationException(Resources.Exception_PhoneBookNotOpened);
            }

            if (this.Contains(item.Name))
            {
                ThrowHelper.ThrowArgumentException("item", Resources.Argument_EntryAlreadyExists, item.Name);
            }

            item.Owner = this.Owner;

            if (this.IsInitializing)
            {
                this._lookUpTable.Insert(index, item);
            }
            else
            {
                if (RasHelper.Instance.SetEntryProperties(this.Owner, item) && !this.Owner.EnableFileWatcher)
                {
                    // The item was inserted while file monitoring was off, manually add it to the collection.
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
            if (this.Owner == null || string.IsNullOrEmpty(this.Owner.Path))
            {
                ThrowHelper.ThrowInvalidOperationException(Resources.Exception_PhoneBookNotOpened);
            }

            if (!this.IsInitializing)
            {
                RasEntry entry = this[index];
                if (entry != null)
                {
                    if (RasHelper.Instance.DeleteEntry(entry.Owner.Path, entry.Name) && !this.Owner.EnableFileWatcher)
                    {
                        // The entry was removed while file monitoring was off, remove the item from the collection.
                        this._lookUpTable.RemoveAt(index);
                    }
                }
            }

            base.RemoveItem(index);
        }

        #endregion

        #region RasEntryItemCollection Class

        /// <summary>
        /// Represents a collection of <see cref="MessagingToolkit.Core.Ras.RasEntry"/> objects keyed by the entry name.
        /// </summary>
        private class RasEntryItemCollection : KeyedCollection<string, RasEntry>
        {
            #region Constructors

            /// <summary>
            /// Initializes a new instance of the <see cref="MessagingToolkit.Core.Ras.RasEntryCollection.RasEntryItemCollection"/> class.
            /// </summary>
            public RasEntryItemCollection()
            {
            }

            #endregion

            #region Methods

            /// <summary>
            /// Changes the lookup table key for the item specified.
            /// </summary>
            /// <param name="item">Required. An <see cref="MessagingToolkit.Core.Ras.RasEntry"/> whose key to change.</param>
            /// <param name="newKey">The new key.</param>
            /// <exception cref="System.ArgumentException"><paramref name="newKey"/> is an empty string or null reference (<b>Nothing</b> in Visual Basic).</exception>
            /// <exception cref="System.ArgumentNullException"><paramref name="item"/> is a null reference (<b>Nothing</b> in Visual Basic).</exception>
            public void ChangeKey(RasEntry item, string newKey)
            {
                if (item == null)
                {
                    ThrowHelper.ThrowArgumentNullException("item");
                }

                if (string.IsNullOrEmpty(newKey))
                {
                    ThrowHelper.ThrowArgumentException("newKey", Resources.Argument_StringCannotBeNullOrEmpty);
                }

                this.ChangeItemKey(item, newKey);
            }

            /// <summary>
            /// Extracts the key for the <see cref="MessagingToolkit.Core.Ras.RasEntry"/> object.
            /// </summary>
            /// <param name="item">Required. An <see cref="MessagingToolkit.Core.Ras.RasEntry"/> from which to extract the key.</param>
            /// <returns>The key for the <paramref name="item"/> specified.</returns>
            /// <exception cref="System.ArgumentNullException"><paramref name="item"/> is a null reference (<b>Nothing</b> in Visual Basic).</exception>
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "The argument has been validated.")]
            protected override string GetKeyForItem(RasEntry item)
            {
                if (item == null)
                {
                    ThrowHelper.ThrowArgumentNullException("item");
                }

                return item.Name;
            }

            #endregion
        }

        #endregion
    }
}