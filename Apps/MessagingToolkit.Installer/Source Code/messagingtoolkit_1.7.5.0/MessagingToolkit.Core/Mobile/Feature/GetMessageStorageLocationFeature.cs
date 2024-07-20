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
using System.Collections;

namespace MessagingToolkit.Core.Mobile.Feature
{
    /// <summary>
    /// Get message storage location
    /// </summary>
    internal sealed class GetMessageStorageLocationFeature: BaseFeature<GetMessageStorageLocationFeature>, IFeature
    {
        #region ================ Private Constants =============================================================
          

        /// <summary>
        /// Command to get the memory location
        /// </summary>
        private const string MemoryCommand= "AT+CPMS?";


        /// <summary>
        /// Expected response
        /// </summary>
        private const string ExpectedResponse = "+CPMS:";

        private const string StoragePattern = "\\s*\\+CPMS:\\s*";


        #endregion =============================================================================================


        #region ====================== Constructor =============================================================

        /// <summary>
        /// Default constructor
        /// </summary>
        private GetMessageStorageLocationFeature(): base()
        {

            // This is a synchronous feature
            ExecutionBehavior = ExecutionBehavior.Synchronous;

            // Add the command
            AddCommand(Command.NewInstance(MemoryCommand, Response.Ok, DoNothing, PostProcess, true));           
        }


        #endregion ============================================================================================


        #region =========== Private Methods ===================================================================

        /// <summary>
        /// Parse the result
        /// </summary>
        /// <param name="context">Execution context</param>
        /// <param name="command">Command instance</param>
        /// <returns>true if successful</returns>
        private bool PostProcess(IContext context, ICommand command)
        {
            List<string> storages = new List<string>(3);
            string[] results = ResultParser.ParseResponse(context.GetData());
            if (results.Length > 0 && results[0].Contains(ExpectedResponse))
            {
                Regex regCpms = new Regex(StoragePattern, RegexOptions.Compiled);
                string response = regCpms.Replace(results[0], "");
                response += ",";
                string[] tokens = response.Split(new string[] { "," }, StringSplitOptions.None);
                int i = 0;
                string loc;              
                while (i <= tokens.GetUpperBound(0))
                {
                    loc = tokens[i].Replace("\"", "");
                    if (!storages.Contains(loc) && !string.IsNullOrEmpty(loc)) storages.Add(loc);
                    i += 3;
                }                
            }
            string[] locations = new string[storages.Count];
            storages.CopyTo(locations, 0);
            context.PutResult(locations);
            return true;
        }

        #endregion =====================================================================================================

        #region =========== Public Methods =============================================================================


        /// <summary>
        /// Return the feature description
        /// </summary>
        /// <returns>Feature description</returns>
        public override string ToString()
        {
            return "GetMessageStorageLocationFeature: Get message storage location";
        }

        #endregion =====================================================================================================


        #region =========== Public Static Methods =======================================================================

        /// <summary>
        /// Return an instance of GetMessageStorageLocationFeature
        /// </summary>
        /// <returns>GetMessageStorageLocationFeature instance</returns>
        public static GetMessageStorageLocationFeature NewInstance()
        {
            return new GetMessageStorageLocationFeature();
        }

        #endregion ======================================================================================================

    }
}

