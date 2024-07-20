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
    /// Message indication support
    /// </summary>
    internal sealed class MessageIndicationSupport
    {
        /// <summary>
        /// Buffer settings
        /// </summary>
        private string buffer;

        /// <summary>
        /// CBM settings
        /// </summary>
        private string cellBroadcast;

        /// <summary>
        /// SMS deliver settings
        /// </summary>
        private string deliver;

        /// <summary>
        /// Indication mode
        /// </summary>
        private string mode;

        /// <summary>
        /// Status report settings
        /// </summary>
        private string statusReport;

        /// <summary>
        /// Constructor
        /// </summary>
        public MessageIndicationSupport()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="mode">Indication mode</param>
        /// <param name="deliver">SMS deliver settings</param>
        /// <param name="cellBroadcast">CBM settings</param>
        /// <param name="statusReport">Delivery status report settings</param>
        /// <param name="buffer">Buffer setting</param>
        public MessageIndicationSupport(string mode, string deliver, string cellBroadcast, string statusReport, string buffer)
        {
            this.mode = mode;
            this.deliver = deliver;
            this.cellBroadcast = cellBroadcast;
            this.statusReport = statusReport;
            this.buffer = buffer;
        }

        /// <summary>
        /// Check if the buffer setting is supported
        /// </summary>
        /// <param name="setting">Buffer mode</param>
        /// <returns>true if supported</returns>
        public bool SupportsBufferSetting(MessageBufferIndication setting)
        {
            return this.SupportsBufferSetting((int)setting);
        }

        /// <summary>
        /// Check buffer setting
        /// </summary>
        /// <param name="setting"></param>
        /// <returns></returns>
        public bool SupportsBufferSetting(int setting)
        {
            return ResultParser.ParseToList(this.buffer).Contains(setting);
        }

        /// <summary>
        /// Check CBM setting
        /// </summary>
        /// <param name="setting"></param>
        /// <returns></returns>
        public bool SupportsCellBroadcastSetting(CellBroadcastMessageIndication setting)
        {
            return this.SupportsCellBroadcastSetting((int)setting);
        }

        /// <summary>
        /// Check CBM setting
        /// </summary>
        /// <param name="setting"></param>
        /// <returns></returns>
        public bool SupportsCellBroadcastSetting(int setting)
        {
            return ResultParser.ParseToList(this.cellBroadcast).Contains(setting);
        }

        /// <summary>
        /// Check SMS deliver setting
        /// </summary>
        /// <param name="setting"></param>
        /// <returns></returns>
        public bool SupportsDeliverSetting(ReceivedMessageIndication setting)
        {
            return this.SupportsDeliverSetting((int)setting);
        }

        /// <summary>
        /// Check SMS deliver setting
        /// </summary>
        /// <param name="setting"></param>
        /// <returns></returns>
        public bool SupportsDeliverSetting(int setting)
        {
            return ResultParser.ParseToList(this.deliver).Contains(setting);
        }

        /// <summary>
        /// Check mode setting
        /// </summary>
        /// <param name="mode"></param>
        /// <returns></returns>
        public bool SupportsMode(MessageIndicationMode mode)
        {
            return this.SupportsMode((int)mode);
        }

        /// <summary>
        /// Check mode setting
        /// </summary>
        /// <param name="mode"></param>
        /// <returns></returns>
        public bool SupportsMode(int mode)
        {
            return ResultParser.ParseToList(this.mode).Contains(mode);
        }

        /// <summary>
        /// Check status report setting
        /// </summary>
        /// <param name="setting"></param>
        /// <returns></returns>
        public bool SupportsStatusReportSetting(StatusReportMessageIndication setting)
        {
            return this.SupportsStatusReportSettings((int)setting);
        }

        /// <summary>
        /// Check status report setting
        /// </summary>
        /// <param name="style"></param>
        /// <returns></returns>
        public bool SupportsStatusReportSettings(int style)
        { 
            return ResultParser.ParseToList(this.statusReport).Contains(style);
        }


        /// <summary>
        /// Buffer settings
        /// </summary>
        /// <value></value>
        public string BufferSettings
        {
            get
            {
                return this.buffer;
            }
        }

        /// <summary>
        /// CBM settings
        /// </summary>
        /// <value></value>
        public string CellBroadcastSettings
        {
            get
            {
                return this.cellBroadcast;
            }
        }

        /// <summary>
        /// SMS deliver settings
        /// </summary>
        /// <value></value>
        public string DeliverSettings
        {
            get
            {
                return this.deliver;
            }
        }

        /// <summary>
        /// Mode settings
        /// </summary>
        /// <value></value>
        public string ModeSettings
        {
            get
            {
                return this.mode;
            }
        }

        /// <summary>
        /// Status report settings
        /// </summary>
        /// <value></value>
        public string StatusReportSettings
        {
            get
            {
                return this.statusReport;
            }
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        override public string ToString()
        {
            string str = String.Empty;
            str = String.Concat(str, "BufferSettings = ", BufferSettings, "\r\n");
            str = String.Concat(str, "CellBroadcastSettings = ", CellBroadcastSettings, "\r\n");
            str = String.Concat(str, "DeliverSettings = ", DeliverSettings, "\r\n");
            str = String.Concat(str, "ModeSettings = ", ModeSettings, "\r\n");
            str = String.Concat(str, "StatusReportSettings = ", StatusReportSettings, "\r\n");
            return str;
        }
    }
}
