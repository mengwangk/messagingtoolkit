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

using MessagingToolkit.Wap.Wtp.Pdu;
using MessagingToolkit.Wap.Log;
using MessagingToolkit.Wap.Helper;
using MessagingToolkit.Wap.Wsp;


namespace MessagingToolkit.Wap.Wtp
{
    /// <summary>
    /// WTP transaction constants
    /// </summary>
    public class IWTPTransactionConstants
    {
        public readonly static byte ClassType0 = (0x00);
        public readonly static byte ClassType1 = (0x01);
        public readonly static byte ClassType2 = (0x02);
    }

    /// <summary>
    /// WTP transaction interface
    /// </summary>
    public interface IWTPTransaction
    {

        /// <summary>
        /// Gets or sets the type of the class.
        /// </summary>
        /// <value>The type of the class.</value>
        byte ClassType
        {
            get;

            set;
        }

        /// <summary>
        /// Gets the TID.
        /// </summary>
        /// <value>The TID.</value>
        int TID
        {
            get;

        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="IWTPTransaction"/> is aborted.
        /// </summary>
        /// <value><c>true</c> if aborted; otherwise, <c>false</c>.</value>
        bool Aborted
        {
            get;
        }

        /// <summary>
        /// Gets the abort code.
        /// </summary>
        /// <value>The abort code.</value>
        short AbortCode
        {
            get;

        }

        /// <summary>
        /// Processes the specified pdu.
        /// </summary>
        /// <param name="pdu">The pdu.</param>
        void Process(CWTPPDU pdu);

        /// <summary>
        /// Processes the specified event.
        /// </summary>
        /// <param name="wtpEvent">The WTP event.</param>
        void Process(CWTPEvent wtpEvent);

        /// <summary>
        /// Processes the specified e.
        /// </summary>
        /// <param name="exception">The exception.</param>
        void Process(EWTPCorruptPDUException exception);

        /// <summary>
        /// Aborts this instance.
        /// </summary>
        void Abort();

        /// <summary>
        /// Aborts the specified abort reason.
        /// </summary>
        /// <param name="abortReason">The abort reason.</param>
        void Abort(short abortReason);

        /// <summary>
        /// Closes the specified reason code.
        /// </summary>
        /// <param name="reasonCode">The reason code.</param>
        void Close(short reasonCode);
    }
}
