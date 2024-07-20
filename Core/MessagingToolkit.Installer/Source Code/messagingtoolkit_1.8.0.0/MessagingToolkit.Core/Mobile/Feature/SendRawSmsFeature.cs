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

namespace MessagingToolkit.Core.Mobile.Feature
{
    /// <summary>
    /// Send message without storing 
    /// </summary>
    internal class SendRawSmsFeature : SendSmsFeature   {
       

        #region ====================== Constructor ====================

        /// <summary>
        /// Default constructor
        /// </summary>
        protected SendRawSmsFeature()
            : base()
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

            // PDU is the content
            string pduCode = Message.Content;
           
            int atLength = GetPduLength(Message, pduCode);
            Command command = Command.NewInstance(string.Format(SendMessageCommand, atLength), SendMessageCommandExpectedResponse, DoNothing, DoNothing);
            AddCommand(command);
            command = Command.NewInstance(pduCode + Convert.ToChar(26), Response.Ok, DoNothing, PostProcess);
            AddCommand(command);
            
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
            return "SendRawSmsFeature: Send raw SMS";
        }

        #endregion ======================================================


        #region =========== Public Static Methods =======================

        /// <summary>
        /// Return an instance of SendRawSmsFeature
        /// </summary>
        /// <returns>SendRawSmsFeature instance</returns>
        public new static SendRawSmsFeature NewInstance()
        {
            return new SendRawSmsFeature();
        }

        #endregion =======================================================
    }
}

