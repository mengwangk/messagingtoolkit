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
    /// Send message without storing 
    /// </summary>
    internal class SendSmsFeature: BaseFeature<SendSmsFeature>, IFeature
    {
        #region ================ Private Variables ====================

        protected List<int> messageIds;

        #endregion ====================================================


        #region ================ Private Constants ====================

        /// <summary>
        /// Command to set PDU mode
        /// </summary>
        protected const string PduModeCommand = "AT+CMGF=0";

        /// <summary>
        /// Command to write the message
        /// </summary>
        protected const string SendMessageCommand = "AT+CMGS={0}";


        /// <summary>
        /// Expected response when writing the message
        /// </summary>
        protected const string SendMessageCommandExpectedResponse = ">";


        /// <summary>
        /// Expected response
        /// </summary>
        protected const string ExpectedResponse = "+CMGS";

        #endregion ====================================================


         #region ====================== Constructor ====================

        /// <summary>
        /// Default constructor
        /// </summary>
        protected SendSmsFeature(): base()
        {

            // This is a synchronous feature
            ExecutionBehavior = ExecutionBehavior.Synchronous;

            // Add the command
            //AddCommand(Command.NewInstance(PduModeCommand, Response.Ok, DoNothing, DoNothing, true));

            // Message id list
            messageIds = new List<int>(1);
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
            
            foreach (string pduCode in pduCodes)
            {               
                int atLength = GetPduLength(Message, pduCode);                
                Command command = Command.NewInstance(string.Format(SendMessageCommand, atLength), SendMessageCommandExpectedResponse, DoNothing, DoNothing);
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
            return "SendSmsFeature: Send SMS";
        }

        #endregion ======================================================


        #region =========== Protected Methods ============================

        /// <summary>
        /// Parse the result to get the service center address
        /// </summary>
        /// <param name="context">Execution context</param>
        /// <param name="command">Command instance</param>
        /// <returns>true if successful</returns>
        protected virtual bool PostProcess(IContext context, ICommand command)
        {
            string[] results = ResultParser.ParseResponse(context.GetData());
            if (results.Length > 0)
            {
                foreach (string line in results)
                {
                    if (!string.IsNullOrEmpty(line) && line.Trim().StartsWith(ExpectedResponse))
                    {
                        string[] cols = line.Split(new string[] { ":" }, StringSplitOptions.None);
                        if (cols.Length > 1)
                        {
                            int index = Convert.ToInt32(cols[1].Trim());
                            messageIds.Add(index);
                            break;
                        }
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Execute the parent execute method
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>Execution result context</returns>
        protected bool BaseExecute(IContext context)
        {
            return base.Execute(context);
        }

        #endregion ======================================================

        

        #region =========== Public Properties ============================

        /// <summary>
        /// SMS property
        /// </summary>
        /// <value>SMS instance</value>
        public Sms Message
        {
            get;
            set;
        }

        #endregion ======================================================

        
        #region =========== Protected Methods ===========================

        /// <summary>
        /// Calculate message length
        /// </summary>
        /// <param name="pduString">PDU string</param>
        /// <returns>Message length</returns>
        protected int GetAtLength(string pduString)
        {
            // Get AT command length
            return (pduString.Length - Convert.ToInt32(pduString.Substring(0, 2), 16) * 2 - 2) / 2;
        }


        /// <summary>
        /// Calculate message length
        /// </summary>
        /// <param name="sms">SMS object</param>
        /// <param name="pdu">PDU string</param>
        /// <returns>Message length</returns>
        protected int GetPduLength(Sms sms, string pdu)
        {
            int pduLength = pdu.Length;
            pduLength /= 2;
            if (string.IsNullOrEmpty(sms.ServiceCenterNumber) || sms.ServiceCenterNumber == Sms.DefaultSmscAddress || PduUtils.IsAlphaNumericAddress(sms.ServiceCenterNumber)) 
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
      
        #endregion ======================================================

        
        #region =========== Public Static Methods =======================

        /// <summary>
        /// Return an instance of SendSmsFeature
        /// </summary>
        /// <returns>SendSmsFeature instance</returns>
        public static SendSmsFeature NewInstance()
        {
            return new SendSmsFeature();
        }

        #endregion =======================================================
    }
}

