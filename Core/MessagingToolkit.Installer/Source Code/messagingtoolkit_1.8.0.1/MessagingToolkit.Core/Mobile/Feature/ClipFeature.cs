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
    /// Turn on/off calling line identification presentation
    /// </summary>
    internal sealed class ClipFeature: BaseFeature<ClipFeature>, IFeature
    {

        #region ================ Private Constants =========================================================

         /// <summary>
        /// Command to set the batch mode
        /// </summary>
        private const string RequestCommand = "AT+CLIP={0}";

       
        #endregion =========================================================================================


        #region ====================== Constructor =========================================================

        /// <summary>
        /// Default constructor
        /// </summary>
        private ClipFeature(): base()
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
            command.Request = string.Format(RequestCommand, StringEnum.GetStringValue(Mode));
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
        /// Flag to turn on and off
        /// </summary>
        /// <value>Flag</value>
        public CapabilityMode Mode
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
            return "ClipFeature: Turn on/off CLIP";
        }

        #endregion ===========================================================================================

        #region =========== Public Static Methods ============================================================

        /// <summary>
        /// Return an instance of ClipFeature
        /// </summary>
        /// <returns>ClipFeature instance</returns>
        public static ClipFeature NewInstance()
        {
            return new ClipFeature();
        }

        #endregion ===========================================================================================
       
    }
}
