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

    /// <summary>
    /// Represents user specific Extensible Authentication Protocol (EAP) information. This class cannot be inherited.
    /// </summary>
    public sealed class RasEapInfo
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DotRas.RasEapInfo"/> class.
        /// </summary>
        public RasEapInfo()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DotRas.RasEapInfo"/> class.
        /// </summary>
        /// <param name="sizeOfEapData">The size of the binary information pointed to by <paramref name="eapData"/>.</param>
        /// <param name="eapData">The pointer to the binary EAP information.</param>
        public RasEapInfo(int sizeOfEapData, IntPtr eapData)
        {
            this.SizeOfEapData = sizeOfEapData;
            this.EapData = eapData;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the size of the binary information pointed to by the <see cref="RasEapInfo.EapData"/> property.
        /// </summary>
        public int SizeOfEapData
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a pointer to the binary data.
        /// </summary>
        public IntPtr EapData
        {
            get;
            set;
        }

        #endregion
    }
}