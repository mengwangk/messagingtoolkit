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
    using Microsoft.Win32.SafeHandles;

    /// <summary>
    /// Represents a wrapper class for remote access service (RAS) handles. This class cannot be inherited.
    /// </summary>
    public sealed class RasHandle : SafeHandleZeroOrMinusOneIsInvalid, IEquatable<RasHandle>
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MessagingToolkit.Core.Ras.RasHandle"/> class.
        /// </summary>
        public RasHandle()
            : base(false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessagingToolkit.Core.Ras.RasHandle"/> class.
        /// </summary>
        /// <param name="handle">The handle to use.</param>
        /// <param name="isMultilink"><b>true</b> if the handle is a single-link in a multi-link connection.</param>
        internal RasHandle(IntPtr handle, bool isMultilink)
            : base(false)
        {
            this.IsMultilink = isMultilink;

            this.SetHandle(handle);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets a value indicating whether the handle is for a single-link in a multi-link connection.
        /// </summary>
        public bool IsMultilink
        {
            get;
            private set;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Determines whether two instances of <see cref="MessagingToolkit.Core.Ras.RasHandle"/> are equal.
        /// </summary>
        /// <param name="objA">The first <see cref="MessagingToolkit.Core.Ras.RasHandle"/> to compare.</param>
        /// <param name="objB">The second <see cref="MessagingToolkit.Core.Ras.RasHandle"/> to compare.</param>
        /// <returns><b>true</b> if <paramref name="objA"/> equals <paramref name="objB"/>, otherwise <b>false</b>.</returns>
        public static bool operator ==(RasHandle objA, RasHandle objB)
        {
            return object.Equals(objA, objB);
        }

        /// <summary>
        /// Determines whether two instances of <see cref="MessagingToolkit.Core.Ras.RasHandle"/> are not equal.
        /// </summary>
        /// <param name="objA">The first <see cref="MessagingToolkit.Core.Ras.RasHandle"/> to compare.</param>
        /// <param name="objB">The second <see cref="MessagingToolkit.Core.Ras.RasHandle"/> to compare.</param>
        /// <returns><b>true</b> if <paramref name="objA"/> does not equal <paramref name="objB"/>, otherwise <b>false</b>.</returns>
        public static bool operator !=(RasHandle objA, RasHandle objB)
        {
            return !(objA == objB);
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to the current <see cref="System.Object"/>.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with the current <see cref="System.Object"/>.</param>
        /// <returns><b>true</b> if <paramref name="obj"/> is equal to the current object, otherwise <b>false</b>.</returns>
        public override bool Equals(object obj)
        {
            RasHandle handle = obj as RasHandle;

            if (!object.ReferenceEquals(handle, null))
            {
                return this.Equals(handle);
            }

            return false;
        }

        /// <summary>
        /// Determines whether the specified <see cref="MessagingToolkit.Core.Ras.RasHandle"/> is equal to the current <see cref="MessagingToolkit.Core.Ras.RasHandle"/>.
        /// </summary>
        /// <param name="other">The <see cref="MessagingToolkit.Core.Ras.RasHandle"/> to compare with the current <see cref="MessagingToolkit.Core.Ras.RasHandle"/>.</param>
        /// <returns><b>true</b> if <paramref name="other"/> is equal to the current object, otherwise <b>false</b>.</returns>
        public bool Equals(RasHandle other)
        {
            if (object.ReferenceEquals(other, null))
            {
                return false;
            }

            if (object.ReferenceEquals(other, this))
            {
                return true;
            }

            return this.handle == other.handle;
        }

        /// <summary>
        /// Serves as a hash function for a particular type.
        /// </summary>
        /// <returns>The hash code for the instance.</returns>
        public override int GetHashCode()
        {
            return this.handle.GetHashCode();
        }

        /// <summary>
        /// Releases the handle.
        /// </summary>
        /// <returns><b>true</b> if the handle was released successfully, otherwise <b>false</b>.</returns>
        /// <remarks>This method will never release the handle, doing so would disconnect the client when the object is finalized.</remarks>
        protected override bool ReleaseHandle()
        {
            return true;
        }

        #endregion
    }
}