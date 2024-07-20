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
    /// Get the service center address
    /// </summary>
    /// <remarks>
    /// Get the current settings. Response in
    /// <![CDATA[
    /// +CSCA: <sca><tosca>
    /// <sca> - Service center address
    /// <tosca> - Type of address. 129 for normal address, 145 for international address
    /// (number contains '+' character)
    /// ]]>
    /// </remarks>
    internal sealed class GetScaFeature : BaseFeature<GetScaFeature>, IFeature
    {
        #region ================ Private Constants ====================

        /// <summary>
        /// Command to retrieve the service center address
        /// </summary>
        private const string RequestCommand = "AT+CSCA?";

        /// <summary>
        /// Expected response
        /// </summary>
        private const string ExpectedResponse = "+CSCA";


        #endregion ====================================================


        #region ====================== Constructor ====================

        /// <summary>
        /// Default constructor
        /// </summary>
        private GetScaFeature(): base()
        {

            // This is a synchronous feature
            ExecutionBehavior = ExecutionBehavior.Synchronous;

            // Add the command
            AddCommand(Command.NewInstance(RequestCommand, Response.Ok, DoNothing, ParseResponse));
        }


        #endregion =====================================================


        #region =========== Private Methods ============================

        /// <summary>
        /// Parse the result to get the service center address
        /// </summary>
        /// <param name="context">Execution context</param>
        /// <param name="command">Command instance</param>
        /// <returns>true if successful</returns>
        private bool ParseResponse(IContext context, ICommand command)
        {
            NumberInformation numberInformation = new NumberInformation();
            string[] results = ResultParser.ParseResponse(context.GetData());
            if (results.Length > 0 && results[0].Contains(ExpectedResponse))
            {
                string[] cols = results[0].Split(new string[] { ":", "," }, StringSplitOptions.None);
                if (cols.Length > 0)
                {                    
                    string sca = cols[1];
                    sca = sca.Replace("\"", "").Trim();

                    numberInformation.Number = sca;

                    if (cols.Length > 1)
                    {
                        string addrType = cols[2].Trim();
                        numberInformation.NumberType = (NumberType)StringEnum.Parse(typeof(NumberType), addrType);
                    }                   
                }
            }
            context.PutResult(numberInformation);
            return true;
        }

        #endregion ======================================================

        #region =========== Public Methods ==============================

       
        /// <summary>
        /// Return the feature description
        /// </summary>
        /// <returns>Feature description</returns>
        public override string ToString()
        {
           return "GetScaFeature: Get the gateway service center address";
        }

        #endregion ======================================================

        #region =========== Public Static Methods =======================

        /// <summary>
        /// Return an instance of GetScaFeature
        /// </summary>
        /// <returns>GetScaFeature instance</returns>
        public static GetScaFeature NewInstance()
        {
            return new GetScaFeature();
        }

        #endregion ======================================================
       
    }
}

