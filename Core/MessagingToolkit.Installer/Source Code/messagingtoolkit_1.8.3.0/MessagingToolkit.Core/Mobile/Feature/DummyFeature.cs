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
    /// This is a dummy feature for testing
    /// </summary>
    internal class DummyFeature : BaseFeature<EchoFeature>, IFeature
    {

        #region ====================== Constructor ====================

        /// <summary>
        /// Default constructor
        /// </summary>
        private DummyFeature(): base()
        {
            // This is a synchronous feature
            ExecutionBehavior = ExecutionBehavior.Synchronous;

            // Add the command
            AddCommand(Command.NewInstance(string.Empty, Response.Ok, Preprocess, DoNothing));


        }

        #endregion =====================================================

        #region =========== Private Method ==============================

        /// <summary>
        /// Set the initialization string
        /// </summary>
        /// <param name="context">Execution context</param>
        /// <param name="command">Command instance</param>
        /// <returns>Just return true</returns>
        private bool Preprocess(IContext context, ICommand command)
        {
            context.PutResult("dummy testing");      
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
            return "DummyFeature: For testing only";
        }

        #endregion ======================================================

        #region =========== Public Static Methods =======================

        /// <summary>
        /// Return an instance of DummyFeature
        /// </summary>
        /// <returns>DummyFeature instance</returns>
        public static DummyFeature NewInstance()
        {
            return new DummyFeature();
        }

        #endregion ======================================================

    }
}
