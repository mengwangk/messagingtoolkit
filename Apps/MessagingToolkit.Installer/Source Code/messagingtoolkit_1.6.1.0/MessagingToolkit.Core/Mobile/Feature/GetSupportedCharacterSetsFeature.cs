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
    /// Get a list of supported character sets
    /// </summary>
    internal sealed class GetSupportedCharacterSetsFeature: BaseFeature<GetSupportedCharacterSetsFeature>, IFeature
    {

        #region ================ Private Constants =========================================================

         /// <summary>
        /// Command to retrieve supported character sets
        /// </summary>
        private const string RequestCommand = "AT+CSCS=?";

        /// <summary>
        /// Expected response
        /// </summary>
        private const string ExpectedResponse = "\\+CSCS: \\((?:\"([^\"]+)\"(?(?!\\)),))+\\)";

        #endregion =========================================================================================


        #region ====================== Constructor =========================================================

        /// <summary>
        /// Default constructor
        /// </summary>
        private GetSupportedCharacterSetsFeature(): base()
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
                    int count = match.Groups[1].Captures.Count;
                    string[] characterSets = new string[count];
                    for (int i = 0; i < count; i++)
                    {
                        characterSets[i] = match.Groups[1].Captures[i].Value;
                    }
                    context.PutResult(characterSets);
                    return true;
                }              
            }

            context.PutResult(new string[] { });
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
            return "GetSupportedCharacterSetsFeature: Get supported character sets";
        }

        #endregion ===========================================================================================

        #region =========== Public Static Methods ============================================================

        /// <summary>
        /// Return an instance of GetSupportedCharacterSetsFeature
        /// </summary>
        /// <returns>GetSupportedCharacterSetsFeature instance</returns>
        public static GetSupportedCharacterSetsFeature NewInstance()
        {
            return new GetSupportedCharacterSetsFeature();
        }

        #endregion ===========================================================================================
       
    }
}