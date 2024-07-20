﻿//===============================================================================
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
    /// Anwser a call
    /// </summary>
    internal sealed class AnswerCallFeature: BaseFeature<AnswerCallFeature>, IFeature
    {

        #region ================ Private Constants =========================================================

        /// <summary>
        /// Command to answer a call
        /// </summary>
        private const string RequestCommand = "ATA";     
               

        #endregion =========================================================================================


        #region ====================== Constructor =========================================================

        /// <summary>
        /// Default constructor
        /// </summary>
        private AnswerCallFeature() : base()
        {

            // This is a synchronous feature
            ExecutionBehavior = ExecutionBehavior.Synchronous;

            // Add the command
            AddCommand(Command.NewInstance(RequestCommand, Response.Ok, DoNothing, Postprocess));          
        }


        #endregion ==========================================================================================


        #region =========== Private Methods =================================================================
            
        /// <summary>
        /// Parse the result
        /// </summary>
        /// <param name="context">Execution context</param>
        /// <param name="command">Command instance</param>
        /// <returns>true if successful</returns>
        private bool Postprocess(IContext context, ICommand command)
        {         
            return true;
        }

        #endregion ===========================================================================================

      
        #region =========== Public Methods ===================================================================

       
        /// <summary>
        /// Return the feature description
        /// </summary>
        /// <returns>Feature description</returns>
        public override string ToString()
        {
            return "AnswerCallFeature: Answer a call";
        }

        #endregion ===========================================================================================

        #region =========== Public Static Methods ============================================================

        /// <summary>
        /// Return an instance of AnswerCallFeature
        /// </summary>
        /// <returns>AnswerCallFeature instance</returns>
        public static AnswerCallFeature NewInstance()
        {
            return new AnswerCallFeature();
        }

        #endregion ===========================================================================================
       
    }
}
