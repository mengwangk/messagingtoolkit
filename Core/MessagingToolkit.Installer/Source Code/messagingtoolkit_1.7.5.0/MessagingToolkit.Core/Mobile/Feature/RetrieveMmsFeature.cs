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
using System.Net;

using MessagingToolkit.Core.Mobile.Message;
using MessagingToolkit.Core.Mobile;
using MessagingToolkit.Core.Base;
using MessagingToolkit.MMS;
using MessagingToolkit.Wap;
using MessagingToolkit.Core.Helper;
using MessagingToolkit.Core.Log;
using MessagingToolkit.Core.Properties;

namespace MessagingToolkit.Core.Mobile.Feature
{

    /// <summary>
    /// Retrieve MMS feature
    /// </summary>
    internal class RetrieveMmsFeature : BaseFeature<RetrieveMmsFeature>, IFeature
    {

        #region ================ Private Variables ==================================================================



        #endregion ==================================================================================================


        #region ================ Private Constants ===================================================================

        /// <summary>
        /// When there is no port number mentioned for the WAP Gateway, use port 9201 for MMS 1.x versions 
        /// and port 8080 for MMS 2.x versions. MMS 2.0 is currently not supported
        /// </summary>
        private const int DefaultGatewayPort = 9201;


        /// <summary>
        /// WAP MMS content type
        /// </summary>
        private const string WapMmsContentType = "application/vnd.wap.mms-message";

        /// <summary>
        /// Content type header
        /// </summary>
        private const string HeaderContentType = "content-type";


        /// <summary>
        /// OK HTTP status
        /// </summary>
        private const int HttpStatusOk = 200;


        /// <summary>
        /// Message priority in queue
        /// </summary>
        private Dictionary<LogLevel, MessagingToolkit.Wap.Log.LogLevel> LogLevelMapping =
                          new Dictionary<LogLevel, MessagingToolkit.Wap.Log.LogLevel>
                            {
                                {LogLevel.Error, MessagingToolkit.Wap.Log.LogLevel.Error },
                                {LogLevel.Info, MessagingToolkit.Wap.Log.LogLevel.Info },
                                {LogLevel.Verbose, MessagingToolkit.Wap.Log.LogLevel.Verbose},
                                {LogLevel.Warn, MessagingToolkit.Wap.Log.LogLevel.Warn }
                            };

        /// <summary>
        /// Log name format mapping
        /// </summary>
        private Dictionary<LogNameFormat, MessagingToolkit.Wap.Log.LogNameFormat> LogNameFormatMapping =
                          new Dictionary<LogNameFormat, MessagingToolkit.Wap.Log.LogNameFormat>
                            {
                                {LogNameFormat.DateName, MessagingToolkit.Wap.Log.LogNameFormat.DateName },
                                {LogNameFormat.Name, MessagingToolkit.Wap.Log.LogNameFormat.Name },
                                {LogNameFormat.NameDate, MessagingToolkit.Wap.Log.LogNameFormat.NameDate }
                            };

        /// <summary>
        /// Log quota format mapping
        /// </summary>
        private Dictionary<LogQuotaFormat, MessagingToolkit.Wap.Log.LogQuotaFormat> LogQuotaFormatMapping =
                          new Dictionary<LogQuotaFormat, MessagingToolkit.Wap.Log.LogQuotaFormat>
                            {
                                {LogQuotaFormat.KbBytes, MessagingToolkit.Wap.Log.LogQuotaFormat.KbBytes },
                                {LogQuotaFormat.NoRestriction, MessagingToolkit.Wap.Log.LogQuotaFormat.NoRestriction },
                                {LogQuotaFormat.Rows, MessagingToolkit.Wap.Log.LogQuotaFormat.Rows}
                            };

        #endregion ==================================================================================================


        #region ====================== Constructor =================================================================

