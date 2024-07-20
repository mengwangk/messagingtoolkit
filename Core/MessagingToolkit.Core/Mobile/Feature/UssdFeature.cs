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
using System.Threading;

using MessagingToolkit.Core.Properties;
using MessagingToolkit.Pdu;

namespace MessagingToolkit.Core.Mobile.Feature
{
    /// <summary>
    /// Sending USSD command to the network
    /// USSD stands for Unstrcutured Supplementary Services Data.
    /// It is a way of sending short commands from the mobile phone to the GSM network.
    /// It uses, like SMS, the signalling channel of the GSM connection.
    /// Unlike SMS, it does not use a store and forward architecture, but a session oriented connection.
    /// USSD text messages can be up to 182 bytes in length. Messages received on the mobile phone are not stored.
    ///
    /// USSD is defined within the GSM standard in the documents:
    ///
    /// GSM 02.90 (USSD Stage 1) 
    /// GSM 03.90 (USSD Stage 2) 
    ///
    /// USSD is most used to make it easy for the (prepaid) mobile user to query his prepaid balance using his mobile phone.
    /// It can also be used in mobile payments systems and information services such as weather forecasts and traffic information.
    /// </summary>
    internal sealed class UssdFeature : BaseFeature<UssdFeature>, IFeature
    {
        #region ================ Private Constants ================================================================================

        /// <summary>
        /// Command to send the USSD command
        /// </summary>
        private const string RequestCommand = "AT+CUSD=1,\"{0}\""; 
          
        /// <summary>
        /// Expected response
        /// </summary>
        private const string ExpectedSynchronousResponse = "+CUSD";

       
        #endregion ================================================================================================================


        #region ====================== Constructor ================================================================================

        /// <summary>
        /// Default constructor
        /// </summary>
        private UssdFeature(): base()
        {
            // This is a synchronous feature
            ExecutionBehavior = ExecutionBehavior.Synchronous;
           
            // Add the command
            AddCommand(Command.NewInstance(RequestCommand, Response.Ok, Preprocess, Postprocess));

            this.Content = string.Empty;
            this.Request = null;
            this.Interactive = false;
            this.Encoded = false;
        }


        #endregion ===============================================================================================================

        #region =========== Public Properties ====================================================================================

        /// <summary>
        /// Request command to be sent to gateway
        /// </summary>
        /// <value>Command string</value>
        public string Content
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="UssdFeature"/> is interactive.
        /// </summary>
        /// <value><c>true</c> if interactive; otherwise, <c>false</c>.</value>
        public bool Interactive
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the request.
        /// </summary>
        /// <value>The request.</value>
        public UssdRequest Request
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the USSD response queue.
        /// </summary>
        /// <value>The USSD response queue.</value>
        public UssdResponseQueue UssdResponseQueue
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether [enable ussd event].
        /// </summary>
        /// <value><c>true</c> if [enable ussd event]; otherwise, <c>false</c>.</value>
        public bool EnableUssdEvent
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="UssdFeature"/> is encoded.
        /// </summary>
        /// <value>
        ///   <c>true</c> if encoded; otherwise, <c>false</c>.
        /// </value>
        public bool Encoded
        {
            get;
            set;
        }

        #endregion ===============================================================================================================

        #region =========== Private Methods ======================================================================================

        /// <summary>
        /// Parse the result
        /// </summary>
        /// <param name="context">Execution context</param>
        /// <param name="command">Command instance</param>
        /// <returns>true if successful</returns>
        private bool Preprocess(IContext context, ICommand command)
        {
            string ussdCommand = string.Empty;
            if (!string.IsNullOrEmpty(this.Content))
            {
                ussdCommand = FormatUssdCommand(this.Content);
            }
            else if (this.Request != null)
            {
                ussdCommand = FormatUssdCommand(Convert.ToString(Request.ResultPresentation.Numeric), Request.Content, Convert.ToString(Request.Dcs.Numeric));
            }
            command.Request = ussdCommand;    
            return true;
        }


