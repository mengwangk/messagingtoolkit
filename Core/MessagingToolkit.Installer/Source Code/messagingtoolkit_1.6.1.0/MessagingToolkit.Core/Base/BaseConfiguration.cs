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

using MessagingToolkit.Core.Log;

namespace MessagingToolkit.Core.Base
{
    /// <summary>
    /// Base class for all configurations
    /// </summary>
    /// <typeparam name="T">Derived configuration</typeparam>
    [Serializable]
    public abstract class BaseConfiguration
    {
        /// <summary>
        /// Default log file name
        /// </summary>
        public const string DefaultLogFileName = "messagingtoolkit";

        /// <summary>
        /// Constructor
        /// </summary>
        protected BaseConfiguration()
        {
            this.LogFile = DefaultLogFileName;
            this.LogLevel = LogLevel.Error;
            this.LicenseKey = string.Empty;
            this.LogNameFormat = LogNameFormat.NameDate;
            this.LogQuotaFormat = LogQuotaFormat.NoRestriction;
            this.LogSizeMax = 0;
            this.DebugMode = false;
            this.LogLocation = string.Empty;
        }

        /// <summary>
        /// Log file location
        /// </summary>
        /// <value>Log file</value>
        [XmlAttribute]
        public string LogFile
        {
            get;
            set;
        }

        /// <summary>
        /// License key
        /// </summary>
        /// <value>License key</value>
        [XmlAttribute]
        public string LicenseKey
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the log level. <see cref="LogLevel"/>
        /// </summary>
        /// <value>The log level</value>
        [XmlAttribute]
        public LogLevel LogLevel
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the log quota format.
        /// </summary>
        /// <value>The log quota format.</value>
        [XmlAttribute]
        public LogQuotaFormat LogQuotaFormat
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the size of the log max size in KB
        /// </summary>
        /// <value>The size of the log max.</value>
        [XmlAttribute]
        public int LogSizeMax
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the log name format.
        /// </summary>
        /// <value>The log name format.</value>
        [XmlAttribute]
        public LogNameFormat LogNameFormat
        {
            get;
            set;
        }


        /// <summary>
        /// Gets or sets the log location.
        /// </summary>
        /// <value>The log location.</value>
        [XmlAttribute]
        public string LogLocation
        {
            get;
            set;
        }

        /// <summary>
        /// If set to true, a dialog box is shown with the error. Default to false
        /// </summary>
        /// <value><c>true</c> if in debug mode; otherwise, <c>false</c>.</value>
        [XmlAttribute]
        public bool DebugMode
        {
            get;
            set;
        }
    }
}
