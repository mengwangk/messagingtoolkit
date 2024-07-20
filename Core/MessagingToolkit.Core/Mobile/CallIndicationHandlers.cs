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
    /// Incoming call indication handlers
    /// </summary>
    public class CallIndicationHandlers
    {
        private List<UnsolicitedCall> calls = new List<UnsolicitedCall>();

        //private const string Clip = "\\+CLIP: \"(\\d+)\",(\\d+)";
        private const string Clip = "\\+CLIP: \"([^\"]*)\",(\\d+)";
        private const string ClipStart = @"\+CLIP: ";
        private const string Ring = "RING";
        private const string RingStart = "RING";
        private const string CRing = "\\+CRING: (\\w+)";
        private const string CRingStart = @"\+CRING: ";

        // For current call listing - March 3th 2017
        private const string Clcc = "\\+CLCC: (\\d+),(\\d+),(\\d+),(\\d+),(\\d+),\"(.+)\",(\\d+),\"(.*)\".*";
        private const string ClccStart = @"\+CLCC: ";

        private delegate bool UnsolicitedCompleteChecker(string input);
        private delegate IIndicationObject UnsolicitedHandler(ref string input);

        /// <summary>
        /// Constructor
        /// </summary>
        public CallIndicationHandlers()
        {
            UnsolicitedCall item = new UnsolicitedCall(Clip, new UnsolicitedHandler(this.HandleIncomingCallClip));
            item.StartPattern = ClipStart;
            item.Description = "Incoming call clip received";
            this.calls.Add(item);

            UnsolicitedCall item2 = new UnsolicitedCall(Ring, new UnsolicitedHandler(this.HandleIncomingCallRing));
            item2.StartPattern = RingStart;
            item2.Description = "Incoming ring received";
            this.calls.Add(item2);

            UnsolicitedCall item3 = new UnsolicitedCall(CRing, new UnsolicitedHandler(this.HandleIncomingCallRing));
            item3.StartPattern = CRingStart;
            item3.Description = "Incoming call ring received";
            this.calls.Add(item3);

            // March 3th 2017
            //UnsolicitedCall item4 = new UnsolicitedCall(Clcc, new UnsolicitedHandler(this.HandleIncomingCallClcc));
            //item4.StartPattern = ClccStart;
            //item4.Description = "Incoming call listing received";
            //this.calls.Add(item4);
        }

        /// <summary>
        /// Handle incoming CLIP event
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private IIndicationObject HandleIncomingCallClip(ref string input)
        {
            Match match = new Regex(Clip).Match(input);
            if (!match.Success)
            {
                throw new ArgumentException("Input string does not contain a CLIP indication.");
            }
            string number = match.Groups[1].Value;
            string numberType = match.Groups[2].Value;
            NumberType type = (NumberType)StringEnum.Parse(typeof(NumberType), numberType);
            return new CallInformation(number, type, DateTime.Now);
        }

        /// <summary>
        /// Handle incoming CLCC event
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private IIndicationObject HandleIncomingCallClcc(ref string input)
        {
            Match match = new Regex(Clcc).Match(input);
            if (!match.Success)
            {
                throw new ArgumentException("Input string does not contain a CLCC indication.");
            }
            int index = int.Parse(match.Groups[1].Value);
            string callType = match.Groups[2].Value;
            string state = match.Groups[3].Value;
            string mode = match.Groups[4].Value;
            string multiparty = match.Groups[5].Value;
            string phoneNumber = match.Groups[6].Value;
            string internationIndicator = match.Groups[7].Value;
            string contactName = match.Groups[8].Value;

            CallType callTypeEnum = (CallType)Enum.Parse(typeof(CallType), callType);
            CallState callStateEnum = CallState.Unknown;
            try
            {
                callStateEnum = (CallState)Enum.Parse(typeof(CallState), state);
            }
            catch (Exception)
            {
                // Unknown call state
                callStateEnum = CallState.Unknown;
            }

            CallMode callModeEnum = CallMode.Unknown;
            try
            {
                callModeEnum = (CallMode)Enum.Parse(typeof(CallMode), mode);
            }
            catch (Exception)
            {
                // Unknown call mode
                callModeEnum = CallMode.Unknown;
            }

            bool isMultiParty = false;
            if (StringEnum.GetStringValue(TrueFalseIndicator.True).Equals(multiparty))
            {
                isMultiParty = true;
            }
            NumberType numberTypeEnum = (NumberType)Enum.Parse(typeof(NumberType), internationIndicator);
            return new CurrentCallInformation(index, callTypeEnum, callStateEnum, callModeEnum, isMultiParty, phoneNumber, numberTypeEnum, contactName);
        }

        /// <summary>
        /// Handle incoming RING or CRING event
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private IIndicationObject HandleIncomingCallRing(ref string input)
        {
            return new CallInformation(string.Empty, NumberType.Domestic, DateTime.Now);
        }

        /// <summary>
        /// </summary>
        /// <param name="input"></param>
        /// <param name="description"></param>
        /// <returns></returns>
        public IIndicationObject HandleUnsolicitedCall(ref string input, out string description)
        {
            foreach (UnsolicitedCall call in this.calls)
            {
                if (call.IsMatch(input))
                {
                    IIndicationObject obj2 = call.Handler(ref input);
                    description = call.Description;
                    return obj2;
                }
            }
            throw new ArgumentException("Input string does not match any of the supported unsolicited calls.");
        }

        /// <summary>
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public bool IsIncompleteUnsolicitedCall(string input)
        {
            foreach (UnsolicitedCall call in this.calls)
            {
                if (call.IsStartMatch(input) && !call.IsMatch(input))
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
        public bool IsUnsolicitedCall(string input)
        {
            foreach (UnsolicitedCall call in this.calls)
            {
                if (call.IsMatch(input))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Unsolicited call 
        /// </summary>
        private class UnsolicitedCall
        {
            private CallIndicationHandlers.UnsolicitedCompleteChecker completeChecker;
            private string description;
            private CallIndicationHandlers.UnsolicitedHandler handler;
            private string pattern;
            private string startPattern;

             /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="pattern">Message pattern</param>
            /// <param name="handler">Message handler</param>
            public UnsolicitedCall(string pattern, CallIndicationHandlers.UnsolicitedHandler handler)
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

            public CallIndicationHandlers.UnsolicitedCompleteChecker CompleteChecker
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

            public CallIndicationHandlers.UnsolicitedHandler Handler
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
