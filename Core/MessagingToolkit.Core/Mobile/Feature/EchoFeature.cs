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
    /// This feature set the echo mode of the gateway
    /// </summary>
    internal class EchoFeature : BaseFeature<EchoFeature>, IFeature
    {        
        #region ================ Private Constants ===========================================

        /// <summary>
        /// Command to retrieve the model
        /// </summary>
        private const string RequestCommand = "ATE{0}";


        #endregion ===========================================================================


        #region ====================== Constructor ===========================================

        /// <summary>
        /// Default constructor
        /// </summary>
        private EchoFeature(): base()
        {
            // This is a synchronous feature
            ExecutionBehavior = ExecutionBehavior.Synchronous;

            // Add the command
            AddCommand(Command.NewInstance(RequestCommand, Response.Ok, Preprocess, DoNothing));

            
        }

        #endregion ==========================================================================

        #region =========== Private Method ==================================================

        /// <summary>
        /// Set the initialization string
        /// </summary>
        /// <param name="context">Execution context</param>
        /// <param name="command">Command instance</param>
        /// <returns>Just return true</returns>
        private bool Preprocess(IContext context, ICommand command)
        {
            if (Echo)
            {
                // Set the command request to echo
                command.Request = String.Format(RequestCommand, "1");
            }
            else
            {
                // Set the command request not to echo
                command.Request = String.Format(RequestCommand, "0");
            }
            return true;
        }

        #endregion ==============================================================================

        #region =========== Public Properties ===================================================

        /// <summary>
        /// Echo mode property
        /// </summary>
        /// <value>Echo mode</value>
        public bool Echo
        {
            get;
            set;
        }

        #endregion =============================================================================


        #region =========== Public Methods =====================================================


        /// <summary>
        /// Return the feature description
        /// </summary>
        /// <returns>Feature description</returns>
        public override string ToString()
        {
            return "EchoFeature: Set the echo mode";
        }

        #endregion =============================================================================

        #region =========== Public Static Methods ==============================================

        /// <summary>
        /// Return an instance of EchoFeature
        /// </summary>
        /// <returns>EchoFeature instance</returns>
        public static EchoFeature NewInstance()
        {
            return new EchoFeature();
        }

        #endregion ============================================================================

    }
}
