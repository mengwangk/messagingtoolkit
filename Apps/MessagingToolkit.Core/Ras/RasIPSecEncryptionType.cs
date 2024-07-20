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

#if (WIN7)

    /// <summary>
    /// Defines the IPSec encryption types.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>Known Limitations:</b>
    /// <list type="bullet">
    /// <item>This type is only available on Windows 7 and later operating systems.</item>
    /// </list>
    /// </para>
    /// </remarks>
    public enum RasIPSecEncryptionType
    {
        /// <summary>
        /// No encryption type specified.
        /// </summary>
        None = 0,

        /// <summary>
        /// DES encryption.
        /// </summary>
        Des = 1,

        /// <summary>
        /// Triple DES encryption.
        /// </summary>
        TripleDes = 2,

        /// <summary>
        /// AES 128-bit encryption.
        /// </summary>
        Aes128 = 3,

        /// <summary>
        /// AES 192-bit encryption.
        /// </summary>
        Aes192 = 4,

        /// <summary>
        /// AES 256-bit encryption.
        /// </summary>
        Aes256 = 5,

        /// <summary>
        /// Maximum encryption.
        /// </summary>
        Max = 6
    }

#endif
}