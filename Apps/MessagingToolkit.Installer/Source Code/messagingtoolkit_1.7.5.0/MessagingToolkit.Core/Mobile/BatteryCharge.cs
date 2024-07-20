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

namespace MessagingToolkit.Core.Mobile
{
    /// <summary>
    /// Battery charge information
    /// </summary>
    public class BatteryCharge
    {

        private int batteryChargeLevel;
        private int batteryChargingStatus;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="batteryChargingStatus">Battery charging status</param>
        /// <param name="batteryChargeLevel">Battery charge level</param>
        public BatteryCharge(int batteryChargingStatus, int batteryChargeLevel)
        {
            this.batteryChargingStatus = batteryChargingStatus;
            this.batteryChargeLevel = batteryChargeLevel;
        }

        /// <summary>
        /// Gets the battery charge level.
        /// </summary>
        /// <value>The battery charge level.</value>
        public int BatteryChargeLevel
        {
            get
            {
                return this.batteryChargeLevel;
            }
        }

        /// <summary>
        /// Gets the battery charging status.
        /// </summary>
        /// <value>The battery charging status.</value>
        public int BatteryChargingStatus
        {
            get
            {
                return this.batteryChargingStatus;
            }
        }
    }
}
