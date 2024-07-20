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
    /// Supported network operator
    /// </summary>
    public class SupportedNetworkOperator
    {
        private string act;
        private string longAlpha;
        private string numeric;
        private string shortAlpha;
        private NetworkOperatorStatus stat;

        /// <summary>
        /// Supported network operators
        /// </summary>
        /// <param name="status">Status</param>
        /// <param name="longAlphanumeric">Long alphanumeric</param>
        /// <param name="shortAlphanumeric">Short alphanumeric</param>
        /// <param name="numeric">Numeric</param>
        public SupportedNetworkOperator(NetworkOperatorStatus status, string longAlphanumeric, string shortAlphanumeric, string numeric)
        {
            this.stat = status;
            this.longAlpha = longAlphanumeric;
            this.shortAlpha = shortAlphanumeric;
            this.numeric = numeric;
            this.act = string.Empty;
        }

        /// <summary>
        /// Supported network operators
        /// </summary>
        /// <param name="status">Status</param>
        /// <param name="longAlphanumeric">Long alphanumeric</param>
        /// <param name="shortAlphanumeric">Short alphanumeric</param>
        /// <param name="numeric">Numeric</param>
        /// <param name="accessTechnology">Access technology</param>
        public SupportedNetworkOperator(NetworkOperatorStatus status, string longAlphanumeric, string shortAlphanumeric, string numeric, string accessTechnology)
        {
            this.stat = status;
            this.longAlpha = longAlphanumeric;
            this.shortAlpha = shortAlphanumeric;
            this.numeric = numeric;
            this.act = accessTechnology;
        }


        /// <summary>
        /// Access technology
        /// </summary>
        /// <value></value>
        public string AccessTechnology
        {
            get
            {
                return this.act;
            }
        }

        /// <summary>
        /// </summary>
        /// <value></value>
        public string LongAlphanumeric
        {
            get
            {
                return this.longAlpha;
            }
        }

        /// <summary>
        /// </summary>
        /// <value></value>
        public string Numeric
        {
            get
            {
                return this.numeric;
            }
        }

        /// <summary>
        /// </summary>
        /// <value></value>
        public string ShortAlphanumeric
        {
            get
            {
                return this.shortAlpha;
            }
        }

        /// <summary>
        /// Network operator status
        /// </summary>
        /// <value></value>
        public NetworkOperatorStatus Status
        {
            get
            {
                return this.stat;
            }
        }
    }
}
