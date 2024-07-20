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
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using System.Text;
    using MessagingToolkit.Core.Ras.Internal;

    /// <summary>
    /// Represents a locally unique identifier (LUID).
    /// </summary>
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct Luid : IEquatable<Luid>, IFormattable
    {
        #region Fields

        /// <summary>
        /// Represents an empty <see cref="Luid"/>. This field is read-only.
        /// </summary>
        public static readonly Luid Empty = new Luid();

        private int lowPart;
        private int highPart;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MessagingToolkit.Core.Ras.Luid"/> structure.
        /// </summary>
        /// <param name="lowPart">The low part.</param>
        /// <param name="highPart">The high part.</param>
        public Luid(int lowPart, int highPart)
        {
            this.lowPart = lowPart;
            this.highPart = highPart;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Generates a new locally unique identifier.
        /// </summary>
        /// <returns>A new <see cref="MessagingToolkit.Core.Ras.Luid"/> structure.</returns>
        /// <remarks>A <see cref="Luid"/> is guaranteed to be unique only on the system on which it was generated. Also, the uniqueness of a <see cref="Luid"/> is guaranteed only until the system is restarted.</remarks>
        public static Luid NewLuid()
        {
            return RasHelper.Instance.AllocateLocallyUniqueId();
        }

        /// <summary>
        /// Indicates whether the objects are equal.
        /// </summary>
        /// <param name="objA">The first object to check.</param>
        /// <param name="objB">The second object to check.</param>
        /// <returns><b>true</b> if the objects are equal, otherwise <b>false</b>.</returns>
        public static bool operator ==(Luid objA, Luid objB)
        {
            return objA.Equals(objB);
        }

        /// <summary>
        /// Indicates whether the objects are not equal.
        /// </summary>
        /// <param name="objA">The first object to check.</param>
        /// <param name="objB">The second object to check.</param>
        /// <returns><b>true</b> if the objects are not equal, otherwise <b>false</b>.</returns>
        public static bool operator !=(Luid objA, Luid objB)
        {
            return !objA.Equals(objB);
        }

        /// <summary>
        /// Indicates whether this instance and a specified object are equal.
        /// </summary>
        /// <param name="obj">The object to compare the current instance to.</param>
        /// <returns><b>true</b> if the objects are equal, otherwise <b>false</b>.</returns>
        public override bool Equals(object obj)
        {
            bool retval = false;

            if (obj is Luid)
            {
                retval = this.Equals((Luid)obj);
            }

            return retval;
        }

        /// <summary>
        /// Indicates whether this instance and a specified object are equal.
        /// </summary>
        /// <param name="other">The <see cref="MessagingToolkit.Core.Ras.Luid"/> to compare the current instance to.</param>
        /// <returns><b>true</b> if the objects are equal, otherwise <b>false</b>.</returns>
        public bool Equals(Luid other)
        {
            return this.highPart == other.highPart && this.lowPart == other.lowPart;
        }

        /// <summary>
        /// Overridden. Returns the hash code for this instance.
        /// </summary>
        /// <returns>The hash code for the instance.</returns>
        public override int GetHashCode()
        {
            long hashCode = (long)this.highPart + (long)this.lowPart;

            int retval = 0;
            if (hashCode < int.MinValue)
            {
                retval = int.MinValue;
            }
            else if (hashCode > int.MaxValue)
            {
                retval = int.MaxValue;
            }
            else if (hashCode >= int.MinValue && hashCode <= int.MaxValue)
            {
                retval = (int)hashCode;
            }
            else
            {
                retval = 0;
            }

            return retval;
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> representation of this locally unique identifier.
        /// </summary>
        /// <returns>A <see cref="System.String"/> representation of this locally unique identifier.</returns>
        public override string ToString()
        {
            return this.ToString(null, null);
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> representation of this locally unique identifier.
        /// </summary>
        /// <param name="format">A format specifier indicating how to format the value of this <see cref="Luid"/>.</param>
        /// <returns>A <see cref="System.String"/> representation of this locally unique identifier.</returns>
        public string ToString(string format)
        {
            return this.ToString(format, null);
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> representation of this locally unique identifier.
        /// </summary>
        /// <param name="format">A format specifier indicating how to format the value of this <see cref="Luid"/>.</param>
        /// <param name="formatProvider">An <see cref="IFormatProvider"/> that supplies culture-specific formatting services.</param>
        /// <returns>A <see cref="System.String"/> representation of this locally unique identifier.</returns>
        public string ToString(string format, IFormatProvider formatProvider)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(this.lowPart.ToString(format, formatProvider)).Append("-").Append(this.highPart.ToString(format, formatProvider));

            return sb.ToString();
        }

        #endregion
    }
}