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

using MessagingToolkit.Core.Base;

namespace MessagingToolkit.Core.Mobile
{
    /// <summary>
    /// Message indication handlers
    /// </summary>
    internal class MessageIndicationHandlers
    {
        private List<UnsolicitedMessage> messages = new List<UnsolicitedMessage>();

        private const string DeliverMemoryIndication = "\\+CMTI: \"(\\w+)\",(\\d+)";
        private const string DeliverMemoryIndication2 = "\\+CMTI: \"(\\w+)\",\r\n(\\d+)";
        private const string DeliverMemoryIndication3 = "\\+CMTI:\r\n \"(\\w+)\",(\\d+)";
        private const string DeliverMemoryIndication4 = "\\+CMTI:\r\n\"(\\w+)\",(\\d+)";
        private const string DeliverMemoryIndicationStart = @"\+CMTI:";

        private const string DeliverPduModeIndication = @"\+CMT: (\w*),(\d+)\r\n(\w+)";
        private const string DeliverPduModeIndicationStart = @"\+CMT:";

        private const string StatusReportMemoryIndication = "\\+CDSI: \"(\\w+)\",(\\d+)";
        private const string StatusReportMemoryIndicationStart = @"\+CDSI:";
        private const string StatusReportPduModeIndication = @"\+CDS: (\d+)\r\n(\w+)";
        private const string StatusReportPduModeIndicationStart = @"\+CDS:";

        // For USSD
        private const string UssdResponseIndication = "\\+CUSD:\\s*(\\d+),\\s*\"([^\"]*)\"(,\\s*(\\d+))*";
        private const string UssdResponseIndicationStart = @"\+CUSD:";

        /// <summary>
        /// Constructor
        /// </summary>
        public MessageIndicationHandlers()
        {
            UnsolicitedMessage item = new UnsolicitedMessage(DeliverMemoryIndication, new UnsolicitedHandler(this.HandleDeliverMemoryIndication));
            item.StartPattern = DeliverMemoryIndicationStart;
            item.Description = "New SMS-DELIVER received (indicated by memory location)";
            this.messages.Add(item);

            UnsolicitedMessage item2 = new UnsolicitedMessage(DeliverMemoryIndication2, new UnsolicitedHandler(this.HandleDeliverMemoryIndication));
            item2.StartPattern = DeliverMemoryIndicationStart;
            item2.Description = "New SMS-DELIVER received (indicated by memory location)";
            this.messages.Add(item2);

            UnsolicitedMessage item3 = new UnsolicitedMessage(DeliverMemoryIndication3, new UnsolicitedHandler(this.HandleDeliverMemoryIndication));
            item3.StartPattern = DeliverMemoryIndicationStart;
            item3.Description = "New SMS-DELIVER received (indicated by memory location)";
            this.messages.Add(item3);

            UnsolicitedMessage item4 = new UnsolicitedMessage(DeliverMemoryIndication4, new UnsolicitedHandler(this.HandleDeliverMemoryIndication));
            item4.StartPattern = DeliverMemoryIndicationStart;
            item4.Description = "New SMS-DELIVER received (indicated by memory location)";
            this.messages.Add(item4);

            UnsolicitedMessage message2 = new UnsolicitedMessage(DeliverPduModeIndication, new UnsolicitedHandler(this.HandleDeliverPduModeIndication));
            message2.StartPattern = DeliverPduModeIndicationStart;
            message2.Description = "New SMS-DELIVER received (indicated by PDU mode version)";
            message2.CompleteChecker = new UnsolicitedCompleteChecker(this.IsCompleteDeliverPduModeIndication);
            this.messages.Add(message2);

            UnsolicitedMessage message3 = new UnsolicitedMessage(StatusReportMemoryIndication, new UnsolicitedHandler(this.HandleStatusReportMemoryIndication));
            message3.StartPattern = StatusReportMemoryIndicationStart;
            message3.Description = "New SMS-STATUS-REPORT received (indicated by memory location)";
            this.messages.Add(message3);

            UnsolicitedMessage message4 = new UnsolicitedMessage(StatusReportPduModeIndication, new UnsolicitedHandler(this.HandleStatusReportPduModeIndication));
            message4.StartPattern = StatusReportPduModeIndicationStart;
            message4.Description = "New SMS-STATUS-REPORT received (indicated by PDU mode version)";
            message4.CompleteChecker = new UnsolicitedCompleteChecker(this.IsCompleteStatusReportPduModeIndication);
            this.messages.Add(message4);

            // For USSD
            UnsolicitedMessage message5 = new UnsolicitedMessage(UssdResponseIndication, new UnsolicitedHandler(this.HandleUssdResponseIndication));
            message5.StartPattern = UssdResponseIndicationStart;
            message5.Description = "USSD response received";
            message5.CompleteChecker = new UnsolicitedCompleteChecker(this.IsCompleteUssdResponseIndication);
            this.messages.Add(message5);
        }

