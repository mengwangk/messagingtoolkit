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
namespace MessagingToolkit.Core.Ras
{
    using System;

    /// <summary>
    /// Defines the remote access service (RAS) device types.
    /// </summary>
    public static class RasDeviceType
    {
        /// <summary>
        /// A modem accessed through a COM port.
        /// </summary>
        public const string Modem = "modem";

        /// <summary>
        /// An ISDN card with a corresponding NDISWAN driver installed.
        /// </summary>
        public const string Isdn = "isdn";

        /// <summary>
        /// An X.25 card with a corresponding NDISWAN driver installed.
        /// </summary>
        public const string X25 = "x25";

        /// <summary>
        /// A virtual private network connection.
        /// </summary>
        public const string Vpn = "vpn";

        /// <summary>
        /// A packet assembler/disassembler.
        /// </summary>
        public const string Pad = "pad";

        /// <summary>
        /// Generic device type.
        /// </summary>
        public const string Generic = "GENERIC";

        /// <summary>
        /// Direct serial connection through a serial port.
        /// </summary>
        public const string Serial = "SERIAL";

        /// <summary>
        /// Frame Relay.
        /// </summary>
        public const string FrameRelay = "FRAMERELAY";

        /// <summary>
        /// Asynchronous Transfer Mode (ATM).
        /// </summary>
        public const string Atm = "ATM";

        /// <summary>
        /// Sonet device type.
        /// </summary>
        public const string Sonet = "SONET";

        /// <summary>
        /// Switched 56K access.
        /// </summary>
        public const string SW56 = "SW56";

        /// <summary>
        /// An Infrared Data Association (IrDA) compliant device.
        /// </summary>
        public const string Irda = "IRDA";

        /// <summary>
        /// Direct parallel connection through a parallel port.
        /// </summary>
        public const string Parallel = "PARALLEL";

#if (WINXP || WIN2K8 || WIN7)
        /// <summary>
        /// Point-to-Point Protocol over Ethernet.
        /// </summary>
        public const string PPPoE = "PPPoE";
#endif
    }
}