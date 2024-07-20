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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace MessagingToolkit.Core.Mobile.Feature
{
    /// <summary>
    /// Get current calls.
    /// <para>
    /// Use the CLCC command
    /// 
    /// <![CDATA[
    /// [
    /// +CLCC: <id1>,<dir>,<stat>,<mode>,<mpty>[,<number>,<type>[,<alpha>]][<cr><lf>
    /// +CLCC: <id2>,<dir>,<stat>,<mode>,<mpty>[,<number>,<type>[,<alpha>]][...]]]
    /// 
    /// ]]>
    /// 
    /// </para>
    /// <para>
    /// <![CDATA[
    /// <idx>: (call identification number, this number can be used in AT+CHLD command
    /// operations)
    /// <dir>:
    ///     0 mobile originated(MO) call
    ///     1 mobile terminated(MT) call
    /// <stat>: (state of the call)
    ///         0 active
    ///         1 held
    ///         2 dialling(MO call)
    ///         3 alerting(MO call)
    ///         4 incoming(MT call)
    ///         5 waiting(MT call)
    /// <mode>: (bearer/teleservice)
    ///         0 voice
    ///         1 data
    ///         9 unknown
    /// <mpty>:
    ///         0 call is not one of multiparty(conference) call parties
    ///         1 call is one of multiparty(conference) call parties
    /// <number>: (phone number in format specified by<type>, within "quotes")
    /// <type>: (type of number)
    ///         129 dial string without the international access character
    ///         145 dial string which includes the international access character "+"
    /// <alpha>: (alphanumeric representation of<number> corresponding to the entry found in phonebook)
    /// 
    /// Examples:
    /// Ring Progress : +CLCC: 1,0,3,0,0,"085793001974",129,"" 
    /// Accepted CAll : +CLCC: 1,0,0,0,0,"085793001974",129,"" 
    /// Reject Call : +CLCC: 1,0,6,0,0,"085793001974",129,""
    /// 
    /// ]]>
    /// 
    /// </para>
    /// </summary>
    internal sealed class GetCurrentCallsFeature : BaseFeature<GetCurrentCallsFeature>, IFeature
    {

        #region ================ Private Constants =========================================================

        /// <summary>
        /// Command to retrieve the SIM status
        /// </summary>
        private const string RequestCommand = "AT+CLCC";

        /// <summary>
        /// Expected response
        /// </summary>
        private const string ExpectedResponse = "+CLCC: ";

        /// <summary>
        /// Operator pattern to look for
        /// </summary>
        private const string CallPattern = "(\\d+),(\\d+),(\\d+),(\\d+),(\\d+),\"(.+)\",(\\d+),\"(.*)\".*\\r\\n";

        #endregion =========================================================================================


        #region ====================== Constructor =========================================================

        /// <summary>
        /// Default constructor
        /// </summary>
        private GetCurrentCallsFeature() : base()
        {

            // This is a synchronous feature
            ExecutionBehavior = ExecutionBehavior.Synchronous;

            // Add the command
            AddCommand(Command.NewInstance(RequestCommand, Response.Ok, DoNothing, Postprocess));
        }


        #endregion ==========================================================================================


        #region =========== Private Methods =================================================================

        /// <summary>
        /// Parse the result to get the service center address
        /// </summary>
        /// <param name="context">Execution context</param>
        /// <param name="command">Command instance</param>
        /// <returns>true if successful</returns>
        private bool Postprocess(IContext context, ICommand command)
        {
            ArrayList list = new ArrayList();
            Regex regex = new Regex(Regex.Escape(ExpectedResponse) + CallPattern);
            for (Match match = regex.Match(context.GetData()); match.Success; match = match.NextMatch())
            {
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
                list.Add(new CurrentCallInformation(index, callTypeEnum, callStateEnum, callModeEnum, isMultiParty, phoneNumber, numberTypeEnum, contactName));
            }
            CurrentCallInformation[] callInfos = new CurrentCallInformation[list.Count];
            list.CopyTo(callInfos, 0);
            context.PutResult(callInfos);
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
            return "GetCurrentCallsFeature: Get list of current calls";
        }

        #endregion ===========================================================================================

        #region =========== Public Static Methods ============================================================

        /// <summary>
        /// Return an instance of GetCurrentCallsFeature
        /// </summary>
        /// <returns>GetCurrentCallsFeature instance</returns>
        public static GetCurrentCallsFeature NewInstance()
        {
            return new GetCurrentCallsFeature();
        }

        #endregion ===========================================================================================

    }
}
