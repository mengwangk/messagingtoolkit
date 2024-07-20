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
    /// Represents the cell location on a CDMA phone.
    /// </summary>
    [DataContract]
    public sealed class CdmaCellLocation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CdmaCellLocation"/> class.
        /// </summary>
        public CdmaCellLocation()
        {
            this.BaseStationdId = -1;
            this.BaseStationLatitude = -1;
            this.BaseStationLongitude = -1;
            this.NetworkId = -1;
            this.SystemId = -1;
        }

        /// <summary>
        /// Gets or sets the base stationd identifier.
        /// </summary>
        /// <value>
        /// CDMA base station identification number, -1 if unknown.
        /// </value>
        [DataMember(Name = "baseStationId")]
        public int BaseStationdId { get; set; }

        /// <summary>
        /// Gets or sets the base station latitude.
        /// </summary>
        /// <value>
        /// CDMA base station latitude in units of 0.25 seconds, Integer.MAX_VALUE if unknown.
        /// </value>
        [DataMember(Name = "baseStationLatitude")]
        public int BaseStationLatitude { get; set; }

        /// <summary>
        /// Gets or sets the base station latitude.
        /// </summary>
        /// <value>
        /// CDMA base station longitude in units of 0.25 seconds, Integer.MAX_VALUE if unknown.
        /// </value>
        [DataMember(Name = "baseStationLongitude")]
        public int BaseStationLongitude { get; set; }

        /// <summary>
        /// Gets or sets the network identifier.
        /// </summary>
        /// <value>
        /// CDMA network identification number, -1 if unknown.
        /// </value>
        [DataMember(Name = "networkId")]
        public int NetworkId { get; set; }

        /// <summary>
        /// Gets or sets the system identifier.
        /// </summary>
        /// <value>
        /// CDMA system identification number, -1 if unknown.
        /// </value>
        [DataMember(Name = "systemId")]
        public int SystemId { get; set; }


        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        override public string ToString()
        {
            string str = String.Empty;
            str = String.Concat(str, "BaseStationdId = ", BaseStationdId, "\r\n");
            str = String.Concat(str, "BaseStationLatitude = ", BaseStationLatitude, "\r\n");
            str = String.Concat(str, "BaseStationLongitude = ", BaseStationLongitude, "\r\n");
            str = String.Concat(str, "NetworkId = ", NetworkId, "\r\n");
            str = String.Concat(str, "SystemId = ", SystemId, "\r\n");
            return str;
        }
    }
}
