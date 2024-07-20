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
    /// Get current network operator
    /// </summary>
    internal sealed class GetNetworkOperatorFeature: BaseFeature<GetNetworkOperatorFeature>, IFeature
    {

        #region ================ Private Constants =========================================================

         /// <summary>
        /// Command to retrieve the SIM status
        /// </summary>
        private const string RequestCommand = "AT+COPS?";

        /// <summary>
        /// Expected response
        /// </summary>
        private const string ExpectedResponse = "\\+COPS: (\\d+)(?:,(\\d+),\"(.+)\")?(?:,(.+))?";

        #endregion =========================================================================================


        #region ====================== Constructor =========================================================

        /// <summary>
        /// Default constructor
        /// </summary>
        private GetNetworkOperatorFeature() : base()
        {

            // This is a synchronous feature
            ExecutionBehavior = ExecutionBehavior.Synchronous;

            // Add the command
            AddCommand(Command.NewInstance(RequestCommand, Response.Ok, DoNothing, Postprocess));
        }


        #endregion ==========================================================================================


        #region =========== Private Methods =================================================================

        /// <summary>
        /// Parse the result to get the service center address
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
                    int selectionMode = int.Parse(match.Groups[1].Value);
                    if (match.Groups.Count > 1)
                    {
                        int num = int.Parse(match.Groups[2].Value);
                        string theOperator = match.Groups[3].Value;
                        string accessTechnology = string.Empty;
                        if (match.Groups.Count > 3)
                        {
                            accessTechnology = match.Groups[4].Value;
                        }
                        NetworkOperator networkOperator =
                            new NetworkOperator((NetworkOperatorFormat)Enum.Parse(typeof(NetworkOperatorFormat), num.ToString()), theOperator, accessTechnology);
                        networkOperator.SelectionMode = (NetworkOperatorSelectionMode)Enum.Parse(typeof(NetworkOperatorSelectionMode), selectionMode.ToString());
                        context.PutResult(networkOperator);
                        return true;
                    }                   
                }
            }
            context.PutResult(new NetworkOperator(NetworkOperatorFormat.LongFormatAlphanumeric, string.Empty));
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
            return "GetNetworkOperatorFeature: Get current network operator";
        }

        #endregion ===========================================================================================

        #region =========== Public Static Methods ============================================================

        /// <summary>
        /// Return an instance of GetNetworkOperatorFeature
        /// </summary>
        /// <returns>GetNetworkOperatorFeature instance</returns>
        public static GetNetworkOperatorFeature NewInstance()
        {
            return new GetNetworkOperatorFeature();
        }

        #endregion ===========================================================================================
       
    }
}