        /// <summary>
        /// Parse the result to get the service center address
        /// </summary>
        /// <param name="context">Execution context</param>
        /// <param name="command">Command instance</param>
        /// <returns>true if successful</returns>
        private bool Postprocess(IContext context, ICommand command)
        {
            if (this.EnableUssdEvent)
            {
                context.PutResult(StringEnum.GetStringValue(Response.Ok));
            }
            else
            {
                // Get the response from the USSD response queue
                // Maximum wait time is 60 seconds
                int count = 0;
                int waitInterval = 1000;
                while (true)
                {
                    if (UssdResponseQueue.Count() > 0)
                    {
                        UssdResponse ussdResponse = UssdResponseQueue.Remove();
                        context.PutResult(ussdResponse);
                        break;
                    }
                    count++;
                    if (count > 60)
                    {
                        throw new GatewayException(string.Format(Resources.UssdResponseException, command.Request));
                    }
                    Thread.Sleep(waitInterval);
                }
            }

            if (!this.Interactive)
            {                
                char esc = Convert.ToChar(0x1b);
                string c = esc + "\r";
                byte[] b = new ASCIIEncoding().GetBytes(c);
                Port.Write(b, 0, b.Length);
                //this.Port.Write(c);
                this.Port.DiscardInBuffer();
                this.Port.DiscardOutBuffer();
            }

            /*
            string[] results = ResultParser.ParseResponse(context.GetData());
            string ussdResponse = string.Empty;
            if (results.Length > 0 && results[0].Contains(ExpectedSynchronousResponse))
            {
                string[] cols = results[0].Split(new string[] { ":", "," }, StringSplitOptions.None);
                if (cols.Length > 1)
                {                    
                    ussdResponse = cols[2];
                    ussdResponse = ussdResponse.Replace("\"", "").Trim();                              
                }
            }
            context.PutResult(ussdResponse);
            */
            return true;
        }

        /// <summary>
        /// Formats the USSD command.
        /// </summary>
        /// <param name="ussdCommand">The ussd command.</param>
        /// <returns></returns>
        private String FormatUssdCommand(string ussdCommand)
        {
            return FormatUssdCommand("1", ussdCommand, null);
        }

        /// <summary>
        /// Formats the USSD command.
        /// </summary>
        /// <param name="presentation">The presentation.</param>
        /// <param name="content">The content.</param>
        /// <param name="dcs">The DCS.</param>
        /// <returns></returns>
        private String FormatUssdCommand(string presentation, string content, string dcs)
        {
            if (!this.Encoded)
            {
                return FormatCommand(presentation, content, dcs);
            }
            else
            {
                byte[] textSeptets = PduUtils.StringToUnencodedSeptets(content);
                byte[] alphaNumBytes = PduUtils.UnencodedSeptetsToEncodedSeptets(textSeptets);
                String ussdCommandEncoded = PduUtils.BytesToPdu(alphaNumBytes);
                return FormatCommand(presentation, ussdCommandEncoded, "15");
            }
        }

         /// <summary>
         /// Formats the command.
         /// </summary>
         /// <param name="presentation">The presentation.</param>
         /// <param name="content">The content.</param>
         /// <param name="dcs">The DCS.</param>
         /// <returns></returns>
        private String FormatCommand(string presentation, string content, string dcs)
        {
            StringBuilder buf = new StringBuilder();
            buf.Append("AT+CUSD=");
            buf.Append(presentation);
            buf.Append(",");
            buf.Append("\"");
            buf.Append(content);
            buf.Append("\"");
            if (!string.IsNullOrEmpty(dcs))
            {
                buf.Append(",");
                buf.Append(dcs);
            }
            //buf.Append("\r");
            return buf.ToString();
        }

        /// <summary>
        /// Formats the USSD response.
        /// </summary>
        /// <param name="ussdResponse">The ussd response.</param>
        /// <returns></returns>
        private string FormatUSSDResponse(string ussdResponse)
        {
            if (!this.Encoded)
            {
                // noop for most modems but some may require additional processing e.g. pdu decode
                return ussdResponse;
            }
            else
            {
                byte[] responseEncodedSeptets = PduUtils.PduToBytes(ussdResponse);
                byte[] responseUnencodedSeptets = PduUtils.EncodedSeptetsToUnencodedSeptets(responseEncodedSeptets);
                return PduUtils.UnencodedSeptetsToString(responseUnencodedSeptets);
            }
        }


        #endregion =====================================================================================================================

        #region =========== Public Methods =============================================================================================

       
        /// <summary>
        /// Return the feature description
        /// </summary>
        /// <returns>Feature description</returns>
        public override string ToString()
        {
            return "UssdFeature: Send USSD command to the network";
        }

        #endregion =====================================================================================================================

        #region =========== Public Static Methods ======================================================================================

        /// <summary>
        /// Return an instance of UssdFeature
        /// </summary>
        /// <returns>UssdFeature instance</returns>
        public static UssdFeature NewInstance()
        {
            return new UssdFeature();
        }

        #endregion =====================================================================================================================
       
    }
}

