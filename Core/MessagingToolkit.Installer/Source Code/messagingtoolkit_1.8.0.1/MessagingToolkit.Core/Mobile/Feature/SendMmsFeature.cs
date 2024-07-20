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

namespace MessagingToolkit.Core.Mobile.Feature
{
  
    /// <summary>
    /// Send MMS feature
    /// </summary>
    internal class SendMmsFeature : BaseFeature<SendMmsFeature>, IFeature
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
        /// OK HTTP status
        /// </summary>
        private const int HttpStatusOk = 200;


        /// <summary>
        /// Log level mapping
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
        protected SendMmsFeature(): base()
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
            string[] temp = Configuration.ProviderWAPGateway.Split(new char[]{':'}, StringSplitOptions.None);
            int gatewayPort = DefaultGatewayPort;
            string gatewayIP = Configuration.ProviderWAPGateway;
            if (temp.Length > 1) 
            {
                try 
                {
                    gatewayPort = Convert.ToInt32(temp[temp.Length-1]);
                    gatewayIP = temp[0];
                } 
                catch (Exception e) 
                {
                    gatewayPort = DefaultGatewayPort;
                }
            }

            bool isGatewayConnected = true;

            // Connect to the MMSC and set the routing table
            GatewayHelper.ConnectGateway(RasEntryName, Configuration.ProviderAPNAccount, Configuration.ProviderAPNPassword, Configuration.ProviderWAPGateway);

            Logger.LogThis("Gateway IP: " + gatewayIP, LogLevel.Verbose);
            Logger.LogThis("Gateway port: " + gatewayPort, LogLevel.Verbose);

            MessagingToolkit.Wap.Log.LogLevel loggingLevel = MessagingToolkit.Wap.Log.LogLevel.Error;
            WAPClient.LogLevel = loggingLevel;
          
            if (LogLevelMapping.TryGetValue(Configuration.LogLevel, out loggingLevel))
            {
                WAPClient.LogLevel = loggingLevel;
            }

            MessagingToolkit.Wap.Log.LogNameFormat logNameFormat = MessagingToolkit.Wap.Log.LogNameFormat.NameDate;
            WAPClient.LogNameFormat = logNameFormat;
            if (LogNameFormatMapping.TryGetValue(Configuration.LogNameFormat, out logNameFormat))
            {
                WAPClient.LogNameFormat = logNameFormat;
            }

            MessagingToolkit.Wap.Log.LogQuotaFormat logQuotaFormat = MessagingToolkit.Wap.Log.LogQuotaFormat.NoRestriction;
            WAPClient.LogQuotaFormat = logQuotaFormat;
            if (LogQuotaFormatMapping.TryGetValue(Configuration.LogQuotaFormat, out logQuotaFormat))
            {
                WAPClient.LogQuotaFormat = logQuotaFormat;
            }

            WAPClient.LogSizeMax = Configuration.LogSizeMax;
            WAPClient.LogLocation = Configuration.LogLocation;
            WAPClient.LogFileName = Configuration.LogFile;

            // Send the MMS
            WAPClient wapClient = new WAPClient(gatewayIP, gatewayPort);
            wapClient.UserAgent = this.UserAgent;
            wapClient.XWAPProfile = this.XWAPProfile;
            
             try
             {
                 Logger.LogThis("Provider MMSC: " + Configuration.ProviderMMSC, LogLevel.Verbose);
                 PostRequest request = new PostRequest(Configuration.ProviderMMSC);
                 request.ContentType = WapMmsContentType;
                 request.RequestBody = encodedMms;
                 Logger.LogThis("Connecting to \"" + gatewayIP + "\":" + gatewayPort + "...", LogLevel.Verbose);
                 wapClient.Connect();

                 Logger.LogThis("Sending MMS message through \"" + Configuration.ProviderMMSC, LogLevel.Verbose);

                 if (this.ConnectionTimeout == 0)
                 {
                     // Assuming transfer at 1KB per second
                     int totalSizeKB = encodedMms.Length / 1024;
                     long estimatedTime = totalSizeKB * 1000;
                     if (estimatedTime > wapClient.ExecutionTimeOut)
                     {
                         wapClient.ExecutionTimeOut = estimatedTime;
                     }
                 }
                 else
                 {
                     wapClient.ExecutionTimeOut = this.ConnectionTimeout;
                 }

                 MessagingToolkit.Wap.Response response = wapClient.Execute(request);
                 byte[] binaryMms = response.ResponseBody;
                 wapClient.Disconnect();
                 wapClient = null;

                 MultimediaMessageDecoder decoder = new MultimediaMessageDecoder(binaryMms);
                 decoder.DecodeMessage();
                 MultimediaMessage message = decoder.GetMessage();

                 if (response.Success)
                 {
                     Logger.LogThis("Message sent. Message id is " + message.MessageId, LogLevel.Verbose);
                     Logger.LogThis("Response status: " + response.Status, LogLevel.Verbose);
                     Logger.LogThis("Response text: " + response.StatusText, LogLevel.Verbose);
                     context.PutResult(message.MessageId);
                     this.Message.MessageId = message.MessageId;
                 }
                 else
                 {
                     Logger.LogThis("Unable to send MMS message", LogLevel.Error);
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
                     if (wapClient != null && wapClient.Connected)
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
            return "SendMmsFeature: Send MMS";
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

        #endregion =========================================================================================================

        
        #region =========== Protected Methods ==============================================================================

       
      
        #endregion =========================================================================================================

        
        #region =========== Public Static Methods ==========================================================================

        /// <summary>
        /// Return an instance of SendMmsFeature
        /// </summary>
        /// <returns>SendMmsFeature instance</returns>
        public static SendMmsFeature NewInstance()
        {
            return new SendMmsFeature();
        }

        #endregion =========================================================================================================
    }
    
}
