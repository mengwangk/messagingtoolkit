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
    /// Get the current phone book size
    /// </summary>
    internal sealed class GetPhoneBookSizeFeature: BaseFeature<GetPhoneBookSizeFeature>, IFeature
    {

        #region ================ Private Constants =========================================================

         /// <summary>
        /// Command to retrieve phone book size
        /// </summary>
        private const string RequestCommand = "AT+CPBR=?";

        /// <summary>
        /// Expected response
        /// </summary>
        private const string ExpectedResponse = @"\+CPBR: \((\d+)-(\d+)\)\,(\d*),(\d*)";
                
        #endregion =========================================================================================


        #region ====================== Constructor =========================================================

        /// <summary>
        /// Default constructor
        /// </summary>
        private GetPhoneBookSizeFeature() : base()
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
                    int lowerBound = int.Parse(match.Groups[1].Value);
                    int upperBound = int.Parse(match.Groups[2].Value);
                    int nLength = (match.Groups[3].Value != "") ? int.Parse(match.Groups[3].Value) : 0;
                    int tLength = (match.Groups[4].Value != "") ? int.Parse(match.Groups[4].Value) : 0;

                    PhoneBookSize phoneBookSize = new PhoneBookSize(lowerBound, upperBound, nLength, tLength);
                    context.PutResult(phoneBookSize);
                    return true;
                }
            }            
            context.PutResult(new PhoneBookSize(0,0,0,0));
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
            return "GetPhoneBookSizeFeature: Get current phone book size";
        }

        #endregion ===========================================================================================

        #region =========== Public Static Methods ============================================================

        /// <summary>
        /// Return an instance of GetPhoneBookSizeFeature
        /// </summary>
        /// <returns>GetPhoneBookSizeFeature instance</returns>
        public static GetPhoneBookSizeFeature NewInstance()
        {
            return new GetPhoneBookSizeFeature();
        }

        #endregion ===========================================================================================
       
    }
}