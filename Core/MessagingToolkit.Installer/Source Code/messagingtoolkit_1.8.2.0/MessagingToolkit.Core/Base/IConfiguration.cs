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

using MessagingToolkit.Core.Log;

namespace MessagingToolkit.Core.Base
{
    /// <summary>
    /// Configuration interface
    /// </summary>
    public interface IConfiguration
    {

        /// <summary>
        /// Log file location
        /// </summary>
        /// <value>Log file</value>
        string LogFile
        {
            get;
            set;
        }

        /// <summary>
        /// License key
        /// </summary>
        /// <value>License key</value>
        string LicenseKey
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the log level. <see cref="LogLevel"/>
        /// </summary>
        /// <value>The log level</value>
        LogLevel LogLevel
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the log quota format.
        /// </summary>
        /// <value>The log quota format.</value>
        LogQuotaFormat LogQuotaFormat
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the size of the log max size in KB
        /// </summary>
        /// <value>The size of the log max.</value>
        int LogSizeMax
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the log name format.
        /// </summary>
        /// <value>The log name format.</value>
        LogNameFormat LogNameFormat
        {
            get;
            set;
        }


        /// <summary>
        /// Gets or sets the log location.
        /// </summary>
        /// <value>The log location.</value>
        string LogLocation
        {
            get;
            set;
        }

        /// <summary>
        /// If set to true, a dialog box is shown with the error. Default to false
        /// </summary>
        /// <value><c>true</c> if in debug mode; otherwise, <c>false</c>.</value>
        bool DebugMode
        {
            get;
            set;
        }

        /// <summary>
        /// Number of times to retries if message sending failed. Default to 3
        /// </summary>
        /// <value>Send retries</value>
        int SendRetries
        {
            get;
            set;
        }

        /// <summary>
        /// Interval in milliseconds to poll for new messages.
        /// Default to 5 seconds
        /// </summary>
        /// <value>Message pollling interval in milliseconds</value>
        int MessagePollingInterval
        {
            get;
            set;
        }
    }
}
