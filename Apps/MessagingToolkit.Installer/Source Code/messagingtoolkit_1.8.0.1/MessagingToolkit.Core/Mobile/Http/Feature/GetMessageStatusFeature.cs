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
using MessagingToolkit.Core.Properties;

namespace MessagingToolkit.Core.Mobile.Http.Feature
{
    /// <summary>
    /// Feature to retrieve message status from device.
    /// </summary>
    internal sealed class GetMessageStatusFeature : BaseHttpFeature<GetMessageStatusFeature>, IHttpFeature
    {

        /// <summary>
        /// Prevents a default instance of the <see cref="GetMessageStatusFeature"/> class from being created.
        /// </summary>
        private GetMessageStatusFeature()
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
            string url = BuildUrl();
            if (!string.IsNullOrEmpty(this.Id))
            {
                    // query message with a particular id
                    if (!url.EndsWith("/")) url += "/";
                    url += this.Id;
            }
            else
            {
                throw new GatewayException(Resources.GetMessageStatusException);
            }

            GetMessageStatusResponse response = RestServiceHelper.Get<GetMessageStatusResponse>(url, this.UserName, this.Password);
            MessageStatusInformation statusInfo = response.MessageStatusInformation;
            statusInfo.Status = response.Status;
            statusInfo.ErrorDescription = response.Description;
            context.PutResult(statusInfo);
            return true;
        }

        #region =========== Public Properties ===================================================================


        /// <summary>
        /// Gets or sets the message id.
        /// </summary>
        /// <value>
        /// The message id.
        /// </value>
        public string Id { get; set; }

        #endregion ===========================================================================================

        #region =========== Public Methods ===================================================================


        /// <summary>
        /// Return the feature description
        /// </summary>
        /// <returns>Feature description</returns>
        public override string ToString()
        {
            return "GetMessageStatusFeature: Get message status from device";
        }

        #endregion ===========================================================================================

        #region =========== Public Static Methods ============================================================

        /// <summary>
        /// Return an instance of GetMessageStatusFeature.
        /// </summary>
        /// <returns>GetMessageStatusFeature instance.</returns>
        public static GetMessageStatusFeature NewInstance()
        {
            return new GetMessageStatusFeature();
        }

        #endregion ===========================================================================================
    }
}
