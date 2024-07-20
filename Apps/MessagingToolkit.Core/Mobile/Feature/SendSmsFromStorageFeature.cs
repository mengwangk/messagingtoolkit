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

using MessagingToolkit.Core.Mobile.Message;
using MessagingToolkit.Core.Mobile.PduLibrary;

namespace MessagingToolkit.Core.Mobile.Feature
{
    /// <summary>
    /// This feature sends SMS through the gateway
    /// </summary>
    internal class SendSmsFromStorageFeature : SendSmsFeature
    {

        #region ================ Private Constants ====================


        /// <summary>
        /// Command to write the message
        /// </summary>
        private const string WriteMessageCommand = "AT+CMGW={0}";


        /// <summary>
        /// Expected response when writing the message
        /// </summary>
        private const string WriteMessageCommandExpectedResponse = ">";


        /// <summary>
        /// Command to send the message
        /// </summary>
        private new const string SendMessageCommand = "AT+CMSS={0}";
        

        #endregion ====================================================


         #region ====================== Constructor ====================

        /// <summary>
        /// Default constructor
        /// </summary>
        private SendSmsFromStorageFeature(): base()
        {
        }


        #endregion =====================================================


        #region =========== Public Methods ==============================


        /// <summary>
        /// Send the SMS
        /// </summary>
        /// <param name="context">Execution context</param>
        /// <returns>true if successful</returns>
        /// <exception cref="GatewayException">GatewayException is thrown when there is error in execution</exception>
        public override bool Execute(IContext context)
        {
            // Do preprocessing on the message
            if (Message == null) return false;                        
            PduFactory messageFactory = PduFactory.NewInstance();
            string[] pduCodes = messageFactory.Generate(Message);

            /* for testing 
            string pduCode = "07910691930000F0317B0A81102992329000000B0CE8329BFD06DDDF72361904";
            string pduCode = "000107910691930000F0317B0A8110299232900008FF1600680065006C006C006F00200066006C006100730068";
            string pduCode = "0001F0317B0A8110299232900008FF1600680065006C006C006F00200066006C006100730068";
            Command command = Command.NewInstance(string.Format(WriteMessageCommand, GetAtLength(pduCode)), WriteMessageCommandExpectedResponse, DoNothing, DoNothing);
            AddCommand(command);
            command = Command.NewInstance(pduCode + (char)0x001A, Response.Ok, DoNothing, PostProcess);
            AddCommand(command);
            */
            
            foreach (string pduCode in pduCodes)
            {               
                int atLength = GetPduLength(Message, pduCode);
                Command command = Command.NewInstance(string.Format(WriteMessageCommand, atLength), WriteMessageCommandExpectedResponse, DoNothing, DoNothing);
                AddCommand(command);
                command = Command.NewInstance(pduCode + Convert.ToChar(26), Response.Ok, DoNothing, PostProcess);
                AddCommand(command);
            }
            // Should call BaseFeature Execute method, not SendSmsFeature
            //bool executionStatus = base.Execute(context);
            // return (int)typeof(object).GetMethod("GetHashCode").Invoke(this, null);
            // reference http://stackoverflow.com/questions/1006530/c-how-to-call-a-second-level-base-class-method-like-base-base-gethashcode

            bool executionStatus = base.BaseExecute(context);
            context.PutResult(messageIds);
            return executionStatus;
        }



        /// <summary>
        /// Return the feature description
        /// </summary>
        /// <returns>Feature description</returns>
        public override string ToString()
        {
            return "SendSmsFromStorageFeature: Send SMS";
        }

        #endregion ======================================================


        #region =========== Private Methods ============================

        /// <summary>
        /// Parse the result to get the service center address
        /// </summary>
        /// <param name="context">Execution context</param>
        /// <param name="command">Command instance</param>
        /// <returns>true if successful</returns>
        protected override bool PostProcess(IContext context, ICommand command)
        {
            string[] results = ResultParser.ParseResponse(context.GetData());
            if (results.Length > 0)
            {
                string[] cols = results[0].Split(new string[] { ":" }, StringSplitOptions.None);
                if (cols.Length > 0)
                {
                    int index = Convert.ToInt32(cols[1].Trim());
                    Command sendCommand = Command.NewInstance(string.Format(SendMessageCommand, index), Response.Ok, DoNothing, DoNothing);
                    SendCommand(context, sendCommand);
                    // Parse for the message id
                    results = ResultParser.ParseResponse(context.GetData());
                    if (results.Length > 0)
                    {
                        cols = results[0].Split(new string[] { ":" }, StringSplitOptions.None);
                        if (cols.Length > 0)
                        {
                            int messageId = Convert.ToInt32(cols[1].Trim());
                            messageIds.Add(messageId);
                        }
                    }
                }
            }
            return true;
        }

        #endregion ======================================================

        
        #region =========== Public Static Methods =======================

        /// <summary>
        /// Return an instance of SendSmsFeature
        /// </summary>
        /// <returns>SendSmsFeature instance</returns>
        public new static SendSmsFromStorageFeature NewInstance()
        {
            return new SendSmsFromStorageFeature();
        }

        #endregion =======================================================
    }
}
