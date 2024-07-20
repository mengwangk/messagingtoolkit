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
    /// Get supported message indications
    /// </summary>
    internal sealed class GetSupportedMessageIndicationFeature: BaseFeature<GetSupportedMessageIndicationFeature>, IFeature
    {

        #region ================ Private Constants =========================================================

         /// <summary>
        /// Command to retrieve the SIM status
        /// </summary>
        private const string RequestCommand = "AT+CNMI=?";

        /// <summary>
        /// Expected response. E.g. "+CNMI: (2),(0,1,3),(0,2),(0,1),(0)"
        /// </summary>
        private const string ExpectedResponse = @"\+CNMI: \(([\d,-])+\),\(([\d,-]+)\),\(([\d,-]+)\),\(([\d,-]+)\),\(([\d,-]+)\)";

        #endregion =========================================================================================


        #region ====================== Constructor =========================================================

        /// <summary>
        /// Default constructor
        /// </summary>
        private GetSupportedMessageIndicationFeature(): base()
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
                    string mode = string.Empty;
                    string deliver = string.Empty;
                    string cellBroadcast = string.Empty;
                    string statusReport = string.Empty;
                    string buffer = string.Empty;
                    foreach (Capture capture in match.Groups[1].Captures)
                    {
                        mode = mode + capture.Value;
                    }
                    foreach (Capture capture2 in match.Groups[2].Captures)
                    {
                        deliver = deliver + capture2.Value;
                    }
                    foreach (Capture capture3 in match.Groups[3].Captures)
                    {
                        cellBroadcast = cellBroadcast + capture3.Value;
                    }
                    foreach (Capture capture4 in match.Groups[4].Captures)
                    {
                        statusReport = statusReport + capture4.Value;
                    }
                    foreach (Capture capture5 in match.Groups[5].Captures)
                    {
                        buffer = buffer + capture5.Value;
                    }
                    context.PutResult(new MessageIndicationSupport(mode, deliver, cellBroadcast, statusReport, buffer));
                }
                else
                {
                    context.PutResult(new MessageIndicationSupport());
                }
            }
            else
            {
                context.PutResult(new MessageIndicationSupport());
            }
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
            return "GetSupportedMessageIndicationFeature: Get supported message indications";
        }

        #endregion ===========================================================================================

        #region =========== Public Static Methods ============================================================

        /// <summary>
        /// Return an instance of GetSupportedMessageIndicationFeature
        /// </summary>
        /// <returns>GetSupportedMessageIndicationFeature instance</returns>
        public static GetSupportedMessageIndicationFeature NewInstance()
        {
            return new GetSupportedMessageIndicationFeature();
        }

        #endregion ===========================================================================================
       
    }
}