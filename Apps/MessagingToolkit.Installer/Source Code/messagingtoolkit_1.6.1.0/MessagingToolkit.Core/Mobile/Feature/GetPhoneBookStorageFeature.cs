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
using System.Text.RegularExpressions;

namespace MessagingToolkit.Core.Mobile.Feature
{
    /// <summary>
    /// Get phone book storages
    /// </summary>
    internal sealed class GetPhoneBookStorageFeature: BaseFeature<GetPhoneBookStorageFeature>, IFeature
    {

        #region ================ Private Constants =========================================================

         /// <summary>
        /// Command to retrieve phone book storage
        /// </summary>
        private const string RequestCommand = "AT+CPBS=?";

        /// <summary>
        /// Expected response
        /// </summary>
        private const string ExpectedResponse = "\\+CPBS: \\((?:\"(\\w+)\"(?(?!\\)),))+\\)";

        #endregion =========================================================================================


        #region ====================== Constructor =========================================================

        /// <summary>
        /// Default constructor
        /// </summary>
        private GetPhoneBookStorageFeature() : base()
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
            string[] results = ResultParser.ParseResponse(context.GetData());
            string[] phoneStorages = null;
            if (results.Length > 0)
            {                
                Match match = new Regex(ExpectedResponse).Match(results[0]);
                if (match.Success)
                {
                    int count = match.Groups[1].Captures.Count;
                    phoneStorages = new string[count];
                    for (int j = 0; j < count; j++)
                    {
                        phoneStorages[j] = match.Groups[1].Captures[j].Value;
                    }
                }              
            }
            if (phoneStorages == null) phoneStorages = new string[] { };
            context.PutResult(phoneStorages);
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
            return "GetPhoneBookStorageFeature: Get available phone book storages";
        }

        #endregion ===========================================================================================

        #region =========== Public Static Methods ============================================================

        /// <summary>
        /// Return an instance of GetPhoneBookStorageFeature
        /// </summary>
        /// <returns>GetPhoneBookStorageFeature instance</returns>
        public static GetPhoneBookStorageFeature NewInstance()
        {
            return new GetPhoneBookStorageFeature();
        }

        #endregion ===========================================================================================
       
    }
}
