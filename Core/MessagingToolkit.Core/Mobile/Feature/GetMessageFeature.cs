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
using MessagingToolkit.Core.Mobile;
using MessagingToolkit.Core.Log;

namespace MessagingToolkit.Core.Mobile.Feature
{
    /// <summary>
    /// This feature retrieves messages from specified memory location in the gateway
    /// </summary>
    /// 
    /// <remarks>
    /// <![CDATA[
    /// AT+CMGF=1;+CMGL="ALL", "REC READ", "REC UNREAD"
    /// AT+CMGL=0-4 , 0 = "REC UNREAD", 1 = "REC READ", 2="STO UNSENT", 3 = "STO SENT", 4="ALL"
    /// ]]>
    /// </remarks>
    internal class GetMessageFeature : BaseFeature<GetMessageFeature>, IFeature
    {
        #region ================ Private Constants ====================

        /// <summary>
        /// Command to retrieve the messages
        /// </summary>
        private const string RequestCommand = "AT+CMGL={0}";


        /// <summary>
        /// Response expected for each message
        /// </summary>
        private const string ExpectedResponse = "+CMGL:";

        #endregion ====================================================


        #region ====================== Constructor ====================

        /// <summary>
        /// Default constructor
        /// </summary>
        private GetMessageFeature(): base()
        {

            // This is a synchronous feature
            ExecutionBehavior = ExecutionBehavior.Synchronous;

            // Add the command
            AddCommand(Command.NewInstance(RequestCommand, Response.Ok, PreProcess, PostProcess));

            // Default to true
            this.Sort = true;
        }


        #endregion =====================================================


        #region =========== Private Methods ============================
        
