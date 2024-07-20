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

namespace MessagingToolkit.Core.Mobile
{
    /// <summary>
    /// Class representing a GSM Unstructured Supplemental Service Data (USSD)
    /// network response.
    /// </summary>
    [Serializable]
    public class UssdRequest: UssdDatagram
    {
     
        /// <summary>
        /// Initializes a new instance of the <see cref="UssdRequest"/> class.
        /// </summary>
        public UssdRequest():base()
        {
            this.ResultPresentation = UssdResultPresentation.PresentationEnabled;
        }


        /// <summary>
        /// Gets or sets the result presentation.
        /// </summary>
        /// <value>The result presentation.</value>
        public UssdResultPresentation ResultPresentation 
        {
            get;
            set;
        }

	
        /// <summary>
        /// Initializes a new instance of the <see cref="UssdRequest"/> class.
        /// </summary>
        /// <param name="presentation">The presentation.</param>
        /// <param name="content">The content.</param>
        /// <param name="dcs">The DCS.</param>
        /// <param name="gatewayId">The gateway id.</param>
        public UssdRequest(UssdResultPresentation presentation, string content, UssdDcs dcs, string gatewayId): base()
        {
            this.ResultPresentation = presentation;
            this.Content = content;
            this.Dcs = dcs;
            this.GatewayId = gatewayId;
        }
	
	
        /// <summary>
        /// Initializes a new instance of the <see cref="UssdRequest"/> class.
        /// </summary>
        /// <param name="content">The content.</param>
        public UssdRequest(string content) : base()
        {
            this.ResultPresentation = UssdResultPresentation.PresentationEnabled;
            this.Content = content;
            this.Dcs = UssdDcs.Unspecified7Bit;
        }

	
        /// <summary>
        /// Gets the raw request.
        /// </summary>
        /// <value>The raw request.</value>
        public string RawRequest 
        {
            get 
            {
                StringBuilder buf = new StringBuilder();
		        buf.Append("AT+CUSD=");
		        buf.Append(ResultPresentation.Numeric);
		        buf.Append(",");
		        buf.Append("\"");
		        buf.Append(this.Content);
		        buf.Append("\"");
		        buf.Append(",");
		        buf.Append(this.Dcs.Numeric);
		        buf.Append("\r");
		        return buf.ToString();
            }
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
	    {
            StringBuilder buf = new StringBuilder("Gateway: ");
		    buf.Append(this.GatewayId);
            buf.Append("\n");
            buf.Append("Result presentation: ");
            buf.Append(this.ResultPresentation);
            buf.Append("\n");
            buf.Append("Data coding scheme: ");
            buf.Append(this.Dcs);
            buf.Append("\n");
            buf.Append("Content: ");
            buf.Append(!string.IsNullOrEmpty(this.Content) ? this.Content : "(EMPTY)");
		    return buf.ToString();
	    }
    }
}
