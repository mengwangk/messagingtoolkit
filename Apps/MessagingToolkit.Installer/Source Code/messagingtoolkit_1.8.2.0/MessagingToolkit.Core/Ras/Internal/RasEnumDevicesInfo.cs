﻿//===============================================================================
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

namespace MessagingToolkit.Core.Ras.Internal
{
    using System;

    /// <summary>
    /// Provides information for the RasEnumDevices API call. This class cannot be inherited.
    /// </summary>
    internal sealed class RasEnumDevicesInfo : BufferedPInvokeInfo
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="RasEnumDevicesInfo"/> class.
        /// </summary>
        public RasEnumDevicesInfo()
        {
        }

        #endregion
    }
}