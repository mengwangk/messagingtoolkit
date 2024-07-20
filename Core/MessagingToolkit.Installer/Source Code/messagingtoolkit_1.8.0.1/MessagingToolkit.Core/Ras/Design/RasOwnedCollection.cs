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

namespace MessagingToolkit.Core.Ras.Design
{
    using System;
    using MessagingToolkit.Core.Ras.Internal;

    /// <summary>
    /// Provides the abstract base class for a remotable collection whose objects are owned by other objects. This class must be inherited.
    /// </summary>
    /// <typeparam name="TOwner">The type of object that owns the objects in the collection.</typeparam>
    /// <typeparam name="TObject">The type of object contained in the collection.</typeparam>
    public abstract class RasOwnedCollection<TOwner, TObject> : RasCollection<TObject>
        where TOwner : class
        where TObject : class
    {
        #region Fields

        private TOwner _owner;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MessagingToolkit.Core.Ras.Design.RasOwnedCollection&lt;TOwner, TObject&gt;"/> class.
        /// </summary>
        /// <param name="owner">The owner of the collection.</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="owner"/> is a null reference (<b>Nothing</b> in Visual Basic).</exception>
        protected RasOwnedCollection(TOwner owner)
        {
            if (owner == null)
            {
                ThrowHelper.ThrowArgumentNullException("owner");
            }

            this._owner = owner;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the owner of the collection.
        /// </summary>
        protected TOwner Owner
        {
            get { return this._owner; }
        }

        #endregion
    }
}