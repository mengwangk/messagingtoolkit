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
    /// </summary>
    internal sealed class WatchDogFeature : BaseFeature<WatchDogFeature>, IFeature
    {
        #region ====================== Constants =========================

        /// <summary>
        /// Command to check connection status
        /// </summary>
        private const string WatchDogCommand = "AT";

        #endregion =======================================================


        #region ====================== Constructor =======================

        /// <summary>
        /// Default constructor
        /// </summary>
        private WatchDogFeature(): base()
        {
            // This is a synchronous feature
            ExecutionBehavior = ExecutionBehavior.Synchronous;

            // Add the command
            Command command = Command.NewInstance(WatchDogCommand, Response.Ok, Preprocess, Postprocess);          
            AddCommand(command);
        }

        #endregion =======================================================

        #region =========== Public Properties ============================

       
        #endregion =======================================================

        #region =========== Private Method ===============================

        /// <summary>
        /// Set the initialization string
        /// </summary>
        /// <param name="context">Execution context</param>
        /// <param name="command">Command instance</param>
        /// <returns>Just return true</returns>
        private bool Preprocess(IContext context, ICommand command)
        {          
            return true;
        }

        /// <summary>
        /// Parse the response
        /// </summary>
        /// <param name="context"></param>
        /// <param name="command"></param>
        /// <returns></returns>
        private bool Postprocess(IContext context, ICommand command)
        {
            context.PutResult(context.GetData());
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
            return string.Format("WatchDogFeature: Send {0} to the gateway", WatchDogCommand);
        }

        #endregion ======================================================

        #region =========== Public Static Methods =======================

        /// <summary>
        /// Return an instance of WatchDogFeature
        /// </summary>
        /// <returns>WatchDogFeature instance</returns>
        public static WatchDogFeature NewInstance()
        {
            return new WatchDogFeature();
        }

        #endregion ======================================================
       
    }
}

