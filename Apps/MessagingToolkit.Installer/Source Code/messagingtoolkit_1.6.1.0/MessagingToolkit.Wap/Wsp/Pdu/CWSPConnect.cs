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
using System.IO;

using MessagingToolkit.Wap.Wtp;
using MessagingToolkit.Wap.Log;
using MessagingToolkit.Wap.Helper;
using MessagingToolkit.Wap.Wsp;


namespace MessagingToolkit.Wap.Wsp.Pdu
{
	public class CWSPConnect:CWSPPDU
	{
        private short version;

		virtual public short Version
		{
			get
			{
				return version;
			}
			
			set
			{
				this.version = value;
			}			
		}

        /// <summary>
        /// Initializes a new instance of the <see cref="CWSPConnect"/> class.
        /// </summary>
		public CWSPConnect():base()
		{
			version = (0x10);
			pduType = PduTypeConnect;
			capabilities.ClientSDUSize = 65535;
			capabilities.ServerSDUSize = 65535;
		}
		
		public override byte[] ToByteArray()
		{
			BitArrayOutputStream result = new BitArrayOutputStream();
			result.Write(pduType, 8);
			result.Write(version, 8);
			
			byte[] cap = capabilities.Bytes;
			byte[] head = null;
			if (headers != null)
			{
				head = headers.Bytes;
			}
			result.WriteUIntVar(cap.Length);
			result.WriteUIntVar(head == null?0:head.Length);
			result.Write(cap);
			if (head != null)
			{
				result.Write(head);
			}
			
			return result.ToByteArray();
		}
				
	}
}