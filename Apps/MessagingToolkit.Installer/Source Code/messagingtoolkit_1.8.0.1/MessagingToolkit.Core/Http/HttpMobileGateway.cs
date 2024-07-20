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


using MessagingToolkit.Core.Log;
using MessagingToolkit.Core.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MessagingToolkit.Core.Http
{
    /// <summary>
    /// Mobile HTTP gateway.
    /// </summary>
    public class HttpMobileGateway : IHttpMobileGateway
    {
        /// <summary>
        /// Store the last exception encountered
        /// </summary>
        protected Exception exception;


        /// <summary>
        /// License
        /// </summary>
        protected License license;
		

        #region constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpMobileGateway"/> class.
        /// </summary>
        /// <param name="config">The config.</param>
        public HttpMobileGateway(HttpMobileGatewayConfiguration config)
        {
            // Initialize the logger
            Logger.UseSensibleDefaults(config.LogFile, config.LogLocation, config.LogLevel, config.LogNameFormat);
            Logger.LogPrefix = LogPrefix.DtLogLevel;

            // Initialize the license
            license = new License(config);

        }

        #endregion constructors




        /// <summary>
        /// Clears the log file content
        /// </summary>
        public void ClearLog()
        {
            Logger.ClearLog();
        }


        #region =========== Public Properties =============================================================


        /// <summary>
        /// Gateway id
        /// </summary>
        /// <value>gateway id</value>
        public string Id
        {
            get;
            set;
        }

        /// <summary>
        /// Gateway status
        /// </summary>
        /// <value>Gateway status</value>
        public GatewayStatus Status
        {
            get;
            set;
        }


        /// <summary>
        /// Return the last exception encountered
        /// </summary>
        /// <value>Exception</value>
        public Exception LastError
        {
            get
            {
                return exception;
            }
        }

        /// <summary>
        /// Set the logging level.
        /// </summary>
        /// <value>LogLevel enum. See <see cref="LogLevel"/></value>
        public virtual LogLevel LogLevel
        {
            get
            {
                return Logger.LogLevel;
            }
            set
            {
                Logger.LogLevel = value;
            }
        }

        /// <summary>
        /// Log destination
        /// </summary>
        /// <value>Log destination. See <see cref="LogDestination"/></value>
        public virtual LogDestination LogDestination
        {
            get
            {
                return Logger.LogWhere;
            }
            set
            {
                Logger.LogWhere = value;
            }
        }

        /// <summary>
        /// Gets the log file.
        /// </summary>
        /// <value>The log file.</value>
        public virtual string LogFile
        {
            get
            {
                return Logger.LogPath;
            }
        }


        #endregion ===========================================================================================


        #region =========== Public Method  ===================================================================

        /// <summary>
        /// Reset the exception
        /// </summary>
        public void ClearError()
        {
            exception = null;
        }

        #endregion ===========================================================================================

    }
}
