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
using MessagingToolkit.Core.Properties;

namespace MessagingToolkit.Core.Mobile.Feature
{

    /// <summary>
    /// Retrieve MMS feature
    /// </summary>
    internal class RetrieveMmsV2Feature : BaseFeature<RetrieveMmsV2Feature>, IFeature
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
        protected RetrieveMmsV2Feature()
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
            if (MessageInformation == null) return false;
            if (Gateway == null) return false;

            // Extract URL            
            //string url = ExtractUrl(MessageInformation.Content);
            string url = string.Empty;
            string from = string.Empty;
            if (MessageInformation.GetType() == typeof(MmsNotification))
            {
                MmsNotification notification = (MmsNotification)MessageInformation;
                url = notification.ContentLocation;
                from = notification.From;
            }

            if (!string.IsNullOrEmpty(url))
            {
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
                    catch (Exception e)
                    {
                        gatewayPort = DefaultGatewayPort;
                    }
                }

                // Disconnect the gateway is it is connected
                bool isConnected = false;
                if (Gateway.Connected)
                {
                    isConnected = true;
                    Gateway.Disable();
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
                    if (!string.IsNullOrEmpty(this.XWAPProfile))
                    {
                        mmsConfig.UaProfileUrl = this.XWAPProfile;
                    }

                    // Not correct as from should not be used as the calling line id
                    //if (!string.IsNullOrEmpty(from))
                    //{
                    //    mmsConfig.CallingLineId = from;
                    //}

                    if (this.ConnectionTimeout != 0)
                    {
                        mmsConfig.HttpSocketTimeout = this.ConnectionTimeout;
                    }

                    mmsConfig.SupportHttpCharsetHeader = this.SupportHttpCharsetHeader;

                    bool isProxySet = !string.IsNullOrEmpty(gatewayIP);

                    Logger.LogThis("Receiving MMS message through " + url, LogLevel.Verbose);
                    byte[] binaryMms = mmsHttpClient.Execute(url, null, MmsHttpClient.MethodGet, isProxySet, gatewayIP, gatewayPort, mmsConfig, DefaultSubscriberId, LogPrefixMms);

                    MultimediaMessageDecoder decoder = new MultimediaMessageDecoder(binaryMms);
                    decoder.DecodeMessage();
                    MultimediaMessage message = decoder.GetMessage();

                    if (message != null)
                    {
                        Mms mms = Mms.NewInstance(string.Empty, string.Empty);
                        GatewayHelper.SetFields<Mms, MultimediaMessage>(message, mms);
                        Logger.LogThis("Message received. Message id is " + message.MessageId, LogLevel.Verbose);
                        context.PutResult(mms);
                        this.Message = mms;
                    }

                    // Disconnect the connection
                    GatewayHelper.DisconnectGateway(this.RasEntryName);
                    isGatewayConnected = false;
                }
                catch (Exception ex)
                {
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
            }
            else
            {
                // throw a exception
                throw new GatewayException(Resources.MmsRetrievalException);
            }

            // Reconnect to the Gateway if required
            //if (isConnected)
            //{
            //    Gateway.Connect();
            //}            

            return true;
        }

        /// <summary>
        /// Return the feature description
        /// </summary>
        /// <returns>Feature description</returns>
        public override string ToString()
        {
            return "RetrieveMmsV2Feature: Receive MMS";
        }

        #endregion ========================================================================================================


        #region =========== Private Methods ===============================================================================


        /// <summary>
        /// Extracts the URL.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <returns>The URL</returns>
        private string ExtractUrl(string content)
        {
            if (string.IsNullOrEmpty(content)) return string.Empty;

            string url = string.Empty;
            string[] values = content.Split(new string[] { "http" }, StringSplitOptions.None);
            if (values.Length > 0)
            {
                string temp = values[values.Length - 1];
                string[] temp1 = temp.Split(new char[] { '\0' });
                if (temp1.Length > 0)
                    url = "http" + temp1[0];
            }
            return url;
        }

        #endregion ========================================================================================================



        #region =========== Public Properties =============================================================================

        /// <summary>
        /// MMS property
        /// </summary>
        /// <value>MMS instance</value>
        public Mms Message
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the message information.
        /// </summary>
        /// <value>The message information.</value>
        public MessageInformation MessageInformation
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
        /// Return an instance of RetrieveMmsV2Feature
        /// </summary>
        /// <returns>RetrieveMmsV2Feature instance</returns>
        public static RetrieveMmsV2Feature NewInstance()
        {
            return new RetrieveMmsV2Feature();
        }

        #endregion =========================================================================================================
    }

}