        /// <summary>
        /// Handle message delivery from memory location
        /// </summary>
        /// <param name="input">Input data</param>
        /// <returns>Memory location object</returns>
        private IIndicationObject HandleDeliverMemoryIndication(ref string input)
        {
            Match match = new Regex(DeliverMemoryIndication).Match(input);
            if (!match.Success)
            {
                match = new Regex(DeliverMemoryIndication2).Match(input);
                if (!match.Success)
                {
                    match = new Regex(DeliverMemoryIndication3).Match(input);
                    if (!match.Success)
                    {
                        match = new Regex(DeliverMemoryIndication4).Match(input);
                        if (!match.Success)
                            throw new ArgumentException("Input string does not contain an SMS-DELIVER memory location indication.");
                    }
                }
            }
            string storage = match.Groups[1].Value;
            int index = int.Parse(match.Groups[2].Value);
            MemoryLocation location = new MemoryLocation(MessageNotification.ReceivedMessage, storage, index);
            input = input.Remove(match.Index, match.Length);
            return location;
        }

        /// <summary>
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private IIndicationObject HandleDeliverPduModeIndication(ref string input)
        {
            Match match = new Regex(DeliverPduModeIndication).Match(input);
            if (!match.Success)
            {
                throw new ArgumentException("Input string does not contain an SMS-DELIVER PDU mode indication.");
            }
            string alpha = match.Groups[1].Value;
            int length = int.Parse(match.Groups[2].Value);
            string data = match.Groups[3].Value;
            ReceivedMessage message = new ReceivedMessage(alpha, length, data);
            input = input.Remove(match.Index, match.Length);
            return message;
        }

        /// <summary>
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private IIndicationObject HandleStatusReportMemoryIndication(ref string input)
        {
            Match match = new Regex(StatusReportMemoryIndication).Match(input);
            if (!match.Success)
            {
                throw new ArgumentException("Input string does not contain an SMS-STATUS-REPORT memory location indication.");
            }
            string storage = match.Groups[1].Value;
            int index = int.Parse(match.Groups[2].Value);
            MemoryLocation location = new MemoryLocation(MessageNotification.StatusReport, storage, index);
            input = input.Remove(match.Index, match.Length);
            return location;
        }


        /// <summary>
        /// Handle USSD response indication
        /// </summary>
        /// <param name="input">USSD response string</param>
        /// <returns>USSD response indication object</returns>
        private IIndicationObject HandleUssdResponseIndication(ref string input)
        {
            Match match = new Regex(UssdResponseIndication).Match(input);
            if (!match.Success)
            {
                throw new ArgumentException("Input string does not contain a valid USSD response indication.");
            }
            UssdResponse ussdResponse = new UssdResponse(input, string.Empty);
            return ussdResponse;
        }

        /// <summary>
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private IIndicationObject HandleStatusReportPduModeIndication(ref string input)
        {
            Match match = new Regex(StatusReportPduModeIndication).Match(input);
            if (!match.Success)
            {
                throw new ArgumentException("Input string does not contain an SMS-STATUS-REPORT PDU mode indication.");
            }
            int length = int.Parse(match.Groups[1].Value);
            string data = match.Groups[2].Value;
            ReceivedMessage message = new ReceivedMessage(string.Empty, length, data);
            input = input.Remove(match.Index, match.Length);
            return message;
        }

        /// <summary>
        /// </summary>
        /// <param name="input"></param>
        /// <param name="description"></param>
        /// <returns></returns>
        public IIndicationObject HandleUnsolicitedMessage(ref string input, out string description)
        {
            foreach (UnsolicitedMessage message in this.messages)
            {
                if (message.IsMatch(input))
                {
                    IIndicationObject obj2 = message.Handler(ref input);
                    description = message.Description;
                    return obj2;
                }
            }
            throw new ArgumentException("Input string does not match any of the supported unsolicited messages.");
        }

