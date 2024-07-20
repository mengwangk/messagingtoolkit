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

using MessagingToolkit.Core.Mobile.Message;
using MessagingToolkit.MMS;
using MessagingToolkit.Core.Helper;
using MessagingToolkit.Core.Log;
using MessagingToolkit.Core.Mm1;

namespace MessagingToolkit.Core.Mobile.Feature
{

    /// <summary>
    /// Send MMS feature
    /// </summary>
    internal class SendMmsV2Feature : BaseFeature<SendMmsV2Feature>, IFeature
    {

        #region ================ Private Variables ==================================================================



        #endregion ==================================================================================================


        #region ================ Private Constants ===================================================================

        /// <summary>
        /// When there is no port number mentioned for the WAP Gateway, use port 9201 for MMS 1.x versions 
        /// and port 8080 for MMS 2.x versions. MMS 2.0 is currently not supported
        /// </summary>
        private const int DefaultGatewayPort = 80;

        /// <summary>
        /// MMS log prefix
        /// </summary>
        private const string LogPrefixMms = "MMS HTTP Client";

        /// <summary>
        /// Default subscriber id
        /// </summary>
        private const int DefaultSubscriberId = 168168;

        #endregion ==================================================================================================


        #region ====================== Constructor =================================================================

        /// <summary>
        /// Default constructor
        /// </summary>
        protected SendMmsV2Feature()
            : base()
        {
            // Data type command
            CommandType = FeatureCommandType.Data;
        }


        #endregion ====================================================================================================


        #region =========== Public Methods =============================================================================


