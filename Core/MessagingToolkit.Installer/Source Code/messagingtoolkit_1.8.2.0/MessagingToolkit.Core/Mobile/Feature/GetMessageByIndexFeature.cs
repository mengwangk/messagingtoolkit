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

using MessagingToolkit.Core.Mobile.Message;
using MessagingToolkit.Core.Mobile.PduLibrary;

namespace MessagingToolkit.Core.Mobile.Feature
{
    /// <summary>
    /// Retrieve message by index
    /// </summary>
    internal sealed class GetMessageByIndexFeature: BaseFeature<GetMessageByIndexFeature>, IFeature
    {
        #region ================ Private Constants =============================================================

        /// <summary>
        /// Command to retrieve the messages
        /// </summary>
        private const string RequestCommand = "AT+CMGR={0}";


        /// <summary>
        /// Response expected for each message
        /// </summary>
        private const string ExpectedResponse = @"\+CMGR: (\d+),(\w*),(\d+)";

        #endregion ============================================================================================


        #region ====================== Constructor ============================================================

        /// <summary>
        /// Default constructor
        /// </summary>
        private GetMessageByIndexFeature() : base()
        {

            // This is a synchronous feature
            ExecutionBehavior = ExecutionBehavior.Synchronous;

            // Add the command
            AddCommand(Command.NewInstance(RequestCommand, Response.Ok, PreProcess, PostProcess));
        }


        #endregion =============================================================================================


        #region =========== Private Methods ====================================================================
        
        /// <summary>
        /// Set the type of messages to retrieve
        /// </summary>
        /// <param name="context">Execution context</param>
        /// <param name="command">Command instance</param>
        /// <returns>true if successful</returns>
        private bool PreProcess(IContext context, ICommand command)
        {
            // Set the memory type
            command.Request = string.Format(command.Request, Index);
            return true;            
        }

        /// <summary>
        /// Parse the result to get the messages
        /// </summary>
        /// <param name="context">Execution context</param>
        /// <param name="command">Command instance</param>
        /// <returns>true if successful</returns>
        private bool PostProcess(IContext context, ICommand command)
        {
            PduFactory messageFactory = PduFactory.NewInstance();
            MessageInformation message = new MessageInformation();
            string[] results = ResultParser.ParseResponse(context.GetData());
            if (results.Length > 0)
            {
                Match match = new Regex(ExpectedResponse).Match(results[0]);
                if (match.Success)
                {
                    string status = match.Groups[1].Value;
                    string alpha = match.Groups[2].Value;
                    int length = int.Parse(match.Groups[3].Value);
                    string data = results[1];

                    try
                    {
                        message = messageFactory.Decode(data);
                        message.Index = Index;
                        message.Indexes.Add(Index);
                        message.MessageStatusType = (MessageStatusType)StringEnum.Parse(typeof(MessageStatusType), status);
                    }
                    catch (Exception ex)
                    {
                        message = new MessageInformation();
                        message.Content = data;
                        message.Index = Index;
                        message.Indexes.Add(Index);
                    }
                }
            }          
            context.PutResult(message);            
            return true;
        }

        #endregion ==============================================================================================

        #region =========== Public Methods ======================================================================


        /// <summary>
        /// Return the feature description
        /// </summary>
        /// <returns>Feature description</returns>
        public override string ToString()
        {
            return "GetMessageByIndexFeature: Retrieve messages from the gateway using the message index";
        }

        #endregion ==============================================================================================



        #region =========== Public Properties ===================================================================

        /// <summary>
        /// Message index
        /// </summary>
        /// <value>Message index</value>
        public int Index
        {
            get;
            set;
        }
        #endregion =============================================================================================


        #region =========== Public Static Methods ==============================================================

        /// <summary>
        /// Return an instance of GetMessageByIndexFeature
        /// </summary>
        /// <returns>GetMessageByIndexFeature instance</returns>
        public static GetMessageByIndexFeature NewInstance()
        {
            return new GetMessageByIndexFeature();
        }

        #endregion ==============================================================================================

    }
}
