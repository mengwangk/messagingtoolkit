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

namespace MessagingToolkit.Core.Mobile.Feature
{
    /// <summary>
    /// Keep the GSM link open so that message sending can be faster
    /// </summary>
    internal sealed class BatchSmsFeature: BaseFeature<BatchSmsFeature>, IFeature
    {

        #region ================ Private Constants =========================================================

         /// <summary>
        /// Command to set the batch mode
        /// </summary>
        private const string RequestCommand = "AT+CMMS={0}";

       
        #endregion =========================================================================================


        #region ====================== Constructor =========================================================

        /// <summary>
        /// Default constructor
        /// </summary>
        private BatchSmsFeature() : base()
        {

            // This is a synchronous feature
            ExecutionBehavior = ExecutionBehavior.Synchronous;

            // Add the command
            AddCommand(Command.NewInstance(RequestCommand, Response.Ok, Preprocess, Postprocess));
        }


        #endregion ==========================================================================================


        #region =========== Private Methods =================================================================

        /// <summary>
        /// Preprocess the command
        /// </summary>
        /// <param name="context">Execution context</param>
        /// <param name="command">Command instance</param>
        /// <returns>true if successful</returns>
        private bool Preprocess(IContext context, ICommand command)
        {
            command.Request = string.Format(RequestCommand, new object[] { (int) BatchMessageMode });
            return true;
        }

        /// <summary>
        /// Parse the result
        /// </summary>
        /// <param name="context">Execution context</param>
        /// <param name="command">Command instance</param>
        /// <returns>true if successful</returns>
        private bool Postprocess(IContext context, ICommand command)
        {            
            string[] results = ResultParser.ParseResponse(context.GetData());
            return true;
        }

        #endregion ===========================================================================================

      
        #region =========== Public Properties=================================================================

        /// <summary>
        /// Message indication settings
        /// </summary>
        /// <value>Message indication settings</value>
        public BatchMessageMode BatchMessageMode
        {
            get;
            set;
        }

        #endregion ===========================================================================================


        #region =========== Public Methods ===================================================================


        /// <summary>
        /// Return the feature description
        /// </summary>
        /// <returns>Feature description</returns>
        public override string ToString()
        {
            return "BatchSmsFeature: SMS batch sending mode";
        }

        #endregion ===========================================================================================

        #region =========== Public Static Methods ============================================================

        /// <summary>
        /// Return an instance of BatchSmsFeature
        /// </summary>
        /// <returns>BatchSmsFeature instance</returns>
        public static BatchSmsFeature NewInstance()
        {
            return new BatchSmsFeature();
        }

        #endregion ===========================================================================================
       
    }
}
