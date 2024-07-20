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
    /// Network operator information
    /// </summary>
    public class NetworkOperator
    {
        private string accessTechnology;
        private NetworkOperatorFormat format;
        private string theOperator;
        private NetworkOperatorSelectionMode selectionMode;

        /// <summary>
        /// </summary>
        /// <param name="format"></param>
        /// <param name="theOperator"></param>
        public NetworkOperator(NetworkOperatorFormat format, string theOperator)
        {
            this.format = format;
            this.theOperator = theOperator;
            this.accessTechnology = string.Empty;
        }

        /// <summary>
        /// </summary>
        /// <param name="format"></param>
        /// <param name="theOperator"></param>
        /// <param name="accessTechnology"></param>
        public NetworkOperator(NetworkOperatorFormat format, string theOperator, string accessTechnology)
        {
            this.format = format;
            this.theOperator = theOperator;
            this.accessTechnology = accessTechnology;
        }

        /// <summary>
        /// </summary>
        /// <value></value>
        public string AccessTechnology
        {
            get
            {
                return this.accessTechnology;
            }
        }

        /// <summary>
        /// </summary>
        /// <value></value>
        public NetworkOperatorFormat Format
        {
            get
            {
                return this.format;
            }
        }

        /// <summary>
        /// </summary>
        /// <value></value>
        public string OperatorInfo
        {
            get
            {
                return this.theOperator;
            }
        }

        /// <summary>
        /// </summary>
        /// <value></value>
        public NetworkOperatorSelectionMode SelectionMode
        {
            get
            {
                return this.selectionMode;
            }
            set
            {
                this.selectionMode = value;
            }
        }

    }
}