        /// <summary>
        /// Default constructor
        /// </summary>
        protected RetrieveMmsFeature()
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
            if (MessageInformation.GetType() == typeof(MmsNotification))
            {
                MmsNotification notification = (MmsNotification)MessageInformation;
                url = notification.ContentLocation;
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

                // Send the MMS
                WAPClient wapClient = new WAPClient(gatewayIP, gatewayPort);
                MessagingToolkit.Wap.Log.LogLevel loggingLevel = MessagingToolkit.Wap.Log.LogLevel.Error;
                wapClient.LogLevel = loggingLevel;
                wapClient.UserAgent = this.UserAgent;
                wapClient.XWAPProfile = this.XWAPProfile;
                if (LogLevelMapping.TryGetValue(Configuration.LogLevel, out loggingLevel))
                {
                    wapClient.LogLevel = loggingLevel;
                }

                MessagingToolkit.Wap.Log.LogNameFormat logNameFormat = MessagingToolkit.Wap.Log.LogNameFormat.NameDate;
                wapClient.LogNameFormat = logNameFormat;
                if (LogNameFormatMapping.TryGetValue(Configuration.LogNameFormat, out logNameFormat))
                {
                    wapClient.LogNameFormat = logNameFormat;
                }

                MessagingToolkit.Wap.Log.LogQuotaFormat logQuotaFormat = MessagingToolkit.Wap.Log.LogQuotaFormat.NoRestriction;
                wapClient.LogQuotaFormat = logQuotaFormat;
                if (LogQuotaFormatMapping.TryGetValue(Configuration.LogQuotaFormat, out logQuotaFormat))
                {
                    wapClient.LogQuotaFormat = logQuotaFormat;
                }

                wapClient.LogSizeMax = Configuration.LogSizeMax;

                if (this.ConnectionTimeout > 0)
                {
                    wapClient.ExecutionTimeOut = this.ConnectionTimeout;
                }
                

                try
                {
                    Logger.LogThis("Provider MMSC: " + Configuration.ProviderMMSC, LogLevel.Verbose);

                    GetRequest request = new GetRequest(url);
                    request.SetHeader(HeaderContentType, WapMmsContentType);
                    Logger.LogThis("Connecting to \"" + gatewayIP + "\":" + gatewayPort + "...", LogLevel.Verbose);
                    wapClient.Connect();

                    Logger.LogThis("Receiving MMS message through \"" + Configuration.ProviderMMSC, LogLevel.Verbose);

                    MessagingToolkit.Wap.Response response = wapClient.Execute(request);
                    byte[] binaryMms = response.ResponseBody;
                    wapClient.Disconnect();

                    MultimediaMessageDecoder decoder = new MultimediaMessageDecoder(binaryMms);
                    decoder.DecodeMessage();
                    MultimediaMessage message = decoder.GetMessage();

                    if (response.Success)
                    {
                        Mms mms = Mms.NewInstance(string.Empty, string.Empty);
                        GatewayHelper.SetFields<Mms, MultimediaMessage>(message, mms);
                        Logger.LogThis("Message received. Message id is " + message.MessageId, LogLevel.Verbose);
                        context.PutResult(mms);
                        this.Message = mms;
                    }
                    else
                    {
                        Logger.LogThis("Unable to retrieve MMS message", LogLevel.Error);
                        Logger.LogThis("Response status: " + response.Status, LogLevel.Error);
                        Logger.LogThis("Response text: " + response.StatusText, LogLevel.Error);
                    }
                    // Disconnect the connection
                    GatewayHelper.DisconnectGateway(this.RasEntryName);
                    isGatewayConnected = false;
                }
                finally
                {
                    try
                    {
                        // If connected then disconnect
                        if (wapClient.Connected)
                        {
                            wapClient.Disconnect();
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.LogThis(string.Format("Error in WAP client disconnect: {0}", ex.Message), LogLevel.Error);
                    }

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
            return "RetrieveMmsFeature: Receive MMS";
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

        #endregion =========================================================================================================


        #region =========== Protected Methods ==============================================================================



        #endregion =========================================================================================================


        #region =========== Public Static Methods ==========================================================================

        /// <summary>
        /// Return an instance of RetrieveMmsFeature
        /// </summary>
        /// <returns>RetrieveMmsFeature instance</returns>
        public static RetrieveMmsFeature NewInstance()
        {
            return new RetrieveMmsFeature();
        }

        #endregion =========================================================================================================
    }

}
