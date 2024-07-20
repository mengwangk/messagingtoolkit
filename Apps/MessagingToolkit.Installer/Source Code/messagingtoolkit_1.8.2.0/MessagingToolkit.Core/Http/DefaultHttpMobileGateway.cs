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
    public class DefaultHttpMobileGateway : IHttpMobileGateway
    {

        #region ====================== Methods =============================================

        /// <summary>
        /// Clears the log file content
        /// </summary>
        public void ClearLog() { }


       
        #endregion ===================== Methods ===========================================


        #region ======================= Properties ==========================================

        /// <summary>
        /// Return the license associated with this software
        /// </summary>
        /// <value>License</value>
        public virtual License License
        {
            get
            {
                HttpMobileGatewayConfiguration config = HttpMobileGatewayConfiguration.NewInstance();
                return new License(config);
            }
        }

        #endregion =========================== Properties ======================================



        #region ================================= events ========================================

      



        #endregion ================================ events ========================================


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
                return null;
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
                return LogLevel.Error;
            }
            set
            {

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
                return LogDestination.File;
            }
            set
            {

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
                return string.Empty;
            }
        }


        #endregion ===========================================================================================


        #region =========== Public Method  ===================================================================

        /// <summary>
        /// Reset the exception
        /// </summary>
        public void ClearError()
        {

        }

        #endregion ===========================================================================================

    }
}