        /// <summary>
        /// Send the SMS
        /// </summary>
        /// <param name="context">Execution context</param>
        /// <returns>true if successful</returns>
        /// <exception cref="GatewayException">GatewayException is thrown when there is error in execution</exception>
        public override bool Execute(IContext context)
        {
            if (Message == null) return false;
            if (Gateway == null) return false;

            // Translate the MMS slides to actual content           
            Message.ComposeContentFromSlide();

            // Encode the message to be sent out
            MultimediaMessageEncoder encoder = new MultimediaMessageEncoder();
            encoder.SetMessage(Message);
            encoder.EncodeMessage();
            byte[] encodedMms = encoder.GetMessage();

            // Disconnect the gateway is it is connected
            bool isConnected = false;
            if (Gateway.Connected)
            {
                isConnected = true;
                Gateway.Disable();
            }

            // Proceed to send MMS
            string[] temp = Configuration.ProviderWAPGateway.Split(new char[] { ':' }, StringSplitOptions.None);
            int gatewayPort = DefaultGatewayPort;
            string gatewayIP = Configuration.ProviderWAPGateway;
            if (temp.Length > 1)
            {
                try
                {
                    gatewayPort = Convert.ToInt32(temp[temp.Length - 1]);
                    gatewayIP = temp[0];
                }
                catch (Exception)
                {
                    gatewayPort = DefaultGatewayPort;
                }
            }

            bool isGatewayConnected = true;

            // Connect to the MMSC and set the routing table
            GatewayHelper.ConnectGateway(RasEntryName, Configuration.ProviderAPNAccount, Configuration.ProviderAPNPassword, Configuration.ProviderWAPGateway);

            Logger.LogThis("Gateway IP: " + gatewayIP, LogLevel.Verbose);
            Logger.LogThis("Gateway port: " + gatewayPort, LogLevel.Verbose);
            
            try
            {
                // Send the MMS
                Logger.LogThis("Provider MMSC: " + Configuration.ProviderMMSC, LogLevel.Verbose);
                MmsHttpClient mmsHttpClient = new MmsHttpClient();

                // MMS configuration
                MmsConfig mmsConfig = new MmsConfig
                {
                    // Set APN user name and password
                    ApnUser = Configuration.ProviderAPNAccount,
                    ApnPassword = Configuration.ProviderAPNPassword
                };

                // User agent
                if (!string.IsNullOrEmpty(this.UserAgent))
                {
                    mmsConfig.UserAgent = this.UserAgent;
                }

                // User profile URL
                if (!string.IsNullOrEmpty(this.XWAPProfile)) {
                    mmsConfig.UaProfileUrl = this.XWAPProfile;
                }

                // Calling line id
                if (!string.IsNullOrEmpty(Message.GetFrom().GetAddress()))
                {
                    mmsConfig.CallingLineId = Message.GetFrom().GetAddress();
                }

                if (this.ConnectionTimeout != 0)
                {
                    mmsConfig.HttpSocketTimeout = this.ConnectionTimeout;
                }

                mmsConfig.SupportHttpCharsetHeader = this.SupportHttpCharsetHeader;

                bool isProxySet = !string.IsNullOrEmpty(gatewayIP);

                Logger.LogThis("Sending MMS message through \"" + Configuration.ProviderMMSC, LogLevel.Verbose);
                byte[] binaryMms = mmsHttpClient.Execute(Configuration.ProviderMMSC, encodedMms, MmsHttpClient.MethodPost, isProxySet, gatewayIP, gatewayPort, mmsConfig, DefaultSubscriberId, LogPrefixMms);

                MultimediaMessageDecoder decoder = new MultimediaMessageDecoder(binaryMms);
                decoder.DecodeMessage();
                MultimediaMessage message = decoder.GetMessage();
                Logger.LogThis("Message sent. Message id is " + message.MessageId, LogLevel.Verbose);
                context.PutResult(message.MessageId);
                this.Message.MessageId = message.MessageId;

                // Disconnect the connection
                GatewayHelper.DisconnectGateway(this.RasEntryName);
                isGatewayConnected = false;
            }
            catch (Exception ex)
            {
                //Logger.LogThis("Unable to send MMS message", LogLevel.Error);
                //Logger.LogThis("Error message: " + ex.Message, LogLevel.Error);
                //Logger.LogThis(ex.StackTrace, LogLevel.Error);
                throw ex;
            }
            finally
            {
                try
                {
                    if (isGatewayConnected)
                    {
                        // Disconnect the connection
                        GatewayHelper.DisconnectGateway(this.RasEntryName);
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogThis(string.Format("Error in gateway disconnect: {0}", ex.Message), LogLevel.Error);
                }
            }
            return true;
        }



        /// <summary>
        /// Return the feature description
        /// </summary>
        /// <returns>Feature description</returns>
        public override string ToString()
        {
            return "SendMmsV2Feature: Send MMS";
        }

        #endregion ========================================================================================================


        #region =========== Private Methods ===============================================================================



        #endregion ========================================================================================================



        #region =========== Public Properties =============================================================================

        /// <summary>
        /// MMS property
        /// </summary>
        /// <value>MMS instance</value>
        public Mms Message
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the configuration.
        /// </summary>
        /// <value>The configuration.</value>
        public MobileGatewayConfiguration Configuration
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the gateway.
        /// </summary>
        /// <value>The gateway.</value>
        public IMobileGateway Gateway
        {
            get;
            set;
        }


        /// <summary>
        /// Gets or sets the name of the RAS entry.
        /// </summary>
        /// <value>The name of the RAS entry.</value>
        public string RasEntryName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the user agent.
        /// </summary>
        /// <value>
        /// The user agent.
        /// </value>
        public string UserAgent
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the XWAP profile.
        /// </summary>
        /// <value>
        /// The XWAP profile.
        /// </value>
        public string XWAPProfile
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the connection timeout.
        /// </summary>
        /// <value>
        /// The connection timeout.
        /// </value>
        public int ConnectionTimeout
        {
            get;
            set;
        }

        /// <summary>
        /// Flag indicating whether charset should be added to Content-Type header for MMS sending and receiving.
        /// </summary>
        public bool SupportHttpCharsetHeader
        {
            get;
            set;
        }

        #endregion =========================================================================================================


        #region =========== Protected Methods ==============================================================================



        #endregion =========================================================================================================


        #region =========== Public Static Methods ==========================================================================

        /// <summary>
        /// Return an instance of SendMmsV2Feature
        /// </summary>
        /// <returns>SendMmsV2Feature instance</returns>
        public static SendMmsV2Feature NewInstance()
        {
            return new SendMmsV2Feature();
        }

        #endregion =========================================================================================================
    }

}
