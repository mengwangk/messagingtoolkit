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

using MessagingToolkit.Core;
using MessagingToolkit.Core.Mobile;

namespace MessagingToolkit.Core.Mobile.Feature
{
    /// <summary>
    /// Get the network registration status
    /// </summary>
    internal sealed class NetworkRegistrationFeature: BaseFeature<NetworkRegistrationFeature>, IFeature
    {

        #region ================ Private Constants =========================================================

         /// <summary>
        /// Command to retrieve the SIM status
        /// </summary>
        private const string RequestCommand = "AT+CREG?";


        /// <summary>
        /// Expected response
        /// </summary>
        private const string ExpectedResponse = "+CREG";
               

        #endregion =========================================================================================


        #region ====================== Constructor =========================================================

        /// <summary>
        /// Default constructor
        /// </summary>
        private NetworkRegistrationFeature() : base()
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
            NetworkRegistration networkRegistration = new NetworkRegistration();

            if (results.Length > 0 && results[0].Contains(ExpectedResponse))
            {
                string[] cols = results[0].Split(new string[] { ":", "," }, StringSplitOptions.None);

                string resultCode = string.Empty;
                string status = string.Empty;

                if (cols.Length > 0)
                {
                    resultCode = cols[1].Trim();
                }
                if (cols.Length > 1) 
                {
                    status = cols[2].Trim();
                }              

                if (!string.IsNullOrEmpty(status))
                {
                    networkRegistration.Status = (NetworkRegistrationStatus)StringEnum.Parse(typeof(NetworkRegistrationStatus), status);
                }

                if (!string.IsNullOrEmpty(resultCode))
                {
                    networkRegistration.UnsolicitedResultCode = (NetworkCapability)StringEnum.Parse(typeof(NetworkCapability), resultCode);
                }
            }
            context.PutResult(networkRegistration);
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
            return "NetworkRegistrationFeature: Get the network registration status";
        }

        #endregion ===========================================================================================

        #region =========== Public Static Methods ============================================================

        /// <summary>
        /// Return an instance of NetworkRegistrationFeature
        /// </summary>
        /// <returns>NetworkRegistrationFeature instance</returns>
        public static NetworkRegistrationFeature NewInstance()
        {
            return new NetworkRegistrationFeature();
        }

        #endregion ===========================================================================================
       
    }
}


