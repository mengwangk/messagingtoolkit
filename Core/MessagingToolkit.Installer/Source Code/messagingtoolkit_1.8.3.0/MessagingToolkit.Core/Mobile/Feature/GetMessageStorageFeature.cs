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
using System.Collections;

namespace MessagingToolkit.Core.Mobile.Feature
{
    /// <summary>
    /// Get message storages
    /// </summary>
    internal sealed class GetMessageStorageFeature: BaseFeature<GetMessageStorageFeature>, IFeature
    {

        #region ================ Private Constants =========================================================

         /// <summary>
        /// Command to retrieve battery charging information
        /// </summary>
        private const string RequestCommand = "AT+CPMS=?";

        /// <summary>
        /// Expected response
        /// </summary>
        private const string ExpectedResponse = "+CPMS: ";

        /// <summary>
        /// Storage pattern to look for
        /// </summary>
        private const string StoragePattern = "\\((?:\"(\\w+)\"(?(?!\\)),))+\\)";

        #endregion =========================================================================================


        #region ====================== Constructor =========================================================

        /// <summary>
        /// Default constructor
        /// </summary>
        private GetMessageStorageFeature() : base()
        {

            // This is a synchronous feature
            ExecutionBehavior = ExecutionBehavior.Synchronous;

            // Add the command
            AddCommand(Command.NewInstance(RequestCommand, Response.Ok, DoNothing, Postprocess));
        }


        #endregion ==========================================================================================


        #region =========== Private Methods =================================================================

        /// <summary>
        /// Parse the result
        /// </summary>
        /// <param name="context">Execution context</param>
        /// <param name="command">Command instance</param>
        /// <returns>true if successful</returns>
        private bool Postprocess(IContext context, ICommand command)
        {            
            string[] results = ResultParser.ParseResponse(context.GetData());
            ArrayList list = new ArrayList();
            if (results.Length > 0 && results[0].Contains(ExpectedResponse))
            {
                string input = results[0];
                int index = input.IndexOf(ExpectedResponse);
                if (index >= 0)
                {
                    input = input.Substring(index + ExpectedResponse.Length);
                    Match match = new Regex(StoragePattern).Match(input);
                    if (match.Success)
                    {
                        do
                        {
                            int matchCount = match.Groups[1].Captures.Count;
                            string[] matchList = new string[matchCount];
                            for (int i = 0; i < matchCount; i++)
                            {
                                matchList[i] = match.Groups[1].Captures[i].Value;
                            }
                            list.Add(matchList);
                            match = match.NextMatch();
                        } while (match.Success);
                    }

                    MessageStorageInfo messageStorageInfo = new MessageStorageInfo(); 
                    int count = list.Count;
                    messageStorageInfo.ReadStorages = (string[])list[0];

                    if (count > 1)
                    {
                        messageStorageInfo.WriteStorages = (string[])list[1];
                    }
                    else
                    {
                        messageStorageInfo.WriteStorages = new string[0];
                    }
                    if (count > 2)
                    {
                        messageStorageInfo.ReceiveStorages = (string[])list[2];
                    }
                    else
                    {
                        messageStorageInfo.ReceiveStorages = new string[0];
                    }
                    context.PutResult(messageStorageInfo);
                    return true;
                }                    
            }
            MessageStorageInfo emptyStorageInfo = new MessageStorageInfo();
            emptyStorageInfo.ReadStorages = new string[0];
            emptyStorageInfo.WriteStorages = new string[0];
            emptyStorageInfo.ReceiveStorages = new string[0];
            context.PutResult(emptyStorageInfo);
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
            return "GetMessageStorageFeature: Get message storage information";
        }

        #endregion ===========================================================================================

        #region =========== Public Static Methods ============================================================

        /// <summary>
        /// Return an instance of GetMessageStorageFeature
        /// </summary>
        /// <returns>GetMessageStorageFeature instance</returns>
        public static GetMessageStorageFeature NewInstance()
        {
            return new GetMessageStorageFeature();
        }

        #endregion ===========================================================================================
       
    }
}