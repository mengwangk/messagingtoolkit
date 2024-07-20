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
using MessagingToolkit.Core.Service;

namespace MessagingToolkit.Core.Base
{
    /// <summary>
    /// Gateway interface
    /// </summary>
    public interface IGateway
    {
        #region ========== Properties signatures ============================================================

        /// <summary>
        /// Logging level. 
        /// Refer to <see cref="LogLevel"/>
        /// </summary>
        /// <value>Logging level enum</value>
        LogLevel LogLevel
        {
            get;
            set;
        }

        /// <summary>
        /// Log destination
        /// </summary>
        /// <value>Log destination. See <see cref="LogDestination"/></value>
        LogDestination LogDestination
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the log file.
        /// </summary>
        /// <value>The log file.</value>
        string LogFile
        {
            get;
        }

        /// <summary>
        /// The last exception encountered
        /// </summary>
        /// <value>Exception</value>
        Exception LastError
        {
            get;
        }

        /// <summary>
        /// Gateway id
        /// </summary>
        /// <value>Gateway id</value>
        string Id
        {
            get;
            set;
        }

        /// <summary>
        /// Gateway status
        /// </summary>
        /// <value>Gateway status</value>
        GatewayStatus Status
        {
            get;
            set;
        }
       
        #endregion =========================================================================================

        #region ============ Method ========================================================================

        /// <summary>
        /// Clear the exception
        /// </summary>
        void ClearError();

        /// <summary>
        /// Clears the log file content
        /// </summary>
        void ClearLog();

        #endregion ========================================================================================


    }
}
