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
using System.IO.Ports;
using System.Xml.Serialization;

using MessagingToolkit.Core.Base;
using MessagingToolkit.Core.Mobile.Event;
using MessagingToolkit.Core.Log;

namespace MessagingToolkit.Core.Mobile
{
    /// <summary>
    /// Mobile gateway configuration class
    /// </summary>
    [System.Reflection.ObfuscationAttribute(Feature = "renaming", ApplyToMembers = true)]
    [Serializable]
    public class MobileGatewayConfiguration : BaseGatewayConfiguration, IConfiguration
    {


        #region =============== Private Constants ============================================================

        /// <summary>
        /// Gateway connected event
        /// </summary>
        public event ConnectedEventHandler GatewayConnected;


        #endregion ===========================================================================================


        #region =============== Constructor ==================================================================

        /// <summary>
        /// Private constructor
        /// </summary>
        private MobileGatewayConfiguration()
            : base()
        {
            // Set default values
            PortName = string.Empty;
            BaudRate = PortBaudRate.BitsPerSecond115200;
            Handshake = PortHandshake.None;
            Parity = PortParity.None;
            DataBits = PortDataBits.Eight;
            StopBits = PortStopBits.One;
            RtsEnable = true;
            DtrEnable = true;
            DataConnectionTimeout = 0;
            SafeConnect = false;
            SafeDisconnect = false;
        }


        #endregion ============================================================================================



        #region ============== Public Properties ==============================================================


        /// <summary>
        /// Communication port baud rate
        /// </summary>
        /// <value>PortBaudRate enum. See <see cref="PortBaudRate"/></value>
        [XmlAttribute]
        public PortBaudRate BaudRate
        {
            get;
            set;
        }

        /// <summary>
        /// Communication data bits
        /// </summary>
        /// <value>PortDataBits enum. See <see cref="PortDataBits"/></value>
        [XmlAttribute]
        public PortDataBits DataBits
        {
            get;
            set;
        }

        /// <summary>
        /// Communcation flow control
        /// </summary>
        /// <value>PortHandshake enum. See <see cref="PortHandshake"/></value>
        [XmlAttribute]
        public PortHandshake Handshake
        {
            get;
            set;
        }

        /// <summary>
        /// Communication port, e.g. COM1, COM2, COM3.
        /// </summary>
        /// <value>Port name</value>
        [XmlAttribute]
        public string PortName
        {
            get;
            set;
        }

        /// <summary>
        /// Port parity
        /// </summary>
        /// <value>PortParity enum. See <see cref="PortParity"/></value>
        [XmlAttribute]
        public PortParity Parity
        {
            get;
            set;
        }

        /// <summary>
        /// Port stop bits
        /// </summary>
        /// <value>PortStopBits enum. See <see cref="PortStopBits"/></value>
        [XmlAttribute]
        public PortStopBits StopBits
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether Ready To Send (RTS) signal is enabled during serial communication.
        /// </summary>
        /// <value>
        ///   <c>true</c> if RTS enabled; otherwise, <c>false</c>.
        /// </value>
        [XmlAttribute]
        public bool RtsEnable
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value that enables the Data Terminal Ready (DTR) signal during serial communication
        /// </summary>
        /// <value>
        ///   <c>true</c> if DTR is enabled; otherwise, <c>false</c>.
        /// </value>
        [XmlAttribute]
        public bool DtrEnable
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the data connection timeout. Default to 60 seconds
        /// </summary>
        /// <value>
        /// The data connection timeout.
        /// </value>
        [XmlAttribute]
        public int DataConnectionTimeout
        {
            get;
            set;
        }

        /// <summary>
        /// Fix the bug as described in http://zachsaw.blogspot.com/2010/07/net-serialport-woes.html
        /// </summary>
        /// <value>
        /// Flag indicating to fix the serial port before opening. Default to false
        /// </value>
        [XmlAttribute]
        public bool SafeConnect
        {
            get;
            set;
        }

        /// <summary>
        /// Handle scenario serial port base stream not closed when the device is unplug
        /// </summary>
        /// <value>
        /// Flag indicating to fix close the base stream when the device is unplug. Default to false
        /// </value>
        [XmlAttribute]
        public bool SafeDisconnect
        {
            get;
            set;
        }

        #endregion ============================================================================================



        #region ============== Protected method   ===============================================================
        /// <summary>
        /// Static factory to create the gateway configuration
        /// </summary>
        /// <returns>A new instance of gateway configuration</returns>
        internal void OnConnected(ConnectionEventArgs args)
        {
            if (this.GatewayConnected != null)
            {
                this.GatewayConnected.BeginInvoke(this, args, new AsyncCallback(this.AsyncCallback), null);
            }
        }

        #endregion ===========================================================================================


        /// <summary>
        /// Asynchronous callback method
        /// </summary>
        /// <param name="param">Result</param>
        private void AsyncCallback(IAsyncResult param)
        {
            System.Runtime.Remoting.Messaging.AsyncResult result = (System.Runtime.Remoting.Messaging.AsyncResult)param;
            if (result.AsyncDelegate is ConnectedEventHandler)
            {
                Logger.LogThis("Ending Async ConnectedEventHandler call", LogLevel.Verbose, this.GatewayId);
                ((ConnectedEventHandler)result.AsyncDelegate).EndInvoke(result);
            }
            else
            {
                Logger.LogThis("Warning: AsyncCallback got unknown delegate: " + result.AsyncDelegate.GetType().ToString(), LogLevel.Verbose, this.GatewayId);
            }
        }

        #region ============== Factory method   ===============================================================
        /// <summary>
        /// Static factory to create the gateway configuration
        /// </summary>
        /// <returns>A new instance of gateway configuration</returns>
        public static MobileGatewayConfiguration NewInstance()
        {
            return new MobileGatewayConfiguration();
        }

        #endregion ===========================================================================================

    }
}
