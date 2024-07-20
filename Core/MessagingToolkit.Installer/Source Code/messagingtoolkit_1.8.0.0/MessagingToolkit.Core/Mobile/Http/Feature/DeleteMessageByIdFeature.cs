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
using MessagingToolkit.Core.Helper;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace MessagingToolkit.Core.Mobile.Http.Feature
{
    /// <summary>
    /// Feature to delete messages from device.
    /// </summary>
    internal sealed class DeleteMessageByIdFeature : BaseHttpFeature<DeleteMessageByIdFeature>, IHttpFeature
    {
        private static String ParamId = "id";

        /// <summary>
        /// Prevents a default instance of the <see cref="DeleteMessageByIdFeature"/> class from being created.
        /// </summary>
        private DeleteMessageByIdFeature()
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
            if (this.Ids != null && this.Ids.Count > 0)
            {
                string url = BuildUrl();
                NameValueCollection nv = new NameValueCollection();
                string[] ids = this.Ids.ToArray();
                nv.Add(ParamId, string.Join(",", ids));
                DeleteMessageResponse response = RestServiceHelper.Delete<DeleteMessageResponse>(url, nv, this.UserName, this.Password);
                context.PutResult(response);
                return true;
            }

            // Still return true
            return true;
        }

        #region =========== Public Properties ===================================================================



        /// <summary>
        /// Gets or sets the ids.
        /// </summary>
        /// <value>
        /// The ids.
        /// </value>
        public List<string> Ids { get; set; }

        #endregion ===========================================================================================

        #region =========== Public Methods ===================================================================


        /// <summary>
        /// Return the feature description
        /// </summary>
        /// <returns>Feature description</returns>
        public override string ToString()
        {
            return "DeleteMessageByIdFeature: Delete messages from device";
        }

        #endregion ===========================================================================================

        #region =========== Public Static Methods ============================================================

        /// <summary>
        /// Return an instance of DeleteMessageByIdFeature.
        /// </summary>
        /// <returns>DeleteMessageByIdFeature instance.</returns>
        public static DeleteMessageByIdFeature NewInstance()
        {
            return new DeleteMessageByIdFeature();
        }

        #endregion ===========================================================================================
    }
}
