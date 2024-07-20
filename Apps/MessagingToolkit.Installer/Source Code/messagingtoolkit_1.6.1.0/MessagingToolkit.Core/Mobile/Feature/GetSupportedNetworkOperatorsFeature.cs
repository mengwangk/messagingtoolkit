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
    /// Get supported network operators
    /// </summary>
    internal sealed class GetSupportedNetworkOperatorsFeature: BaseFeature<GetSupportedNetworkOperatorsFeature>, IFeature
    {

        #region ================ Private Constants =========================================================

         /// <summary>
        /// Command to retrieve the SIM status
        /// </summary>
        private const string RequestCommand = "AT+COPS=?";

        /// <summary>
        /// Expected response
        /// </summary>
        private const string ExpectedResponse = @"\+COPS: .*";

        /// <summary>
        /// Operator pattern to look for
        /// </summary>
        private const string OperatorPattern = "\\((\\d+),(?:\"([^\\(\\)\\,]+)\")?,(?:\"([^\\(\\)\\,]+)\")?,(?:\"(\\d+)\")?(?:,([^\\(\\)\\,]+))?\\)";

        #endregion =========================================================================================


        #region ====================== Constructor =========================================================

        /// <summary>
        /// Default constructor
        /// </summary>
        private GetSupportedNetworkOperatorsFeature() : base()
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
                ArrayList operatorList = new ArrayList();              
                if (Regex.IsMatch(results[0],ExpectedResponse))
                {
                    string data = context.GetData();
                    Regex regEx = new Regex(OperatorPattern);
                    for (Match match = regEx.Match(data); match.Success; match = match.NextMatch())
                    {
                        int num = int.Parse(match.Groups[1].Value);
                        string longAlphanumeric = match.Groups[2].Value;
                        string shortAlphanumeric = match.Groups[3].Value;
                        string numeric = match.Groups[4].Value;
                        string accessTechnology = match.Groups[5].Value;
                        NetworkOperatorStatus status = (NetworkOperatorStatus)Enum.Parse(typeof(NetworkOperatorStatus), num.ToString());
                        SupportedNetworkOperator info = new SupportedNetworkOperator(status, longAlphanumeric, shortAlphanumeric, numeric, accessTechnology);
                        operatorList.Add(info);
                    }
                    SupportedNetworkOperator[] operatorArray = new SupportedNetworkOperator[operatorList.Count];
                    operatorList.CopyTo(operatorArray, 0);
                    context.PutResult(operatorArray);
                    return true;
                }
            }
            context.PutResult(new SupportedNetworkOperator[]{});
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
            return "GetSupportedNetworkOperatorsFeature: Get supported network operators";
        }

        #endregion ===========================================================================================

        #region =========== Public Static Methods ============================================================

        /// <summary>
        /// Return an instance of GetSupportedNetworkOperatorsFeature
        /// </summary>
        /// <returns>GetSupportedNetworkOperatorsFeature instance</returns>
        public static GetSupportedNetworkOperatorsFeature NewInstance()
        {
            return new GetSupportedNetworkOperatorsFeature();
        }

        #endregion ===========================================================================================
       
    }
}


   
