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

using MessagingToolkit.Core.Base;
using MessagingToolkit.Core.Mobile;

namespace MessagingToolkit.Core.Mobile.Feature
{
    /// <summary>
    /// Initialization feature
    /// </summary>
    internal class ExecuteFeature : BaseFeature<ExecuteFeature>, IFeature
    {
        #region ====================== Constructor ====================

        /// <summary>
        /// Default constructor
        /// </summary>
        private ExecuteFeature(): base()
        {
            // This is a synchronous feature
            ExecutionBehavior = ExecutionBehavior.Synchronous;

            // Add the command
            Command command = Command.NewInstance(Request, Response.Ok, Preprocess, Postprocess, true);
            command.ExceptionHandler = ExceptionHandler;
            AddCommand(command);
        }

        #endregion =======================================================

        #region =========== Public Properties ============================

        /// <summary>
        /// Request command to be sent to gateway
        /// </summary>
        /// <value>Command string</value>
        public string Request
        {
            get;
            set;
        }      

        #endregion ======================================================

        #region =========== Private Method ==============================

        /// <summary>
        /// Set the initialization string
        /// </summary>
        /// <param name="context">Execution context</param>
        /// <param name="command">Command instance</param>
        /// <returns>Just return true</returns>
        private bool Preprocess(IContext context, ICommand command)
        {
            // Set the command request
            command.Request = Request;

            return true;
        }

        /// <summary>
        /// Parse the response
        /// </summary>
        /// <param name="context"></param>
        /// <param name="command"></param>
        /// <returns></returns>
        private bool Postprocess(IContext context, ICommand command)
        {
            context.PutResult(context.GetData());
            return true;
        }

        /// <summary>
        /// Parse the response
        /// </summary>
        /// <param name="context"></param>
        /// <param name="command"></param>
        /// <param name="ex"></param>
        /// <returns></returns>
        private bool ExceptionHandler(IContext context, ICommand command, Exception ex)
        {
            context.PutResult(ex.Message);
            return true;
        }

        #endregion ======================================================

        #region =========== Public Methods ==============================


        /// <summary>
        /// Return the feature description
        /// </summary>
        /// <returns>Feature description</returns>
        public override string ToString()
        {
            return String.Format("ExecuteFeature: Send {0} to the gateway", Request);
        }

        #endregion ======================================================

        #region =========== Public Static Methods =======================

        /// <summary>
        /// Return an instance of InitializationFeature
        /// </summary>
        /// <returns>InitializationFeature instance</returns>
        public static ExecuteFeature NewInstance()
        {
            return new ExecuteFeature();
        }

        #endregion ======================================================
       
    }
}

