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

namespace MessagingToolkit.Core.Mobile.Feature
{

    /// <summary>
    /// Set GPRS attach mode
    /// </summary>
    internal class SetGprsAttachFeature : BaseFeature<SetGprsAttachFeature>, IFeature
    {

        #region ================ Private Constants =========================================================

        /// <summary>
        /// Set GPRS attach mode
        /// </summary>
        private const string SetGprsAttachCommand = "AT+CGATT=1";

        #endregion =========================================================================================


        #region ====================== Constructor =========================================================

        /// <summary>
        /// Default constructor
        /// </summary>
        private SetGprsAttachFeature(): base()
        {

            // This is a synchronous feature
            ExecutionBehavior = ExecutionBehavior.Synchronous;

            AddCommand(Command.NewInstance(SetGprsAttachCommand, Response.Ok, DoNothing, DoNothing, true));
        }


        #endregion ==========================================================================================


        #region =========== Private Methods =================================================================




        #endregion ===========================================================================================

        #region =========== Public Properties ===============================================================


        #endregion ==========================================================================================



        #region =========== Public Methods ===================================================================


        /// <summary>
        /// Return the feature description
        /// </summary>
        /// <returns>Feature description</returns>
        public override string ToString()
        {
            return "SetGprsAttachFeature: Set GPRS attach mode";
        }

        #endregion ===========================================================================================

        #region =========== Public Static Methods ============================================================

        /// <summary>
        /// Return an instance of SetGprsAttachFeature
        /// </summary>
        /// <returns>SetGprsAttachFeature instance</returns>
        public static SetGprsAttachFeature NewInstance()
        {
            return new SetGprsAttachFeature();
        }

        #endregion ===========================================================================================

    }
}
