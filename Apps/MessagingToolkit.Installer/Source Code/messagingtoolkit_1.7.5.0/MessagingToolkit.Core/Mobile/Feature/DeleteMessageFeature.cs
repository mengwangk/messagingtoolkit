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
    /// Delete message from the gateway
    /// </summary>
    internal class DeleteMessageFeature : BaseFeature<DeleteMessageFeature>, IFeature
    {
        #region ================ Private Constants ====================

        /// <summary>
        /// Command to retrieve the service center address
        /// </summary>
        private const string DeleteByIndexCommand = "AT+CMGD={0}";


        private const string DeleteByMessageStoreCommand = "AT+CMGD=1,{0}";
        


        #endregion ====================================================


        #region ====================== Constructor ====================

        /// <summary>
        /// Default constructor
        /// </summary>
        private DeleteMessageFeature(): base()
        {

            // This is a synchronous feature
            ExecutionBehavior = ExecutionBehavior.Synchronous;

            // Add the command
            AddCommand(Command.NewInstance(DeleteByIndexCommand, Response.Ok, PreProcess, PostProcess));
        }


        #endregion =====================================================


        #region =========== Private Methods ============================

        /// <summary>
        /// Set the type of messages to delete
        /// </summary>
        /// <param name="context">Execution context</param>
        /// <param name="command">Command instance</param>
        /// <returns>true if successful</returns>
        private bool PreProcess(IContext context, ICommand command)
        {
            if (MessageDeleteOption == MessageDeleteOption.ByIndex)
            {
                command.Request = string.Format(DeleteByIndexCommand, MessageIndex);
            }
            else
            {
                command.Request = string.Format(DeleteByMessageStoreCommand, StringEnum.GetStringValue(MessageDeleteOption));
            }            
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
            return "DeleteMessageFeature: Delete message from the gateway";
        }

        #endregion ======================================================


        #region =========== Public Properties ============================

        /// <summary>
        /// Message deletion option. See <see cref="MessageDeleteOption"/>
        /// </summary>
        /// <value>Messsage deletion option</value>
        public MessageDeleteOption MessageDeleteOption
        {
            get;
            set;
        }

        /// <summary>
        /// Message index
        /// </summary>
        /// <value>Message index</value>
        public int MessageIndex
        {
            get;
            set;
        }

        #endregion ======================================================



        #region =========== Public Static Methods =======================

        /// <summary>
        /// Return an instance of DeleteMessageFeature
        /// </summary>
        /// <returns>DeleteMessageFeature instance</returns>
        public static DeleteMessageFeature NewInstance()
        {
            return new DeleteMessageFeature();
        }

        #endregion ======================================================
       
    }
}


