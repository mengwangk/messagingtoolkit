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
    /// Represents remote access service (RAS) compression options. This class cannot be inherited.
    /// </summary>
    [Serializable]
    public sealed class RasCompressionOptions : ICloneable
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DotRas.RasCompressionOptions"/> class.
        /// </summary>
        internal RasCompressionOptions()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DotRas.RasCompressionOptions"/> class.
        /// </summary>
        /// <param name="value">The flags value to set.</param>
        internal RasCompressionOptions(NativeMethods.RASCCPO value)
        {
            this.CompressionOnly = Utilities.HasFlag(value, NativeMethods.RASCCPO.CompressionOnly);
            this.HistoryLess = Utilities.HasFlag(value, NativeMethods.RASCCPO.HistoryLess);
            this.Encryption56Bit = Utilities.HasFlag(value, NativeMethods.RASCCPO.Encryption56Bit);
            this.Encryption40Bit = Utilities.HasFlag(value, NativeMethods.RASCCPO.Encryption40Bit);
            this.Encryption128Bit = Utilities.HasFlag(value, NativeMethods.RASCCPO.Encryption128Bit);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets a value indicating whether compression without encryption will be used.
        /// </summary>
        public bool CompressionOnly
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a value indicating whether Microsoft Point-to-Point Encryption (MPPE) is in stateless mode.
        /// </summary>
        public bool HistoryLess
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a value indicating whether Microsoft Point-to-Point Encryption (MPPE) is using 56-bit keys.
        /// </summary>
        public bool Encryption56Bit
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a value indicating whether Microsoft Point-to-Point Encryption (MPPE) is using 40-bit keys.
        /// </summary>
        public bool Encryption40Bit
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a value indicating whether Microsoft Point-to-Point Encryption (MPPE) is using 128-bit keys.
        /// </summary>
        public bool Encryption128Bit
        {
            get;
            private set;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Creates a copy of this object.
        /// </summary>
        /// <returns>A new <see cref="DotRas.RasCompressionOptions"/> object.</returns>
        public object Clone()
        {
            RasCompressionOptions retval = new RasCompressionOptions();

            retval.CompressionOnly = this.CompressionOnly;
            retval.HistoryLess = this.HistoryLess;
            retval.Encryption56Bit = this.Encryption56Bit;
            retval.Encryption40Bit = this.Encryption40Bit;
            retval.Encryption128Bit = this.Encryption128Bit;

            return retval;
        }

        #endregion
    }
}