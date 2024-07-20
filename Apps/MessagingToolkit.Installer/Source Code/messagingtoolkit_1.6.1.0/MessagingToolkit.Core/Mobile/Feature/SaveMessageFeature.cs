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
    /// Save message to storage
    /// </summary>
    internal sealed class SaveMessageFeature: BaseFeature<SaveMessageFeature>, IFeature
    {
        #region ================ Private Variables =========================================================================

        private List<int> messageIds;

        #endregion =========================================================================================================


        #region ================ Private Constants =========================================================================

        /// <summary>
        /// Command to write the message
        /// </summary>
        private const string WriteMessageCommand = "AT+CMGW={0},{1}";


        /// <summary>
        /// Expected response when writing the message
        /// </summary>
        private const string WriteMessageCommandExpectedResponse = ">";

        
        #endregion =========================================================================================================


         #region ====================== Constructor ========================================================================

        /// <summary>
        /// Default constructor
        /// </summary>
        private SaveMessageFeature(): base()
        {
            // This is a synchronous feature
            ExecutionBehavior = ExecutionBehavior.Synchronous;

            // Message id list
            messageIds = new List<int>(1);
        }


        #endregion =========================================================================================================


        #region =========== Public Methods =================================================================================


        /// <summary>
        /// Save the message content
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
            foreach (string pduCode in pduCodes)
            {
                int atLength = GetPduLength(Message, pduCode);
                Command command = Command.NewInstance(string.Format(WriteMessageCommand, atLength, StringEnum.GetStringValue(MessageStatus)), WriteMessageCommandExpectedResponse, DoNothing, DoNothing);
                AddCommand(command);
                command = Command.NewInstance(pduCode + Convert.ToChar(26), Response.Ok, DoNothing, PostProcess);
                AddCommand(command);
            }
            bool executionStatus = base.Execute(context);
            context.PutResult(messageIds);
            return executionStatus;
        }



        /// <summary>
        /// Return the feature description
        /// </summary>
        /// <returns>Feature description</returns>
        public override string ToString()
        {
            return "SaveMessageFeature: Save message into the message storage";
        }

        #endregion ==========================================================================================================


        #region =========== Private Methods =================================================================================

        /// <summary>
        /// Parse the result to get the service center address
        /// </summary>
        /// <param name="context">Execution context</param>
        /// <param name="command">Command instance</param>
        /// <returns>true if successful</returns>
        private bool PostProcess(IContext context, ICommand command)
        {
            string[] results = ResultParser.ParseResponse(context.GetData());
            if (results.Length > 0)
            {
                string[] cols = results[0].Split(new string[] { ":" }, StringSplitOptions.None);
                if (cols.Length > 0)
                {
                    int index = Convert.ToInt32(cols[1].Trim());
                    messageIds.Add(index);
                }
            }
            return true;
          
        }

        #endregion =========================================================================================================================

        

        #region =========== Public Properties ==============================================================================================

        /// <summary>
        /// SMS property
        /// </summary>
        /// <value>SMS instance</value>
        public Sms Message
        {
            get;
            set;
        }

        /// <summary>
        /// Message status
        /// </summary>
        /// <value>Message status</value>
        public MessageStatusType MessageStatus
        {
            get;
            set;
        }

        #endregion =========================================================================================================================

        
        #region =========== Protected Methods ==============================================================================================


        /// <summary>
        /// Calculate message length
        /// </summary>
        /// <param name="sms">SMS object</param>
        /// <param name="pdu">PDU string</param>
        /// <returns>Message length</returns>
        private int GetPduLength(Sms sms, string pdu)
        {
            int pduLength = pdu.Length;
            pduLength /= 2;
            if (string.IsNullOrEmpty(sms.ServiceCenterNumber))
                pduLength--;
            else
            {
                int smscNumberLen = sms.ServiceCenterNumber.Length;
                if (sms.ServiceCenterNumber[0] == '+') smscNumberLen--;
                if (smscNumberLen % 2 != 0) smscNumberLen++;
                int smscLen = (2 + smscNumberLen) / 2;
                pduLength = pduLength - smscLen - 1;
            }
            return pduLength;
        }
      
        #endregion =========================================================================================================

        
        #region =========== Public Static Methods ==========================================================================

        /// <summary>
        /// Return an instance of SaveMessageFeature
        /// </summary>
        /// <returns>SaveMessageFeature instance</returns>
        public static SaveMessageFeature NewInstance()
        {
            return new SaveMessageFeature();
        }

        #endregion ==========================================================================================================
    }
}
