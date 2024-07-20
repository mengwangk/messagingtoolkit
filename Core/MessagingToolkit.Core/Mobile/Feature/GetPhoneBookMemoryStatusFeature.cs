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
    /// Get current phone book memory status
    /// </summary>
    internal sealed class GetPhoneBookMemoryStatusFeature: BaseFeature<GetPhoneBookMemoryStatusFeature>, IFeature
    {

        #region ================ Private Constants =========================================================

         /// <summary>
        /// Command to retrieve phone book memory status
        /// </summary>
        private const string RequestCommand = "AT+CPBS?";

        /// <summary>
        /// Expected response
        /// </summary>
        private const string ExpectedResponse = "\\+CPBS: \"(\\w+)\",(\\d+),(\\d+)";
                
        #endregion =========================================================================================


        #region ====================== Constructor =========================================================

        /// <summary>
        /// Default constructor
        /// </summary>
        private GetPhoneBookMemoryStatusFeature(): base()
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
            if (results.Length > 0)
            {
                Match match = new Regex(ExpectedResponse).Match(results[0]);
                if (match.Success)
                {
                    string storage = match.Groups[1].Value;
                    int used = int.Parse(match.Groups[2].Value);
                    MemoryStatusWithStorage memoryStatusWithStorage = 
                        new MemoryStatusWithStorage(storage, used, int.Parse(match.Groups[3].Value));
                    context.PutResult(memoryStatusWithStorage);
                    return true;
                }
            }            
            context.PutResult(new MemoryStatusWithStorage(string.Empty,0,0));
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
            return "GetPhoneBookMemoryStatusFeature: Get current phone book memory status";
        }

        #endregion ===========================================================================================

        #region =========== Public Static Methods ============================================================

        /// <summary>
        /// Return an instance of GetPhoneBookMemoryStatusFeature
        /// </summary>
        /// <returns>GetPhoneBookMemoryStatusFeature instance</returns>
        public static GetPhoneBookMemoryStatusFeature NewInstance()
        {
            return new GetPhoneBookMemoryStatusFeature();
        }

        #endregion ===========================================================================================
       
    }
}
