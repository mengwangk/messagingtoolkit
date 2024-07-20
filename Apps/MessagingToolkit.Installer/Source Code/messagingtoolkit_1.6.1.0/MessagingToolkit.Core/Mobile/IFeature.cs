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
using System.IO.Ports;

namespace MessagingToolkit.Core.Mobile
{
    /// <summary>
    /// Command interface
    /// </summary>
    internal interface IFeature
    {

        #region ============ Properties ===================================

        /// <summary>
        /// Indicator to show that if this feature is supported
        /// </summary>
        /// <value>true or false. true if it is supported</value>
        bool Supported
        {
            get;
            set;
        }

        /// <summary>
        /// Indicate the feature execution behavior.
        /// See <see cref="ExecutionBehavior"/>
        /// </summary>
        /// <value>Return the feature type</value>
        ExecutionBehavior ExecutionBehavior
        {
            get;
            set;
        }

        /// <summary>
        /// Physical connection to the gateway
        /// </summary>
        /// <value>Serial port. See <see cref="SerialPort"/></value>
        SerialPort Port
        {
            get;
            set;
        }

        /// <summary>
        /// Data queue for incoming data from gateway
        /// </summary>
        /// <value>Data queue</value>
        IncomingDataQueue IncomingDataQueue
        {
            get;
            set;
        }

        /// <summary>
        /// Commands to to be executed in sequence for this feature.
        /// See <see cref="Command"/>
        /// </summary>
        /// <value>List of commands</value>
        List<ICommand> Commands
        {
            get;            
        }

        /// <summary>
        /// Gets or sets the type of the command.
        /// </summary>
        /// <value>The type of the command.</value>
        FeatureCommandType CommandType
        {
            get;
            set;
        }

        /// <summary>
        /// Indicate if the command is being echo
        /// </summary>
        /// <value><c>true</c> if this instance has echo; otherwise, <c>false</c>.</value>
        bool HasEcho
        {
            get;
        }

       
        /// <summary>
        /// Indicate if the gateway requires reset
        /// </summary>
        bool RequiredReset
        {
            get;
        }

        #endregion =============================================================


        #region ============ Method ============================================

        /// <summary>
        /// Execute the feature
        /// </summary>
        /// <param name="context">Execution context</param>
        /// <returns>true if successful and execution continue, false if do not want to continue</returns>
        /// <exception cref="GatewayException">Thrown when there is execution exception</exception>
        bool Execute(IContext context);

      
        #endregion ===============================================



    }
}
