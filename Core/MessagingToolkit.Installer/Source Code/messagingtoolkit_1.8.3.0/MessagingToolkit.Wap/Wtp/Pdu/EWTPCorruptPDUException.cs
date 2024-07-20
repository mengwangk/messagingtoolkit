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

using MessagingToolkit.Wap.Wtp;
using MessagingToolkit.Wap.Log;
using MessagingToolkit.Wap.Helper;
using MessagingToolkit.Wap.Wsp;


namespace MessagingToolkit.Wap.Wtp.Pdu
{

    /// <summary>
    /// This Exception is thrown by the WTP state machine, if it received
    /// a PDU, which can not be decoded - perhaps because it IS NO WTP PDU...
    /// </summary>
	[Serializable]
	public class EWTPCorruptPDUException:Exception
	{
		virtual public bool TidAvailable
		{
			get
			{
				return tidAvailable;
			}
			
		}
		virtual public int TID
		{
			get
			{
				return tid;
			}
			
		}
		private int tid;
		private bool tidAvailable = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="EWTPCorruptPDUException"/> class.
        /// </summary>
        /// <param name="s">An error message</param>
		public EWTPCorruptPDUException(string s):base(s)
		{
		}

        /// <summary>
        /// Initializes a new instance of the <see cref="EWTPCorruptPDUException"/> class.
        /// </summary>
        /// <param name="s">The s.</param>
        /// <param name="tid">The tid.</param>
		public EWTPCorruptPDUException(string s, int tid):base(s)
		{
			this.tid = tid;
			this.tidAvailable = true;
		}
	}
}