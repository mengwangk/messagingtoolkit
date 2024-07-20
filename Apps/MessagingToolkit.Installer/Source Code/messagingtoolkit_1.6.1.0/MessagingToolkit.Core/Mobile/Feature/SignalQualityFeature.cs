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
    /// Retrieve signal quality from the gateway
    /// 
    /// 0-1	    -110dBm or less
    //  2-30	-109dBm to -53dBm
    //  31      -51dBm or greater
    //  99	    Not known or not detectable 
    /// </summary>
    internal sealed class SignalQualityFeature: BaseFeature<SignalQualityFeature>, IFeature
    {

        #region ================ Private Constants =========================================================

         /// <summary>
        /// Command to retrieve the SIM status
        /// </summary>
        private const string RequestCommand = "AT+CSQ";


        /// <summary>
        /// Expected response
        /// </summary>
        private const string ExpectedResponse = @"\+CSQ: (\d+),(\d+)";
               

        #endregion =========================================================================================


        #region ====================== Constructor =========================================================

        /// <summary>
        /// Default constructor
        /// </summary>
        private SignalQualityFeature(): base()
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
                    int signalStrength = int.Parse(match.Groups[1].Value);
                    if (signalStrength == 99) signalStrength = 31;
                    int bitErrorRate = int.Parse(match.Groups[2].Value);                   
                    context.PutResult(new SignalQuality(signalStrength, bitErrorRate));
                    return true;
                }
            }
            context.PutResult(new SignalQuality(0,0));
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
            return "SignalQualityFeature: Get the signal quality";
        }

        #endregion ===========================================================================================

        #region =========== Public Static Methods ============================================================

        /// <summary>
        /// Return an instance of SignalQualityFeature
        /// </summary>
        /// <returns>SignalQualityFeature instance</returns>
        public static SignalQualityFeature NewInstance()
        {
            return new SignalQualityFeature();
        }

        #endregion ===========================================================================================
       
    }
}


