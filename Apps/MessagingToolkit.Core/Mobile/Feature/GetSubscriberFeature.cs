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
    /// Get subscriber information
    /// </summary>
    internal sealed class GetSubscriberFeature: BaseFeature<GetSubscriberFeature>, IFeature
    {

        #region ================ Private Constants =========================================================

         /// <summary>
        /// Command to retrieve supported character sets
        /// </summary>
        private const string RequestCommand = "AT+CNUM";

        /// <summary>
        /// Expected response
        /// </summary>
        private const string ExpectedResponse = "\\+CNUM: (.*),\"(.*)\",(\\d+)(?:,(\\d+),(\\d+)(?:,(\\d+))?)?(?:\\r\\n)?";

        #endregion =========================================================================================


        #region ====================== Constructor =========================================================

        /// <summary>
        /// Default constructor
        /// </summary>
        private GetSubscriberFeature(): base()
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
                Match match = new Regex(ExpectedResponse).Match(context.GetData());
                List<Subscriber> subscriberList = new List<Subscriber>();
                while (match.Success)
                {
                    string alpha = ((match.Groups[1].Captures.Count > 0) && (match.Groups[1].Length > 0)) ? match.Groups[1].Value : string.Empty;
                    string number = match.Groups[2].Value;
                    int type = int.Parse(match.Groups[3].Value);
                    int speed = ((match.Groups[4].Captures.Count > 0) && (match.Groups[4].Length > 0)) ? int.Parse(match.Groups[4].Value) : -1;
                    int service = ((match.Groups[5].Captures.Count > 0) && (match.Groups[5].Length > 0)) ? int.Parse(match.Groups[5].Value) : -1;
                    int itc = ((match.Groups[6].Captures.Count > 0) && (match.Groups[6].Length > 0)) ? int.Parse(match.Groups[6].Value) : -1;
                    Subscriber item = new Subscriber(alpha, number, type, speed, service, itc);
                    subscriberList.Add(item);
                    match = match.NextMatch();
                }
                Subscriber[] array = new Subscriber[subscriberList.Count];
                subscriberList.CopyTo(array,0);
                context.PutResult(array);
                return true;
            }
            context.PutResult(new Subscriber[] { });
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
            return "GetSubscriberFeature: Get subscriber information";
        }

        #endregion ===========================================================================================

        #region =========== Public Static Methods ============================================================

        /// <summary>
        /// Return an instance of GetSubscriberFeature
        /// </summary>
        /// <returns>GetSubscriberFeature instance</returns>
        public static GetSubscriberFeature NewInstance()
        {
            return new GetSubscriberFeature();
        }

        #endregion ===========================================================================================
       
    }
}
   

