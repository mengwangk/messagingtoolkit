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

using MessagingToolkit.Core.Log;

namespace MessagingToolkit.Core.Mobile
{
    /// <summary>
    /// USSD response
    /// </summary>
    [Serializable]
    public class UssdResponse : UssdDatagram, IIndicationObject
    {
        public const string Pattern = "^\\+CUSD:\\s+(\\d)(?:,\\s*\"([^\"]*))?(?:\",\\s*(\\d+)\\s*)?\"?\r?$";
                                      
        private const int StatusIndex = 1;
        private const int ContentIndex = 2;
        private const int EncodingIndex = 3;


        /// <summary>
        /// Class representing GSM USSD network response.
        /// </summary>
        /// <value>The raw response.</value>
        public string RawResponse
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the session status.
        /// </summary>
        /// <value>The session status.</value>
        public UssdSessionStatus SessionStatus
        {
            get;
            set;
        }

       
        /// <summary>
        /// Initializes a new instance of the <see cref="UssdResponse"/> class.
        /// </summary>
        public UssdResponse()
            : base()
        {
            RawResponse = string.Empty;
            GatewayId = string.Empty;
            Content = string.Empty;            
            SessionStatus = null;
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="UssdResponse"/> class.
        /// </summary>
        /// <param name="rawResponse">The raw response.</param>
        /// <param name="gatewayId">The gateway id.</param>
        public UssdResponse(string rawResponse, string gatewayId)
        {           
		    Match matcher = Regex.Match(rawResponse, Pattern);
            if (!matcher.Success)
            {
                Logger.LogThis("Not a well-formed +CUSD response: |" + rawResponse + "|", LogLevel.Error);
                throw new Exception("Not a well-formed +CUSD response: |" + rawResponse + "|");
            }
		    try
		    {
                this.GatewayId = gatewayId;
                this.RawResponse = rawResponse;
                this.SessionStatus = UssdSessionStatus.GetByNumeric(Convert.ToInt32(matcher.Groups[StatusIndex].Value));
			     if (matcher.Groups.Count >= ContentIndex && !string.IsNullOrEmpty(matcher.Groups[ContentIndex].Value))
			    {
                    this.Content = matcher.Groups[ContentIndex].Value;				   
			    }
			    if (matcher.Groups.Count >= EncodingIndex && !string.IsNullOrEmpty(matcher.Groups[EncodingIndex].Value))
			    {
                    this.Dcs = UssdDcs.GetByNumeric(Convert.ToInt32(matcher.Groups[EncodingIndex].Value));				   
			    }
		    }
		    catch (Exception e)
		    {
			    throw new Exception("Session status: " + matcher.Groups[StatusIndex] + "; DCS: " + matcher.Groups[EncodingIndex] + ": " + e.Message);
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
            buf.Append("Session status: ");
            buf.Append(SessionStatus);
            buf.Append("\n");
            buf.Append("Data coding scheme: ");
            buf.Append(this.Dcs != null ? this.Dcs.ToString() : "Unspecified");
            buf.Append("\n");
            buf.Append("Content: ");
            buf.Append(this.Content != null ? this.Content : "(EMPTY)");
            return buf.ToString();
        }
    }
}
