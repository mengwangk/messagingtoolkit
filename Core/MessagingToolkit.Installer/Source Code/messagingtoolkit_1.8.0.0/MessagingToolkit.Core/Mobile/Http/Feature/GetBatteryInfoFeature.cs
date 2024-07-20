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
using MessagingToolkit.Core.Helper;

namespace MessagingToolkit.Core.Mobile.Http.Feature
{
    /// <summary>
    /// Feature to retrieve battery information.
    /// </summary>
    internal sealed class GetBatteryInfoFeature : BaseHttpFeature<GetBatteryInfoFeature>, IHttpFeature
    {

        /// <summary>
        /// Prevents a default instance of the <see cref="GetBatteryInfoFeature"/> class from being created.
        /// </summary>
        /// <summary>
        /// Prevents a default instance of the <see cref="GetBatteryInfoFeature"/> class from being created.
        /// </summary>
        private GetBatteryInfoFeature()
            : base()
        {
        }

        /// <summary>
        /// Execute the feature.
        /// </summary>
        /// <param name="context">Execution context.</param>
        /// <returns>
        /// true if successful and execution continue, false if do not want to continue.
        /// </returns>
        public bool Execute(IContext context)
        {
            GetBatteryInfoResponse batteryInfo = RestServiceHelper.Get<GetBatteryInfoResponse>(BuildUrl(), this.UserName, this.Password);
            context.PutResult(batteryInfo.DeviceBatteryInformation);
            return true;
        }

        #region =========== Public Methods ===================================================================


        /// <summary>
        /// Return the feature description
        /// </summary>
        /// <returns>Feature description</returns>
        public override string ToString()
        {
            return "GetBatteryInfoFeature: Get device battery information";
        }

        #endregion ===========================================================================================

        #region =========== Public Static Methods ============================================================

        /// <summary>
        /// Return an instance of GetBatteryInfoFeature.
        /// </summary>
        /// <returns>GetBatteryInfoFeature instance.</returns>
        public static GetBatteryInfoFeature NewInstance()
        {
            return new GetBatteryInfoFeature();
        }

        #endregion ===========================================================================================
    }
}
