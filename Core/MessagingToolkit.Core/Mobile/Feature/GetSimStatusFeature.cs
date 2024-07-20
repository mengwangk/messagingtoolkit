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
    /// Retrieve the SIM status. SIM can be
    /// 1. READY
    /// 2. BUSY
    /// 3. SIM PIN
    /// </summary>
    internal class GetSimStatusFeature : BaseFeature<GetSimStatusFeature>, IFeature
    {

        #region ================ Private Constants =========================================================

         /// <summary>
        /// Command to retrieve the SIM status
        /// </summary>
        private const string RequestCommand = "AT+CPIN?";

        /// <summary>
        /// Expected response
        /// </summary>
        private const string ExpectedResponse = "+CPIN";

        #endregion =========================================================================================


        #region ====================== Constructor =========================================================

        /// <summary>
        /// Default constructor
        /// </summary>
        private GetSimStatusFeature(): base()
        {

            // This is a synchronous feature
            ExecutionBehavior = ExecutionBehavior.Synchronous;

            // Add the command
            AddCommand(Command.NewInstance(RequestCommand, Response.Ok, Response.SimPinRequired, DoNothing, Postprocess));
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
            // Default to ready
            Response response = Response.Ready;

            string[] results = ResultParser.ParseResponse(context.GetData());
            if (results.Length > 0 && results[0].Contains(ExpectedResponse))
            {
                string[] cols = results[0].Split(new string[] { ":", "," }, StringSplitOptions.None);
                
                if (cols.Length > 0)
                {                    
                    string status = cols[1];
                    status = status.Replace("\"", "").Trim();

                    if (!string.IsNullOrEmpty(status))
                    {
                        if (status.Contains(StringEnum.GetStringValue(Response.Ready))) 
                        {
                            response = Response.Ready;
                        } else if (status.Contains(StringEnum.GetStringValue(Response.Busy))) 
                        {
                            response = Response.Busy;
                        } else if (status.Contains(StringEnum.GetStringValue(Response.SimPinRequired)))
                        {
                            response = Response.SimPinRequired;
                        } else if (status.Contains(StringEnum.GetStringValue(Response.SimPin2Required)))
                        {
                            response = Response.SimPin2Required;
                        }        
                    }                        
                }
            }
            context.PutResult(response);
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
           return "GetSimStatusFeature: Get the SIM card status";
        }

        #endregion ===========================================================================================

        #region =========== Public Static Methods ============================================================

        /// <summary>
        /// Return an instance of GetSimStatusFeature
        /// </summary>
        /// <returns>GetSimStatusFeature instance</returns>
        public static GetSimStatusFeature NewInstance()
        {
            return new GetSimStatusFeature();
        }

        #endregion ===========================================================================================
       
    }
}

