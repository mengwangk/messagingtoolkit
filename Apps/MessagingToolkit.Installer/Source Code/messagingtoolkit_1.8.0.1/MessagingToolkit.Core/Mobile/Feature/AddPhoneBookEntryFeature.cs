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
    /// Add phone book entry
    /// </summary>
    internal sealed class AddPhoneBookEntryFeature: BaseFeature<AddPhoneBookEntryFeature>, IFeature
    {

        #region ================ Private Constants =========================================================

         /// <summary>
        /// Command to retrieve supported character sets
        /// </summary>
        private const string RequestCommand = "AT+CPBW=,\"{0}\",,\"{1}\"";
               

        #endregion =========================================================================================


        #region ====================== Constructor =========================================================

        /// <summary>
        /// Default constructor
        /// </summary>
        private AddPhoneBookEntryFeature() : base()
        {

            // This is a synchronous feature
            ExecutionBehavior = ExecutionBehavior.Synchronous;

            // Add the command
            AddCommand(Command.NewInstance(RequestCommand, Response.Ok, Preprocess, Postprocess));
        }


        #endregion ==========================================================================================


        #region =========== Public Properties=================================================================

        /// <summary>
        /// Phone book entry
        /// </summary>
        /// <value>Phone book entry</value>
        public PhoneBookEntry PhoneBookEntry
        {
            get;
            set;
        }

        #endregion ===========================================================================================


        #region =========== Private Methods =================================================================

        /// <summary>
        /// Preprocess the command
        /// </summary>
        /// <param name="context">Execution context</param>
        /// <param name="command">Command instance</param>
        /// <returns>true if successful</returns>
        private bool Preprocess(IContext context, ICommand command)
        {
            command.Request = string.Format(RequestCommand, PhoneBookEntry.Number, PhoneBookEntry.Text);
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
            return "AddPhoneBookEntryFeature: Add phone book entry to the specified storage";
        }

        #endregion ===========================================================================================

        #region =========== Public Static Methods ============================================================

        /// <summary>
        /// Return an instance of AddPhoneBookEntryFeature
        /// </summary>
        /// <returns>AddPhoneBookEntryFeature instance</returns>
        public static AddPhoneBookEntryFeature NewInstance()
        {
            return new AddPhoneBookEntryFeature();
        }

        #endregion ===========================================================================================
       
    }
}

