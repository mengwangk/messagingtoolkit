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
using System.Collections;
using System.Text.RegularExpressions;

namespace MessagingToolkit.Core.Mobile.Feature
{
    /// <summary>
    /// Find phone book entries
    /// </summary>
    internal sealed class FindPhoneBookEntriesFeature: BaseFeature<FindPhoneBookEntriesFeature>, IFeature
    {

        #region ================ Private Constants =========================================================

         /// <summary>
        /// Command to retrieve supported character sets
        /// </summary>
        private const string RequestCommand = "AT+CPBF=\"{0}\"";

        /// <summary>
        /// Expected response
        /// </summary>
        private const string ExpectedResponse = "+CPBF: ";

        /// <summary>
        /// Find pattern
        /// </summary>
        private const string FindPattern = "(\\d+),\"(.+)\",(\\d+),\"(.+)\".*\\r\\n";

        #endregion =========================================================================================


        #region ====================== Constructor =========================================================

        /// <summary>
        /// Default constructor
        /// </summary>
        private FindPhoneBookEntriesFeature(): base()
        {

            // This is a synchronous feature
            ExecutionBehavior = ExecutionBehavior.Synchronous;

            // Add the command
            AddCommand(Command.NewInstance(RequestCommand, Response.Ok, Preprocess, Postprocess));
        }


        #endregion ==========================================================================================


        #region =========== Public Properties=================================================================

        /// <summary>
        /// Text pattern to search
        /// </summary>
        /// <value>Text search pattern</value>
        public string FindText
        {
            get;
            set;
        }

        #endregion ===========================================================================================


        #region =========== Private Methods =================================================================

        /// <summary>
        /// Preprocess the command
        /// </summary>
        /// <param name="context">Execution context</param>
        /// <param name="command">Command instance</param>
        /// <returns>true if successful</returns>
        private bool Preprocess(IContext context, ICommand command)
        {
            command.Request = string.Format(RequestCommand, FindText);
            return true;
        }


        /// <summary>
        /// Parse the result
        /// </summary>
        /// <param name="context">Execution context</param>
        /// <param name="command">Command instance</param>
        /// <returns>true if successful</returns>
        private bool Postprocess(IContext context, ICommand command)
        { 
            ArrayList list = new ArrayList();
            Regex regex = new Regex(Regex.Escape(ExpectedResponse) + FindPattern);
            for (Match match = regex.Match(context.GetData()); match.Success; match = match.NextMatch())
            {
                int index = int.Parse(match.Groups[1].Value);
                string number = match.Groups[2].Value;
                int type = int.Parse(match.Groups[3].Value);
                string text = match.Groups[4].Value;
                list.Add(new PhoneBookEntry(index, number, type, text));
            }
            PhoneBookEntry[] phoneBookEntries = new PhoneBookEntry[list.Count];
            list.CopyTo(phoneBookEntries, 0);
            context.PutResult(phoneBookEntries);
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
            return "FindPhoneBookEntriesFeature: Search phone book entries";
        }

        #endregion ===========================================================================================

        #region =========== Public Static Methods ============================================================

        /// <summary>
        /// Return an instance of FindPhoneBookEntriesFeature
        /// </summary>
        /// <returns>FindPhoneBookEntriesFeature instance</returns>
        public static FindPhoneBookEntriesFeature NewInstance()
        {
            return new FindPhoneBookEntriesFeature();
        }

        #endregion ===========================================================================================
       
    }
}

