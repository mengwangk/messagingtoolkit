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

namespace MessagingToolkit.Core.Mobile.Feature
{
    
    /// <summary>
    /// This feature set the gatewy memory location which can be
    /// - ME (mobile equipment)
    /// - SM (SIM)
    /// </summary>
    internal class MessageStorageFeature : BaseFeature<MessageStorageFeature>, IFeature
    {
        #region ================ Private Constants ====================

        /// <summary>
        /// Command to set the memory location
        /// </summary>
        private const string MemoryCommand1 = "AT+CPMS=\"{0}\",\"{0}\",\"{0}\"";

        /// <summary>
        /// Command to set the memory location
        /// </summary>
        private const string MemoryCommand2 = "AT+CPMS=\"{0}\",\"{0}\"";


        /// <summary>
        /// Command to set the memory location
        /// </summary>
        private const string MemoryCommand3 = "AT+CPMS=\"{0}\"";


        #endregion ====================================================


        #region ====================== Constructor ====================

        /// <summary>
        /// Default constructor
        /// </summary>
        private MessageStorageFeature(): base()
        {

            // This is a synchronous feature
            ExecutionBehavior = ExecutionBehavior.Synchronous;

            // Add the command
            AddCommand(Command.NewInstance(MemoryCommand1, Response.Ok, PreProcess, PostProcess, true));
            AddCommand(Command.NewInstance(MemoryCommand2, Response.Ok, PreProcess, PostProcess, true));
            AddCommand(Command.NewInstance(MemoryCommand3, Response.Ok, PreProcess, PostProcess));
        }


        #endregion =====================================================


        #region =========== Private Methods ============================

        /// <summary>
        /// Parse the result
        /// </summary>
        /// <param name="context">Execution context</param>
        /// <param name="command">Command instance</param>
        /// <returns>true if successful</returns>
        private bool PreProcess(IContext context, ICommand command)
        {
            // Set the memory location
            command.Request = String.Format(command.Request, StringEnum.GetStringValue(MessageStorage));
            return true;
        }

        /// <summary>
        /// Parse the result
        /// </summary>
        /// <param name="context">Execution context</param>
        /// <param name="command">Command instance</param>
        /// <returns>true if successful</returns>
        private bool PostProcess(IContext context, ICommand command)
        {
            string[] results = ResultParser.ParseResponse(context.GetData());
            context.PutResult(results[0]);
            StopExecution = true;
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
            return "MemoryLocationFeature: Set the gateway memory location";
        }

        #endregion ======================================================

        #region =========== Public Properties ============================

        /// <summary>
        /// Memory location
        /// </summary>
        /// <value>Memory location</value>
        public MessageStorage MessageStorage
        {
            get;
            set;
        }

        #endregion ======================================================


        #region =========== Public Static Methods =======================

        /// <summary>
        /// Return an instance of MemoryLocationFeature
        /// </summary>
        /// <returns>MemoryLocationFeature instance</returns>
        public static MessageStorageFeature NewInstance()
        {
            return new MessageStorageFeature();
        }

        #endregion ======================================================

    }
}
