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
    /// Get current message memory status
    /// </summary>
    internal sealed class GetMessageStorageStatusFeature : BaseFeature<GetMessageStorageStatusFeature>, IFeature
    {
        #region ================ Private Constants =============================================================
          

        /// <summary>
        /// Command to set the memory location
        /// </summary>
        private const string MemoryCommand= "AT+CPMS=\"{0}\"";


        /// <summary>
        /// Expected response
        /// </summary>
        private const string ExpectedResponse = @"\+CPMS: (\d+),(\d+),(\d+),(\d+)(?:,(\d+),(\d+))?";

        private const string ExpectedResponse2 = "\\+CPMS: \"(\\w+)\",(\\d+),(\\d+),\"(\\w+)\",(\\d+),(\\d+)(?:,\"(\\w+)\",(\\d+),(\\d+))?";


        #endregion =============================================================================================


        #region ====================== Constructor =============================================================

        /// <summary>
        /// Default constructor
        /// </summary>
        private GetMessageStorageStatusFeature() : base()
        {

            // This is a synchronous feature
            ExecutionBehavior = ExecutionBehavior.Synchronous;

            // Add the command
            AddCommand(Command.NewInstance(MemoryCommand, Response.Ok, PreProcess, PostProcess, true));           
        }


        #endregion ============================================================================================


        #region =========== Private Methods ===================================================================

        /// <summary>
        /// Parse the result
        /// </summary>
        /// <param name="context">Execution context</param>
        /// <param name="command">Command instance</param>
        /// <returns>true if successful</returns>
        private bool PreProcess(IContext context, ICommand command)
        {
            // Set the memory location
            command.Request = String.Format(command.Request, StringEnum.GetStringValue(MessageStorage));
            return true;
        }

        /// <summary>
        /// Parse the result
        /// </summary>
        /// <param name="context">Execution context</param>
        /// <param name="command">Command instance</param>
        /// <returns>true if successful</returns>
        private bool PostProcess(IContext context, ICommand command)
        {
            string[] results = ResultParser.ParseResponse(context.GetData());
            if (results.Length > 0)
            {
                MessageMemoryStatus status;
                Match match = new Regex(ExpectedResponse).Match(results[0]);
                if (match.Success)
                {
                    status = new MessageMemoryStatus();
                    int used = int.Parse(match.Groups[1].Value.Trim());
                    int total = int.Parse(match.Groups[2].Value.Trim());
                    int num3 = int.Parse(match.Groups[3].Value.Trim());
                    int num4 = int.Parse(match.Groups[4].Value.Trim());
                    status.ReadStorage = new MemoryStatus(used, total);
                    status.WriteStorage = new MemoryStatus(num3, num4);
                    if ((match.Groups[5].Captures.Count > 0) && (match.Groups[6].Captures.Count > 0))
                    {
                        int num5 = int.Parse(match.Groups[5].Value);
                        int num6 = int.Parse(match.Groups[6].Value);
                        status.ReceiveStorage = new MemoryStatus(num5, num6);
                    }
                    context.PutResult(status);
                    return true;
                }
                else
                {
                       match = new Regex(ExpectedResponse2).Match(results[0]);
                       if (match.Success)
                       {
                           status = new MessageMemoryStatus();
                           string storage = match.Groups[1].Value;
                           int used = int.Parse(match.Groups[2].Value);
                           int total = int.Parse(match.Groups[3].Value);
                           string str2 = match.Groups[4].Value;
                           int num3 = int.Parse(match.Groups[5].Value);
                           int num4 = int.Parse(match.Groups[6].Value);
                           status.ReadStorage = new MemoryStatusWithStorage(storage, used, total);
                           status.WriteStorage = new MemoryStatusWithStorage(str2, num3, num4);
                           if (match.Groups[7].Captures.Count > 0)
                           {
                               string str3 = match.Groups[7].Value;
                               int num5 = int.Parse(match.Groups[8].Value);
                               int num6 = int.Parse(match.Groups[9].Value);
                               status.ReceiveStorage = new MemoryStatusWithStorage(str3, num5, num6);
                           }
                           context.PutResult(status);
                           return true;
                       }
                }
            }
            context.PutResult(new MessageMemoryStatus());
            return true;
        }

        #endregion =====================================================================================================

        #region =========== Public Methods =============================================================================


        /// <summary>
        /// Return the feature description
        /// </summary>
        /// <returns>Feature description</returns>
        public override string ToString()
        {
            return "GetMessageStorageStatusFeature: Get message storage memory status";
        }

        #endregion =====================================================================================================

        #region =========== Public Properties ===========================================================================

        /// <summary>
        /// Memory location
        /// </summary>
        /// <value>Memory location</value>
        public MessageStorage MessageStorage
        {
            get;
            set;
        }

        #endregion ======================================================================================================


        #region =========== Public Static Methods =======================================================================

        /// <summary>
        /// Return an instance of GetMessageStorageStatusFeature
        /// </summary>
        /// <returns>GetMessageStorageStatusFeature instance</returns>
        public static GetMessageStorageStatusFeature NewInstance()
        {
            return new GetMessageStorageStatusFeature();
        }

        #endregion ======================================================================================================

    }
}
