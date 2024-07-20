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
using System.Runtime.Serialization;
using System.Text;

namespace MessagingToolkit.Core.Mobile.Http
{
    /// <summary>
    /// Device battery information.
    /// </summary>
    [DataContract]
    public sealed class DeviceBatteryInformation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DeviceBatteryInformation"/> class.
        /// </summary>
        public DeviceBatteryInformation()
        {
            this.Health = string.Empty;
            this.IsPresent = false;
            this.Level = -1;
            this.PlugType = string.Empty;
            this.Status = string.Empty;
            this.Technology = string.Empty;
            this.Temperature = -1;
            this.Voltage = -1;
        }

        /// <summary>
        /// Gets or sets the health.
        /// </summary>
        /// <value>
        /// Possible values are COLD, DEAD, GOOD, OVERHEAT, OVER_VOLTAGE, UNKNOWN, and UNSPECIFIED_FAILURE.
        /// </value>
        [DataMember(Name = "health")]
        public string Health { get; set; }

        /// <summary>
        /// Gets or sets the technology.
        /// </summary>
        /// <value>
        /// Battery technology.
        /// </value>
        [DataMember(Name = "technology")]
        public string Technology { get; set; }

        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        /// <value>
        /// Possible values are CHARGING, DISCHARGING, FULL, NOT_CHARGING, and UNKNOWN.
        /// </value>
        [DataMember(Name = "status")]
        public string Status { get; set; }

        /// <summary>
        /// Gets or sets the type of the plug.
        /// </summary>
        /// <value>
        /// Possible values are AC, USB, and WIRELESS.
        /// </value>
        [DataMember(Name = "plugType")]
        public string PlugType { get; set; }

        /// <summary>
        /// Gets or sets the level.
        /// </summary>
        /// <value>
        /// Battery charge level.
        /// </value>
        [DataMember(Name = "level")]
        public int Level { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is present.
        /// </summary>
        /// <value>
        /// Indicates if a battery is available.
        /// </value>
        [DataMember(Name = "isPresent")]
        public bool IsPresent { get; set; }

        /// <summary>
        /// Gets or sets the temperature.
        /// </summary>
        /// <value>
        /// Battery temperature in tenths of a degree centigrade.
        /// </value>
        [DataMember(Name = "temperature")]
        public int Temperature { get; set; }

        /// <summary>
        /// Gets or sets the voltage.
        /// </summary>
        /// <value>
        /// Battery voltage in millivolts.
        /// </value>
        [DataMember(Name = "voltage")]
        public int Voltage { get; set; }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        override public string ToString()
        {
            string str = String.Empty;
            str = String.Concat(str, "Health = ", Health, "\r\n");
            str = String.Concat(str, "Technology = ", Technology, "\r\n");
            str = String.Concat(str, "Status = ", Status, "\r\n");
            str = String.Concat(str, "PlugType = ", PlugType, "\r\n");
            str = String.Concat(str, "Level = ", Level, "\r\n");
            str = String.Concat(str, "IsPresent = ", IsPresent, "\r\n");
            str = String.Concat(str, "Temperature = ", Temperature, "\r\n");
            str = String.Concat(str, "Voltage = ", Voltage, "\r\n");
            return str;
        }
    }
}
