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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MessagingToolkit.Core.Base;

namespace MessagingToolkit.Core.Mobile
{
    /// <summary>
    /// Device information
    /// </summary>
    public class DeviceInformation : BaseClass<DeviceInformation>
    {

        /// <summary>
        /// Default constructor
        /// </summary>
        public DeviceInformation()
        {
            Model = string.Empty;
            Manufacturer = string.Empty;
            FirmwareVersion = string.Empty;
            SerialNo = string.Empty;           
            Imsi = string.Empty;
        }

        /// <summary>
        /// Gateway model
        /// </summary>
        /// <value>Model</value>
        public string Model
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gateway manufacturer
        /// </summary>
        /// <value>Manufacturer</value>
        public string Manufacturer
        {
            get;
            internal set;
        }

        /// <summary>
        /// Firmware version
        /// </summary>
        /// <value>Firmware</value>
        public string FirmwareVersion
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gateway serial number
        /// </summary>
        /// <value></value>
        public string SerialNo
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gateway IMSI 
        /// </summary>
        /// <value></value>
        public string Imsi
        {
            get;
            internal set;
        }
        
    }
}
