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
using System.Web;

using MessagingToolkit.Core.Helper;


namespace MessagingToolkit.Core.Mobile.Http.Feature
{
    /// <summary>
    /// Feature to retrieve messages from device.
    /// </summary>
    internal sealed class GetMessagesFeature : BaseHttpFeature<GetMessagesFeature>, IHttpFeature
    {

        /// <summary>
        /// Prevents a default instance of the <see cref="GetMessagesFeature"/> class from being created.
        /// </summary>
        private GetMessagesFeature()
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

            if (this.Query != null)
            {
                if (this.Query.Id >= 0)
                {
                    // query message with a particular id
                    if (!url.EndsWith("/")) url += "/";
                    url += this.Query.Id;
                }
                else
                {
                    UriBuilder uriBuilder = new UriBuilder(url);
                    var query = HttpUtility.ParseQueryString(uriBuilder.Query);
                    query[GetMessageQuery.ParameterTo] = this.Query.To;
                    query[GetMessageQuery.ParameterThreadId] = this.Query.ThreadId.ToString();
                    query[GetMessageQuery.ParameterDateSent] = this.Query.DateSent;
                    query[GetMessageQuery.ParameterFrom] = this.Query.From;
                    query[GetMessageQuery.ParameterPage] = this.Query.Page.ToString();
                    query[GetMessageQuery.ParameterPageSize] = this.Query.PageSize.ToString();
                    query[GetMessageQuery.ParameterStatus] = this.Query.Status.ToString();
                    uriBuilder.Query = query.ToString();
                    url = uriBuilder.ToString();
                    //url = AppendGuidtoUrl(url);
                }
            }
            GetMessagesResponse request = RestServiceHelper.Get<GetMessagesResponse>(url, this.UserName, this.Password);
            List<DeviceMessage> messages = request.Messages;
            context.PutResult(messages);
            return true;
        }

        #region =========== Public Properties ===================================================================

        /// <summary>
        /// Gets or sets the query the filter the messages.
        /// </summary>
        /// <value>
        /// The query.
        /// </value>
        public GetMessageQuery Query { get; set; }

        #endregion ===========================================================================================

        #region =========== Public Methods ===================================================================


        /// <summary>
        /// Return the feature description
        /// </summary>
        /// <returns>Feature description</returns>
        public override string ToString()
        {
            return "GetMessagesFeature: Get messages from device";
        }

        #endregion ===========================================================================================

        #region =========== Public Static Methods ============================================================

        /// <summary>
        /// Return an instance of GetMessagesFeature.
        /// </summary>
        /// <returns>GetMessagesFeature instance.</returns>
        public static GetMessagesFeature NewInstance()
        {
            return new GetMessagesFeature();
        }

        #endregion ===========================================================================================
    }
}
