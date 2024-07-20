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
    /// Represents remote access service (RAS) link control protocol options. This class cannot be inherited.
    /// </summary>
    [Serializable]
    public sealed class RasLcpOptions : ICloneable
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DotRas.RasLcpOptions"/> class.
        /// </summary>
        internal RasLcpOptions()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DotRas.RasLcpOptions"/> class.
        /// </summary>
        /// <param name="value">The flags value to set.</param>
        internal RasLcpOptions(NativeMethods.RASLCPO value)
        {
            this.Pfc = Utilities.HasFlag(value, NativeMethods.RASLCPO.Pfc);
            this.Acfc = Utilities.HasFlag(value, NativeMethods.RASLCPO.Acfc);
            this.Sshf = Utilities.HasFlag(value, NativeMethods.RASLCPO.Sshf);
            this.Des56 = Utilities.HasFlag(value, NativeMethods.RASLCPO.Des56);
            this.TripleDes = Utilities.HasFlag(value, NativeMethods.RASLCPO.TripleDes);

#if (WIN2K8 || WIN7)

            this.Aes128 = Utilities.HasFlag(value, NativeMethods.RASLCPO.Aes128);
            this.Aes256 = Utilities.HasFlag(value, NativeMethods.RASLCPO.Aes256);

#endif
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets a value indicating whether protocol field compression is being used.
        /// </summary>
        public bool Pfc
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a value indicating whether address and control field compression is being used.
        /// </summary>
        public bool Acfc
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a value indicating whether short sequence number header format is being used.
        /// </summary>
        public bool Sshf
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a value indicating whether DES 56-bit encryption is being used.
        /// </summary>
        public bool Des56
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a value indicating whether 3 DES encryption is being used.
        /// </summary>
        public bool TripleDes
        {
            get;
            private set;
        }

#if (WIN2K8 || WIN7)

        /// <summary>
        /// Gets a value indicating whether AES 128-bit encryption is being used.
        /// </summary>
        /// <remarks><b>Windows Vista and later:</b> This property is available.</remarks>
        public bool Aes128
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a value indicating whether AES 256-bit encryption is being used.
        /// </summary>
        /// <remarks><b>Windows Vista and later:</b> This property is available.</remarks>
        public bool Aes256
        {
            get;
            private set;
        }

#endif

        #endregion

        #region Methods

        /// <summary>
        /// Creates a copy of this object.
        /// </summary>
        /// <returns>A new <see cref="DotRas.RasLcpOptions"/> object.</returns>
        public object Clone()
        {
            RasLcpOptions retval = new RasLcpOptions();

            retval.Pfc = this.Pfc;
            retval.Acfc = this.Acfc;
            retval.Sshf = this.Sshf;
            retval.Des56 = this.Des56;
            retval.TripleDes = this.TripleDes;

#if (WIN2K8 || WIN7)

            retval.Aes128 = this.Aes128;
            retval.Aes256 = this.Aes256;

#endif

            return retval;
        }

        #endregion
    }
}