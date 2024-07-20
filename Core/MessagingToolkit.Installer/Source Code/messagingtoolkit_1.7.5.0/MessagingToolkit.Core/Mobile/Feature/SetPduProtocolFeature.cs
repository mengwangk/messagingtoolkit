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
    /// Set gateway message protocol
    /// </summary>
    internal sealed class SetPduProtocolFeature : BaseFeature<SetPduProtocolFeature>, IFeature
    {

        #region ================ Private Constants =============================================

        /// <summary>
        /// Command to set PDU mode
        /// </summary>
        private const string PduCommand = "AT+CMGF=0";

        #endregion ==============================================================================

        #region ====================== Constructor ==============================================

        /// <summary>
        /// Default constructor
        /// </summary>
        private SetPduProtocolFeature(): base()
        {
            // This is a synchronous feature
            ExecutionBehavior = ExecutionBehavior.Synchronous;

            // Add the command
            Command command = Command.NewInstance(PduCommand, Response.Ok, DoNothing, Postprocess);           
            AddCommand(command);
        }

        #endregion ===============================================================================

        #region =========== Private Method =======================================================

        /// <summary>
        /// Parse the response
        /// </summary>
        /// <param name="context">Execution context</param>
        /// <param name="command">Command</param>
        /// <returns></returns>
        private bool Postprocess(IContext context, ICommand command)
        {
            context.PutResult(context.GetData());
            return true;
        }

       
        #endregion ==============================================================================

        #region =========== Public Methods ======================================================


        /// <summary>
        /// Return the feature description
        /// </summary>
        /// <returns>Feature description</returns>
        public override string ToString()
        {
            return String.Format("SetPduProtocolFeature: Set PDU mode");
        }

        #endregion =============================================================================

        #region =========== Public Static Methods ==============================================

        /// <summary>
        /// Return an instance of SetPduProtocolFeature
        /// </summary>
        /// <returns>SetPduProtocolFeature instance</returns>
        public static SetPduProtocolFeature NewInstance()
        {
            return new SetPduProtocolFeature();
        }

        #endregion =============================================================================
       
    }
}

