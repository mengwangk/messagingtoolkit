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
using System.Collections.Specialized;
using System.Linq;
using System.Text;

using MessagingToolkit.Core.Helper;

namespace MessagingToolkit.Core.Mobile.Http.Feature
{
    /// <summary>
    /// Send a message through HTTP POST method.
    /// </summary>
    internal sealed class PostMessageFeature : BaseHttpFeature<PostMessageFeature>, IHttpFeature
    {

        /// <summary>
        /// Prevents a default instance of the <see cref="PostMessageFeature"/> class from being created.
        /// </summary>
        private PostMessageFeature()
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
            // Message cannot be null
            if (this.Message == null) return false;

            // To number or contact cannot be empty
            if (string.IsNullOrEmpty(this.Message.To))
            {
                return false;
            }

            // Send the message  
            NameValueCollection nv = new NameValueCollection();
            nv.Add(PostMessage.ParamTo, this.Message.To);
            nv.Add(PostMessage.ParamMessage, this.Message.Message);
            nv.Add(PostMessage.ParamDeliveryReport, Convert.ToString(this.Message.DeliveryReport));
            nv.Add(PostMessage.ParamScAddress, this.Message.ScAddress);
            nv.Add(PostMessage.ParamSlot, this.Message.Slot);
            
            PostMessageResponse response = RestServiceHelper.Post<PostMessageResponse>(BuildUrl(), nv, this.UserName, this.Password);
            context.PutResult(response);
            return true;
        }

        #region =========== Public Properties ===================================================================

        /// <summary>
        /// Gets or sets the message to be posted.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        public PostMessage Message { get; set; }

        #endregion ===========================================================================================

        #region =========== Public Methods ===================================================================


        /// <summary>
        /// Return the feature description
        /// </summary>
        /// <returns>Feature description</returns>
        public override string ToString()
        {
            return "PostMessageFeature: Send a message through the device";
        }

        #endregion ===========================================================================================

        #region =========== Public Static Methods ============================================================

        /// <summary>
        /// Return an instance of PostMessageFeature.
        /// </summary>
        /// <returns>PostMessageFeature instance.</returns>
        public static PostMessageFeature NewInstance()
        {
            return new PostMessageFeature();
        }

        #endregion ===========================================================================================
    }
}
