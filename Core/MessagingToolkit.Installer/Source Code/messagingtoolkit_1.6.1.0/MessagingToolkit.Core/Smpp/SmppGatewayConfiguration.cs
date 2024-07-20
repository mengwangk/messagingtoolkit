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
using System.Xml.Serialization;

using MessagingToolkit.Core.Base;

namespace MessagingToolkit.Core.Smpp
{
    /// <summary>
    /// SMPP gateway configuration
    /// </summary>
    [System.Reflection.ObfuscationAttribute(Feature = "renaming", ApplyToMembers = true)]
    [Serializable]
    public class SmppGatewayConfiguration : BaseConfiguration, IConfiguration
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="SmppGatewayConfiguration"/> class.
        /// </summary>
        private SmppGatewayConfiguration()
            : base() 
        {
            Port = SmppGateway.DefaultPort;
            BindType = BindingType.BindAsTransceiver;
            Host = SmppGateway.DefaultHost;
            NpiType = NpiType.ISDN;
            TonType = TonType.International;
            Version = SmppVersionType.Version3_4;
            AddressRange = string.Empty;
            Password = string.Empty;
            SystemId = string.Empty;
            SystemType = string.Empty;
            EnquireLinkInterval = 0;    // Disable the timer
            SleepTimeAfterSocketFailure = SmppGateway.DefaultSleepTimeAfterSocketFailure;
            RetryAfterSocketFailure = SmppGateway.DefaultRetryAfterSocketFailure;
        }

        /// <summary>
        /// Gets or sets the port.
        /// </summary>
        /// <value>The port.</value>
        [XmlAttribute]
        public short Port 
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the type of the bind.
        /// </summary>
        /// <value>The type of the bind.</value>
        [XmlAttribute]
        public BindingType BindType 
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the host.
        /// </summary>
        /// <value>The host.</value>
        [XmlAttribute]
        public string Host 
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the type of the npi.
        /// </summary>
        /// <value>The type of the npi.</value>
        [XmlAttribute]
        public NpiType NpiType
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the type of the ton.
        /// </summary>
        /// <value>The type of the ton.</value>
        [XmlAttribute]
        public TonType TonType
        {
            get;
            set;
        }


        /// <summary>
        /// Gets or sets the version.
        /// </summary>
        /// <value>The version.</value>
        [XmlAttribute]
        public SmppVersionType  Version
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the address range.
        /// </summary>
        /// <value>The address range.</value>
        [XmlAttribute]
        public string AddressRange
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        /// <value>The password.</value>
        [XmlAttribute]
        public string Password
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the system id.
        /// </summary>
        /// <value>The system id.</value>
        [XmlAttribute]
        public string SystemId
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the type of the system.
        /// </summary>
        /// <value>The type of the system.</value>
        [XmlAttribute]
        public string SystemType
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the enquire link interval.
        /// </summary>
        /// <value>The enquire link interval.</value>
        [XmlAttribute]
        public int EnquireLinkInterval
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the sleep time after socket failure.
        /// </summary>
        /// <value>The sleep time after socket failure.</value>
        [XmlAttribute]
        public int SleepTimeAfterSocketFailure
        {
            get;
            set;
        }

        
        /// <summary>
        /// Number of retry after socket failure
        /// </summary>
        /// <value>The retry after socket failure.</value>
        [XmlAttribute]
        public int RetryAfterSocketFailure
        {
            get;
            set;
        }



        #region ============== Factory method   ===============================================================
        
        /// <summary>
        /// Static factory to create the SMPP gateway configuration
        /// </summary>
        /// <returns>A new instance of SMPP gateway configuration</returns>
        public static SmppGatewayConfiguration NewInstance()
        {
            return new SmppGatewayConfiguration();
        }

        #endregion ===========================================================================================

    }
}
