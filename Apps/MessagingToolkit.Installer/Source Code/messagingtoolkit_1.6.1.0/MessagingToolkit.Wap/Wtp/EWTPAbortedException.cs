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
    /// WTP abort exception
    /// </summary>
	[Serializable]
	public class EWTPAbortedException:Exception
	{
        // abortCodes defined in CWTPAbort PDU
        private short abortCode;

        /// <summary>
        /// Gets the abort code.
        /// </summary>
        /// <value>The abort code.</value>
		virtual public short AbortCode
		{
			get
			{
				return abortCode;
			}			
		}

        /// <summary>
        /// Initializes a new instance of the <see cref="EWTPAbortedException"/> class.
        /// </summary>
        /// <param name="abortCode">The abort code.</param>
		public EWTPAbortedException(short abortCode):base("Transaction aborted! Code: " + abortCode)
		{
			this.abortCode = abortCode;
		}
	}
}