        /// <summary>
        /// Set the type of messages to retrieve
        /// </summary>
        /// <param name="context">Execution context</param>
        /// <param name="command">Command instance</param>
        /// <returns>true if successful</returns>
        private bool PreProcess(IContext context, ICommand command)
        {
            // Set the memory type
            command.Request = string.Format(command.Request, StringEnum.GetStringValue(MessageStatusType));
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
            List<MessageInformation> messages = new List<MessageInformation>(1);
            string[] results = ResultParser.ParseResponse(context.GetData());
            int lineNo = 0;
            foreach (string line in results)
            {
                int messageIndex = 0;
                int status = 0;
                if (line.Contains(ExpectedResponse))
                {
                    string[] cols = line.Split(new string[]{":",","}, StringSplitOptions.None);
                    
                    // Get message index
                    if (cols.Length > 0)
                    {
                        messageIndex = Convert.ToInt32(cols[1].Trim());
                    }
                    if (cols.Length > 1)
                    {
                        status = Convert.ToInt32(cols[2].Trim());
                    }
                    string pduCode = results[lineNo + 1].Trim();
                    try
                    {
                        MessageInformation messageInformation = messageFactory.Decode(pduCode);
                        messageInformation.Index = messageIndex;
                        messageInformation.Indexes.Add(messageIndex);
                        //messageInformation.MessageStatusType = this.MessageStatusType;
                        messageInformation.MessageStatusType = (MessageStatusType)StringEnum.Parse(typeof(MessageStatusType), Convert.ToString(status));
                        messageInformation.Status = status;
                        messages.Add(messageInformation);
                    }
                    catch (Exception ex)
                    {
                        /*
                        // If cannot decode, then just put the original content 
                        MessageInformation messageInformation = new MessageInformation();
                        messageInformation.Content = pduCode;
                        messageInformation.Index = messageIndex;
                        messageInformation.Indexes.Add(messageIndex);
                        //messageInformation.MessageStatusType = this.MessageStatusType;
                        messageInformation.MessageStatusType = (MessageStatusType)StringEnum.Parse(typeof(MessageStatusType), Convert.ToString(status));
                        messageInformation.Status = status;
                        messages.Add(messageInformation);
                        */
                        Logger.LogThis("Unable to decode: " + pduCode, LogLevel.Warn);
                    }
                }
                lineNo++;
            }
            List<List<MessageInformation>> messageLookup = new List<List<MessageInformation>>(5);
            if (ConcatenateMessage)
            {
                List<MessageInformation> tmpMessageList = new List<MessageInformation>(messages.Count);
                foreach (MessageInformation message in messages)
                {
                    //if (message.TotalPiece > 1 && message.ReferenceNo != 0)
                    if (message.TotalPiece > 1)
                    {
                        bool found = false;
                        bool duplicate = false;
                        bool addToList = true;
                        foreach (List<MessageInformation> messageList in messageLookup)
                        {
                            MessageInformation tmpMsg = messageList[0];
                            if (tmpMsg.ReferenceNo == message.ReferenceNo)
                            {
                                duplicate = false;
                                foreach (MessageInformation listMsg in messageList)
                                {
                                    if (listMsg.CurrentPiece == message.CurrentPiece)
                                    {
                                        duplicate = true;

                                        // Additional verification
                                        if (listMsg.ReceivedDate.Equals(message.ReceivedDate))
                                        {
                                            addToList = false;
                                        }

                                        break;
                                    }
                                }
                                if (!duplicate)
                                {
                                    messageList.Add(message);
                                    found = true;
                                    break;
                                }
                            }
                        }
                        if (!found && addToList)
                        {
                            List<MessageInformation> tmpList = new List<MessageInformation>();
                            tmpList.Add(message);
                            messageLookup.Add(tmpList);
                        }
                    }
                    else
                    {
                        tmpMessageList.Add(message);
                    }
                }

                // Build messages from message lookup
                foreach (List<MessageInformation> msgList in messageLookup)
                {
                    List<MessageInformation> orderList = msgList.OrderByDescending(message => message.CurrentPiece).ToList();
                    MessageInformation lastMsg = orderList[0];
                    for (int i = 1; i< orderList.Count(); i++)
                    {
                        MessageInformation msg = orderList[i];
                        lastMsg.Content = msg.Content + lastMsg.Content;
                        lastMsg.Indexes.Add(msg.Index);
                        lastMsg.TotalPieceReceived++;
                        lastMsg.RawMessage = msg.RawMessage + "\r\n" + lastMsg.RawMessage;
                        List<byte> userDataBytes = msg.DataBytes;
                        userDataBytes.AddRange(lastMsg.DataBytes);
                        lastMsg.DataBytes = userDataBytes;

                        if (msg.MessageStatusType == MessageStatusType.ReceivedUnreadMessages)
                        {
                            lastMsg.MessageStatusType = MessageStatusType.ReceivedUnreadMessages;
                        }
                    }

                    // Perform additional processing for smart messages
                    if (lastMsg.DestinationPort == MmsConstants.MmsNotificationDestinationPort &&
                        !string.IsNullOrEmpty(lastMsg.Content))
                    {
                        MessageInformation msgInfo = PduDecoder.DecodeWapMms(lastMsg.DataBytes.ToArray());
                        if (msgInfo != null)
                        {
                            // Copy from lastMsg to this new msg object
                            msgInfo.Content = lastMsg.Content;
                            msgInfo.CurrentPiece = lastMsg.CurrentPiece;
                            msgInfo.DataBytes = lastMsg.DataBytes;
                            msgInfo.DeliveryStatus = lastMsg.DeliveryStatus;
                            msgInfo.DestinationPort = lastMsg.DestinationPort;
                            msgInfo.DestinationReceivedDate = lastMsg.DestinationReceivedDate;
                            msgInfo.GatewayId = lastMsg.GatewayId;
                            msgInfo.Index = lastMsg.Index;
                            msgInfo.Indexes = lastMsg.Indexes;
                            msgInfo.MessageStatusType = lastMsg.MessageStatusType;
                            msgInfo.MessageType = lastMsg.MessageType;
                            msgInfo.PhoneNumber = lastMsg.PhoneNumber;
                            msgInfo.RawMessage = lastMsg.RawMessage;
                            msgInfo.ReceivedDate = lastMsg.ReceivedDate;
                            msgInfo.ReferenceNo = lastMsg.ReferenceNo;
                            msgInfo.SourcePort = lastMsg.SourcePort;
                            msgInfo.Status = lastMsg.Status;
                            msgInfo.Timezone = lastMsg.Timezone;
                            msgInfo.TotalPiece = lastMsg.TotalPiece;
                            msgInfo.TotalPieceReceived = lastMsg.TotalPieceReceived;
                            msgInfo.ValidityTimestamp = lastMsg.ValidityTimestamp;
                            msgInfo.ServiceCentreAddress = lastMsg.ServiceCentreAddress;
                            msgInfo.ServiceCentreAddressType = lastMsg.ServiceCentreAddressType;

                            // Add to tmpMessageList
                            tmpMessageList.Add(msgInfo);
                            continue;
                        }
                    }
                    // Add to tmpMessageList
                    tmpMessageList.Add(lastMsg);
                }

                if (this.Sort)
                {
                    context.PutResult(tmpMessageList.OrderBy(message => message.Index).ToList());
                }
                else
                {
                    context.PutResult(tmpMessageList);
                }
            }
            else
            {
                context.PutResult(messages);
            }
            
            return true;
        }

        #endregion ======================================================

        #region =========== Public Methods ==============================


        /// <summary>
        /// Return the feature description
        /// </summary>
        /// <returns>Feature description</returns>
        public override string ToString()
        {
            return "GetMessageFeature: Retrieve messages from the gateway";
        }

        #endregion ======================================================



        #region =========== Public Properties ============================

        /// <summary>
        /// Memory type
        /// </summary>
        /// <value>Memory type</value>
        public MessageStatusType MessageStatusType
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether toconcatenate message.
        /// </summary>
        /// <value><c>true</c> if concatenate message; otherwise, <c>false</c>.</value>
        public bool ConcatenateMessage
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the messages are sorted by the index
        /// </summary>
        /// <value><c>true</c> if sort; otherwise, <c>false</c>.</value>
        public bool Sort
        {
            get;
            set;
        }

        #endregion ======================================================


        #region =========== Public Static Methods =======================

        /// <summary>
        /// Return an instance of GetMessageFeature
        /// </summary>
        /// <returns>GetMessageFeature instance</returns>
        public static GetMessageFeature NewInstance()
        {
            return new GetMessageFeature();
        }

        #endregion ======================================================

    }
}
