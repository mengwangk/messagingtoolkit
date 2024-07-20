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
    /// Gateway command interface
    /// </summary>
    internal interface ICommand
    {
        #region ============ Properties =====================================

        /// <summary>
        /// Command to be sent
        /// </summary>
        /// <value>Command string</value>
        string Request
        {
            get;
            set;
        }

        /// <summary>
        /// Function/method to parse the actual response, returning
        /// any expected result in the context
        /// </summary>
        /// <value>
        /// Function to parse the response and send back the result in context
        /// </value>
        Func<IContext, ICommand, bool> Postprocessing
        {
            get;
        }

        /// <summary>
        /// Function/method to preprocess the request. E.g. formatting the request
        /// command to be sent
        /// </summary>
        /// <value>
        /// Function to parse the response and send back the result in context
        /// </value>
        Func<IContext, ICommand, bool> Preprocessing
        {
            get;
        }

        /// <summary>
        /// Exception handler. Handler get executed when there is 
        /// error in executing the command
        /// </summary>
        /// <value>Exception handler function</value>
        Func<IContext, ICommand, Exception, bool> ExceptionHandler
        {
            get;
            set;
        }

        /// <summary>
        /// If set to true, the command is executed and any exceptions are ignored
        /// </summary>
        /// <value>Set to true to ignore exception. Default is false</value>
        bool IgnoreError
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the expected response.
        /// </summary>
        /// <value>The expected response.</value>
        string ExpectedResponse
        {
            get;
            set;
        }

        #endregion ===========================================================

        #region =============== Method =======================================

        /// <summary>
        /// Match the response with the expected result
        /// </summary>
        /// <param name="response">Response from gateway</param>
        /// <returns>true if met, false otherwise</returns>
        bool MatchExpectedResponse(string response);

        #endregion ===========================================================

    }
}