        /// <summary>
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private bool IsCompleteDeliverPduModeIndication(string input)
        {
            Match match = new Regex(DeliverPduModeIndication).Match(input);
            if (match.Success)
            {
                string text1 = match.Groups[1].Value;
                int num = int.Parse(match.Groups[2].Value);
                string s = match.Groups[3].Value;
                if ( (s.Length /2) > 0)
                {
                    int b = GetByte(s, 0);
                    int num3 = ((num * 2) + (b * 2)) + 2;
                    return (s.Length >= num3);
                }
            }
            return false;
        }

        /// <summary>
        /// Verify if the USSD response is complete
        /// </summary>
        /// <param name="input">USSD response</param>
        /// <returns>true if the response is complete, otherwise false</returns>
        private bool IsCompleteUssdResponseIndication(string input)
        {
            Match match = new Regex(UssdResponseIndication).Match(input);
            if (match.Success)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private bool IsCompleteStatusReportPduModeIndication(string input)
        {
            Match match = new Regex(StatusReportPduModeIndication).Match(input);
            if (match.Success)
            {
                int num = int.Parse(match.Groups[1].Value);
                string s = match.Groups[2].Value;
                if (CountBytes(s) > 0)
                {
                    int b = GetByte(s, 0);
                    int num3 = ((num * 2) + (b * 2)) + 2;
                    return (s.Length >= num3);
                }
            }
            return false;
        }

        private int CountBytes(string s)
        {
            return (s.Length / 2);
        }

        private byte GetByte(string s, int index)
        {
            return HexToInt(s.Substring(index * 2, 2))[0];
        }


        private byte[] HexToInt(string s)
        {
            byte[] buffer = new byte[s.Length / 2];
            for (int i = 0; i < (s.Length / 2); i++)
            {
                string str = s.Substring(i * 2, 2);
                buffer[i] = Convert.ToByte(str, 0x10);
            }
            return buffer;
        } 

        /// <summary>
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public bool IsIncompleteUnsolicitedMessage(string input)
        {
            foreach (UnsolicitedMessage message in this.messages)
            {
                if (message.IsStartMatch(input) && !message.IsMatch(input))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public bool IsUnsolicitedMessage(string input)
        {
            foreach (UnsolicitedMessage message in this.messages)
            {
                if (message.IsMatch(input))
                {
                    return true;
                }
            }
            return false;
        }
       
        private delegate bool UnsolicitedCompleteChecker(string input);

        private delegate IIndicationObject UnsolicitedHandler(ref string input);

        /// <summary>
        /// Unsolicited message
        /// </summary>
        private class UnsolicitedMessage
        {            
            private MessageIndicationHandlers.UnsolicitedCompleteChecker completeChecker;
            private string description;
            private MessageIndicationHandlers.UnsolicitedHandler handler;
            private string pattern;
            private string startPattern;

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="pattern">Message pattern</param>
            /// <param name="handler">Message handler</param>
            public UnsolicitedMessage(string pattern, MessageIndicationHandlers.UnsolicitedHandler handler)
            {
                this.pattern = pattern;
                this.startPattern = pattern;
                this.description = string.Empty;
                this.handler = handler;
                this.completeChecker = null;
            }

            public bool IsMatch(string input)
            {
                if (this.completeChecker != null)
                {
                    return this.completeChecker(input);
                }
                return Regex.IsMatch(input, this.pattern);
            }

            public bool IsStartMatch(string input)
            {
                return Regex.IsMatch(input, this.startPattern);
            }
                       
            public MessageIndicationHandlers.UnsolicitedCompleteChecker CompleteChecker
            {
                get
                {
                    return this.completeChecker;
                }
                set
                {
                    this.completeChecker = value;
                }
            }

            public string Description
            {
                get
                {
                    return this.description;
                }
                set
                {
                    this.description = value;
                }
            }

            public MessageIndicationHandlers.UnsolicitedHandler Handler
            {
                get
                {
                    return this.handler;
                }
                set
                {
                    this.handler = value;
                }
            }

            public string Pattern
            {
                get
                {
                    return this.pattern;
                }
                set
                {
                    this.pattern = value;
                }
            }

            public string StartPattern
            {
                get
                {
                    return this.startPattern;
                }
                set
                {
                    this.startPattern = value;
                }
            }
        }
    }
}


 

 
